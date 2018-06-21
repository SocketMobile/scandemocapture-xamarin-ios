using System;

using UIKit;
using SocketMobile.Capture;
using System.Threading.Tasks;

namespace scandemocapture
{
    public partial class ViewController : UIViewController
    {
        // Member variables for the Capture Helper client, and for the
        // currently-connected scanner (if any)

        CaptureHelper capture = new CaptureHelper();
        CaptureHelperDevice CurrentDevice = null;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        // This code does the actual "open" of the Capture SDK.
        //
        // NOTE: each new application you create requires its own unique
        // application ID, which can be obtained from the Socket Mobile
        // Developers website.

        public async Task<long> OpenCaptureClient(CaptureHelper captureHelper)
        {
            Console.WriteLine("Start OpenCaptureClient()");
            Console.WriteLine("About to call capture.OpenAsync().");

            long result = await captureHelper.OpenAsync("ios:com.socketmobile.scandemocapture",
                                                        "bb57d8e1-f911-47ba-b510-693be162686a",
                                                        "MC4CFQCfxxekfXioQRFwZQnq3PaTC6/UNwIVALRvY8iFzO8umeBv/kO5O374UOQu");

            Console.WriteLine("Back from capture.OpenAsync(), result: " + result);

            Console.WriteLine("End OpenCaptureClient()");
            return result;
        }

        // This code does the actual "close" of the Capture SDK.

        public async Task<long> CloseCaptureClient(CaptureHelper captureHelper)
        {
            Console.WriteLine("Start CloseCaptureClient()");
            long result = await captureHelper.CloseAsync();
            Console.WriteLine("End CloseCaptureClient()");
            return result;
        }

        // Initialize the CaptureHelper object here

        public override async void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // Assign event handlers for the Capture events we're interested in

            Console.WriteLine("ViewDidAppear: Assigning handlers");
            capture.DeviceArrival += OnDeviceArrival;
            capture.DeviceRemoval += OnDeviceRemoval;
            capture.DecodedData += OnDecodedData;
            capture.Errors += OnError;

            long result = await OpenCaptureClient(capture);

            if (!SktErrors.SKTSUCCESS(result))
            {
                Console.WriteLine("OpenCaptureClient failed!");
            }
        }

        // Shut down the CaptureHelper object here

        public override async void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            Console.WriteLine("ViewWillDisappear: Unassigning handlers");

            // Wait on the close first

            long result = await CloseCaptureClient(capture);

            // then unassign the event handlers

            capture.DeviceArrival -= OnDeviceArrival;
            capture.DeviceRemoval -= OnDeviceRemoval;
            capture.DecodedData -= OnDecodedData;
            capture.Errors -= OnError;
        }

        // The following routines are not functioning properly

#if false

        // Handler to display battery level changes on screen as they occur

        void OnDeviceBatteryLevel(object sender, CaptureHelper.BatteryLevelArgs e)
        {
            Console.WriteLine("Start OnDeviceBatteryLevel()");
            CurrentDevice.BatteryLevel = CaptureHelper.ConvertBatteryLevelInPercentage(e.MinLevel, e.MaxLevel, e.CurrentLevel);

            InvokeOnMainThread(() =>
            {
                battLevelLabel.Text = CurrentDevice.BatteryLevel;
            });
            Console.WriteLine("End OnDeviceBatteryLevel()");
        }

        // Handler to display power state changes on screen as they occur

        void OnDevicePowerState(object sender, CaptureHelper.PowerStateArgs e)
        {
            string strPowerState;

            Console.WriteLine("Start OnDevicePowerState()");
            switch (e.State)
            {
                case CaptureHelper.PowerState.AC:
                    strPowerState = "On AC";
                    break;
                case CaptureHelper.PowerState.Battery:
                    strPowerState = "On Battery";
                    break;
                case CaptureHelper.PowerState.Cradle:
                    strPowerState = "In Cradle";
                    break;
                default:
                    strPowerState = "Unknown";
                    break;
            }
            InvokeOnMainThread(() =>
            {
                powerStatusLabel.Text = strPowerState;
            });
            Console.WriteLine("End OnDevicePowerState()");
        }

