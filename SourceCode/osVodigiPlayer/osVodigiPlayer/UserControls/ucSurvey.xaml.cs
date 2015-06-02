using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Configuration;
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
    public partial class ucSurvey : UserControl
    {
        // Public properties
        public Survey dsSurvey { get; set; }
        public List<SurveyQuestion> dsSurveyQuestions { get; set; }
        public List<SurveyQuestionOption> dsSurveyQuestionOptions { get; set; }
        public string dsMouseOverControl { get; set; }

        // Local Variables
        int iQuestionIndex = 0;
        int iAnsweredSurveyID = 0;

        object surveyoption;

        DispatcherTimer timer;
        DispatcherTimer optiontimer;

        Storyboard sbFadeIn;
        Storyboard sbFadeOut;

        // Survey Closed Event
        public static readonly RoutedEvent SurveyClosedEvent = EventManager.RegisterRoutedEvent(
            "SurveyClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSurvey));

        public event RoutedEventHandler SurveyClosed
        {
            add { AddHandler(SurveyClosedEvent, value); }
            remove { RemoveHandler(SurveyClosedEvent, value); }
        }

        // SurveyButtonMouseEnter Event
        public static readonly RoutedEvent SurveyButtonMouseEnterEvent = EventManager.RegisterRoutedEvent(
            "SurveyButtonMouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSurvey));

        public event RoutedEventHandler SurveyButtonMouseEnter
        {
            add { AddHandler(SurveyButtonMouseEnterEvent, value); }
            remove { RemoveHandler(SurveyButtonMouseEnterEvent, value); }
        }

        // SurveyButtonMouseLeave Event
        public static readonly RoutedEvent SurveyButtonMouseLeaveEvent = EventManager.RegisterRoutedEvent(
            "SurveyButtonMouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucSurvey));

        public event RoutedEventHandler SurveyButtonMouseLeave
        {
            add { AddHandler(SurveyButtonMouseLeaveEvent, value); }
            remove { RemoveHandler(SurveyButtonMouseLeaveEvent, value); }
        }

        public ucSurvey()
        {
            try
            {
                InitializeComponent();

                gridMain.Opacity = 0;

                sbFadeIn = (Storyboard)FindResource("sbFadeIn");
                sbFadeOut = (Storyboard)FindResource("sbFadeOut");
                sbFadeOut.Completed += new EventHandler(sbFadeOut_Completed);

                // Create the timer for the auto-close
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = TimeSpan.FromSeconds(120);

                // Create the timer for the option toggle
                optiontimer = new DispatcherTimer();
                optiontimer.Tick += new EventHandler(optiontimer_Tick);
                optiontimer.Interval = TimeSpan.FromMilliseconds(1750);
            }
            catch { }
        }

        public void ResetControl()
        {
            try
            {
                iAnsweredSurveyID = 0;
                iQuestionIndex = 0;

                DisplayQuestion();
            }
            catch { }
        }

        private void DisplayQuestion()
        {
            try
            {
                // Reset the timer
                timer.Stop();
                timer.Start();

                gridNextButton.Visibility = Visibility.Visible;

                // Display the survey name and image
                if (iQuestionIndex == 0)
                {
                    txtSurveyName.Text = dsSurvey.SurveyName;
                    foreach (Image image in CurrentSchedule.Images)
                    {
                        if (image.ImageID == dsSurvey.SurveyImageID)
                        {
                            string imagepath = Helpers.PlayerSettings.GetPlayerSetting("DownloadFolder").PlayerSettingValue + @"Images\" + image.StoredFilename;
                            imgSurveyImage.Source = new BitmapImage(new Uri(imagepath, UriKind.Absolute));
                        }
                    }
                }

                // Display the question
                stackQuestion.Children.Clear();

                TextBlock questiontext = new TextBlock();
                questiontext.Margin = new Thickness(25, 25, 25, 25);
                questiontext.TextWrapping = TextWrapping.Wrap;
                questiontext.Text = dsSurveyQuestions[iQuestionIndex].SurveyQuestionText;
                questiontext.FontFamily = new FontFamily("Verdana");
                questiontext.FontSize = 18;
                questiontext.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                stackQuestion.Children.Add(questiontext);

                // Display the question options for the current question
                foreach (SurveyQuestionOption option in dsSurveyQuestionOptions)
                {
                    if (option.SurveyQuestionID == dsSurveyQuestions[iQuestionIndex].SurveyQuestionID)
                    {
                        TextBlock optiontext = new TextBlock();
                        optiontext.Margin = new Thickness(10, 0, 0, 0);
                        optiontext.TextWrapping = TextWrapping.Wrap;
                        optiontext.Text = option.SurveyQuestionOptionText;
                        optiontext.FontFamily = new FontFamily("Verdana");
                        optiontext.FontSize = 14;
                        optiontext.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

                        if (dsSurveyQuestions[iQuestionIndex].AllowMultiselect)
                        {
                            CheckBox checkbox = new CheckBox();
                            checkbox.Margin = new Thickness(50, 10, 25, 10);
                            checkbox.Content = optiontext;
                            checkbox.IsChecked = false;
                            checkbox.Tag = option.SurveyQuestionOptionID;
                            stackQuestion.Children.Add(checkbox);
                        }
                        else
                        {
                            RadioButton radiobutton = new RadioButton();
                            radiobutton.Margin = new Thickness(50, 10, 25, 10);
                            radiobutton.Content = optiontext;
                            radiobutton.IsChecked = false;
                            radiobutton.Tag = option.SurveyQuestionOptionID;
                            stackQuestion.Children.Add(radiobutton);
                        }
                    }
                }

                // Display the counter information
                txtCounter.Text = (iQuestionIndex + 1).ToString() + " : " + dsSurveyQuestions.Count.ToString();

                // Hide the Next button if end of survey
                if ((iQuestionIndex + 1) == dsSurveyQuestions.Count)
                {
                    gridNextButton.Visibility = Visibility.Hidden;
                }


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

        private void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CloseClick();
            }
            catch { }
        }

        private void SaveQuestionData()
        {
            try
            {
                osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();
                ws.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(PlayerConfiguration.configVodigiWebserviceURL));

                // Save the data
                if (iAnsweredSurveyID == 0)
                {
                    // Create the answered survey
                    try
                    {
                        iAnsweredSurveyID = ws.AnsweredSurvey_Create(PlayerConfiguration.configAccountID, dsSurvey.SurveyID, PlayerConfiguration.configPlayerID);
                    }
                    catch { return; }
                }

                if (stackQuestion.Children.Count > 0)
                {
                    // Skip the first item since it's the text label
                    for (int i = 1; i < stackQuestion.Children.Count; i += 1)
                    {
                        try
                        {
                            int iSurveyID = dsSurvey.SurveyID;
                            int iQuestionID = dsSurveyQuestions[iQuestionIndex].SurveyQuestionID;
                            int iSurveyQuestionOptionID = 0;
                            bool bIsSelected = false;
                            if (dsSurveyQuestions[iQuestionIndex].AllowMultiselect)
                            {
                                CheckBox checkbox = (CheckBox)stackQuestion.Children[i];
                                iSurveyQuestionOptionID = Convert.ToInt32(checkbox.Tag);
                                bIsSelected = Convert.ToBoolean(checkbox.IsChecked);
                            }
                            else
                            {
                                RadioButton radiobutton = (RadioButton)stackQuestion.Children[i];
                                iSurveyQuestionOptionID = Convert.ToInt32(radiobutton.Tag);
                                bIsSelected = Convert.ToBoolean(radiobutton.IsChecked);
                            }

                            // Update the answered survey question option
                            try
                            {
                                ws.AnsweredSurveyQuestionOption_CreateAsync(iAnsweredSurveyID, iSurveyQuestionOptionID, bIsSelected);
                            }
                            catch { }
                        }
                        catch { }

                    }
                }
            }
            catch { }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                FadeOut();
                SaveQuestionData();
            }
            catch { }
        }

        void optiontimer_Tick(object sender, EventArgs e)
        {
            try
            {
                optiontimer.Stop();
                try
                {
                    RadioButton radiobutton = (RadioButton)surveyoption;
                    radiobutton.IsChecked = !radiobutton.IsChecked;
                }
                catch { }

                try
                {
                    CheckBox checkbox = (CheckBox)surveyoption;
                    checkbox.IsChecked = !checkbox.IsChecked;
                }
                catch { }
            }
            catch { }
        }

        public void NextClick()
        {
            try
            {
                iQuestionIndex += 1;
                DisplayQuestion();
                SaveQuestionData();
            }
            catch { }
        }

        public void CloseClick()
        {
            try
            {
                timer.Stop();
                SaveQuestionData();
                FadeOut();
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

        void sbFadeOut_Completed(object sender, EventArgs e)
        {
            try
            {
                RaiseEvent(new RoutedEventArgs(SurveyClosedEvent));
                this.Visibility = Visibility.Collapsed;
            }
            catch { }
        }
    }
}
