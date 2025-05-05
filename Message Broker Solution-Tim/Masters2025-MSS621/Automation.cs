using System.Collections.Generic;
using MessageBroker;


namespace Masters2025_MSS621
{
    public class Automation
    {
        /*
         * This is a class just to organize our program wide automation methods.
         * Everything in here is simply going to accept messages from our message system and send messages out
         * to that same system in sequence just like a stepper without the delays and times.
         *
         * Could you do all of this in the touchpanel class and elsewhere? Absolutely.  But it would also mean you have to remember
         * where you put them and hunt them down.   by leveraging a class that has these specific automations it makes maintaining
         * your program later a lot easier.   think of this as a Simpl folder you made that holds the steppers.
         * 
         * Note: this works because we are using a messaging system.  if you use traditional signal routing you would have to
         *       make sure this class has access to those objects in order to communicate.
         *
         * The methods here are private as we are using the messaging and they are not needed to be accessed directly
         * by the ControlSystem class.  If you did need that access then make them public.
         */
        public List<string> GlobalNvxAddresses = new List<string>();

        public Automation()
        {
            Broker.AddDelegate("SystemOn", SystemOn);
            Broker.AddDelegate("SystemOff", SystemOff);
            Broker.AddDelegate("SourceSelect", SourceSelect);
        }

        /*
         *  The hardware you are interfacing to MAY need some delays or waiting for feedbacks before you trigger the next step
         * It is up to you to add a timer to trigger that next step later, or spin up a thread,  or add code that listens to the feedback
         * from the display to then trigger the next step in the process.
         * Also what if the display is already on? checks to see if the display is on and skip the process is also a good idea.
         * there is a lot of code you could add to improve operation and automation.  What if the customer got confused and turned on the TV
         * manually?  you could detect that and start the system on process for them, the same if the display powers off.
         *
         * This means we should refrain from littering code all over the place to do these tasks.  your other classes should just expose controls
         * and do your actual functions here.  For example on and off do the page flips here and not in the PageNavigation class
         * because what if I added Fusion to the room?  fusion calling on and off will ensure the panel tracks the actual state of the
         * system.  Subpages on a page for source control are a different story and should be tied to source feedback
         *
         * NOTE:  Direct Device functions are in the DeviceSetup class
         */
        private void SystemOn(Message m)
        {
            Broker.SendMessage("DisplayPower", new Message { Digital = true });
            Broker.SendMessage("SourceSelect", new Message { Analog = 1 }); // Default to Airmedia
            Broker.SendMessage("ShowMainPage", new Message());
        }

        private void SystemOff(Message m)
        {
            Broker.SendMessage("DisplayPower", new Message { Digital = false }); // Display off
            Broker.SendMessage("NvxSetInput", new Message { Analog = 0 }); // Stop sending video to display
            Broker.SendMessage("ShowStartPage", new Message());
        }


        /*
         * Below is the logic for source select.
         * As you can see this is "fake feedback" we simply send the commands to do the switching just like a simplified
         * program would in Simpl.  We send messages to the different devices listeners.
         * 
         * If you wanted to force the display device to the first HDMI input on every switching operation that can
         * easily be added (an example is commented out)
         * 
         * If you look closely we are using the NVX as a video switcher.  we have 2 HDMI inputs on the chosen hardware
         * so we are leveraging that hardware as our switcher for this simple conference room.  When a global NVX source
         * is selected we switch to the stream input and then tune in that stream. 
         * 
         * 
         * NOTE:  IF you do not have a stream location loaded into the location that is recalled, an exception will be thrown
         *        and the next line of code will not be executed.
         *        Remember the broker is NOT spawning threads, it's simply calling delegates. therefore exceptions thrown
         *        and blocking code can affect code execution here.
         */
        private void SourceSelect(Message m)
        {
            //SendMessage("DisplayInput", new Message { Analog = 1 });  // Default TV to input 1
            switch (m.Analog)
            {
                case 0:
                    Broker.SendMessage("NvxSetInput", new Message { Analog = 1 }); // Set to NVX HDMI input 1
                    Broker.SendMessage("SetSourceFeedback", new Message { Serial = "Apple TV" });
                    //SendMessage("ShowMediaPlayPage", new Message()); // trigger the page flip
                    break;
                case 1:
                    Broker.SendMessage("NvxSetInput", new Message { Analog = 2 }); // Set to NVX HDMI input 2
                    Broker.SendMessage("SetSourceFeedback", new Message { Serial = "AirMedia" });
                    break;
                default:
                    Broker.SendMessage("NvxSetInput", new Message { Analog = 3 }); // Set to NVX STREAM

                    Broker.SendMessage("SetSourceFeedback", new Message { Serial = "Global Stream " + (m.Analog - 1) });

                    // Instead of displaying an address how about a "channel" name?
                    var location = GetNvxAddress(m.Analog - 2);
                    Broker.SendMessage("NvxSetStreamLocation", new Message { Serial = location });
                    Broker.SendMessage("NvxAddressFeedback", new Message { Serial = location });
                    break;
            }

            Broker.SendMessage("SetSourceListFeedback", new Message { Analog = m.Analog });
        }



        // Utility methods
        private string GetNvxAddress(int index) // use uint so we dont have to check for negative
        {
            if (index > GlobalNvxAddresses.Count || index < 0)
                index = 0;

            return GlobalNvxAddresses[index];
        }
    }
}
