-- Create the new tables required for tagging

create table dbo.Tag (
    [Id] [int] identity(1,1) not null constraint PK_Tag_Id primary key,
    [Name] [nvarchar](50) not null
)
go

create table dbo.TagItem (
    [Id] [int] identity(1,1) not null constraint PK_TagItem_Id primary key,
    [TagId] [int] not null constraint FK_TagItem_TagId foreign key references dbo.Tag(Id),
    [EntryId] [int] not null constraint FK_TagItem_EntryId foreign key references dbo.Entry(Id),
)
go

-- Add the status column. The default status is 'Public-Page', since we never had the 
-- concept of Private pages before now
alter table dbo.Entry
	add [Status] nvarchar(20) not null constraint DF_EntryStatus default('Public-Page')
go

-- Posts that had appeared in an RSS feed can be assumed to be blog posts
update dbo.[Entry]
	set [Status] = 'Public-Blog'
	where (Id in (select fi.ItemId from dbo.FeedItem fi));

go

create function [dbo].[SplitTags]
(
    @input nvarchar(500)
)
returns @tags table ( Tag nvarchar(500) )
as
begin
    if @input is null return
    
    declare @iStart int, @iPos int
    if substring( @input, 1, 1 ) = ','
        set @iStart = 2
    else set @iStart = 1
      
    while 1=1
    begin
        set @iPos = charindex( ',', @input, @iStart );
        
        if @iPos = 0 set @iPos = len(@input) + 1;
            
        if (@iPos - @iStart > 0) 
            
            insert into @tags values (replace(lower(ltrim(rtrim(substring( @input, @iStart, @iPos-@iStart )))), ' ', '-'))
            
        set @iStart = @iPos+1
        
        if @iStart > len( @input ) 
            break
    end
    return
end
go


-- Discover a list of all tags from meta keywords
insert into Tag (Name)
    select distinct(tags.Tag) as Name from dbo.[Entry] e
    cross apply dbo.[SplitTags](e.MetaKeywords) as tags

-- Associate new tags with posts
insert into TagItem (TagId, EntryId)
    select 
        (select Id from Tag where Name = tags.Tag) as TagId,
        e.Id as PostId
    from dbo.[Entry] e
    cross apply dbo.[SplitTags](e.MetaKeywords) as tags

-- I normally take care to name constraints, but kept forgetting to do it for defaults, damnit!

declare @defaultConstraintName nvarchar(100)
select @defaultConstraintName = name
    from sys.default_constraints 
    where name like 'DF_%MetaKeywo%'

declare @str nvarchar(200)
set @str = 'alter table dbo.[Entry] drop constraint ' + @defaultConstraintName
exec (@str)

if (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) 
begin
    alter fulltext index on dbo.[Entry] disable
    alter fulltext index on dbo.[Entry] drop (MetaKeywords)
    alter fulltext index on dbo.[Entry] enable
end

alter table dbo.Entry
    drop column MetaKeywords
go

drop table dbo.FeedItem
go

drop table dbo.Feed
go

drop function dbo.SplitTags
