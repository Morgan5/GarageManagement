using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.BackOffice.Models
{
    public class ReparationDetail
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Reparation")]
        public long ReparationId { get; set; }
        public Reparation Reparation { get; set; }

        [Required]
        [ForeignKey("ReparationType")]
        public long ReparationTypeId { get; set; }
        public ReparationType ReparationType { get; set; }

        [Required]
        public bool IsCustomCost { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CustomCost { get; set; }
    }
}