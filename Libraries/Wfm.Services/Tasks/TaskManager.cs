﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Wfm.Core.Infrastructure;

namespace Wfm.Services.Tasks
{
    /// <summary>
    /// Represents task manager
    /// </summary>
    public partial class TaskManager
    {
        private static readonly TaskManager _taskManager = new TaskManager();
        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();
        private int _largeInterval = 60 * 30;   // 30 minutes

        private TaskManager()
        {
        }

        /// <summary>
        /// Initializes the task manager with the property values specified in the configuration file.
        /// </summary>
        public void Initialize()
        {
            this._taskThreads.Clear();

            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            var scheduleTasks = taskService
                .GetAllTasks()
                .OrderBy(x => x.Seconds)
                .ToList();

            //group by threads with the same seconds
            foreach (var scheduleTaskGrouped in scheduleTasks.GroupBy(x => x.Seconds))
            {
                //create a thread
                var taskThread = new TaskThread
                                     {
                                         Seconds = scheduleTaskGrouped.Key
                                     };
                foreach (var scheduleTask in scheduleTaskGrouped)
                {
                    var task = new ScheduledTask(scheduleTask);
                    taskThread.AddTask(task);
                }
                this._taskThreads.Add(taskThread);
            }

            //sometimes a task period could be set to several hours (or even days).
            //in this case a probability that it'll be run is quite small (an application could be restarted)
            //we should manually run the tasks which weren't run for a long time
            var utcNow = DateTime.UtcNow;
            utcNow = utcNow.AddTicks(-(utcNow.Ticks % 10000000));   // round down to second (remove milliseconds)
            var notRunTasks = scheduleTasks.Where(x => x.Seconds >= _largeInterval)
                .Where(x => !x.LastStartUtc.HasValue || x.LastStartUtc.Value.AddSeconds(_largeInterval) < utcNow);

            //create a thread for the tasks which weren't run for a long time
            var expiredTasks = notRunTasks.Where(x => !x.LastStartUtc.HasValue || (((int)(utcNow - x.LastStartUtc.Value).TotalSeconds) / x.Seconds > 0));
            if (expiredTasks.Any())
            {
                var taskThread = new TaskThread
                {
                    RunOnlyOnce = true,
                    Seconds = 60 * 5    // let's run such tasks in 5 minutes after application start
                };
                foreach (var scheduleTask in expiredTasks)
                {
                    var task = new ScheduledTask(scheduleTask);
                    taskThread.AddTask(task);
                }
                this._taskThreads.Add(taskThread);
            }

            // create one-time thread for each of expiring tasks
            var expiringTasks = notRunTasks.Where(x => x.LastStartUtc.HasValue && (((int)(utcNow - x.LastStartUtc.Value).TotalSeconds) / x.Seconds == 0));
            foreach (var scheduleTask in expiringTasks)
            {
                var taskThread = new TaskThread()
                {
                    RunOnlyOnce = true,
                    Seconds = scheduleTask.Seconds - (int)(utcNow - scheduleTask.LastStartUtc.Value).TotalSeconds
                };
                taskThread.AddTask(new ScheduledTask(scheduleTask));
                this._taskThreads.Add(taskThread);
            }
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in this._taskThreads)
            {
                taskThread.Dispose();
            }
        }

        /// <summary>
        /// Gets the task mamanger instance
        /// </summary>
        public static TaskManager Instance
        {
            get
            {
                return _taskManager;
            }
        }

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        public IList<TaskThread> TaskThreads
        {
            get
            {
                return new ReadOnlyCollection<TaskThread>(this._taskThreads);
            }
        }
    }
}
