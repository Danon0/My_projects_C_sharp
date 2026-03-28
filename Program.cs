using System;

namespace CarDescriptionApp
{
    public interface ICar
    {
        string GetDescription();
    }

    public interface IElectricCar : ICar
    {
        double BatteryCapacity { get; }
    }

    public interface IMechanicalCar : ICar
    {
        double EngineVolume { get; }
        string FuelType { get; }
    }

    public interface IAutomaticTransmission
    {
        string TransmissionType { get; }
    }

    public interface IManualTransmission
    {
        string TransmissionType { get; }
    }

    public abstract class ACar : ICar
    {
        public string Brand { get; }
        public int Seats { get; }
        public string MultimediaSystem { get; }

        protected ACar(string brand, int seats, string multimedia)
        {
            Brand = brand;
            Seats = seats;
            MultimediaSystem = multimedia;
        }

        public virtual string GetDescription()
        {
            return $"{Brand} {GetEngineType()} car with {GetTransmissionType()} transmission, {Seats} seats, {MultimediaSystem} on board";
        }

        protected abstract string GetEngineType();
        protected abstract string GetTransmissionType();
    }

    public abstract class AutomaticCar : ACar, IAutomaticTransmission
    {
        protected AutomaticCar(string brand, int seats, string multimedia) : base(brand, seats, multimedia) { }

        public string TransmissionType => "automatic";
        protected override string GetTransmissionType() => TransmissionType;
    }

    public abstract class ManualCar : ACar, IManualTransmission
    {
        protected ManualCar(string brand, int seats, string multimedia) : base(brand, seats, multimedia) { }

        public string TransmissionType => "manual";
        protected override string GetTransmissionType() => TransmissionType;
    }

    public abstract class ElectricCar : AutomaticCar, IElectricCar
    {
        public double BatteryCapacity { get; }

        protected ElectricCar(string brand, int seats, string multimedia, double batteryCapacity)
            : base(brand, seats, multimedia)
        {
            BatteryCapacity = batteryCapacity;
        }

        protected override string GetEngineType() => "electrical";

        public override string GetDescription()
        {
            return base.GetDescription() + $", battery capacity: {BatteryCapacity} kWh";
        }
    }

    public abstract class MechanicalCar : ACar, IMechanicalCar
    {
        public double EngineVolume { get; }
        public string FuelType { get; }

        protected MechanicalCar(string brand, int seats, string multimedia, double engineVolume, string fuelType)
            : base(brand, seats, multimedia)
        {
            EngineVolume = engineVolume;
            FuelType = fuelType;
        }

        protected override string GetEngineType() => $"internal combustion ({FuelType})";

        public override string GetDescription()
        {
            return base.GetDescription() + $", engine: {EngineVolume}L {FuelType}";
        }
    }

    public class Tesla : ElectricCar
    {
        public Tesla() : base("Tesla", 5, "Android Auto", 75.0) { }
    }

    public class BMWX5 : MechanicalCar, IAutomaticTransmission
    {
        public BMWX5() : base("BMW X5", 5, "iDrive", 3.0, "petrol") { }

        protected override string GetTransmissionType() => "automatic";
        string IAutomaticTransmission.TransmissionType => "automatic";
    }

    public class LadaVesta : MechanicalCar, IManualTransmission
    {
        public LadaVesta() : base("Lada Vesta", 5, "standard multimedia", 1.6, "petrol") { }

        protected override string GetTransmissionType() => "manual";
        string IManualTransmission.TransmissionType => "manual";
    }

    public enum CarType
    {
        Tesla,
        BMW_X5,
        Lada_Vesta
    }

    public static class CarFactory
    {
        public static ICar CreateCar(CarType type)
        {
            return type switch
            {
                CarType.Tesla => new Tesla(),
                CarType.BMW_X5 => new BMWX5(),
                CarType.Lada_Vesta => new LadaVesta(),
                _ => throw new ArgumentException("Неизвестный тип")
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Программа по описанию марки машины с параметрами");
            while (true)
            {
                Console.Write("Введите марку автомобиля или done для остановки ввода: ");
                string input = Console.ReadLine();
                if (input.Equals("done", StringComparison.OrdinalIgnoreCase))
                    break;

                if (Enum.TryParse<CarType>(input, true, out CarType carType))
                {
                    try
                    {
                        ICar car = CarFactory.CreateCar(carType);
                        Console.WriteLine(car.GetDescription());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Описание авто");
                }
            }
            Console.WriteLine("Всего доброго!");
        }
    }
}