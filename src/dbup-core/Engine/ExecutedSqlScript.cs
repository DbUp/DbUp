using System;

namespace DbUp.Engine
{
    public class ExecutedSqlScript
    {

        public string Hash { get; set; }
        public string Contents { get; set; }
        public string Name { get; set; }
        public DateTime Applied { get; set; }
        public ExecutedSqlScript()
        {
            
        }
    }
}