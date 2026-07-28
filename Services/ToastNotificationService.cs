using System;

using Avalonia.Controls.Notifications;

namespace PriceTrail.Services;

public sealed class ToastNotificationService
{
    private static readonly TimeSpan DefaultNotificationDuration = TimeSpan.FromSeconds(5);

    private WindowNotificationManager? _manager;

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
