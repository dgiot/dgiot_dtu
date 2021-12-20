// <copyright file="TimerHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;

namespace Dgiot_dtu
{
    public class TimerHelper
    {
        private TimerHelper()
        {
        }

        private static TimerHelper instance;
        private static Dictionary<string, Timer> timers = new Dictionary<string, Timer> { };

        public static TimerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new TimerHelper();
            }

            return instance;
        }

        public static void Start(string key, object state = null, int period = 1000, int dueTime = Timeout.Infinite )
        {
            if (timers.ContainsKey(key))
            {
                try
                {
                    timers[key].Change(0, 1000);
                }
                catch
                {
                }
            }

            TimerCallback callback = new TimerCallback(ReadValue);

            Timer timer = new Timer(callback, state, dueTime, period);
            timer.InitializeLifetimeService();
            timers.Add(key, timer);
        }

        public static void Stop(string key)
        {
            if (timers.ContainsKey(key))
            {
                try
                {
                    timers[key].Change(0, 1000);
                }
                catch
                {
                }
            }
        }

        public static void ReadValue(object state)
        {
            LogHelper.Log("GetState " + state.ToString());
            GC.Collect();
        }
    }
}