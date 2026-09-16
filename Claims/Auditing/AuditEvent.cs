namespace Claims.Auditing;

public enum AuditEntityType
{
    Claim,
    Cover
}

public record AuditEvent(AuditEntityType EntityType, string EntityId, string HttpRequestType, DateTime Created);
