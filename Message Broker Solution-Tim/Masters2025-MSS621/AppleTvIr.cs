
using Crestron.SimplSharpPro;
using Crestron.SimplSharp.CrestronIO;
using Nvx3xx;
using MessageBroker;
using Crestron.SimplSharp.Ssh;

namespace Masters2025_MSS621
{
    public class AppleTvIr
    {
        /*
         * 
         */

        private IROutputPort _port;

        public AppleTvIr(IROutputPort Port)
        {
            _port = Port;

            /*
             * The IR driver is included in the resources for this program
             * We need to point the path at that location.
             * Remember the names given to the commands in the IR file are how you access them
             * In this case we are using a simple AppleTV ir file. this can be anything
             * If your ir files are consistant, technically it can be called "device.ir" and placed
             * in the user directory to make it easy to swap out the device with another as long as 
             * the IR file was learned cleanly and naming convention standards were adhered to.
             * 
             * change driver file,  restart program,  test and complete the service call.
             * 
             */

            string IRpath = string.Format("{0}/AppleTV.ir", Directory.GetApplicationDirectory());
            _port.LoadIRDriver(IRpath);

            /*
             *  Naming of signals insode the IR files are as follows
             *  UP_ARROW
             *  DN_ARROW
             *  LEFT_ARROW
             *  RIGHT_ARROW
             *  ENTER
             *  MENU
             *  PLAY
             *  
             *  Notice there is no back ir funtion. Older AppleTVs do not have this available sending the menu IR command will work for back
             *  Funny thing is that the older Apple TV's support back via CEC control.
             */

            Broker.AddDelegate("AtvUp", m => _port.Press("UP_ARROW")); // Example using a lambda What is the m => for?  we dont care about the message sent but we have to accept it.
            Broker.AddDelegate("AtvRelease", SendRelease);


        }
        //Example of a helper method to send the IR command
        private void SendUp (Message m)
        {
            _port.Press("UP_ARROW");
        }


        private void SendRelease(Message m)
        {
            _port.Release();
        }
    }
}
