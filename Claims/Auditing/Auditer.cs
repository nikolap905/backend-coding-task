using System.Threading.Channels;

namespace Claims.Auditing
{
    public class Auditer : IAuditer
    {
        private readonly ChannelWriter<AuditEvent> _writer;

        public Auditer(Channel<AuditEvent> channel)
        {
            _writer = channel.Writer;
        }

        public void AuditClaim(string id, string httpRequestType)
        {
            _writer.TryWrite(new AuditEvent(AuditEntityType.Claim, id, httpRequestType, DateTime.Now));
        }

        public void AuditCover(string id, string httpRequestType)
        {
            _writer.TryWrite(new AuditEvent(AuditEntityType.Cover, id, httpRequestType, DateTime.Now));
        }
    }
}
