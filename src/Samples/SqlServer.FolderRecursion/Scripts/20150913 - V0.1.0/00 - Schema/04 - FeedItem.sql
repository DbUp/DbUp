-- Creates the following tables:
--  * FeedItem

create table $schema$.[FeedItem](
	[Id] [int] identity(1,1) not null constraint PK_FeedItem_Id primary key,
	[FeedId] [int] not null,
	[ItemId] [int] not null,
	[SortDate] [datetime] not null,
)
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
