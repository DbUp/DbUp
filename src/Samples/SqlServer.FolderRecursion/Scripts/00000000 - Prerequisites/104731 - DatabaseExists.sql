if db_id('$(DatabaseName') is null
	begin
		raiserror('Database $(DatabaseName) does not exist.',16,1) with nowait;
	end;