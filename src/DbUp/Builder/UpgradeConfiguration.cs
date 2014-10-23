﻿using System;
using System.Collections.Generic;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Engine;

namespace DbUp.Builder
{
    /// <summary>
    /// Represents the configuration of an UpgradeEngine.
    /// </summary>
    public class UpgradeConfiguration
    {
        private readonly List<IScriptProvider> scriptProviders = new List<IScriptProvider>();
        private readonly List<IScriptPreprocessor> preProcessors = new List<IScriptPreprocessor>();
        private readonly Dictionary<string, string> variables = new Dictionary<string, string>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeConfiguration"/> class.
        /// </summary>
        public UpgradeConfiguration()
        {
            Log = new TraceUpgradeLog();
            VariablesEnabled = true;
        }

        /// <summary>
        /// Manages your database connections, allowing you to control the use of transactions and the behaviour of those transactions
        /// </summary>
        public IConnectionManager ConnectionManager { get; set; }

        /// <summary>
        /// Gets or sets a log which captures details about the upgrade.
        /// </summary>
        public IUpgradeLog Log { get; set; }

        /// <summary>
        /// Gets a mutable list of script providers.
        /// </summary>
        public List<IScriptProvider> ScriptProviders { get { return scriptProviders; } }

        /// <summary>
        /// Gets a mutable list of script pre-processors.
        /// </summary>
        public List<IScriptPreprocessor> ScriptPreprocessors { get { return preProcessors; } }

        /// <summary>
        /// Gets or sets the journal, which tracks the scripts that have already been run.
        /// </summary>
        public IJournal Journal { get; set; }

        /// <summary>
        /// Gets or sets the script executor, which runs scripts against the underlying database.
        /// </summary>
        public IScriptExecutor ScriptExecutor { get; set; }

        /// <summary>
        /// Get or sets the query provider, which holds sql queries.
        /// </summary>
        public SqlStatementsContainer SqlStatementsContainer { get; set; }

        /// <summary>
        /// A collection of variables to be replaced in scripts before they are run
        /// </summary>
        public Dictionary<string, string> Variables
        {
            get { return variables; }
        }

        /// <summary>
        /// Determines if variables should be replaced in scripts before they are run.
        /// </summary>
        public bool VariablesEnabled { get; set; }

        /// <summary>
        /// Ensures all expectations have been met regarding this configuration.
        /// </summary>
        public void Validate()
        {
            if (Log == null) throw new ArgumentException("A log is required to build a database upgrader. Please use one of the logging extension methods");
            if (ScriptExecutor == null) throw new ArgumentException("A ScriptExecutor is required");
            if (Journal == null) throw new ArgumentException("A journal is required. Please use one of the Journal extension methods before calling Build()");
            if (ScriptProviders.Count == 0) throw new ArgumentException("No script providers were added. Please use one of the WithScripts extension methods before calling Build()");
            if (ConnectionManager == null) throw new ArgumentException("The ConnectionManager is null. What do you expect to upgrade?");
            if( SqlStatementsContainer == null) throw new ArgumentException("The QueryProvider is required");
        }

        /// <summary>
        /// Adds variables to the configuration which will be substituted for every script
        /// </summary>
        /// <param name="newVariables">The variables </param>
        public void AddVariables(IDictionary<string, string> newVariables)
        {
            foreach (var variable in newVariables)
            {
                Variables.Add(variable.Key, variable.Value);
            }
        }
    }
}