using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSXpanel;                    // Bring in your contract namespace

namespace Masters_2024_MSS_521.UserInterface
{
    /// <summary>
    /// Class definition for the media subpage (apple tv).
    /// </summary>
    public class MediaSubPage
    {
        #region Object Declarations
        /// <summary>
        /// Logging header for easier reading.
        /// @MM
        /// </summary>
        private const string LogHeader = "[xMediaSubPage] -> ";

        /// <summary>
        /// Crestron construct contract object.
        /// </summary>
        private MSSXpanel.Contract _myContract;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for the media subpage class.
        /// </summary>
        /// <remarks>
        /// A crestron contract object is instantiated and several button event handlers are defined.
        /// </remarks>
        /// <param name="myContract">Crestron construct object parameter</param>
        public MediaSubPage(Contract myContract)
        {
            _myContract = myContract;
            _myContract.MediaControl.Dpad.Button_PressEvent += Dpad_Button_PressEvent;
            _myContract.MediaControl.BackButton_PressEvent += MediaPage_BackButton_PressEvent;
            _myContract.MediaControl.MenuButton_PressEvent += MediaPage_MenuButton_PressEvent;
            _myContract.MediaControl.PlayButton_PressEvent += MediaPage_PlayButton_PressEvent;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for the play/pause button on the media page.
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void MediaPage_PlayButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                LogusMaximus.LogusDecimusMaximus(LogHeader, "Play Button Press", e.SigArgs.Sig.Name, 0);
            else
                LogusMaximus.LogusDecimusMaximus(LogHeader, "Play Button Release", e.SigArgs.Sig.Name, 0);
        }
        /// <summary>
        /// Event handler for the menu button on the media page.
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void MediaPage_MenuButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                LogusMaximus.LogusDecimusMaximus(LogHeader, "Menu Button Press", e.SigArgs.Sig.Name, 0);
            else
                LogusMaximus.LogusDecimusMaximus(LogHeader, "Menu Button Release", e.SigArgs.Sig.Name, 0);
        }
        /// <summary>
        /// Event handler for the back button on the media page.
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void MediaPage_BackButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                LogusMaximus.LogusDecimusMaximus(LogHeader, "Back Button Press", e.SigArgs.Sig.Name, 0);
            else
                LogusMaximus.LogusDecimusMaximus(LogHeader, "Back Button Release", e.SigArgs.Sig.Name, 0);
        }
        /// <summary>
        /// Event handler for the d-pad buttons on the media page.
        /// </summary>
        /// <param name="sender">Touch panel object which generated this event</param>
        /// <param name="e">Arguments with respect to the touch panel object which generated this event</param>
        private void Dpad_Button_PressEvent(object sender, DpadEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
                LogusMaximus.LogusDecimusMaximus(LogHeader, "D-Pad Button Press", e.SigArgs.Sig.Name, 0);
            else
                LogusMaximus.LogusDecimusMaximus(LogHeader, "D-Pad Button Release", e.SigArgs.Sig.Name, 0);
        }
        #endregion
    }
}
