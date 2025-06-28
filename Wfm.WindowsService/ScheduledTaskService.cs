using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Data;
using Wfm.Services.Tasks;

namespace Wfm.WindowsService
{
    public partial class ScheduledTaskService : ServiceBase
    {
        public ScheduledTaskService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // degbug
            //System.Diagnostics.Debugger.Launch();

            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            //start scheduled tasks
            if (databaseInstalled)
            {
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }
        }

        protected override void OnStop()
        {
            TaskManager.Instance.Stop();
        }
    }
}
