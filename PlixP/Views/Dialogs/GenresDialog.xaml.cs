using System;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Interactivity;
using PlixP.Repositories.Contracts;
using PlixP.Services.Contracts;
using PlixP.ViewModels;
using Card = MaterialDesignThemes.Wpf.Card;

namespace PlixP.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for GenresDialog
    /// </summary>
    public partial class GenresDialog : UserControl
    {
        public event EventHandler<string> GenreSelected;
        public GenresDialog(IServiceWrapper serviceWrapper,IRepositoryWrapper repositoryWrapper)
        {
            var viewModel = new GenresDialogViewModel(serviceWrapper, repositoryWrapper);
            DataContext = viewModel;
            viewModel.GenreSelected += ((model, genre) =>
            {
                GenreSelected?.Invoke(model,genre);
                ControlCommands.Close.Execute(this,this);
            });


            InitializeComponent();
        }

        private void GenreItem_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is Card card && card.DataContext is GenreItemModel model && DataContext is GenresDialogViewModel viewModel)
                viewModel.GenreSelectCommand.Execute(model);
        }
    }
}
