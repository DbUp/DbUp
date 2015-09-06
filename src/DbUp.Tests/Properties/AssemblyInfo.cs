using System.Reflection;
using System.Runtime.InteropServices;
using ApprovalTests.Reporters;

[assembly: AssemblyTitle("DbUp")]
[assembly: AssemblyDescription("")]
[assembly: Guid("9f833e49-6e35-4e4d-b2a0-3d4fed527c89")]

[assembly: UseReporter(typeof(DiffReporter))]