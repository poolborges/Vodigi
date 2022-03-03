using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Xml.Linq;
using osVodigiPlayer.Helpers;

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
        private int downloadDelayInMillis = 5 * 1000; // 5 SECOND
        private int downloadCheckDelayInMillis = 1800 * 1000; // 30 MINU

        public void DownloadThreadWorker()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        List<Download> downloads = MediaManager.GetFilesToDownloadAsync().ConfigureAwait(false).GetAwaiter().GetResult(); ;

                        foreach (Download download in downloads)
                        {
                            if (MediaManager.DownloadAndSaveFile(download))
                            {
                                Thread.Sleep(downloadDelayInMillis);
                            } 
                        }
                        Thread.Sleep(downloadCheckDelayInMillis);

                    }
                    catch { }
                }
            }
            catch { }
        }

    }
}
