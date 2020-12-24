using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HandyControl.Controls;
using MaterialDesignThemes.Wpf;
using PlixP.Models;
using PlixP.Repositories.Contracts;
using PlixP.Services.Contracts;
using PlixP.Views.Dialogs;
using MessageBox = HandyControl.Controls.MessageBox;

namespace PlixP.ViewModels
{
    public class MovieListViewModel : BindableBase
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private int pagingCount = 25;
        private int currentPage = 0;
        public bool IsAllActive { get; set; } = true;
        public MovieListModel MovieListModel { get; set; } = new MovieListModel();
        public ICommand RefreshCommand { get; set; }
        public ICommand CheckByRealease { get; set; }
        public ICommand MovieSelectedCommand { get; set; }
        public ICommand RefreshFilesCommand { get; set; }
        public ICommand CheckByAddedCommand { get; set; }
        public ICommand CheckByImdbCommand { get; set; }
        public ICommand GenreSelectedCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public MovieListViewModel(IServiceWrapper serviceWrapper, IRepositoryWrapper repositoryWrapper)
        {
            _serviceWrapper = serviceWrapper;
            _repositoryWrapper = repositoryWrapper;
            Initialize();
            CheckByImdbCommand = new DelegateCommand(async () =>
            {
                ChangeByImdb();
            });
            RefreshCommand = new DelegateCommand(async () =>
            {
                //await _serviceWrapper.MovieServices.SyncMovies();
                await Initialize();
                IsAllActive = true;
            });

            MovieSelectedCommand = new DelegateCommand<Movie>((dataContext) =>
            {
                Dialog.Show(new MovieDialog() { DataContext = new MovieDialogViewModel(serviceWrapper, repositoryWrapper, dataContext) });
            });

            RefreshFilesCommand = new DelegateCommand(() =>
            {
                serviceWrapper.LocalServices.ResetFolder();
            });

            CheckByAddedCommand = new DelegateCommand(async () =>
            {
                await Initialize();
            });

            CheckByRealease = new DelegateCommand(() =>
            {
                ChangeByRelease();
            });

            GenreSelectedCommand = new DelegateCommand(() =>
            {
                var genreDialog = new GenresDialog(_serviceWrapper,_repositoryWrapper);
                genreDialog.GenreSelected += (async (ss, genre) =>
                {
                    await ChangeGenre(genre);
                });
                Dialog.Show(genreDialog);
            });
            SearchCommand = new DelegateCommand<string>(async (search) =>
            {
                try
                {
                    MovieListModel.UnPagedMovies.Clear();
                    foreach (var originalMovie in MovieListModel.OriginalMovies)
                    {
                        if (originalMovie.Title.ToLower().Contains(search.ToLower()))
                            MovieListModel.UnPagedMovies.Add(originalMovie);
                    }
                    MovieListModel.PageMovies.Clear();
                    currentPage = 0;
                    Pagination();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    await Initialize();
                }
            });
            NextPageCommand = new DelegateCommand(async () =>
            {
                currentPage++;
                Pagination();
            });

        }

        private void ChangeByImdb()
        {
            MovieListModel.UnPagedMovies.Clear();
            MovieListModel.OriginalMovies.OrderByDescending(m =>
                {
                    float rate;
                    if (float.TryParse(m.imdbRating, out rate))
                        return rate;
                    return 0;
                }).ToList()
                .ForEach(m => MovieListModel.UnPagedMovies.Add(m));
            MovieListModel.PageMovies.Clear();
            currentPage = 0;
            Pagination();
        }

        private async Task ChangeGenre(List<string> genres)
        {
            MovieListModel.UnPagedMovies.Clear();
            foreach (var genre in genres)
            {
                var moviesGenre = await _serviceWrapper.MovieServices.SelectByGenre(genre);
                moviesGenre.ForEach(m => MovieListModel.UnPagedMovies.Add(m));
            }
            MovieListModel.PageMovies.Clear();
            currentPage = 0;
        }
        private async Task ChangeGenre(string genre)
        {
            MovieListModel.UnPagedMovies.Clear();
            var moviesGenre = await _serviceWrapper.MovieServices.SelectByGenre(genre);
            moviesGenre.ForEach(m => MovieListModel.UnPagedMovies.Add(m));
            MovieListModel.PageMovies.Clear();
            currentPage = 0;
        }

        private async Task ChangeByRelease()
        {
            MovieListModel.UnPagedMovies.Clear();
            MovieListModel.OriginalMovies.OrderByDescending(m => m.IntYear).ToList()
                .ForEach(m => MovieListModel.UnPagedMovies.Add(m));
            MovieListModel.PageMovies.Clear();
            currentPage = 0;
            Pagination();
        }

        private async Task Initialize()
        {
            MovieListModel.PageMovies.Clear();
            MovieListModel.UnPagedMovies.Clear();
            MovieListModel.OriginalMovies.Clear();
            (await _serviceWrapper.MovieServices.GetMovies())
                .OrderByDescending(m => m.CreationDate)
                .ToList()
                .ForEach(m =>
                {
                    MovieListModel.OriginalMovies.Add(m);
                });

            MovieListModel.OriginalMovies.ToList()
                .ForEach(m => MovieListModel.UnPagedMovies.Add(m));
            MovieListModel.OriginalCategories = new ObservableCollection<Category>(_repositoryWrapper
                .SetRepository<Category>().TableNoTracking);
            MovieListModel.MoviesCount = MovieListModel.UnPagedMovies.Count;
            MovieListModel.OriginalGenres = await _serviceWrapper.MovieServices.GetGenres();
            currentPage = 0;
            Pagination();
        }

        public void Pagination()
        {
            var selected = MovieListModel.UnPagedMovies.Skip((currentPage * pagingCount)).Take(pagingCount).ToList();
            foreach (var movie in selected)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MovieListModel.PageMovies.Add(movie);
                });
            }
        }

    }
}
