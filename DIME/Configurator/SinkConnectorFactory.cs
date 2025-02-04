using DIME.Connectors;

namespace DIME.Configurator;

public static class SinkConnectorFactory
{
    public static IConnector Create(string connectorType, Dictionary<object, object> section, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor)
    {
        IConnector connector = null;
        
        switch (connectorType)
        {
            case "httpserver":
                connector = HttpServer.Sink.Create(section, disruptor);
                break;
            case "influxlp":
                connector = InfluxLp.Sink.Create(section, disruptor);
                break;
            case "mqtt":
                connector = Mqtt.Sink.Create(section, disruptor);
                break;
            case "mtconnectshdr":
                connector = MtConnectShdr.Sink.Create(section, disruptor);
                break;
            case "mtconnectagent":
                connector = MtConnectAgent.Sink.Create(section, disruptor);
                break;
            case "redis":
                connector = Redis.Sink.Create(section, disruptor);
                break;
            case "sparkplugb":
                connector = SparkplugB.Sink.Create(section, disruptor);
                break;
            default:
                break;
        }
        
        return connector;
    }
}