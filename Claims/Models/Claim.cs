using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Models;

public class Claim
{
    [BsonId]
    public string? Id { get; set; }

    [BsonElement("coverId")]
    public string CoverId { get; set; } = default!;

    [BsonElement("created")]
    public DateTime Created { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = default!;

    [BsonElement("claimType")]
    public ClaimType Type { get; set; }

    [BsonElement("damageCost")]
    [Range(0, 100_000)]
    public decimal DamageCost { get; set; }
}

public enum ClaimType
{
    Collision = 0,
    Grounding = 1,
    BadWeather = 2,
    Fire = 3
}
