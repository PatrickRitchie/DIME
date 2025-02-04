using DIME.Connectors;
using DIME.Configuration.Mqtt;

namespace DIME.Configurator.Mqtt;

public static class Sink
{
    public static IConnector Create(Dictionary<object, object> section, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor)
    {
        ConnectorConfiguration config = new();
        config.ConnectorType = section.ContainsKey("connector") ? Convert.ToString(section["connector"]) : "MQTT";
        config.Direction = Configuration.ConnectorDirectionEnum.Sink;
        config.Enabled = section.ContainsKey("enabled") ? Convert.ToBoolean(section["enabled"]) : true;
        config.ScanIntervalMs = section.ContainsKey("scan_interval") ? Convert.ToInt32(section["scan_interval"]) : 1000;
        config.Name = section.ContainsKey("name") ? Convert.ToString(section["name"]) : Guid.NewGuid().ToString();
        config.Address = section.ContainsKey("address") ? Convert.ToString(section["address"]) : "127.0.0.1";
        config.Port = section.ContainsKey("port") ? Convert.ToInt32(section["port"]) : 1883;
        config.Username = section.ContainsKey("username") ? Convert.ToString(section["username"]) : null;
        config.Password = section.ContainsKey("password") ? Convert.ToString(section["password"]) : null;
        config.BaseTopic = section.ContainsKey("base_topic") ? Convert.ToString(section["base_topic"]) : "ids";
        config.QoS = section.ContainsKey("qos") ? Convert.ToInt32(section["qos"]) : 0;
        config.RetainPublish = section.ContainsKey("retain") ? Convert.ToBoolean(section["retain"]) : true;

        var connector = new Connectors.Mqtt.Sink(config, disruptor);

        return connector;
    }
}