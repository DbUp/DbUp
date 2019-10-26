using System;
using System.Collections.Generic;
using DbUp.Engine;

namespace DbUp.Builder
{
    /// <summary>
    /// Builds a UpgradeEngine by accepting a list of callbacks to execute. For custom configuration, you should 
    /// implement extension methods on top of this class.
    /// </summary>
    public class UpgradeEngineBuilder
    {
        readonly List<Action<UpgradeConfiguration>> callbacks = new List<Action<UpgradeConfiguration>>();

        /// <summary>
        /// Adds a callback that will be run to configure the upgrader when Build is called.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void Configure(Action<UpgradeConfiguration> configuration)
        {
            callbacks.Add(configuration);
        }

        /// <summary>
        /// Creates an UpgradeConfiguration based on this configuration.
        /// </summary>
        /// <returns></returns>
        public UpgradeConfiguration BuildConfiguration()
        {
            var config = new UpgradeConfiguration();
            foreach (var callback in callbacks)
            {
                callback(config);
            }

            config.Validate();

            return config;
        }

        /// <summary>
        /// Creates an UpgradeEngine based on this configuration.
        /// </summary>
        /// <returns></returns>
        public UpgradeEngine Build()
        {
            var config = BuildConfiguration();
            return new UpgradeEngine(config);
        }
    }
}