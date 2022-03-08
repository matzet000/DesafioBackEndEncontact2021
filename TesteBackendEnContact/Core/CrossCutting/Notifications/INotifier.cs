using System.Collections.Generic;

namespace TesteBackendEnContact.Core.CrossCutting.Notifications
{
    public interface INotifier
    {
        bool HasNotification();
        List<string> GetNotifications();
        void Handle(string notification);
    }
}
