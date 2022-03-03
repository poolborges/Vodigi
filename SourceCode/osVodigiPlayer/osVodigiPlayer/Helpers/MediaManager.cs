using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using osVodigiPlayer.Helpers;
using System.Threading.Tasks;

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
    class MediaManager
    {
        public static string DownloadBaseFolder = @"C:\osVodigi\";
        public static string MediaSourceUrl = String.Empty;
        private static List<string> downloadFolders = new List<string> { @"Images", @"Music", @"Videos", @"Media" };

        public static string VideosDirectory { get { return DownloadBaseFolder + @"Videos\"; } private set { } }
        public static string AudiosDirectory { get { return DownloadBaseFolder + @"Music\"; } private set { } }
        public static string ImagesDirectory { get { return DownloadBaseFolder + @"Images\"; } private set { } }

        public static string GetVideoPath(string fileName) { return VideosDirectory + fileName; }
        public static string GetAudioPath(string fileName) { return AudiosDirectory + fileName; }
        public static string GetImagePath(string fileName) { return ImagesDirectory + fileName; }

        public static void CreateDownloadFolders()
        {

            try
            {
                if (!DownloadBaseFolder.EndsWith(@"\"))
                {
                    DownloadBaseFolder += @"\";
                }

                downloadFolders.ForEach(delegate (string name)
                {
                    string fullName = DownloadBaseFolder + name;
                    Console.WriteLine(@"osVodigiPlayer.DownloadManager create Directory on: " + fullName);
                    createDirectory(fullName);
                });
            }
            catch { }
        }

        private static void createDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public static async Task<bool> DownloadAndSaveFileAsync(string sourceURL, string destinationUri)
        {
            bool res = false;
            try
            {
                // Don't process if the destination exists
                if (File.Exists(destinationUri))
                {
                    return true;
                }

                // Download the file
                WebClient wcDownload = new WebClient();
                await wcDownload.DownloadFileTaskAsync(sourceURL, destinationUri);
                res = true;
            }
            catch {  }
            return res;
        }

        public static bool DownloadAndSaveFile(Download download) { return DownloadAndSaveFileAsync(download).ConfigureAwait(false).GetAwaiter().GetResult(); }

        public static async Task<bool> DownloadAndSaveFileAsync(Download download)
        {
            // Save to C:\osVodigi\Images\GUID.ext or C:\osVodigi\Videos\GUID.ext
            bool resul = false;
            string source = GetMediaRemoteUrl(download);
            string target = GetMediaPath(download.StoredFilename, download.FileType);

            if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(target))
            {
                resul = await DownloadAndSaveFileAsync(source, target);
            }

            return resul;
        }
        
        public static string GetMediaRemoteUrl(Download download)
        {
            string fullUrl = String.Empty;
            if (download.FileType.ToLower() == "video")
            {
                fullUrl = MediaManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Videos/" + download.StoredFilename;
            }
            else if (download.FileType.ToLower() == "music")
            {
                fullUrl = MediaManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Music/" + download.StoredFilename;
            }
            else if (download.FileType.ToLower() == "image")
            {
                fullUrl = MediaManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Images/" + download.StoredFilename;
            }

            return fullUrl;
        }

        public static string GetMediaPath(string fileName, string fileType)
        {
            string filePath = String.Empty;
            if (fileType.ToLower() == "video")
            {
                filePath = MediaManager.GetVideoPath(fileName);
            }
            else if (fileType.ToLower() == "music")
            {
                filePath = MediaManager.GetAudioPath(fileName);
            }
            else if (fileType.ToLower() == "image")
            {
                filePath = MediaManager.GetImagePath(fileName);
            }

            return filePath;
        }

        public static async Task<List<Download>> GetFilesToDownloadAsync()
        {

            List<Download> downloads = new List<Download>();

            try
            {
                VodigiWSClient ws = new VodigiWSClient();
                string xml = await ws.GetMediaDescriptorAsync();

                XDocument xmldoc = XDocument.Parse(xml);

                var images = (from Image in xmldoc.Descendants("Image")
                              select new Download
                              {
                                  ID = Convert.ToInt32(Image.Attribute("ImageID").Value),
                                  StoredFilename = Convert.ToString(Image.Attribute("StoredFilename").Value),
                                  Name = XMLUtils.DecodeXMLString(Image.Attribute("ImageName").Value),
                                  FileType = "Image",
                              }).ToList();

                downloads.AddRange(images);

                var videos = (from Video in xmldoc.Descendants("Video")
                              select new Download
                              {
                                  ID = Convert.ToInt32(Video.Attribute("VideoID").Value),
                                  StoredFilename = Convert.ToString(Video.Attribute("StoredFilename").Value),
                                  Name = XMLUtils.DecodeXMLString(Video.Attribute("VideoName").Value),
                                  FileType = "Video"
                              }).ToList();

                downloads.AddRange(videos);

                var musics = (from Music in xmldoc.Descendants("Music")
                              select new Download
                              {
                                  ID = Convert.ToInt32(Music.Attribute("MusicID").Value),
                                  StoredFilename = Convert.ToString(Music.Attribute("StoredFilename").Value),
                                  Name = XMLUtils.DecodeXMLString(Music.Attribute("MusicName").Value),
                                  FileType = "Music",
                              }

                ).ToList();

                downloads.AddRange(musics);
            }
            catch { }

            return downloads;
        }

    }
}
