using FanControl.Plugins;
using System.Numerics;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.CodeDom;
using System.Drawing.Text;
using System.Diagnostics;
using static FanControl.PowerShell.Configuration;

namespace FanControl.PowerShell
{
    public class PowerShellPlugin : IPlugin2
    {
        public string Name => "PowerShell";

        private Configuration _cfg;
        const string _cfgFilename = @"FanControl.PowerShell.yml";
        private List<ShellTempSensor> _tempSensor = new List<ShellTempSensor>();
        public void Close()
        {
        }

        public void Initialize()
        {
            string text = File.ReadAllText(_cfgFilename);
            var deserializer = new DeserializerBuilder().Build();
            _cfg = deserializer.Deserialize<Configuration>(text);
        }

        public void Load(IPluginSensorsContainer _container)
        {
            foreach(var tempSensor in _cfg.TempSensors)
            {
                if (!tempSensor.Enabled)
                {
                    continue;
                }

                var sensor = new ShellTempSensor(tempSensor.Id, tempSensor.Name, tempSensor.Interval);
                _tempSensor.Add(sensor);
                _container.TempSensors.Add(sensor);
            }
        }

        public void Update()
        {
            foreach(var sensor in _tempSensor)
            {
                sensor.Update();
            }            
        }
    }
}