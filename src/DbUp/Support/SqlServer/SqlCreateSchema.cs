using System;
using DbUp.Engine;

namespace DbUp.Support.SqlServer
{
    /// <summary>
    /// Represents a sql script for the creation of a new schema (table)
    /// </summary>
    public class SqlCreateSchema : ICreateSchema
    {
        /// <summary>
        /// Sql script for creating a new schema
        /// </summary>
        /// <param name="schema"></param>
        /// <returns>Sql for creating a new schema (table)</returns>
        public string Command(string schema)
        {
            return string.Format(
                @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{0}') Exec('CREATE SCHEMA [{0}]')", schema);
        }
    }
}
