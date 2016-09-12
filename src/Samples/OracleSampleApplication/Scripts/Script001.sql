﻿CREATE TABLE "Employees" 
   (	"EmployeeId" NUMBER(10,0) NOT NULL ENABLE, 
	"Name" VARCHAR2(255), 
	"HireDate" DATE NOT NULL ENABLE, 
	"Location" VARCHAR2(255), 
	 CONSTRAINT "PK_Employees" PRIMARY KEY ("EmployeeId"));

CREATE SEQUENCE "SQ_Employees" 
	MINVALUE 1 
	MAXVALUE 9999999999999999999999999999 
	INCREMENT BY 1 START WITH 1 CACHE 20 
	NOORDER 
	NOCYCLE;

 CREATE OR REPLACE TRIGGER "TR_Employees"
	BEFORE INSERT ON "Employees"
    FOR EACH ROW 
	BEGIN
		SELECT "SQ_Employees".nextval INTO 
		:new."EmployeeId" 		
		FROM dual; 
	END; 
/

 ALTER TRIGGER "TR_Employees" ENABLE;
