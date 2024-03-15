using System;

namespace DbUp.Engine.Transactions;

/// <summary>
/// makes it possible to set up the TransactionModes allowed for a Database provider
/// </summary>
[Flags]
public enum AllowedTransactionMode
{
    None = 0,

    /// <summary>
    /// Its allowed to run single transactions for an entire run
    /// </summary>
    SingleTransaction = 1,

    /// <summary>
    /// Its allowed to run transactions pr. script
    /// </summary>
    TransactionPerScript = 2,

    /// <summary>
    /// Its allowed to run a single transaction that rolls back at the end
    /// </summary>
    SingleTransactionAlwaysRollback = 4,

    All = SingleTransaction | TransactionPerScript | SingleTransactionAlwaysRollback
}
