using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using PlixP.Views.Dialogs;
using Prism.Regions;
using Window = HandyControl.Controls.Window;

namespace PlixP.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar snackbar = new Snackbar();
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            DialogHost.Show(new LoadingDialog());
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(MovieList));
            mainGrid.Children.Add(snackbar);
        }

    }
}
