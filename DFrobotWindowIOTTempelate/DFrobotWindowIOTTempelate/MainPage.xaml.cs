using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Diagnostics;

namespace DFrobotWindowIOTTempelate
{
    public sealed partial class MainPage : Page
    {
        UsbSerial usb;                          //Handle the USB connction
        RemoteDevice arduino;                   //Handle the arduino
        private DispatcherTimer blinkTimer;     //Timer for the LED to blink every one second
        private const int LED_PIN = 13;         //Pin number of the on board LED
        private PinState ledState;              //Pin state of the LED

        public MainPage()
        {
            this.InitializeComponent();

            //USB VID and PID of the "Arduino Expansion Shield for Raspberry Pi B+"
            usb = new UsbSerial("VID_2341", "PID_8036");

            //Arduino RemoteDevice Constractor via USB.
            arduino = new RemoteDevice(usb);
            //Add DeviceReady callback when connecting successfully
            arduino.DeviceReady += onDeviceReady;

            //Baudrate on 57600 and SerialConfig.8N1 is the default config for Arduino devices over USB
            usb.begin(57600, SerialConfig.SERIAL_8N1);
        }

        private void onDeviceReady()
        {
            //After device is ready this function will be evoked.

            //Debug message "Device Ready" will be shown in the "Output" dialog.
            Debug.WriteLine("Device Ready");
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                setup();
            }));
        }

        private void setup()
        {
            //Set the initial state of the led.
            ledState = PinState.LOW;

            //Set the pin mode of the led.
            arduino.pinMode(LED_PIN, PinMode.OUTPUT);

            //Set the timer to schedule blink() every one second.
            blinkTimer = new DispatcherTimer();
            blinkTimer.Interval = TimeSpan.FromMilliseconds(1000);
            blinkTimer.Tick += blink;
            blinkTimer.Start();
        }

        private void blink(object sender, object e)
        {
            if (ledState == PinState.HIGH)  //LED state is HIGH.
            {
                //Turn off the LED.
                arduino.digitalWrite(LED_PIN, PinState.LOW);
                //Show the message in the Output dialog.
                Debug.WriteLine("OFF");
                //Set local LED state to Low.
                ledState = PinState.LOW;
            }
            else    //LED state is LOW.
            {
                //Turn on the LED.
                arduino.digitalWrite(LED_PIN, PinState.HIGH);
                //Show the message in the Output dialog.
                Debug.WriteLine("ON");
                //Set local LED state to Low.
                ledState = PinState.HIGH;
            }
        }
    }
}