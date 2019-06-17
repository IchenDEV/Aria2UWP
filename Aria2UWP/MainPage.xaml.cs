using System;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Advertising.WinRT.UI;
using Windows.UI.Xaml.Media.Imaging;
using Aria2UWP.Tools;
using System.Threading.Tasks;
using System.Threading;

namespace Aria2UWP
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// 应用ID和广告ID
        /// </summary>
        static string AppId = "9mz7pkhr7drh";
        static string AdUnitId = "1100048399";

        public bool IsAppUpdated => SystemInformation.IsAppUpdated;

        public MainPage()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Current_Suspending;
        }

        private void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            //  throw new NotImplementedException();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            Window.Current.SetTitleBar(Title);

            await InitialzeEnvAndPage();
        }

        private void MyNativeAdsManager_ErrorOccurred(object sender, NativeAdErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("NativeAd error " + e.ErrorMessage + " ErrorCode: " + e.ErrorCode.ToString());

            ((NativeAdsManagerV2)sender).RequestAd();
        }

        private void WebView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            args.Handled = true;
            sender.Navigate(new Uri("ms-appdata:///local/webui/index.html" + args.Uri.Fragment));
        }

        private void MyNativeAd_AdReady(object sender, NativeAdReadyEventArgs e)
        {
            SetAdUI(e.NativeAd);
        }

        private void SetAdUI(NativeAdV2 nativeAd)
        {
            if (nativeAd.AdIcon != null)
            {
                AdIconImage2.Source = nativeAd.AdIcon.Source;
            }

            // Show the ad title.
            TitleTextBlock.Text = nativeAd.Title;

            if (nativeAd.MainImages.Count > 0)
            {
                NativeImage mainImage = nativeAd.MainImages[0];
                BitmapImage bitmapImage = new BitmapImage
                {
                    UriSource = new Uri(mainImage.Url)
                };
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

        private async System.Threading.Tasks.Task Repair()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("aria2.session", CreationCollisionOption.ReplaceExisting);
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            wb.NavigationCompleted += Wb_NavigationCompleted;
            wb.Navigate(new Uri("ms-appdata:///local/webui/index.html"));
        }
        private async void Wb_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            

            string js = "window.location = \"#!/settings/rpc/set?protocol=http&host=127.0.0.1&port=6800&interface=jsonrpc&secret=ar\"";
            await sender.InvokeScriptAsync("eval", new string[] { js });
            sender.NavigationCompleted -= Wb_NavigationCompleted;
            AllLoaded();
        }

        private void AllLoaded()
        {
            PR.Visibility = Visibility.Collapsed;
            Header.Visibility = Visibility.Visible;
            LoadingText.Text = "";
        }

        private async System.Threading.Tasks.Task InitialzeEnvAndPage()
        {
            LoadingText.Text = "加载广告";
            PrepareAds();


            if (IsAppUpdated || SystemInformation.IsFirstRun)
            {
                LoadingText.Text = "解压 AriaNg";
                await PackageTools.UnZip(new Uri("ms-appx:///webui.zip"));


                ContentDialog contentDialog = new ContentDialog()
                {
                    IsPrimaryButtonEnabled = true,
                    Title = "还差一步",
                    Content = "激活Aria2",
                    PrimaryButtonText = "激活"
                };
                contentDialog.PrimaryButtonClick += async (e, a) =>
                {
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                };
                await contentDialog.ShowAsync();
            }
            else
            {
                LoadingText.Text = "加载Aria2";
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();

            }

            LoadingText.Text = "处理Session";
            await GetSessionAndStartAriaNG();

        }

        private async Task GetSessionAndStartAriaNG()
        {
            var session = await ApplicationData.Current.LocalFolder.TryGetItemAsync("aria2.session");
            if (session == null)
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("aria2.session");
            }
            var log = await ApplicationData.Current.LocalFolder.TryGetItemAsync("aria2.log");
            if (log == null)
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("aria2.log");
            }
            wb.NavigationCompleted += Wb_NavigationCompleted;
            LoadingText.Text = "加载 AriaNG";
            wb.Navigate(new Uri("ms-appdata:///local/webui/index.html"));
        }

        private void PrepareAds()
        {
            NativeAdsManagerV2 myNativeAdsManager = new NativeAdsManagerV2(AppId, AdUnitId);
            myNativeAdsManager.AdReady += MyNativeAd_AdReady;
            myNativeAdsManager.ErrorOccurred += MyNativeAdsManager_ErrorOccurred;
            myNativeAdsManager.RequestAd();
        }

        private async void RepairButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            ContentDialog contentDialog = new ContentDialog
            {
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true,
                Title = "确认要重置吗？",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消"
            };
            contentDialog.PrimaryButtonClick += async (a, b) =>
            {
                await Repair();
            };

            await contentDialog.ShowAsync();
        }

        private void BurMyBeerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BDButton_Click(object sender, RoutedEventArgs e)
        {
            VSCodeInAppNotification.Show();
        }

        private void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            VSCodeInAppNotification.Dismiss();
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            VSCodeInAppNotification.Dismiss();
            Grid pa = ((StackPanel)((Button)sender).Parent).Parent as Grid;
            var BdUriTB = pa.FindName("BdUriTB") as TextBox;
            var BdPass = pa.FindName("BdPassTB") as TextBox;
            if (BdUriTB == null || BdUriTB.Text == String.Empty)
            {
                return;
            }
            if (BdPass == null || BdPass.Text == String.Empty)
            {
                return;
            }

            WebView NoUI = new WebView(WebViewExecutionMode.SeparateThread);
            NoUI.NavigationCompleted += NoUI_NavigationFirstPageCompleted;
            NoUI.Navigate(new Uri(String.Format("{0}?pwd={1}", BdUriTB.Text.Replace("pan.baidu.com", "pan.baiduwp.com"), BdPass.Text)));


        }

        private async void NoUI_NavigationFirstPageCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            sender.NavigationCompleted -= NoUI_NavigationFirstPageCompleted;
            var resList = await sender.InvokeScriptAsync("eval", new[] { "document.getElementsByClassName('list-group')[0].innerHTML" });
            while (resList == "")
            {
                Thread.Sleep(10000);
                resList = await sender.InvokeScriptAsync("eval", new[] { "document.getElementsByClassName('list-group')[0].innerHTML" });
            }
            HtmlAgilityPack.HtmlDocument list = new HtmlAgilityPack.HtmlDocument();
            list.LoadHtml(resList);
            var items = list.DocumentNode.SelectNodes("/li/a");
            foreach (var item in items)
            {

            }
        }
    }
}
