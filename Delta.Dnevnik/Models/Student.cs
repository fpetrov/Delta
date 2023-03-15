using System.Text.Json.Serialization;

namespace Delta.Dnevnik.Models;

public class Student
{
    [JsonPropertyName("profile")] 
    public Profile Profile { get; set; }

    [JsonPropertyName("hash")] 
    public string Hash { get; set; }
}

public class Profile
{
    [JsonPropertyName("last_name")] 
    public string LastName { get; set; }

    [JsonPropertyName("first_name")] 
    public string FirstName { get; set; }

    [JsonPropertyName("middle_name")] 
    public string MiddleName { get; set; }

    [JsonPropertyName("birth_date")] 
    public DateTimeOffset? BirthDate { get; set; }

    [JsonPropertyName("sex")] 
    public string Sex { get; set; }

    [JsonPropertyName("user_id")]
    public long? UserId { get; set; }

    [JsonPropertyName("id")] 
    public long Id { get; set; }

    [JsonPropertyName("phone")] 
    public string Phone { get; set; }

    [JsonPropertyName("email")] 
    public string Email { get; set; }

    [JsonPropertyName("snils")] 
    public string Snils { get; set; }

    [JsonPropertyName("type")] 
    public string Type { get; set; }
}