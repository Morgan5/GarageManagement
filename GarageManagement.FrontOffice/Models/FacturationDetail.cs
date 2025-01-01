using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.FrontOffice.Models
{
    public class FacturationDetail
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Facturation")]
        public long FacturationId { get; set; }
        public Facturation Facturation { get; set; }

        public string ReparationTypeLabel { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Cost { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}