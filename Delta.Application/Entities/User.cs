using System.ComponentModel.DataAnnotations;

namespace Delta.Application.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public long TelegramId { get; set; }
    public long ProfileId { get; set; }
    public string DnevnikToken { get; set; }
    public OlimpiadType OlimpiadType { get; set; }
    public List<Notification> Notifications { get; set; } = new();
}

public enum OlimpiadType
{
    Math,
    Social,
    Chemistry
}