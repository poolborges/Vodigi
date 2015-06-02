using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media.Animation;
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
    public partial class ucTimeline : UserControl
    {
        // Complete Event
        public static readonly RoutedEvent TimelineCompleteEvent = EventManager.RegisterRoutedEvent(
            "TimelineComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucTimeline));

        public event RoutedEventHandler TimelineComplete
        {
            add { AddHandler(TimelineCompleteEvent, value); }
            remove { RemoveHandler(TimelineCompleteEvent, value); }
        }

        // Public properties
        public Color dsBackgroundColor { get; set; }
        public int dsSlideDurationInSeconds { get; set; }
        public List<string> dsMediaURLs { get; set; }
        public List<string> dsMusicURLs { get; set; }
        public bool dsFireCompleteEvent { get; set; }
        public bool dsMuteMusicOnPlayback { get; set; }

        // Local variables
        DispatcherTimer timer;
        int mediaIndex = -1; // Zero-based index
        int imageToDisplay = 1; // 1 or 2 to indicate which Image control is currently visible
        int musicIndex = -1; // Zero-based index
        string currentMediaType = "Image"; // Image or Video

        // Storyboard variables
        Storyboard sbFadeOutImageOne;
        Storyboard sbFadeInImageOne;
        Storyboard sbFadeOutImageTwo;
        Storyboard sbFadeInImageTwo;
        Storyboard sbFadeOutVideo;
        Storyboard sbFadeInVideo;
        Storyboard sbVolumeUp;
        Storyboard sbVolumeDown;

        public ucTimeline()
        {
            try
            {
                InitializeComponent();

                sbFadeOutImageOne = (Storyboard)FindResource("sbFadeOutImageOne");
                sbFadeInImageOne = (Storyboard)FindResource("sbFadeInImageOne");
                sbFadeOutImageTwo = (Storyboard)FindResource("sbFadeOutImageTwo");
                sbFadeInImageTwo = (Storyboard)FindResource("sbFadeInImageTwo");
                sbFadeOutVideo = (Storyboard)FindResource("sbFadeOutVideo");
                sbFadeInVideo = (Storyboard)FindResource("sbFadeInVideo");
                sbVolumeUp = (Storyboard)FindResource("sbVolumeUp");
                sbVolumeDown = (Storyboard)FindResource("sbVolumeDown");
            }
            catch { }
        }

        public void Pause()
        {
            try
            {
                timer.Stop();
                if (currentMediaType == "Video")
                    vidVideo.Pause();
                mediaPlayerMusic.Pause();
            }
            catch { }
        }

        public void StopAll()
        {
            try
            {
                vidVideo.Stop();
                vidVideo.Source = null;
                mediaPlayerMusic.Stop();
                mediaPlayerMusic.Source = null;
                timer.Stop();
            }
            catch { }
        }

        public void Resume()
        {
            try
            {
                if (currentMediaType == "Image")
                    timer.Start();
                if (currentMediaType == "Video")
                    vidVideo.Play();
                if ((currentMediaType == "Video" && !dsMuteMusicOnPlayback) || currentMediaType == "Image")
                    mediaPlayerMusic.Volume = 1;
                else
                    mediaPlayerMusic.Volume = 0;
                if (mediaPlayerMusic.Source != null)
                    mediaPlayerMusic.Play();
            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                // Resizes and positions the control contents according to the specified properties

                // Set the clipping region
                rectClip.Rect = new Rect(0, 0, this.Width, this.Height);

                // Set the width/height of the media controls
                imgImageOne.Width = this.Width;
                imgImageOne.Height = this.Height;
                imgImageTwo.Width = this.Width;
                imgImageTwo.Height = this.Height;

                vidVideo.Width = this.Width;
                vidVideo.Height = this.Height;

                // Set the Background color - applied to gridMain
                gridMain.Background = new SolidColorBrush(dsBackgroundColor);

                // Validate the slide duration - no less than 5 seconds
                if (dsSlideDurationInSeconds < 5) dsSlideDurationInSeconds = 5;

                this.Unloaded += ucTimeline_Unloaded;

                vidVideo.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(vidVideo_MediaFailed);
                vidVideo.MediaEnded += new RoutedEventHandler(vidVideo_MediaEnded);

                mediaPlayerMusic.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(mediaPlayerMusic_MediaFailed);
                mediaPlayerMusic.MediaEnded += new RoutedEventHandler(mediaPlayerMusic_MediaEnded);

                mediaIndex = -1;
                musicIndex = -1;

                // Create the timer for the transition
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = TimeSpan.FromSeconds(dsSlideDurationInSeconds);

                SetNextMedia();
                SetNextMusic();
            }
            catch { }
        }

        void mediaPlayerMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetNextMusic();
            }
            catch { }
        }

        void mediaPlayerMusic_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            try
            {
                SetNextMusic();
            }
            catch { }
        }

        private void SetNextMusic()
        {
            try
            {
                if (dsMusicURLs.Count == 0)
                    return;

                if (musicIndex + 1 < dsMusicURLs.Count)
                {
                    musicIndex = musicIndex + 1;
                }
                else
                {
                    musicIndex = 0;
                }

                mediaPlayerMusic.Source = new Uri(dsMusicURLs[musicIndex]);
                mediaPlayerMusic.Play();
            }
            catch { }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            SetNextMedia();
        }

        void vidVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetNextMedia();
            }
            catch { }
        }

        void vidVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            try
            {
                SetNextMedia();
            }
            catch { }
        }

        private void SetNextMedia()
        {
            try
            {
                timer.Stop();

                if (mediaIndex + 1 < dsMediaURLs.Count)
                    mediaIndex = mediaIndex + 1;
                else
                {
                    if (dsFireCompleteEvent)
                        RaiseEvent(new RoutedEventArgs(TimelineCompleteEvent));

                    mediaIndex = 0;
                }

                string url = dsMediaURLs[mediaIndex];

                if (url.ToLower().EndsWith(".mp4") || url.ToLower().EndsWith(".wmv"))
                {
                    currentMediaType = "Video";

                    if (dsMuteMusicOnPlayback)
                        sbVolumeDown.Begin();

                    vidVideo.Opacity = 0;
                    vidVideo.Source = new Uri(url);

                    vidVideo.Play();
                    sbFadeInVideo.Begin();

                    if (imageToDisplay == 1)
                    {
                        // Means Image Two is showing
                        sbFadeOutImageTwo.Begin();
                    }
                    else
                    {
                        // Image One is showing
                        sbFadeOutImageOne.Begin();
                    }
                }
                else
                {
                    currentMediaType = "Image";

                    if (mediaPlayerMusic.Volume == 0)
                        sbVolumeUp.Begin();

                    if (vidVideo.Opacity > 0)
                        sbFadeOutVideo.Begin();

                    if (imgImageOne.Opacity > 0)
                        sbFadeOutImageOne.Begin();

                    if (imgImageTwo.Opacity > 0)
                        sbFadeOutImageTwo.Begin();

                    if (imageToDisplay == 1)
                    {
                        imgImageOne.Source = GetBitmap(url);
                        sbFadeInImageOne.Begin();
                        imageToDisplay = 2;
                    }
                    else
                    {
                        imgImageTwo.Source = GetBitmap(url);
                        sbFadeInImageTwo.Begin();
                        imageToDisplay = 1;
                    }

                    timer.Start();
                }

            }
            catch { }
        }
        
        void ucTimeline_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                vidVideo.Stop();
                vidVideo.Source = null;
                mediaPlayerMusic.Stop();
                mediaPlayerMusic.Source = null;
                timer.Stop();
            }
            catch { }
        }

        private BitmapImage GetBitmap(string sFile)
        {
            try
            {
                if (File.Exists(sFile))
                {
                    BitmapImage bmpimg = new BitmapImage();
                    bmpimg.BeginInit();
                    bmpimg.UriSource = new Uri(sFile, UriKind.Absolute);
                    bmpimg.EndInit();
                    return bmpimg;
                }
                else
                    return null;
            }
            catch { return null; }
        }


    }
}
