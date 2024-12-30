using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.BackOffice.Models
{
    public class Facturation
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Reparation")]
        public long ReparationId { get; set; }
        public Reparation Reparation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalCost { get; set; }

        public ICollection<FacturationDetail> FacturationDetails { get; set; }
    }
}