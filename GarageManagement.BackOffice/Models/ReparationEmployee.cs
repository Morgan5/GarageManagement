using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageManagement.BackOffice.Models
{
    public class ReparationEmployee
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Reparation")]
        public long ReparationId { get; set; }
        public Reparation Reparation { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}