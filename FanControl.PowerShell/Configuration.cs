namespace FanControl.PowerShell
{


    public class Configuration
    {
        public class Sensor
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public int Interval { get; set; }
            public bool Enabled { get; set; }
            public string PowerShellFilePath { get; set; } = string.Empty;
        }

        public List<Sensor> TempSensors { get; set; } = new List<Sensor>();
        public List<Sensor> FanSensors { get; set; } = new List<Sensor>();

        public List<Sensor> ControlSensors { get; set; } = new List<Sensor>();
    }
}
