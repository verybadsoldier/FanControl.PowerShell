using FanControl.Plugins;
using System.Numerics;
using System.CodeDom;
using System.Drawing.Text;
using System.Diagnostics;
using static FanControl.PowerShell.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using YamlDotNet.Serialization;
using System.Linq;

namespace FanControl.PowerShell
{
    public class PowerShellPlugin : IPlugin2
    {
        public string Name => "PowerShell";

        private Configuration _cfg;
        const string _cfgFilename = @"FanControl.PowerShell.yml";
        private List<ShellSensor> _tempSensors = new List<ShellSensor>();
        private List<ShellSensor> _fanSensors = new List<ShellSensor>();
        internal IPluginLogger _logger;
        private bool _errorState = false;

        public PowerShellPlugin(IPluginLogger logger)
        {
            _logger = logger;
            _cfg = new Configuration();
        }

        public void Close()
        {
        }

        public void Initialize()
        {
            _logger.Log("PowerShellPlugin: Initializing...");

            _logger.Log($"PowerShellPlugin: Reading config file: '{_cfgFilename}'");
            string text = File.ReadAllText(_cfgFilename);
            var deserializer = new DeserializerBuilder().Build();
            _cfg = deserializer.Deserialize<Configuration>(text);


            foreach (var sensorCfg in _cfg.ControlSensors.Concat(_cfg.FanSensors).Concat(_cfg.TempSensors))
            {
                if (sensorCfg.Interval < 0)
                {
                    _logger.Log($"PowerShellPlugin: ERROR: Interval of sensor {sensorCfg.Name} cannot be less than 0. Current value: {sensorCfg.Interval}");
                    _errorState = true;
                }

                if (!File.Exists(sensorCfg.PowerShellFilePath))
                {
                    _logger.Log($"PowerShellPlugin: ERROR: PowerShell script for sensor {sensorCfg.Name} cannot be found at path '{sensorCfg.PowerShellFilePath}'");
                    _errorState = true;

                }
            }

            if (_errorState)
            {
                _logger.Log($"PowerShellPlugin: Config Error (see above) - Plugin disabled");
            }
            else
            {
                _logger.Log($"PowerShellPlugin: Config OK");
            }
        }


        public void Load(IPluginSensorsContainer _container)
        {
            if (_errorState)
            {
                return;
            }

            foreach (var sensorCfg in _cfg.TempSensors.Where(x => x.Enabled))
            {
                var sensor = new ShellSensor(sensorCfg, _logger);
                _tempSensors.Add(sensor);
                _container.TempSensors.Add(sensor);
            }

            foreach (var sensorCfg in _cfg.FanSensors.Where(x => x.Enabled))
            {
                var sensor = new ShellSensor(sensorCfg, _logger);
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