using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlixP.Models
{
    [Serializable]
    public class Category : Entity
    {
        public string Name { get; set; }
        public virtual ICollection<CategoryMovie> Movies { get; set; }
    }
}
