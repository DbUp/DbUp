-- Creates the following tables:
--  * Feed

create table $schema$.[Feed](
	[Id] [int] identity(1,1) not null constraint PK_Feed_Id primary key,
	[Name] [nvarchar](100) not null,
	[Title] [nvarchar](255) not null,
)
go

