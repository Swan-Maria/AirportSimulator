# Airport Simulator Project

## Structure of project

### 1.  `Models/Passenger.cs`

This file contains class that has next properties:

* `Name`;
* `FlightNumber`;
* `HasTicket`;
* `PassedSecurity`;
* `IsOnBoard`.

### 2.  `Models/Flight.cs`

This file contains enum FlightStatus and class Flight that has next properties:

* `FlightNumber`;
* `Destination`;
* `DepartureTime`;
* `Status`;
* `Capacity`;
* `PassengersOnBoard`.

Also class Flight has override method ToString() from System.Object.

### 3.  `Services/Airport.cs`

This file contains class Airport that implements the simulation logic.
This class has next fields:

* `List<Flight> _flights`;
* `List<Passenger> _passengers`;
* `Queue<Passenger> _checkInQueue`;
* `Queue<Passenger> _securityQueue`;
* `Random _rnd`;
* `int _currentTick`;
* `const int CheckInCounters`;
* `const int SecurityGates`;
* `const int BoardingRate`;
* `const double ChanceNewPassenger`;
* `const double ChanceDelayedStatus`;
* `string[] _passengerFirstNames`;
* `string[] _passengerLastNames`.

And next methods:

* `AddFlight(string flightNumber, string destination, int departureTime, int capacity)` - in current project\`s versin it\`s `private`, but if you want to add flights outside class Airport - in Main() method, you can change it\`s to `public`.
* `Run()` - a method that starts the simulation.
* `GeneratePassengers()` - a method for generating a passenger with a given probability (`ChanceNewPassenger`).It also displays information about the arrival of a new passenger.
* `ProcessCheckIn()` - a method simulating passenger check-in with a given number of check-in counters  (`CheckInCounters`). It also displays information about the passenger's check-in status.
* `ProcessSecurity()` - a method simulating passenger safety control with a given number of control points (`SecurityGates`). It also displays information about the passenger's passage through the checkpoint.
* `UpdateFlights()` - a method that updates the flight status (OnTime, Delay, Boarding, Departed). It also displays announcements about changes in flight status, passenger boarding (boarded, didn't make it, no seats left)
* `PrintStatus()` - a method that displays information about the status of flights and the number of passengers who are in line or waiting to board their flight.

### 4.  `Program.cs`

This file contains class Program with one\`s method Main.