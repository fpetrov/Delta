using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Delta.Core.Entities.Authentication;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    // TODO: Add type to this field.
    public List<string> Subscriptions { get; set; } = new();
    public List<string> LikedMovies { get; set; } = new();
    
    // Additional things.
    public long TelegramId { get; set; }
    public long Class { get; set; }  
    public Role Role { get; set; }
}