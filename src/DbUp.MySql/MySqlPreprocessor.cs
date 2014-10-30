using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUp.Engine;

namespace DbUp.MySql
{
    public class MySqlPreprocessor : IScriptPreprocessor
    {
        public string Process(string contents)
        {
            return contents;
        }
    }
}
