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
        public ShellControlSensor(string id, string name, long intervalS, string powerShellFilePath, IPluginLogger logger) : base(id, name, intervalS, powerShellFilePath, logger)
        {
        }

        public void Set(float val)
        {
            using var ps = System.Management.Automation.PowerShell.Create();

            ps.AddScript(_script);
            ps.AddParameter("Command", "Set");
            ps.AddParameter("Value", val);
            ps.AddParameter("SensorName", Name);

            try
            {
                ps.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Log($"PowerShellPlugin: Set - Error running PowerShell script: {_script}: {ex.Message}");
            }
        }

        public void Reset()
        {
            using var ps = System.Management.Automation.PowerShell.Create();
            ps.AddScript(_script);
            ps.AddParameter("Command", "Reset");
            ps.AddParameter("SensorName", Name);

            try
            {
                ps.Invoke();
            }
            catch (Exception ex)
            {
                _logger.Log($"PowerShellPlugin: Reset - Error running PowerShell script: {_script}: {ex.Message}");
            }
        }
    }
}
