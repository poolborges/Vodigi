using System;
using osVodigiWeb7x.Models;

namespace osVodigiWeb7.Domain.Business
{
    public class PlayerManager
    {
        readonly IPlayerRepository playerRepository;
        readonly IPlayerGroupRepository playerGroupRepository;
        readonly IPlayerGroupScheduleRepository playerGroupScheduleRepository;

        public PlayerManager(IPlayerRepository _playerRepository,
            IPlayerGroupRepository _playerGroupRepository,
            IPlayerGroupScheduleRepository _playerGroupScheduleRepository)
        {
            playerRepository = _playerRepository;
            playerGroupRepository = _playerGroupRepository;
            playerGroupScheduleRepository = _playerGroupScheduleRepository;
        }

        public IPlayerRepository GetIPlayerRepository()
        {
            return playerRepository;
        }

        public IPlayerGroupRepository GetIPlayerGroupRepository()
        {
            return playerGroupRepository;
        }

        public IPlayerGroupScheduleRepository GetIPlayerGroupScheduleRepository()
        {
            return playerGroupScheduleRepository;
        }
    }
}
