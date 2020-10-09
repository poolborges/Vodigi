using System;
using osVodigiWeb7x.Models;

namespace osVodigiWeb7.Domain.Business
{
    public interface IAssetManager
    {

        public IImageRepository GetIImageRepository();
        IVideoRepository GetIVideoRepository();
        IMusicRepository GetIMusicRepository();
    }
}
