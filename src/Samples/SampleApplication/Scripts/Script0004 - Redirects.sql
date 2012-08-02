-- Settings and Statistics

create table $schema$.Redirect(
    [Id] [int] identity(1,1) not null constraint PK_Redirect_Id primary key,
	[From] [nvarchar](255) not null,
	[To] [nvarchar](255) not null
)
go

