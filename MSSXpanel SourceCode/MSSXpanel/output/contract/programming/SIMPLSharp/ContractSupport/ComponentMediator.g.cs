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
using System.Globalization;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;

namespace MSSXpanel
{
    /// <summary>
    /// Crestron class to wire up signals to and from the panel.
    /// </summary>
    internal class ComponentMediator : IDisposable
    {
        #region Members

        private readonly IList<SmartObject> _smartObjects;
        private IList<SmartObject> SmartObjects { get { return _smartObjects; } }
        private Action<GenericBase, SmartObjectEventArgs> _activityEvent;

        private Dictionary<uint, Func<uint, uint, eSigType, Indexes>> _smartObjectGetIndexesLookup = new Dictionary<uint, Func<uint, uint, eSigType, Indexes>>();

        private readonly Dictionary<string, Action<IndexedEventArgs>> _booleanItemsOutputs;
        private Dictionary<string, Action<IndexedEventArgs>> BooleanItemOutputs { get { return _booleanItemsOutputs; } }

        private readonly Dictionary<string, Action<SmartObjectEventArgs>> _booleanOutputs;
        private Dictionary<string, Action<SmartObjectEventArgs>> BooleanOutputs { get { return _booleanOutputs; } }

        private readonly Dictionary<string, Action<IndexedEventArgs>> _numericItemsOutputs;
        private Dictionary<string, Action<IndexedEventArgs>> NumericItemOutputs { get { return _numericItemsOutputs; } }

        private readonly Dictionary<string, Action<SmartObjectEventArgs>> _numericOutputs;
        private Dictionary<string, Action<SmartObjectEventArgs>> NumericOutputs { get { return _numericOutputs; } }

        private readonly Dictionary<string, Action<IndexedEventArgs>> _stringItemsOutputs;
        private Dictionary<string, Action<IndexedEventArgs>> StringItemOutputs { get { return _stringItemsOutputs; } }

        private readonly Dictionary<string, Action<SmartObjectEventArgs>> _stringOutputs;
        private Dictionary<string, Action<SmartObjectEventArgs>> StringOutputs { get { return _stringOutputs; } }

        #endregion

        #region Construction & Initialization

        public ComponentMediator()
        {
            _smartObjects = new List<SmartObject>();

            _booleanOutputs = new Dictionary<string, Action<SmartObjectEventArgs>>();
            _numericOutputs = new Dictionary<string, Action<SmartObjectEventArgs>>();
            _stringOutputs = new Dictionary<string, Action<SmartObjectEventArgs>>();
            _booleanItemsOutputs = new Dictionary<string, Action<IndexedEventArgs>>();
            _numericItemsOutputs = new Dictionary<string, Action<IndexedEventArgs>>();
            _stringItemsOutputs = new Dictionary<string, Action<IndexedEventArgs>>();
        }

        public void HookSmartObjectEvents(SmartObject smartObject)
        {
            SmartObjects.Add(smartObject);
            smartObject.SigChange += SmartObject_SigChange;
        }
        public void UnHookSmartObjectEvents(SmartObject smartObject)
        {
            SmartObjects.Remove(smartObject);
            smartObject.SigChange -= SmartObject_SigChange;
        }

        #endregion

        #region Smart Object Event Handler

        private string GetKey(uint smartObjectId, uint join)
        {
            return smartObjectId.ToString(CultureInfo.InvariantCulture) + "." + join.ToString(CultureInfo.InvariantCulture);
        }

        internal void ConfigureBooleanItemEvent(uint controlJoinId, uint join, Func<uint, uint, eSigType, Indexes> getIndexes, Action<IndexedEventArgs> action)
        {
            string key = GetKey(controlJoinId, join);
            if (!string.IsNullOrEmpty(key))
            {
                BooleanItemOutputs[key] = action;
                //ErrorLog.Error("Adding key {0} to BooleanItemOutputs.", key);

                if (_smartObjectGetIndexesLookup.ContainsKey(controlJoinId))
                {
                    return;
                }

                _smartObjectGetIndexesLookup.Add(controlJoinId, getIndexes);
            }
        }
        
        internal void ConfigureNumericItemEvent(uint controlJoinId, uint join, Func<uint, uint, eSigType, Indexes> getIndexes, Action<IndexedEventArgs> action)
        {
            string key = GetKey(controlJoinId, join);
            if (!string.IsNullOrEmpty(key))
            {
                NumericItemOutputs[key] = action;
                //ErrorLog.Error("Adding key {0} to NumericItemOutputs.", key);
                if (_smartObjectGetIndexesLookup.ContainsKey(controlJoinId))
                {
                    return;
                }

                _smartObjectGetIndexesLookup.Add(controlJoinId, getIndexes);
            }
        }
        
