using Delta.Dnevnik.Models;

namespace Delta.Dnevnik;

public interface IDnevnikConnection
{
    public Task<Schedule> GetScheduleAsync(DateTime dateTime);
    public Task<Student> GetStudentAsync();
}