using System;
using System.IO.Compression;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Advertising.WinRT.UI;
using Windows.UI.Xaml.Media.Imaging;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Aria2UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public bool IsAppUpdated => SystemInformation.IsAppUpdated;
        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.SetTitleBar(Title);
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
            NativeAdsManagerV2 myNativeAdsManager = null;
            string myAppId = "9mz7pkhr7drh";
            string myAdUnitId = "1100048399";
            //string myAppId = "d25517cb-12d4-4699-8bdc-52040c712cab";
            //string myAdUnitId = "test";
            myNativeAdsManager = new NativeAdsManagerV2(myAppId, myAdUnitId);
            myNativeAdsManager.AdReady += MyNativeAd_AdReady;
            myNativeAdsManager.ErrorOccurred += MyNativeAdsManager_ErrorOccurred;
            myNativeAdsManager.RequestAd();

            await InitialzeEnvAndPage();
        }

        private void MyNativeAdsManager_ErrorOccurred(object sender, NativeAdErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("NativeAd error " + e.ErrorMessage +
        " ErrorCode: " + e.ErrorCode.ToString());
        }

        private void MyNativeAd_AdReady(object sender, NativeAdReadyEventArgs e)
        {
            NativeAdV2 nativeAd = e.NativeAd;

            //// Show the ad icon.
            if (nativeAd.AdIcon != null)
            {
                AdIconImage2.Source = nativeAd.AdIcon.Source;
                //// Adjust the Image control to the height and width of the 
                //// provided ad icon.
                AdIconImage2.Height = nativeAd.AdIcon.Height;
                AdIconImage2.Width = nativeAd.AdIcon.Width;
            }

            // Show the ad title.
            TitleTextBlock.Text = nativeAd.Title;

            if (nativeAd.MainImages.Count > 0)
            {
                NativeImage mainImage = nativeAd.MainImages[0];
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(mainImage.Url);
                AdIconImage.Source = bitmapImage;

                AdIconImage.Visibility = Visibility.Visible;
            }

            // Add the call to action string to the button.
            if (!string.IsNullOrEmpty(nativeAd.CallToActionText))
            {
                CallToActionButton.Content = nativeAd.CallToActionText;
                CallToActionButton.Visibility = Visibility.Visible;
            }
            nativeAd.RegisterAdContainer(NativeAdContainer);   
    }

        private void WebView_UnsafeContentWarningDisplaying(WebView sender, object args)
        {

        }

        private void WebView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            args.Handled = true;
            sender.Navigate(new Uri("ms-appdata:///local/webui/index.html" + args.Uri.Fragment));
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true,
                Title = "确认要重置吗？",
                PrimaryButtonText="OK",
                SecondaryButtonText="Cancel"
            };
            contentDialog.PrimaryButtonClick += async (a, b) =>
            {
                await Repair();
            };
            
            await contentDialog.ShowAsync();


        }

        private async System.Threading.Tasks.Task Repair()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("aria2.session", CreationCollisionOption.ReplaceExisting);
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            wb.NavigationCompleted += Wb_NavigationCompleted;
            wb.Navigate(new Uri("ms-appdata:///local/webui/index.html"));
        }
        bool FirstLaunchRepairFlag = false;
        private async void Wb_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string js = "window.location = \"#!/settings/rpc/set?protocol=http&host=127.0.0.1&port=6800&interface=jsonrpc&secret=ar\"";
            await sender.InvokeScriptAsync("eval", new string[] { js });
            sender.NavigationCompleted -= Wb_NavigationCompleted;
            PR.IsActive = false;
            if ((IsAppUpdated || SystemInformation.IsFirstRun)&&!FirstLaunchRepairFlag)
            {
                FirstLaunchRepairFlag = true;
                await Repair();
            }
        }


        private async System.Threading.Tasks.Task InitialzeEnvAndPage()
        {
            if (IsAppUpdated || SystemInformation.IsFirstRun)
            {
                await UnZip(new Uri("ms-appx:///webui.zip"));
                //await CoreApplication.RequestRestartAsync(string.Empty);
            }
           
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                var session = await ApplicationData.Current.LocalFolder.TryGetItemAsync("aria2.session");
                if (session == null)
                {
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("aria2.session");
                }
                wb.NavigationCompleted += Wb_NavigationCompleted;
                wb.Navigate(new Uri("ms-appdata:///local/webui/index.html"));
            
        }


        private static async System.Threading.Tasks.Task UnZip(Uri uri, bool overwrite = true)
        {
            StorageFile sf = await StorageFile.GetFileFromApplicationUriAsync(uri);
            sf = await sf.CopyAsync(ApplicationData.Current.LocalFolder, sf.Name, NameCollisionOption.ReplaceExisting);
            using (ZipArchive archive = ZipFile.Open(sf.Path, ZipArchiveMode.Update))
            {
                archive.ExtractToDirectory(ApplicationData.Current.LocalFolder.Path, true);
            }
        }


    }
}
