using System;
using System.Windows;
using System.Windows.Forms;
using Windows.Storage;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace Launcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KillAria2c();
            KillOtherLauncher();
            LaunchAria();
        }

        private void LaunchAria()
        {
            try
            {
                //设置启动程序的信息
                System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();
                //设置外部程序名  
                Info.FileName = "aria2c.exe";
                Info.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                //最小化方式启动
                Info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Info.Arguments = "--log=" + ApplicationData.Current.LocalFolder.Path+"\\aria2.log  --input-file=" + ApplicationData.Current.LocalFolder.Path + "\\aria2.session --save-session=" + ApplicationData.Current.LocalFolder.Path + "\\aria2.session --dir=" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Aria\\Downloads --conf-path=aria2.conf -D";
                var Proc = System.Diagnostics.Process.Start(Info);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private static void KillOtherLauncher()
        {
            string strProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if (System.Diagnostics.Process.GetProcessesByName(strProcessName).Length > 1)
            {
                foreach (var item in System.Diagnostics.Process.GetProcessesByName(strProcessName))
                {
                    if (item.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
                    {
                        try
                        {
                            item.Kill();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        private static void KillAria2c()
        {
            foreach (var item in System.Diagnostics.Process.GetProcessesByName("aria2c"))
            {
                try
                {
                    item.Kill();
                }
                catch (Exception)
                {
                }
            }
        }

    }
}
