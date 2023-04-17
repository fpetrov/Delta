using Delta.Dnevnik.Models;

namespace Delta.Dnevnik;

public interface IDnevnikConnection
{
    public Task<Schedule> GetScheduleAsync(DateTime dateTime);
    public Task<Student?> GetStudentAsync();
}

public interface IDnevnikPlainConnection
{
    public Task<Schedule?> GetScheduleAsync(string token, long profileId, string dateTime);
    public Task<Student?> GetStudentAsync(string token);
}