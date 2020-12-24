using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PlixP.Models
{
    public enum SyncStatus
    {
        Synced,
        NotSynced
    }
    [Serializable]
    public class Movie : Entity, ICloneable
    {
        public string FullName
        {
            get
            {
                if (Location != null && FileName != null)
                    return Path.Combine(Location, FileName);
                else
                    return null;

            }
        }
        public string FileName { get; set; }
        public string Location { get; set; }
        public string Quality { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }

        public int IntYear
        {
            get
            {
                int year;
                if (string.IsNullOrEmpty(Year))
                    return 0;
                if (int.TryParse(Year, out year))
                    return year;
                else
                    return 0;
            }
        }

        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public string Metascore { get; set; }
        public string imdbRating { get; set; }
        public string imdbVotes { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string DVD { get; set; }
        public string BoxOffice { get; set; }
        public string Production { get; set; }
        public string Website { get; set; }
        public string Response { get; set; }

        [NotMapped]
        public bool IsDubbed
        {
            get
            {
                if (FullName != null && FullName.ToLower().Contains("dubbed"))
                    return true;
                else
                    return false;
            }
        }

        public bool IsSeen { get; set; }

        [NotMapped]
        public Color SeenStatusColor
        {
            get
            {
                if (IsSeen)
                    return Color.LimeGreen;
                else
                    return Color.OrangeRed;
            }
        }

        public SyncStatus SyncStatus { get; set; }
        public List<Rating> Ratings { get; set; }
        public ObservableCollection<CategoryMovie> Category { get; set; }
        [NotMapped]
        public List<string> CategoryNames { get; set; }

        public object Clone()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);
                formatter.Serialize(ms, this);
                ms.Position = 0;
                return formatter.Deserialize(ms);
            }
        }

    }
}
