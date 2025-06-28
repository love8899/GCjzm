using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Infrastructure;


namespace Wfm.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //initialize engine context
            EngineContext.Initialize(false);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ScheduledTaskService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
