using DIME.Connectors;
using DIME.Configuration.HaasShdr;

namespace DIME.Configurator.HaasShdr;

public static class Source
{
    public static IConnector Create(Dictionary<object, object> section, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor)
    {
        ConnectorConfiguration config = new();
        config.Direction = Configuration.ConnectorDirectionEnum.Source;
        config.Enabled = section.ContainsKey("enabled") ? Convert.ToBoolean(section["enabled"]) : true;
        config.ScanIntervalMs = section.ContainsKey("scan_interval") ? Convert.ToInt32(section["scan_interval"]) : 1000;
        config.ReportByException = section.ContainsKey("rbe") ? Convert.ToBoolean(section["rbe"]) : true;
        config.Name = section.ContainsKey("name") ? Convert.ToString(section["name"]) : Guid.NewGuid().ToString();
        config.InitScript = section.ContainsKey("init_script") ? Convert.ToString(section["init_script"]) : null;
        config.DeinitScript = section.ContainsKey("deinit_script") ? Convert.ToString(section["deinit_script"]) : null;
        config.LoopEnterScript = section.ContainsKey("enter_script") ? Convert.ToString(section["enter_script"]) : null;
        config.LoopExitScript = section.ContainsKey("exit_script") ? Convert.ToString(section["exit_script"]) : null;
        config.ItemizedRead = section.ContainsKey("itemized_read") ? Convert.ToBoolean(section["itemized_read"]) : false;
        // custom
        config.ConnectorType = section.ContainsKey("connector") ? Convert.ToString(section["connector"]) : "HaasSHDR";
        config.IpAddress = section.ContainsKey("address") ? Convert.ToString(section["address"]) : "0.0.0.0";
        config.Port = section.ContainsKey("port") ? Convert.ToInt32(section["port"]) : 9998;
        config.TimeoutMs = section.ContainsKey("timeout") ? Convert.ToInt32(section["timeout"]) : 1000;
        config.HeartbeatMs = section.ContainsKey("heartbeat_interval") ? Convert.ToInt32(section["heartbeat_interval"]) : 4000;
        config.RetryMs = section.ContainsKey("retry_interval") ? Convert.ToInt32(section["retry_interval"]) : 10000;
        config.Items = new List<ConnectorItem>();

        if (section.ContainsKey("items"))
        {
            var items = section["items"] as List<object>;
            if (items is not null)
            {
                foreach (var item in items)
                {
                    var itemDictionary = item as Dictionary<object, object>;
                    if (itemDictionary is not null)
                    {
                        config.Items.Add(new ConnectorItem()
                        {
                            Configuration = config,
                            Enabled = itemDictionary.ContainsKey("enabled") ? Convert.ToBoolean(itemDictionary["enabled"]) : true,
                            Name = itemDictionary.ContainsKey("name") ? Convert.ToString(itemDictionary["name"]) : Guid.NewGuid().ToString(),
                            ReportByException = itemDictionary.ContainsKey("rbe") ? Convert.ToBoolean(itemDictionary["rbe"]) : config.ReportByException,
                            Script = itemDictionary.ContainsKey("script") ? Convert.ToString(itemDictionary["script"]) : null,
                            Meta = itemDictionary.ContainsKey("meta") ? itemDictionary["meta"] as Dictionary<object, object> : null,
                            // custom
                            Address = itemDictionary.ContainsKey("address") ? Convert.ToString(itemDictionary["address"]) : null
                        });
                    }
                }
            }
        }

        var connector = new Connectors.HaasShdr.Source(config, disruptor);

        return connector;
    }
}