using System;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothApp_V1
{
    public partial class Form1 : Form
    {
        
        DeviceInformation[] bluetoothDevices = new DeviceInformation[20];
        private int detectedDevices = 0;
        private bool deviceAlreadyFound;
        private const string batteryUuid= "180f";
        private byte receivedBatteryLevelValue;
        private bool firstTime;

        public Form1()
        {
            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher deviceWatcher =
                        DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;


            deviceWatcher.Start();
            InitializeComponent();
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // do nothing
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // do nothing
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (args.Name != "")
            {
                for (int j = 0; j < detectedDevices; j++)
                {
                    if (args.Name == bluetoothDevices[j].Name)
                    {
                        deviceAlreadyFound = true;
                    }
                }
                if (deviceAlreadyFound == false && detectedDevices < 19) 
                    bluetoothDevices[detectedDevices++] = args;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int j = 0; j < detectedDevices; j++)
            {
                if (!(bluetoothList.Items.Contains(bluetoothDevices[j].Name)))
                {
                    bluetoothList.Items.Add(bluetoothDevices[j].Name);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            batteryLevelHandler(receivedBatteryLevelValue);
        }

        private void bluetoothList_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Visible = true;
            label1.Text = "Pairing";
            connectToDevice(bluetoothDevices[bluetoothList.SelectedIndex]);
        }

        async void connectToDevice(DeviceInformation deviceInfo)
        {
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id).AsTask();
            GattSession gattSessionForBlDevice = await GattSession.FromDeviceIdAsync(bluetoothLeDevice.BluetoothDeviceId);
            gattSessionForBlDevice.MaintainConnection = true;
            GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();
            if (result.Status == GattCommunicationStatus.Success)
            {
                label1.Text = "Connected";
                bluetoothList.Enabled = false;
                var services = result.Services;
                foreach (var service in services)
                {
                    if (service.Uuid.ToString("N").Substring(4, 4) == batteryUuid)
                    {
                        GattCharacteristicsResult theresult = await service.GetCharacteristicsAsync();
                        if (theresult.Status == GattCommunicationStatus.Success)
                        {
                            var characteristics = theresult.Characteristics;
                            foreach (var characteristic in characteristics)
                            {
                                timer2.Enabled = true;
                                getFirstRead(characteristic);
                                subscribeForNotifications(characteristic);
                            }
                        }
                    }
                }
            }
        }

        async void subscribeForNotifications(GattCharacteristic gattCharacteristic)
        {
            GattCommunicationStatus status = await gattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if (status == GattCommunicationStatus.Success)
            {
                gattCharacteristic.ValueChanged += GattCharacteristic_ValueChanged;
                batteryLevelHandler(receivedBatteryLevelValue);
            }
        }

        async void getFirstRead(GattCharacteristic gattCharacteristic)
        {
            GattReadResult result = await gattCharacteristic.ReadValueAsync();
            if (result.Status == GattCommunicationStatus.Success)
            {
                var reader = DataReader.FromBuffer(result.Value);
                byte[] input = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(input);
                receivedBatteryLevelValue = input[0];
            }
        }

        private void GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] input = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(input);
            receivedBatteryLevelValue = input[0];
        }

        private void batteryLevelHandler(byte batteryLevel)
        {
            const int maximumPercentange = 100;
            if (batteryLevel > maximumPercentange)  // This means that the smartwatch is charging
            {
                if (firstTime == true) // first time when the smartwatch is in charging state
                {
                    progressBar.Value = 0;
                    firstTime = false;
                }
                label3.Text = "Charging...";
                label3.Visible = true;
                progressBar.Increment(5);
                progressBar.Visible = true;
                if (progressBar.Value == 100)
                    progressBar.Value = 0;
            }
            else 
            {
                firstTime = true;
                label3.Text = batteryLevel + "%";
                label3.Visible = true;
                progressBar.Value = batteryLevel;
                progressBar.Visible = true;
            }
        }

    }
}
