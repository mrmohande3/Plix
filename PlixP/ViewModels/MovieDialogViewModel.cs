using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Interactivity;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using PlixP.Models;
using PlixP.Repositories.Contracts;
using PlixP.Services;
using PlixP.Services.Contracts;
using PlixP.Views.Dialogs;
using MessageBox = System.Windows.MessageBox;

namespace PlixP.ViewModels
{
    public class MovieDialogViewModel : BindableBase
    {
        private Movie _movie;
        public Movie Movie
        {
            get { return _movie; }
            set { SetProperty(ref _movie, value); }
        }

        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand PlayFileCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
        public ICommand AddCategoryCommand { get; set; }
        public ICommand RemoveCategoryCommand { get; set; }
        private IServiceWrapper ServiceWrapper { get; set; }
        private IRepositoryWrapper RepositoryWrapper { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public List<object> Properties { get; set; } = new List<object>();
        public MovieDialogViewModel()
        {

        }
        public MovieDialogViewModel(IServiceWrapper serviceWrapper, IRepositoryWrapper repositoryWrapper, Movie movie)
        {
            Categories = new ObservableCollection<Category>();
            repositoryWrapper.SetRepository<Category>().TableNoTracking.ToList().ForEach(c => Categories.Add(c));
            Movie = movie;
            ServiceWrapper = serviceWrapper;
            RepositoryWrapper = repositoryWrapper;
            Properties.Add(new
            {
                Name = "Director : ",
                Value = movie.Director
            });
            Properties.Add(new
            {
                Name = "Writer : ",
                Value = movie.Writer
            });
            Properties.Add(new
            {
                Name = "Awarads : ",
                Value = movie.Awards
            });
            Properties.Add(new
            {
                Name = "Actors : ",
                Value = movie.Actors
            });
            Properties.Add(new
            {
                Name = "Country : ",
                Value = movie.Country
            });
            DeleteCommand = new DelegateCommand(() =>
            {
                QuestionDialog dialog = new QuestionDialog("ایا از حذف فیلم مطمئن هستید ؟");
                dialog.Submited += (async (ss, ee) =>
                {
                    await serviceWrapper.MovieServices.DeleteMovie(movie);
                    ControlCommands.CloseAll.Execute(null, null);
                    HandyControl.Controls.MessageBox.Show($"{movie.Title} Has Removed");
                });
                Dialog.Show(dialog);
            });
            PlayFileCommand = new DelegateCommand(() =>
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = @Movie.Location + "\\" + Movie.FullName;
                process.Start();
            });
            OpenFolderCommand = new DelegateCommand(() =>
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = @Movie.Location;
                    process.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
            AddCategoryCommand = new DelegateCommand<Category>(async (category) =>
            {
                if (category.Id != 0)
                {
                    if (Movie.Category.FirstOrDefault(c => c.CategoryId == category.Id) == null)
                    {
                        Movie.Category.Add(new CategoryMovie
                        {
                            MovieId = Movie.Id,
                            Category = category,
                            CategoryId = category.Id
                        });
                        await repositoryWrapper.SetRepository<CategoryMovie>().AddAsync(new CategoryMovie
                        {
                            MovieId = Movie.Id,
                            CategoryId = category.Id
                        },CancellationToken.None);
                    }
                }
                else
                {
                    Movie.Category.Add(new CategoryMovie
                    {
                        MovieId = Movie.Id,
                        Category = category
                    });
                    await repositoryWrapper.SetRepository<CategoryMovie>().AddAsync(new CategoryMovie
                    {
                        MovieId = Movie.Id,
                        Category = category
                    },CancellationToken.None);
                }

            });
            RemoveCategoryCommand = new DelegateCommand<CategoryMovie>(async (cMovie)=>
                {
                    try
                    {
                        await RepositoryWrapper.SetRepository<CategoryMovie>().DeleteAsync(cMovie,CancellationToken.None);
                        Movie.Category.Remove(cMovie);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                });
            SaveCommand = new DelegateCommand<Movie>(async (m) =>
            {
                await ServiceWrapper.MovieServices.EditMovie(m);
            });
        }
    }
}
