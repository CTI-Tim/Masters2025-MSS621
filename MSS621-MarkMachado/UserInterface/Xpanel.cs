using System;
using System.IO;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using Independentsoft.Exchange.Autodiscover;
using Masters_2024_MSS_521.MessageSystem;
using MSSXpanel;// Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    /// <summary>
    /// Class representing the "Xpanel" object in the program.
    /// </summary>
    /// <remarks>
    /// Do be aware that there is an "Xpanel" class definition inside of "Crestron.SimplSharpPro.Ui" and that it is
    /// critically important to call upon the correct "Xpanel" when manipulating things within this program.
    /// </remarks>
    /// <see cref="ms-its:C:\Program Files (x86)\Crestron\Cresdb\Help\SIMPLSharpPro.chm::/html/16803fd9-ed25-1975-1036-2fcfe01b4792.htm"/>
    public class Xpanel
    {
        #region Object Declarations
        /// <summary>
        /// Logging header for easier reading.
        /// @MM
        /// </summary>
        private const string LogHeader = "[xUi] -> ";
        /// <summary>
        /// Get the current working directory.
        /// @MM
        /// </summary>
        string appDir = Directory.GetCurrentDirectory();
        /// <summary>
        /// Crestron processor object
        /// @MM
        /// </summary>
        private CrestronControlSystem _cs;

        public MainPage _mainPage;
        private MediaSubPage _mediaPage;
        public readonly Contract _myContract;
        private readonly XpanelForHtml5 _myXpanel;

        public readonly PageNavigation _navigation;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for the panel class.
        /// </summary>
        /// <remarks>
        /// Do be aware that there is an "Xpanel" class definition inside of "Crestron.SimplSharpPro.Ui" and that it is
        /// critically important to call upon the correct "Xpanel" when manipulating things within this program.
        /// </remarks>
        /// <see cref="ms-its:C:\Program Files (x86)\Crestron\Cresdb\Help\SIMPLSharpPro.chm::/html/16803fd9-ed25-1975-1036-2fcfe01b4792.htm"/>
        /// <param name="ipId">IP ID of the touch panel</param>
        /// <param name="cs">Control system object</param>
        public Xpanel(uint ipId, ControlSystem cs)
        {
            _myXpanel = new XpanelForHtml5(ipId, cs);
            _myXpanel.OnlineStatusChange += _myXpanel_OnlineStatusChange;
            _myXpanel.Register();


            _myContract = new Contract();
            _myContract.AddDevice(_myXpanel);

            // Bring in the pages classes
            _navigation = new PageNavigation(_myContract);
            _mainPage = new MainPage(_myContract);
            _mediaPage = new MediaSubPage(_myContract);

            // subscribing to the power buttons here to trigger their automation
            _myContract.StartPage.Button_PressEvent += StartPage_Button_PressEvent; // On
            _myContract.PowerOffOk.PowerOffYesButton_PressEvent +=
                PowerOffOkSubpage_PowerOffYesButton_PressEvent; //off

            // Lets set the room name header bar 
            _myContract.HeaderBar.RoomNameLabel_Indirect("MSS-521 Conference Room");


            // Customer wants the system to flip to the start page on power loss or program load
            _navigation.ShowStartPage();

            // Pass in objects @MM
            _mainPage.SetCs(_cs);
            _mainPage.SetUi(_myXpanel);

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
        #endregion

        #region Private Methods
        /// <summary>
        /// If the xpanel goes offline or online this event handler will make sure we go back to the page the program wants us on
        /// </summary>
        /// <param name="currentDevice">Touch panel object that generated the event</param>
        /// <param name="args">Arguments with respect to the touch panel object that generated the event</param>
        private void _myXpanel_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            _navigation.ReloadCurrentPage();
        }
        /// <summary>
        /// Start page event handler.
        /// </summary>
        /// <param name="sender">Touch panel object that generated the event</param>
        /// <param name="e">Arguments with respect to the touch panel that generated the event</param>
        private void StartPage_Button_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                //MessageBroker.SendMessage("SystemOn", new Message()); //trigger the system on automation

                Dom.GetDisp().On();
                Dom.GetUi()._navigation.ShowMainPage();
                Dom.GetUi()._navigation.ShowAirMediaPage(1);
            }
        }
        /// <summary>
        /// Power off "yes" button event handler.
        /// </summary>
        /// <param name="sender">Touch panel object that generated the event</param>
        /// <param name="e">Arguments with respect to the touch panel that generated the event</param>
        private void PowerOffOkSubpage_PowerOffYesButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                //MessageBroker.SendMessage("SystemOff", new Message()); //trigger the system off automation

                Dom.GetDisp().Off();
                Dom.GetDev().MyNvx.SetInput(Devices.Nvx351.ESource.Disable);
                Dom.GetUi()._navigation.ShowStartPage();
            }
        }
        #endregion
    }
}