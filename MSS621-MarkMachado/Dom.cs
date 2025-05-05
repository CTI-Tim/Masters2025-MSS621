using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;
using Masters_2024_MSS_521.Devices;
using Masters_2024_MSS_521.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masters_2024_MSS_521
{
    /// <summary>
    /// Program-wide stash for the objects to buzz around where they're needed.
    /// </summary>
    /// <remarks>
    /// This is the polar opposite of the MessageBroker, and a clear example of what it looks to resolve.
    /// </remarks>
    public static class Dom
    {
        #region Object Declarations
        /// <summary>
        /// Crestron processor object
        /// @MM
        /// </summary>
        private static CrestronControlSystem _cs;

        /// <summary>
        /// Interface object
        /// @MM
        /// </summary>
        private static Xpanel _ui;

        /// <summary>
        /// Display object
        /// @MM
        /// </summary>
        private static CrestronConnected _disp;

        /// <summary>
        /// Automation object
        /// @MM
        /// </summary>
        private static Automation _auto;

        /// <summary>
        /// Devices object
        /// @MM
        /// </summary>
        private static DeviceSetup _dev;
        #endregion

        #region Set Methods
        /// <summary>
        /// Method for passing in the control system object.
        /// @MM
        /// </summary>
        /// <param name="cs">Control system object being passed in</param>
        public static void SetCs(CrestronControlSystem cs)
        {
            _cs = cs;
        }

        /// <summary>
        /// Method for passing in the interface object.
        /// @MM
        /// </summary>
        /// <param name="ui">Interface object being passed in</param>
        public static void SetUi(Xpanel ui)
        {
            _ui = ui;
        }

        /// <summary>
        /// Method for setting display object.
        /// @MM
        /// </summary>
        /// <param name="disp">Display object being passed in</param>
        public static void SetDisp(CrestronConnected disp)
        {
            _disp = disp;
        }

        /// <summary>
        /// Method for setting automation object.
        /// </summary>
        /// <param name="auto">Automation object being passed in</param>
        public static void SetAuto(Automation auto)
        {
            _auto = auto;
        }

        /// <summary>
        /// Method for setting devices object.
        /// </summary>
        /// <param name="dev">Devices object being passed in</param>
        public static void SetDev(DeviceSetup dev)
        {
            _dev = dev;
        }
        #endregion

        #region Get Methods
        /// <summary>
        /// Method to grab display object.
        /// @MM
        /// </summary>
        /// <returns>Display object</returns>
        public static CrestronConnected GetDisp()
        {
            return _disp;
        }

        /// <summary>
        /// Method to grab ui object.
        /// </summary>
        /// <returns>Ui object</returns>
        public static Xpanel GetUi()
        {
            return _ui;
        }

        /// <summary>
        /// Method to grab control system object.
        /// </summary>
        /// <returns></returns>
        public static CrestronControlSystem GetControlSystem()
        {
            return _cs;
        }

        /// <summary>
        /// Method to grab automation object.
        /// </summary>
        /// <returns></returns>
        public static Automation GetAuto()
        {
            return _auto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DeviceSetup GetDev()
        {
            return _dev;
        }
        #endregion
    }
}
