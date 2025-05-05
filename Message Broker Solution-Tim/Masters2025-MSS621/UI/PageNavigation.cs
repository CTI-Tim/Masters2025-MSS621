using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSXpanel;
using MessageBroker;
using Crestron.SimplSharp;


namespace Masters2025_MSS621.UI
{
    /*
     *  This class should really only do page navigation. refrain from littering code in here to do other tasks or call other tasks
     * this class is for touchpanel navigation,  so leave it at that.   the nice part of the contracts being it's own class means
     * we can subscribe to the button events elsewhere and put the code for actually triggering things there leaving this clean for only
     * doing one job, which is the page navigation for the panel.
     *
     * Because the Power ON and Off button event handlers are in here.  I will call those messages to the message broker from here.
     * Refrain from attaching those events to the page flip logic.   let the automation call the actual page flips.
     */
    public class PageNavigation
    {
        private readonly Contract _myContract;
        private int _currentPage = 0;
        public PageNavigation(Contract myContract)
        {
            _myContract = myContract;

            Broker.AddDelegate("ShowStartPage", ShowSystemStartPage);
            Broker.AddDelegate("ShowMainPage", ShowSystemMainPage);

            _myContract.StartPage.Button_PressEvent += StartPage_Button_PressEvent;
            _myContract.HeaderBar.PowerButton_PressEvent += MainPage_PowerButton_PressEvent;
            
            // The below subpage/Widget was set to "global" in construct to get access like this.
            // Select the widget, then in layer manager window select the WidgetContainer.  there you will find global for ControlContract
            _myContract.PowerOffOk.PowerOffNoButton_PressEvent += PowerOffOkSubpage_PowerOffNoButton_PressEvent;
            _myContract.PowerOffOk.PowerOffYesButton_PressEvent += PowerOffOkSubpage_PowerOffYesButton_PressEvent;

        }

        public void ShowStartPage()
        {
            _myContract.StartPage.StartPage_VisibilityJoin((sig, m) => sig.Pulse(20));
            _currentPage = 0;
        }
        public void ShowMainPage()
        {
            _myContract.MainPage.MainPage_VisibilityJoin((sig, m) => sig.Pulse(20));
            _currentPage = 1;
        }


        public void ReloadCurrentPage()
        {
            switch (_currentPage)
            {
                case 0:
                    ShowStartPage();
                    break;
                default:
                    ShowMainPage();
                    break;
            }
        }

        public void ShowPowerOffSubpage()
        {
            // You still have to show the subpage on that page.
            // so you will have to trigger visibility on every page the subpage exists on.
            // Note: this was renamed from earlier versions of Construct.   Visibility_Fb() is the new naming.
            _myContract.MainPage.PowerOffOk.PowerOffOk_Visibility_fb(true);
        }
        public void HidePowerOffSubpage()
        {
            _myContract.MainPage.PowerOffOk.PowerOffOk_Visibility_fb(false);
        }


        // Wrapper methods to accept a message to fit the signature
        private void ShowSystemStartPage(Message m)
        {
            ShowStartPage();
        }
        private void ShowSystemMainPage(Message m)
        {
            ShowMainPage();
        }

        // Event handler methods
        private void StartPage_Button_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                ShowMainPage();
            }
        }
        private void MainPage_PowerButton_PressEvent(object sender, UIEventArgs e)
        {
            CrestronConsole.PrintLine("power pressed");
            if (e.SigArgs.Sig.BoolValue == true)
            {
               
                ShowPowerOffSubpage();
            }
        }
        private void PowerOffOkSubpage_PowerOffNoButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                HidePowerOffSubpage();
            }
        }
        private void PowerOffOkSubpage_PowerOffYesButton_PressEvent(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue == true)
            {
                HidePowerOffSubpage();
            }
        }


    }
}
