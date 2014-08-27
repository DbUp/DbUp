-- Settings and Statistics

--insert into Entry values()
--/
create table FOOBAR(
	ID INTEGER GENERATED ALWAYS AS IDENTITY (start with 1 increment by 1 nocycle),
    NAME VARCHAR2(50) NOT NULL
)
/
create table FOOBAR2(
	ID INTEGER GENERATED ALWAYS AS IDENTITY (start with 1 increment by 1 nocycle),
    NAME VARCHAR2(50) NOT NULL
)
/
