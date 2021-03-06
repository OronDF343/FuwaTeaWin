﻿using System.Reflection;
using System.Runtime.InteropServices;
using FuwaTea.Metadata;
using ModularFramework.Attributes;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FuwaTea.Metadata")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("FuwaTea.Metadata")]
[assembly: AssemblyCopyright("Copyright © 2015 OronDF343")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Modular
[assembly: ModuleDefinition("MetadataLoaders", typeof(MetadataLoaderAttribute), typeof(IMetadataLoader))]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("17853f95-1a56-448c-825e-5662e1fc1426")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]
