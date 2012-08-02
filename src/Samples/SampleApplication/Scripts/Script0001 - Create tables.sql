-- Creates the following tables:
--  * Entry
--  * Feed
--  * FeedItem
--  * Revision

create table $schema$.[Entry](
	[Id] [int] identity(1,1) not null constraint PK_Entry_Id primary key,
	[Name] [nvarchar](50) not null,
	[Title] [nvarchar](200) not null,
	[Summary] [nvarchar](max) not null,
	[IsVisible] [bit] not null,
	[Published] [datetime] not null,
	[LatestRevisionId] [int] null
)
go

create table $schema$.[Feed](
	[Id] [int] identity(1,1) not null constraint PK_Feed_Id primary key,
	[Name] [nvarchar](100) not null,
	[Title] [nvarchar](255) not null,
)
go

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

create table $schema$.[FeedItem](
	[Id] [int] identity(1,1) not null constraint PK_FeedItem_Id primary key,
	[FeedId] [int] not null,
	[ItemId] [int] not null,
	[SortDate] [datetime] not null,
)
go

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

alter table $schema$.[Revision]  with check add  constraint [FK_Revision_Entry] foreign key([EntryId])
references $schema$.[Entry] ([Id])
go

alter table $schema$.[Revision] check constraint [FK_Revision_Entry]
go

alter table $schema$.[Revision] add  constraint [DF_Revision_RevisionNumber]  default ((0)) FOR [RevisionNumber]
go

alter table $schema$.[FeedItem]  with check add  constraint [FK_FeedItem_Entry] foreign key([ItemId])
references $schema$.[Entry] ([Id])
go

alter table $schema$.[FeedItem] check constraint [FK_FeedItem_Entry]
go

alter table $schema$.[FeedItem]  with check add  constraint [FK_FeedItem_Feed] foreign key([FeedId])
references $schema$.[Feed] ([Id])
go

alter table $schema$.[FeedItem] check constraint [FK_FeedItem_Feed]
go

alter table $schema$.[Comment]  with check add  constraint [FK_Comment_Comment] foreign key([EntryId])
references $schema$.[Entry] ([Id])
go

alter table $schema$.[Comment] check constraint [FK_Comment_Comment]
go
