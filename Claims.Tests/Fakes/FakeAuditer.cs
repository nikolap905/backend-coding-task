using Claims.Application;

namespace Claims.Tests.Fakes;

public class FakeAuditer : IAuditer
{
    public List<(string Id, string HttpRequestType)> ClaimAudits { get; } = new();
    public List<(string Id, string HttpRequestType)> CoverAudits { get; } = new();

    public void AuditClaim(string id, string httpRequestType) => ClaimAudits.Add((id, httpRequestType));

    public void AuditCover(string id, string httpRequestType) => CoverAudits.Add((id, httpRequestType));
}
