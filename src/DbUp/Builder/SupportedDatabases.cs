using System;

namespace DbUp.Builder
{
    /// <summary>
    /// Add extension methods to this type if you plan to add support for additional databases.
    /// </summary>
    public class SupportedDatabases
    {
        /// <summary>
        /// Enumeration of supported Databases
        /// </summary>
        public sealed class Type
        {
            #region -= Supported classes =-
            /// <summary>
            /// Int value of Supported type enum
            /// </summary>
            public class TypeValue
            {
                /// <summary>
                /// Microsoft SQL Server int enum
                /// </summary>
                public const int MsSql = 0;
                /// <summary>
                /// Sqlite int enum
                /// </summary>
                public const int Sqlite = 1;
                /// <summary>
                /// Oracle int enum
                /// </summary>
                public const int Oracle = 2;
            }
            #endregion
            private readonly string dbMsName;
            private readonly int value;
            /// <summary>
            /// Microsoft SQL Server
            /// </summary>
            public static readonly Type MsSql = new Type(TypeValue.MsSql, "MsSql");
            /// <summary>
            /// SQLite
            /// </summary>
            public static readonly Type Sqlite = new Type(TypeValue.Sqlite, "Sqlite");
            /// <summary>
            /// Oracle 
            /// </summary>
            public static readonly Type Oracle = new Type(TypeValue.Oracle, "Oracle");

            private Type(int value, String name)
            {
                this.dbMsName = name;
                this.value = value;
            }
            /// <summary>
            /// Returns a string that represents the current SupportedDatabases object.
            /// </summary>
            /// <returns>A string that represents the current SupportedDatabases object.</returns>
            public override String ToString()
            {
                return dbMsName;
            }
            /// <summary>
            /// Get SupportedDatabases Type from int value.
            /// </summary>
            /// <param name="value">Int value for SupportedDatabases Type</param>
            /// <returns>SupportedDatabases Type</returns>
            public static Type Parse(int value)
            {
                if (MsSql.value == value)
                    return MsSql;
                if (Sqlite.value == value)
                    return Sqlite;
                if (Oracle.value == value)
                    return Oracle;
                return null;
            }

            /// <summary>
            /// Get enum int value.
            /// </summary>
            /// <param name="supportedDbType">SupportedDatabases Type</param>
            /// <returns>Return int value of passed SupportedDatabases Type</returns>
            public static implicit operator int(Type supportedDbType)
            {
                return supportedDbType.value;
            }
        }
    }
}