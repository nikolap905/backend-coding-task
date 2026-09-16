using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Models;

public class Cover : IValidatableObject
{
    [BsonId]
    public string? Id { get; set; }

    [BsonElement("startDate")]
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime EndDate { get; set; }

    [BsonElement("claimType")]
    public CoverType Type { get; set; }

    [BsonElement("premium")]
    public decimal Premium { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.Date < DateTime.UtcNow.Date)
        {
            yield return new ValidationResult("StartDate cannot be in the past.", new[] { nameof(StartDate) });
        }

        if (EndDate.Date <= StartDate.Date)
        {
            yield return new ValidationResult("EndDate must be after StartDate.", new[] { nameof(EndDate) });
        }
        else if ((EndDate.Date - StartDate.Date).TotalDays > 365)
        {
            yield return new ValidationResult("Insurance period cannot exceed 1 year.", new[] { nameof(EndDate) });
        }
    }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}
