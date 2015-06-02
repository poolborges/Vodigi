using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

/* ----------------------------------------------------------------------------------------
    Vodigi - Open Source Interactive Digital Signage
    Copyright (C) 2005-2013  JMC Publications, LLC

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
---------------------------------------------------------------------------------------- */

namespace osVodigiPlayer.UserControls
{
    public partial class ucRegister : UserControl
    {
        Storyboard sbFadeIn;
        Storyboard sbFadeOut;

        public static readonly RoutedEvent RegisterClosedEvent = EventManager.RegisterRoutedEvent(
            "RegisterClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucRegister));

        public event RoutedEventHandler RegisterClosed
        {
            add { AddHandler(RegisterClosedEvent, value); }
            remove { RemoveHandler(RegisterClosedEvent, value); }
        }

        public ucRegister()
        {
            try
            {
                InitializeComponent();

                sbFadeIn = (Storyboard)FindResource("sbFadeIn");
                sbFadeOut = (Storyboard)FindResource("sbFadeOut");
                sbFadeOut.Completed += sbFadeOut_Completed;

                btnRegister.MouseLeftButtonUp += btnRegister_MouseLeftButtonUp;
                btnRegister.TouchUp += btnRegister_TouchUp;
            }
            catch { }
        }

        void sbFadeOut_Completed(object sender, EventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Collapsed;
                RaiseEvent(new RoutedEventArgs(RegisterClosedEvent));
            }
            catch { }
        }

        void btnRegister_TouchUp(object sender, TouchEventArgs e)
        {
            RegisterClicked();
        }

        void btnRegister_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RegisterClicked();
        }

        private void RegisterClicked()
        {
            try
            {
                lblError.Text = String.Empty;

                if (String.IsNullOrEmpty(txtAccountName.Text.Trim()) || String.IsNullOrEmpty(txtPlayerName.Text.Trim()))
                {
                    lblError.Text = "Please enter Account and Player Names.";
                    return;
                }

                osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();
                ws.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(PlayerConfiguration.configVodigiWebserviceURL));

                // Validate the account
                osVodigiWS.Account account = ws.Account_GetByName(txtAccountName.Text.Trim());
                if (account == null)
                {
                    lblError.Text = "Invalid Account Name. Please retry.";
                    return;
                }

                PlayerConfiguration.configAccountID = account.AccountID;
                PlayerConfiguration.configAccountName = account.AccountName;

                // Validate the player
                osVodigiWS.Player player = ws.Player_GetByName(account.AccountID, txtPlayerName.Text.Trim());
                if (player == null)
                {
                    lblError.Text = "Invalid Player Name. Please retry.";
                    return;
                }

                PlayerConfiguration.configPlayerID = player.PlayerID;
                PlayerConfiguration.configPlayerName = player.PlayerName;

                // Set the remaining properties on PlayerConfiguration and save the configuration
                PlayerConfiguration.configIsPlayerInitialized = true;
                PlayerConfiguration.SavePlayerConfiguration();

                // Since registration can cause accountid/playerid changes, delete the local schedule file
                ScheduleFile.DeleteScheduleFile();

                // Register the player at vodigi.com
                try
                {
                    VodigiWS.VodigiWSSoapClient vws = new VodigiWS.VodigiWSSoapClient();
                    vws.PlayerRegistered("PlayerRegistration");
                }
                catch { }

                FadeOut();
            }
            catch { lblError.Text = "Cannot connect to Vodigi Server. Please retry."; }
        }

        public void FadeIn()
        {
            try
            {
                ResetControl();
                gridMain.Opacity = 0;
                this.Visibility = Visibility.Visible;
                sbFadeIn.Begin();
            }
            catch { }
        }

        public void FadeOut()
        {
            try
            {
                sbFadeOut.Begin();
            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                txtAccountName.Text = String.Empty;
                txtPlayerName.Text = String.Empty;
                lblError.Text = String.Empty;
                lblWebserviceUrl.Text = PlayerConfiguration.configVodigiWebserviceURL;
            }
            catch { }
        }
    }
}
