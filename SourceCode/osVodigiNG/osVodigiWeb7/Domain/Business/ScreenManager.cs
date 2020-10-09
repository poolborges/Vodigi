using System;
using osVodigiWeb7x.Models;

namespace osVodigiWeb7.Domain.Business
{
    public class ScreenManager
    {
        readonly IScreenRepository screenRepository;

        public ScreenManager(IScreenRepository _screenRepository)
        {
            screenRepository = _screenRepository;
        }

        public IScreenRepository GetIScreenRepository()
        {
            return screenRepository;
        }
    }
}
