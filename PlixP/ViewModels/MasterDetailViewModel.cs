using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using PlixP.Models;
using PlixP.Repositories.Contracts;
using PlixP.Services.Contracts;
using PlixP.Views;
using PlixP.Views.Dialogs;

namespace PlixP.ViewModels
{
    public class MasterDetailViewModel : BindableBase
    {
        public ObservableCollection<string> Paths { get; set; } = new ObservableCollection<string>();
        public DelegateCommand<string> PathCommand { get; set; }
        public DelegateCommand<string> AddCategoryCommand { get; set; }
        public DelegateCommand ResetFolderCommand { get; set; }
        public DelegateCommand ChangeBackGroundCommand { get; set; }
        public DelegateCommand<string> RemovePathCommand { get; set; }
        public DelegateCommand ResetDbCommand { get; set; }
        public MasterDetailViewModel(IRepositoryWrapper repositoryWrapper, IServiceWrapper serviceWrapper)
        {
            var pathsJson = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Paths", string.Empty);
            if (!string.IsNullOrEmpty(pathsJson))
            {
                var paths = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(pathsJson);
                if (paths != null)
                    paths.ForEach(p => Paths.Add(p));
            }
            PathCommand = new DelegateCommand<string>((str) =>
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Paths.Add(str);
                    var pathJson = Newtonsoft.Json.JsonConvert.SerializeObject(Paths.ToList());
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Paths", pathJson);
                    MessageBox.Show("Seccuss");
                    //serviceWrapper.LocalServices.ResetFolder();
                }
            });
            ResetDbCommand = new DelegateCommand(() =>
            {
                QuestionDialog dialog = new QuestionDialog("ایا از ریست کردن دیتابیس مطمئن هستید ؟");
                dialog.Submited += (async (ss, ee) =>
                {
                    var context = new PlixContext();
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                });
                DialogHost.OpenDialogCommand.Execute(dialog,null);
            });
            RemovePathCommand = new DelegateCommand<string>(str =>
            {
                Paths.Remove(str);
                var pathJson = Newtonsoft.Json.JsonConvert.SerializeObject(Paths.ToList());
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Paths", pathJson);
                MessageBox.Show("Seccuss");
            });
            AddCategoryCommand = new DelegateCommand<string>(async(cat) =>
            {
                try
                {
                    await repositoryWrapper.SetRepository<Category>().AddAsync(new Category
                    {
                        Name = cat
                    },CancellationToken.None);
                    MessageBox.Show("Seccuss");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
            ResetFolderCommand = new DelegateCommand(() =>
            {
                serviceWrapper.LocalServices.ResetFolder();
            });
            ChangeBackGroundCommand = new DelegateCommand( ()=>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF*.PNG)|*.BMP;*.JPG;*.GIF*.PNG|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    var str = openFileDialog.FileName;
                    if (!string.IsNullOrEmpty(str))
                    {
                        Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Background", str);
                        DialogHost.OpenDialogCommand.Execute(new RestartAppDialog(),null);
                    }
                }
            });
        }
    }
}
