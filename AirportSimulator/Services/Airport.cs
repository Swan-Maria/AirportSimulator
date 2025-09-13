using AirportSimulator.Models;

namespace AirportSimulator.Services;

public class Airport
{
    private readonly List<Flight> _flights = new();
    private readonly List<Passenger> _passengers = new();
    private readonly Queue<Passenger> _checkInQueue = new();
    private readonly Queue<Passenger> _securityQueue = new();

    private readonly Random _rnd = new();
    private int _currentTick = 1;

    private const int CheckInCounters = 3;
    private const int SecurityGates = 2;
    private const int BoardingRate = 1;
    private const double ChanceNewPassenger = 1;
    private const double ChanceDelayedStatus = 0.5;

    private readonly string[] _passengerFirstNames =
        ["Joey", "Monica", "Chandler", "Rachel", "Ross", "Phoebe", "Jennifer", "Matthew", "Lisa", "David"];

    private readonly string[] _passengerLastNames =
        ["Tribbiani", "Geller", "Bing", "Green", "Schwimmer", "Buffay", "Cox", "Perry", "Kudrow", "LeBlanc"];

    //Можно сделать public и добавлять рейсы в главном методе программы перед запуском симуляции
    private void AddFlight(string flightNumber, string destination, int departureTime, int capacity)
    {
        _flights.Add(new Flight(flightNumber, destination, departureTime, capacity));
    }

    public Airport()
    {
        //Добавляем рейсы, данные для примера очень малы, чтобы сработало больше предусмотренных ситуаций 
        AddFlight("PS101", "Kyiv", departureTime: 10, capacity: 12);
        AddFlight("PS202", "Lviv", departureTime: 8, capacity: 10);
        AddFlight("PS303", "Odesa", departureTime: 5, capacity: 2);
        AddFlight("PS404", "Kharkov", departureTime: 6, capacity: 4);
        AddFlight("PS505", "Sevastopol", departureTime: 10, capacity: 1);
    }

    public void Run()
    {
        //Симуляция длиться пока все рейсы не вылетят из аэропорта
        while (_flights.Any(f => f.Status != FlightStatus.Departed))
        {
            Console.Clear();
            Console.WriteLine($"=== Simulation Tick {_currentTick} ===");

            GeneratePassengers();
            ProcessCheckIn();
            ProcessSecurity();
            UpdateFlights();

            PrintStatus();

            _currentTick++;
            Thread.Sleep(2000);
        }
    }

    private void GeneratePassengers()
    {
        if (_rnd.NextDouble() <= ChanceNewPassenger)
        {
            var availableFlights = _flights
                .Where(f => f.Status == FlightStatus.OnTime || f.Status == FlightStatus.Delayed)
                .ToList();
            if (availableFlights.Count == 0)
            {
                return;
            }

            var flight = availableFlights[_rnd.Next(availableFlights.Count)];
            var name = _passengerFirstNames[_rnd.Next(_passengerFirstNames.Length)] + " " +
                       _passengerLastNames[_rnd.Next(_passengerLastNames.Length)];

            var passenger = new Passenger(name, flight.FlightNumber);

            _passengers.Add(passenger);
            _checkInQueue.Enqueue(passenger);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[EVENT] {passenger.Name} arrived for flight {flight.FlightNumber}");
            Console.ResetColor();
        }
    }

