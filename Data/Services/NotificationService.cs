using System;

namespace SecureJournal.Data.Services
{
    // Popup notification service for showing messages across the app
    public class NotificationService
    {
        public event Action<AppNotification>? OnNotify;

        public void ShowError(string title, string message) =>
            OnNotify?.Invoke(new AppNotification(title, message, NotificationType.Error));

        public void ShowSuccess(string title, string message) =>
            OnNotify?.Invoke(new AppNotification(title, message, NotificationType.Success));

        public void ShowInfo(string title, string message) =>
            OnNotify?.Invoke(new AppNotification(title, message, NotificationType.Info));
    }

    // Notification types used for styling and icons
    public enum NotificationType
    {
        Info,
        Success,
        Error
    }

    // Notification model used by NotificationHost
    public record AppNotification(string Title, string Message, NotificationType Type);
}
