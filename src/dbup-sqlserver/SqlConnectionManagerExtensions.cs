using System;
using DbUp.Builder;
using DbUp.SqlServer;

public static class SqlConnectionManagerExtensions
{
    public static UpgradeEngineBuilder WithNewSqlParser(this UpgradeEngineBuilder builder)
    {
        builder.Configure(c =>
        {
            var connMgr = (c.ConnectionManager as SqlConnectionManager)
                            ?? throw new NotSupportedException("New SQL Parser is only supported when using the SqlConnectionManager class");

            connMgr.CommandSplitter = new TSqlCommandSplitter();
        });

        return builder;
    }
}
