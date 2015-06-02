using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Windows.Media.Imaging;
using System.Net;

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

namespace osVodigiPlayer.Helpers
{
    class Weather
    {
        public static List<WeatherDay> GetWeather(decimal dLatitude, decimal dLongitude, bool displayfahrenheit)
        {
            List<WeatherDay> weatherdays = new List<WeatherDay>();

            try
            {
                StringBuilder sXML = new StringBuilder();
                string sTempHighDay1 = "";
                string sTempHighDay2 = "";
                string sTempHighDay3 = "";
                string sTempLowDay1 = "";
                string sTempLowDay2 = "";
                string sTempLowDay3 = "";
                string sWeather1 = "";
                string sWeather2 = "";
                string sWeather3 = "";
                string sIconURL1 = "";
                string sIconURL2 = "";
                string sIconURL3 = "";

                // Get three days worth of weather - coded to read to 4 levels of node hierarchy

                string sCurrentNode = "";
                string noaarest = "http://graphical.weather.gov/xml/sample_products/browser_interface/ndfdBrowserClientByDay.php?lat=" + dLatitude.ToString() + "&lon=" + dLongitude.ToString() + "&format=24+hourly&numDays=3";

                WebClient webclient = new WebClient();
                string sWeatherXML = webclient.DownloadString(noaarest);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sWeatherXML);
                XPathNavigator nav1 = doc.CreateNavigator();
                nav1.MoveToRoot();
                nav1.MoveToFirstChild(); // dwml
                // Level 1 - find data node ---------------------------
                if (nav1.HasChildren)
                {
                    nav1.MoveToFirstChild();
                    do
                    {
                        if (nav1.NodeType == XPathNodeType.Element)
                        {
                            if (nav1.Name.ToLower() == "data")
                            {
                                // Level 2 - find the parameters node -----------------------------
                                if (nav1.HasChildren)
                                {
                                    XPathNavigator nav2 = nav1.Clone();
                                    nav2.MoveToFirstChild();
                                    do
                                    {
                                        if (nav2.NodeType == XPathNodeType.Element)
                                        {
                                            if (nav2.Name.ToLower() == "parameters")
                                            {
                                                // Level 3 - find the temperature, weather and conditions-icon nodes
                                                if (nav2.HasChildren)
                                                {
                                                    XPathNavigator nav3 = nav2.Clone();
                                                    nav3.MoveToFirstChild();
                                                    do
                                                    {
                                                        if (nav3.NodeType == XPathNodeType.Element)
                                                        {
                                                            if (nav3.Name.ToLower() == "temperature" || nav3.Name.ToLower() == "weather" || nav3.Name.ToLower() == "conditions-icon")
                                                            {
                                                                sCurrentNode = nav3.Name.ToLower();

                                                                // Level 4 - read the values
                                                                if (sCurrentNode == "temperature")
                                                                {
                                                                    if (nav3.HasAttributes)
                                                                    {
                                                                        XPathNavigator nava = nav3.Clone();
                                                                        nava.MoveToAttribute("type", "");
                                                                        if (nava.Value.ToLower() == "maximum")
                                                                        {
                                                                            XPathNavigator nav4a = nav3.Clone();
                                                                            nav4a.MoveToFirstChild();
                                                                            do
                                                                            {
                                                                                if (nav4a.NodeType == XPathNodeType.Element)
                                                                                {
                                                                                    if (nav4a.Name.ToLower() == "value")
                                                                                    {
                                                                                        if (String.IsNullOrEmpty(sTempHighDay1))
                                                                                            sTempHighDay1 = nav4a.Value;
                                                                                        else if (String.IsNullOrEmpty(sTempHighDay2))
                                                                                            sTempHighDay2 = nav4a.Value;
                                                                                        else if (String.IsNullOrEmpty(sTempHighDay3))
                                                                                            sTempHighDay3 = nav4a.Value;
                                                                                    }
                                                                                }
                                                                            } while (nav4a.MoveToNext());

                                                                        }
                                                                        else if (nava.Value.ToLower() == "minimum")
                                                                        {
                                                                            XPathNavigator nav4b = nav3.Clone();
                                                                            nav4b.MoveToFirstChild();
                                                                            do
                                                                            {
                                                                                if (nav4b.NodeType == XPathNodeType.Element)
                                                                                {
                                                                                    if (nav4b.Name.ToLower() == "value")
                                                                                    {
                                                                                        if (String.IsNullOrEmpty(sTempLowDay1))
                                                                                            sTempLowDay1 = nav4b.Value;
                                                                                        else if (String.IsNullOrEmpty(sTempLowDay2))
                                                                                            sTempLowDay2 = nav4b.Value;
                                                                                        else if (String.IsNullOrEmpty(sTempLowDay3))
                                                                                            sTempLowDay3 = nav4b.Value;
                                                                                    }
                                                                                }
                                                                            } while (nav4b.MoveToNext());
                                                                        }
                                                                    }
                                                                }
                                                                else if (sCurrentNode == "weather")
                                                                {
                                                                    XPathNavigator nav4c = nav3.Clone();
                                                                    nav4c.MoveToFirstChild();
                                                                    do
                                                                    {
                                                                        if (nav4c.NodeType == XPathNodeType.Element)
                                                                        {
                                                                            if (nav4c.Name.ToLower() == "weather-conditions")
                                                                            {
                                                                                XPathNavigator nav4ca = nav4c.Clone();
                                                                                nav4ca.MoveToAttribute("weather-summary", "");
                                                                                if (String.IsNullOrEmpty(sWeather1))
                                                                                    sWeather1 = nav4ca.Value;
                                                                                else if (String.IsNullOrEmpty(sWeather2))
                                                                                    sWeather2 = nav4ca.Value;
                                                                                else if (String.IsNullOrEmpty(sWeather3))
                                                                                    sWeather3 = nav4ca.Value;
                                                                            }
                                                                        }
                                                                    } while (nav4c.MoveToNext());
                                                                }
                                                                else if (sCurrentNode == "conditions-icon")
                                                                {
                                                                    XPathNavigator nav4d = nav3.Clone();
                                                                    nav4d.MoveToFirstChild();
                                                                    do
                                                                    {
                                                                        if (nav4d.NodeType == XPathNodeType.Element)
                                                                        {
                                                                            if (nav4d.Name.ToLower() == "icon-link")
                                                                            {
                                                                                if (String.IsNullOrEmpty(sIconURL1))
                                                                                    sIconURL1 = nav4d.Value;
                                                                                else if (String.IsNullOrEmpty(sIconURL2))
                                                                                    sIconURL2 = nav4d.Value;
                                                                                else if (String.IsNullOrEmpty(sIconURL3))
                                                                                    sIconURL3 = nav4d.Value;
                                                                            }
                                                                        }
                                                                    } while (nav4d.MoveToNext());
                                                                }
                                                            }
                                                        }
                                                    } while (nav3.MoveToNext());
                                                }
                                            }
                                        }
                                    } while (nav2.MoveToNext());
                                }
                            }
                        }
                    } while (nav1.MoveToNext());
                }

                WeatherDay weatherday1 = new WeatherDay();
                weatherday1.WeatherImage = new System.Windows.Controls.Image();
                weatherday1.WeatherImage.Source = new BitmapImage(new Uri(GetImagePath(sWeather1), UriKind.Relative));
                weatherday1.DayOfWeek = GetDay(Convert.ToInt32(DateTime.Today.DayOfWeek));
                weatherday1.Weather = sWeather1;
                weatherday1.High = sTempHighDay1;
                if (!displayfahrenheit)
                    weatherday1.High = ConvertFahrenheitToCelsius(weatherday1.High).ToString();
                weatherday1.Low = sTempLowDay1;
                if (!displayfahrenheit)
                    weatherday1.Low = ConvertFahrenheitToCelsius(weatherday1.Low).ToString();
                weatherdays.Add(weatherday1);

                WeatherDay weatherday2 = new WeatherDay();
                weatherday2.WeatherImage = new System.Windows.Controls.Image();
                weatherday2.WeatherImage.Source = new BitmapImage(new Uri(GetImagePath(sWeather2), UriKind.Relative));
                weatherday2.DayOfWeek = GetDay(Convert.ToInt32(DateTime.Today.DayOfWeek));
                weatherday2.Weather = sWeather2;
                weatherday2.High = sTempHighDay2;
                if (!displayfahrenheit)
                    weatherday2.High = ConvertFahrenheitToCelsius(weatherday2.High).ToString();
                weatherday2.Low = sTempLowDay2;
                if (!displayfahrenheit)
                    weatherday2.Low = ConvertFahrenheitToCelsius(weatherday2.Low).ToString();
                weatherdays.Add(weatherday2);

                WeatherDay weatherday3 = new WeatherDay();
                weatherday3.WeatherImage = new System.Windows.Controls.Image();
                weatherday3.WeatherImage.Source = new BitmapImage(new Uri(GetImagePath(sWeather3), UriKind.Relative));
                weatherday3.DayOfWeek = GetDay(Convert.ToInt32(DateTime.Today.DayOfWeek));
                weatherday3.Weather = sWeather3;
                weatherday3.High = sTempHighDay3;
                if (!displayfahrenheit)
                    weatherday3.High = ConvertFahrenheitToCelsius(weatherday3.High).ToString();
                weatherday3.Low = sTempLowDay3;
                if (!displayfahrenheit)
                    weatherday3.Low = ConvertFahrenheitToCelsius(weatherday3.Low).ToString();
                weatherdays.Add(weatherday3);
            }
            catch
            {
                WeatherDay weatherday1 = new WeatherDay();
                weatherday1.WeatherImage = new System.Windows.Controls.Image();
                weatherday1.WeatherImage.Source = new BitmapImage(new Uri("/Images/partlycloudy.png", UriKind.Relative));
                weatherday1.DayOfWeek = "N/A";
                weatherday1.Weather = "N/A";
                weatherday1.High = "N/A";
                weatherday1.Low = "N/A";

                WeatherDay weatherday2 = weatherday1;
                WeatherDay weatherday3 = weatherday1;

                weatherdays.Add(weatherday1);
                weatherdays.Add(weatherday2);
                weatherdays.Add(weatherday3);
            }

