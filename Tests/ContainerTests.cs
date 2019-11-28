using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WispFramework.Patterns;
using WispFramework.Patterns.Containers;

namespace Tests
{
    public class ContainerTests
    {
        public interface IVehicle
        {
            string Name { get; set; }
        }

        public class Boat : IVehicle
        {
            public string Name { get; set; } = "Boat1";
            public Container cont = new Container();
        }

        public class Car : IVehicle
        {
            public string Name { get; set; } = "Car1";
        }

        public static void Run()
        {
            Container ct = new Container();

            ct.Register(new Car());
            Console.WriteLine("Car found: " + ct.Resolve<Car>().Name);

            ct.Register(new Boat());
            Console.WriteLine("Boat found: " + ct.Resolve<Boat>().Name);

            var specialBoat = new Boat() { Name = "Boat2" };
            ct.Register(specialBoat, "Boat2");

            var result = ct.Resolve<Boat>("Boat2");
            result.cont.Register(result);
             
            Console.WriteLine($"Special boat: {result.Name}");

            Console.WriteLine($"Special boat: {result.cont.Resolve<Boat>().Name}");

            var vehicles = ct.ResolveMany<IVehicle>().Select(t => t.Name);
            var vehicleListStr = String.Join((string) ",", (IEnumerable<string>) vehicles);
            Console.WriteLine($"Resolved IVehicles {vehicleListStr}");
            Console.ReadLine();
        }
    }
}
