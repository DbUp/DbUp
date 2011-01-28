-- Ability to manually set the description and keywords for META tags in the final pages

alter table dbo.Entry
    add MetaDescription nvarchar(500) not null default('')
go

alter table dbo.Entry
    add MetaKeywords nvarchar(500) not null default('')
go
