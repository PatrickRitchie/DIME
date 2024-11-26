using IDS.Transporter.Connectors;
using Newtonsoft.Json;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace IDS.Transporter.Configurator;

public partial class Configurator
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    public static Dictionary<object,object> Read(string[] configurationFilenames)
    {
        Logger.Debug("[Configurator.Read] Reading files {0}", configurationFilenames);
        var yaml = "";
        foreach (var configFile in configurationFilenames) yaml += File.ReadAllText(configFile);
        Logger.Trace("[Configurator.Read] YAML \r\n{0}", yaml);
        var stringReader = new StringReader(yaml);
        var parser = new Parser(stringReader);
        var mergingParser = new MergingParser(parser);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        Dictionary<object, object> dictionary = new();
        try
        {
            dictionary = deserializer.Deserialize<Dictionary<object, object>>(mergingParser);
        }
        catch (SemanticErrorException e)
        {
            Logger.Error(e, "[Configurator.Read] Error while parsing yaml.");
        }
        
        Logger.Trace("[Configurator.Read] Dictionary \r\n{0}", JsonConvert.SerializeObject(dictionary));
        return dictionary;
    }

    public static List<IConnector> CreateConnectors(Dictionary<object, object> configuration, Disruptor.Dsl.Disruptor<MessageBoxMessage> disruptor)
    {
        var _connectors = new List<IConnector>();

        if (configuration.ContainsKey("sinks"))
        {
            var sinks = configuration["sinks"] as List<object>;
            if (sinks != null)
            {
                foreach (var section in sinks)
                {
                    var sectionDictionary = section as Dictionary<object, object>;
                    if (sectionDictionary != null)
                    {
                        IConnector connector = null;
                        var connectorType = (sectionDictionary.ContainsKey("connector")
                            ? Convert.ToString(sectionDictionary["connector"])?.ToLower()
                            : "undefined");

                        switch (connectorType)
                        {
                            case "mqtt":
                                connector = Mqtt.Sink.Create(sectionDictionary, disruptor);
                                break;
                            case "mtconnect":
                                connector = MtConnect.Sink.Create(sectionDictionary, disruptor);
                                break;
                            default:
                                break;
                        }

                        if (connector == null)
                        {
                            Logger.Error($"[Configurator.Sinks] Connector type is not supported: '{connectorType}'");
                        }
                        else if (connector.Configuration.Enabled)
                        {
                            _connectors.Add(connector);
                        }
                        else
                        {
                            Logger.Info($"[Configuration.Sinks] [{connector.Configuration.Name}] Connector is disabled.");
                        }
                    }
                    else
                    {
                        Logger.Warn($"[Configurator.Sinks] Configuration is invalid: '{section}'");
                    }
                }
            }
            else
            {
                Logger.Warn($"[Configurator.Sinks] Configuration key exists but it is empty.");
            }
        }
        else
        {
            Logger.Warn($"[Configurator.Sinks] Configuration key does not exist.");
        }

        if (configuration.ContainsKey("sources"))
        {
            var sources = configuration["sources"] as List<object>;
            if (sources != null)
            {
                foreach (var section in sources)
                {
                    var sectionDictionary = section as Dictionary<object, object>;
                    if (sectionDictionary != null)
                    {
                        IConnector connector = null;
                        var connectorType = (sectionDictionary.ContainsKey("connector")
                            ? Convert.ToString(sectionDictionary["connector"])?.ToLower()
                            : "undefined");

                        switch (connectorType)
                        {
                            case "ethernetip":
                                connector = EthernetIp.Source.Create(sectionDictionary, disruptor);
                                break;
                            case "mqtt":
                                connector = Mqtt.Source.Create(sectionDictionary, disruptor);
                                break;
                            default:
                                break;
                        }

                        if (connector == null)
                        {
                            Logger.Error($"[Configurator.Sources] Connector type is not supported: '{connectorType}'");
                        }
                        if (connector.Configuration.Enabled)
                        {
                            _connectors.Add(connector);
                        }
                        else
                        {
                            Logger.Info($"[Configurator.Sources] [{connector.Configuration.Name}] Connector is disabled.");
                        }
                    }
                    else
                    {
                        Logger.Warn($"[Configurator.Sinks] Configuration is invalid: '{section}'");
                    }
                }
            }
            else
            {
                Logger.Warn($"[Configurator.Sources] configuration key exists but it is empty.");
            }
        }
        else
        {
            Logger.Warn($"[Configurator.Sources] Configuration key does not exist.");
        }

        return _connectors;
    }

}