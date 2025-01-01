using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarageManagement.FrontOffice.Models
{
    public class VehicleViewModel
    {
        public IEnumerable<Vehicle> Vehicles { get; set; }
        public IEnumerable<Model> Models { get; set; }
        public Vehicle NewVehicle { get; set; }
    }
}