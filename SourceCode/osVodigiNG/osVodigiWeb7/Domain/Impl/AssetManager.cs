using System;
using osVodigiWeb7x.Models;

namespace osVodigiWeb7.Domain.Business
{
    public class AssetManager : IAssetManager
    {
        readonly IImageRepository imageRepository;
        IMusicRepository musicRepository;
        IVideoRepository videoRepository;


        public AssetManager(IImageRepository _imageRepository,
            IMusicRepository _musicRepository,
            IVideoRepository _videoRepository)
        {
            imageRepository = _imageRepository;
            musicRepository = _musicRepository;
            videoRepository = _videoRepository;
        }

        public IImageRepository GetIImageRepository()
        {
            return imageRepository;
        }

        public IMusicRepository GetIMusicRepository()
        {
            return musicRepository;
        }

        public IVideoRepository GetIVideoRepository()
        {
            return videoRepository;
        }
    }
}
