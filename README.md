# Smart Watch Battery Tracker

This repository contains the code and GUI for Smart Watch Battery Tracker written in C# programming language using Microsoft Visual Studio C#.

Application Requirements

The application should be able to:

- search all nearby devices using Bluetooth Low Energy protocol
- connect to a bluetooth device (iHuntSmartwatch)
- get the battery level of the device
- show the battery level of Bluetooth Device on GUI

Below is the application architecture:

![BluetoothAppArchitecture](https://github.com/Andrei1999CJ/SmartWatchBatteryTracker/assets/86969370/bfc1c29b-8988-42e4-95c4-07f2a829eb2a)

How it works

- the app use a DeviceWatcher in order to find all nearby bluetooth devices
- when a device is found is added to a list
- when the user is selecting a device the application is connecting to the device
- we are configuring the GATT Session(Generic Attribute session) to always maintain the connection
- in all the device's services we are searching the one for battery level using the service's uuid

![image](https://github.com/Andrei1999CJ/SmartWatchBatteryTracker/assets/86969370/4048351e-b8e0-46e9-9b84-e1e04c3c3f1d)

- we get the battery level characteristic(app is searching for all characteristics)
- app subscribes for notifications for every characteristic
- refresh the value of the battery on GUI

Images

![image](https://github.com/Andrei1999CJ/SmartWatchBatteryTracker/assets/86969370/9ee6cd8b-6997-434f-8039-993f784cd544)

![image](https://github.com/Andrei1999CJ/SmartWatchBatteryTracker/assets/86969370/9f741283-bebe-4c0a-ab52-cfd0c98d0c4b)

![image](https://github.com/Andrei1999CJ/SmartWatchBatteryTracker/assets/86969370/624a52e2-9e0e-4c3a-9355-4675ca2b4fde)

![image](https://github.com/Andrei1999CJ/SmartWatchBatteryTracker/assets/86969370/e90bec63-e4d4-48e6-87a8-4628b9493dcd)

![image](https://github.com/Andrei1999CJ/SmartWatchBatteryTracker/assets/86969370/15cf04a9-7163-438a-90c0-339d5129d197)

References

https://btprodspecificationrefs.blob.core.windows.net/assigned-numbers/Assigned%20Number%20Types/Assigned_Numbers.pdf

https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/gatt-client





