using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using PlixP.Models;
using PlixP.ViewModels;

namespace PlixP.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for MovieDialog
    /// </summary>
    public partial class MovieDialog : UserControl
    {
        public Movie Movie
        {
            get
            {
                return (DataContext as MovieDialogViewModel).Movie;
            }
        }

        public MovieDialog()
        {
            InitializeComponent();
            seenToggle.Checked += SeenToggle_OnChecked;
            dubbedToggle.Checked += DubbedToggle_OnChecked;
        }
        public MovieDialog(Movie dataContext)
        {
            InitializeComponent();
        }


        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Content = new PackIcon
            {
                Kind = PackIconKind.Check,
                Width = 20
            };
            categoryItems.IsEnabled = true;
            button.Click += AcceptEdit_Click;
            titleTextBox.IsReadOnly = false;
            dubbedToggle.IsEnabled = true;
            seenToggle.IsEnabled = true;
            addCategoryButton.IsEnabled = true;
        }

        private void AcceptEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Movie newMovie = (DataContext as MovieDialogViewModel)?.Movie.Clone() as Movie;
                if (Movie.Title != null && !Movie.Title.Equals(titleTextBox.Text))
                {
                    newMovie.Title = titleTextBox.Text;
                }
                if (Movie.IsDubbed != dubbedToggle.IsChecked)
                {
                    if (dubbedToggle.IsChecked == true)
                    {
                        var newName = Movie.FileName.Split('.');
                        string nn = "";
                        for (int i = 0; i < newName.Length; i++)
                        {
                            if (i == newName.Length - 1)
                            {
                                nn += "Dubbed" + ".";
                            }

                            nn += newName[i] + ".";
                        }
                        newMovie.FileName = nn;
                    }
                    else
                    {
                        var newName = Movie.FileName.Replace("Dubbed.", "");
                        newMovie.FileName = newName;
                    }
                }
                if (Movie.IsSeen != seenToggle.IsChecked)
                {
                    newMovie.IsSeen = seenToggle.IsChecked ?? false;
                }

                (DataContext as MovieDialogViewModel)?.SaveCommand.Execute(newMovie);
            }

            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void DubbedToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            var toggel = sender as CheckBox;
            if (!categoryItems.IsEnabled)
                toggel.IsChecked = !toggel.IsChecked;
        }

        private void SeenToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            var toggel = sender as CheckBox;
            if (!categoryItems.IsEnabled)
                toggel.IsChecked = !toggel.IsChecked;
        }

        private void AddCategoryButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!AddCategoryPopUp.IsPopupOpen)
                AddCategoryPopUp.IsPopupOpen = true;
            else
                AddCategoryPopUp.IsPopupOpen = false;
        }

        private void CategoryNameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            categoryCombo.IsEnabled = true;
        }

        private void AddCategoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (categoryCombo.SelectedValue != null)
            {
                var category = categoryCombo.SelectedValue as Category;
                (DataContext as MovieDialogViewModel)?.AddCategoryCommand.Execute(category);
            }
        }

        private void RemoveCategoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            (DataContext as MovieDialogViewModel)?.RemoveCategoryCommand.Execute((sender as Button).DataContext);
        }
    }
}
