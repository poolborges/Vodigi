using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
    public partial class ucPlayList : UserControl
    {
        // Complete Event
        public static readonly RoutedEvent PlayListCompleteEvent = EventManager.RegisterRoutedEvent(
            "PlayListComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucPlayList));

        public event RoutedEventHandler PlayListComplete
        {
            add { AddHandler(PlayListCompleteEvent, value); }
            remove { RemoveHandler(PlayListCompleteEvent, value); }
        }

        // Public properties
        public List<string> dsVideoURLs { get; set; }
        public bool dsFireCompleteEvent { get; set; }

        // Local Variables
        int iVideoIndex = -1; // Zero-based index

        public ucPlayList()
        {
            try
            {
                InitializeComponent();

                mediaPlayer.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(mediaPlayer_MediaFailed);
                mediaPlayer.MediaEnded += new RoutedEventHandler(mediaPlayer_MediaEnded);
                this.Unloaded += ucPlayList_Unloaded;
            }
            catch { }
        }

        public void Pause()
        {
            try
            {
                mediaPlayer.Pause();
            }
            catch { }
        }

        public void Resume()
        {
            try
            {
                mediaPlayer.Play();
            }
            catch { }
        }

        public void StopAll()
        {
            try
            {
                mediaPlayer.Stop();
                mediaPlayer.Source = null;
            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                LayoutRoot.Width = this.Width;
                LayoutRoot.Height = this.Height;

                gridMain.Width = this.Width;
                gridMain.Height = this.Height;

                iVideoIndex = -1;
                SetNextMedia();
            }
            catch { }
        }

        void ucPlayList_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaPlayer.Stop();
                mediaPlayer.Source = null;
            }
            catch { }
        }

        void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetNextMedia();
            }
            catch { }
        }

        void mediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
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
                if (iVideoIndex + 1 < dsVideoURLs.Count)
                    iVideoIndex = iVideoIndex + 1;
                else
                {
                    if (dsFireCompleteEvent)
                        RaiseEvent(new RoutedEventArgs(PlayListCompleteEvent));

                    iVideoIndex = 0;
                }

                mediaPlayer.Source = new Uri(dsVideoURLs[iVideoIndex]);
                mediaPlayer.Play();
            }
            catch { }
        }
    }
}
