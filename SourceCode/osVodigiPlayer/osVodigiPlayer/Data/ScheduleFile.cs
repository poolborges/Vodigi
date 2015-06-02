using System;
using System.IO;
using System.Configuration;

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
    class ScheduleFile
    {
        public static string ReadScheduleFile()
        {
            try
            {
                // Make sure the file exists
                string sFilePath = GetScheduleFilePath();
                if (File.Exists(sFilePath))
                {
                    string xml = String.Empty;

                    // Read the schedule file
                    using (StreamReader reader = new StreamReader(File.Open(sFilePath, FileMode.Open, FileAccess.Read)))
                    {
                        xml = reader.ReadToEnd();
                    }

                    return xml;
                }
                else
                {
                    return String.Empty;
                }
            }
            catch { return String.Empty; }
        }

        public static void SaveScheduleFile(string xml)
        {
            try
            {
                DeleteScheduleFile();
                File.WriteAllText(GetScheduleFilePath(), xml);
            }
            catch { }
        }

        public static void DeleteScheduleFile()
        {
            try
            {
                if (File.Exists(GetScheduleFilePath()))
                {
                    File.Delete(GetScheduleFilePath());
                }
            }
            catch {  }            
        }

        public static string GetScheduleFilePath()
        {
            try
            {
                string downloadfolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                if (!downloadfolder.EndsWith(@"\")) downloadfolder += @"\";
                string filepath = downloadfolder + "ScheduleFile.xml";
                if (filepath.StartsWith("file:\\")) filepath = filepath.Substring(6);
                return filepath;
            }
            catch { return String.Empty; }
        }

    }

}
