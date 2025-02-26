using MicrowaveOven.Interfaces;
using Timer = System.Timers.Timer;

namespace MicrowaveOven
{
    public class MicrowaveController
    {
        //TODO: Move texts to global dictionary
        public const string time_is_up = "Time is up.";
        public const string time_ramaining = "Time ramaining:";
        public const string door_is_open = "Door is open.";
        public const string door_is_closed = "Door is closed.";
        public const string remaining_time = "Remaining time:";

        private readonly IMicrowaveOvenHW _microwave;
        private static Timer? _timer;
        private static int _remainingTime = 0;
        private static int _timeStep = 60;   

        public MicrowaveController(IMicrowaveOvenHW microwave)
        {
            _microwave = microwave;
            _timer = new Timer(1000);
            _timer.Elapsed += (sender, e) =>
            {
                if (_remainingTime > 0)
                {
                    _remainingTime--;
                }
                else
                {
                    _timer?.Stop();
                    _microwave.TurnOffHeater();
                    Console.WriteLine(time_is_up);
                }
            };
            _timer.AutoReset = true;
        }

        public void OpenDoor()
        {
            if (!_microwave.DoorOpen)
            {
                _microwave.DoorOpenChanged += OnDoorOpenChanged;
            }
            _microwave.TurnOffHeater();
            _timer?.Stop();
        }

        public void CloseDoor()
        {
            if (_microwave.DoorOpen)
            {
                _microwave.DoorOpenChanged += OnDoorOpenChanged;
                if (_remainingTime > 0)
                {
                    _timer?.Start();
                }
            }
            _microwave.TurnOnHeater();            
        }

        public void StartButton()
        {
            if (!_microwave.DoorOpen)
            {
                _microwave.StartButtonPressed += OnStartButtonPressed;
                _microwave.TurnOnHeater();
            }
        }

        private void OnStartButtonPressed(object sender, EventArgs e)
        {
            _remainingTime += _timeStep;
            _timer?.Start();
            Console.WriteLine($"{time_ramaining} {_remainingTime}");                
        }

        private void OnDoorOpenChanged(bool isOpen)
        {
            if (isOpen)
            {
                Console.WriteLine($"{door_is_open} ({remaining_time} {_remainingTime})");
            }
            else
            {
                Console.WriteLine($"{door_is_closed} ({remaining_time} {_remainingTime})");
            }
        }

    }
}
