using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Xml.Linq;

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
    class DownloadThread
    {
        private List<Download> downloads = new List<Download>();
        private List<Image> images = new List<Image>();
        private List<Music> musics = new List<Music>();
        private List<Video> videos = new List<Video>();
        private int downloadDelayInSecs = 5;
        private int downloadCheckDelayInSecs = 1800;

        public void DownloadThreadWorker()
        {
            try
            {
                // Wait 5 minutes before starting the download main work
                Thread.Sleep(300000);

                int infinite = 1;
                while (infinite != 0)
                {
                    try
                    {
                        GetFilesToDownload();

                        foreach (Download download in downloads)
                        {
                            // Save to C:\osVodigi\Images\GUID.ext or C:\osVodigi\Videos\GUID.ext
                            string target = String.Empty;
                            string source = String.Empty;
                            if (download.FileType.ToLower() == "video")
                            {
                                source = DownloadManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Videos/" + download.StoredFilename;
                                target = DownloadManager.DownloadFolder + @"Videos\" + download.StoredFilename;
                            }
                            else if (download.FileType.ToLower() == "music")
                            {
                                source = DownloadManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Music/" + download.StoredFilename;
                                target = DownloadManager.DownloadFolder + @"Music\" + download.StoredFilename;
                            }
                            else
                            {
                                source = DownloadManager.MediaSourceUrl + PlayerConfiguration.configAccountID.ToString() + "/Images/" + download.StoredFilename;
                                target = DownloadManager.DownloadFolder + @"Images\" + download.StoredFilename;
                            }
                            DownloadManager.DownloadAndSaveFile(source, target);
                            Thread.Sleep(downloadDelayInSecs * 1000);
                        }

                        Thread.Sleep(downloadCheckDelayInSecs * 1000);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void GetFilesToDownload()
        {
            try
            {
                osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();

                // Set the web service url
                ws.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(PlayerConfiguration.configVodigiWebserviceURL));
 
                string xml = ws.Player_GetMediaToDownload(PlayerConfiguration.configAccountID);
                downloads = new List<Download>();
                images = new List<Image>();
                videos = new List<Video>();
                musics = new List<Music>();
 
                XDocument xmldoc = XDocument.Parse(xml);
 
                images = (from Image in xmldoc.Descendants("Image") 
                          select new Image
                           {
                              ImageID = Convert.ToInt32(Image.Attribute("ImageID").Value),
                              StoredFilename = Convert.ToString(Image.Attribute("StoredFilename").Value),
                              ImageName = Utility.DecodeXMLString(Image.Attribute("ImageName").Value),
                          }
                ).ToList();
 
                videos = (from Video in xmldoc.Descendants("Video")
                          select new Video
                          {
                              VideoID = Convert.ToInt32(Video.Attribute("VideoID").Value),
                              StoredFilename = Convert.ToString(Video.Attribute("StoredFilename").Value),
                              VideoName = Utility.DecodeXMLString(Video.Attribute("VideoName").Value)
                          }
 
                ).ToList();
 
                musics = (from Music in xmldoc.Descendants("Music") 
                          select new Music
                          {
                              MusicID = Convert.ToInt32(Music.Attribute("MusicID").Value),
                              StoredFilename = Convert.ToString(Music.Attribute("StoredFilename").Value),
                              MusicName = Utility.DecodeXMLString(Music.Attribute("MusicName").Value),
                          }
 
                ).ToList();
 
                foreach (Image image in images)
                {
                    Download download = new Download();
                    download.FileType = "Image";
                    download.StoredFilename = image.StoredFilename;
                    downloads.Add(download);
                }
 
                foreach (Video video in videos)
                {
                    Download download = new Download();
                    download.FileType = "Video";
                    download.StoredFilename = video.StoredFilename;
                    downloads.Add(download);
                }
 
                foreach (Music music in musics)
                {
                    Download download = new Download();
                    download.FileType = "Music";
                    download.StoredFilename = music.StoredFilename;
                    downloads.Add(download);
                }
            }
            catch { } 
        }

    }
}
