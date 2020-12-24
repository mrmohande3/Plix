using System.Windows;
using System.Windows.Controls;
using PlixP.ViewModels;

namespace PlixP.Views
{
    /// <summary>
    /// Interaction logic for MasterDetail
    /// </summary>
    public partial class MasterDetail : UserControl
    {
        public MasterDetail()
        {
            InitializeComponent();
        }

        private void Path_OnClick(object sender, RoutedEventArgs e)
        {
            (DataContext as MasterDetailViewModel)?.PathCommand.Execute(addPathTextBox.Text);
        }

        private void Cat_OnClick(object sender, RoutedEventArgs e)
        {
            (DataContext as MasterDetailViewModel)?.AddCategoryCommand.Execute(addCategoryTextBox.Text);
        }


        private void RemovePathButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.DataContext is string str && DataContext is MasterDetailViewModel viewModel)
                viewModel.RemovePathCommand.Execute(str);
        }
    }
}
