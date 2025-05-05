using Airmedia3100;                 // Bring in our custom Airmedia Library
using CrestronConnected;
using Nvx3xx;
using MessageBroker;
using Crestron.SimplSharp.CrestronIO;

namespace Masters2025_MSS621
{
 /*
 *  This class is simply used to setup all of our hardware devices and set up our communication between
 * the different parts.   This is purely for organization and readability of the code.  Performance is no different than
 * if you were to put all this in ControlSystem.cs
 *
 * The rules there STILL apply here.   We Instantiated this class in the initialize method,  we have 20 seconds to return so that
 * method can finish.   Just because we jumped over to another class does not mean those rules do not apply anymore.
 * If you have to do something that takes time, you MUST spin up a thread and have that do the work while you let the
 * constructor finish quickly and return.
 *
 * This class can get large if you have a large amount of hardware.  it would be a good idea to split this class up to multiple
 * classes that do each job if you find it starts to become huge and hard to find what you are looking for in it.
 *
 * Code in here should deal with the devices only or pass information to the event broker.
 *
 */
    public class DeviceSetup
    {
        // Add the classes here globally so they stay persistent.
        private AirMedia myAirmedia;
        private ConnectedDisplay myConnectedDisplay;
        private Nvx myNvx;
        private AppleTvIr myAtvIr;

        public DeviceSetup(ControlSystem cs) 
        {
            // Setup the NVX
            myNvx = new Nvx(0x08, Nvx.EMode.Rx, cs);
            myNvx.BaseEvent += MyNvx_BaseEvent;

            // Connect the Apple TV IR control to the NVX's IR port using our custom class
            myAtvIr = new AppleTvIr(myNvx.NvxIrPort);


            //Add the Message Listeners
            Broker.AddDelegate("NvxSetStreamLocation", NvxSetStreamLocation);
            Broker.AddDelegate("NvxSetInput", NvxSetInput);

            myAirmedia = new AirMedia(0x12, cs);
            myAirmedia.BaseEvent += MyAirmedia_BaseEvent;
            myAirmedia.SetPinCodeRandom();

            myConnectedDisplay = new ConnectedDisplay(0x22, cs);
            myConnectedDisplay.BaseEvent += MyConnectedDisplay_BaseEvent;
            Broker.AddDelegate("DisplayPower", DisplayPower);
            Broker.AddDelegate("DisplayInput", DisplayInput);
            Broker.AddDelegate("DisplayVolumeDown", DisplayVolumeDown);
            Broker.AddDelegate("DisplayVolumeUp", DisplayVolumeUp);
            Broker.AddDelegate("DisplayVolumeStop", DisplayVolumeStop);
            Broker.AddDelegate("DisplayMuteToggle", DisplayMuteToggle);

            
        }

        /*
        *  Below is where we create the messaging broker wrapper methods for all the different hardware as well as the event handlers
        *  
        * your helper classes or frameworks may or may not be designed around the messaging system or other frameworks you
        * designed.  It is 100% ok to write simple wrapper methods to make something work or interface one method with another.
        * Our messaging broker system sends a payload in a Message class, absolutely none of the crestron devices support this
        * data object, nor does any of the framework classes around them.   we simply create a method to accept the message
        * and then we just use the part of the message we need to send to the device or framework we are using.
        * be sure to be organized and clear with these.   this is a point that can cause confusion for yourself or others
        * that deal with your code later.

               Question:  can I just not write a wrapper method and just use a lambda?  you sure can and are free to do so.
               They are written here as seperate methods for learning clarity for programmers that are not as familiar with
               lambdas.
        *
        * Commenting your code as to what they are and even comments by the hardware setup as to what class these are in will make your life
        * easier and the next  programmers life easier.
        */

        #region #### Methods and Handlers
        // ########### NVX ############
        private void MyNvx_BaseEvent(object sender, Nvx.Args e)
        {
            // These are not currently used in this program but can be used to show if sync is available
            //SendMessage("NvxInput1SyncFeedback", new Message { Digital = e.Hdmi1Sync });
            //SendMessage("NvxInput2SyncFeedback", new Message { Digital = e.Hdmi2Sync });
        }
        private void NvxSetStreamLocation(Message m)
        {
            myNvx.SetStreamLocation(m.Serial);
        }
        private void NvxSetInput(Message m)
        {
            /*
             *  the Nvx Interface class we have wants to pass the enum for selecting the input chosen
             *  This is great for using that class directly, but in this example we are only passing
             *  the 3 base crestron data types so we need to convert the 0-3 "analog" value to what the
             *  class wants, here we can simply cast it.
             */
            myNvx.SetInput((Nvx.ESource)m.Analog); 
        }

        // ########### AirMedia #############
        private void MyAirmedia_BaseEvent(object sender, AirMedia.Args e)
        {
                // We have an event from the airmedia, lets update any feedbacks by sending messages to the broker

                Broker.SendMessage("AirmediaPinFb",
                    new Message { Serial = e.CurrentPinCode.ToString().PadLeft(4, '0') });

                Broker.SendMessage("AirmediaAddressFb", 
                    new Message { Serial = e.CurrentAddress });
        }


        // #############  Connected Display Methods ################

        //Event Handler
        private void MyConnectedDisplay_BaseEvent(object sender, ConnectedDisplay.Args e)
        {
            if (e.Message == "Volume")
                Broker.SendMessage("SetVolumeBarFeedback",
                    new Message { Analog = e.VolumeFb });

            else if (e.Message == "Mute")
                Broker.SendMessage("SetMuteFeedback",
                    new Message { Digital = e.MuteOn });
        }

        //Work Methods
        private void DisplayPower(Message m)
        {
            switch (m.Digital)
            {
                case true:
                    myConnectedDisplay.On();
                    break;
                case false:
                    myConnectedDisplay.Off();
                    break;
            }
        }
        private void DisplayInput(Message m)
        {
            myConnectedDisplay.Input(m.Analog);
        }
        private void DisplayVolumeDown(Message m)
        {
            myConnectedDisplay.VolumeDown();
        }
        private void DisplayVolumeUp(Message m)
        {
            myConnectedDisplay.VolumeUp();
        }
        private void DisplayVolumeStop(Message m)
        {
            myConnectedDisplay.VolumeStop();
        }
        private void DisplayMuteToggle(Message m)
        {
            myConnectedDisplay.MuteToggle();
        }
        #endregion #### Methods and Handlers
    }
}
