namespace AirportSimulator.Models;

public enum FlightStatus
{
    OnTime,
    Delayed,
    Boarding,
    Departed
}

public class Flight(string flightNumber, string destination, int departureTime, int capacity)
{
    public string FlightNumber { get; } = flightNumber;
    private string Destination { get; } = destination;
    public int DepartureTime { get; set; } = departureTime;
    public FlightStatus Status { get; set; } = FlightStatus.OnTime;
    public int Capacity { get; } = capacity;
    public List<Passenger> PassengersOnBoard { get; } = new();

    public override string ToString()
    {
        return $"{FlightNumber} to {Destination} - {Status} (Departs at {DepartureTime})";
    }
}