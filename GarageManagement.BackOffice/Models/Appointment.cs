using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.BackOffice.Models
{
    public class Appointment
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Vehicle")]
        public long VehicleId { get; set; }

        [Required]
        [StringLength(255)]
        public string Motif { get; set; }

        [Display(Name = "Créé le")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Pour le")]
        public DateTime? ExpectedAt { get; set; }

        [Display(Name = "Programmé le")]
        public DateTime? ProgrammedAt { get; set; }

        [Display(Name = "Vehicule")]
        public Vehicle Vehicle { get; set; }
    }
}
