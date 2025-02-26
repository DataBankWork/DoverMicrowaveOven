using MicrowaveOven.Interfaces;
using MicrowaveOven.Manufacturers;

namespace MicrowaveOven
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IMicrowaveOvenHW hardware = new MicrowaveOvenHW();
            var controller = new MicrowaveController(hardware);
                        
            Console.WriteLine("Options: open, close, start, exit");

            while (true)
            {
                Console.Write(">> ");
                string? command = Console.ReadLine()?.ToLower();

                switch (command)
                {
                    case "open":
                        controller.OpenDoor();
                        break;
                    case "close":
                        controller.CloseDoor();
                        break;
                    case "start":
                        controller.StartButton();
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }
    }    
}
