using System;


namespace DbUp.Oracle
{
    public class OracleCommandReader : IDisposable
    {
        public string Sql { get; set; }

        public OracleCommandReader(string sql)
        {
            Sql = sql;
        }

        public void ReadAllCommands(Action<string> handleCommand)
        {
            var parser = new OracleSqlParser(Sql);

            foreach (var command in parser.Commands)
                handleCommand(command);
        }

        public void Dispose()
        {   
        }
    }
}
