-- This table records the status of a long-running task
create table dbo.TaskState (
	Id int identity not null constraint PK_LongRunningTask_Id primary key,
	TaskName nvarchar(50) not null,
	Arguments nvarchar(max) not null,
	ProgressEstimate int,
	[Status] nvarchar(30),
	OutputLog nvarchar(max) not null,
	Started datetime not null,
	Updated datetime not null
)
go
