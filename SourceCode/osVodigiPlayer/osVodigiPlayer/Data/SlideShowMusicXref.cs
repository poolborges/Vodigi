using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osVodigiPlayer
{
    class SlideShowMusicXref
    {
        public int SlideShowMusicXrefID { get; set; }
        public int SlideShowID { get; set; }
        public int MusicID { get; set; }
        public string StoredFilename { get; set; }
        public int PlayOrder { get; set; }
    }
}
