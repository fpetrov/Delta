using AutoMapper;
using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;
using Delta.Application.Repositories.User;
using Delta.Dnevnik;
using Delta.Dnevnik.Models;

namespace Delta.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IDnevnikPlainConnection _dnevnikConnection;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper, IDnevnikPlainConnection dnevnikConnection)
    {
        _repository = repository;
        _mapper = mapper;
        _dnevnikConnection = dnevnikConnection;
    }


    public async Task Create(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = _mapper.Map<Entities.User>(request);

        var profile = await _dnevnikConnection.GetStudentAsync(request.DnevnikToken);

        user.ProfileId = profile.Profile.Id;
        
        await _repository.Create(user, cancellationToken);
    }

    public async Task<Schedule?> GetSchedule(GetScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindByTelegramId(request.TelegramId, cancellationToken);

        return await _dnevnikConnection.GetScheduleAsync(user.DnevnikToken, user.ProfileId, request.Date.ToString("yyyy-MM-dd"));
    }
    
    public async IAsyncEnumerable<Homework> GetHomework(GetHomeworkRequest request, CancellationToken cancellationToken = default)
    {
        var schedule = await GetSchedule(new GetScheduleRequest(request.TelegramId, request.Date), cancellationToken);

        foreach (var activity in schedule.Activities)
        {
            if (activity.Type == TypeEnum.Lesson)
                yield return new Homework(activity.Lesson.SubjectName, activity.Lesson.Homework);
        }
    }

    public async Task AddNotification(AddNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindByTelegramId(request.TelegramId, cancellationToken);
        
        user!.Notifications.Add(request.Notification);

        await _repository.Update(user!, cancellationToken);
    }

    public IAsyncEnumerable<Entities.User> FindAllByType(
        OlimpiadType olimpiadType,
        CancellationToken cancellationToken = default)
    {
        return _repository.FindAllByType(olimpiadType, cancellationToken);
    }
    
    public async IAsyncEnumerable<Notification> FindIncomingNotifications(Entities.User user,
        CancellationToken cancellationToken = default)
    {
        foreach (var notification in user.Notifications)
        {
            var subtraction = notification.Expires.Subtract(DateTime.Now);

            if (subtraction <= TimeSpan.FromDays(1))
            {
                yield return notification;
            }
        }
    }

    public async IAsyncEnumerable<Notification> FindNotifications(long telegramId,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindByTelegramId(telegramId, cancellationToken);

        foreach (var notification in user.Notifications)
        {
            yield return notification;
        }
    }
    
    public async Task RemoveNotifications(Entities.User user,
        IAsyncEnumerable<Notification> notifications,
        CancellationToken cancellationToken = default)
    {
        await foreach (var notification in notifications)
        {
            user.Notifications.Remove(notification);
        }

        await _repository.Update(user, cancellationToken);
    }
    
    public IAsyncEnumerable<Entities.User> FindAll(
        CancellationToken cancellationToken = default)
    {
        return _repository.FindAll(cancellationToken);
    }
}