using System;
using System.Collections.Generic;
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
using HandyControl.Interactivity;
using MaterialDesignThemes.Wpf;

namespace PlixP.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for QuestionDialog.xaml
    /// </summary>
    public partial class QuestionDialog : UserControl
    {
        public event EventHandler Submited;
        public QuestionDialog(string message)
        {
            InitializeComponent();
            messageTextBlock.Text = message;
        }

        private void SubmitButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ControlCommands.Close.Execute(null,null);
            Submited?.Invoke(this,new EventArgs());
        }
    }
}
