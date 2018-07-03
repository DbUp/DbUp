DbUp v3.0 added support for transactions. Not all changes can be rolled back, but for many DDL and data changes on databases other than MySql transactions work fine. Out of the box there are 3 strategies:

* No Transactions (**default**) - `builder.WithoutTransaction()`
* Transaction per script - `builder.WithTransactionPerScript()`
* Single transaction - `builder.WithTransaction()`

For more details about what statements can and cannot be executed in a transaction see:
- This [article](http://wiki.postgresql.org/wiki/Transactional_DDL_in_PostgreSQL:_A_Competitive_Analysis) has a good overview of many popular databases
- [Sql Server](https://docs.microsoft.com/en-us/sql/t-sql/language-elements/transactions-sql-data-warehouse#limitations-and-restrictions)
- [MySql](https://dev.mysql.com/doc/refman/5.7/en/implicit-commit.html)