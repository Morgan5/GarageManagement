using System;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.BackOffice.Models
{
    public class Employee
    {
        [Key]
        public long Id { get; set; }

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

        public ICollection<ReparationEmployee> ReparationEmployees { get; set; }
    }
}
