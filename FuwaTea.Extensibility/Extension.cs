using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DryIoc.MefAttributedModel;
using FuwaTea.Extensibility.Attributes;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace FuwaTea.Extensibility
{
    /// <summary>
    /// A realized instance of an extension. Safe to be (de)serialized.
    /// </summary>
    public class Extension
    {
        /// <summary>
        /// Empty constructor for deserialization.
        /// </summary>
        public Extension()
        {

        }

        /// <summary>
        /// Constructor for DLL files.
        /// </summary>
        /// <param name="dllFile">The DLL file.</param>
        public Extension([NotNull] string dllFile)
        {
            LastWriteTimeUtc = new FileInfo(dllFile).LastWriteTimeUtc;
            FilePath = dllFile;
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
            // First, load the assembly if needed - Return immediately if failed
            if (Assembly == null)
            {
                AssemblyLoadResult = AssemblyName.TryLoadAssembly(out var a);
                Assembly = a;
                if (AssemblyLoadResult != AssemblyLoadResult.OK || Assembly == null) return;
            }

            // Check if the file has changed
            // If there is no path, assume it has changed
            var fileChanged = FilePath != null;
            if (fileChanged)
            {
                var lastWrite = new FileInfo(FilePath).LastWriteTimeUtc;
                fileChanged = lastWrite != LastWriteTimeUtc;
                // Update last write time
                LastWriteTimeUtc = lastWrite;
            }

            // Load extension attributes
            // Only if file has changed, or they are missing
            if (fileChanged || string.IsNullOrWhiteSpace(Key) || ApiVersion == null)
            {
                var extDef = Assembly.GetCustomAttribute<ExtensionAttribute>();
                if (extDef == null)
                {
                    ExtensionCheckResult = ExtensionCheckResult.NotAnExtension;
                    return;
                }

                Key = extDef.Key;
                ApiVersion = extDef.ApiVersion;
            }
            
            // Check ApiVersion
            // Always check
            if (!Utils.CheckApiVersion(ApiVersion.Value, overrideApiVersionWhitelist))
            {
                ExtensionCheckResult = ExtensionCheckResult.ApiVersionMismatch;
                return;
            }

            // Check platform
            // Always check, but only get attribute if file changed, or if needed
            if (fileChanged || PlatformFilter == null)
                PlatformFilter = Assembly.GetCustomAttribute<PlatformFilterAttribute>();
            if (PlatformFilter != null)
            {
                // First, check the arch
                if (PlatformFilter.Action == FilterAction.Whitelist
                    ^ PlatformFilter.ProcessArchitecture.AppliesTo(RuntimeInformation.ProcessArchitecture))
                {
                    ExtensionCheckResult = ExtensionCheckResult.ProcessArchMismatch;
                    return;
                }

                // Next, the OS
                if (PlatformFilter.Action == FilterAction.Whitelist
                    ^ RuntimeInformation.IsOSPlatform(PlatformFilter.OSKind.ToOSPlatform()))
                {
                    ExtensionCheckResult = ExtensionCheckResult.OSKindMismatch;
                    return;
                }

                // Finally, the OS version
                if (PlatformFilter.Action == FilterAction.Whitelist ^ PlatformFilter.OSVersionMatches())
                {
                    ExtensionCheckResult = ExtensionCheckResult.OSVersionMismatch;
                    return;
                }
            }
            ExtensionCheckResult = ExtensionCheckResult.OK;
            
            // Read exports
            // Only if file has changed or if needed (null or empty)
            if (fileChanged || Exports == null || Exports.Count < 1)
                Exports = AttributedModel.Scan(new[] { Assembly }).ToList();

            // Set IsLoaded
            IsLoaded = true;
        }

        // Metadata properties:

        /// <summary>
        /// The path to the DLL file.
        /// </summary>
        [CanBeNull]
        public string FilePath { get; set; }

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
