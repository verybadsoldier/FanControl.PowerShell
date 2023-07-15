using FanControl.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.PowerShell
{
    internal class ShellControlSensor : ShellSensor, IPluginControlSensor
    {
        public ShellControlSensor(string id, string name, long intervalS, string powerShellFilePath) : base(id, name, intervalS, powerShellFilePath)
        {
        }

        public void Set(float val)
        {
            using var ps = System.Management.Automation.PowerShell.Create();
            ps.AddScript(_script);
            ps.AddParameter("Command", "Set");
            ps.AddParameter("Value", val);
            ps.AddParameter("SensorName", Name);
        }

        public void Reset()
        {
            using var ps = System.Management.Automation.PowerShell.Create();
            ps.AddScript(_script);
            ps.AddParameter("Command", "Reset");
            ps.AddParameter("SensorName", Name);
        }
    }
}
