using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DryIoc.MefAttributedModel;
using FuwaTea.Extensibility.Attributes;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ExtensionAttribute = FuwaTea.Extensibility.Attributes.ExtensionAttribute;

namespace FuwaTea.Extensibility
{
    /// <summary>
    /// A realized instance of an extension. Safe to be (de)serialized.
    /// </summary>
    public class Extension
    {
        private static readonly string MyDirPath =
            Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        /// <summary>
        /// Empty constructor for deserialization.
        /// </summary>
        public Extension()
        {

        }

        /// <summary>
        /// Constructor for DLL files.
        /// </summary>
        /// <param name="dllFile">The path to the DLL file.</param>
        /// <param name="isRelative">Whether the specified path is relative or absolute.</param>
        public Extension([NotNull] string dllFile, bool isRelative)
        {
            if (isRelative)
            {
                RelativeFilePath = dllFile;
                FilePath = BaseUtils.MakeAbsolutePath(MyDirPath, dllFile);
            }
            else
            {
                FilePath = dllFile;
                RelativeFilePath = BaseUtils.MakeRelativePath(MyDirPath, dllFile);
            }
            LastWriteTimeUtc = new FileInfo(dllFile).LastWriteTimeUtc;
            AssemblyName = AssemblyName.GetAssemblyName(dllFile);
        }

        /// <summary>
        /// Constructor for named assemblies.
        /// </summary>
        /// <param name="assemblyName">The assembly identifier.</param>
        public Extension([NotNull] AssemblyName assemblyName)
        {
            AssemblyName = assemblyName;
        }

        /// <summary>
        /// Constructor for pre-loaded assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public Extension([NotNull] Assembly assembly)
        {
            AssemblyName = assembly.GetName();
            Assembly = assembly;
            AssemblyLoadResult = AssemblyLoadResult.OK;
        }

