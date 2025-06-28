using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using Wfm.Core.Configuration;

namespace Wfm.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the Wfm engine.
    /// </summary>
    public class EngineContext
    {
        #region Utilities

        /// <summary>
        /// Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <param name="config">Config</param>
        /// <returns>New engine instance</returns>
        protected static IEngine CreateEngineInstance(WfmConfig config)
        {
            if (config != null && !string.IsNullOrEmpty(config.EngineType))
            {
                var engineType = Type.GetType(config.EngineType);
                if (engineType == null)
                    throw new ConfigurationErrorsException("The type '" + config.EngineType + "' could not be found. Please check the configuration at /configuration/wfm/engine[@engineType] or check for missing assemblies.");
                if (!typeof(IEngine).IsAssignableFrom(engineType))
                    throw new ConfigurationErrorsException("The type '" + engineType + "' doesn't implement 'Wfm.Core.Infrastructure.IEngine' and cannot be configured in /configuration/wfm/engine[@engineType] for that purpose.");
                return Activator.CreateInstance(engineType) as IEngine;
            }

            return new WfmEngine();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a static instance of the Wfm factory.
        /// </summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                var config = ConfigurationManager.GetSection("WfmConfig") as WfmConfig;
                if (config == null)
                {
                    //default values as fallback
                    var attributes = new Dictionary<string, string>();
                    attributes.Add("Type", "Wfm.Core.Infrastructure.WfmEngine, Wfm.Core");
                    attributes.Add("DynamicDiscovery", "true");
                    attributes.Add("IgnoreStartupTasks", "true");
                    config = new WfmConfig().Create("Engine", attributes) as WfmConfig;
                }
                Singleton<IEngine>.Instance = CreateEngineInstance(config);
                Singleton<IEngine>.Instance.Initialize(config);
            }
            return Singleton<IEngine>.Instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Wfm engine used to access Wfm services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
