using System;
using System.Data;
using DbUp.Builder;

namespace DbUp.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SqlScriptGeneratedAtRuntimeBase : CodeScriptBase
    {
        public string Name
        {
            get { return this.GetType().FullName + ".cs"; }
        }

        public override void Execute(UpgradeConfiguration configuration)
        {
            new SqlScript(Name, ProvideScript(configuration.ConnectionFactory())).Execute(configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string ProvideScript(IDbConnection connection);
    }
}