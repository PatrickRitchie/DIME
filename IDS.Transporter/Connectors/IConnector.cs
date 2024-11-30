using System.Collections.Concurrent;
using IDS.Transporter.Configuration;

namespace IDS.Transporter.Connectors;

public interface ISourceConnector: IConnector
{
    public ConcurrentBag<MessageBoxMessage> Inbox { get; }
    public ConcurrentBag<MessageBoxMessage> Samples { get; }
    public ConcurrentBag<MessageBoxMessage> Current { get; }
    public bool Read();
}

public interface ISinkConnector: IConnector
{
    public ConcurrentBag<MessageBoxMessage> Outbox { get; }
    public bool Write();
}

public interface IConnector
{
    public ConnectorRunner Runner { get; }
    public IConnectorConfiguration Configuration { get; }
    public Exception FaultReason { get; }
    public bool IsFaulted { get; }
    public bool IsConnected { get; }
    public bool Initialize(ConnectorRunner runner);
    public bool Create();
    public bool BeforeUpdate();
    public bool Connect();
    public bool AfterUpdate();
    public bool Disconnect();
}