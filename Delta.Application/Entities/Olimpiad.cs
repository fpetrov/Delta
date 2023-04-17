using System.ComponentModel.DataAnnotations;

namespace Delta.Application.Entities;

public class Olimpiad
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public DateTime Expires { get; set; }
    public OlimpiadType Type { get; set; }
}