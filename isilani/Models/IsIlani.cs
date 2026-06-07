using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace isilani.Models
{
    public class IsIlani
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Baslik { get; set; }

        [Required, StringLength(100)]
        public string Sirket { get; set; }

        [StringLength(100)]
        public string? Konum { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Maas { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir kategori seçiniz")]
        public int? IsKategoriId { get; set; }
        public IsKategori? IsKategori { get; set; }
    }
}
