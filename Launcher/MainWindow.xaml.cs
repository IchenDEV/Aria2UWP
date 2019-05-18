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
        //声明一个程序类  
        System.Diagnostics.Process Proc;
        public MainWindow()
        {
            InitializeComponent();    
            string strProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if(System.Diagnostics.Process.GetProcessesByName(strProcessName).Length > 1)
            {
                System.Windows.Application.Current.Shutdown(0);
                return;
            }
            foreach (var item in System.Diagnostics.Process.GetProcessesByName("aria2c"))
            {
                try
                {
                    item.Close();
                    item.CloseMainWindow();
                    item.Kill();
                }
                catch (Exception)
                {
                }         
            }
            try
            {
                AddTrayIcon();
                //设置启动程序的信息
                System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();
                //设置外部程序名  
                Info.FileName = "aria2c.exe";
                Info.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                //最小化方式启动
                Info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                Random random = new Random();
                int r=random.Next(6800, 6900);
                Info.Arguments = "--input-file=" + ApplicationData.Current.LocalFolder.Path + "\\aria2.session --save-session=" + ApplicationData.Current.LocalFolder.Path + "\\aria2.session --dir=" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Aria\\Downloads --conf-path=aria2.conf -D";
                Proc = System.Diagnostics.Process.Start(Info);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }
        private static NotifyIcon trayIcon;
        private void AddTrayIcon()
        {
            if (trayIcon != null)
            {
                return;
            }
            trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(System.AppDomain.CurrentDomain.BaseDirectory+"\\favicon.ico"),
                Text = "Aria2UWP"
            };
            trayIcon.Visible = true;
            trayIcon.Click += TrayIcon_Click;

            ContextMenu menu = new System.Windows.Forms.ContextMenu();

            MenuItem closeItem = new MenuItem();
            closeItem.Text = "关闭";
            closeItem.Click += new EventHandler(delegate {
                try
                {
                    RemoveTrayIcon(); Proc.Close();  this.Close();
                }
                catch (Exception)
                {

                    throw;
                }
              
            });
            menu.MenuItems.Add(closeItem);

            trayIcon.ContextMenu = menu;    //设置NotifyIcon的右键弹出菜单
        }

        private  void TrayIcon_Click(object sender, EventArgs e)
        {
            try
            {
                string str = "ariauwp://";
                Uri uri = new Uri(str);
                var success = Windows.System.Launcher.LaunchUriAsync(uri).GetResults();
            }
            catch (Exception)
            {
            }
           
        }

        private void RemoveTrayIcon()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }
    }
}
