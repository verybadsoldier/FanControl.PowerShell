# FanControl.PowerShell

## Introduction

This is a plugin for FanControl that can run custom PowerShell scripts to retrieve data and to send control commands. The plugin is customizable to define

My use case for developing this was to connect FanControl to my home automation system. I wanted the fans to spin at full speed when nobody is at home. So I can have a virtual temperature sensor with this plugin that reports a high temperature when nobody is at home so the fans will spin up.

## Requirements

FanControl .NET 7.0


## Configuration

The plugin has to be configured by a YAML configuration file. The file has to be named `FanControl.PowerShell.yml` and placed into the FanControl application folder.

There are three different sensor types that can be created by this plugin:
* Temperature Sensor
* Fan Sensor
* Control Sensor

All three types are supporting the mode `Update` and are expected to deliver a value on every call in this mode.

The `Control Sensor` is special as it is able to control something. But it also supports the modes `Reset` and `Set`.

In the configuration file on the top level there is a list element for every sensor type named accordingly `TempSensors`, `FanSensors` and `ControlSensors`.

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
      Name: FHEM Presence
      Interval: 60
      Enabled: True
      PowerShellFilePath: "FanControl.PowerShell_presence.ps1"

FanSensors:
    - Id: 2
      Name: FHEM Fan
      Interval: 4
      Enabled: False
      PowerShellFilePath: "fhem_presence.ps1"

```

## The PowerShell Scripts

The PowerShell scripts being used have to meet certain criterias to be compatible with this plugin. Scripts used for `TempSensor` and `FanSensor` to retrieve the sensor value (tempreature or fan speed) in some way and return it to the plugin. This is done by writing an object of type `PSCustomObject` to the sucess stream. To be able for the plugin to identify this result object, it has to has a property named `Tag` with the value `FanControl.PowerShell`. The sensor value has to be put into a property called `SensorValue` as type `[double]`, `[decimal]`or `[float]`.

*Simple example:*
```powershell
[PSCustomObject]@{
    Tag         = "FanControl.PowerShell"
    SensorValue = 23.3
}
```

More complex example that queries a value via HTTP and returns it to the plugin:
```powershell
Example:
```powershell
$Response = Invoke-WebRequest -URI "https://mydata.website.com/TemperatureEndpoint" | ConvertFrom-Json

[PSCustomObject]@{
    Tag         = "FanControl.PowerShell"
    SensorValue = [double]$Response.temperature.Value
}
```

They have to have one mandatory input paramter of type `string` called `Mode`. When the plugin starts the script, then this parameter will be passed over. The value will usually by `Update` for regular sensor updates:

```powershell
Param(
    [Parameter(Mandatory = $True)]
    [string]$Mode
)
```

In this mode, the PowerShell script is expected to retrieve the sensor value (temperature or fan speed) and return it to the plugin. 

