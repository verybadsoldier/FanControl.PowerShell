using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.PowerShell
{
    internal class ShellSensor : Plugins.IPluginSensor
    {
        public ShellSensor(Configuration.Sensor sensorCfg) : this(sensorCfg.Id, sensorCfg.Name, sensorCfg.Interval, sensorCfg.PowerShellFilePath) { }

        public ShellSensor(string id, string name, long intervalS, string powerShellFilePath)
        {
            _id = id;
            _name = name;
            _intervalS = intervalS;

            _script = File.ReadAllText(powerShellFilePath);
        }

        private readonly string _id;
        private readonly string _name;
        private readonly long _intervalS;
        protected readonly string _script;

        private float? x = 1.0f;
        private long _lastUpdate = 0;

        public string Id => _id;

        public string Name => _name;

        public float? Value
        {
            get
            {
                return x;
            }
            set { x = value; }
        }

        public void Update()
        {
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (_lastUpdate + _intervalS > now)
                return;

            using var ps = System.Management.Automation.PowerShell.Create();
            ps.AddScript(_script);
            ps.AddParameter("Command", "Update");
            ps.AddParameter("SensorName", Name);

            float? newValue = null;
            try
            {
                Collection<PSObject> objects = ps.Invoke();
                foreach (var obj in objects)
                {
                    if (obj == null)
                        continue;

                    if (obj.Properties.Any(x => x.Name == "Tag" && String.Equals(x.Value, "FanControl.PowerShell")))
                    {
                        var prop = obj.Properties["SensorValue"];
                        if (prop != null)
                        {
                            newValue = Convert.ToSingle(prop.Value);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Exception while executing script
            }

            if (newValue == null)
            {
                // Error reading value
            }

            Value = newValue;

            _lastUpdate = now;
        }
    }
}
