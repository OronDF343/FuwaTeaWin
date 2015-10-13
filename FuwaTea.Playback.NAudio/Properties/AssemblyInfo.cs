using System.Reflection;
using System.Runtime.InteropServices;
using FuwaTea.Playback.NAudio.Codecs;
using ModularFramework.Attributes;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FuwaTea.Playback.NAudio")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("OronDF343")]
[assembly: AssemblyProduct("FuwaTea.Playback.NAudio")]
[assembly: AssemblyCopyright("Copyright © 2015 OronDF343")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Modular
[assembly: ModuleDefinition("NAudio", typeof(CodecAttribute), typeof(IWaveStreamProvider))]
[assembly: ModuleImplementation("Players")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c6003f63-4717-4b45-8890-b3c02f7209d9")]

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
