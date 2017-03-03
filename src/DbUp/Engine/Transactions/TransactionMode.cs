namespace DbUp.Engine.Transactions
{
    /// <summary>
    /// The transaction strategy to use
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// Run creates a new connection for each script, without a transaction
        /// </summary>
        NoTransaction,

        /// <summary>
        /// DbUp will run using a single transaction for the whole upgrade operation
        /// </summary>
        SingleTransaction,
        
        /// <summary>
        /// DbUp will create a new connection and transaction per script
        /// </summary>
        TransactionPerScript
    }
}