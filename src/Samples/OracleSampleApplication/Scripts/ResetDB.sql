/* 
	This script will reset the sample database.
*/
drop table "DBUPUSER"."_SchemaVersions" cascade constraints PURGE;
drop table "DBUPUSER"."Departments" cascade constraints PURGE;
drop table "DBUPUSER"."Employees" cascade constraints PURGE;

drop sequence "DBUPUSER"."SQ_SchemaVersions";
drop sequence "DBUPUSER"."SQ_Departments";
drop sequence "DBUPUSER"."SQ_Employees";