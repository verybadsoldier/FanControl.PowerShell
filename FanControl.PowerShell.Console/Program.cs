using FanControl.Plugins;
using FanControl.PowerShell;

//var c2 = new Class1();
//c2.Run();

var c = new Container();
var plugin = new FanControl.PowerShell.PowerShellPlugin();
plugin.Initialize();
plugin.Load(c);

while (true)
{
    plugin.Update();
    Thread.Sleep(1000);
}


class Container : IPluginSensorsContainer
{
    public List<IPluginControlSensor> ControlSensors => new List<IPluginControlSensor>();

    public List<IPluginSensor> FanSensors => new List<IPluginSensor>();

    public List<IPluginSensor> TempSensors => new List<IPluginSensor>();
}
