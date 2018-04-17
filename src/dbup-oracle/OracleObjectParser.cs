using System;
using System.Collections.Generic;
using System.Text;
using DbUp.Support;

namespace DbUp.Oracle
{
    public class OracleObjectParser : SqlObjectParser
    {
        public OracleObjectParser() : base("\"", "\"")
        {
        }
    }
}
