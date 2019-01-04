using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.MefAttributedModel;
using JetBrains.Annotations;

namespace FuwaTea.Extensibility
{
    /// <summary>
    /// IoC container for all extensibility features, including configuration.
    /// </summary>
    public partial class ExtensibilityContainer : IDisposable
    {
        internal readonly IContainer IocContainer = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient()).WithMefAttributedModel();
        [NotNull]
        private readonly Dictionary<string, Extension> _extensions = new Dictionary<string, Extension>();
        
        /// <summary>
        /// Gets information about the loaded libraries.
        /// </summary>
        public IReadOnlyDictionary<string, Extension> LoadedExtensions => new ReadOnlyDictionary<string, Extension>(_extensions);

        /// <summary>
        /// Register a loaded <see cref="Extension"/>.
        /// </summary>
        /// <remarks>Note: You must call <see cref="Extension.Load"/> before calling this method.</remarks>
        /// <param name="ext">The extension to register.</param>
        /// <exception cref="InvalidOperationException">If the <see cref="Extension"/> hasn't been successfully loaded</exception>
        public void RegisterExtension(Extension ext)
        {
            if (!ext.IsLoaded) throw new InvalidOperationException("The Extension must be loaded first! Please call Extension.Load() and verify that no error has occurred.");
            IocContainer.RegisterExports(ext.Exports);
            
            // Get extension info
            var info = IocContainer.Resolve<IExtensionBasicInfo>(ext.Key, IfUnresolved.ReturnDefault) ?? new ExtensionBasicInfo
            {
                Author = ext.Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company,
                Description = ext.Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
                Title = ext.Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title,
                Version = ext.Assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
            };

            // AutoInitialize
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            IocContainer.ResolveMany(null, serviceKey: ExtensibilityConstants.AutoInitKey).ToList();
            // This should have forced initialization of the objects

            // Finishing up
            _extensions.Add(ext.Key, ext);
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
