//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by CrestronConstruct.
//     Version: 1.3001.21.0
//
//     Project:     MSSXpanel
//     Version:     1.0.0.0
//     Sdk:         CH5:2.7.0
//     Strategy:    Modern
//     IndexOnly:   True
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;
using MSSXpanel;

namespace MSSXpanel
{

    /// <summary>
    /// StartPage
    /// </summary>
    public interface IStartPage 
    {
        object UserObject { get; set; }

        /// <summary>
        /// Button.Press
        /// </summary>
        event EventHandler<UIEventArgs> Button_PressEvent;

        /// <summary>
        /// StartPage.VisibilityJoin Feedback
        /// </summary>
        /// <param name="callback">The bool delegate to update the panel.</param>
        void StartPage_VisibilityJoin(StartPageBoolInputSigDelegate callback);

        /// <summary>
        /// StartPage.VisibilityJoin Feedback
        /// </summary>
        /// <param name="digital">The bool to update the panel.</param>
        void StartPage_VisibilityJoin(bool digital);

        /// <summary>
        /// Button.Selected Feedback
        /// </summary>
        /// <param name="callback">The bool delegate to update the panel.</param>
        void Button_Selected(StartPageBoolInputSigDelegate callback);

        /// <summary>
        /// Button.Selected Feedback
        /// </summary>
        /// <param name="digital">The bool to update the panel.</param>
        void Button_Selected(bool digital);
    }

    /// <summary>
    /// Digital callback used in feedback events.
    /// </summary>
    /// <param name="boolInputSig">The <see cref="BoolInputSig"/> signal data.</param>
    /// <param name="startpage">The <see cref="IStartPage"/> on which to apply the feedback.</param>
    public delegate void StartPageBoolInputSigDelegate(BoolInputSig boolInputSig, IStartPage startpage);

    /// <summary>
    /// StartPage
    /// </summary>
    internal partial class StartPage : IStartPage, IDisposable
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
                /// Output or Event digital signal from panel to Control System: StartPage.Button.Press
                /// Button.Press
                /// </summary>
                public const uint Button_PressEvent = 3;


                /// <summary>
                /// Input or Feedback digital signal from Control System to panel: StartPageVisibilityJoin
                /// StartPage.VisibilityJoin
                /// </summary>
                public const uint StartPage_VisibilityJoinState = 1;

                /// <summary>
                /// Input or Feedback digital signal from Control System to panel: StartPage.Button.Selected
                /// Button.Selected
                /// </summary>
                public const uint Button_SelectedState = 4;

            }
        }

        #endregion

        #region Construction and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPage"/> component class.
        /// </summary>
        /// <param name="componentMediator">The <see cref="ComponentMediator"/> used to instantiate the component.</param>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        /// <param name="itemCount">The number of items.</param>
        internal StartPage(ComponentMediator componentMediator, uint controlJoinId, uint? itemCount)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId, itemCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPage"/> component class.
        /// </summary>
        /// <param name="componentMediator">The <see cref="ComponentMediator"/> used to instantiate the component.</param>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        internal StartPage(ComponentMediator componentMediator, uint controlJoinId) : this(componentMediator, controlJoinId, null)
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
        /// Initializes a new instance of the <see cref="StartPage"/> component class.
        /// </summary>
        /// <param name="controlJoinId">The SmartObjectId at which to create the component.</param>
        /// <param name="itemCount">The number of items.</param>
        private void Initialize(uint controlJoinId, uint? itemCount)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Button_PressEvent, onButton_Press);
        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        #endregion

        #region CH5 Contract

        /// <summary>
        /// Event Button.Press (from panel to Control System)
        /// </summary>
        public event EventHandler<UIEventArgs> Button_PressEvent;
        private void onButton_Press(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Button_PressEvent;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        /// <summary>
        /// Boolean feedback Button.Selected (from Control System to Panel)
        /// </summary>
        public void Button_Selected(StartPageBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Button_SelectedState], this);
            }
        }

        /// <summary>
        /// Boolean feedback Button.Selected (from Control System to Panel)
        /// </summary>
        public void Button_Selected(bool digital)
        {
            Button_Selected((sig, component) => sig.BoolValue = digital);
        }
        /// <summary>
        /// Boolean feedback StartPage.VisibilityJoin (from Control System to Panel)
        /// </summary>
        public void StartPage_VisibilityJoin(StartPageBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.StartPage_VisibilityJoinState], this);
            }
        }

        /// <summary>
        /// Boolean feedback StartPage.VisibilityJoin (from Control System to Panel)
        /// </summary>
        public void StartPage_VisibilityJoin(bool digital)
        {
            StartPage_VisibilityJoin((sig, component) => sig.BoolValue = digital);
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return (int)ControlJoinId;
        }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "StartPage", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            Button_PressEvent = null;
        }

        #endregion
    }
}
