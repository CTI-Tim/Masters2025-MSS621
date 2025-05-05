using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker
{
    public static class Broker
    {

        /*  Yes this is the message broker from Masters 2024 example code.
         *  It has been cleaned up, all the crestron specific debug code removed so it can be compiled as a
         *  stand alone DLL that technically can be used anywhere.
         *  
         *  
         * 
         * 
         */



        public delegate void MessageDelegate(Message m);

        private static readonly Dictionary<string, MessageDelegate> Messages =
            new Dictionary<string, MessageDelegate>();

        /// <summary>
        /// Add a delegate listener to the message broker. 
        /// </summary>
        /// <param name="key">String "key" to use for sending the message to this specific listener</param>
        /// <param name="method">The method to be called when this message is sent.  The method must accept an object MethodCalled(Message m)</param>
        /// <exception cref="Exception">Exception thrown if a duplicate key is found</exception>
        public static void AddDelegate(string key, MessageDelegate method)
        {
            if (!Messages.ContainsKey(key)) // we do not want duplicate keys.
                Messages.Add(key, method);
            else
                throw new Exception($"## Duplicate key attempted in Message Broker Class  key {key} already exists ##");
        }
        /// <summary>
        /// Removes a delegate listener from the message broker.
        /// </summary>
        /// <param name="key"> string "key" that matches an existing key</param>
        public static void RemoveDelegate(string key)
        {
            if (Messages.ContainsKey(key))
                Messages.Remove(key);
        }
        /// <summary>
        /// Sends a message that will be matched with any available listener. if no listener is found the message is sent to outer space
        /// Message send example:  Broker.SendMessage("SourceSelect", new Message { Analog = e.ButtonIndex })
        /// </summary>
        /// <param name="key">string "key" to match a listener that has been already created</param>
        /// <param name="m"> Message sent in the class object defined in Message class</param>
        public static void SendMessage(string key, Message m)
        {
            if (Messages.ContainsKey(key))
            {
                Messages[key].DynamicInvoke(m);
            }
        }
    }
}
