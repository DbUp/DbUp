---------------------------------------
-- Script 0003
-- Creates Sql Authentication Tables
---------------------------------------

-- User Table
CREATE TABLE [User] (
	[Id] INTEGER CONSTRAINT 'PK_User_ID' PRIMARY KEY AUTOINCREMENT,
	[Name] NVARCHAR(100) NOT NULL,
	[Username] NVARCHAR(50) NOT NULL,
	[Password] NVARCHAR(50) NOT NULL,
	[Email] NVARCHAR(50) NOT NULL
)
;

-- Role Table
CREATE TABLE [Role] (
	[Id] INTEGER CONSTRAINT 'PK_Role_ID' PRIMARY KEY AUTOINCREMENT,
	[Name] NVARCHAR(50) NOT NULL
)
;

-- User Roles Table
CREATE TABLE [UserRoles] (
	[UserId] INT NOT NULL,
	[RoleId] INT NOT NULL,
	CONSTRAINT 'FK_UserRoles_ID' PRIMARY KEY (UserId, RoleId),
	CONSTRAINT 'FK_UserRoles_UserId' FOREIGN KEY (UserId) REFERENCES User(Id),
	CONSTRAINT 'FK_UserRoles_RoleId' FOREIGN KEY (RoleId) REFERENCES Role(Id)
)
;
