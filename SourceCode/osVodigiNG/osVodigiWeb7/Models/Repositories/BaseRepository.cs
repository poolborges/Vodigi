using System;
using osVodigiWeb7x.Models;

namespace osVodigiWeb7.Models
{
    public abstract class BaseRepository
    {
        private readonly VodigiContext db;

        public BaseRepository(VodigiContext context)
        {
            db = context;
        }
    }
}
