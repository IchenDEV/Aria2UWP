﻿using System;
using System.Windows;
using Windows.Storage;

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
            //设置启动程序的信息
            System.Diagnostics.ProcessStartInfo Info = new System.Diagnostics.ProcessStartInfo();
            //设置外部程序名  
            Info.FileName = "aria2c.exe";
            Info.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            //最小化方式启动
            Info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            Info.Arguments = "--input-file=" + ApplicationData.Current.LocalFolder.Path+ "\\aria2.session --save-session=" + ApplicationData.Current.LocalFolder.Path+"\\aria2.session --dir=" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +"\\Aria\\Downloads --conf-path=aria2.conf";
            //声明一个程序类  
            System.Diagnostics.Process Proc;
            try
            {
                Proc = System.Diagnostics.Process.Start(Info);
                System.Threading.Thread.Sleep(500);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                MessageBox.Show("cannot find aria2c.exe" + Info.WorkingDirectory);
            }
            Application.Current.Shutdown(0);
        }
    }
}