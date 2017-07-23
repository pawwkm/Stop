using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyProduct("Topz Toolchain")]
[assembly: AssemblyCopyright("Copyright © Paw W. K. Møller 2016")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

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

// This version number is the one that .NET uses when linking assemblies.
[assembly: AssemblyVersion("0.1")]

// This is the version of the entire toolchain.
[assembly: AssemblyInformationalVersion("0.1")]