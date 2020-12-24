using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlixP.Models
{
    [Serializable]
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
