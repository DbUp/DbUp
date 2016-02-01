-- SQLCMD Mode
-- The SQLCMD command-line tool. SQL Server Management Studio (SSMS) and Visual Studio (VS) all provide the
-- ability to preprocess a SQL script. is SSMS and VS, this requires that the query window have SQLCMD-mode 
-- enabled. All of the preprocessing is done these utilities, not SQL Server. This means that SQL Server 
-- will raise an error if any of the preprocessing directives are included in a batch. Consequently,
-- DbUp will recognize these directives and address them before the statement is passed to SQL Server.

-- The next two lines (beginning with ":") are SQLCMD commands. They will be converted to comments by 
-- prepending "-- " to the line. Not that SQLCMD commands are only recognized if the ":" is in column 1. 
-- This should not be a problem since the SQL utilities mentioned above have the same requirement.

:setvar DatabaseName "ThisLineWillBecomeAComment"
:r .\this\line\will\be.commented.sql

-- The folowing contains a SQLCMD-mode variable reference - "$(DatabaseName)". DbUp will recognize these
-- variable references and do variable substitution. Of course, this means that a value must be given to 
-- the variable (in this case, "DatabaseName") be the calling code. Otherwise, DbUp will throw an exception.

if db_id('$(DatabaseName)') is null
	begin
		raiserror('Database $(DatabaseName) does not exist.',16,1) with nowait;
	end;