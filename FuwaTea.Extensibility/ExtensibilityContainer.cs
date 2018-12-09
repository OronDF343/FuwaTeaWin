using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DryIoc;
using DryIoc.MefAttributedModel;
using JetBrains.Annotations;

namespace FuwaTea.Extensibility
{
    public class ExtensibilityContainer : IDisposable
    {
        // TODO: Export self at init!
        private IContainer _iocContainer = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());
        [NotNull]
        private readonly Dictionary<string, ExtensionInfo> _extensions = new Dictionary<string, ExtensionInfo>();
        public IReadOnlyDictionary<string, ExtensionInfo> LoadedExtensions => new ReadOnlyDictionary<string, ExtensionInfo>(_extensions);

        public ExtensionInfo LoadExtension(AssemblyName dll, bool overrideApiVersionWhitelist = false)
        {
            if (dll == null) throw new ArgumentNullException(nameof(dll));

            Assembly a;
            try
            {
                a = Assembly.Load(dll);
            }
            catch (Exception ex)
            {
                throw new ExtensibilityException("Failed to load assembly: see InternalException", ex);
            }
            if (a == null)
                throw new ExtensibilityException("Failed to load assembly: result was null - possible platform error");
            return LoadExtension(a, overrideApiVersionWhitelist);
        }
        
        public ExtensionInfo LoadExtension(Assembly a, bool overrideApiVersionWhitelist = false)
        {
            var extdef = a.GetCustomAttribute<ExtensionAttribute>();
            if (extdef == null) throw new ExtensibilityException($"The assembly {a.FullName} is not an extension!");
            // Check ApiVersion
            if (!ExtensibilityConstants.CheckApiVersion(extdef.ApiVersion, overrideApiVersionWhitelist))
                throw new ExtensibilityException($"API version mismatch: Extension {extdef.Key} targets the wrong API version {extdef.ApiVersion}, current version is {ExtensibilityConstants.CurrentApiVersion}!");

            // Check platform
            var platformFilter = a.GetCustomAttribute<PlatformFilterAttribute>();
            if (platformFilter != null)
            {
                // First, check the arch
                if (platformFilter.Action == FilterAction.Whitelist ^ platformFilter.ProcessArchitecture.AppliesTo(RuntimeInformation.ProcessArchitecture))
                    throw new ExtensibilityException($"Process architecture mismatch: Extension {extdef.Key} will {platformFilter.Action} platform {platformFilter.ProcessArchitecture}, but this process runs as {RuntimeInformation.ProcessArchitecture}!");
                // Next, the OS
                if (platformFilter.Action == FilterAction.Whitelist ^ RuntimeInformation.IsOSPlatform(platformFilter.OSKind.ToOSPlatform()))
                    throw new ExtensibilityException($"OS kind mismatch: Extension {extdef.Key} will {platformFilter.Action} OS kind {platformFilter.OSKind}, but the current OS is different!");
                // Finally, the OS version
                if (platformFilter.Action == FilterAction.Whitelist ^ platformFilter.OSVersionMatches())
                    throw new ExtensibilityException($"OS version mismatch: Extension {extdef.Key} will {platformFilter.Action} an OS version that is {platformFilter.Rule} {platformFilter.Version}{(platformFilter.Rule == FilterRule.Between ? " and " + platformFilter.OtherVersion : "")}, but the current OS is of a version outside of this range!");
            }

            // Building and registration
            var exports = AttributedModel.Scan(new[] { a }).ToList();
            _iocContainer.RegisterExports(exports);
            
            // Get extension info
            IExtensionBasicInfo info;
            try { info = _iocContainer.Resolve<IExtensionBasicInfo>(extdef.Key); }
            catch
            {
                info = new ExtensionBasicInfo
                {
                    Author = a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company,
                    Description = a.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
                    Title = a.GetCustomAttribute<AssemblyTitleAttribute>()?.Title,
                    Version = a.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                };
            }
            var extinfo = new ExtensionInfo(a.GetName(), a.Location, extdef.Key, extdef.ApiVersion, info);

            // AutoInitialize
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            _iocContainer.ResolveMany(null, serviceKey: ExtensibilityConstants.AutoInitKey).ToList();
            // This should have forced initialization of the objects

            // Finishing up
            _extensions.Add(extdef.Key, extinfo);
            return extinfo;
        }

        // TODO: Save/load cache - in Core
        
        // Use with caution!!!
        public void DeleteAllSingletonsAndCache()
        {
            _iocContainer = _iocContainer.WithoutSingletonsAndCache();
        }

        // The main way to access the container
        public IResolverContext OpenScope(string name = null)
        {
            return _iocContainer.OpenScope(name);
        }

        public void Dispose()
        {
            _iocContainer.Dispose();
        }
    }
}
