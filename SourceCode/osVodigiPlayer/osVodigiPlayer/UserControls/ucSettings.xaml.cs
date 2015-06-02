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
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;
using System.Net;
using System.IO;

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
    public partial class ucSettings : UserControl
    {
        public static readonly RoutedEvent SettingsCompleteEvent = EventManager.RegisterRoutedEvent(
            "SettingsComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSettings));

        public event RoutedEventHandler SettingsComplete
        {
            add { AddHandler(SettingsCompleteEvent, value); }
            remove { RemoveHandler(SettingsCompleteEvent, value); }
        }

        public ucSettings()
        {
            InitializeComponent();
        }

        public void ResetControl()
        {
            try
            {
                txtAccountName.Text = PlayerConfiguration.configAccountName;
                txtAccountName.IsEnabled = false;
                txtPlayerName.Text = PlayerConfiguration.configPlayerName;
                txtPlayerName.IsEnabled = false;
                lblIDs.Text = "Player ID: " + PlayerConfiguration.configPlayerID.ToString() + "   Account ID: " + PlayerConfiguration.configAccountID.ToString();
                btnUnregister.IsEnabled = true;
                btnClose.IsEnabled = true;
            }
            catch { }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Collapsed;

                // Close this dialog and resume processing on main screen
                RaiseEvent(new RoutedEventArgs(SettingsCompleteEvent));
            }
            catch { }
        }

        private void btnUnregister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PlayerConfiguration.configPlayerID = 0;
                PlayerConfiguration.configPlayerName = "N/A";
                PlayerConfiguration.configAccountID = 0;
                PlayerConfiguration.configAccountName = "N/A";
                PlayerConfiguration.configIsPlayerInitialized = false;
                PlayerConfiguration.SavePlayerConfiguration();

                RaiseEvent(new RoutedEventArgs(SettingsCompleteEvent));
            }
            catch {  }
        }

        private void btnWebServerSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblWebServerValidate.Text = String.Empty;

                osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();

                if (!txtWebServiceURL.Text.ToLower().StartsWith("http://"))
                {
                    lblWebServerValidate.Text = "The Vodigi Web Service URL must begin with http://";
                    return;
                }

                try
                {
                    ws.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(txtWebServiceURL.Text.Trim()));
                    osVodigiWS.UserAccount useraccount = ws.User_Validate("defaultusername", "defaultpassword");
                }
                catch
                {
                    lblWebServerValidate.Text = "The Vodigi Web Service URL is not valid.";
                    return;
                }

                if (!txtMediaSourceURL.Text.ToLower().StartsWith("http://"))
                {
                    lblMediaSourceURL.Text = "The Media Source URL must begin with http://";
                    return;
                }

                if (!txtMediaSourceURL.Text.EndsWith("/"))
                    txtMediaSourceURL.Text = txtMediaSourceURL.Text + "/";

                try
                {
                    WebRequest request = WebRequest.Create(txtMediaSourceURL.Text + "DONOTDELETE.txt");
                    WebResponse response = request.GetResponse();
                    if (response == null) throw new Exception("Invalid");
                }
                catch
                {
                    lblWebServerValidate.Text = "The Media Source URL is not valid.";
                    return;
                }

                //Utility.SaveWebserviceURL(txtWebServiceURL.Text.Trim());
                //Utility.SaveAppSetting("MediaSourUrl", txtMediaSourceURL.Text.Trim());

                MessageBox.Show("The settings were saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

        private void btnServerReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetWebServerTab();
            }
            catch { }
        }

        private void btnWebDownloadsSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblDownloadsValidate.Text = String.Empty;

                if (!txtDownloadFolder.Text.EndsWith(@"\"))
                    txtDownloadFolder.Text += @"\";

                if (!txtDownloadFolder.Text.Contains(@":\"))
                {
                    lblDownloadsValidate.Text = "Folder is invalid. You must specify the drive letter.";
                    return;
                }

                try
                {
                    Directory.CreateDirectory(txtDownloadFolder.Text.Trim());
                }
                catch
                {
                    lblDownloadsValidate.Text = "Folder is invalid.";
                    return;
                }

                //Utility.SaveAppSetting("DownloadFolder", txtDownloadFolder.Text.Trim());

                MessageBox.Show("The settings were saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }

        }

        private void btnDownloadsReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetDownloadsTab();
            }
            catch { }
        }

        private void btnCursorSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save the cursor setting
                string setting = "true";
                if (!Convert.ToBoolean(chkShowCursor.IsChecked))
                    setting = "false";

                //Utility.SaveAppSetting("ShowCursor", setting);

                // Save the date/time weather bar setting
                setting = "true";
                if (!Convert.ToBoolean(chkShowDateTimeBar.IsChecked))
                    setting = "false";

                //Utility.SaveAppSetting("ShowDateTimeWeatherBar", setting);

                // Save the temperature setting
                setting = "true";
                if (!Convert.ToBoolean(chkShowFahrenheit.IsChecked))
                    setting = "false";

               // Utility.SaveAppSetting("ShowFahrenheit", setting);

                // Save the High Temp Label
               // Utility.SaveAppSetting("WeatherTextHigh", txtHighTempLabel.Text.Trim());
               // Utility.SaveAppSetting("WeatherTextLow", txtLowTempLabel.Text.Trim());

                // Save the image fill mode
                if (Convert.ToBoolean(chkAspectRatio.IsChecked))
                    setting = "UniformToFill";
                else
                    setting = "Fill";

                //Utility.SaveAppSetting("ImageFillMode", setting);

                MessageBox.Show("The settings were saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

        private void btnCursorReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetMiscTab();
            }
            catch { }
        }

        private void btnControlSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblControlsValidate.Text = String.Empty;

                decimal latitude = 0;
                decimal longitude = 0;

                try
                {
                    latitude = Convert.ToDecimal(txtLatitude.Text.Trim());
                    if (latitude < -90 || latitude > 90)
                        throw new Exception("Out of Range");
                }
                catch
                {
                    lblControlsValidate.Text = "Latitude is invalid (Range -90.0 to 90.0)";
                    return;
                }

                try
                {
                    longitude = Convert.ToDecimal(txtLongitude.Text.Trim());
                    if (longitude < -180 || longitude > 180)
                        throw new Exception("Out of Range");
                }
                catch
                {
                    lblControlsValidate.Text = "Longitude is invalid (Range -180.0 to 180.0)";
                    return;
                }

                if (String.IsNullOrEmpty(txtWebcamName.Text.Trim()))
                {
                    lblControlsValidate.Text = "Webcam Name is required.";
                    return;
                }

                //Utility.SaveAppSetting("WeatherLatitude", String.Format("{0:0.0000}", latitude));
                //Utility.SaveAppSetting("WeatherLongitude", String.Format("{0:0.0000}", longitude));
                //Utility.SaveAppSetting("WebcamName", txtWebcamName.Text.Trim());

                // Do this to show the formatted values
                ResetControlsTab();

                MessageBox.Show("The settings were saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

        private void btnControlReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetControlsTab();
            }
            catch { }
        }

        private void btnButtonsSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblButtonsValidate.Text = String.Empty;

                if (String.IsNullOrEmpty(txtOpenButtonText.Text.Trim()))
                {
                    lblButtonsValidate.Text = "You must enter text for the Open button.";
                    return;
                }
                if (String.IsNullOrEmpty(txtCloseButtonText.Text.Trim()))
                {
                    lblButtonsValidate.Text = "You must enter text for the Close button.";
                    return;
                }
                if (String.IsNullOrEmpty(txtBackButtonText.Text.Trim()))
                {
                    lblButtonsValidate.Text = "You must enter text for the Back button.";
                    return;
                }
                if (String.IsNullOrEmpty(txtNextButtonText.Text.Trim()))
                {
                    lblButtonsValidate.Text = "You must enter text for the Next button.";
                    return;
                }

                //Utility.SaveAppSetting("ButtonTextOpen", txtOpenButtonText.Text.Trim());
                //Utility.SaveAppSetting("ButtonTextClose", txtCloseButtonText.Text.Trim());
                //Utility.SaveAppSetting("ButtonTextNext", txtNextButtonText.Text.Trim());
                //Utility.SaveAppSetting("ButtonTextBack", txtBackButtonText.Text.Trim());

                MessageBox.Show("The settings were saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }

        }

        private void btnButtonsReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetButtonsTab();
            }
            catch { }
        }

        private void btnCleanupMedia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("This operation will delete all downloaded local media that is not part of the current schedule. Click OK to continue.",
                    "Continue?", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                    return;

                string imagesfolder = Helpers.PlayerSettings.GetPlayerSetting("DownloadFolder").PlayerSettingValue + "Images";
                string videosfolder = Helpers.PlayerSettings.GetPlayerSetting("DownloadFolder").PlayerSettingValue + "Videos"; 

                // Delete each image if it isn't part of the current schedule
                string[] images = Directory.GetFiles(imagesfolder);
                foreach (string image in images)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(image);
                        if (!ImageIsInCurrentSchedule(fi.Name))
                            File.Delete(image);
                    }
                    catch { }
                }

                // Delete each video if it isn't part of the current schedule
                string[] videos = Directory.GetFiles(videosfolder);
                foreach (string video in videos)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(video);
                        if (!VideoIsInCurrentSchedule(fi.Name))
                            File.Delete(video);
                    }
                    catch { }
                }

                MessageBox.Show("Operation complete.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

        private bool ImageIsInCurrentSchedule(string imagename)
        {
            try
            {
                foreach (Image image in CurrentSchedule.Images)
                {
                    if (image.StoredFilename == imagename)
                        return true;
                }
                return false;
            }
            catch { return true; }
        }

        private bool VideoIsInCurrentSchedule(string videoname)
        {
            try
            {
                foreach (Video video in CurrentSchedule.Videos)
                {
                    if (video.StoredFilename == videoname)
                        return true;
                }
                return false;
            }
            catch { return true; }
        }

        private void tabTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch (tabTabs.SelectedIndex)
                {
                    case 0: // Registration 
                        // Do Nothing - data in PlayerConfiguration.xml
                        break;
                    case 1: // Web Server
                        ResetWebServerTab();
                        break;
                    case 2: // Downloads
                        ResetDownloadsTab();
                        break;
                    case 3: // Controls
                        ResetControlsTab();
                        break;
                    case 4: // Buttons
                        ResetButtonsTab();
                        break;
                    default: // Misc
                        ResetMiscTab();
                        break;
                }
            }
            catch { }
        }

        private void ResetWebServerTab()
        {
            lblWebServerValidate.Text = String.Empty;
            txtWebServiceURL.Text = PlayerConfiguration.configVodigiWebserviceURL;
            //txtMediaSourceURL.Text = Utility.GetAppSetting("MediaSourUrl");
        }

        private void ResetDownloadsTab()
        {
            lblDownloadsValidate.Text = String.Empty;
            //txtDownloadFolder.Text = Utility.GetAppSetting("DownloadFolder");
        }

        private void ResetMiscTab()
        {
            string setting = String.Empty;//Utility.GetAppSetting("ShowCursor");
            if (setting.ToLower() == "true")
                chkShowCursor.IsChecked = true;
            else
                chkShowCursor.IsChecked = false;

           // setting = Utility.GetAppSetting("ShowDateTimeWeatherBar");
            if (setting.ToLower() == "true")
                chkShowDateTimeBar.IsChecked = true;
            else
                chkShowDateTimeBar.IsChecked = false;

            //setting = Utility.GetAppSetting("ShowFahrenheit");
            if (setting.ToLower() == "true")
                chkShowFahrenheit.IsChecked = true;
            else
                chkShowFahrenheit.IsChecked = false;

            //txtHighTempLabel.Text = Utility.GetAppSetting("WeatherTextHigh");
            //txtLowTempLabel.Text = Utility.GetAppSetting("WeatherTextLow");

            //setting = Utility.GetAppSetting("ImageFillMode");
            if (setting == "UniformToFill")
                chkAspectRatio.IsChecked = true;
            else
                chkAspectRatio.IsChecked = false;            
        }

        private void ResetControlsTab()
        {
            lblControlsValidate.Text = String.Empty;
            //txtLatitude.Text = Utility.GetAppSetting("WeatherLatitude");
            //txtLongitude.Text = Utility.GetAppSetting("WeatherLongitude");
            //txtWebcamName.Text = Utility.GetAppSetting("WebcamName");
        }

        private void ResetButtonsTab()
        {
            lblButtonsValidate.Text = String.Empty;
           // txtOpenButtonText.Text = Utility.GetAppSetting("ButtonTextOpen");
            //txtCloseButtonText.Text = Utility.GetAppSetting("ButtonTextClose");
            //txtNextButtonText.Text = Utility.GetAppSetting("ButtonTextNext");
            //txtBackButtonText.Text = Utility.GetAppSetting("ButtonTextBack");
        }

    }
}
