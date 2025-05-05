using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.AirMedia;

namespace Masters_2025_MSS_621_JW.Devices
{
    /*
     * This is a class framework for BASIC addition operation of a Crestron Am3100 Airmedia device.
     * There is a lot more functions and capabilities that can be added to this class by the programmer.
     * This is for learning purposes and therefore simplified for clarity of understanding. It is up to the
     * Programmer to add in any security and data checks before using this in a production system.
     *
     * It is a good idea and a best practice to write yourself consistent wrapper framework classes like this for
     * all hardware of a type making sure they are consistent with each other  inputs and outputs identical in operation
     * and use as much as possible.  These classes can simplify your programming down the road.
     *
     * This would be a great place to leverage Interfaces to enforce compatibility.
     */

    public class AirMedia3100
    {
        public delegate void PinCodeHandler(ushort pin);
        public delegate void AddressHandler(string address);

        public event PinCodeHandler PinCodeChanged;
        public event AddressHandler AddressChanged;

        public enum ELoginCodeMode // This matches  eAirMediaLoginCodeMode
        {
            Disabled,
            Random,
            Fixed
        }

        private readonly Am3100 _myAm3100;

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
                SetPinCodeRandom();
            }
        }

        public bool Online { get; private set; }
        public bool Registered { get; private set; }
        public ushort CurrentPinCode { get; private set; }
        public ELoginCodeMode PinCodeMode { get; private set; }
        public ushort NumberOfUsers { get; private set; }
        public string CurrentAddress { get; private set; }
        public string CurrentHostName { get; private set; }


        //public Methods


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


        // Private Methods

        private void UpdateInformation() // Update our properties so they are always accurate
        {
            Online = _myAm3100.IsOnline;
            if (_myAm3100.AirMedia.LoginCodeFeedback.UShortValue != CurrentPinCode) {
                CurrentPinCode = _myAm3100.AirMedia.LoginCodeFeedback.UShortValue;
                PinCodeChanged?.Invoke(CurrentPinCode);
            }
            if (_myAm3100.AirMedia.IpAddressFeedback.StringValue != CurrentAddress) {
                CurrentAddress = _myAm3100.AirMedia.IpAddressFeedback.StringValue;
                AddressChanged?.Invoke(CurrentAddress);
            }
            PinCodeMode = (ELoginCodeMode)_myAm3100.AirMedia.LoginCodeModeFeedback;
            NumberOfUsers = _myAm3100.AirMedia.NumberOfUsersConnectedFeedback.UShortValue;
            CurrentHostName = _myAm3100.AirMedia.HostNameFeedback.StringValue;
        }

        // Event Handlers

        private void MyAm3100_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            UpdateInformation();
        }

        private void MyAm3100_BaseEvent(GenericBase device, BaseEventArgs args)
        {
            UpdateInformation();
        }

        private void AirMedia_AirMediaChange(object sender, GenericEventArgs args)
        {
            UpdateInformation();
        }

    }
}