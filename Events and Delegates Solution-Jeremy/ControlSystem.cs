using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Masters_2025_MSS_621_JW.Devices;
using Masters_2025_MSS_621_JW.UserInterface;
using MSSXpanel;


namespace Masters_2025_MSS_621_JW
{
    public class ControlSystem : CrestronControlSystem
    {
        public static ControlSystem global;
        public EventTimers SetupTimers;

        public Xpanel xpanel;
        public AirMedia3100 MyAirMedia;
        public CrestronConnected MyCrestronConnected;
        public Nvx351 MyNvx;
        public Audio audio;

        List<string> GlobalNvxAddresses = new List<string>();

        private bool power = false;
        public bool SystemPower {
            get {
                return power;
            }
            set {
                if (value != power) {
                    power = value;
                    if (value) SystemOn();
                    else SystemOff();
                }
            }
        }

        public ControlSystem()
        {
            global = this;
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
                // I prefer to have all the hardware devices set up in one place, but YMMV
                xpanel = new Xpanel(0x04, this);
                MyNvx = new Nvx351(0x11, Nvx351.EMode.Rx, this);
                MyAirMedia = new AirMedia3100(0x22, this);
                MyCrestronConnected = new CrestronConnected(0x09, this);

                // software components
                audio = (Audio)new AudioTV(MyCrestronConnected, xpanel);
                SetupTimers = new EventTimers(); // timers such as automatic shutoff
                SetupTimers.scheduleDaily("22:00", SystemOff);

                // connect events
                xpanel.SourceSelect += SourceSelect;

                MyAirMedia.AddressChanged += xpanel.UpdateAirMediaAddress;
                MyAirMedia.PinCodeChanged += xpanel.UpdateAirMediaPin;

                // configure components
                xpanel.SetSources(new List<string> {
                    "Apple TV",
                    "AirMedia",
                    "Global Source 1",
                    "Global Source 2",
                    "Global Source 3",
                    "Global Source 4"
                });


                // Populate the NVX global addresses.   This would be handy to have populated from a file that was read on startup..
                // Maybe add a configuration class that does this?  NvxProducer?
                GlobalNvxAddresses.Add("192.168.8.2");
                GlobalNvxAddresses.Add("192.168.8.4");
                GlobalNvxAddresses.Add("192.168.8.6");
                GlobalNvxAddresses.Add("192.168.8.8");
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        private void SystemOn() {
            MyCrestronConnected.On();
            MyCrestronConnected.Input(1);
        }

        private void SystemOff() {
            MyCrestronConnected.Off();
            MyNvx.SetInput(Nvx351.ESource.Disable);
        }

        private void SourceSelect(int index) {
            MyCrestronConnected.Input(1);
            switch (index) {
                case 0:
                    MyNvx.SetInput(Nvx351.ESource.Hdmi1);
                    xpanel.SourceControls("MediaControl");
                    break;
                case 1:
                    MyNvx.SetInput(Nvx351.ESource.Hdmi2);
                    xpanel.SourceControls("AirMediaInfo");
                    break;
                default:
                    MyNvx.SetInput(Nvx351.ESource.Stream);
                    MyNvx.SetStreamLocation(GlobalNvxAddresses[index - 2]);
                    xpanel.SourceControls("NvxInfo");
                    break;
            }
        }

        public void log(string msg) {
            CrestronConsole.PrintLine(msg);
        }

    }
}