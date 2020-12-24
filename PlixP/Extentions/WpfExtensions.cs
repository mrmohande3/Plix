using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MessageBox = HandyControl.Controls.MessageBox;

namespace PlixP.Extentions
{
    public static class WpfExtensions
    {
        public static Brush FromHex(string hexColor)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColor));
        }

        public static void ConfigureExceptionHandling(this Application application, AppDomain appDomain)
        {
            application.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            application.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            appDomain.UnhandledException += AppDomain_UnhandledException;
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private static void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Handled == false)
            {
                MessageBox.Show(e.Exception.Message);
                e.Handled = true;
            }
        }

        private static void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Handled == false)
            {
                MessageBox.Show(e.Exception.Message);
                e.Handled = true;
            }
        }
    }
}
