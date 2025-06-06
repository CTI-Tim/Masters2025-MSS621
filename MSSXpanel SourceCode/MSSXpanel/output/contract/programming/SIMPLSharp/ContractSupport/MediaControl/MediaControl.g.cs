//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by CrestronConstruct.
//     AppHost:     2.201.20.0
//     UI Plugin:   1.3801.23.0
//
//     Project:     MSSXpanel
//     Version:     1.0.0.0
//     Sdk:         CH5:2.12.0
//     Strategy:    Modern
//     IndexOnly:   False
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using MSSXpanel;

namespace MSSXpanel.MediaControl
{

    /// <summary>
    /// MediaControl
    /// </summary>
    public partial interface IMediaControl 
    {
        object UserObject { get; set; }

        /// <summary>
        /// Event BackButton.Press (from panel to Control System)
        /// </summary>
        event EventHandler<UIEventArgs> BackButton_PressEvent;

        /// <summary>
        /// Event MenuButton.Press (from panel to Control System)
        /// </summary>
        event EventHandler<UIEventArgs> MenuButton_PressEvent;

        /// <summary>
        /// Event PlayButton.Press (from panel to Control System)
        /// </summary>
        event EventHandler<UIEventArgs> PlayButton_PressEvent;

        /// <summary>
        /// BackButton.Selected Feedback
        /// </summary>
        /// <param name="callback">The bool delegate to update the panel.</param>
        void BackButton_Selected(MediaControlBoolInputSigDelegate callback);

        /// <summary>
        /// BackButton.Selected Feedback
        /// </summary>
        /// <param name="digital">The bool to update the panel.</param>
        void BackButton_Selected(bool digital);

        /// <summary>
        /// MenuButton.Selected Feedback
        /// </summary>
        /// <param name="callback">The bool delegate to update the panel.</param>
        void MenuButton_Selected(MediaControlBoolInputSigDelegate callback);

        /// <summary>
        /// MenuButton.Selected Feedback
        /// </summary>
        /// <param name="digital">The bool to update the panel.</param>
        void MenuButton_Selected(bool digital);

        /// <summary>
        /// PlayButton.Selected Feedback
        /// </summary>
        /// <param name="callback">The bool delegate to update the panel.</param>
        void PlayButton_Selected(MediaControlBoolInputSigDelegate callback);

        /// <summary>
        /// PlayButton.Selected Feedback
        /// </summary>
        /// <param name="digital">The bool to update the panel.</param>
        void PlayButton_Selected(bool digital);

        /// <summary>
        /// ComplexComponent Dpad
        /// </summary>
        MSSXpanel.MediaControl.IDpad Dpad { get; }
    }

    /// <summary>
    /// Digital callback used in feedback events.
    /// </summary>
    /// <param name="boolInputSig">The <see cref="BoolInputSig"/> joinInfo data.</param>
    /// <param name="mediacontrol">The <see cref="IMediaControl"/> on which to apply the feedback.</param>
    public delegate void MediaControlBoolInputSigDelegate(BoolInputSig boolInputSig, IMediaControl mediacontrol);

    /// <summary>
    /// MediaControl
    /// </summary>
    internal partial class MediaControl : IMediaControl, IDisposable
    {
        #region Standard CH5 Component members

        private ComponentMediator ComponentMediator { get; set; }

        public object UserObject { get; set; }

        /// <summary>
        /// Gets the ControlJoinId a.k.a. SmartObjectId.  This Id identifies the extender symbol.
        /// </summary>
        public uint ControlJoinId { get; private set; }

        private IList<BasicTriListWithSmartObject> _devices;

        /// <summary>
        /// Gets the list of devices.
        /// </summary>
        public IList<BasicTriListWithSmartObject> Devices { get { return _devices; } }

        #endregion

        #region Joins

        private static class Joins
        {
            /// <summary>
            /// Digital signals,
            /// </summary>
            internal static class Booleans
            {
                /// <summary>
                /// Output or Event digital joinInfo from panel to Control System: MediaControl.BackButton.Press
                /// BackButton.Press
                /// </summary>
                public const uint BackButton_PressEvent = 3;

                /// <summary>
                /// Output or Event digital joinInfo from panel to Control System: MediaControl.MenuButton.Press
                /// MenuButton.Press
                /// </summary>
                public const uint MenuButton_PressEvent = 4;

