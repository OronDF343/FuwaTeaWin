using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.MefAttributedModel;
using JetBrains.Annotations;

namespace Sage.Extensibility
{
    /// <summary>
    /// IoC container for all extensibility features, including configuration.
    /// </summary>
    public partial class ExtensibilityContainer : IDisposable
    {
        internal readonly IContainer IocContainer = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient()).WithMefAttributedModel();

        [NotNull]
        private readonly Dictionary<string, Extension> _loadedExtensions = new Dictionary<string, Extension>();
        
        /// <summary>
        /// Gets information about the loaded libraries.
        /// </summary>
        public IReadOnlyDictionary<string, Extension> LoadedExtensions => new ReadOnlyDictionary<string, Extension>(_loadedExtensions);

        /// <summary>
        /// Register a loaded <see cref="Extension"/>.
        /// </summary>
        /// <remarks>Note: You must call <see cref="Extension.Load"/> before calling this method.</remarks>
        /// <param name="ext">The extension to register.</param>
        /// <exception cref="InvalidOperationException">If the <see cref="Extension"/> hasn't been successfully loaded -or- an extension with the same key already exists</exception>
        public void RegisterExtension(Extension ext)
        {
            if (!ext.IsLoaded) throw new InvalidOperationException("The Extension must be loaded first! Please call Extension.Load() and verify that no error has occurred.");
            if (_loadedExtensions.ContainsKey(ext.Key)) throw new InvalidOperationException("An extension with the same key already exists.");
            IocContainer.RegisterExports(ext.Exports);
            
            // Get extension info if needed
            if (ext.BasicInfo == null)
                ext.BasicInfo = IocContainer.Resolve<ExtensionBasicInfo>(ext.Key, IfUnresolved.ReturnDefault)
                                ?? new ExtensionBasicInfo
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
            _loadedExtensions.Add(ext.Key, ext);
        }

        /// <summary>
        /// Add a simple service registration.
        /// </summary>
        /// <remarks>Should only be used where absolutely required.</remarks>
        /// <param name="serviceKey">An optional service key.</param>
        /// <param name="reuse">The optional reuse type.</param>
        /// <param name="metadata">The optional metadata.</param>
        /// <typeparam name="TInterface">The service type.</typeparam>
        /// <typeparam name="TClass">The implementation type.</typeparam>
        public void Register<TInterface, TClass>(object serviceKey = null, IReuse reuse = null, object metadata = null) where TClass : TInterface
        {
            IocContainer.Register<TInterface, TClass>(serviceKey: serviceKey, reuse: reuse,
                                                      setup: metadata == null ? null : Setup.With(metadata));
        }

        /// <summary>
        /// Add a simple service registration, where the implementation is registered as its own type.
        /// </summary>
        /// <remarks>Should only be used where absolutely required.</remarks>
        /// <param name="serviceKey">An optional service key.</param>
        /// <param name="reuse">The optional reuse type.</param>
        /// <param name="metadata">The optional metadata.</param>
        /// <typeparam name="TClass">The implementation type.</typeparam>
        public void Register<TClass>(object serviceKey = null, IReuse reuse = null, object metadata = null)
        {
            IocContainer.Register<TClass>(serviceKey: serviceKey, reuse: reuse,
                                          setup: metadata == null ? null : Setup.With(metadata));
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
