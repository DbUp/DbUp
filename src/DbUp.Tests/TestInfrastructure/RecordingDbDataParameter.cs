using System;
using System.Data;

namespace DbUp.Tests.TestInfrastructure
{
    internal class RecordingDbDataParameter : IDbDataParameter
    {
        public DbType DbType { get; set; }
        public ParameterDirection Direction { get; set; }
        public bool IsNullable { get; private set; }
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public int Size { get; set; }

        public override string ToString()
        {
            return string.Format("{0}={1}", ParameterName, Value);
        }
    }
}