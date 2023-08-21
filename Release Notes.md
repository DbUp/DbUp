**Breaking Changes:**

 - Remove Unsupported Target Frameworks (PR #655)
   - Removed .NET Framework 3.5 and .NET Framework 4.5
   - Added .NET Framework 4.6.2

 - "Microsoft.Data.SqlClient" used for .NET 6 targets (target framework `net6.0`) instead of "System.Data.SqlClient"
 
**Fixes / Changes:**

 - NuGet packages were improved with Source Link and deterministic builds (PR #624)
 - EnsureDatabase.For.PostgresqlDatabase database does not exists error (Issue #619)
 - `dbup-redshift` project was moved to it's own repository at https://github.com/DbUp/dbup-redshift
 - Patches SqlClient security vulnerability (PR #654)