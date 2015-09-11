DbUp v3.0 added support for transactions. Not all changes can be rolled back, but for many DDL and data changes transactions work fine. Out of the box there are 3 strategies:

* No Transactions (**default**) - `builder.WithoutTransaction()`
* Transaction per script - `builder.WithTransactionPerScript()`
* Single transaction - `builder.WithTransaction()`
