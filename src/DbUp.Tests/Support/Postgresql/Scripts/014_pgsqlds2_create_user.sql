
-- pgsqlds2_create_db.sql: DVD Store Database Version 2.1 Build Script - Postgres version
-- Copyright (C) 2011 Vmware, Inc. 
-- Last updated 2/13/11


-- Database

CREATE USER web WITH PASSWORD 'web' ;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO WEB;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO WEB;
GRANT ALL PRIVILEGES ON SCHEMA public TO WEB;
