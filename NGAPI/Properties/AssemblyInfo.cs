using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NGAPI")]
[assembly: AssemblyProduct("NGGameSim")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyDescription("Simulation code and public API for writing algorithms.")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("Copyright © NGGameSim Team 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Expopse the internals in this library to the server and client projects
[assembly: InternalsVisibleTo("SimCommon")]
[assembly: InternalsVisibleTo("SimManager")]
[assembly: InternalsVisibleTo("SimViewer")]


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
[assembly: AssemblyVersion("0.0.*")]
