using FanControl.Plugins;
using System.Numerics;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.CodeDom;
using System.Drawing.Text;
using System.Diagnostics;
using static FanControl.PowerShell.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Core;
using System.ComponentModel;

namespace FanControl.PowerShell
{
    public class PowerShellPlugin : IPlugin2
    {
        public string Name => "PowerShell";

        private Configuration _cfg;
        const string _cfgFilename = @"FanControl.PowerShell.yml";
        private List<ShellSensor> _tempSensors = new List<ShellSensor>();
        private List<ShellSensor> _fanSensors = new List<ShellSensor>();

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
            foreach (var sensorCfg in _cfg.TempSensors.Where(x => x.Enabled))
            {
                var sensor = new ShellSensor(sensorCfg);
                _tempSensors.Add(sensor);
                _container.TempSensors.Add(sensor);
            }

            foreach (var sensorCfg in _cfg.FanSensors.Where(x => x.Enabled))
            {
                var sensor = new ShellSensor(sensorCfg);
                _fanSensors.Add(sensor);
                _container.FanSensors.Add(sensor);
            }
        }

        public void Update()
        {
            foreach(var sensor in _tempSensors.Concat(_fanSensors))
            {
                sensor.Update();
            }            
        }
    }
}