            return weatherdays;
        }

        private static string GetDay(int dayOfWeek)
        {
            if (dayOfWeek == 0)
                return "Sunday";
            else if (dayOfWeek == 1)
                return "Monday";
            else if (dayOfWeek == 2)
                return "Tuesday";
            else if (dayOfWeek == 3)
                return "Wednesday";
            else if (dayOfWeek == 4)
                return "Thursday";
            else if (dayOfWeek == 5)
                return "Friday";
            else
                return "Saturday";
        }

        private static string GetImagePath(string weatherDescription)
        {
            try
            {
                weatherDescription = weatherDescription.ToLower();

                string imagePath = String.Empty;

                if (weatherDescription == "sunny")
                    imagePath = "/Images/sunny.png";
                else if (weatherDescription == "mostly sunny")
                    imagePath = "/Images/sunny.png";
                else if (weatherDescription == "partly sunny")
                    imagePath = "/Images/partlycloudy.png";
                else if (weatherDescription == "mostly cloudy")
                    imagePath = "/Images/partlycloudy.png";
                else if (weatherDescription == "cloudy")
                    imagePath = "/Images/cloudy.png";
                else if (weatherDescription.Contains("clear"))
                    imagePath = "/Images/sunny.png";
                else if (weatherDescription == "partly cloudy")
                    imagePath = "/Images/partlycloudy.png";
                else if (weatherDescription.Contains("clouds"))
                    imagePath = "/Images/partlycloudy.png";
                else if (weatherDescription.Contains("cloudy"))
                    imagePath = "/Images/partlycloudy.png";
                else if (weatherDescription.Contains("clearing"))
                    imagePath = "/Images/sunny.png";
                else if (weatherDescription.Contains("sunny"))
                    imagePath = "/Images/sunny.png";
                else if (weatherDescription.Contains("fog"))
                    imagePath = "/Images/partlycloudy.png";
                else if (weatherDescription.Contains("snow"))
                    imagePath = "/Images/snow.png";
                else if (weatherDescription.Contains("flurries"))
                    imagePath = "/Images/snow.png";
                else if (weatherDescription.Contains("blizzard"))
                    imagePath = "/Images/snow.png";
                else if (weatherDescription.Contains("ice"))
                    imagePath = "/Images/wintermix.png";
                else if (weatherDescription.Contains("freezing"))
                    imagePath = "/Images/wintermix.png";
                else if (weatherDescription.Contains("sleet"))
                    imagePath = "/Images/wintermix.png";
                else if (weatherDescription.Contains("chance rain showers"))
                    imagePath = "/Images/rainsun.png";
                else if (weatherDescription.Contains("chance rain"))
                    imagePath = "/Images/rainsun.png";
                else if (weatherDescription.Contains("rain showers"))
                    imagePath = "/Images/rain.png";
                else if (weatherDescription.Contains("rain"))
                    imagePath = "/Images/rain.png";
                else if (weatherDescription.Contains("drizzle"))
                    imagePath = "/Images/rainsun.png";
                else if (weatherDescription.Contains("rain/snow"))
                    imagePath = "/Images/wintermix.png";
                else if (weatherDescription.Contains("freezing rain"))
                    imagePath = "/Images/wintermix.png";
                else if (weatherDescription.Contains("wintry mix"))
                    imagePath = "/Images/wintermix.png";
                else if (weatherDescription.Contains("thunderstorms"))
                    imagePath = "/Images/thunderstorm.png";
                else if (weatherDescription.Contains("tstms"))
                    imagePath = "/Images/thunderstorm.png";
                else
                    imagePath = "/Images/partlycloudy.png";

                return imagePath;
            }
            catch { return "/Images/partlycloudy.png"; }
        }

        public static int ConvertFahrenheitToCelsius(string tempfahrenheit)
        {
            double fahrenheit = Convert.ToDouble(tempfahrenheit);
            return Convert.ToInt32((fahrenheit - 32) * 5 / 9);
        }
    }
}
