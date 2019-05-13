using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 2)
            {
               if(e.Args[2] == "Stop")
                {
                    string strProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                    if (System.Diagnostics.Process.GetProcessesByName(strProcessName).Length > 1)
                    {
                        foreach (var item in System.Diagnostics.Process.GetProcessesByName(strProcessName))
                        {
                            if(item.Id!= System.Diagnostics.Process.GetCurrentProcess().Id)
                            {
                                item.Kill();
                            }
                        }
                      
                        Application.Current.Shutdown(0);
                    }
                }
            }
            base.OnStartup(e);
        }

    }
}
