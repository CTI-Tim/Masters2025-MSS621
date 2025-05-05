using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.UI;
using Masters_2024_MSS_521.MessageSystem;
using MSSXpanel;
using System;// Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    /// <summary>
    /// Class definition representing the main page on the touch panel
    /// </summary>
    public class MainPage
    {
        #region Object Declarations
        /// <summary>
        /// Logging header for easier reading.
        /// @MM
        /// </summary>
        private const string LogHeader = "[xMainPage] -> ";
        /// <summary>
        /// <summary>
        /// Crestron processor object
        /// @MM
        /// </summary>
        private CrestronControlSystem _cs;
        /// <summary>
        /// Interface object
        /// @MM
        /// </summary>
        private XpanelForHtml5 _ui;
        /// <summary>
        /// Crestron construct contract
        /// </summary>
        private readonly Contract _myContract;
        #endregion

        #region Contstructors
        /// <summary>
        /// Main page constructor
        /// </summary>
        /// <param name="myContract">Construct contract object parameter</param>
        public MainPage(Contract myContract)
        {
            _myContract = myContract;

            MessageBroker.AddDelegate("SetSourceListFeedback", SetSourceListFeedback);
            MessageBroker.AddDelegate("SetVolumeBarFeedback", SetVolumeBarFeedback);
            MessageBroker.AddDelegate("SetMuteFeedback", SetMuteFeedback);
            MessageBroker.AddDelegate("SetSourceFeedback", SetSourceFeedback);
            MessageBroker.AddDelegate("AirmediaAddressFb", SetAirmediaAddressFb);
            MessageBroker.AddDelegate("AirmediaPinFb", SetAirmediaPinFb);
            MessageBroker.AddDelegate("NvxAddressFeedback", SetNvxAddressFeedback);

            

            // All the buttons on the MainPage are button lists.
            _myContract.MainPage.SourceList.Button_PressEvent += SourceList_Button_PressEvent;
            _myContract.MainPage.VolumeButtonList.Button_PressEvent += VolumeButtonList_Button_PressEvent;


            // Lets populate the source list
            _myContract.MainPage.SourceList.Button_Text(0, "Apple TV");
            _myContract.MainPage.SourceList.Button_Text(1, "Airmedia");
            for (ushort i = 2; i <= 5; i++) _myContract.MainPage.SourceList.Button_Text(i, "Global Source " + i);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Method for passing in the control system object.
        /// @MM
        /// </summary>
        /// <param name="cs">Control system object being passed in</param>
        public void SetCs(CrestronControlSystem cs)
        {
            _cs = cs;
        }

        /// <summary>
        /// Method for passing in the interface object.
        /// @MM
        /// </summary>
        /// <param name="ui">Interface object being passed in</param>
        public void SetUi(XpanelForHtml5 ui)
        {
            _ui = ui;
        }

        /// <summary>
        /// Method which sets the source list feedback.
        /// @MM
        /// </summary>
        /// <param name="m">Parameter which represents the source selection as well as the subpage.</param>
        public void SetSourceListFeedback(ushort m)
        {
            for (ushort i = 0; i <= 5; i++)
                _myContract.MainPage.SourceList.Button_Selected(i, false);
            _myContract.MainPage.SourceList.Button_Selected(m, true);

            ShowMainSubpages(m);
        }

        /// <summary>
        /// Method which sets the source text feedback.
        /// @MM
        /// </summary>
        /// <param name="m">Parameter which represents the source text.</param>
        public void SetSourceFeedback(string m)
        {
            _myContract.MainPage.SourceFeedbackLabel_Indirect(m);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for the volume list buttons.
        /// </summary>
        /// <param name="sender">Button object within the list</param>
        /// <param name="e">Arguments with respect to the button in the list</param>
        private void VolumeButtonList_Button_PressEvent(object sender, IndexedButtonEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                //volume buttons are a list
                switch (e.ButtonIndex)
                {
                    case 0: // Vol Up
                        {
                            //MessageBroker.SendMessage("DisplayVolumeUp", new Message());
                            Dom.GetDisp().VolumeUp(); // @MM
                            ErrorLog.Notice(String.Format(LogHeader +
                                "Volume Up button {0}",
                                    e.ButtonIndex));
                            break;
                        }
                    case 1: // Mute Toggle
                        {
                            //MessageBroker.SendMessage("DisplayMuteToggle", new Message());
                            Dom.GetDisp().MuteToggle(); // @MM
                            ErrorLog.Notice(String.Format(LogHeader +
                                "Volume Mute button {0}",
                                    e.ButtonIndex));
                            break;
                        }
                    case 2: // Vol Dn
                        {
                            //MessageBroker.SendMessage("DisplayVolumeDown", new Message());
                            Dom.GetDisp().VolumeDown(); // @MM
                            ErrorLog.Notice(String.Format(LogHeader +
                                "Volume Down button {0}",
                                    e.ButtonIndex));
                            break;
                        }
                }
            }
            else
            {
                // Button was released stop the ramping
                if (e.ButtonIndex != 1)
                {
                    //MessageBroker.SendMessage("DisplayVolumeStop", new Message());
                    Dom.GetDisp().VolumeStop(); //@MM
                }
            }
        }

        /// <summary>
        /// Event handler for the source list buttons.
        /// </summary>
        /// <param name="sender">Button object within the list</param>
        /// <param name="e">Arguments with respect to the button in the list</param>
        private void SourceList_Button_PressEvent(object sender, IndexedButtonEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                //MessageBroker.SendMessage("SourceSelect", new Message { Analog = e.ButtonIndex }); //Source Select is in Automation.cs
                
                switch(e.ButtonIndex)
                {
                    case 0:
                        Dom.GetUi()._navigation.ShowMediaPage(e.ButtonIndex);
                        break;
                    case 1:
                        Dom.GetUi()._navigation.ShowAirMediaPage(e.ButtonIndex);
                        break;
                    default:
                        Dom.GetUi()._navigation.ShowNvxPage(e.ButtonIndex);

                        break;
                }
                
            }
        }

        #endregion

        #region MessageBroker Methods
        /// <summary>
        /// Method used by the MessageBroker to manipulate the source list feedback
        /// </summary>
        /// <param name="m">Value parameter sent via the message broker</param>
        private void SetSourceListFeedback(Message m)
        {
            for (ushort i = 0; i <= 5; i++)
                _myContract.MainPage.SourceList.Button_Selected(i, false);
            _myContract.MainPage.SourceList.Button_Selected(m.Analog, true);

            ShowMainSubpages(m.Analog);
        }
        /// <summary>
        /// MessageBroker method for setting volume feedback.
        /// </summary>
        /// <param name="m">Value parameter sent via the message broker</param>
        private void SetVolumeBarFeedback(Message m)
        {
            _myContract.MainPage.VolumeBar_Touchfb(m.Analog);
        }
        /// <summary>
        /// MessageBroker method for setting mute feedback.
        /// </summary>
        /// <param name="m">Value parameter sent via the message broker</param>
        private void SetMuteFeedback(Message m)
        {
            _myContract.MainPage.MutedFeedback_Visibility(m.Digital);
        }
        /// <summary>
        /// MessageBroker method for setting source feedback.
        /// </summary>
        /// <param name="m">Value parameter sent via the message broker</param>
        private void SetSourceFeedback(Message m)
        {
            _myContract.MainPage.SourceFeedbackLabel_Indirect(m.Serial);
        }
        /// <summary>
        /// MessageBroker method for setting air media address feedback.
        /// </summary>
        /// <param name="m">Value parameter sent via the message broker</param>
        private void SetAirmediaAddressFb(Message m)
        {
            _myContract.AirMediaInfo.AirmediaAddressFb_Indirect(m.Serial);
        }
        /// <summary>
        /// MessageBroker method for setting pin code feedback.
        /// </summary>
        /// <param name="m">Value parameter sent via the message broker</param>
        private void SetAirmediaPinFb(Message m)
        {
            _myContract.AirMediaInfo.AirmediaPinFb_Indirect(m.Serial);
        }
        /// <summary>
        /// MessageBroker method for setting nvx address feedback.
        /// </summary>
        /// <param name="m">Value parameter sent via the message broker</param>
        private void SetNvxAddressFeedback(Message m)
        {
            _myContract.NvxInfo.NvxAddressFb_Indirect(m.Serial);
        }
        #endregion

        #region ShowMainSubpages
        /// <summary>
        /// Page will contain what was requested.
        /// Add in code to show the subpages one at a tim as they are needed.
        /// There are 3 total subpages,  one needs to be displayed for multiple sources.
        /// The variable page above will contain a number from 0-6,
        /// 0 = media page  1 = Airmedia page all the rest are the NVX global source page.
        /// </summary>
        /// <remarks>
        /// You will need to find in the contract files the subpage classes and objects to use.
        /// Hint:  MainPage is the page they are on.
        /// </remarks>
        /// <param name="page"></param>
        private void ShowMainSubpages(int page)
        {
            // TODO: Finish subpage visibility 

            // Done... work completed in "PageNavigation.cs"
            // Calls come from above in the event handler "SourceList_Button_PressEvent"
        }
        #endregion
    }
}