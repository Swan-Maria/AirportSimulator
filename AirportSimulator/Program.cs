using AirportSimulator.Services;

namespace AirportSimulator;

internal static class Program
{
    static void Main()
    {
        var airport = new Airport();
        airport.Run();
    }
}