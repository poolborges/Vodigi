using System;
using osVodigiWeb7x.Models;

namespace osVodigiWeb7.Domain.Business
{
    public class LogManager
    {
        IActivityLogRepository activityLogRepository;

        public LogManager(IActivityLogRepository _activityLogRepository)
        {
            activityLogRepository = _activityLogRepository;
        }

        public IActivityLogRepository GetIActivityLogRepository()
        {
            return activityLogRepository;
        }

    }
}
