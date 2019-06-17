using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Aria2UWP
{
    public sealed partial class BMBCon : UserControl
    {
        private StoreContext context = null;
        public BMBCon()
        {
            this.InitializeComponent();
        }
        public async void PurchaseAddOn(string storeId)
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
                // If your app is a desktop app that uses the Desktop Bridge, you
                // may need additional code to configure the StoreContext object.
                // For more info, see https://aka.ms/storecontext-for-desktop.
            }

            workingProgressRing.IsActive = true;
            StorePurchaseResult result = await context.RequestPurchaseAsync(storeId);
            workingProgressRing.IsActive = false;

            // Capture the error message for the operation, if any.
            string extendedError = string.Empty;
            if (result.ExtendedError != null)
            {
                extendedError = result.ExtendedError.Message;
            }

            switch (result.Status)
            {
                case StorePurchaseStatus.Succeeded:
                    textBlock.Text = "感谢赞助";
                    break;

                case StorePurchaseStatus.NotPurchased:
                    textBlock.Text = "取消赞助" +
                        "ExtendedError: " + extendedError;
                    break;

                case StorePurchaseStatus.NetworkError:
                    textBlock.Text = "网络错误赞助失败" +
                        "ExtendedError: " + extendedError;
                    break;

                case StorePurchaseStatus.ServerError:
                    textBlock.Text = "服务器错误赞助失败" +
                        "ExtendedError: " + extendedError;
                    break;

                default:
                    textBlock.Text = "未知错误赞助失败 " +
                        "ExtendedError: " + extendedError;
                    break;
            }
        }
        public async void GetAddOnInfo()
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
                // If your app is a desktop app that uses the Desktop Bridge, you
                // may need additional code to configure the StoreContext object.
                // For more info, see https://aka.ms/storecontext-for-desktop.
            }

            // Specify the kinds of add-ons to retrieve.
            string[] productKinds = { "Durable", "Consumable", "UnmanagedConsumable" };
            List<String> filterList = new List<string>(productKinds);

            workingProgressRing.IsActive = true;
            StoreProductQueryResult queryResult = await context.GetAssociatedStoreProductsAsync(filterList);
            workingProgressRing.IsActive = false;

            if (queryResult.ExtendedError != null)
            {
                // The user may be offline or there might be some other server failure.
                textBlock.Text = $"ExtendedError: {queryResult.ExtendedError.Message}";
                return;
            }

            foreach (KeyValuePair<string, StoreProduct> item in queryResult.Products)
            {
                // Access the Store product info for the add-on.
                StoreProduct product = item.Value;
                BMBBT.Content ="赞助我"+ item.Value.Price.FormattedPrice;

                // Use members of the product object to access listing info for the add-on...
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetAddOnInfo();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PurchaseAddOn("9N36176MLKZV");

        }
    }
}
