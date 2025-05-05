using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using MSSXpanel;
using System;
using System.Collections.Generic;// Bring in your contract namespace

namespace Masters_2025_MSS_621_JW.UserInterface
{
    public class Xpanel
    {
        private readonly Contract _myContract;
        private readonly XpanelForHtml5 _myXpanel;

        // events
        public delegate void SourceSelectHandler(int source);
        public event SourceSelectHandler SourceSelect;

        // components
        private ControlSystem cs;
        private Audio audio;
        private List<string> sources = new List<string>();

        // state of the panel
        bool confirmShutdown = false;
        string sourceControls = "";

        public Xpanel(uint ipId, ControlSystem cs)
        {
            this.cs = cs;
            _myXpanel = new XpanelForHtml5(ipId, cs);
            _myXpanel.OnlineStatusChange += _myXpanel_OnlineStatusChange;
            _myXpanel.Register();
            _myContract = new Contract();
            _myContract.AddDevice(_myXpanel);

            // subscribe to contract events
            _myContract.StartPage.Button_PressEvent += StartPage_Button_PressEvent;
            _myContract.PowerOffOk.PowerOffYesButton_PressEvent += PowerOffOk_PowerOffYesButton_PressEvent;

            _myContract.MainPage.SourceList.Button_PressEvent += SourceList_Button_PressEvent;
            _myContract.MainPage.VolumeButtonList.Button_PressEvent += VolumeButtonList_Button_PressEvent;

            _myContract.HeaderBar.PowerButton_PressEvent += HeaderBar_PowerButton_PressEvent;
            _myContract.PowerOffOk.PowerOffNoButton_PressEvent += PowerOffOk_PowerOffNoButton_PressEvent;

            //Lets populate the source list
            _myContract.MainPage.SourceList.Button_Text(0, "Apple TV");
            _myContract.MainPage.SourceList.Button_Text(1, "Airmedia");
            for (ushort i = 2; i <= 5; i++) _myContract.MainPage.SourceList.Button_Text(i, "Global Source " + i);


            // Lets set the room name header bar 
            _myContract.HeaderBar.RoomNameLabel_Indirect("MSS-621 Conference Room");
        }

        // If the xpanel goes offline or online this will make sure we go back to the page the program wants us on
        private void _myXpanel_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            RefreshPage();
        }

        private void SourceList_Button_PressEvent(object sender, IndexedButtonEventArgs e) {
            if (!e.SigArgs.Sig.BoolValue) return;
            SourceSelect?.Invoke(e.ButtonIndex);
            for (ushort i=0; i<=5; i++) {
                _myContract.MainPage.SourceList.Button_Selected(i, i==e.ButtonIndex);
            }
        }

        #region Page Navigation

        private void PowerOffOk_PowerOffNoButton_PressEvent(object sender, UIEventArgs e) {
            if (!e.SigArgs.Sig.BoolValue) return;
            confirmShutdown = false;
            RefreshPage();
        }

        private void HeaderBar_PowerButton_PressEvent(object sender, UIEventArgs e) {
            if (!e.SigArgs.Sig.BoolValue) return;
            confirmShutdown = true;
            RefreshPage();
        }

        private void PowerOffOk_PowerOffYesButton_PressEvent(object sender, UIEventArgs e) {
            if (!e.SigArgs.Sig.BoolValue) return;
            confirmShutdown = false;
            cs.SystemPower = false;
            RefreshPage();
        }

        private void StartPage_Button_PressEvent(object sender, UIEventArgs e) {
            if (!e.SigArgs.Sig.BoolValue) return;
            cs.SystemPower = true;
            RefreshPage();
        }

        public void RefreshPage() {
            _myContract.StartPage.StartPage_VisibilityJoin(cs.SystemPower == false);
            _myContract.MainPage.MainPage_VisibilityJoin(cs.SystemPower == true);

            _myContract.MainPage.PowerOffOk.PowerOffOk_Visibility(confirmShutdown);

            _myContract.MainPage.MediaControl.MediaControl_Visibility(sourceControls == "MediaControl");
            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility(sourceControls == "AirMediaInfo");
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility(sourceControls == "NvxInfo");
        }

        #endregion

        #region Audio
        public void ConnectAudio(Audio audio) { this.audio = audio; }

        public void UpdateVolume(ushort vol) {
            _myContract.MainPage.VolumeBar_Touchfb(vol);
        }

        public void UpdateMute(bool mute) {
            _myContract.MainPage.MutedFeedback_Visibility(mute);
        }

        private void VolumeButtonList_Button_PressEvent(object sender, IndexedButtonEventArgs e) {
            if (e.SigArgs.Sig.BoolValue) {
                //volume buttons are a list
                switch (e.ButtonIndex) {
                    case 0: // Vol Up
                        audio.VolumeUpRamp();
                        break;
                    case 1: // Mute Toggle
                        audio.MuteToggle();
                        break;
                    case 2: // Vol Dn
                        audio.VolumeDownRamp();
                        break;
                }
            } else {
                //Button was released stop the ramping
                if (e.ButtonIndex != 1)
                    audio.VolumeStopRamp();
            }
        }
        #endregion

        #region Source Feedback

        public void UpdateAirMediaPin(ushort pin) {
            _myContract.AirMediaInfo.AirmediaPinFb_Indirect(pin.ToString());
        }

        public void UpdateAirMediaAddress(string addr) {
            _myContract.AirMediaInfo.AirmediaAddressFb_Indirect(addr);
        }

        internal void SetSources(List<string> list) {
            sources = list;
            for (ushort i=0; i<sources.Count; i++) {
                _myContract.MainPage.SourceList.Button_Text(i, sources[i]);
            }
        }

        internal void SourceControls(string v) {
            sourceControls = v;
            RefreshPage();
        }

        #endregion
    }
}