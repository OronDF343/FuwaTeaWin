﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using DryIoc.MefAttributedModel;
using Sage.Extensibility.Attributes;
using Serilog;
using ExtensionAttribute = Sage.Extensibility.Attributes.ExtensionAttribute;

namespace Sage.Extensibility
{
    /// <summary>
    /// A realized instance of an extension. Safe to be (de)serialized.
    /// </summary>
    public class Extension
    {
        private static readonly string MyDirPath = AppContext.BaseDirectory;

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
        public Extension(string dllFile, bool isRelative)
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
        public Extension(AssemblyName assemblyName)
        {
            AssemblyName = assemblyName;
        }

        /// <summary>
        /// Constructor for pre-loaded assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public Extension(Assembly assembly)
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
            var log = Log.ForContext(typeof(Extension));
            log.Debug($"Beginning loading, override = {overrideApiVersionWhitelist}");
            // Check if the file has changed
            // First, expand the relative path if needed, as only the relative path is saved to the cache
            if (RelativeFilePath != null && FilePath == null)
            {
                FilePath = BaseUtils.MakeAbsolutePath(MyDirPath, RelativeFilePath);
                log.Debug($"Expanded relative path \"{RelativeFilePath}\" to \"{FilePath}\"");
            }
            // If there is no path, assume it has changed
            var fileChanged = FilePath == null;
            if (!fileChanged)
            {
                log.Debug("Extension has file path, checking LastWriteTimeUtc");
                var lastWrite = new FileInfo(FilePath).LastWriteTimeUtc;
                fileChanged = lastWrite != LastWriteTimeUtc;
                log.Debug($"File changed: {fileChanged}");
                // Update last write time
                LastWriteTimeUtc = lastWrite;
            }
            else log.Debug("Extension has no file path");

            // Different flow for unchanged / never loaded files
            // Aggressive inlining will be used to reduce number of method calls at runtime

            // If changed, or the assembly was never loaded, or the assembly loaded successfully but the extension was never checked:
            if (fileChanged || AssemblyLoadResult == AssemblyLoadResult.NotLoaded
                            || AssemblyLoadResult == AssemblyLoadResult.OK && ExtensionCheckResult == ExtensionCheckResult.NotLoaded)
            {
                log.Debug("Full reload required");
                // Everything must be reloaded. Let's reset the properties so nobody gets confused:
                AssemblyLoadResult = AssemblyLoadResult.NotLoaded;
                Key = null;
                ApiVersion = null;
                PlatformFilter = null;
                ExtensionCheckResult = ExtensionCheckResult.NotLoaded;
                Exports = null;
                BasicInfo = null;

                // Load assembly first
                if (Assembly == null)
                {
                    log.Debug("Assembly load required");
                    LoadAssembly();
                }

                // Return immediately if failed
                if (AssemblyLoadResult != AssemblyLoadResult.OK || Assembly == null)
                {
                    log.Debug("Failed to load assembly");
                    return;
                }

                // Load extension attributes
                if (!LoadExtDef())
                {
                    log.Debug("Not an extension");
                    return;
                }

                // Check API version
                if (!CheckApiVersion(overrideApiVersionWhitelist))
                {
                    log.Debug("API version mismatch");
                    return;
                }

                // Check platform
                PlatformFilter = Assembly.GetCustomAttribute<PlatformFilterAttribute>();
                if (!CheckPlatform())
                {
                    log.Debug("Platform mismatch");
                    return;
                }

                // Done checking
                ExtensionCheckResult = ExtensionCheckResult.OK;
                log.Debug("All checks passed, loading exports");
                // Load exports:
                LoadExports();

            }
            // If not changed, and we can trust the existing data:
            else
            {
                // Do not load the assembly if we are sure that it isn't an extension
                if (ExtensionCheckResult == ExtensionCheckResult.NotAnExtension)
                {
                    log.Debug("Cache says: Not an extension");
                    return;
                }

                // Check API version if it exists
                if (ApiVersion != null && !CheckApiVersion(overrideApiVersionWhitelist))
                {
                    log.Debug("Cache says: API version mismatch");
                    return;
                }

                // Check platform filter only if it exists
                if (PlatformFilter != null && !CheckPlatform())
                {
                    log.Debug("Cache says: Platform mismatch");
                    return;
                }

                // ~Load assembly - if loading hasn't been attempted, or if file wasn't found last time~
                // EDIT: Why did I do this? It should be loaded always if it hasn't been loaded yet...
                if (Assembly == null)
                {
                    log.Debug("Assembly load required");
                    LoadAssembly();
                }

                // Return immediately if failed
                if (Assembly == null || AssemblyLoadResult != AssemblyLoadResult.OK)
                {
                    log.Debug("Failed to load assembly");
                    return;
                }
                
                // Get and check API version if we haven't yet
                if (ApiVersion == null && (!LoadExtDef() || !CheckApiVersion(overrideApiVersionWhitelist)))
                {
                    log.Debug("API version mismatch");
                    return;
                }

                // Get and check platform if we haven't yet
                if (PlatformFilter == null)
                {
                    PlatformFilter = Assembly.GetCustomAttribute<PlatformFilterAttribute>();
                    if (!CheckPlatform())
                    {
                        log.Debug("Platform mismatch");
                        return;
                    }
                }

                // Done checking
                ExtensionCheckResult = ExtensionCheckResult.OK;
                log.Debug("All checks passed, loading exports");
                // Load exports ~if they aren't already:~ EDIT: Can't do that. TODO: Compile-time optimizations
                /*if (Exports == null)*/ LoadExports();
            }
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
        private void LoadAssembly()
        {
            // Load the assembly if needed
            if (Assembly != null) return;
            AssemblyLoadResult = AssemblyName.TryLoadAssembly(out var a);
            Assembly = a;
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
            // TODO: Test this on Linux
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
        public string? RelativeFilePath { get; set; }

        /// <summary>
        /// The time at which the DLL file was last modified, in universal coordinated time (UTC).
        /// </summary>
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
        public string? Key { get; set; }

        /// <summary>
        /// The API version targeted by the extension.
        /// </summary>
        public int? ApiVersion { get; set; }

        /// <summary>
        /// The platform filter requested by the extension (if specified).
        /// </summary>
        public PlatformFilterAttribute? PlatformFilter { get; set; }

        /// <summary>
        /// The result of checking the extension metadata.
        /// </summary>
        public ExtensionCheckResult ExtensionCheckResult { get; set; }

        /// <summary>
        /// The exported registrations from the extension.
        /// </summary>
        [JsonIgnore] // Serializing this won't work sadly
        public IList<ExportedRegistrationInfo>? Exports { get; set; }

        /// <summary>
        /// The basic information exported by the extension.
        /// </summary>
        public ExtensionBasicInfo? BasicInfo { get; set; }

        // Non-serialized properties:
        
        /// <summary>
        /// The absolute path to the DLL file.
        /// </summary>
        [JsonIgnore]
        public string? FilePath { get; set; }

        /// <summary>
        /// The assembly instance. Will be null if loading has failed.
        /// </summary>
        [JsonIgnore]
        public Assembly? Assembly { get; private set; }

        /// <summary>
        /// Gets whether the extension is fully loaded successfully.
        /// </summary>
        [JsonIgnore]
        public bool IsLoaded { get; private set; }
    }
}