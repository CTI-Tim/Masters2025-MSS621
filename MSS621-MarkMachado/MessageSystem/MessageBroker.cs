using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Independentsoft.Exchange.Autodiscover; // Only need this for CrestronConsole.PrintLine()

namespace Masters_2024_MSS_521.MessageSystem
{
    /// <summary>
    /// Why do we need this?   the reason is Programming Context.   Class A can not talk to anything inside Class B
    /// or is even aware of class B existing unless we program class A to know about class B and then pass a reference to it.
    /// This forces you to start making all your classes not only coupled but tightly coupled. In some instances that is not a
    /// big deal, but when your goal is not having to re write classes or drivers, etc over and over and modify them for each
    /// use case, it becomes a problem.  Being able to create a messaging system that can send updates to a central class
    /// that every single class in the program can see (static) means class A only needs to know about the broker
    /// and the message format. Now Class A can trigger something in class B without knowing anything or even
    /// able to see class B. There are far more complex message brokers out there.
    /// </summary>
    /// <remarks>
    /// This one is intentionally to be as simple as possible and only using delegates. an advantage of c# delegates
    /// is that they are async by default as the mechanics in them use async and await in the background.
    /// REMEMBER! Your code called is called synchronously and any blocking code will cause delays and performance issues.
    /// We need to create a delegate signature for our messaging we we are going to pass the message class defined in Message.cs
    /// Then we create a dictionary that lets you set a string key (name) and a delegate. These are going to store
    /// the delegates we create to basically create our messaging system.
    /// </remarks>
    internal static class MessageBroker
    {
        /// <summary>
        /// The delegate definition used for the MessageBroker system.
        /// </summary>
        /// <param name="m">Message parameter which is passed in to the delegates at creation time</param>
        public delegate void MessageDelegate(Message m);

        /// <summary>
        /// Dictionary which essentially is the MessageBroker system
        /// </summary>
        private static readonly Dictionary<string, MessageDelegate> Messages =
            new Dictionary<string, MessageDelegate>();

        /// <summary>
        /// Message debug boolean
        /// </summary>
        private static bool messageDebug;
        /*
         * Note: if you use resharper you will get a notice that the above should be readonly.   This is an example of a "style"
         * rule that makes code less readable and is a bad idea in some cases like this one.
         *
         *  For those curious as to why: When a reference type is declared as readonly, the pointer is immutable, but not the object it points to
         *  This means that a reference type data member can be initialized in order to point to an instance of a class,
         *  but once this is done it's not possible to make it point to another instance of a class outside
         *  of constructors anyways, therefore the the readonly modifier has no effect on the object the readonly data member points to.
         *  However as the object the readonly field points to can have it’s state change, marking a field as readonly
         *  can be misleading at times. So think about if it helps the reader of your code understand your design or not.
         */

        // We are not going to use a default constructor because we do not need one.
        // You do not have to create a default constructor if you do not have a use for one.

        /// <summary>
        /// Method which facilitates adding to the MessageBroker system.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <exception cref="Exception"></exception>
        public static void AddDelegate(string key, MessageDelegate method)
        {
            if (!Messages.ContainsKey(key)) // we do not want duplicate keys.
                Messages.Add(key, method);
            else
                throw new Exception($"## Duplicate key attempted in Message Broker Class  key {key} already exists ##");
            // I throw and exception here because this would be a critical error and cause problems.
        }
        /// <summary>
        /// Method which allows for the removal of items added to the MessageBroker system.
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveDelegate(string key)
        {
            if (Messages.ContainsKey(key))
                Messages.Remove(key);

            // Not throwing an Exception here because not deleting something that does not exist will not cause any harm
        }

        /// <summary>
        /// Method for sending messages via the MessageBroker system.
        /// </summary>
        /// <param name="key">Dictionary key value used for the "lookup"</param>
        /// <param name="m">Message to be sent to the recipient within the program</param>
        public static void SendMessage(string key, Message m)
        {
            DebugMessage(key);

            if (Messages.ContainsKey(key))
            {
                Messages[key].DynamicInvoke(m);
            }
            else
            {
                string errmsg = $"### Message sent to non existing key named {key,-25} ###";
                ErrorLog.Error(errmsg);
                if (messageDebug) CrestronConsole.PrintLine(errmsg);
            }
        }

        /// <summary>
        /// Method which will list all the "keys" which have been added to the MessageBroker system.
        /// </summary>
        /// <param name="s">String parameter which isn't used for anything</param>
        public static void ListKeys(string s)
        {
            CrestronConsole.PrintLine("Current list of Broker message keys\r----------------------------------");
            foreach (var item in Messages) CrestronConsole.PrintLine($" {item.Key}");
        }

        /// <summary>
        /// Method for enabling or disabling MessageBroker system traffic monitoring.
        /// </summary>
        /// <remarks>
        /// Send "on" to enable, if anything else is sent it turns monitoring off.
        /// </remarks>
        /// <param name="s">String parameter to control monitoring.</param>
        public static void MonitorTraffic(string s)
        {
            if (s.Contains("on"))
            {
                messageDebug = true;
                CrestronConsole.PrintLine("Message Debug is on");
            }
            else
            {
                messageDebug = false;
                CrestronConsole.PrintLine("Message Debug is off");
            }
        }

        /// <summary>
        /// Method to debug MessageBroker key "lookups".
        /// </summary>
        /// <remarks>
        /// Submit a key to receive a lookup and timestamp.
        /// </remarks>
        /// <param name="key">Key to be submitted for a lookup</param>
        private static void DebugMessage(string key)
        {
            if (messageDebug)
                CrestronConsole.PrintLine($" {key,-25} called at {DateTime.Now.ToString("hh:mm:ss.ff")}");
            // system.clock is not more accurate than 15ms so going more than 2 places to show actual ms is useless
        }
    }
}