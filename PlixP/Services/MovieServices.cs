using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper.QueryableExtensions;
using HandyControl.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using PlixP.Models;
using PlixP.Repositories;
using PlixP.ViewModels;
using PlixP.Views.Dialogs;
using MessageBox = HandyControl.Controls.MessageBox;
using Timer = System.Timers.Timer;

namespace PlixP.Services
{
    public class MovieServiceRealTime
    {
        private static MovieServiceRealTime _instance;
        public static MovieServiceRealTime Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MovieServiceRealTime();
                return _instance;
            }
        }

        private LocalServices _localServices;
        private MovieServices _movieServices;
        private bool IsWorking = false;
        private List<FileInfo> _currentFiles = new List<FileInfo>();
        private Timer timer;
        public List<Movie> CurreMovies
        {
            get { return _movieServices.GetMovies().Result; }
        }

        public MovieServiceRealTime()
        {
            timer = new Timer(6000);
            _localServices = new LocalServices();
            _movieServices = new MovieServices();
            timer.Elapsed += Timer_Elapsed;
        }

        public void StartReal() => timer.Start();

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckAll();
        }

        private async Task CheckAll()
        {
            try
            {
                if (IsWorking)
                    return;
                IsWorking = true;
                _localServices.ResetFolder();
                var movieFolders = _localServices.GetMovieFiles();
                foreach (var f in movieFolders)
                {
                    if (!CurreMovies.Any(m => m.Location == f.DirectoryName))
                    {
                        await _movieServices.AddNewMovieToDb(f);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Growl.SuccessGlobal("Movie Added : " + f.Name);
                        });
                    }
                }

                foreach (var m in CurreMovies.Where(m => m.SyncStatus == SyncStatus.NotSynced))
                {
                    var creationDate = m.CreationDate;
                    var movie = await _movieServices.GetMovieAsync(m.Title);
                    if (movie != null)
                    {
                        movie.CreationDate = creationDate;
                        movie.Id = m.Id;
                        movie.Location = m.Location;
                        movie.FileName = m.FileName;
                        movie.SyncStatus = SyncStatus.Synced;
                        await _movieServices.EditMovie(movie, SyncStatus.Synced);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Growl.WarningGlobal("Movie Synced : " + movie.FullName);
                        });
                    }
                }



                IsWorking = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
    public class MovieServices
    {
        private readonly RepositoryWrapper _repositoryWrapper;
        private readonly LocalServices _localServices;
        private readonly CatcherService _catcherService;

        public MovieServices()
        {
            _repositoryWrapper = new RepositoryWrapper(new PlixContext());
            _localServices = new LocalServices();
            _catcherService = new CatcherService();
        }


        public async Task<List<Movie>> GetMovies()
        {
            try
            {
                List<Movie> Movies = new List<Movie>();
                Movies = _catcherService.Get<List<Movie>>(CatcherKeys.MovieList);
                if (Movies != null)
                    if (Movies.Count != 0)
                        return Movies;
                var dtos = _repositoryWrapper.SetRepository<Movie>()
                    .TableNoTracking
                    .ProjectTo<MovieDto>()
                    .ToList();
                dtos.ForEach(m => Movies.Add(m.ToEntity()));
                foreach (var movie in Movies)
                {
                    foreach (var categoryMovie in movie.Category)
                    {
                        categoryMovie.Category = await _repositoryWrapper.SetRepository<Category>()
                           .GetByIdAsync(CancellationToken.None, categoryMovie.CategoryId);
                    }
                }
                _catcherService.Set(Movies, CatcherKeys.MovieList);
                return Movies;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Movie> AddNewMovieToDb(FileInfo f)
        {
            var cMovie = new Movie();
            var mName = f.Directory.Name.Split("(")[0];
            cMovie = await GetMovieAsync(mName);
            string q = " ";
            if (f.Name.Contains("720"))
                q = "720P";
            else if (f.Name.Contains("1080"))
                q = "1080P";
            if (cMovie != null)
            {
                cMovie.FileName = f.Name;
                cMovie.Location = f.DirectoryName;
                cMovie.Quality = q;
                cMovie.SyncStatus = SyncStatus.Synced;
                cMovie.CreationDate = DateTime.Now;
                await _repositoryWrapper.SetRepository<Movie>().AddAsync(cMovie, CancellationToken.None);
                await _repositoryWrapper.SetRepository<CategoryMovie>().AddAsync(new CategoryMovie
                {
                    MovieId = cMovie.Id,
                    CategoryId = 1
                }, CancellationToken.None);

            }
            else
            {
                cMovie = new Movie();
                cMovie.Location = f.DirectoryName;
                cMovie.FileName = f.Name;
                cMovie.Quality = q;
                cMovie.Title = mName;
                cMovie.CreationDate = DateTime.Now;
                cMovie.SyncStatus = SyncStatus.NotSynced;


                await _repositoryWrapper.SetRepository<Movie>().AddAsync(cMovie, CancellationToken.None);
                await _repositoryWrapper.SetRepository<CategoryMovie>().AddAsync(new CategoryMovie
                {
                    MovieId = cMovie.Id,
                    CategoryId = 1
                }, CancellationToken.None);
            }
            _catcherService.Delete<List<Movie>>(CatcherKeys.MovieList);
            return cMovie;
        }
        public async Task<Movie> GetMovieAsync(string movieName)
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest("http://omdbapi.com", Method.GET);
            request.AddQueryParameter("t", movieName);
            request.AddQueryParameter("apikey", "a915dcc9");
            var res = await client.ExecuteGetAsync<Movie>(request);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                if (res.Data.Response != "False")
                    return res.Data;
                else
                {
                    /*Application.Current.Dispatcher.Invoke(() =>
                    {
                        Growl.ErrorGlobal($"Cant Sync Movie {movieName} , Please Edit Movie Detail");
                    });*/
                    return null;
                }
            }
            else
            {
                /*Application.Current.Dispatcher.Invoke(() =>
                {
                    Growl.ErrorGlobal($"Cant Sync Movie {movieName} , Please Edit Movie Detail");
                });*/
                return null;
            }
        }
        public async Task EditMovie(Movie newMovie, SyncStatus syncStatus = SyncStatus.NotSynced)
        {
            try
            {
                var oldMovie = await _repositoryWrapper.SetRepository<Movie>()
                    .GetByIdAsync(CancellationToken.None, newMovie.Id);
                string newMovieFileName = string.Empty;

                foreach (var str in newMovie.Title.Split(" "))
                {
                    if (string.IsNullOrWhiteSpace(str))
                        continue;
                    newMovieFileName += str + ".";
                }
                if (!string.IsNullOrWhiteSpace(newMovie.Year))
                    newMovieFileName += newMovie.Year + ".";
                else
                {
                    foreach (var str in oldMovie.FileName.Split("."))
                    {

                        int year = 0;
                        if (int.TryParse(str, out year) && (year >= 1920 && year <= 2142))
                        {
                            newMovieFileName += year + ".";
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(newMovie.Quality))
                    newMovieFileName += newMovie.Quality + ".";
                else
                {
                    bool qulited = false;
                    foreach (var str in oldMovie.FileName.Split("."))
                    {
                        if (str.Contains("720") || str.Contains("1080") || str.ToLower().Contains("dvdsrc") || str.ToLower().Contains("hdrip") || str.ToLower().Contains("dvdrip"))
                        {
                            newMovieFileName += str + ".";
                            newMovie.Quality = str;
                            qulited = true;
                        }
                    }
                    if (!qulited)
                    {
                        newMovieFileName += "720" + ".";
                        newMovie.Quality = "720";
                    }
                }

                newMovieFileName += oldMovie.FileName.Split(".").Last();
                newMovie.FileName = newMovieFileName;
                newMovie.FileName = newMovie.FileName.Replace(":", "");
                newMovie.FileName = newMovie.FileName.Replace(@"\", "");
                newMovie.FileName = newMovie.FileName.Replace("?", "");
                newMovie.FileName = newMovie.FileName.Replace("*", "");
                newMovie.FileName = newMovie.FileName.Replace("/", "");
                newMovie.FileName = newMovie.FileName.Replace("<", "");
                newMovie.FileName = newMovie.FileName.Replace(">", "");
                newMovie.FileName = newMovie.FileName.Replace("|", "");

                var folderName = _localServices.GetFolderNameByMovieFileName(newMovie.FileName);
                var location = Path.Combine(newMovie.Location.Split("\\").Take(newMovie.Location.Split("\\").Length - 1)
                    .ToArray());
                newMovie.Location = $"{Path.Combine(location, folderName)}";
                newMovie.SyncStatus = syncStatus;
                newMovie.Category = null;
                await _repositoryWrapper.SetRepository<Movie>().UpdateAsync(newMovie, CancellationToken.None);
                if (oldMovie.Location != newMovie.Location)
                    _localServices.RenameDirection(new DirectoryInfo(oldMovie.Location), new DirectoryInfo(newMovie.Location));
                oldMovie.Location = Path.Combine(location, folderName);
                _localServices.RenameFile(new FileInfo(oldMovie.FullName), new FileInfo(newMovie.FullName));
                _catcherService.Delete<List<Movie>>(CatcherKeys.MovieList);
                MessageBox.Show($"Movie {newMovie.Title} Was Edited !");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public async Task<Dictionary<string, int>> GetGenres()
        {
            List<Movie> Movies = await GetMovies();
            var originalGenres = Movies.Select(m => m.Genre).ToList();
            Dictionary<string, int> genres = new Dictionary<string, int>();
            foreach (var originalGenre in originalGenres)
            {
                if (string.IsNullOrEmpty(originalGenre))
                    continue;
                var split = originalGenre.Split(',');
                foreach (var str in split)
                {
                    var trim = str.Trim();
                    if (genres.ContainsKey(trim))
                        genres[trim]++;
                    else
                        genres.Add(trim, 1);
                }

            }
            return genres;
        }
        public async Task<List<GenreItemModel>> GetGenresModel()
        {
            List<Movie> Movies = await GetMovies();
            var originalGenres = Movies.Select(m => m.Genre).ToList();
            List<GenreItemModel> genres = new List<GenreItemModel>();
            foreach (var originalGenre in originalGenres)
            {
                if (string.IsNullOrEmpty(originalGenre))
                    continue;
                var split = originalGenre.Split(',');
                foreach (var str in split)
                {
                    var trim = str.Trim();
                    var genre = genres.FirstOrDefault(g=>g.Name==trim);
                    if (genre!=null)
                        genre.MovieCount++;
                    else
                    {
                        var item = new GenreItemModel
                        {
                            MovieCount = 1,
                            Name = trim
                        };
                        foreach (var movie in Movies.Where(m=>m.Genre!=null && m.Genre.Contains(trim)))
                        {
                            var poster = movie.Poster;
                            if (!genres.Any(g => g.Image == poster))
                                item.Image = poster;
                        }

                        if (item.Image == null)
                            item.Image = Movies.FirstOrDefault(m => m.Genre != null && m.Genre.Contains(trim))?.Poster;
                        genres = genres.OrderByDescending(g => g.MovieCount).ToList();
                        genres.Add(item);
                    }
                }


            }
            return genres;
        }
        public async Task<List<Movie>> SelectByGenre(string genre)
        {
            return (await GetMovies()).Where(m => m.Genre != null && m.Genre.Contains(genre))
                         .ToList();
        }
        public async Task DeleteMovie(Movie movie)
        {
            movie.Category = null;
            await _repositoryWrapper.SetRepository<Movie>()
                .DeleteAsync(movie, CancellationToken.None);
            _catcherService.Delete<List<Movie>>(CatcherKeys.MovieList);
            MovieServiceRealTime.Instance.CurreMovies.RemoveAll(m => m.Id == movie.Id);
        }
    }
}
