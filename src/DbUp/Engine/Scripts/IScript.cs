using System;
using DbUp.Builder;

namespace DbUp.Engine
{
    public interface IScript
    {
        /// <summary>
        /// Gets the name of the script.
        /// </summary>
        /// <value></value>
        string Name { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        void Execute(UpgradeConfiguration configuration);
    }
}