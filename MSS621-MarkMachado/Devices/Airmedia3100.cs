﻿using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.AirMedia;

namespace Masters_2024_MSS_521.Devices
{
    /// <summary>
    /// This is a class framework for BASIC addition operation of a Crestron Am3100 Airmedia device.
    /// There is a lot more functions and capabilities that can be added to this class by the programmer.
    /// This is for learning purposes and therefore simplified for clarity of understanding. It is up to the
    /// Programmer to add in any security and data checks before using this in a production system.
    /// </summary>
    /// <remarks>
    /// It is a good idea and a best practice to write yourself consistent wrapper framework classes like this for
    /// all hardware of a type making sure they are consistent with each other  inputs and outputs identical in operation
    /// and use as much as possible.  These classes can simplify your programming down the road.
    /// This would be a great place to leverage Interfaces to enforce compatibility.
    /// </remarks>
    public class AirMedia3100
    {
        #region Enums
        /// <summary>
        /// AirMedia pin code mode enum values.
        /// </summary>
        public enum ELoginCodeMode // This matches  eAirMediaLoginCodeMode
        {
            Disabled,
            Random,
            Fixed
        }
        #endregion

        #region Object Declarations
        /// <summary>
        /// AirMedia object declaration.
        /// </summary>
        private readonly Am3100 _myAm3100;
        #endregion

        #region Constructors
        /// <summary>
        ///     Create a new Airmedia AM3100 device in the Program. Device will be created and registered
        ///     Error log will be written to if unable to register and the Registered property will be set to false
        /// </summary>
        /// <param name="cs">The ControlSystem object it needs to be connected to</param>
        /// <param name="ipId">The IPID in hex that the device should be programmed to</param>
        public AirMedia3100(uint ipId, CrestronControlSystem cs)
        {
            _myAm3100 = new Am3100(ipId, cs);
            _myAm3100.BaseEvent += MyAm3100_BaseEvent;
            _myAm3100.OnlineStatusChange += MyAm3100_OnlineStatusChange;
            _myAm3100.AirMedia.AirMediaChange += AirMedia_AirMediaChange;
            _myAm3100.Register();

            if (_myAm3100.RegistrationFailureReason !=
                eDeviceRegistrationUnRegistrationFailureReason.DEVICE_REG_UNREG_RESPONSE_NO_FAILURE)
            {
                Online = false;
                Registered = false;
                ErrorLog.Error($"## Error Unable to register AM3100 at IPID {ipId:X}");
            }
            else
            {
                Registered = true;
            }
        }
        #endregion

        #region Properties
        public bool Online { get; private set; }
        public bool Registered { get; private set; }
        public ushort CurrentPinCode { get; private set; }
        public ELoginCodeMode PinCodeMode { get; private set; }
        public ushort NumberOfUsers { get; private set; }
        public string CurrentAddress { get; private set; }
        public string CurrentHostName { get; private set; }
        #endregion

        #region Events
        public event EventHandler<Args> BaseEvent;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Sets the Pin Code for AirMedia Access.  IF set to 0 it will Disable the PinCode. If set to any
        ///     Number from 1 to 9999 it will enable Fixed Pin code mode and set that number as the pincode
        ///     Any number above 9999 will be set to 0 and PinCode Disabled
        /// </summary>
        /// <param name="pinCode"> (USHORT) Number from 0001 to 9999</param>
        public void SetPinCode(ushort pinCode)
        {
            if (pinCode > 9999) // If the number is larger than 4 digits invalidate it
                pinCode = 0;

            if (pinCode < 1)
            {
                _myAm3100.AirMedia.LoginCodeMode(eAirMediaLoginCodeMode.Disabled);
            }
            else
            {
                _myAm3100.AirMedia.LoginCodeMode(eAirMediaLoginCodeMode.Fixed);
                _myAm3100.AirMedia.LoginCode.UShortValue = pinCode;
            }

            UpdateInformation();
        }

