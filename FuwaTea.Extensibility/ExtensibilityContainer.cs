﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.MefAttributedModel;
using JetBrains.Annotations;

namespace FuwaTea.Extensibility
{
    public class ExtensibilityContainer : IDisposable
    {
        [NotNull]
        private IContainer _iocContainer;
        [NotNull]
        private readonly Dictionary<string, (ExtensionInfo, IContainer)> _extensions;

        private readonly Func<AssemblyName, Assembly> _loadDllFunc;

        public ExtensibilityContainer(Func<AssemblyName, Assembly> loadDllFunc)
        {
            _extensions = new Dictionary<string, (ExtensionInfo, IContainer)>();
            _iocContainer = new Container();
            _loadDllFunc = loadDllFunc;
        }

        public ExtensionInfo LoadExtension(AssemblyName dll, bool overrideApiVersionWhitelist = false)
        {
            // TODO: Add platform checking!
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
            // TODO: Update code for ApiVersion checking?
            
            var extdef = a.GetCustomAttribute<ExtensionAttribute>();
            // Check ApiVersion here

            var exports = AttributedModel.Scan(new[] { a }).ToList();
            var c = new Container().WithMefAttributedModel();
            c.RegisterExports(exports);

            var info = c.Resolve<IExtensionBasicInfo>(ExtensibilityConstants.InfoExportKey) ?? new ExtensionBasicInfo
            {
                Author = a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company,
                Description = a.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
                Title = a.GetCustomAttribute<AssemblyTitleAttribute>()?.Title,
                Version = a.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
            };

            _iocContainer = _iocContainer.With(rules => rules.WithFallbackContainer(c));
            var extinfo = new ExtensionInfo(a.GetName(), a.Location, extdef.Key, extdef.ApiVersion, info);
            _extensions.Add(extdef.Key, (extinfo, c));
            return extinfo;
        }

        // TODO: Save/load cache - in Core

        public void UnloadExtension(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var c = GetExtensionContainer(key);
            if (c == null) return;
            _iocContainer = _iocContainer.With(rules => rules.WithoutFallbackContainer(c));
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

        public IEnumerable<ExtensionInfo> GetAllExtensionInfo()
        {
            return _extensions.Values.Select(ext => ext.Item1);
        }

        // Extension method?
        public ExtensionInfo GetExtensionInfo(string key = null, string dllPath = null)
        {
            return _extensions.FirstOrDefault(p => p.Key == key || p.Value.Item1.DllPath == dllPath).Value.Item1;
        }

        public bool IsExtensionLoaded([NotNull] string extensionKey)
        {
            if (extensionKey == null) throw new ArgumentNullException(nameof(extensionKey));
            var c = GetExtensionContainer(extensionKey);
            return c != null && IsExtensionLoaded(c);
        }

        public bool ReloadExtension([NotNull] string extensionKey)
        {
            // TODO: Check if extension is already loaded
            if (extensionKey == null) throw new ArgumentNullException(nameof(extensionKey));
            var c = GetExtensionContainer(extensionKey);
            if (c == null) return false;
            _iocContainer = _iocContainer.With(rules => rules.WithFallbackContainer(c));
            return true;
        }

        [CanBeNull]
        private IContainer GetExtensionContainer([NotNull] string extensionKey)
        {
            return _extensions.TryGetValue(extensionKey, out var ext) ? ext.Item2 : null;
        }

        private bool IsExtensionLoaded([NotNull] IContainer c)
        {
            return _iocContainer.Rules.FallbackContainers.Any(cwr => cwr.Container == c);
        }

        public void Dispose()
        {
            _iocContainer.Dispose();
        }
    }
}