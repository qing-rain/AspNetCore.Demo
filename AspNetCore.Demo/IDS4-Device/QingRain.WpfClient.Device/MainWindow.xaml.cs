using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QingRain.WpfClient.Device
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Load_Loaded(object sender, RoutedEventArgs e)
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            var deviceResponse = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = "wpfclient",
                ClientSecret = "secret"
            });

            CreateQrCode(deviceResponse.VerificationUriComplete);

            Process.Start(new ProcessStartInfo(deviceResponse.VerificationUriComplete) { UseShellExecute = true });

            string accessToken;

            while (true)
            {
                // request token
                var tokenResponse = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "wpfclient",
                    ClientSecret = "secret",
                    DeviceCode = deviceResponse.DeviceCode
                });

                if (!tokenResponse.IsError)
                {
                    accessToken = tokenResponse.AccessToken;
                    qrcodeImage.Visibility = Visibility.Collapsed;
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(deviceResponse.Interval));
            }

            await CallApiAsync(accessToken);
        }

        private async Task CallApiAsync(string token)
        {
            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(token);

            var response = await apiClient.GetAsync("https://localhost:6001/identity");

            if (!response.IsSuccessStatusCode)
            {
                apiResult.Text = response.StatusCode + response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();

                apiResult.Text = JArray.Parse(content).ToString();
            }
        }

        private void CreateQrCode(string verificationUriComplete)
        {
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(verificationUriComplete, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            qrcodeImage.Source = BitmapToImageSource(qrCodeImage);
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream memory = new();

            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }
    }
}
