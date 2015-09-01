using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using FuwaTea.Annotations;
using log4net;

namespace FuwaTea.Lib
{
    // Retrieved from http://www.codeproject.com/Articles/20917/Creating-a-Custom-Settings-Provider?msg=4759869#xx4759869xx (deBUGer!)
    // Which was derived from http://www.codeproject.com/Articles/20917/Creating-a-Custom-Settings-Provider?msg=2934144#xx2934144xx (gpgemini)
    // Which was ported from http://www.codeproject.com/Articles/20917/Creating-a-Custom-Settings-Provider (CodeChimp - CPOL license)
    // Derived again after all that. (OronDF343)

    // Util functions
    public static class XExtensions
    {
        public static XElement GetOrAddElement([NotNull] this XContainer parent, [NotNull] XName name)
        {
            var element = parent.Element(name);
            if (element == null)
            {
                element = new XElement(name);
                parent.Add(element);
            }
            return element;
        }
    }

    // TODO: Logging
    public class PortableSettingsProvider : SettingsProvider
    {
        const string SettingsRootName = "Settings";
        const string RoamingSettingsRootName = "Roaming";
        const string LocalSettingsRootName = "Local";

        readonly string _fileName;
        readonly Lazy<XDocument> _settingsXml;

        public static readonly string DefaultDirectory = Assembly.GetEntryAssembly().GetUserDataPath();
        public static readonly string DefaultFileName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".settings";

        public PortableSettingsProvider()
            : this(DefaultDirectory, DefaultFileName)
        {
        }

        public PortableSettingsProvider([NotNull] string settingsDirectory, [NotNull] string settingsFileName)
        {
            Directory.CreateDirectory(settingsDirectory);

            _fileName = Path.Combine(settingsDirectory, settingsFileName);
            _settingsXml = new Lazy<XDocument>(() => LoadOrCreateSettings(_fileName), LazyThreadSafetyMode.PublicationOnly);
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(ApplicationName, config);
        }

        public override string ApplicationName
        {
            get
            {
                return (Assembly.GetEntryAssembly().GetName().Name);
            }
            set { }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            // Create new collection of values
            var values = new SettingsPropertyValueCollection();

            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                values.Add(new SettingsPropertyValue(setting)
                {
                    IsDirty = false,
                    SerializedValue = GetValue(setting),
                });
            }
            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection properties)
        {
            // Only dirty settings are included in properties, and only ones relevant to this provider
            foreach (SettingsPropertyValue propertyValue in properties)
            {
                SetValue(propertyValue);
            }

            try
            {
                _settingsXml.Value.Save(_fileName);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error("Failed to save the setting!", ex);
            }
        }

        XElement SettingsRoot => _settingsXml.Value.Root;

        object GetValue(SettingsProperty setting)
        {
            var propertyPath = IsRoaming(setting)
               ? string.Concat("./", RoamingSettingsRootName, "/", setting.Name)
               : string.Concat("./", LocalSettingsRootName, "/", Environment.MachineName, "/", setting.Name);

            var propertyElement = SettingsRoot.XPathSelectElement(propertyPath);
            return propertyElement?.Value ?? setting.DefaultValue;
        }

        void SetValue(SettingsPropertyValue setting)
        {
            var parentElement = IsRoaming(setting.Property)
                                    ? SettingsRoot.GetOrAddElement(RoamingSettingsRootName)
                                    : SettingsRoot.GetOrAddElement(LocalSettingsRootName)
                                                  .GetOrAddElement(Environment.MachineName);

            parentElement.GetOrAddElement(setting.Name).Value = setting.SerializedValue.ToString();
        }

        static XDocument LoadOrCreateSettings(string filePath)
        {
            XDocument settingsXml = null;
            try
            {
                settingsXml = XDocument.Load(filePath);

                if (settingsXml.Root == null)
                {
                    LogManager.GetLogger(typeof(PortableSettingsProvider)).Fatal("Null XML root in settings!");
                    throw new NullReferenceException("Null XML root in settings!");
                }
                if (settingsXml.Root.Name.LocalName != SettingsRootName)
                {
                    LogManager.GetLogger(typeof(PortableSettingsProvider)).Error("Invalid settings format!");
                    settingsXml = null;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(typeof(PortableSettingsProvider)).Error("Invalid settings file!", ex);
            }

            return settingsXml
                   ?? new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                    new XElement(SettingsRootName, string.Empty));
        }

        static bool IsRoaming(SettingsProperty property)
        {
            return property.Attributes.Cast<DictionaryEntry>().Any(a => a.Value is SettingsManageabilityAttribute);
        }
    }
}
