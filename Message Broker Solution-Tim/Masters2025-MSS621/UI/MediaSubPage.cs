using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSXpanel;

using Crestron.SimplSharp;
using MessageBroker;

namespace Masters2025_MSS621.UI
{
    public class MediaSubPage
    {
        private MSSXpanel.Contract _myContract;
        public MediaSubPage(Contract myContract)
        {
            _myContract = myContract;
            _myContract.MediaControl.Dpad.Button_PressEvent += Dpad_Button_PressEvent;
            _myContract.MediaControl.BackButton_PressEvent += MediaPage_BackButton_PressEvent;
            _myContract.MediaControl.MenuButton_PressEvent += MediaPage_MenuButton_PressEvent;
            _myContract.MediaControl.PlayButton_PressEvent += MediaPage_PlayButton_PressEvent;
        }

        private void MediaPage_PlayButton_PressEvent(object sender, UIEventArgs e)
        {
            //ToDo: Play Event
        }

        private void MediaPage_MenuButton_PressEvent(object sender, UIEventArgs e)
        {
            //ToDo: Menu
        }

        private void MediaPage_BackButton_PressEvent(object sender, UIEventArgs e)
        {
            //ToDo: Back
        }

        private void Dpad_Button_PressEvent(object sender, DpadEventArgs e)
        {
            /*
             * Construct Contract gives us the D-Pad overall and we need to compare what was pressed on it
             * In this case we are using a switch/case to simplify.
             * 
             * Note: the event name for this is a "PressEvent" and is poorly named.  It's actually a change event.
             *       The actual state is buried in e.SigArgs.Sig.BoolValue. So we need to look at both in order to 
             *       use the Dpad
             * 
             */

            if (e.SigArgs.Sig.BoolValue)
            {
                //button was pushed
                switch (e.DpadButton)
                {
                    case DpadButton.Left:

                        break;
                    case DpadButton.Right:

                        break;
                    case DpadButton.Up:

                        break;
                    case DpadButton.Down:

                        break;
                    case DpadButton.Center:

                        break;
                }
            }
            else
            {
                // Button was released
                Broker.SendMessage("AtvRelease", new Message());  // WE dont care about data being sent, send an empty message

            }
        }
    }
}
