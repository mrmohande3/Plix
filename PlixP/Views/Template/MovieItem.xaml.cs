using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using PlixP.Models;

namespace PlixP.Views.Template
{
    /// <summary>
    /// Interaction logic for MovieItem
    /// </summary>
    public partial class MovieItem : UserControl
    {
        public MovieItem()
        {
            InitializeComponent();

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DataContextProperty)
            {
                if (DataContext is Movie)
                {
                    if ((DataContext as Movie).IsDubbed)
                        dubbedCard.Visibility = Visibility.Visible;
                    else
                        dubbedCard.Visibility = Visibility.Hidden;
                }
            }
            base.OnPropertyChanged(e);
        }

        private void Item_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var card = sender as Card;
            var grid = card.Content as Grid;
            var mGrid = grid.Children[1] as Grid;
            mGrid.Visibility = Visibility.Visible;
        }

        private void Item_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var card = sender as Card;
            var grid = card.Content as Grid;
            var mGrid = grid.Children[1] as Grid;
            mGrid.Visibility = Visibility.Hidden;
        }
    }
}
