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
using AutoMapper;

namespace PlixP.Models
{
    public class MovieDto : BaseDto<MovieDto,Movie>
    {
        public int Id { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime CreationDate { get; set; }
        public string FullName
        {
            get;
            set;
        }
        public string FileName { get; set; }
        public string Location { get; set; }
        public string Quality { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
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
        public bool IsDubbed
        {
            get;
            set;
        }

        public bool IsSeen { get; set; }

        public Color SeenStatusColor
        {
            get;
            set;
        }

        public SyncStatus SyncStatus { get; set; }
        public List<Rating> Ratings { get; set; }
        public ObservableCollection<CategoryMovie> Category { get; set; }
        public List<string> CategoryNames { get; set; }
        public override void CustomMappings(IMappingExpression<Movie, MovieDto> mapping)
        {
            base.CustomMappings(mapping);
            mapping.ForMember(m => m.CategoryNames
                , org => org.MapFrom(m => m.Category.Select(c => c.Category.Name).ToList()));
        }
    }
}
