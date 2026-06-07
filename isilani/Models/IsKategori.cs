using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace isilani.Models
{
    public class IsKategori
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Ad { get; set; }

        public ICollection<IsIlani> IsIlanlari { get; set; }
    }
}
