
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
    class Screen
    {
        public int ScreenID { get; set; }
        public int AccountID { get; set; }
        public string ScreenName { get; set; }
        public int SlideShowID { get; set; }
        public int PlayListID { get; set; }
        public int TimelineID { get; set; }
        public bool IsInteractive { get; set; }
        public int ButtonImageID { get; set; }
    }
}
