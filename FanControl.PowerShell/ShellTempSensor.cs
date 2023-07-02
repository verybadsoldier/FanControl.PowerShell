using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.PowerShell
{
    internal class ShellTempSensor : Plugins.IPluginSensor
    {
        public ShellTempSensor(string id, string name, long intervalS)
        {
            _id = id;
            _name = name;
            _intervalS = intervalS;
        }

        private readonly string _id;
        private readonly string _name;
        private readonly long _intervalS;

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
            if ( _lastUpdate + _intervalS <= now)
            {
                Value += 0.1f;
                _lastUpdate = now;
            }

            Console.WriteLine($"Sensor {_name} {Value}");
        }
    }
}
