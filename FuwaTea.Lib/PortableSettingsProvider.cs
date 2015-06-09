/*************************************************************
 * PortableSettingsProvider.cs
 * Portable Settings Provider for C# applications
 * 
 * 2010- Michael Nathan
 * http://www.Geek-Republic.com
 * 
 * Licensed under Creative Commons CC BY-SA
 * http://creativecommons.org/licenses/by-sa/3.0/legalcode
 * 
 * Modified by OronDF343
 * No changes to license
 * 
 *************************************************************/

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace FuwaTea.Lib
{
    public class PortableSettingsProvider : SettingsProvider
    {
        // Define some static strings later used in our XML creation
        // XML Root node
        const string XmlRoot = "configuration";

        // Configuration declaration node
        const string ConfigNode = "configSections";

        // Configuration section group declaration node
        const string GroupNode = "sectionGroup";

        // User section node
        const string UserNode = "userSettings";

        // Application Specific Node
        readonly string _appNode = Assembly.GetEntryAssembly().GetName().Name + ".Properties.Settings";

        private XmlDocument _xmlDoc;



        // Override the Initialize method
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(ApplicationName, config);
        }

        // Override the ApplicationName property, returning the solution name.  No need to set anything, we just need to
        // retrieve information, though the set method still needs to be defined.
        public override string ApplicationName
        {
            get
            {
                return (Assembly.GetEntryAssembly().GetName().Name);
            }
            set { }
        }

        // Simply returns the name of the settings file, which is the solution name plus ".config"
        public virtual string GetSettingsFilename()
        {
            return ApplicationName + ".config";
        }

        // Gets current executable path in order to determine where to read and write the config file
        public virtual string GetAppPath()
        {
            // TODO: update if other usage is updated
            // TODO: this has temporary company and app names until the name is normalized
            return Assembly.GetEntryAssembly().GetUserDataPath("OronDF343", "SJE");
        }

        // Retrieve settings from the configuration file
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sContext, SettingsPropertyCollection settingsColl)
        {
            // Create a collection of values to return
            var retValues = new SettingsPropertyValueCollection();

            // Create a temporary SettingsPropertyValue to reuse

            // Loop through the list of settings that the application has requested and add them
            // to our collection of return values.
            foreach (SettingsProperty sProp in settingsColl)
            {
                var setVal = new SettingsPropertyValue(sProp) {IsDirty = false, SerializedValue = GetSetting(sProp)};
                retValues.Add(setVal);
            }
            return retValues;
        }

        // Save any of the applications settings that have changed (flagged as "dirty")
        public override void SetPropertyValues(SettingsContext sContext, SettingsPropertyValueCollection settingsColl)
        {
            // Set the values in XML
            foreach (SettingsPropertyValue spVal in settingsColl)
            {
                SetSetting(spVal);
            }

            // Write the XML file to disk
            try
            {
                XmlConfig.Save(Path.Combine(GetAppPath(), GetSettingsFilename()));
            }
            catch (Exception ex)
            {
                // Create an informational message for the user if we cannot save the settings.
                // Enable whichever applies to your application type.

                // Uncomment the following line to enable a MessageBox for forms-based apps
                //System.Windows.Forms.MessageBox.Show(ex.Message, "Error writting configuration file to disk", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                // Uncomment the following line to enable a console message for console-based apps
                Console.WriteLine("Error writing configuration file to disk: " + ex.Message); // TODO: handle this differently
            }
        }

        private XmlDocument XmlConfig
        {
            get
            {
                // Check if we already have accessed the XML config file. If the xmlDoc object is empty, we have not.
                if (_xmlDoc != null) return _xmlDoc;
                _xmlDoc = new XmlDocument();

                // If we have not loaded the config, try reading the file from disk.
                try
                {
                    _xmlDoc.Load(Path.Combine(GetAppPath(), GetSettingsFilename()));
                }

                    // If the file does not exist on disk, catch the exception then create the XML template for the file.
                catch (Exception)
                {
                    // XML Declaration
                    // <?xml version="1.0" encoding="utf-8"?>
                    var dec = _xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    _xmlDoc.AppendChild(dec);

                    // Create root node and append to the document
                    // <configuration>
                    var rootNode = _xmlDoc.CreateElement(XmlRoot);
                    _xmlDoc.AppendChild(rootNode);

                    // Create Configuration Sections node and add as the first node under the root
                    // <configSections>
                    var configNode = _xmlDoc.CreateElement(ConfigNode);
                    _xmlDoc.DocumentElement.PrependChild(configNode);

                    // Create the user settings section group declaration and append to the config node above
                    // <sectionGroup name="userSettings"...>
                    var groupNode = _xmlDoc.CreateElement(GroupNode);
                    groupNode.SetAttribute("name", UserNode);
                    groupNode.SetAttribute("type", "System.Configuration.UserSettingsGroup");
                    configNode.AppendChild(groupNode);

                    // Create the Application section declaration and append to the groupNode above
                    // <section name="AppName.Properties.Settings"...>
                    var newSection = _xmlDoc.CreateElement("section");
                    newSection.SetAttribute("name", _appNode);
                    newSection.SetAttribute("type", "System.Configuration.ClientSettingsSection");
                    groupNode.AppendChild(newSection);

                    // Create the userSettings node and append to the root node
                    // <userSettings>
                    var userNode = _xmlDoc.CreateElement(UserNode);
                    _xmlDoc.DocumentElement.AppendChild(userNode);

                    // Create the Application settings node and append to the userNode above
                    // <AppName.Properties.Settings>
                    var appNode = _xmlDoc.CreateElement(_appNode);
                    userNode.AppendChild(appNode);
                }
                return _xmlDoc;
            }
        }

        // Retrieve values from the configuration file, or if the setting does not exist in the file, 
        // retrieve the value from the application's default configuration
        private object GetSetting(SettingsProperty setProp)
        {
            object retVal;
            try
            {
                // Search for the specific settings node we are looking for in the configuration file.
                // If it exists, return the InnerText or InnerXML of its first child node, depending on the setting type.

                // If the setting is serialized as a string, return the text stored in the config
                if (setProp.SerializeAs.ToString() == "String")
                {
                    return XmlConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']").FirstChild.InnerText;
                }

                    // If the setting is stored as XML, deserialize it and return the proper object.  This only supports
                    // StringCollections at the moment - I will likely add other types as I use them in applications.
                var settingType = setProp.PropertyType.ToString();
                var xmlData = XmlConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']").FirstChild.InnerXml;
                var xs = new XmlSerializer(typeof(string[]));
                var data = (string[])xs.Deserialize(new XmlTextReader(xmlData, XmlNodeType.Element, null));

                switch (settingType)
                {
                    case "System.Collections.Specialized.StringCollection":
                        var sc = new StringCollection();
                        sc.AddRange(data);
                        return sc;
                    default:
                        return "";
                }
            }
            catch (Exception)
            {
                // Check to see if a default value is defined by the application.
                // If so, return that value, using the same rules for settings stored as Strings and XML as above
                if ((setProp.DefaultValue != null))
                {
                    if (setProp.SerializeAs.ToString() == "String")
                    {
                        retVal = setProp.DefaultValue.ToString();
                    }
                    else
                    {
                        var settingType = setProp.PropertyType.ToString();
                        var xmlData = setProp.DefaultValue.ToString();
                        var xs = new XmlSerializer(typeof(string[]));
                        var data = (string[])xs.Deserialize(new XmlTextReader(xmlData, XmlNodeType.Element, null));

                        switch (settingType)
                        {
                            case "System.Collections.Specialized.StringCollection":
                                var sc = new StringCollection();
                                sc.AddRange(data);
                                return sc;

                            default: return "";
                        }
                    }
                }
                else
                {
                    retVal = "";
                }
            }
            return retVal;
        }

        private void SetSetting(SettingsPropertyValue setProp)
        {
            // Define the XML path under which we want to write our settings if they do not already exist
            XmlNode settingNode;

            try
            {
                // Search for the specific settings node we want to update.
                // If it exists, return its first child node, (the <value>data here</value> node)
                settingNode = XmlConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']").FirstChild;
            }
            catch (Exception)
            {
                settingNode = null;
            }

            // If we have a pointer to an actual XML node, update the value stored there
            if ((settingNode != null))
            {
                if (setProp.Property.SerializeAs.ToString() == "String")
                {
                    settingNode.InnerText = setProp.SerializedValue.ToString();
                }
                else
                {
                    // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                    // the value, otherwise .Net's configuration system complains about the additional declaration.
                    settingNode.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                }
            }
            else
            {
                // If the value did not already exist in this settings file, create a new entry for this setting

                // Search for the application settings node (<Appname.Properties.Settings>) and store it.
                var tmpNode = XmlConfig.SelectSingleNode("//" + _appNode);

                // Create a new settings node and assign its name as well as how it will be serialized
                var newSetting = _xmlDoc.CreateElement("setting");
                newSetting.SetAttribute("name", setProp.Name);

                newSetting.SetAttribute("serializeAs", setProp.Property.SerializeAs.ToString() == "String" ? "String" : "Xml");

                // Append this node to the application settings node (<Appname.Properties.Settings>)
                tmpNode.AppendChild(newSetting);

                // Create an element under our named settings node, and assign it the value we are trying to save
                var valueElement = _xmlDoc.CreateElement("value");
                if (setProp.Property.SerializeAs.ToString() == "String")
                {
                    valueElement.InnerText = setProp.SerializedValue.ToString();
                }
                else
                {
                    // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                    // the value, otherwise .Net's configuration system complains about the additional declaration
                    valueElement.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                }

                //Append this new element under the setting node we created above
                newSetting.AppendChild(valueElement);
            }
        }
    }
}
