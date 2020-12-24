using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlixP.Models
{
    [Serializable]
    public class Rating : Entity
    {
        public string Source { get; set; }
        public string Value { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
