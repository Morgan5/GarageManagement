using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.FrontOffice.Models
{
    public class ReparationType
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Label { get; set; }

        [Required, Column(TypeName = "decimal(18, 2)")]
        public decimal DefaultCost { get; set; }

        public ICollection<ReparationDetail> ReparationDetails { get; set; }
    }
}