using System;
using System.Collections.Generic;
using System.Timers;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Ssh;

namespace Masters_2025_MSS_621_JW
{
    public class EventTimers : IDisposable // see the dispose method below for why this is IDisposable
    {
        private const int OneMinute = 60 * 1000; // milliseconds in one minute

        /* This is a quick and dirty timer system to run scheduled events. we have a timer that runs a chunk of code
         * that looks at the time at that moment and compares it to a time we specified, if equal it triggers the
         * method we specified.    This is great for daily reoccurring tasks like out night off sweep
         * This acts exactly like the Simpl WHEN symbol.  it will not trigger if you reboot the processor
         * and it missed the event horizon. 
         */

        private readonly Timer _myTimer;
        private Dictionary<string, List<Action>> callbacks = new Dictionary<string, List<Action>>();

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

        public void Dispose()
        {
            // In C# it seems that timers do not get disposed of in the garbage collector.
            //  So we make this class Idisposable and add in code to clean it up.
            _myTimer.Stop(); // Stop the timer now, if you don't, it may run for a little while after dispose is called.
            _myTimer.Dispose();
        }

        public void scheduleDaily(string when, Action callback) {
            if (!callbacks.ContainsKey(when)) {
                callbacks[when] = new List<Action>();
            }
            callbacks[when].Add(callback);
        }

        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now.ToString("HH:mm"); // convert to a nice formatted string to make it easy to compare

            if (callbacks.ContainsKey(now)) {
                foreach (var callback in callbacks[now]) {
                    try {
                        callback();
                    } catch (Exception ex) {
                        CrestronConsole.PrintLine("Exception in scheduled callback: " + ex);
                    }
                }
            }
        }
    }
}