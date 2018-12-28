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
    /// <summary>
    /// IoC container for all extensibility features.
    /// </summary>
    public class ExtensibilityContainer : IDisposable
    {
        internal readonly IContainer IocContainer = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient()).WithMefAttributedModel();
        [NotNull]
        private readonly Dictionary<string, ExtensionInfo> _extensions = new Dictionary<string, ExtensionInfo>();

        /// <summary>
        /// Gets information about the loaded libraries.
        /// </summary>
        public IReadOnlyDictionary<string, ExtensionInfo> LoadedExtensions => new ReadOnlyDictionary<string, ExtensionInfo>(_extensions);

        /// <summary>
        /// Loads a specific library into the container.
        /// </summary>
        /// <param name="dll">The library's <see cref="AssemblyName"/>.</param>
        /// <param name="overrideApiVersionWhitelist">Optionally overrides the API version whitelist check.</param>
        /// <returns>Information about the extension.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="dll"/> parameter is null.</exception>
        /// <exception cref="ExtensibilityException">If the <see cref="Assembly"/> has failed to load, or if the library is not supported on the current platform, or if the library is not supported on the current API version.</exception>
        /// // TODO IMPORTANT: Use 3 different exception types, with correct fields. Potentially avoid exceptions altogether!
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
                throw new ExtensibilityException("Failed to load assembly: See InternalException for details", ex);
            }
            if (a == null)
                throw new ExtensibilityException("Failed to load assembly: Result was null - possible platform error");
            return LoadExtension(a, overrideApiVersionWhitelist);
        }
        
        /// <summary>
        /// Loads a specific <see cref="Assembly"/> into the container.
        /// </summary>
        /// <param name="a">The <see cref="Assembly"/> to load.</param>
        /// <param name="overrideApiVersionWhitelist">Optionally overrides the API version whitelist check.</param>
        /// <returns>Information about the extension.</returns>
        /// <exception cref="ExtensibilityException">If the library is not supported on the current platform, or if the library is not supported on the current API version.</exception>
        public ExtensionInfo LoadExtension(Assembly a, bool overrideApiVersionWhitelist = false)
        {
            var extDef = a.GetCustomAttribute<ExtensionAttribute>();
            if (extDef == null) throw new ExtensibilityException($"The assembly {a.FullName} is not an extension!");
            // Check ApiVersion
            if (!ExtensibilityUtils.CheckApiVersion(extDef.ApiVersion, overrideApiVersionWhitelist))
                throw new ExtensibilityException($"API version mismatch: Extension {extDef.Key} targets the wrong API version {extDef.ApiVersion}, current version is {ExtensibilityConstants.CurrentApiVersion}!");

            // Check platform
            var platformFilter = a.GetCustomAttribute<PlatformFilterAttribute>();
            if (platformFilter != null)
            {
                // First, check the arch
                if (platformFilter.Action == FilterAction.Whitelist ^ platformFilter.ProcessArchitecture.AppliesTo(RuntimeInformation.ProcessArchitecture))
                    throw new ExtensibilityException($"Process architecture mismatch: Extension {extDef.Key} will {platformFilter.Action} platform {platformFilter.ProcessArchitecture}, but this process runs as {RuntimeInformation.ProcessArchitecture}!");
                // Next, the OS
                if (platformFilter.Action == FilterAction.Whitelist ^ RuntimeInformation.IsOSPlatform(platformFilter.OSKind.ToOSPlatform()))
                    throw new ExtensibilityException($"OS kind mismatch: Extension {extDef.Key} will {platformFilter.Action} OS kind {platformFilter.OSKind}, but the current OS is different!");
                // Finally, the OS version
                if (platformFilter.Action == FilterAction.Whitelist ^ platformFilter.OSVersionMatches())
                    throw new ExtensibilityException($"OS version mismatch: Extension {extDef.Key} will {platformFilter.Action} an OS version that is {platformFilter.Rule} {platformFilter.Version}{(platformFilter.Rule == FilterRule.Between ? " and " + platformFilter.OtherVersion : "")}, but the current OS is of a version outside of this range!");
            }

            // Building and registration
            var exports = AttributedModel.Scan(new[] { a }).ToList();
            // TODO IMPORTANT: Save/load exports - in Core. Split this method right here!
            IocContainer.RegisterExports(exports);
            
            // Get extension info
            IExtensionBasicInfo info;
            try { info = IocContainer.Resolve<IExtensionBasicInfo>(extDef.Key); }
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
            var extInfo = new ExtensionInfo(a.GetName(), a.Location, extDef.Key, extDef.ApiVersion, info);

            // AutoInitialize
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            IocContainer.ResolveMany(null, serviceKey: ExtensibilityConstants.AutoInitKey).ToList();
            // This should have forced initialization of the objects

            // Finishing up
            _extensions.Add(extDef.Key, extInfo);
            return extInfo;
        }
        
        /// <summary>
        /// Add a simple service registration.
        /// </summary>
        /// <remarks>Should only be used where absolutely required.</remarks>
        /// <param name="serviceKey">An optional service key.</param>
        /// <typeparam name="TInterface">The service type.</typeparam>
        /// <typeparam name="TClass">The implementation type.</typeparam>
        public void Register<TInterface, TClass>(object serviceKey = null) where TClass : TInterface
        {
            IocContainer.Register<TInterface, TClass>(serviceKey: serviceKey);
        }
        
        /// <summary>
        /// Add an instance registration.
        /// </summary>
        /// <remarks>Useful for constants that are only known at runtime. Should only be used where absolutely required.</remarks>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance to register.</param>
        /// <param name="serviceKey">An optional service key.</param>
        public void RegisterInstance<T>(T instance, object serviceKey = null)
        {
            IocContainer.UseInstance(instance, serviceKey: serviceKey);
        }
        
        /// <summary>
        /// Opens a Scope for resolving classes.
        /// </summary>
        /// <param name="name">Optional name of the scope.</param>
        /// <returns>An <see cref="IResolverContext"/>.</returns>
        public IResolverContext OpenScope(string name = null)
        {
            return IocContainer.OpenScope(name);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            IocContainer.Dispose();
        }
    }
}
