declare @hasFullText bit
select @hasFullText = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')
if (@hasFullText = 1)
begin
	exec sp_fulltext_catalog 'FTCatalog', 'create' 
	exec sp_fulltext_table 'Entry', 'create', 'FTCatalog', 'PK_Entry_Id' 
	exec sp_fulltext_column 'Entry', 'Name', 'add', 0x0409
	exec sp_fulltext_column 'Entry', 'Title', 'add', 0x0409
	exec sp_fulltext_column 'Entry', 'Summary', 'add', 0x0409
	exec sp_fulltext_column 'Entry', 'MetaDescription', 'add', 0x0409
	exec sp_fulltext_column 'Entry', 'MetaKeywords', 'add', 0x0409
	exec sp_fulltext_table 'Entry', 'activate'
	exec sp_fulltext_catalog 'FTCatalog', 'start_full' 
	exec sp_fulltext_table 'Entry', 'start_change_tracking'
    exec sp_fulltext_table 'Entry', 'start_background_updateindex'
end
