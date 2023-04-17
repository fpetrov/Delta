using Microsoft.EntityFrameworkCore;

namespace Delta.Application.Entities;

[Owned]
public class Notification
{
    public string Name { get; set; }
    public string Descriptions { get; set; }
    public DateTime Expires { get; set; }
}