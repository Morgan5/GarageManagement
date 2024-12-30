using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.BackOffice.Models
{
    public class Reparation
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Vehicle")]
        public long VehicleId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        public long Status { get; set; }

        [Display(Name = "Créée le")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Date prévue de fin")]
        public DateTime? FinishedAt { get; set; }

        [Display(Name = "Date de début")]
        public DateTime? StartAt { get; set; }
        
        [Display(Name = "Date de fin réelle")]
        public DateTime? EndAt { get; set; }

        [Display(Name = "Véhicule")]
        public Vehicle Vehicle { get; set; }

        public ICollection<ReparationDetail> ReparationDetails { get; set; }

        public ICollection<ReparationEmployee> ReparationEmployees { get; set; }
    }
}
