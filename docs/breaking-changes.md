# 4.0.0
 - If `builder.LogTo..()` is called multiple times, the logs are combined instead of replacing the previous logger
 - Default Encoding is now UTF rather than Encoding.Default
 - AdHocSqlRunner changed to use Expression<Func<>> rather than Func<>
 - EmbeddedScriptAndCodeProvider (used by the extension method WithScriptsAndCodeEmbeddedInAssembly) now also applies the given filter to the code based scripts