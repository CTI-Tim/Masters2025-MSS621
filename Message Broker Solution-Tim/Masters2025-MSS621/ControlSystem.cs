using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using MessageBroker;
using Masters2025_MSS621.UI;

namespace Masters2025_MSS621
{
    public class ControlSystem : CrestronControlSystem
    {
        // Why make these global?  so they stick around.
        public DeviceSetup SetupHardware;
        public Automation SetupAutomation;
        //public EventTimers SetupTimers; // Old simplified timer class
        public SimpleEventTimers2 ClockTimers; 
        public Xpanel SetupXpanel;

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }


        public override void InitializeSystem()
        {
            try
            {
                // Lets instantiate our classes and get this started
                SetupHardware = new DeviceSetup(this);
                SetupAutomation = new Automation();
                SetupXpanel = new Xpanel(0x03, this);

                // Add in NVX source list.  
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.128");
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.129");
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.130");
                SetupAutomation.GlobalNvxAddresses.Add("192.168.8.131");

                // Set up timers
                //SetupTimers = new EventTimers();  // Old simplified class

                // This leverages a slightly more advanced class that allows us to easily add multiple timers if we wanted to
                ClockTimers = new SimpleEventTimers2();

                ClockTimers.AddEvent("22:00", "SystemOff");

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }
    }
}