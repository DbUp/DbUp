using System;
using System.Data;
using DbUp.Builder;

namespace DbUp.Engine
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CodeScriptBase : IScript
    {
        public string Name
        {
            get { return this.GetType().FullName + ".cs"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public abstract void Execute(UpgradeConfiguration configuration);
    }



    /// <summary>
    /// 
    /// </summary>
    public abstract class SqlScriptGeneratedAtRuntimeBase : IScript
    {
        public string Name
        {
            get { return this.GetType().FullName + ".cs"; }
        }

        public void Execute(UpgradeConfiguration configuration)
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