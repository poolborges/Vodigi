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
    public partial class ucDownload : UserControl
    {
        DispatcherTimer timerdownload;
        private List<Download> downloads = new List<Download>();
        int currentfileindex = 0;
        int totalfilecount = 0;

        Storyboard sbFadeIn;
        Storyboard sbFadeOut;

        public static readonly RoutedEvent DownloadClosedEvent = EventManager.RegisterRoutedEvent(
            "DownloadClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucDownload));

        public event RoutedEventHandler DownloadClosed
        {
            add { AddHandler(DownloadClosedEvent, value); }
            remove { RemoveHandler(DownloadClosedEvent, value); }
        }

        public ucDownload()
        {
            try
            {
                InitializeComponent();

                sbFadeIn = (Storyboard)FindResource("sbFadeIn");
                sbFadeOut = (Storyboard)FindResource("sbFadeOut");
                sbFadeOut.Completed += sbFadeOut_Completed;

                timerdownload = new DispatcherTimer();
                timerdownload.Interval = TimeSpan.FromSeconds(1);
                timerdownload.Tick += timerdownload_Tick;
            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                GetDownloadsList();

                progressBar.Minimum = 0;
                progressBar.Maximum = downloads.Count - 1;
                progressBar.Value = 0;

                currentfileindex = 0;
                if (downloads != null && downloads.Count > 0)
                {
                    lblDownloadFile.Text = downloads[currentfileindex].FileType + ": " + downloads[currentfileindex].Name;
                    lblDownloadStatus.Text = "File " + (currentfileindex + 1).ToString() + " of " + totalfilecount.ToString();
                }
                else
                {
                    lblDownloadFile.Text = "Please wait. Downloading media.";
                    lblDownloadStatus.Text = String.Empty;
                }

                lblDownloadFile.Visibility = Visibility.Visible;
                progressBar.Visibility = Visibility.Visible;
                lblDownloadStatus.Visibility = Visibility.Visible;

                timerdownload.Start();
            }
            catch { }
        }

        void sbFadeOut_Completed(object sender, EventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Collapsed;
                RaiseEvent(new RoutedEventArgs(DownloadClosedEvent));
            }
            catch { }
        }

        void timerdownload_Tick(object sender, EventArgs e)
        {
            try
            {
                timerdownload.Stop();

                if (currentfileindex != totalfilecount)
                {
                    DownloadNextFile();
                    timerdownload.Start();
                }
                else
                {
                    lblDownloadFile.Text = "Downloads Complete";
                    lblDownloadStatus.Text = "File " + currentfileindex.ToString() + " of " + totalfilecount.ToString();

                    FadeOut();
                }
            }
            catch { }
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
                timerdownload.Stop();
                sbFadeOut.Begin();
            }
            catch { }
        }

        public void GetDownloadsList()
        {
            try
            {
                downloads = new List<Download>();

                foreach (Image image in CurrentSchedule.Images)
                {
                    Download download = new Download();
                    download.FileType = "Image";
                    download.StoredFilename = image.StoredFilename;
                    download.Name = image.ImageName;
                    downloads.Add(download);
                }

                foreach (Music music in CurrentSchedule.Musics)
                {
                    Download download = new Download();
                    download.FileType = "Music";
                    download.StoredFilename = music.StoredFilename;
                    download.Name = music.MusicName;
                    downloads.Add(download);
                }

                foreach (Video video in CurrentSchedule.Videos)
                {
                    Download download = new Download();
                    download.FileType = "Video";
                    download.StoredFilename = video.StoredFilename;
                    download.Name = video.VideoName;
                    downloads.Add(download);
                }

                totalfilecount = downloads.Count;

                progressBar.Minimum = 0;
                progressBar.Maximum = downloads.Count;
                progressBar.Value = 0;
            }
            catch { }
        }

        public void DownloadNextFile()
        {
            try
            {
                // Build the source and target urls
                // Save to C:\osVodigi\Images\GUID.ext or C:\osVodigi\Videos\GUID.ext by default
                string target = String.Empty;
                string source = String.Empty;

                if (downloads[currentfileindex].FileType.ToLower() == "video")
                {
                    source = DownloadManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Videos/" + downloads[currentfileindex].StoredFilename;
                    target = DownloadManager.DownloadFolder + @"Videos\" + downloads[currentfileindex].StoredFilename;
                }
                else if (downloads[currentfileindex].FileType.ToLower() == "music")
                {
                    source = DownloadManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Music/" + downloads[currentfileindex].StoredFilename;
                    target = DownloadManager.DownloadFolder + @"Music\" + downloads[currentfileindex].StoredFilename;
                }
                else
                {
                    source = DownloadManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Images/" + downloads[currentfileindex].StoredFilename;
                    target = DownloadManager.DownloadFolder + @"Images\" + downloads[currentfileindex].StoredFilename;
                }

                try
                {
                    lblDownloadFile.Text = downloads[currentfileindex + 1].FileType + ": " + downloads[currentfileindex + 1].Name;
                    lblDownloadStatus.Text = "File " + (currentfileindex + 2).ToString() + " of " + totalfilecount.ToString();
                    progressBar.Value = progressBar.Value + 1;
                    System.Windows.Forms.Application.DoEvents();
                }
                catch { }

                if (!File.Exists(target))
                {
                    DownloadManager.DownloadAndSaveFile(source, target);
                }

                currentfileindex = currentfileindex + 1;
            }
            catch { }
        }

    }
}
