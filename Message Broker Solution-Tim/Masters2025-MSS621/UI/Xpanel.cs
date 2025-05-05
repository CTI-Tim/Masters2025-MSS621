using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using MessageBroker;

using MSSXpanel;  // Bring in the Construct Contract namespace


namespace Masters2025_MSS621.UI
{
    public class Xpanel
    {
        private XpanelForHtml5 myXpanel;
        private Contract myContract;
        private PageNavigation myPageNavigation;
        private MainPage myMainPage;
        private MediaSubPage myMediaSubPage;

        public Xpanel(uint IpId, ControlSystem cs)
        {
            myXpanel = new XpanelForHtml5(IpId, cs);
            myXpanel.OnlineStatusChange += MyXpanel_OnlineStatusChange;
            myContract = new Contract();
            myContract.AddDevice(myXpanel);
            myXpanel.Register();

            //Global Xpanel Important Press Events
            myContract.StartPage.Button_PressEvent += StartPage_Button_PressEvent;
            myContract.PowerOffOk.PowerOffYesButton_PressEvent += PowerOffOk_PowerOffYesButton_PressEvent;
            
            //Instantiate our helper classes passing them the contact object.
            myPageNavigation = new PageNavigation(myContract);
            myMainPage = new MainPage(myContract);
            myMediaSubPage = new MediaSubPage(myContract);


            //Set the main page title
            myContract.HeaderBar.RoomNameLabel_Indirect("Masters 2025 MSS-621");





        }

        private void PowerOffOk_PowerOffYesButton_PressEvent(object sender, UIEventArgs e)      // Power off was interacted with
        {
            if (e.SigArgs.Sig.BoolValue)
                Broker.SendMessage("SystemOff", new Message()); //trigger the system off automation
        }

        private void StartPage_Button_PressEvent(object sender, UIEventArgs e)                  // Touch to start was interacted with
        {
            if (e.SigArgs.Sig.BoolValue)
                Broker.SendMessage("SystemOn", new Message()); //trigger the system on automation
        }

        private void MyXpanel_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            myPageNavigation.ReloadCurrentPage();
        }
    }
}
