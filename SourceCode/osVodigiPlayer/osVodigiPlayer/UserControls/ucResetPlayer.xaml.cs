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
    public partial class ucResetPlayer : UserControl
    {

        public static readonly RoutedEvent ResetPlayerEvent = EventManager.RegisterRoutedEvent(
            "ResetPlayer", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucResetPlayer));

        public event RoutedEventHandler ResetPlayer
        {
            add { AddHandler(ResetPlayerEvent, value); }
            remove { RemoveHandler(ResetPlayerEvent, value); }
        }

        public static readonly RoutedEvent ResetPlayerCancelEvent = EventManager.RegisterRoutedEvent(
            "ResetPlayerCancel", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucResetPlayer));

        public event RoutedEventHandler ResetPlayerCancel
        {
            add { AddHandler(ResetPlayerCancelEvent, value); }
            remove { RemoveHandler(ResetPlayerCancelEvent, value); }
        }

        public ucResetPlayer()
        {
            try
            {
                InitializeComponent();

                btnReset.MouseLeftButtonUp += btnReset_MouseLeftButtonUp;
                btnReset.TouchUp += btnReset_TouchUp;

                btnCancel.MouseLeftButtonUp += btnCancel_MouseLeftButtonUp;
                btnCancel.TouchUp += btnCancel_TouchUp;
            }
            catch { }
        }

        void btnCancel_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            try
            {
                CancelClicked();
            }
            catch { }
        }

        void btnCancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                CancelClicked();
            }
            catch { }
        }

        void btnReset_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            try
            {
                ResetClicked();
            }
            catch { }
        }

        void btnReset_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                ResetClicked();
            }
            catch { }
        }

        private void ResetClicked()
        {
            this.Visibility = Visibility.Collapsed;
            RaiseEvent(new RoutedEventArgs(ResetPlayerEvent));
        }

        private void CancelClicked()
        {
            this.Visibility = Visibility.Collapsed;
            RaiseEvent(new RoutedEventArgs(ResetPlayerCancelEvent));
        }
    }
}
