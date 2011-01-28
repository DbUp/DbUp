-- Ability to enable/disable discussion on certain threads

alter table dbo.Entry
    add IsDiscussionEnabled bit not null default(1)
go