        internal void ConfigureStringItemEvent(uint controlJoinId, uint join, Func<uint, uint, eSigType, Indexes> getIndexes, Action<IndexedEventArgs> action)
        {
            string key = GetKey(controlJoinId, join);
            if (!string.IsNullOrEmpty(key))
            {
                StringItemOutputs[key] = action;
                //ErrorLog.Error("Adding key {0} to StringItemOutputs.", key);
                if (_smartObjectGetIndexesLookup.ContainsKey(controlJoinId))
                {
                    return;
                }

                _smartObjectGetIndexesLookup.Add(controlJoinId, getIndexes);
            }
        }

        internal void ConfigureBooleanEvent(uint controlJoinId, uint join, Action<SmartObjectEventArgs> action)
        {
            string key = GetKey(controlJoinId, join);
            BooleanOutputs[key] = action;
            //ErrorLog.Error("Adding key {0} to BooleanOutputs.", key);
        }
        internal void ConfigureNumericEvent(uint controlJoinId, uint join, Action<SmartObjectEventArgs> action)
        {
            string key = GetKey(controlJoinId, join);
            NumericOutputs[key] = action;
            //ErrorLog.Error("Adding key {0} to NumericOutputs.", key);
        }
        internal void ConfigureStringEvent(uint controlJoinId, uint join, Action<SmartObjectEventArgs> action)
        {
            string key = GetKey(controlJoinId, join);
            StringOutputs[key] = action;
            //ErrorLog.Error("Adding key {0} to StringOutputs.", key);
        }

        internal void ConfigureActivityEvent(Action<GenericBase, SmartObjectEventArgs> onActivity)
        {
            _activityEvent = onActivity;
        }

        private void SmartObject_SigChange(GenericBase currentDevice, SmartObjectEventArgs args)
        {
            try
            {
                if (_activityEvent != null)
                {
                    _activityEvent.Invoke(currentDevice, args);
                }

                Func<uint, uint, eSigType, Indexes> getIndexes;
                while (_smartObjectGetIndexesLookup.TryGetValue(args.SmartObjectArgs.ID, out getIndexes))
                {
                    Dictionary<string, Action<IndexedEventArgs>> itemSignals = null;

                    //Resolve and invoke the corresponding method
                    Action<IndexedEventArgs> itemAction;

                    switch (args.Sig.Type)
                    {
                        case eSigType.Bool:
                            itemSignals = BooleanItemOutputs;
                            break;
                        case eSigType.UShort:
                            itemSignals = NumericItemOutputs;
                            break;
                        case eSigType.String:
                            itemSignals = StringItemOutputs;
                            break;
                    }

                    Indexes indexes = getIndexes.Invoke(args.SmartObjectArgs.ID, args.Sig.Number, args.Sig.Type);
                    if (indexes.IsError)
                        break;
                        string theKey = GetKey(args.SmartObjectArgs.ID - indexes.ItemIndex, args.Sig.Number - indexes.JoinIndex);

                    if (itemSignals != null &&
                        itemSignals.TryGetValue(theKey, out itemAction) &&
                        itemAction != null)
                    {
                        itemAction.Invoke(new IndexedEventArgs(args, indexes.ItemIndex, indexes.JoinIndex));
                    }
                    else
                    {
                        ErrorLog.Error("Unable to find action for {0} from {1} {2}", theKey, args.SmartObjectArgs.ID, args.Sig.Number);
                    }

                    break;
                }

                Dictionary<string, Action<SmartObjectEventArgs>> signals = null;

                //Resolve and invoke the corresponding method
                Action<SmartObjectEventArgs> action;

                switch (args.Sig.Type)
                {
                    case eSigType.Bool:
                        signals = BooleanOutputs;
                        break;
                    case eSigType.UShort:
                        signals = NumericOutputs;
                        break;
                    case eSigType.String:
                        signals = StringOutputs;
                        break;
                }

                string key = GetKey(args.SmartObjectArgs.ID, args.Sig.Number);

                if (signals != null && 
                    signals.TryGetValue(key, out action) &&
                    action != null)
                    action.Invoke(args);
                else
                    ErrorLog.Error("Unable to find action for {0}", key);

            }
            catch (Exception ex)
            {
                ErrorLog.Exception("Error while receiving feedback.", ex);
            }
        }
        #endregion

        #region IDisposable

        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            for (int i = 0; i < SmartObjects.Count; i++)
            {
                SmartObjects[i].SigChange -= SmartObject_SigChange;
            }
        }

        #endregion
    }
}