#endif

        // Callback for Capture, when a scanner connects

        async void OnDeviceArrival(object sender, CaptureHelper.DeviceArgs arrivedDevice)
        {
            Console.WriteLine("Start ScannerConnect()");
            if (CurrentDevice == null)
            {
                CurrentDevice = arrivedDevice.CaptureDevice;

                // Friendly name is available immediately
                InvokeOnMainThread(() =>
                {
                    scannerLabel.Text = CurrentDevice.GetDeviceInfo().Name;
                    Console.WriteLine("Device friendly name is: " + CurrentDevice.GetDeviceInfo().Name);
                });

                // Get the Bluetooth address of the connected device
                Console.WriteLine("Requesting the Bluetooth address.");
                CaptureHelperDevice.BluetoothAddressResult resultBdAddr = await CurrentDevice.GetBluetoothAddressAsync(".");

                // Put the BdAddr out there
                if (SktErrors.SKTSUCCESS(resultBdAddr.Result))
                {
                    Console.WriteLine("Got the Bluetooth address.");
                    InvokeOnMainThread(() =>
                    {
                        Console.WriteLine("BdAddr is: " + resultBdAddr.BluetoothAddress);
                        bdaddrLabel.Text = resultBdAddr.BluetoothAddress;
                    });
                }
                else
                    Console.WriteLine("Error retrieving the Bluetooth address!");

                // Get the current battery level
                Console.WriteLine("Requesting the battery level.");
                CaptureHelperDevice.BatteryLevelResult resultBattery = await CurrentDevice.GetBatteryLevelAsync();

                // Put the Battery Level out there
                if (SktErrors.SKTSUCCESS(resultBattery.Result))
                {
                    Console.WriteLine("Got the battery level.");
                    InvokeOnMainThread(() =>
                    {
                        battLevelLabel.Text = resultBattery.Percentage;
                    });
                }
                else
                    Console.WriteLine("Error retrieving the battery level!");

                // This section causing problems on some scanners

#if false

                CaptureHelperDevice.PowerStateResult powerState = await CurrentDevice.GetPowerStateAsync();
                if (SktErrors.SKTSUCCESS(powerState.Result))
                {
                    string strPowerState;

                    switch (powerState.State)
                    {
                        case CaptureHelper.PowerState.AC:
                            strPowerState = "On AC";
                            break;
                        case CaptureHelper.PowerState.Battery:
                            strPowerState = "On Battery";
                            break;
                        case CaptureHelper.PowerState.Cradle:
                            strPowerState = "In Cradle";
                            break;
                        default:
                            strPowerState = "Unknown";
                            break;
                    }
                    InvokeOnMainThread(() =>
                    {
                        powerStatusLabel.Text = strPowerState;
                    });
                }
                else
                    Console.WriteLine("Error retrieving the power state!");

                 //See if we can register for live battery status updates
                
                 //Should assign a handler for battery status changes up where
                 //the capture openAsyc() happens.

                Console.WriteLine("Requesting notifications.");

                CaptureHelperDevice.NotificationsResult resultNotifications = await CurrentDevice.GetNotificationsAsync();
                Console.WriteLine("Got the notifications.");
                if (resultNotifications.IsSuccessful())
                {
                    if ((!resultNotifications.Notifications.BatteryLevel) || (!resultNotifications.Notifications.PowerState))
                    {
                        resultNotifications.Notifications.BatteryLevel = true;
                        resultNotifications.Notifications.PowerState = true;
                        CaptureHelper.AsyncResult result = await CurrentDevice.SetNotificationsAsync(resultNotifications.Notifications);

                        if (!result.IsSuccessful())
                        {
                            Console.WriteLine("Unable to set the power notifications.");
                        }
                        else
                        {
                            CurrentDevice.DeviceBatteryLevel += OnDeviceBatteryLevel;
                            CurrentDevice.DevicePowerState += OnDevicePowerState;
                        }
                    }
                    else
                    {
                        CurrentDevice.DeviceBatteryLevel += OnDeviceBatteryLevel;
                        CurrentDevice.DevicePowerState += OnDevicePowerState;
                    }
                }
                else
                {
                    // display an error message...
                    Console.WriteLine("Device.GetNotificationsAsync did not work.");
                }
                //DoScannerConnect();
            }
            else
            {
                // Maybe a new scanner has arrived, but the simple act of
                // assigning the DeviceArrival handler will cause Capture
                // to notify us if a scanner is connected.

                Console.WriteLine("We already have a scanner connected!!");

                // Assign power info handlers

                //CurrentDevice.DeviceBatteryLevel += OnDeviceBatteryLevel;
                //CurrentDevice.DevicePowerState += OnDevicePowerState;

#endif

            }
            Console.WriteLine("End ScannerConnect()");
        }

        // Clear the scanner information from the text boxes on-screen when
        // the scanner goes away

        void DoScannerDisconnect()
        {
            Console.WriteLine("Start DoScannerDisconnect()");
            InvokeOnMainThread(() =>
            {
                bdaddrLabel.Text = "";
                scannerLabel.Text = "";
                battLevelLabel.Text = "";
                powerStatusLabel.Text = "";
                scannedDataLabel.Text = "";
            });
            Console.WriteLine("End DoScannerDisconnect()");
        }

        // Callback for Capture, when a scanner disconnects

        void OnDeviceRemoval(object sender, CaptureHelper.DeviceArgs removedDevice)
        {
            Console.WriteLine("Start ScannerDisconnect()");
            CurrentDevice = null; // we don't have a connected scanner
            DoScannerDisconnect(); // do the work
            Console.WriteLine("End ScannerDisconnect()");
        }

        // Display the scanned data on screen in the appropriate text control

        void OnDecodedData(object sender, CaptureHelper.DecodedDataArgs decodedData)
        {
            Console.WriteLine("Start OnDecodedData()");
            string Data = decodedData.DecodedData.DataToUTF8String;
            InvokeOnMainThread(() =>
            {
                scannedDataLabel.Text = Data;
            });
            Console.WriteLine("End OnDecodedData()");
        }

        // Report Capture SDK errors on the console

        void OnError(object sender, CaptureHelper.ErrorEventArgs e)
        {
            Console.WriteLine(String.Format("OnError(): {0}", e.Message));
            if (SktErrors.SKTSUCCESS(e.Result))
                Console.WriteLine("Result is SUCCESSFUL");
            else
                Console.WriteLine("Result is FAILURE");
        }
    }
}
