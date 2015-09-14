-- Creates the following tables:
--  * Comment

create table $schema$.[Comment](
	[Id] [int] identity(1,1) not null constraint PK_Comment_Id primary key,
	[Body] [nvarchar](max) not null,
	[AuthorName] [nvarchar](100) not null,
	[AuthorCompany] [nvarchar](100) not null,
	[AuthorEmail] [nvarchar](100) not null,
	[AuthorUrl] [nvarchar](100) not null,
	[Posted] [datetime] not null,
	[EntryId] [int] not null,
	[Status] int not null
)
go

alter table $schema$.[Comment]  with check add  constraint [FK_Comment_Comment] foreign key([EntryId])
references $schema$.[Entry] ([Id])
go

alter table $schema$.[Comment] check constraint [FK_Comment_Comment]
go