                /// <summary>
                /// Output or Event digital joinInfo from panel to Control System: MediaControl.PlayButton.Press
                /// PlayButton.Press
                /// </summary>
                public const uint PlayButton_PressEvent = 5;


                /// <summary>
                /// Input or Feedback digital joinInfo from Control System to panel: MediaControl.BackButton.Selected
                /// BackButton.Selected
                /// </summary>
                public const uint BackButton_SelectedState = 3;

                /// <summary>
                /// Input or Feedback digital joinInfo from Control System to panel: MediaControl.MenuButton.Selected
                /// MenuButton.Selected
                /// </summary>
                public const uint MenuButton_SelectedState = 4;

                /// <summary>
                /// Input or Feedback digital joinInfo from Control System to panel: MediaControl.PlayButton.Selected
                /// PlayButton.Selected
                /// </summary>
                public const uint PlayButton_SelectedState = 5;

            }
        }

        #endregion

        #region Construction and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaControl"/> component class.
        /// </summary>
        /// <param name="componentMediator">The <see cref="ComponentMediator"/> used to instantiate the component.</param>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        /// <param name="itemCount">The number of items.</param>
        internal MediaControl(ComponentMediator componentMediator, uint controlJoinId, uint? itemCount)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId, itemCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaControl"/> component class.
        /// </summary>
        /// <param name="componentMediator">The <see cref="ComponentMediator"/> used to instantiate the component.</param>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        internal MediaControl(ComponentMediator componentMediator, uint controlJoinId) : this(componentMediator, controlJoinId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance with default itemCount.
        /// </summary>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        private void Initialize(uint controlJoinId)
        {
            Initialize(controlJoinId, null);
        }

        private Dictionary<string, Indexes> _indexLookup = new Dictionary<string, Indexes>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaControl"/> component class.
        /// </summary>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        /// <param name="itemCount">The number of items.</param>
        private void Initialize(uint controlJoinId, uint? itemCount)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.BackButton_PressEvent, onBackButton_Press);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.MenuButton_PressEvent, onMenuButton_Press);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.PlayButton_PressEvent, onPlayButton_Press);
            Dpad = new MSSXpanel.MediaControl.Dpad(ComponentMediator, 15);
        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);

            ((MSSXpanel.MediaControl.Dpad)Dpad).AddDevice(device);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);

            ((MSSXpanel.MediaControl.Dpad)Dpad).RemoveDevice(device);
        }

        #endregion

        #region CH5 Contract

        /// <inheritdoc/>
        public event EventHandler<UIEventArgs> BackButton_PressEvent;
        private void onBackButton_Press(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = BackButton_PressEvent;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        /// <inheritdoc/>
        public event EventHandler<UIEventArgs> MenuButton_PressEvent;
        private void onMenuButton_Press(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = MenuButton_PressEvent;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        /// <inheritdoc/>
        public event EventHandler<UIEventArgs> PlayButton_PressEvent;
        private void onPlayButton_Press(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = PlayButton_PressEvent;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        /// <inheritdoc/>
        public void BackButton_Selected(MediaControlBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.BackButton_SelectedState], this);
            }
        }

        /// <inheritdoc/>
        public void BackButton_Selected(bool digital)
        {
            BackButton_Selected((sig, component) => sig.BoolValue = digital);
        }
        /// <inheritdoc/>
        public void MenuButton_Selected(MediaControlBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.MenuButton_SelectedState], this);
            }
        }

        /// <inheritdoc/>
        public void MenuButton_Selected(bool digital)
        {
            MenuButton_Selected((sig, component) => sig.BoolValue = digital);
        }
        /// <inheritdoc/>
        public void PlayButton_Selected(MediaControlBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.PlayButton_SelectedState], this);
            }
        }

        /// <inheritdoc/>
        public void PlayButton_Selected(bool digital)
        {
            PlayButton_Selected((sig, component) => sig.BoolValue = digital);
        }

        /// <summary>
        /// ComplexComponent Dpad
        /// </summary>
        public MSSXpanel.MediaControl.IDpad Dpad { get; private set; }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return (int)ControlJoinId;
        }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "MediaControl", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            BackButton_PressEvent = null;
            MenuButton_PressEvent = null;
            PlayButton_PressEvent = null;
        }

        #endregion
    }
}
