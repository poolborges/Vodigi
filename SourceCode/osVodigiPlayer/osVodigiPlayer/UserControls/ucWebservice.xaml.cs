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
using System.Windows.Threading;
using System.Xml.Linq;
using System.IO;
using System.Configuration;

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
    public partial class ucWebservice : UserControl
    {
        Storyboard sbFadeIn;
        Storyboard sbFadeOut;

        public static readonly RoutedEvent WebserviceClosedEvent = EventManager.RegisterRoutedEvent(
            "WebserviceClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucWebservice));

        public event RoutedEventHandler WebserviceClosed
        {
            add { AddHandler(WebserviceClosedEvent, value); }
            remove { RemoveHandler(WebserviceClosedEvent, value); }
        }

        public ucWebservice()
        {
            try
            {
                InitializeComponent();

                sbFadeIn = (Storyboard)FindResource("sbFadeIn");
                sbFadeOut = (Storyboard)FindResource("sbFadeOut");
                sbFadeOut.Completed += sbFadeOut_Completed;

                btnVerify.MouseLeftButtonUp += btnVerify_MouseLeftButtonUp;
                btnVerify.TouchUp += btnVerify_TouchUp;
            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                lblError.Text = String.Empty;
                txtVodigiWebserviceURL.Text = String.Empty;
            }
            catch { }
        }

        void sbFadeOut_Completed(object sender, EventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Collapsed;
                RaiseEvent(new RoutedEventArgs(WebserviceClosedEvent));
            }
            catch { }
        }

        void btnVerify_TouchUp(object sender, TouchEventArgs e)
        {
            VerifyClicked();
        }

        void btnVerify_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VerifyClicked();
        }

        private void VerifyClicked()
        {
            string errortext = "URL is invalid. Please try again.";
            try
            {
                // Try to get the database version at the url specified
                osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();
                ws.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(txtVodigiWebserviceURL.Text.Trim()));

                osVodigiWS.DatabaseVersion version = ws.DatabaseVersion_Get();
                if (version == null)
                {
                    lblError.Text = errortext;
                }
                else
                {
                    PlayerConfiguration.configVodigiWebserviceURL = txtVodigiWebserviceURL.Text.Trim();
                    FadeOut();
                }
            }
            catch { lblError.Text = errortext; }
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

    }
}
