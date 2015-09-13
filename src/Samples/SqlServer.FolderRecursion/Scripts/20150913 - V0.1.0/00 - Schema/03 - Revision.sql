-- Creates the following tables:
--  * FeedItem

create table $schema$.[Revision](
	[Id] [int] identity(1,1) not null constraint PK_Revision_Id primary key,
	[EntryId] [int] not null,
	[Body] [nvarchar](max) not null,
	[ChangeSummary] [nvarchar](1000) not null,
	[Reason] [nvarchar](1000) not null,
	[Revised] [datetime] not null,
	[Tags] [nvarchar](1000) not null,
	[Status] [int] not null,
	[IsVisible] [bit] not null,
	[RevisionNumber] [int] not null,
)
go

alter table $schema$.[Revision]  with check add  constraint [FK_Revision_Entry] foreign key([EntryId])
references $schema$.[Entry] ([Id])
go

alter table $schema$.[Revision] check constraint [FK_Revision_Entry]
go

alter table $schema$.[Revision] add  constraint [DF_Revision_RevisionNumber]  default ((0)) FOR [RevisionNumber]
go

