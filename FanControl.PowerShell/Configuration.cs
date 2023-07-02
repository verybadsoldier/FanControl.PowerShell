namespace FanControl.PowerShell
{


    public class Configuration
    {
        public class TempSensor
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public int Interval { get; set; }
            public bool Enabled { get; set; }
            public string PowerShellFilePath { get; set; } = string.Empty;
        }

        public class ControlSensor
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public int Interval { get; set; }
            public bool Enabled { get; set; }
            public string PowerShellFilePath { get; set; } = string.Empty;
        }

        public List<TempSensor> TempSensors { get; set; } = new List<TempSensor>();

        public List<ControlSensor> ControlSensors { get; set; } = new List<ControlSensor>();
    }
}
