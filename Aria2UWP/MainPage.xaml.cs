using System;
using System.IO.Compression;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Aria2UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Window.Current.SetTitleBar(Title);
            CoreApplication.Suspending += CoreApplication_Suspending;
        }

        private async void CoreApplication_Suspending(object sender, SuspendingEventArgs e)
        {
            var def = e.SuspendingOperation.GetDeferral();
            string js = "window.location = \"#!/settings/rpc/set?protocol=http&host=127.0.0.1&port=6800&interface=jsonrpc&secret=ar\"";
            await wb.InvokeScriptAsync("eval", new string[] { js });
            def.Complete();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await UnZip(new Uri("ms-appx:///webui.zip"));
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            var session = await ApplicationData.Current.LocalFolder.TryGetItemAsync("aria2.session");
            if (session == null)
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("aria2.session");
            }
            wb.NavigationCompleted += Wb_NavigationCompleted;
            wb.Navigate(new Uri("ms-appdata:///local/webui/index.html"));
        }

        private async void Wb_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string js = "window.location = \"#!/settings/rpc/set?protocol=http&host=127.0.0.1&port=6800&interface=jsonrpc&secret=ar\"";
            await sender.InvokeScriptAsync("eval", new string[] { js });
            sender.NavigationCompleted -= Wb_NavigationCompleted;
            PR.IsActive = false;
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
            await ApplicationData.Current.LocalFolder.CreateFileAsync("aria2.session", CreationCollisionOption.ReplaceExisting);
            wb.NavigationCompleted += Wb_NavigationCompleted;
            wb.Navigate(new Uri("ms-appdata:///local/webui/index.html"));
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
