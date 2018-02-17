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
        private IContainer _iocContainer = new Container();
        [NotNull]
        private readonly Dictionary<string, ExtensionInfo> _extensions = new Dictionary<string, ExtensionInfo>();
        public IReadOnlyDictionary<string, ExtensionInfo> LoadedExtensions => new ReadOnlyDictionary<string, ExtensionInfo>(_extensions);
        private readonly Dictionary<string, IContainer> _isolatedContainers = new Dictionary<string, IContainer>();

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
        
        public ExtensionInfo LoadExtension(Assembly a, bool isolated = false, bool overrideApiVersionWhitelist = false)
        {
            var extdef = a.GetCustomAttribute<ExtensionAttribute>();
            if (extdef == null) throw new ExtensibilityException($"The assembly {a.FullName} is not an extension!");
            // Check ApiVersion
            if (!ExtensibilityConstants.CheckApiVersion(extdef.ApiVersion, overrideApiVersionWhitelist))
                throw new ExtensibilityException($"API version mismatch: Extension {extdef.Key} targets the wrong API version {extdef.ApiVersion}, current version is {ExtensibilityConstants.CurrentApiVersion}!");

            // Check platform
            var platformFilter = a.GetCustomAttribute<PlatformFilterAttribute>();
            // First, check the arch
            if (platformFilter.Action == FilterAction.Whitelist ^ platformFilter.ProcessArchitecture.AppliesTo(RuntimeInformation.ProcessArchitecture))
                throw new ExtensibilityException($"Process architecture mismatch: Extension {extdef.Key} will {platformFilter.Action} platform {platformFilter.ProcessArchitecture}, but this process runs as {RuntimeInformation.ProcessArchitecture}!");
            // Next, the OS
            if (platformFilter.Action == FilterAction.Whitelist ^ RuntimeInformation.IsOSPlatform(platformFilter.OSKind.ToOSPlatform()))
                throw new ExtensibilityException($"OS kind mismatch: Extension {extdef.Key} will {platformFilter.Action} OS kind {platformFilter.OSKind}, but the current OS is different!");
            // Finally, the OS version
            if (platformFilter.Action == FilterAction.Whitelist ^ platformFilter.OSVersionMatches())
                throw new ExtensibilityException($"OS version mismatch: Extension {extdef.Key} will {platformFilter.Action} an OS version that is {platformFilter.Rule} {platformFilter.Version}{(platformFilter.Rule == FilterRule.Between ? " and " + platformFilter.OtherVersion : "")}, but the current OS is of a version outside of this range!");
            
            var exports = AttributedModel.Scan(new[] { a }).ToList();
            IContainer target;
            if (isolated)
            {
                target = new Container().WithMefAttributedModel();
                target.RegisterExports(exports);
            }
            else
            {
                target = _iocContainer;
                _iocContainer.RegisterExports(exports);
            }

            IExtensionBasicInfo info;
            try { info = target.Resolve<IExtensionBasicInfo>(ExtensibilityConstants.InfoExportKey); }
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
            if (target != _iocContainer)
            {
                _iocContainer = _iocContainer.With(rules => rules.WithFallbackContainer(target));
                _isolatedContainers.Add(extdef.Key, target);
            }
            _extensions.Add(extdef.Key, extinfo);
            return extinfo;
        }

        public void AddIsolatedContainer(IContainer c, string key)
        {
            _iocContainer = _iocContainer.With(rules => rules.WithFallbackContainer(c));
            _isolatedContainers.Add(key, c);
        }

        // TODO: Save/load cache - in Core
        
        public void UnloadIsolated(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var c = _isolatedContainers[key];
            _iocContainer = _iocContainer.With(rules => rules.WithoutFallbackContainer(c));
            _isolatedContainers.Remove(key);
        }

        public void UnloadAllIsolated()
        {
            foreach (var key in _isolatedContainers.Keys)
                UnloadIsolated(key);
        }

        // Use with caution!!!
        public void DeleteAllSingletonsAndCache()
        {
            _iocContainer = _iocContainer.WithoutSingletonsAndCache();
        }

        // The main way to access the container
        public IContainer OpenScope(string name = null)
        {
            return _iocContainer.OpenScope(name);
        }

        //public bool ReloadExtension([NotNull] string extensionKey)
        //{
        //    // TODO: Check if extension is already loaded
        //    if (extensionKey == null) throw new ArgumentNullException(nameof(extensionKey));
        //    var c = GetExtensionContainer(extensionKey);
        //    if (c == null) return false;
        //    _iocContainer = _iocContainer.With(rules => rules.WithFallbackContainer(c));
        //    return true;
        //}

        public void Dispose()
        {
            _iocContainer.Dispose();
        }
    }
}
