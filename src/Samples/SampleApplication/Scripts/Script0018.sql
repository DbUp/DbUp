-- Can't even remember what these columns were meant to be used for...

alter table [dbo].[Entry]
	drop column [IsVisible]
go

alter table [dbo].[Revision]
	drop column [IsVisible]
go

alter table [dbo].[Revision]
	drop column [ChangeSummary]
go

alter table [dbo].[Comment]
	drop column [AuthorCompany]
go

-- 50 was a nice size, but when we import from other blog engines they may have used large URL's
alter table [dbo].[Entry] 
	alter column [Name] nvarchar(100)
go
