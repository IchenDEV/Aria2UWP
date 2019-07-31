using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Launch2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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
                Info.CreateNoWindow = true;
                string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AKDownload";
                string DocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AKDownload\\Configs";

                if (!File.Exists($"{DocPath}\\aria2.log"))
                {
                    File.Create($"{DocPath}\\aria2.log");
                }
                if (!File.Exists($"{DocPath}\\aria2.session"))
                {
                    File.Create($"{DocPath}\\aria2.session");
                }
                Info.Arguments = $"--log= {DocPath}\\aria2.log  " +
                    $"--input-file={DocPath}\\aria2.session " +
                    $"--save-session={DocPath} aria2.session " +
                    $"--dir={downloadPath} " +
                    $"--conf-path=aria2.conf -D ";
                var Proc = System.Diagnostics.Process.Start(Info);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.StackTrace);
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
