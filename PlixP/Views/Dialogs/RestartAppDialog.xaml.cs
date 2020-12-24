using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PlixP.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for RestartAppDialog.xaml
    /// </summary>
    public partial class RestartAppDialog : UserControl
    {
        public RestartAppDialog()
        {
            InitializeComponent();
        }

        private void SubmitButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
    }
}
