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
using System.Windows.Shapes;
using System.Configuration;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Media.Animation;

namespace osVodigiPlayer.Windows
{
    /// <summary>
    /// Lógica interna para AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        osVodigiPlayer.Helpers.VodigiWSClient ws = new osVodigiPlayer.Helpers.VodigiWSClient(new Uri("http://127.0.0.1"));

        public AdminWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
            this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
            this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
            this.MouseMove += new MouseEventHandler(MainWindow_MouseMove);

            ucSettings.SettingsComplete += new RoutedEventHandler(ucSettings_SettingsComplete);
            ucSchedule.ScheduleClosed += ucSchedule_ScheduleClosed;
            ucDownload.DownloadClosed += ucDownload_DownloadClosed;
            ucResetPlayer.ResetPlayer += ucResetPlayer_ResetPlayer;
            ucResetPlayer.ResetPlayerCancel += ucResetPlayer_ResetPlayerCancel;
        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeApplication();
            }
            catch { }
        }

        private void InitializeApplication()
        {
            try
            {
                //this.Title = ;
                MediaManager.CreateDownloadFolders();
                WindowStartupState();
                ResizeChildrenControls();

                PlayerConfiguration.LoadPlayerConfiguration();
                if (!PlayerConfiguration.configIsPlayerInitialized)
                {
                    PlayerConfiguration.SavePlayerConfiguration();
                }

                if (PlayerConfiguration.configIsPlayerInitialized)
                    ucSchedule.FadeIn();
                else
                    ucSettings.FadeIn();
            }
            catch { }
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void ucResetPlayer_ResetPlayerCancel(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowStartupState();
            }
            catch { }
        }

        void ucResetPlayer_ResetPlayer(object sender, RoutedEventArgs e)
        {
            try
            {
                PlayerConfiguration.configIsPlayerInitialized = false;
                PlayerConfiguration.SavePlayerConfiguration();
                InitializeApplication();
            }
            catch { }
        }


        void ucDownload_DownloadClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hide the cursor if applicable
                if (Helpers.PlayerSettings.GetPlayerSetting("ShowCursor").PlayerSettingValue.ToLower() == "true")
                    this.Cursor = Cursors.Arrow;
                else
                    this.Cursor = Cursors.None;

                WindowStartupState();
            }
            catch { }
        }

        void ucSchedule_ScheduleClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaManager.DownloadBaseFolder = Helpers.PlayerSettings.GetPlayerSetting("DownloadFolder").PlayerSettingValue;
                MediaManager.MediaSourceUrl = Helpers.PlayerSettings.GetPlayerSetting("MediaSourceUrl").PlayerSettingValue;

                ucDownload.FadeIn();
            }
            catch { }
        }

        void ucSettings_SettingsComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowStartupState();
            }
            catch { }
        }

        void ResizeChildrenControls()
        {
            try
            {
                // Size the children controls
                gridFullScreen.Width = this.Width - 20;
                gridFullScreen.Height = this.Height - 20;

                gridMain.Width = this.Width - 20;
                gridMain.Height = this.Height - 20;
            }
            catch { }
        }


        private void WindowStartupState()
        {
            try
            {
                ucSettings.Visibility = Visibility.Collapsed;
                ucSchedule.Visibility = Visibility.Collapsed;
                ucDownload.Visibility = Visibility.Collapsed;
                ucResetPlayer.Visibility = Visibility.Collapsed;
            }
            catch { }
        }



        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                // Resize the children controls
                ResizeChildrenControls();
            }
            catch { }
        }

        void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                // F12 = Exit
                if (e.Key == Key.F12 || e.SystemKey == Key.F12)
                {
                    MessageBoxResult mbr = MessageBox.Show("Do you want to close ?", "Close Administion Window", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbr == MessageBoxResult.Yes)
                    {
                        this.Close();
                        //Application.Current.Shutdown();
                    }

                }
                // F11 = Reset Player
                else if (e.Key == Key.F11 || e.SystemKey == Key.F11)
                {
                    ucResetPlayer.Visibility = Visibility.Visible;
                }
                // F10 - Schedule
                else if (e.Key == Key.F10 || e.SystemKey == Key.F10)
                {
                    ucResetPlayer.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // Position the mouse countdown grid based on the mouse position
                System.Windows.Point pt = e.GetPosition(this);
            }
            catch { }
        }

    }
}
