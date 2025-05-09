﻿using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;

namespace Masters_2024_MSS_521.Devices
{
    /// <summary>
    /// WE are only going to program the basics here for the display. Power, Input Select and Volume+mute
    /// this will cover 100% of what we need for this conference room. If you need more features in your design
    /// feel free to add them to this class and expand your device capabilities. You do not have to program 100% of all device
    /// capabilities into it's class. I have never needed to give a customer the ability to adjust keystone from the touchpanel
    /// and in most cases giving the user a way to break something is a bad idea. So why waste time adding those features?
    /// </summary>
    public class CrestronConnected
    {
        #region Object Declarations
        /// <summary>
        /// Crestron processor object
        /// @MM
        /// </summary>
        private CrestronControlSystem _cs;

        private readonly CrestronConnectedDisplayV2 _myDisplay;

        public CrestronConnected(uint ipId, CrestronControlSystem cs)
        {
            _myDisplay = new CrestronConnectedDisplayV2(ipId, cs);
            _myDisplay.OnlineStatusChange += MyDisplay_OnlineStatusChange;
            _myDisplay.BaseEvent += MyDisplay_BaseEvent;
            
            _myDisplay.Audio.MuteOff();

            _myDisplay.Register();
            if (_myDisplay.RegistrationFailureReason != eDeviceRegistrationUnRegistrationFailureReason
                    .DEVICE_REG_UNREG_RESPONSE_NO_FAILURE)
            {
                RegisteredFb = false;
                ErrorLog.Error($"Unable to register Crestron Connected display at IpID {ipId:X2}");
            }
            else
            {
                RegisteredFb = true;
            }
        }
        #endregion

        #region Properties
        public bool OnlineFb { get; private set; }
        public ushort SourceFb { get; private set; }
        public bool OnFb { get; private set; }
        public bool RegisteredFb { get; }
        #endregion

