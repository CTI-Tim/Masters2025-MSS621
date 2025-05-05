using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSXpanel;
using MessageBroker;

namespace Masters2025_MSS621.UI
{
    public class MainPage
    {
        private readonly Contract _myContract;

        public MainPage(Contract myContract)
        {
            _myContract = myContract;

            Broker.AddDelegate("SetSourceListFeedback", SetSourceListFeedback);
            Broker.AddDelegate("SetVolumeBarFeedback", SetVolumeBarFeedback);
            Broker.AddDelegate("SetMuteFeedback", SetMuteFeedback);
            Broker.AddDelegate("SetSourceFeedback", SetSourceFeedback);
            Broker.AddDelegate("AirmediaAddressFb", SetAirmediaAddressFb);
            Broker.AddDelegate("AirmediaPinFb", SetAirmediaPinFb);
            Broker.AddDelegate("NvxAddressFeedback", SetNvxAddressFeedback);

            //NOTE: We subscribe to the generic button press event, you will notice the contact has all of them seperate
            _myContract.MainPage.SourceList.Button_PressEvent += SourceList_Button_PressEvent;
 
            //Volume buttons
            //NOTE: Here we are using the seperate button press events instead of the overall event.
            _myContract.MainPage.VolumeButtonList.Button_1_Button_PressEvent += VolumeVert_VolUp_PressEvent; // VolUp
            _myContract.MainPage.VolumeButtonList.Button_2_Button_PressEvent += VolumeVert_VolMute_PressEvent; // Mute
            _myContract.MainPage.VolumeButtonList.Button_3_Button_PressEvent += VolumeVert_VolDn_PressEvent; // VolDn

            //Lets populate the source list button names
            _myContract.MainPage.SourceList.Button_Text(0, "Apple TV");
            _myContract.MainPage.SourceList.Button_Text(1, "Airmedia");

            for (ushort i = 2; i <= 5; i++)
                _myContract.MainPage.SourceList.Button_Text(i, "Global Source "+ i);
        }

        private void VolumeVert_VolMute_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                Broker.SendMessage("DisplayMuteToggle", new Message());
        }

        private void VolumeVert_VolDn_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                Broker.SendMessage("DisplayVolumeDown", new Message());
            else
                Broker.SendMessage("DisplayVolumeStop", new Message());
        }

        private void VolumeVert_VolUp_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                Broker.SendMessage("DisplayVolumeUp", new Message());
            else
                Broker.SendMessage("DisplayVolumeStop", new Message());

        }

        
        private void SourceList_Button_PressEvent(object sender, IndexedButtonEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                Broker.SendMessage("SourceSelect", new Message { Analog = e.ButtonIndex }); //Source Select logic is in Automation.cs

        }

        private void SetSourceListFeedback(Message m)
        {
            for (ushort i = 0; i <= 5; i++)
            {
                _myContract.MainPage.SourceList.Button_Selected(i, false);
            }

            _myContract.MainPage.SourceList.Button_Selected(m.Analog, true);

            ShowMainSubpages(m.Analog);
        }

        private void SetVolumeBarFeedback(Message m)
        {
            _myContract.MainPage.VolumeBar_Touchfb(m.Analog); 
        }
        private void SetMuteFeedback(Message m)
        {
            _myContract.MainPage.MutedFeedback_Visibility_fb(m.Digital);
        }

        private void SetSourceFeedback(Message m)
        {

            // _myContract.Main.SourceFeedbackLabel_Indirect(m.Serial);
            // Not implimented in this touchpanel but you could add it.
        }

        private void SetAirmediaAddressFb(Message m)
        {
            _myContract.AirMediaInfo.AirmediaAddressFb_Indirect(m.Serial);
        }
        private void SetAirmediaPinFb(Message m)
        {
           _myContract.AirMediaInfo.AirmediaPinFb_Indirect(m.Serial);
        }

        private void SetNvxAddressFeedback(Message m)
        {
            _myContract.NvxInfo.NvxAddressFb_Indirect(m.Serial);
        }

        private void ShowMainSubpages(int page)
        {
            // Hide all the AV subpages

            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility_fb(false);
            _myContract.MainPage.MediaControl.MediaControl_Visibility_fb(false);
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility_fb(false);

            switch (page)
            {
                case 0:
                    _myContract.MainPage.MediaControl.MediaControl_Visibility_fb(true);
                    break;
                case 1:
                    _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility_fb(true);
                    break;
                default:
                    _myContract.MainPage.NvxInfo.NvxInfo_Visibility_fb(true);
                    break;
            }


        }
    }
}
