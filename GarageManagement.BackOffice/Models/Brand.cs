using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GarageManagement.BackOffice.Models
{
    public class Brand
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Label { get; set; }

        public ICollection<Model> Models { get; set; }
    }
}
