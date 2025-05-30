﻿using System;
using System.Timers;
using Crestron.SimplSharp;
using Masters_2024_MSS_521.MessageSystem;

namespace Masters_2024_MSS_521
{
    /// <summary>
    /// This is a quick and dirty timer system to run scheduled events. we have a timer that runs a chunk of code
    /// that looks at the time at that moment and compares it to a time we specified, if equal it triggers the
    /// method we specified. This is great for daily reoccurring tasks like out night off sweep
    /// This acts exactly like the Simpl WHEN symbol.  it will not trigger if you reboot the processor
    /// and it missed the event horizon.
    /// </summary>
    /// <remarks>
    /// This class is IDisposable. In C# it seems that timers do not get disposed of in the garbage collector.
    /// So we make this class Idisposable and add in code to clean it up.
    /// </remarks>
    public class EventTimers : IDisposable // 
    {
        private const int OneMinute = 60 * 1000; // milliseconds in one minute

        private readonly Timer _myTimer;

        /// <summary>
        /// Constructor which sets up the class and initializes a timer.
        /// </summary>
        public EventTimers()
        {
            // Quick and dirty we set up a system.timer to run for one minute
            _myTimer = new Timer(OneMinute);
            _myTimer.Elapsed += MyTimer_Elapsed;
            _myTimer.Enabled = true;
            _myTimer.AutoReset = true;
            _myTimer.Start();
            CrestronConsole.PrintLine("Timer Setup complete");
        }

        /// <summary>
        /// In C# it seems that timers do not get disposed of in the garbage collector.
        /// So we make this class Idisposable and add in code to clean it up.
        /// </summary>
        public void Dispose()
        {
            _myTimer.Stop(); // Stop the timer now, if you don't, it may run for a little while after dispose is called.
            _myTimer.Dispose();
        }

        /// <summary>
        /// Callback method which is called when a timer concludes
        /// </summary>
        /// <param name="sender">Timer object that has had its time elapse</param>
        /// <param name="e">Arguments with respect to the elapsed timer</param>
        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now.ToString("HH:mm"); // convert to a nice formatted string to make it easy to compare

            if (now == "22:00") // 10 pm
            {
                MessageBroker.SendMessage("SystemOff", new Message());
            }
        }
    }
}