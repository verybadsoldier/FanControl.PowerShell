using FanControl.Plugins;
using FanControl.PowerShell;

var c = new Container();
var logger = new Logger();
var plugin = new FanControl.PowerShell.PowerShellPlugin(logger);
plugin.Initialize();
plugin.Load(c);

while (true)
{
    plugin.Update();
    Thread.Sleep(1000);

    Console.WriteLine(c.TempSensors.Select(x => x.Value.ToString()).ToList()[0]);
}

class Logger : IPluginLogger
{
    public void Log(string message)
    {
        Console.WriteLine($"{message}");
    }
}

class Container : IPluginSensorsContainer
{
    private List<IPluginControlSensor> _controlSensors = new List<IPluginControlSensor>();
    private List<IPluginSensor> _fanSensors = new List<IPluginSensor>();
    private List<IPluginSensor> _tempSensors = new List<IPluginSensor>();

    public List<IPluginControlSensor> ControlSensors { get { return _controlSensors; } }

    public List<IPluginSensor> FanSensors { get { return _fanSensors; } }

    public List<IPluginSensor> TempSensors { get {  return _tempSensors; } }

}
