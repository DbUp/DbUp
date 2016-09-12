Insert into "Employees" ("Name", "HireDate") Values ('Bill Lumberg', TO_DATE('02/21/1980','MM/DD/YYYY'));
Insert into "Employees" ("Name", "HireDate") Values ('Peter Gibbons', TO_DATE('03/28/1982','MM/DD/YYYY'));
Insert into "Employees" ("Name", "HireDate") Values ('Michael Bolton', TO_DATE('04/08/1984','MM/DD/YYYY'));
Insert into "Employees" ("Name", "HireDate") Values ('Samir Nagheenanajar', TO_DATE('08/25/1986','MM/DD/YYYY'));
Insert into "Employees" ("Name", "HireDate") Values ('Milton Waddams', TO_DATE('01/25/1975','MM/DD/YYYY'));

INSERT INTO "Departments" ("Name", "ManagerId")
SELECT 'Initech Software', "EmployeeId"
FROM "Employees"
WHERE "Name" = 'Bill Lumberg';
