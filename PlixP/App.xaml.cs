using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prism.Ioc;
using PlixP.Views;
using System.Windows;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlixP.Extentions;
using PlixP.Models;
using PlixP.Repositories;
using PlixP.Repositories.Contracts;
using PlixP.Services;
using PlixP.Services.Contracts;

namespace PlixP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            var _catcherService = new CatcherService();
            _catcherService.Delete<List<Movie>>(CatcherKeys.MovieList);
            using (var _context = new PlixContext())
            {
                _context.Database.Migrate();
                if (_context.Categories.Count() == 0)
                {
                    _context.Categories.Add(new Category
                    {
                        Name = "Uncategorized"
                    });
                    _context.SaveChanges();
                }
            }

            AutoMapperConfig.ConfigurationMapper();
            App.Current.ConfigureExceptionHandling(AppDomain.CurrentDomain);
            MovieServiceRealTime.Instance.StartReal();
            return Container.Resolve<MainWindow>();
        }
       

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MovieList>();
            containerRegistry.Register<IServiceWrapper,ServiceWrapper>();
            containerRegistry.Register<IRepositoryWrapper, RepositoryWrapper>();
        }
    }
}
