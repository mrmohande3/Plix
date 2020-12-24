using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlixP.Models
{
    [Serializable]
    public class CategoryMovie : Entity
    {
        public int CategoryId { get; set; }
        public int MovieId { get; set; }
        public Category Category { get; set; }
        public Movie Movie { get; set; }
        [NotMapped]
        public bool IsEditable { get; set; } = false;

    }
}
