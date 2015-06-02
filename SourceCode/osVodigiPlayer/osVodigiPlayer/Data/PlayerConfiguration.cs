using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

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

namespace osVodigiPlayer
{
    class PlayerConfiguration
    {
        public PlayerConfiguration()
        {
            configPlayerID = 0;
            configPlayerName = String.Empty;
            configAccountID = 0;
            configAccountName = String.Empty;
            configIsPlayerInitialized = false;
            configVodigiWebserviceURL = String.Empty;
        }

        public static int configPlayerID { get; set; }
        public static string configPlayerName { get; set; }
        public static int configAccountID { get; set; }
        public static string configAccountName { get; set; }
        public static bool configIsPlayerInitialized { get; set; }
        public static string configVodigiWebserviceURL { get; set; }

        public static void LoadPlayerConfiguration()
        {
            try
            {
                // Make sure the file exists
                if (File.Exists(GetConfigurationFilePath()))
                {
                    string xml = String.Empty;

                    // Read the registration file
                    using (StreamReader reader = new StreamReader(File.Open(GetConfigurationFilePath(), FileMode.Open, FileAccess.Read)))
                    {
                        xml = reader.ReadToEnd();
                    }

                    // Parse the XML

                    // PlayerID
                    try
                    {
                        XDocument xmldoc = XDocument.Parse(xml);
                        configPlayerID = (from PlayerID in xmldoc.Descendants("PlayerID")
                                          select new PlayerID
                                          {
                                              ID = Convert.ToInt32(PlayerID.Value),
                                          }
                        ).First().ID;
                    }
                    catch { configPlayerID = 0; }

                    // Player Name
                    try
                    {
                        XDocument xmldoc = XDocument.Parse(xml);
                        configPlayerName = (from PlayerName in xmldoc.Descendants("PlayerName")
                                            select new PlayerName
                                            {
                                                Name = Convert.ToString(PlayerName.Value),
                                            }
                        ).First().Name;
                    }
                    catch { configPlayerName = "N/A"; }

                    // AccountID
                    try
                    {
                        XDocument xmldoc = XDocument.Parse(xml);
                        configAccountID = (from AccountID in xmldoc.Descendants("AccountID")
                                           select new AccountID
                                          {
                                              ID = Convert.ToInt32(AccountID.Value),
                                          }
                        ).First().ID;
                    }
                    catch { configAccountID = 0; }

                    // Account Name
                    try
                    {
                        XDocument xmldoc = XDocument.Parse(xml);
                        configAccountName = (from AccountName in xmldoc.Descendants("AccountName")
                                             select new AccountName
                                            {
                                                Name = Convert.ToString(AccountName.Value),
                                            }
                        ).First().Name;
                    }
                    catch { configAccountName = "N/A"; }

                    // IsPlayerInitialized
                    try
                    {
                        XDocument xmldoc = XDocument.Parse(xml);
                        configIsPlayerInitialized = (from IsPlayerInitialized in xmldoc.Descendants("IsPlayerInitialized")
                                                     select new IsPlayerInitialized
                                                      {
                                                          PlayerInitialized = Convert.ToBoolean(IsPlayerInitialized.Value),
                                                      }
                        ).First().PlayerInitialized;
                    }
                    catch { configIsPlayerInitialized = false; }

                    // Vodigi Webservice URL
                    try
                    {
                        XDocument xmldoc = XDocument.Parse(xml);
                        configVodigiWebserviceURL = (from VodigiWebserviceURL in xmldoc.Descendants("VodigiWebserviceURL")
                                             select new VodigiWebserviceURL
                                             {
                                                 WebserviceURL = Convert.ToString(VodigiWebserviceURL.Value),
                                             }
                        ).First().WebserviceURL;
                    }
                    catch { configVodigiWebserviceURL = "http://free.vodigi.com/osVodigiService.asmx"; }
                }
                else
                {
                    SavePlayerConfiguration();
                }
            }
            catch { }
        }

        public static void SavePlayerConfiguration()
        {
            try
            {
                // Create the XML
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?><PlayerConfiguration>");
                sb.AppendLine("<PlayerID>" + configPlayerID.ToString() + "</PlayerID>");
                sb.AppendLine("<PlayerName>" + configPlayerName + "</PlayerName>");
                sb.AppendLine("<AccountID>" + configAccountID.ToString() + "</AccountID>");
                sb.AppendLine("<AccountName>" + configAccountName + "</AccountName>");
                if (configIsPlayerInitialized)
                    sb.AppendLine("<IsPlayerInitialized>true</IsPlayerInitialized>");
                else
                    sb.AppendLine("<IsPlayerInitialized>false</IsPlayerInitialized>");
                sb.AppendLine("<VodigiWebserviceURL>" + configVodigiWebserviceURL + "</VodigiWebserviceURL>");
                sb.AppendLine("</PlayerConfiguration>");

                // Delete the file if it exists
                if (File.Exists(GetConfigurationFilePath()))
                {
                    File.Delete(GetConfigurationFilePath());
                }

                // Save the file
                File.WriteAllText(GetConfigurationFilePath(), sb.ToString());

            }
            catch { }
        }

        public static string GetConfigurationFilePath()
        {
            try
            {
                string downloadfolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                if (!downloadfolder.EndsWith(@"\")) downloadfolder += @"\";
                string filepath = downloadfolder + "PlayerConfiguration.xml";
                if (filepath.StartsWith("file:\\")) filepath = filepath.Substring(6);
                return filepath;
            }
            catch { return String.Empty; }
        }
    }

    class PlayerID
    {
        public int ID;
    }

    class PlayerName
    {
        public string Name;
    }

    class AccountID
    {
        public int ID;
    }

    class AccountName
    {
        public string Name;
    }

    class IsPlayerInitialized
    {
        public bool PlayerInitialized;
    }

    class VodigiWebserviceURL
    {
        public string WebserviceURL;
    }

    class ConfigurationSetting
    {
        public string ConfigurationSettingName { get; set; }
        public string ConfigurationSettingTypeID { get; set; }
        public string ConfigurationSettingValue { get; set; }
    }

}
