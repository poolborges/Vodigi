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
using System.Threading;

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

        public static readonly RoutedEvent DownloadClosedEvent = EventManager.RegisterRoutedEvent(
            "DownloadClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucDownload));

        Storyboard sbFadeIn;
        Storyboard sbFadeOut;

        Thread dlthread;

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

            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                lblDownloadFile.Text = "Please wait. Downloading media.";
                lblDownloadFile.Visibility = Visibility.Visible;
                lblDownloadStatus.Text = String.Empty;
                lblDownloadStatus.Visibility = Visibility.Visible;

                progressBar.Minimum = 0;
                //progressBar.Maximum = downloads.Count;
                progressBar.Value = 0;
                progressBar.Visibility = Visibility.Visible;

                Progress<int> progress = new Progress<int>();
                progress.ProgressChanged += ReportProgressHandler;

                GetDownloadsList();
            }
            catch { }
        }

        private void ReportProgressHandler(object sender, int e)
        {
            progressBar.Value = e;
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

        public void FadeIn()
        {
            try
            {
                ResetControl();
                gridMain.Opacity = 0;
                this.Visibility = Visibility.Visible;
                sbFadeIn.Begin();

                // Start the download thread if not already running
                if (dlthread == null)
                {
                    DownloadThread downloadthread = new DownloadThread();
                    dlthread = new Thread(downloadthread.DownloadThreadWorker);
                    dlthread.Start();
                }
            }
            catch { }
        }

        public void FadeOut()
        {
            try
            {
                sbFadeOut.Begin();

                // Attempt to kill the download thread
                if (dlthread != null) dlthread.Abort();
            }
            catch { }
        }

        private List<Download> GetDownloadsList()
        {
            List<Download> downloads = new List<Download>();
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

            }
            catch { }

            return downloads;
        }

        private void DownloadNextFile(List<Download> downloads, IProgress<int> progress)
        {
            int currentfileindex = 1;
            int totalfilecount = downloads.Count();
            foreach (var down in downloads) 
            {
                Download download = downloads[currentfileindex];
                try
                {
                    lblDownloadFile.Text = download.FileType + ": " + download.Name;
                    lblDownloadStatus.Text = "File " + (currentfileindex).ToString() + " of " + totalfilecount.ToString();
                    progressBar.Value = progressBar.Value + 1;
                    //REMOVED System.Windows.Forms.Application.DoEvents();

                    MediaManager.DownloadAndSaveFile(download);
                }
                catch { }
                currentfileindex++;
            }
        }

    }
}
