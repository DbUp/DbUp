using System;
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
}

