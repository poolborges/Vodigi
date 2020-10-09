using System;
using Microsoft.AspNetCore.Http;

namespace osVodigiWeb7.Models.ViewModel
{
    public class ImageUploadVM
    {
        public string ImageName { get; set; }
        public string Tags { get; set; }
        public bool IsActive { get; set; }

        public IFormFile Fileupload { get; set; }

    }
}
