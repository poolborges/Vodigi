using System;
using System.IO;
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

namespace osVodigiPlayer
{
    class DownloadManager
    {
        public static string DownloadFolder = @"C:\osVodigi\";
        public static string MediaSourceUrl = String.Empty;

        public static void CreateDownloadFolders()
        {
            try
            {
                if (!DownloadFolder.EndsWith(@"\"))
                {
                    DownloadFolder += @"\";
                }
                if (!Directory.Exists(DownloadFolder))
                {
                    Directory.CreateDirectory(DownloadFolder);
                }
                if (!Directory.Exists(DownloadFolder + @"Images"))
                {
                    Directory.CreateDirectory(DownloadFolder + @"Images");
                }
                if (!Directory.Exists(DownloadFolder + @"Music"))
                {
                    Directory.CreateDirectory(DownloadFolder + @"Music");
                }
                if (!Directory.Exists(DownloadFolder + @"Videos"))
                {
                    Directory.CreateDirectory(DownloadFolder + @"Videos");
                }
            }
            catch { }
        }

        public static bool DownloadAndSaveFile(string sourceURL, string destinationUri)
        {
            try
            {
                // Don't process if the destination exists
                if (File.Exists(destinationUri))
                    return true;

                // Download the file
                WebClient wcDownload = new WebClient();
                wcDownload.DownloadFile(sourceURL, destinationUri);

                return true;
            }
            catch { return false; }
        }

    }
}
