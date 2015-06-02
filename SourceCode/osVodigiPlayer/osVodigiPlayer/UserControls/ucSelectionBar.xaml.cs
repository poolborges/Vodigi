using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    public partial class ucSelectionBar : UserControl
    {
        public Color dsBackgroundColor { get; set; }
        public List<string> dsImageURLs { get; set; }
        public List<string> dsImageCaptions { get; set; }
        public int dsCurrentIndex { get; set; }
        public int dsSelectionIndex { get; set; }
        public int dsSelectionMouseIndex { get; set; }
        public string dsMouseOverControl { get; set; }
        public string dsButtonCloseText { get; set; }
        public string dsButtonNextText { get; set; }
        public string dsButtonBackText { get; set; }
        public double dsMainWindowWidth { get; set; }


        // Local variables
        DispatcherTimer timer;
        private List<ucSelection> selections = new List<ucSelection>();
        double xPosition = 0;
        int selectionindex = 0;
        int zorder = 300;

        // Storyboard variables
        Storyboard sbSelectionMoveLeft;
        Storyboard sbSelectionMoveRight;
        Storyboard sbFadeIn;
        Storyboard sbFadeOut;

        // Animation Variables
        DoubleAnimation daMoveLeft;
        DoubleAnimation daMoveRight;

        // Selection Bar Closed Event
        public static readonly RoutedEvent SelectionBarClosedEvent = EventManager.RegisterRoutedEvent(
            "SelectionBarClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSelectionBar));

        public event RoutedEventHandler SelectionBarClosed
        {
            add { AddHandler(SelectionBarClosedEvent, value); }
            remove { RemoveHandler(SelectionBarClosedEvent, value); }
        }

        // SelectionComplete Event
        public static readonly RoutedEvent SelectionBarCompleteEvent = EventManager.RegisterRoutedEvent(
            "SelectionBarComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSelectionBar));

        public event RoutedEventHandler SelectionBarComplete
        {
            add { AddHandler(SelectionBarCompleteEvent, value); }
            remove { RemoveHandler(SelectionBarCompleteEvent, value); }
        }

        // SelectionBarMouseEnter Event
        public static readonly RoutedEvent SelectionBarMouseEnterEvent = EventManager.RegisterRoutedEvent(
            "SelectionBarMouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSelectionBar));

        public event RoutedEventHandler SelectionBarMouseEnter
        {
            add { AddHandler(SelectionBarMouseEnterEvent, value); }
            remove { RemoveHandler(SelectionBarMouseEnterEvent, value); }
        }

        // SelectionBarMouseLeave Event
        public static readonly RoutedEvent SelectionBarMouseLeaveEvent = EventManager.RegisterRoutedEvent(
            "SelectionBarMouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSelectionBar));

        public event RoutedEventHandler SelectionBarMouseLeave
        {
            add { AddHandler(SelectionBarMouseLeaveEvent, value); }
            remove { RemoveHandler(SelectionBarMouseLeaveEvent, value); }
        }

        public ucSelectionBar()
        {
            try
            {
                InitializeComponent();

                sbSelectionMoveLeft = (Storyboard)FindResource("sbSelectionMoveLeft");
                sbSelectionMoveRight = (Storyboard)FindResource("sbSelectionMoveRight");
                sbFadeIn = (Storyboard)FindResource("sbFadeIn");
                sbFadeOut = (Storyboard)FindResource("sbFadeOut");

                sbFadeOut.Completed += new EventHandler(sbFadeOut_Completed);

                daMoveLeft = (DoubleAnimation)sbSelectionMoveLeft.Children[0];
                daMoveRight = (DoubleAnimation)sbSelectionMoveRight.Children[0];

                // Create the timer for the auto-close
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = TimeSpan.FromSeconds(60);
            }
            catch { }
        }

        void sbFadeOut_Completed(object sender, EventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                // Resizes and positions the control contents according to the specified properties

                selections = new List<ucSelection>();
                gridSelections.Children.Clear();
                dsSelectionIndex = 0;
                dsCurrentIndex = 0;
                xPosition = 0;
                selectionindex = 0;
                zorder = 300;

                MoveCenterReset();

                // Set the clipping region
                rectClip.Rect = new Rect(0, 0, this.Width, this.Height + 60);

                // Set the Background color - applied to gridMain
                gridMain.Background = new SolidColorBrush(dsBackgroundColor);
                gridMain.Width = this.Width;
                gridMain.Height = this.Height;

                // Reposition the buttons if width is less than 1280
                if (dsMainWindowWidth < 1280)
                {
                    gridNextButton.HorizontalAlignment = HorizontalAlignment.Center;
                    gridNextButton.Margin = new Thickness(525, 0, 0, -55);

                    gridBackButton.HorizontalAlignment = HorizontalAlignment.Center;
                    gridBackButton.Margin = new Thickness(240, 0, 0, -55);

                    gridCloseButton.HorizontalAlignment = HorizontalAlignment.Center;
                    gridCloseButton.Margin = new Thickness(-525, 0, 0, -55);
                }

                // Set the button label text
                txtBack.Text = dsButtonBackText;
                txtNext.Text = dsButtonNextText;
                txtClose.Text = dsButtonCloseText;

                // Create the selections
                double startingLeft = (this.Width / 2) - 150.0;
                int i = 0;
                foreach (string image in dsImageURLs)
                {
                    ucSelection selection = new ucSelection();
                    selection.Width = 300;
                    selection.Height = 210;
                    selection.VerticalAlignment = VerticalAlignment.Top;
                    selection.HorizontalAlignment = HorizontalAlignment.Left;
                    selection.dsImageURL = image;
                    selection.dsSelectionIndex = i;
                    selection.Margin = new Thickness((startingLeft + (i * 310)), 65, 0, 0);
                    selection.SelectionComplete += new RoutedEventHandler(selection_SelectionComplete);
                    selection.SelectionBack += new RoutedEventHandler(selection_SelectionBack);
                    selection.SelectionNext += new RoutedEventHandler(selection_SelectionNext);
                    selection.ResetControl();
                    selections.Add(selection);
                    gridSelections.Children.Add(selection);
                    i += 1;
                }

                if (selections.Count > 0)
                {
                    zorder += 1;
                    selections[selectionindex].SetValue(Canvas.ZIndexProperty, zorder);
                    selections[0].ZoomIn();
                    txtCaption.Text = dsImageCaptions[0];
                }

                timer.Start();
            }
            catch { }
        }

        void selection_SelectionNext(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectionindex < selections.Count - 1)
                {
                    selections[selectionindex].ZoomOut();
                    selectionindex += 1;
                    selections[selectionindex].ZoomIn();
                    zorder += 1;
                    selections[selectionindex].SetValue(Canvas.ZIndexProperty, zorder);
                    MoveLeft();
                    txtCaption.Text = dsImageCaptions[selectionindex];
                    dsCurrentIndex = selectionindex;
                }
                timer.Stop();
                timer.Start();
            }
            catch { }
        }

        void selection_SelectionBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectionindex > 0)
                {
                    selections[selectionindex].ZoomOut();
                    selectionindex -= 1;
                    selections[selectionindex].ZoomIn();
                    zorder += 1;
                    selections[selectionindex].SetValue(Canvas.ZIndexProperty, zorder);
                    MoveRight();
                    txtCaption.Text = dsImageCaptions[selectionindex];
                    dsCurrentIndex = selectionindex;
                }
                timer.Stop();
                timer.Start();
            }
            catch { }
        }

        public void MoveCenterReset()
        {
            try
            {
                daMoveRight.From = xPosition;
                daMoveRight.To = xPosition;
                sbSelectionMoveRight.Begin();
            }
            catch { }
        }

        void selection_SelectionComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                timer.Stop();
                ucSelection selection = (ucSelection)e.Source;
                this.dsSelectionIndex = selection.dsSelectionIndex;
                RaiseEvent(new RoutedEventArgs(SelectionBarCompleteEvent));
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

        public void MoveLeft()
        {
            try
            {
                daMoveLeft.From = xPosition;
                daMoveLeft.To = xPosition - 310.0;
                xPosition -= 310.0;
                sbSelectionMoveLeft.Begin();
            }
            catch { }
        }

        public void MoveRight()
        {
            try
            {
                daMoveRight.From = xPosition;
                daMoveRight.To = xPosition + 310.0;
                xPosition += 310.0;
                sbSelectionMoveRight.Begin();
            }
            catch { }
        }

        private void btnBack_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BackClick();
            }
            catch { }
        }

        public void BackClick()
        {
            try
            {
                if (selectionindex > 0)
                {
                    selections[selectionindex].ZoomOut();
                    selectionindex -= 1;
                    selections[selectionindex].ZoomIn();
                    zorder += 1;
                    selections[selectionindex].SetValue(Canvas.ZIndexProperty, zorder);
                    MoveRight();
                    txtCaption.Text = dsImageCaptions[selectionindex];
                    dsCurrentIndex = selectionindex;
                }
                timer.Stop();
                timer.Start();
            }
            catch { }
        }

        private void btnNext_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                NextClick();
            }
            catch { }
        }

        public void NextClick()
        {
            try
            {
                if (selectionindex < selections.Count - 1)
                {
                    selections[selectionindex].ZoomOut();
                    selectionindex += 1;
                    selections[selectionindex].ZoomIn();
                    zorder += 1;
                    selections[selectionindex].SetValue(Canvas.ZIndexProperty, zorder);
                    MoveLeft();
                    txtCaption.Text = dsImageCaptions[selectionindex];
                    dsCurrentIndex = selectionindex;
                }
                timer.Stop();
                timer.Start();
            }
            catch { }
        }

        private void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CloseClick();
            }
            catch { }
        }

        public void CloseClick()
        {
            try
            {
                timer.Stop();

                // Raise the event that the selection bar is being closed
                RaiseEvent(new RoutedEventArgs(SelectionBarClosedEvent));
            }
            catch { }
        }

        public void SelectionClick()
        {
            try
            {
                selections[dsSelectionMouseIndex].ClickThis();
            }
            catch { }
        }

        public void SelectionVoice()
        {
            try
            {
                selections[dsCurrentIndex].ClickThis();
            }
            catch { }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();

                // Raise the event that the selection bar is being closed
                RaiseEvent(new RoutedEventArgs(SelectionBarClosedEvent));
            }
            catch { }
        }

        public void StopTimer()
        {
            try
            {
                timer.Stop();
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
