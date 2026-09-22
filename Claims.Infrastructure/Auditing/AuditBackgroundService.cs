using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Claims.Infrastructure.Auditing;

public class AuditBackgroundService : BackgroundService
{
    private readonly ChannelReader<AuditEvent> _reader;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditBackgroundService> _logger;

    public AuditBackgroundService(Channel<AuditEvent> channel, IServiceScopeFactory scopeFactory, ILogger<AuditBackgroundService> logger)
    {
        _reader = channel.Reader;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var auditEvent in _reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();

                if (auditEvent.EntityType == AuditEntityType.Claim)
                {
                    auditContext.Add(new ClaimAudit
                    {
                        ClaimId = auditEvent.EntityId,
                        HttpRequestType = auditEvent.HttpRequestType,
                        Created = auditEvent.Created
                    });
                }
                else
                {
                    auditContext.Add(new CoverAudit
                    {
                        CoverId = auditEvent.EntityId,
                        HttpRequestType = auditEvent.HttpRequestType,
                        Created = auditEvent.Created
                    });
                }

                await auditContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write audit event for {EntityType} {EntityId}", auditEvent.EntityType, auditEvent.EntityId);
            }
        }
    }
}
