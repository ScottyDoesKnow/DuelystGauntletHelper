using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GauntletHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string ERROR_LOG = "error.log";
        private const long ERROR_LOG_MAX = 1024 * 1024; // 1 MB

        static App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException((Exception)e.ExceptionObject);
        }

        private static void LogException(Exception ex)
        {
            if (File.Exists(ERROR_LOG) && new FileInfo(ERROR_LOG).Length > ERROR_LOG_MAX)
            {
                File.Copy(ERROR_LOG, ERROR_LOG + ".old", true);
                File.Delete(ERROR_LOG);
            }

            using (TextWriter writer = new StreamWriter("error.log", true))
            {
                writer.WriteLine(DateTime.UtcNow);
                writer.WriteLine(ex.ToString());
                writer.WriteLine();
            }
        }
    }
}
