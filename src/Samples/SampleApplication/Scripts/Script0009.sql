alter table dbo.Entry
    add MetaTitle nvarchar(255) not null default('')
go

update dbo.Entry set MetaTitle = Title
go