        /// <summary>
        /// Load the extension's assembly and exports, if required. All necessary checks will be performed.
        /// </summary>
        /// <param name="overrideApiVersionWhitelist">Optionally override the API version whitelist check.</param>
        /// <exception cref="NullReferenceException">If <see cref="AssemblyName"/> is null.</exception>
        public void Load(bool overrideApiVersionWhitelist = false)
        {
            // Check if the file has changed
            // First, expand the relative path if needed, as only the relative path is saved to the cache
            if (RelativeFilePath != null && FilePath == null)
                FilePath = BaseUtils.MakeAbsolutePath(MyDirPath, RelativeFilePath);
            // If there is no path, assume it has changed
            var fileChanged = FilePath != null;
            if (fileChanged)
            {
                var lastWrite = new FileInfo(FilePath).LastWriteTimeUtc;
                fileChanged = lastWrite != LastWriteTimeUtc;
                // Update last write time
                LastWriteTimeUtc = lastWrite;
            }
            
            // Different flow for unchanged / never loaded files
            // Aggressive inlining will be used to reduce number of method calls at runtime

            // If changed, or the assembly was never loaded, or the assembly loaded successfully but the extension was never checked:
            if (fileChanged || AssemblyLoadResult == AssemblyLoadResult.NotLoaded
                            || AssemblyLoadResult == AssemblyLoadResult.OK && ExtensionCheckResult == ExtensionCheckResult.NotLoaded)
            {
                // Everything must be reloaded. Let's reset the properties so nobody gets confused:
                AssemblyLoadResult = AssemblyLoadResult.NotLoaded;
                Key = null;
                ApiVersion = null;
                PlatformFilter = null;
                ExtensionCheckResult = ExtensionCheckResult.NotLoaded;
                Exports = null;
                BasicInfo = null;

                // Load assembly first
                LoadAssembly(true);

                // Return immediately if failed
                if (AssemblyLoadResult != AssemblyLoadResult.OK || Assembly == null) return;

                // Load extension attributes
                if (!LoadExtDef()) return;

                // Check API version
                if (!CheckApiVersion(overrideApiVersionWhitelist)) return;

                // Check platform
                PlatformFilter = Assembly.GetCustomAttribute<PlatformFilterAttribute>();
                if (!CheckPlatform()) return;

                // Done checking
                ExtensionCheckResult = ExtensionCheckResult.OK;

            }
            // If not changed, and we can trust the existing data:
            else
            {
                // Do not load the assembly if we are sure that it isn't an extension
                if (ExtensionCheckResult == ExtensionCheckResult.NotAnExtension) return;

                // Check API version
                // Must already be present
                if (!CheckApiVersion(overrideApiVersionWhitelist)) return;

                // Check platform filter only if it exists
                if (PlatformFilter != null && !CheckPlatform()) return;

                // Load assembly
                LoadAssembly(false);

                // Return immediately if failed
                if (AssemblyLoadResult != AssemblyLoadResult.OK || Assembly == null) return;

                // Done checking
                ExtensionCheckResult = ExtensionCheckResult.OK;
            }
            
            // Load exports if they aren't already:
            if (Exports == null) LoadExports();
            // Set IsLoaded
            IsLoaded = true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool LoadExtDef()
        {
            var extDef = Assembly.GetCustomAttribute<ExtensionAttribute>();
            if (extDef == null)
            {
                ExtensionCheckResult = ExtensionCheckResult.NotAnExtension;
                return false;
            }

            Key = extDef.Key;
            ApiVersion = extDef.ApiVersion;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadAssembly(bool changed)
        {
            // Load the assembly if needed - if file has changed, or if loading hasn't been attempted, or if file wasn't found last time
            if (Assembly == null && (changed || AssemblyLoadResult == AssemblyLoadResult.NotLoaded
                                             || AssemblyLoadResult == AssemblyLoadResult.FileNotFound))
            {
                AssemblyLoadResult = AssemblyName.TryLoadAssembly(out var a);
                Assembly = a;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckApiVersion(bool overrideApiVersionWhitelist)
        {
            if (BaseUtils.CheckApiVersion(ApiVersion ?? 0, overrideApiVersionWhitelist)) return true;
            ExtensionCheckResult = ExtensionCheckResult.ApiVersionMismatch;
            return false;

        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckPlatform()
        {
            if (PlatformFilter == null) return true;

            // First, check the arch
            if (PlatformFilter.Action == FilterAction.Whitelist
                ^ PlatformFilter.ProcessArchitecture.AppliesTo(RuntimeInformation.ProcessArchitecture))
            {
                ExtensionCheckResult = ExtensionCheckResult.ProcessArchMismatch;
                return false;
            }

            // Next, the OS
            if (PlatformFilter.Action == FilterAction.Whitelist
                ^ RuntimeInformation.IsOSPlatform(PlatformFilter.OSKind.ToOSPlatform()))
            {
                ExtensionCheckResult = ExtensionCheckResult.OSKindMismatch;
                return false;
            }

            // Finally, the OS version
            if (PlatformFilter.Action == FilterAction.Whitelist ^ PlatformFilter.OSVersionMatches())
            {
                ExtensionCheckResult = ExtensionCheckResult.OSVersionMismatch;
                return false;
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadExports()
        {
            Exports = AttributedModel.Scan(new[] { Assembly }).ToList();
        }

        // Metadata properties:
        
        /// <summary>
        /// The relative path to the DLL file.
        /// </summary>
        [CanBeNull]
        public string RelativeFilePath { get; set; }

        /// <summary>
        /// The time at which the DLL file was last modified, in universal coordinated time (UTC).
        /// </summary>
        [CanBeNull]
        public DateTime? LastWriteTimeUtc { get; set; }

        // Extension properties:

        /// <summary>
        /// The assembly identifier.
        /// </summary>
        public AssemblyName AssemblyName { get; set; }

        /// <summary>
        /// The result of loading the assembly.
        /// </summary>
        public AssemblyLoadResult AssemblyLoadResult { get; set; }

        /// <summary>
        /// The extension's unique identifier.
        /// </summary>
        [CanBeNull]
        public string Key { get; set; }

        /// <summary>
        /// The API version targeted by the extension.
        /// </summary>
        [CanBeNull]
        public int? ApiVersion { get; set; }

        /// <summary>
        /// The platform filter requested by the extension (if specified).
        /// </summary>
        [CanBeNull]
        public IPlatformFilter PlatformFilter { get; set; }

        /// <summary>
        /// The result of checking the extension metadata.
        /// </summary>
        public ExtensionCheckResult ExtensionCheckResult { get; set; }

        /// <summary>
        /// The exported registrations from the extension.
        /// </summary>
        [CanBeNull]
        public IList<ExportedRegistrationInfo> Exports { get; set; }

        /// <summary>
        /// The basic information exported by the extension.
        /// </summary>
        [CanBeNull]
        public IExtensionBasicInfo BasicInfo { get; set; }

        // Non-serialized properties:
        
        /// <summary>
        /// The absolute path to the DLL file.
        /// </summary>
        [CanBeNull, JsonIgnore]
        public string FilePath { get; set; }

        /// <summary>
        /// The assembly instance. Will be null if loading has failed.
        /// </summary>
        [CanBeNull, JsonIgnore]
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets whether the extension is fully loaded successfully.
        /// </summary>
        [JsonIgnore]
        public bool IsLoaded { get; private set; }
    }
}
