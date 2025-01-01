using System;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.FrontOffice.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Prénom")]
        public string Firstname { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Nom")]
        public string Lastname { get; set; }

        [Required]
        [Display(Name = "Date de naissance")]
        public DateTime Birthdate { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Téléphone")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Display(Name = "Adresse email")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Reparation> Reparations { get; set; }
    }
}
