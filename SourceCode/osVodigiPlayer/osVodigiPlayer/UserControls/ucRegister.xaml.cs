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
using osVodigiPlayer.Data;

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

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegisterClicked();
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

                if (String.IsNullOrEmpty(txtVodigiWebserviceURL.Text.Trim()))
                {
                    lblError.Text = "Please enter Webservice URL.";
                    return;
                }

                if (String.IsNullOrEmpty(txtAccountName.Text.Trim()) || String.IsNullOrEmpty(txtPlayerName.Text.Trim()))
                {
                    lblError.Text = "Please enter Account and Player Names.";
                    return;
                }

                osVodigiPlayer.Helpers.VodigiWSClient ws = new osVodigiPlayer.Helpers.VodigiWSClient(new Uri(txtVodigiWebserviceURL.Text.Trim()));

                var version = ws.GetDatabaseVersionAsync().ConfigureAwait(false).GetAwaiter().GetResult(); ;
                if (version == null)
                {
                    lblError.Text = "invalid Webservice URL. Please try again.";
                }

                // Validate the account
                Account account = ws.GetAccountByNameAsync(txtAccountName.Text.Trim()).ConfigureAwait(false).GetAwaiter().GetResult(); ;
                if (account == null)
                {
                    lblError.Text = "Invalid Account Name. Please retry.";
                    return;
                }


                // Validate the player
                Player player = ws.GetPlayerByNameAsync(account.AccountID, txtPlayerName.Text.Trim()).ConfigureAwait(false).GetAwaiter().GetResult();
                if (player == null)
                {
                    lblError.Text = "Invalid Player Name. Please retry.";
                    return;
                }


                PlayerConfiguration.configVodigiWebserviceURL = txtVodigiWebserviceURL.Text.Trim();
                PlayerConfiguration.configAccountID = account.AccountID;
                PlayerConfiguration.configAccountName = account.AccountName;

                PlayerConfiguration.configPlayerID = player.PlayerID;
                PlayerConfiguration.configPlayerName = player.PlayerName;

                // Set the remaining properties on PlayerConfiguration and save the configuration
                PlayerConfiguration.configIsPlayerInitialized = true;
                PlayerConfiguration.SavePlayerConfiguration();

                // Since registration can cause accountid/playerid changes, delete the local schedule file
                ScheduleFile.DeleteScheduleFile();

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
                txtVodigiWebserviceURL.Text = PlayerConfiguration.configVodigiWebserviceURL;
            }
            catch { }
        }
    }
}