        #region Events
        public event EventHandler<Args> BaseEvent;

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
        /// Method for getting the display.
        /// @MM
        /// </summary>
        /// <returns>Display object</returns>
        public CrestronConnectedDisplayV2 GetDisplay()
        {
            return _myDisplay;
        }
        /// <summary>
        /// 
        /// </summary>
        public void On()
        {
            _myDisplay.Power.PowerOn();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Off()
        {
            _myDisplay.Power.PowerOff();
        }

        /*
         * We are using a method to change the input number.  You do not have to do it this way.
         * Another way is to create a property that when it is set runs code to set the myDisplay.Video.Source.SourceSelect value
         * as soon as it is changed.   Some programmers add both to the module to give them more flexibility in programming.
         */

        public void Input(ushort num)
        {
            // Make sure we dont select a source number greatrer than the device can handle
            var numSources = _myDisplay.Video.Source.SourceCountFeedback.UShortValue;
            if (num > numSources) num = numSources;
            _myDisplay.Video.Source.SourceSelect.UShortValue = num;
        }

        /// <summary>
        ///     Used to set the volume directly. For example a forces startup volume.
        /// </summary>
        /// <param name="volume">value  0-65535</param>
        public void Volume(ushort volume)
        {
            _myDisplay.Audio.Volume.UShortValue = volume;
        }

        /*
         *  Crestron equipment analogs have the ability to create ramps that will automatically
         *  Increase or decrease the value for us at the speed we define.
         *  This greatly simplifies things like volume level changes.
         */

        //TODO: Student needs to explore what you would do for a 3rd party display

        // NOTE: data coming back from a device is NEVER smooth and complete.  it can be and most of the time will be jumpy

        /// <summary>
        ///     Creates a ramp to ramp UP from the current volume towards max volume
        ///     Mute is automatically turned off
        /// </summary>
        public void VolumeUp()
        {
            _myDisplay.Audio.MuteOff();
            _myDisplay.Audio.Volume.CreateRamp(65535, 500); //5 seconds
        }

        /// <summary>
        ///     Creates a ramp DOWN from the current volume to min volume
        ///     Mute is automatically turned off
        /// </summary>
        public void VolumeDown()
        {
            _myDisplay.Audio.MuteOff();
            _myDisplay.Audio.Volume.CreateRamp(0, 500);
        }

        /// <summary>
        ///     Stops the Ramp wherever it is currently at
        /// </summary>
        public void VolumeStop()
        {
            _myDisplay.Audio.Volume.StopRamp();
        }

        /// <summary>
        /// 
        /// </summary>
        public void MuteOn()
        {
            _myDisplay.Audio.MuteOn();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MuteOff()
        {
            _myDisplay.Audio.MuteOff();
        }
        /// <summary>
        /// 
        /// </summary>
        public void MuteToggle()
        {
            _myDisplay.Audio.MuteToggle();
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// a drawback of setting these properties here instead of programming the properties to load
        /// and return them in the property it's self is that they could be set with old data.
        /// keep this in mind as to how important the information is. You may want code in the property to update
        /// this when you read them.
        /// </summary>
        /// <remarks>
        /// Also Base events are REALLY chatty. you will get the same things firing over and over.  it is not a bad idea to filter
        /// by event ID and then make your own custom events for separate things like volume. filtering things out
        /// will improve your programs performance as you don't have to process things you do not care about.
        /// </remarks>
        /// <param name="device">Device object which triggered the event</param>
        /// <param name="args">Arguments with respect to the device which triggered the event</param>
        private void MyDisplay_BaseEvent(GenericBase device, BaseEventArgs args)
        {
            switch (args.EventId)
            {
                case 37: // Volume Change found in the enum RoomViewConnectedDisplay.VolumeFeedbackEventId
                {
                    OnRaiseEvent(new Args("Volume"));
                    break;
                }

                case 17: //UnMute Feedback found in RoomViewConnectedDisplay.MuteOnFeedbackEventId
                case 18: //Mute Feedback found in RoomViewConnectedDisplay.MuteOffFeedbackEventId
                    {
                    OnRaiseEvent(new Args("Mute"));
                    break;
                }
                default: //Everything else
                {
                    SourceFb = _myDisplay.Video.Source.CurrentSourceFeedback.UShortValue;
                    OnFb = _myDisplay.Power.PowerOnFeedback.BoolValue;
                    OnRaiseEvent(new Args("BaseEvent"));
                    break;
                }
            }
        }

        /// <summary>
        /// Event handler for the device object online and offline status changes.
        /// </summary>
        /// <param name="currentDevice">Device object which generated the event</param>
        /// <param name="args">Arguments with respect to the device object which generated the event</param>
        private void MyDisplay_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            OnlineFb = args.DeviceOnLine;
            OnRaiseEvent(new Args("OnlineOffline"));
        }
        #endregion

        #region Class Event
        /// <summary>
        /// Class Event
        /// </summary>
        /// <param name="e">Arguments parameter for new arguments</param>
        protected virtual void OnRaiseEvent(Args e)
        {
            // we make a copy because if subscribers un-subscribed during the event it can cause an exception
            var raiseEvent = BaseEvent;
            if (raiseEvent != null)
            {
                e.Online = _myDisplay.IsOnline;
                e.Registered = _myDisplay.Registered;
                e.OnFb = _myDisplay.Power.PowerOnFeedback.BoolValue;
                e.Input = _myDisplay.Video.Source.CurrentSourceFeedback.UShortValue;
                e.MuteOn = _myDisplay.Audio.MuteOnFeedback.BoolValue;
                e.VolumeFb = _myDisplay.Audio.VolumeFeedback.UShortValue;

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
            public bool OnFb { get; set; }
            public ushort Input { get; set; }
            public bool MuteOn { get; set; }
            public ushort VolumeFb { get; set; }

            #endregion
        }
    }
}