using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlixP.Models
{
    public class MovieListModel : INotifyPropertyChanged
    {
        public ObservableCollection<Movie> OriginalMovies { get; set; } = new ObservableCollection<Movie>();
        public ObservableCollection<Category> OriginalCategories { get; set; } = new ObservableCollection<Category>();
        public Dictionary<string, int> OriginalGenres { get; set; } = new Dictionary<string, int>();

        public List<string> Genres
        {
            get
            {
                return OriginalGenres.Select(g => string.Format("{0} | {1}", g.Key, g.Value.ToString())).ToList();
            }
        }

        public ObservableCollection<Movie> UnPagedMovies { get; set; } = new ObservableCollection<Movie>();
        public ObservableCollection<Movie> PageMovies { get; set; } = new ObservableCollection<Movie>();

        public int MoviesCount { get; set; }
        public bool AllChecked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
