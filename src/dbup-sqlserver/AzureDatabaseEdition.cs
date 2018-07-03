using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DbUp.SqlServer
{
    /// <summary>
    /// Azure SQL Instance Edition for Database Creation
    /// </summary>
    public enum AzureDatabaseEdition
    {
        /// <summary>
        /// Not an Azure SQL Instance
        /// </summary>
        None = 0,
        /// <summary>
        /// Basic Azure SQL Instance
        /// </summary>
        Basic,
        /// <summary>
        /// Standard Azure SQL Instance
        /// </summary>
        Standard,
        /// <summary>
        /// Premium Azure SQL Instance
        /// </summary>
        Premium
    }
}
