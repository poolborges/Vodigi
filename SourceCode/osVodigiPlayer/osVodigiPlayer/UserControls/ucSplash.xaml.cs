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
using System.Windows.Threading;
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
    public partial class ucSplash : UserControl
    {
        Storyboard sbFadeIn;
        Storyboard sbFadeOut;

        public static readonly RoutedEvent SplashClosedEvent = EventManager.RegisterRoutedEvent(
            "SplashClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSplash));

        public event RoutedEventHandler SplashClosed
        {
            add { AddHandler(SplashClosedEvent, value); }
            remove { RemoveHandler(SplashClosedEvent, value); }
        }

        public static readonly RoutedEvent SplashFadeInCompleteEvent = EventManager.RegisterRoutedEvent(
            "SplashFadeInComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSplash));

        public event RoutedEventHandler SplashFadeInComplete
        {
            add { AddHandler(SplashFadeInCompleteEvent, value); }
            remove { RemoveHandler(SplashFadeInCompleteEvent, value); }
        }

        public ucSplash()
        {
            try
            {
                InitializeComponent();

                sbFadeIn = (Storyboard)FindResource("sbFadeIn");
                sbFadeOut = (Storyboard)FindResource("sbFadeOut");
                sbFadeOut.Completed += sbFadeOut_Completed;
                sbFadeIn.Completed += sbFadeIn_Completed;
            }
            catch { }
        }

        void sbFadeIn_Completed(object sender, EventArgs e)
        {
            try
            {
                RaiseEvent(new RoutedEventArgs(SplashFadeInCompleteEvent));
            }
            catch { }
        }

        void sbFadeOut_Completed(object sender, EventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Collapsed;
                RaiseEvent(new RoutedEventArgs(SplashClosedEvent));
            }
            catch { }
        }

        public void FadeIn()
        {
            try
            {
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
