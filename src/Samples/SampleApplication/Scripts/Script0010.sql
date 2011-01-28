declare @hasFullText bit
select @hasFullText = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')
if (@hasFullText = 1)
begin
	exec sp_fulltext_table 'Revision', 'create', 'FTCatalog', 'PK_Revision_Id' 
	exec sp_fulltext_column 'Revision', 'Body', 'add', 0x0409
	exec sp_fulltext_table 'Revision', 'activate'
	exec sp_fulltext_catalog 'FTCatalog', 'start_full' 
	exec sp_fulltext_table 'Revision', 'start_change_tracking'
    exec sp_fulltext_table 'Revision', 'start_background_updateindex'
end
