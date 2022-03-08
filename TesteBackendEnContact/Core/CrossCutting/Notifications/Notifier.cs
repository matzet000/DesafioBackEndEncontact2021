using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace TesteBackendEnContact.Core.CrossCutting.Notifications
{
    public class Notifier : INotifier
    {
        private readonly ILogger<Notifier> _logger;
        private List<string> _notifications;

        public Notifier(ILogger<Notifier> logger)
        {
            _logger = logger;
            _notifications = new List<string>();
        }

        public List<string> GetNotifications()
        {
            return _notifications;
        }

        public void Handle(string notification)
        {
            _notifications.Add(notification);
            _logger.LogError(notification);
        }

        public bool HasNotification()
        {
            return _notifications.Any();
        }
    }
}
