using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.BackOffice.Models
{
    public class Model
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Label { get; set; }

        [Required]
        [ForeignKey("Brand")]
        public long BrandId { get; set; }

        [Display(Name = "Marque")]
        public Brand Brand { get; set; }
    }
}
