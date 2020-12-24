using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PlixP.Services.Contracts;
using PlixP.Views;
using Prism.Mvvm;
using Prism.Regions;

namespace PlixP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private int _count;
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        private string _background;

        public string Background
        {
            get { return _background; }
            set { SetProperty(ref _background, value); }
        }


        public MainWindowViewModel(IServiceWrapper serviceWrapper , IRegionManager regionManager)
        {
             var bg = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Background", string.Empty);
             if (string.IsNullOrEmpty(bg))
                 Background = "https://www.jakpost.travel/wimages/large/166-1664172_lord-of-the-rings-background.jpg";
             else
                 Background = bg;
        }
    }
}
