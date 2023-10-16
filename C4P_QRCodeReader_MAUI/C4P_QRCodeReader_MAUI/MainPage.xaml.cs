using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace C4P_QRCodeReader_MAUI
{
    public partial class MainPage : ContentPage
    {
        private string _LastCode;
       
        public string LastCode
        {
            get => _LastCode;

            set
            {
                _LastCode = value;
                OnPropertyChanged(nameof(LastCode));    
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            BindingContext = this;

            // On paramètre la caméra

            CameraView.BarCodeOptions = new Camera.MAUI.ZXingHelper.BarcodeDecodeOptions
            {
                AutoRotate = false,
                PossibleFormats = { ZXing.BarcodeFormat.QR_CODE },
                ReadMultipleCodes = false,
                TryHarder = false,
                TryInverted = false
            };

            CameraView.BarCodeDetectionFrameRate = 10;
            CameraView.BarCodeDetectionMaxThreads = 10;
            CameraView.ControlBarcodeResultDuplicate = false;
            CameraView.BarCodeDetectionEnabled = true;
            CameraView.TorchEnabled = false;           
        }

        private async void ContentPage_Unloaded(object sender, EventArgs e)
        {
            await CameraView.StopCameraAsync();
        }

        private async void CameraView_Loaded(object sender, EventArgs e)
        {
            if (CameraView.NumCamerasDetected > 0)
            {
                CameraView.Camera = CameraView.Cameras.First();
                await CameraView.StopCameraAsync();
                await CameraView.StartCameraAsync();
            }
        }

        private async void CameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {
            Debug.WriteLine("[CAMERA VIEW] - BARCODE DETECTED");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var r = args.Result.FirstOrDefault();

                if (r != null)
                {
                    LastCode = r.Text;
                    Debug.WriteLine($"[CAMERA VIEW] - {r.Text}");
                    Vibration.Vibrate();
                }
                else
                {
                    Debug.WriteLine("[CAMERA VIEW] - NO RESULT !");
                }
            });
        }

        private void CameraViewTapGesture_Tapped(object sender, TappedEventArgs e)
        {
            Debug.WriteLine("CAMERA VIEW FORCE AUTOFOCUS");
            CameraView.ForceAutoFocus();
        }

        private void TorchTapGesture_Tapped(object sender, TappedEventArgs e)
        {
            CameraView.TorchEnabled = !CameraView.TorchEnabled;
            TorchPath.Fill = CameraView.TorchEnabled ? Colors.Black : Colors.LightGray;
        }

        private async void LogoTapGesture_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                Uri uri = new Uri("https://www.coding4phone.com");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                // An unexpected error occurred. No browser may be installed on the device.
            }
        }
    }
}