# FanControl.PowerShell

![Logo](https://github.com/PowerShell/PowerShell/blob/master/assets/Powershell_256.png?raw=true)
## Introduction

This is a plugin for FanControl that can run custom PowerShell scripts to retrieve data and to send control commands. The plugin is customizable by defining PowerShell script files that are executed everytime a sensor has is to be retrieved or a control action is executed.

My use case for developing this was to connect FanControl to my home automation system. I wanted the fans to spin at full speed when nobody is at home. So I can have a virtual temperature sensor with this plugin that reports a high temperature when nobody is at home so the fans will spin up. Is there a use case for anybody else? Well, I don't know actually.

## Requirements

FanControl .NET 7.0. _Does not work with .NET Framework version_

## Quick Start

As a quick start without having to read the whole documentation, Just do this:

1. Download the release ZIP file and extract it to your `Plugins` directory
2. Create the configuration file `FanControl.PowerShell.yml` in your FanControl folder and put for example this content to create one temperature sensor:
```yaml
TempSensors:
    - Id: 1
      Name: My PowerShell Temp Sensor
      Interval: 1
      Enabled: True
      PowerShellFilePath: "my_temp_sensor.ps1"
```
3. Create the PowerShell script file `my_temp_sensor.ps1` mentioned in the configuration file with the following example content:
```powershell
Param(
    [string]$Command,
    [string]$SensorName
)

$value = 12.2

[PSCustomObject]@{
    Tag         = "FanControl.PowerShell"
    SensorValue = $value
}
```

4. (Re)Start FanControl. Now should you see a new temperature sensor named `My PowerShell Temp Sensor` in any temperature sensor list


## Configuration

The plugin has to be configured by a YAML configuration file. The file has to be named `FanControl.PowerShell.yml` and placed into the FanControl application folder.

There are three different sensor types that can be configured:
* Temperature Sensor
* Fan Sensor
* Control Sensor

In the configuration file on the top level in the YAML structure, there is a list element for every sensor type named accordingly `TempSensors`, `FanSensors` and `ControlSensors`.

### Parameters
All three types have the same configuration parameters:

| Name     | Type | Decription |
| -------- | ------- | --- |
| Id  | string    | An ID used internally that should be unique. |
| Name | string     | A string describing the sensor. This name is visible in FanControl. |
| Interval    | int    | An integer value in seconds. FanControl interally updates sensors with 1 Hz but with this parameter the update frequency can be lowered as needed individualyl for every sensor. |
| Enabled | boolean | Enable or disable the sensor. |
| PowerShellFilePath | string | A file path to the PowerShell script to be executed for every mode. This can either be an absolute path or a path relative to the FanControl application directory. |




### Example

```yaml
TempSensors:
    - Id: 1
      Name: My Temp
      Interval: 60
      Enabled: True
      PowerShellFilePath: "FanControl.PowerShell_temp.ps1"

FanSensors:
    - Id: 2
      Name: My Fan
      Interval: 4
      Enabled: False
      PowerShellFilePath: "fan.ps1"

```

## The PowerShell Scripts

The PowerShell scripts being used have to meet certain criterias to be compatible with this plugin. With the `Update` comand (see below), scripts used for `TempSensor` and `FanSensor` have to retrieve the sensor value (tempreature or fan speed) in some way and return it to the plugin. This is done by writing an object of type `PSCustomObject` to the success stream in the PowerShell script. To be able for the plugin to identify this result object, it has to has a property named `Tag` with the value `FanControl.PowerShell`. The sensor value has to be put into a property called `SensorValue` as type `[double]`, `[decimal]`or `[float]`.

### Input Paramters
There are three different types of `Command` a plugin can be called with. `Update` is the most common one. It is supported by all plugin types and it's used to retrieve a sensor value as described above. The plugin type `ControlSensor` also supports the command `Set` and `Reset`.


| Plugin Type     | Update | Set | Reset |
| --------------- |:------:|:---:|:-----:|
| TempSensor      | x      |     |       |
| FanSensor       | x      |     |       |
| ControlSensor   | x      | x   |  x    |

The `Command` is passed over to every script call as a parameter. Inside the script, it can be used to differentiate between the command types and act accordingly.

Then there are also these additional parameters passed over to script calls. This is the complete list of parameters:
| Parameter Name  | Command   | Type   | Description                                                                                      |
| --------------- |:---------:|:------:|--------------------------------------------------------------------------------------------------|
| Command         | Always    | string | Contains the command type (`Update`, `Reset`, `Set`)                                             |
| SensorName      | Always    | string | Name of the sensor as configured. Can be used to handle multiple sensors in the same script      |
| Value           | Set       | float  | Value to set for a ControlSensor                                                                 |


Simple example for a `TempSensor` or `FanSensor` script that returns a hardcoded value:
```powershell
Param(
    [string]$Command,
    [string]$SensorName
)

$value = 12.2

[PSCustomObject]@{
    Tag         = "FanControl.PowerShell"
    SensorValue = $value
}
```

More complex example that queries a value via HTTP and returns it to the plugin:
```powershell
Param(
    [string]$Command,
    [string]$SensorName
)

$Response = Invoke-WebRequest -URI "https://mydata.website.com/TemperatureEndpoint" | ConvertFrom-Json

[PSCustomObject]@{
    Tag         = "FanControl.PowerShell"
    SensorValue = [double]$Response.temperature.Value
}
```

In this mode, the PowerShell script is expected to retrieve the sensor value (temperature or fan speed) and return it to the plugin. 

A `ConstrolSensor` script would need to also have the `Value` parameter for the `Set` command:

```powershell
Param(
    [string]$Command,
    [string]$SensorName
    [string]$Value
)

# Do things here
```

## FAQ

### It does not work! What can I do?

Try to run your PowerShell script manually in a console and see if it works there.


### It works when running manually, but not in FanControl!

Take a look at the file `log.txt` in the FanControl application folder. It might give you an idea what the problem is. Feel free to file an issue on Github describing what the problem is, what you did and how your configuration looks.

### When using this plugin, FanControl gets super sluggish!

Depending on how heavy your PowerShell script is, it might slow down FanControl when called to frequently. Maybe run it less often by increasing the `Interval` parameter.