    private void ProcessCheckIn()
    {
        for (var i = 0; i < CheckInCounters && _checkInQueue.Count > 0; i++)
        {
            var passenger = _checkInQueue.Dequeue();
            passenger.HasTicket = true;
            _securityQueue.Enqueue(passenger);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"[CHECK-IN] {passenger.Name} checked in for {passenger.FlightNumber}");
            Console.ResetColor();
        }
    }

    private void ProcessSecurity()
    {
        for (var i = 0; i < SecurityGates && _securityQueue.Count > 0; i++)
        {
            var passenger = _securityQueue.Dequeue();
            passenger.PassedSecurity = true;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SECURITY] {passenger.Name} passed security");
            Console.ResetColor();
        }
    }

    private void UpdateFlights()
    {
        foreach (var flight in _flights)
        {
            //Вероятность задержки рейса
            if (_currentTick == flight.DepartureTime - 3 && flight.Status == FlightStatus.OnTime)
            {
                if (_rnd.NextDouble() <= ChanceDelayedStatus)
                {
                    flight.Status = FlightStatus.Delayed;
                    flight.DepartureTime += 3;

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(
                        $"[FLIGHT] {flight.FlightNumber} is Delayed! New departure time: {flight.DepartureTime}");
                    Console.ResetColor();
                }
            }

            //Начало посадки
            if (_currentTick == flight.DepartureTime - 2 &&
                (flight.Status == FlightStatus.OnTime || flight.Status == FlightStatus.Delayed))
            {
                flight.Status = FlightStatus.Boarding;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(
                    $"[FLIGHT] {flight.FlightNumber} is Boarding!");
                Console.ResetColor();
            }

            //Посадка пассажиров с учетом пассажировместимости рейса
            if (flight.Status == FlightStatus.Boarding)
            {
                var freeSeats = flight.Capacity - flight.PassengersOnBoard.Count;

                if (freeSeats > 0)
                {
                    var waitingPassengers = _passengers
                        .Where(p => p.FlightNumber == flight.FlightNumber &&
                                    p is { HasTicket: true, PassedSecurity: true, IsOnBoard: false })
                        .Take(Math.Min(BoardingRate, freeSeats))
                        .ToList();

                    foreach (var p in waitingPassengers)
                    {
                        p.IsOnBoard = true;
                        flight.PassengersOnBoard.Add(p);
                        
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"[BOARDING] {p.Name} boarded {flight.FlightNumber}");
                        Console.ResetColor();
                    }
                }
            }

            //Вылет
            if (_currentTick >= flight.DepartureTime && flight.Status != FlightStatus.Departed)
            {
                flight.Status = FlightStatus.Departed;

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(
                    $"[FLIGHT] {flight.FlightNumber} is Departed!");
                Console.ResetColor();
                
                foreach (var p in flight.PassengersOnBoard)
                {
                    _passengers.Remove(p);
                }

                var missedPassengers = _passengers
                    .Where(p => p.FlightNumber == flight.FlightNumber &&
                                p is { HasTicket: true, IsOnBoard: false })
                    .ToList();

                foreach (var mp in missedPassengers)
                {
                    if (mp.PassedSecurity && flight.PassengersOnBoard.Count >= flight.Capacity)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine(
                            $"[NO SEAT] Passenger {mp.Name} could not board {flight.FlightNumber} (no seats left).");
                        Console.ResetColor();
                        _passengers.Remove(mp);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"[NO TIME] Passenger {mp.Name} missed flight {flight.FlightNumber}!");
                        Console.ResetColor();
                        _passengers.Remove(mp);
                    }
                }
            }
        }
    }

    private void PrintStatus()
    {
        Console.WriteLine("\n--- Flight Status ---");
        foreach (var flight in _flights)
        {
            switch (flight.Status)
            {
                case FlightStatus.OnTime:
                    Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                case FlightStatus.Delayed:
                    Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case FlightStatus.Boarding:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                case FlightStatus.Departed:
                    Console.ForegroundColor = ConsoleColor.DarkGray; break;
            }
            
            Console.WriteLine(flight);
            Console.ResetColor();
        }

        Console.WriteLine("\n--- Queues ---");
        Console.WriteLine($"Check-in queue: {_checkInQueue.Count}");
        Console.WriteLine($"Security queue: {_securityQueue.Count}");

        var waiting = _passengers.Count(p => p is { HasTicket: true, PassedSecurity: true, IsOnBoard: false });
        Console.WriteLine($"Waiting for boarding: {waiting}");
    }
}