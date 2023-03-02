using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Delta.Core.Entities.Media;

public class Movie
{
    [Key]
    public int Id { get; set; }
    
    public string Title { get; set; }
    public string Genre { get; set; }
    public string Year { get; set; }
    public string Actors { get; set; }
    public string Director { get; set; }
    public string Plot { get; set; }
    [JsonPropertyName("Poster")]
    public string PosterAddress { get; set; }
    public string BoxOffice { get; set; }
    
    [JsonPropertyName("imdbRating")]
    public double Rating { get; set; }
}