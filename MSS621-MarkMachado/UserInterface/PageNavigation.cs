using Crestron.SimplSharpPro;
using Crestron.SimplSharp;
using Masters_2024_MSS_521.MessageSystem;
using MSSXpanel;
using MSSXpanel.MainPage;
using System.Net.NetworkInformation; // Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    /// <summary>
    /// This class should really only do page navigation. 
    /// This class is for touchpanel navigation, so leave it at that. The nice part of the contracts being it's own class means
    /// we can subscribe to the button events elsewhere and put the code for actually triggering things there leaving this clean for only
    /// doing one job, which is the page navigation for the panel.
    /// </summary>
    /// <remarks>
    /// Because the Power ON and Off button event handlers are in here. I will call those messages to the message broker from here.
    /// Refrain from attaching those events to the page flip logic. Let the automation call the actual page flips.
    /// </remarks>
    public class PageNavigation
    {
        #region Object Declarations
        /// <summary>
        /// Logging header for easier reading.
        /// @MM
        /// </summary>
        private const string LogHeader = "[xPageNavigation] -> ";

        /// <summary>
        /// Crestron construct contract
        /// </summary>
        private MSSXpanel.Contract _myContract;

        /// <summary>
        /// Currently selected touch panel page
        /// </summary>
        private int _currentPage = 0;

        #endregion

        #region Constructors
        /// <summary>
        /// PageNavigation constructor which adds delegates and establishes event handlers.
        /// </summary>
        /// <param name="myContract">Parameter for a crestron construct contract</param>
        public PageNavigation(Contract myContract)
        {
            _myContract = myContract;

            MessageSystem.MessageBroker.AddDelegate("ShowMediaPlayPage", ShowMediaPlayPage);
            MessageSystem.MessageBroker.AddDelegate("ShowStartPage", ShowSystemStartPage);
            MessageSystem.MessageBroker.AddDelegate("ShowMainPage", ShowSystemMainPage);

            _myContract.StartPage.Button_PressEvent += StartPage_Button_PressEvent;
            _myContract.HeaderBar.PowerButton_PressEvent += MainPage_PowerButton_PressEvent;
            // The below subpage/Widget was set to "global" in construct to get access like this.
            // Select the widget, then in layer manager window select the WidgetContainer.  there you will find global for ControlContract
            _myContract.PowerOffOk.PowerOffNoButton_PressEvent += PowerOffOkSubpage_PowerOffNoButton_PressEvent;
            _myContract.PowerOffOk.PowerOffYesButton_PressEvent += PowerOffOkSubpage_PowerOffYesButton_PressEvent;

        }
        #endregion

        #region Page Display Methods
        /// <summary>
        /// Method for displaying the start page.
        /// @MM
        /// </summary>
        public void ShowStartPage()
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowStartPage", "Current Page 1", 0);
            _myContract.StartPage.StartPage_VisibilityJoin((sig, m) => sig.Pulse(20));
            _currentPage = 0;
        }
        /// <summary>
        /// Method for displaying the main page.
        /// @MM
        /// </summary>
        public void ShowMainPage()
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowMainPage", "Current Page 1", 0);
            _myContract.MainPage.MainPage_VisibilityJoin((sig, m) => sig.Pulse(20));
            _currentPage = 1;
        }
        /// <summary>
        /// Method for displaying the media control page.
        /// @MM
        /// </summary>
        public void ShowMediaPage()
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowMediaPage", "Current Page 2", 0);
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.MediaControl.MediaControl_Visibility((sig, m) => sig.BoolValue = true);
            _currentPage = 2;
        }
        /// <summary>
        /// Method for displaying the media control page.
        /// @MM
        /// </summary>
        public void ShowMediaPage(ushort e)
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowMediaPage", "Current Page 2", 0);
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.MediaControl.MediaControl_Visibility((sig, m) => sig.BoolValue = true);
            _currentPage = 2;
            Dom.GetUi()._mainPage.SetSourceListFeedback(e); // Set to NVX HDMI input 1
            Dom.GetUi()._mainPage.SetSourceFeedback("Apple TV"); // Set the source text feedback
        }
        /// <summary>
        /// Method for displaying the nvx information page.
        /// @MM
        /// </summary>
        public void ShowNvxPage()
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowNvxPage", "Current Page 3", 0);
            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.MediaControl.MediaControl_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility((sig, m) => sig.BoolValue = true);
            _currentPage = 3;
        }
        /// <summary>
        /// Method for displaying the nvx information page.
        /// @MM
        /// </summary>
        /// <param name="e">Ui button index value</param>
        public void ShowNvxPage(ushort e)
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowNvxPage", "Current Page 3", 0);
            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.MediaControl.MediaControl_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility((sig, m) => sig.BoolValue = true);
            _currentPage= 3;
            Dom.GetUi()._mainPage.SetSourceListFeedback(e); // Set to NVX Stream
            Dom.GetUi()._mainPage.SetSourceFeedback("GlobalStream " + (e - 1)); // Set the source text feedback
            string location = Dom.GetAuto().GetNvxAddress((e - 2));
            Dom.GetDev().MyNvx.SetStreamLocation(location);
            Dom.GetUi()._myContract.NvxInfo.NvxAddressFb_Indirect(location);
        }
        /// <summary>
        /// Method for displaying the airmedia information page.
        /// @MM
        /// </summary>
        public void ShowAirMediaPage()
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowAirMediaPage", "Current Page 4", 0);
            _myContract.MainPage.MediaControl.MediaControl_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility((sig, m) => sig.BoolValue = true);
            _currentPage= 4;

            //Feedback is handled directly from the "UpdateInformation" method in the AirMedia3100 class
        }
        /// <summary>
        /// Method for displaying the airmedia information page.
        /// @MM
        /// </summary>
        /// <param name="e">Ui button index value</param>
        public void ShowAirMediaPage(ushort e)
        {
            LogusMaximus.LogusDecimusMaximus(LogHeader, "ShowAirMediaPage", "Current Page 4", 0);
            _myContract.MainPage.MediaControl.MediaControl_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.NvxInfo.NvxInfo_Visibility((sig, m) => sig.BoolValue = false);
            _myContract.MainPage.AirMediaInfo.AirMediaInfo_Visibility((sig, m) => sig.BoolValue = true);
            _currentPage = 4;
            Dom.GetUi()._mainPage.SetSourceListFeedback(e); // Set to NVX HDMI input 2
            Dom.GetUi()._mainPage.SetSourceFeedback("AirMedia"); // Set the source text feedback

            //Feedback is handled directly from the "UpdateInformation" method in the AirMedia3100 class
        }
        #endregion

        #region Page Reload Method
        /// <summary>
        /// Method used for reloading the currently "loaded" page.
        /// </summary>
        public void ReloadCurrentPage()
        {
            switch (_currentPage)
            {
                case 0:
                    ShowStartPage();
                    break;
                case 1:
                    ShowMainPage();
                    break;
                case 2:
                    ShowMediaPage();
                    break;
                case 3:
                    ShowNvxPage();
                    break;
                case 4:
                    ShowAirMediaPage();
                    break;
            }
        }
        #endregion

        #region Power Off Confirmation Methods
        /// <summary>
        /// Method which displays the power off confirmation page.
        /// </summary>
        public void ShowPowerOffSubpage()
        {
            // You still have to show the subpage on that page. so you will have to trigger visibility on every page the subpage exists on.
            _myContract.MainPage.PowerOffOk.PowerOffOk_Visibility(true);
            
        }
        /// <summary>
        /// Method which hides the power off confirmation page.
        /// </summary>
        public void HidePowerOffSubpage()
        {
            _myContract.MainPage.PowerOffOk.PowerOffOk_Visibility(false);
        }
        #endregion
        
        #region MessageBroker Methods
        /// <summary>
        /// Media control page method to accept a message to fit the signature.
        /// </summary>
        /// <param name="m">MessageBroker message parameter</param>
        private void ShowMediaPlayPage(MessageSystem.Message m)
        {
            ShowMediaPage();
        }
        /// <summary>
        /// System start page method to accept a message to fit the signature.
        /// </summary>
        /// <param name="m">MessageBroker message parameter</param>
        private void ShowSystemStartPage(MessageSystem.Message m)
        {
            ShowStartPage();
        }
        /// <summary>
        /// Main page method to accept a message to fit the signature.
        /// </summary>
        /// <param name="m">MessageBroker message parameter</param>
        private void ShowSystemMainPage(MessageSystem.Message m)
        {
            ShowMainPage();
        }
        #endregion

        #region Event Handler Methods
        /// <summary>
        /// Event handler for the home button on the media page.
        /// </summary>
        /// <remarks>
        /// They need to add this as well for the first lab
        /// </remarks>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void MediaPage_HomeButton_PressEvent(object sender, UIEventArgs e)
        {
            ShowMainPage();
        }
        /// <summary>
        /// Event handler for the start page button, another exists in "Xpanel.cs".
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void StartPage_Button_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                ShowMainPage();
            }
        }
        /// <summary>
        /// Event handler for the power button in the header.
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void MainPage_PowerButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                ShowPowerOffSubpage();
            }
        }
        /// <summary>
        /// Event handler for the no button on the power off confirmation page.
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void PowerOffOkSubpage_PowerOffNoButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                HidePowerOffSubpage();
            }
        }
        /// <summary>
        /// Event handler for the yes button on the power off confirmation page.
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void PowerOffOkSubpage_PowerOffYesButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                HidePowerOffSubpage();
            }
        }
        #endregion

    }
}