        /// <summary>
        ///     Sets the Airmedia login code mode to random
        /// </summary>
        public void SetPinCodeRandom()
        {
            _myAm3100.AirMedia.LoginCodeMode(eAirMediaLoginCodeMode.Random);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Method which updates stored values about the AirMedia unit.
        /// </summary>
        /// <remarks>
        /// This method also sends pertinent information to the touch panel.
        /// </remarks>
        private void UpdateInformation() // Update our properties so they are always accurate
        {
            Online = _myAm3100.IsOnline;
            CurrentPinCode = _myAm3100.AirMedia.LoginCodeFeedback.UShortValue;
            PinCodeMode = (ELoginCodeMode)_myAm3100.AirMedia.LoginCodeModeFeedback;
            NumberOfUsers = _myAm3100.AirMedia.NumberOfUsersConnectedFeedback.UShortValue;
            CurrentAddress = _myAm3100.AirMedia.IpAddressFeedback.StringValue;
            CurrentHostName = _myAm3100.AirMedia.HostNameFeedback.StringValue;

            // Feedback for pinCode and address on the touch panel.
            string pinCode = CurrentPinCode.ToString();
            string ipAddr = CurrentAddress.ToString();

            Dom.GetUi()._myContract.AirMediaInfo.AirmediaPinFb_Indirect(pinCode);
            Dom.GetUi()._myContract.AirMediaInfo.AirmediaAddressFb_Indirect(ipAddr);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event handler for online and offline status changes for the airmedia.
        /// </summary>
        /// <param name="currentDevice">AirMedia object which has triggered the event</param>
        /// <param name="args">Arguments with respect to the airmedia which has triggered the event</param>
        private void MyAm3100_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            UpdateInformation();
            OnRaiseEvent(new Args("OnlineOffline"));
        }
        /// <summary>
        /// Event handler for "base" events for airmedia.
        /// </summary>
        /// <param name="device">AirMedia object which has triggered the event</param>
        /// <param name="args">Arguments with respect to the airmedia which has triggered the event</param>
        private void MyAm3100_BaseEvent(GenericBase device, BaseEventArgs args)
        {
            UpdateInformation();
            OnRaiseEvent(new Args("BaseEvent"));
        }
        /// <summary>
        /// Event handler for "changes" to the airmedia.
        /// </summary>
        /// <param name="sender">AirMedia object which has triggered the event</param>
        /// <param name="args">Arguments with respect to the airmedia which has triggered the event</param>
        private void AirMedia_AirMediaChange(object sender, GenericEventArgs args)
        {
            UpdateInformation();
            OnRaiseEvent(new Args("AirMediaChange"));
        }
        #endregion

        #region Class Event
        /// <summary>
        /// Class Event
        /// </summary>
        /// <param name="e">Arguments parameter for new arguments</param>
        protected virtual void OnRaiseEvent(Args e)
        {
            // we make a copy because if subscribers unsubscribed during the event it can cause an exception to be thrown
            var raiseEvent = BaseEvent;
            if (raiseEvent != null)
            {
                e.Online = _myAm3100.IsOnline;
                e.Registered = _myAm3100.Registered;
                e.Users = _myAm3100.AirMedia.NumberOfUsersConnectedFeedback.UShortValue;
                e.CurrentPinCode = _myAm3100.AirMedia.LoginCodeFeedback.UShortValue;
                e.PinCodeMode = (ELoginCodeMode)_myAm3100.AirMedia.LoginCodeModeFeedback;
                e.CurrentAddress = _myAm3100.AirMedia.IpAddressFeedback.StringValue;
                e.CurrentHostName = _myAm3100.AirMedia.HostNameFeedback.StringValue;

                raiseEvent(this, e); // Fire the event
            }
        }

        /// <summary>
        /// Nested class used define our custom event args.
        /// </summary>
        public class Args
        {
            /// <summary>
            /// Constructor for the args class.
            /// </summary>
            /// <param name="message">Parameter for passing in custom messages</param>
            public Args(string message)
            {
                Message = message;
            }
            // Collection of properties which define the custom args
            public string Message { get; set; }
            public bool Online { get; set; }
            public bool Registered { get; set; }
            public uint Users { get; set; }
            public ushort CurrentPinCode { get; set; }
            public ELoginCodeMode PinCodeMode { get; set; }
            public string CurrentAddress { get; set; }
            public string CurrentHostName { get; set; }

            #endregion
        }
    }
}