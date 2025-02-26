using MicrowaveOven.Interfaces;

namespace MicrowaveOven.Manufacturers
{
    public class MicrowaveOvenHW : IMicrowaveOvenHW
    {
        public const string light_is_on = "Light is on";
        public const string light_is_off = "Light is off";
        public const string heater_turned_on = "Heater turned on.";
        public const string heater_turned_off = "Heater turned off.";

        private bool? IsRemainingTimeFinished = null;

        private bool _doorOpen;
        public bool DoorOpen
        {
            get => _doorOpen;
            private set
            {
                if (_doorOpen != value)
                {
                    _doorOpen = value;
                    //Console.WriteLine($"LOG: Door status: {(_doorOpen ? "open" : "close")}");
                    Logger(_doorOpen ? light_is_on : light_is_off);
                }
            }
        }

        public event Action<bool>? DoorOpenChanged = null;
        public event EventHandler? StartButtonPressed = null;

        public void TurnOnHeater()
        {
            if (DoorOpen && DoorOpenChanged is not null)
            {
                DoorOpen = false;
                DoorOpenChanged.Invoke(DoorOpen);
                DoorOpenChanged = null;
                if (IsRemainingTimeFinished is not null && !IsRemainingTimeFinished.Value)
                {
                    Logger(heater_turned_on);
                }
            }

            if (!DoorOpen && StartButtonPressed is not null)
            {
                StartButtonPressed?.Invoke(this, EventArgs.Empty);
                StartButtonPressed = null;
                Logger(heater_turned_on);
                IsRemainingTimeFinished = false;
            }
        }        

        public void TurnOffHeater()
        {
            if (!DoorOpen && DoorOpenChanged is not null)
            {
                DoorOpen = true;
                DoorOpenChanged.Invoke(DoorOpen);
                DoorOpenChanged = null;
                Logger(heater_turned_off);
            }

            if (!DoorOpen && DoorOpenChanged is null)
            {
                Logger(heater_turned_off);
                IsRemainingTimeFinished = true;
            }
        }

        #region Tools
        //TODO: Add Nlog or something similar to the method
        private static void Logger(string message)
        {
            Console.WriteLine($"LOG: {message}");
        }

        #endregion
    }
}
