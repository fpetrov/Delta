using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;
using Delta.Dnevnik.Models;

namespace Delta.Application.Services.User;

public interface IUserService
{
    public Task Create(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Все напоминания, которые скоро пройдут.
    /// </summary>
    IAsyncEnumerable<Notification> FindIncomingNotifications(
        Entities.User user,
        CancellationToken cancellationToken = default);

    public Task RemoveNotifications(
        Entities.User user,
        IAsyncEnumerable<Notification> notifications,
        CancellationToken cancellationToken = default);

    public Task<Schedule?> GetSchedule(
        GetScheduleRequest request,
        CancellationToken cancellationToken = default);
    
    public IAsyncEnumerable<Homework> GetHomework(
        GetHomeworkRequest request,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Entities.User> FindAllByType(
        OlimpiadType olimpiadType,
        CancellationToken cancellationToken = default);
    
    public IAsyncEnumerable<Entities.User> FindAll(
        CancellationToken cancellationToken = default);

    public Task AddNotification(
        AddNotificationRequest request,
        CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Notification> FindNotifications(
        long telegramId,
        CancellationToken cancellationToken = default);
}