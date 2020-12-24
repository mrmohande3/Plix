using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using PlixP.Models;
using PlixP.ViewModels;
using PlixP.Views.Template;
using Prism.Regions;
using MessageBox = HandyControl.Controls.MessageBox;
using ScrollViewer = System.Windows.Controls.ScrollViewer;

namespace PlixP.Views
{
    /// <summary>
    /// Interaction logic for MovieList
    /// </summary>
    public partial class MovieList : UserControl
    {
        public ObservableCollection<Movie> Movies = new ObservableCollection<Movie>();
        public MovieList(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("SuggestiontRegion", typeof(SuggestionBox));
        }



        private void MovieItem_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MovieListViewModel)?.MovieSelectedCommand.Execute((sender as MovieItem).DataContext as Movie);
        }


        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (DataContext is MovieListViewModel viewModel && sender is ScrollViewer scrollViewer && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                viewModel.NextPageCommand.Execute(null);
            }
        }

        private void AllToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button && DataContext is MovieListViewModel viewModel)
            {
                if(button.IsChecked.Value==true)
                    viewModel.CheckByAddedCommand.Execute(null);
            }
        }

        private void ByReleaseToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button && DataContext is MovieListViewModel viewModel)
            {
                if (button.IsChecked.Value == true)
                    viewModel.CheckByRealease.Execute(null);
            }
        }

        private void SearchButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchTextBox.Text))
            {
                MessageBox.Show("Please Enter Movie Name");
                return;
            }
            if(DataContext is MovieListViewModel viewModel)
                viewModel.SearchCommand.Execute(searchTextBox.Text);
        }

        private void ByImdbRatingToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button && DataContext is MovieListViewModel viewModel)
            {
                if (button.IsChecked.Value == true)
                    viewModel.CheckByImdbCommand.Execute(null);
            }
        }

        private void GenreSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is CheckComboBox comboBox && comboBox.SelectedItems is IList items && DataContext is MovieListViewModel viewModel)
            {
                var genres = new List<string>();
                for (var i = 0; i < items.Count; i++)
                {
                    if(items[0] is string str)
                        genres.Add(str);
                }
                viewModel.GenreSelectedCommand.Execute(genres);
            }
        }
    }
}
