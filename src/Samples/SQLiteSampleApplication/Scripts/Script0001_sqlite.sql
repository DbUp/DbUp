---------------------------------------
-- Script 0001
-- Creates following tables:
--  * Entry
--  * Revision
--  * Comment
--  * Setting
--  * Redirect
--  * Pingback
--  * Tag
--  * TagItem
--  * TaskState
---------------------------------------

-- Entry Table
CREATE TABLE [Entry]
(
	[Id] INTEGER CONSTRAINT 'PK_Entry_ID' PRIMARY KEY AUTOINCREMENT,
	[Name] NVARCHAR(50) NOT NULL,
	[Title] NVARCHAR(200) NOT NULL,
	[Summary] TEXT NOT NULL,
	[Published] DATETIME NOT NULL,
	[LatestRevisionId] INT NULL,
	[IsDiscussionEnabled] BIT NOT NULL CONSTRAINT[DF_Entry_IsDiscussionEnabled] DEFAULT(1), 
	[MetaDescription] NVARCHAR(500) NOT NULL CONSTRAINT[DF_Entry_MetaDescription] DEFAULT(''), 
	[MetaTitle] NVARCHAR(255) NOT NULL CONSTRAINT[DF_Entry_MetaTitle] DEFAULT(''),
	[HideChrome] BIT NOT NULL CONSTRAINT[DF_Entry_HideChrome] DEFAULT(0),
	[Status] NVARCHAR(20) NOT NULL CONSTRAINT [DF_Entry_Status] DEFAULT('Public-Page'),
	[PageTemplate] NVARCHAR(20) NULL,
	[Author] NVARCHAR(100) NOT NULL,
	[RevisionAuthor] NVARCHAR(100) NOT NULL,
	[LastRevised] DATETIME NOT NULL,
	[LatestRevisionFormat] NVARCHAR(20) NOT NULL,
	[TagsCommaSeparated] NVARCHAR(255) NOT NULL CONSTRAINT [DF_Entry_TagsCommaSeparated] DEFAULT(''),
	[CommentCount] INT NOT NULL CONSTRAINT [DF_Entry_CommentCount] DEFAULT(0)
);

-- Entry Revision Table
CREATE TABLE [Revision]
(
	[Id] INTEGER CONSTRAINT 'PK_Revision_ID' PRIMARY KEY AUTOINCREMENT,
	[EntryId] INT NOT NULL,
	[Body] TEXT NOT NULL,
	[Reason] NVARCHAR(1000) NOT NULL,
	[Revised] DATETIME NOT NULL,
	[Status] INT NOT NULL,
	[RevisionNumber] INT NOT NULL CONSTRAINT[DF_Revision_RevisionNumber] DEFAULT 0,
	[Format] NVARCHAR(20) NOT NULL CONSTRAINT[DF_Revision_Format] DEFAULT('Markdown'),
	[Author] NVARCHAR(100) NOT NULL,
	CONSTRAINT 'FK_Revision_EntryId' FOREIGN KEY (EntryId) REFERENCES Entry(Id) 
		ON UPDATE NO ACTION 
		ON DELETE NO ACTION
);

-- Comment Table
CREATE TABLE [Comment]
(
	[Id] INTEGER CONSTRAINT 'PK_Comment_ID' PRIMARY KEY AUTOINCREMENT,
	[Body] TEXT NOT NULL,
	[AuthorName] NVARCHAR(100) NOT NULL,
	[AuthorEmail] NVARCHAR(100) NOT NULL,
	[AuthorUrl] NVARCHAR(100) NOT NULL,
	[Posted] DATETIME NOT NULL,
	[EntryId] INT NOT NULL,
	[Status] INT NOT NULL,
	CONSTRAINT 'FK_Comment_EntryId' FOREIGN KEY (EntryId) REFERENCES Entry(Id)
	    ON UPDATE NO ACTION 
		ON DELETE NO ACTION
);

-- Settings and Statistics Table
CREATE TABLE [Setting]
(
    [Id] INTEGER CONSTRAINT 'PK_Setting_ID' PRIMARY KEY AUTOINCREMENT,
	[Name] NVARCHAR(50) NOT NULL,
	[Description] TEXT NOT NULL,
	[DisplayName] NVARCHAR(200) NOT NULL,
	[Value] TEXT NOT NULL
)
;

-- Redirect Table
CREATE TABLE [Redirect]
(
    [Id] INTEGER CONSTRAINT 'PK_Redirect_ID' PRIMARY KEY AUTOINCREMENT,
	[From] NVARCHAR(255) NOT NULL,
	[To] NVARCHAR(255) NOT NULL
)
;

-- Pingback Table
CREATE TABLE [Pingback]
(
    [Id] INTEGER CONSTRAINT 'PK_Pingback_ID' PRIMARY KEY AUTOINCREMENT,
	[EntryId] INT NOT NULL,
	[TargetUri] NVARCHAR(255) NOT NULL,
	[TargetTitle] NVARCHAR(255) NOT NULL,
	[IsSpam] BIT NOT NULL,
	[Received] DATETIME NOT NULL,
	CONSTRAINT 'FK_Pingback_EntryId' FOREIGN KEY (EntryId) REFERENCES Entry(Id)
		ON UPDATE NO ACTION 
		ON DELETE NO ACTION
)
;

-- Tag Table
CREATE TABLE [Tag]
(
    [Id] INTEGER CONSTRAINT 'PK_Tag_ID' PRIMARY KEY AUTOINCREMENT,
    [Name] NVARCHAR(50) NOT NULL
)
;

-- TagItem Table
CREATE TABLE [TagItem]
(
    [Id] INTEGER CONSTRAINT 'PK_TagItem_ID' PRIMARY KEY AUTOINCREMENT,
    [EntryId] INT NOT NULL,
    [TagId] INT NOT NULL,
    CONSTRAINT 'FK_TagItem_EntryId' FOREIGN KEY (EntryId) REFERENCES Entry(Id)
		ON UPDATE NO ACTION 
		ON DELETE NO ACTION,
    CONSTRAINT 'FK_TagItem_TagId' FOREIGN KEY (TagId) REFERENCES Tag(Id)
		ON UPDATE NO ACTION 
		ON DELETE NO ACTION
)
;

-- TaskState Table (records the status of a long-running task)
CREATE TABLE [TaskState]
(
	[Id] INTEGER CONSTRAINT 'PK_TaskState_ID' PRIMARY KEY AUTOINCREMENT,
	[TaskName] NVARCHAR(50) NOT NULL,
	[Arguments] TEXT NOT NULL,
	[ProgressEstimate] INT,
	[Status] NVARCHAR(30),
	[OutputLog] TEXT NOT NULL,
	[Started] DATETIME NOT NULL,
	[Updated] DATETIME NOT NULL
)
;
