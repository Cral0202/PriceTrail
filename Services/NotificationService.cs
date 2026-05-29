using System;

using Avalonia.Controls.Notifications;

namespace PriceTrail.Services;

public sealed class NotificationService
{
    public static NotificationService Instance { get; } = new();

    private static readonly TimeSpan DefaultNotificationDuration = TimeSpan.FromSeconds(5);

    private WindowNotificationManager? _manager;

    private NotificationService()
    {
    }

    public void Initialize(WindowNotificationManager manager)
    {
        _manager = manager;
    }

    public void ShowMessage(string title, string message, NotificationType type, TimeSpan? duration = null)
    {
        var expirationTime = duration ?? DefaultNotificationDuration;

        _manager?.Show(new Notification(title, message, type, expirationTime));
    }
}
