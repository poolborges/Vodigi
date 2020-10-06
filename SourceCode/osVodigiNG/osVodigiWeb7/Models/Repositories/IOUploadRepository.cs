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

using System;
using System.Collections.Generic;
using System.IO;

namespace osVodigiWeb6x.Models
{
    public class IOUploadRepository : IUploadRepository
    {
        public IEnumerable<Upload> GetImageUploads(int accountid, string serverpath)
        {
            List<Upload> uploads = new List<Upload>();

            if (!serverpath.EndsWith(@"\"))
                serverpath += @"\";
            serverpath += accountid.ToString() + @"\Images\";

            string[] imgs = Directory.GetFiles(serverpath);
            foreach (string img in imgs)
            {
                FileInfo fi = new FileInfo(img);

                Upload upload = new Upload();
                upload.FileName = fi.Name;
                upload.FileType = "Image";
                double filesize = Convert.ToDouble(fi.Length) / Convert.ToDouble(2097152);
                upload.FileSize = String.Format("{0:0.00}", filesize) + " MB";

                uploads.Add(upload);
            }

            return uploads;
        }

        public IEnumerable<Upload> GetVideoUploads(int accountid, string serverpath)
        {
            List<Upload> uploads = new List<Upload>();

            if (!serverpath.EndsWith(@"\"))
                serverpath += @"\";
            serverpath += accountid.ToString() + @"\Videos\";

            string[] vids = Directory.GetFiles(serverpath);
            foreach (string vid in vids)
            {
                FileInfo fi = new FileInfo(vid);

                Upload upload = new Upload();
                upload.FileName = fi.Name;
                upload.FileType = "Video";
                double filesize = Convert.ToDouble(fi.Length) / Convert.ToDouble(2097152);
                upload.FileSize = String.Format("{0:0.00}", filesize) + " MB";

                uploads.Add(upload);
            }

            return uploads;
        }

        public IEnumerable<Upload> GetMusicUploads(int accountid, string serverpath)
        {
            List<Upload> uploads = new List<Upload>();

            if (!serverpath.EndsWith(@"\"))
                serverpath += @"\";
            serverpath += accountid.ToString() + @"\Music\";

            string[] musics = Directory.GetFiles(serverpath);
            foreach (string music in musics)
            {
                FileInfo fi = new FileInfo(music);

                Upload upload = new Upload();
                upload.FileName = fi.Name;
                upload.FileType = "Music";
                double filesize = Convert.ToDouble(fi.Length) / Convert.ToDouble(2097152);
                upload.FileSize = String.Format("{0:0.00}", filesize) + " MB";

                uploads.Add(upload);
            }

            return uploads;
        }

        public bool DeleteImageUpload(int accountid, string filename, string serverpath)
        {
            try
            {
                if (!serverpath.EndsWith(@"\"))
                    serverpath += @"\";
                serverpath += accountid.ToString() + @"\Images\" + filename;

                File.Delete(serverpath);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteVideoUpload(int accountid, string filename, string serverpath)
        {
            try
            {
                if (!serverpath.EndsWith(@"\"))
                    serverpath += @"\";
                serverpath += accountid.ToString() + @"\Videos\" + filename;

                File.Delete(serverpath);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteMusicUpload(int accountid, string filename, string serverpath)
        {
            try
            {
                if (!serverpath.EndsWith(@"\"))
                    serverpath += @"\";
                serverpath += accountid.ToString() + @"\Music\" + filename;

                File.Delete(serverpath);

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}