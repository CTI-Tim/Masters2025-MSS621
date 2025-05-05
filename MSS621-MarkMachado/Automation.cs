using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronWebSocketServer;
using Masters_2024_MSS_521.MessageSystem;

namespace Masters_2024_MSS_521
{
    /// <summary>
    /// This is a class just to organize our program wide automation methods.
    /// Everything in here is simply going to accept messages from our message system and send messages out
    /// to that same system in sequence just like a stepper without the delays and times.
    /// 
    /// Could you do all of this in the touchpanel class and elsewhere? Absolutely.  But it would also mean you have to remember
    /// where you put them and hunt them down. By leveraging a class that has these specific automations it makes maintaining
    /// your program later a lot easier. Think of this as a Simpl folder you made that holds the steppers.
    /// The methods here are private as we are using the messaging and they are not needed to be accessed directly
    /// by the ControlSystem class.  If you did need that access then make them public.
    /// </summary>
    /// <remarks>
    /// The hardware you are interfacing to MAY need some delays or waiting for feedbacks before you trigger the next step
    /// It is up to you to add a timer to trigger that next step later, or spin up a thread,  or add code that listens to the feedback
    /// from the display to then trigger the next step in the process.
    /// 
    /// Also what if the display is already on? checks to see if the display is on and skip the process is also a good idea.
    /// there is a lot of code you could add to improve operation and automation.  What if the customer got confused and turned on the TV
    /// manually?  you could detect that and start the system on process for them, the same if the display powers off.
    /// 
    /// This means we should refrain from littering code all over the place to do these tasks.  your other classes should just expose controls
    /// and do your actual functions here.  For example on and off do the page flips here and not in the PageNavigation class
    /// because what if I added Fusion to the room?  fusion calling on and off will ensure the panel tracks the actual state of the
    /// system.  Subpages on a page for source control are a different story and should be tied to source feedback
    /// 
    /// NOTE:  Direct Device functions are in the DeviceSetup class
    /// </remarks>
    public class Automation
    {
        #region Object Declarations
        /// <summary>
        /// Storage list for NVX addresses used in requesting streams.
        /// </summary>
        public List<string> GlobalNvxAddresses = new List<string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Automation class default constructor. This builds the class when the object is instantiated.
        /// </summary>
        public Automation()
        {
            MessageBroker.AddDelegate("SystemOn", SystemOn);
            MessageBroker.AddDelegate("SystemOff", SystemOff);
            MessageBroker.AddDelegate("SourceSelect", SourceSelect);
        }
        #endregion

        #region MessageBroker Methods
        /// <summary>
        /// Method used by the MessageBroker system for "Power On"
        /// </summary>
        /// <param name="m">MessageBroker message parameter</param>
        private void SystemOn(Message m)
        {
            MessageBroker.SendMessage("DisplayPower", new Message { Digital = true });
            MessageBroker.SendMessage("SourceSelect", new Message { Analog = 1 }); // Default to Airmedia
            MessageBroker.SendMessage("ShowMainPage", new Message());
        }
        /// <summary>
        /// Method used by the MessageBroker system for "Power Off"
        /// </summary>
        /// <param name="m">MessageBroker message parameter</param>
        private void SystemOff(Message m)
        {
            MessageBroker.SendMessage("DisplayPower", new Message { Digital = false }); // Display off
            MessageBroker.SendMessage("NvxSetInput", new Message { Analog = 0 }); // Stop sending video to display
            MessageBroker.SendMessage("ShowStartPage", new Message());
        }

        /// <summary>
        /// Method used by the MessageBroker system for "Source Select"
        /// </summary>
        /// <param name="m">MessageBroker message parameter</param>
        private void SourceSelect(Message m)
        {
            //MessageBroker.SendMessage("DisplayInput", new Message { Analog = 1 });  // Default TV to input 1
            switch (m.Analog)
            {
                case 0:
                    
                    MessageBroker.SendMessage("NvxSetInput", new Message { Analog = 1 }); // Set to NVX HDMI input 1
                    MessageBroker.SendMessage("SetSourceFeedback", new Message { Serial = "Apple TV" });
                    //MessageBroker.SendMessage("ShowMediaPlayPage", new Message()); // trigger the page flip
                    break;
                case 1:
                    MessageBroker.SendMessage("NvxSetInput", new Message { Analog = 2 }); // Set to NVX HDMI input 2
                    MessageBroker.SendMessage("SetSourceFeedback", new Message { Serial = "AirMedia"});
                    break;
                default:
                    MessageBroker.SendMessage("NvxSetInput", new Message { Analog = 3 }); // Set to NVX STREAM

                    MessageBroker.SendMessage("SetSourceFeedback", new Message { Serial = "Global Stream " + (m.Analog - 1) });

                    // Instead of displaying an address how about a "channel" name?
                    var location = GetNvxAddress(m.Analog - 2); 
                    MessageBroker.SendMessage("NvxSetStreamLocation", new Message { Serial = location });
                    MessageBroker.SendMessage("NvxAddressFeedback", new Message { Serial = location });
                    break;
            }

            MessageBroker.SendMessage("SetSourceListFeedback", new Message { Analog = m.Analog });
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Method which translates UI button analog values into NVX addresses
        /// </summary>
        /// <param name="index">UI button index value parameter</param>
        /// <returns>NVX address which has been looked up from UI button analog values</returns>
        public string GetNvxAddress(int index) // use uint so we dont have to check for negative
        {
            if (index > GlobalNvxAddresses.Count || index < 0) 
                index = 0;

            return GlobalNvxAddresses[index];
        }
        #endregion
    }
}