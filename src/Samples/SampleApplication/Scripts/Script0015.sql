alter table dbo.Revision
    add Format nvarchar(20) not null default('Markdown')
go

alter table dbo.Entry
    add HideChrome bit not null default(0)
go
