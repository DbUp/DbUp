﻿using System;
using System.Collections.Generic;

namespace DbUp.Engine
{
    /// <summary>
    /// Represents the results of a database upgrade.
    /// </summary>
    public sealed class DatabaseUpgradeResult
    {
        private readonly List<SqlScript> scripts;
        private readonly bool successful;
        private readonly Exception error;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseUpgradeResult"/> class.
        /// </summary>
        /// <param name="scripts">The scripts that were executed.</param>
        /// <param name="successful">if set to <c>true</c> [successful].</param>
        /// <param name="error">The error.</param>
        public DatabaseUpgradeResult(IEnumerable<SqlScript> scripts, bool successful, Exception error)
        {
            this.scripts = new List<SqlScript>();
            this.scripts.AddRange(scripts);
            this.successful = successful;
            this.error = error;
        }

        /// <summary>
        /// Gets the scripts that were executed.
        /// </summary>
        public IEnumerable<SqlScript> Scripts
        {
            get { return scripts; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DatabaseUpgradeResult"/> is successful.
        /// </summary>
        public bool Successful
        {
            get { return successful; }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public Exception Error
        {
            get { return error; }
        }
    }
}