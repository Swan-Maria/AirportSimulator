namespace AirportSimulator.Models;

public class Passenger(string name, string flightNumber)
{
    public string Name { get; set; } = name;
    public string FlightNumber { get; set; } = flightNumber;
    public bool HasTicket { get; set; } = false;
    public bool PassedSecurity { get; set; } = false;
    public bool IsOnBoard { get; set; } = false;
}