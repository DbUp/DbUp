namespace DbUp.Engine.Transactions
{
    /// <summary>
    /// The transaction strategy to use
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// DbUp will run scripts without a transaction
        /// </summary>
        NoTransaction,

        /// <summary>
        /// DbUp will run scripts using a single transaction for the whole upgrade operation
        /// </summary>
        SingleTransaction,

        /// <summary>
        /// DbUp will run scripts using a separate transaction per script
        /// </summary>
        TransactionPerScript,

        /// <summary>
        /// DbUp will run scripts using a single transaction for the whole upgrade operation but will rollback at the end
        /// </summary>
        SingleTransactionAlwaysRollback
    }
}
