using System;
using System.Collections.Generic;
using System.Timers;
using Independentsoft.Exchange;
using MessageBroker;

namespace Masters2025_MSS621
{
    /*
     * This is the same class as the EventTimers class but is leveraging a list so that this can be used for multiple
     * events without havign to edit the class code.
     * Simply add an event to the TimedEventlist and every minute it will check through the whole list for a match and trigger 
     * the message broker with the message name you specify.
     * 
     * Why use a class to hold the information in a list instead of a dictionary?
     * First a dictionary only allows a single instance of a key so if we keyed on the time only one thing can happen at that time
     * If we indexed on the message then only one use of that message is allowed.
     * by using an object I could specify 100 events at the same time or multiple events to triggert he same message.
     * By using an object we can now expand it's capability. instead of just sending a message how about sending information
     * with that message? this could be expanded to do exactly that.
     */

    public class SimpleEventTimers2 : IDisposable // see the dispose method below for why this is IDisposable
    {

        public List<Event> TimedEvent = new List<Event>();
        private readonly Timer _myTimer;
        private const int OneMinute = 60 * 1000; // milliseconds in one minute

        public SimpleEventTimers2() 
        {
            _myTimer = new Timer(OneMinute);
            _myTimer.Elapsed += MyTimer_Elapsed;
            _myTimer.Enabled = true;
            _myTimer.AutoReset = true;
            _myTimer.Start();
        }
        public void Dispose()
        {
            // In C# it seems that timers do not get disposed of in the garbage collector.
            //  So we make this class Idisposable and add in code to clean it up.
            _myTimer.Stop(); // Stop the timer now, if you don't, it may run for a little while after dispose is called.
            _myTimer.Dispose();
            TimedEvent.Clear();
        }

        /// <summary>
        /// Adds an event to the simple Timer class that will be checked every minute
        /// </summary>
        /// <param name="TimeOFEvent"> Time to trigger in HH:MM string format</param>
        /// <param name="Message">Message that will be sent to the Message Broker to trigger any subscribors for that message</param>
        public void AddEvent(string TimeOfEvent, string Message)
        {
            TimedEvent.Add(new Event { Message = "SystemOff", Time = "22:00" });
        }
        
        /// <summary>
        /// Deletes a timed event at the specified index
        /// </summary>
        /// <param name="Index">Index number of the timer in the list you would like to delete</param>
        public void DeleteEvent(int Index)
        {
            if( Index < TimedEvent.Count && Index >= 0 ) 
                TimedEvent.RemoveAt(Index);
        }

        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now.ToString("HH:mm"); // convert to a nice formatted string to make it easy to compare

            foreach (Event i in TimedEvent)
            {
                if (i.Time == now)
                    Broker.SendMessage(i.Message, new MessageBroker.Message());
            }
        }
        public class Event
        {
            public string Time;
            public string Message;
        }
    }
}
