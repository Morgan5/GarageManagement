using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.BackOffice.Models
{
    public class Vehicle
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Immatriculation { get; set; }

        [Required]
        [ForeignKey("Model")]
        public long ModelId { get; set; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Display(Name = "Modèle")]
        public Model Model { get; set; }

        [Display(Name = "Client")]
        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public ICollection<Reparation> Reparations { get; set; }
    }
}
