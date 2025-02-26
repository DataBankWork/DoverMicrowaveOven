using MicrowaveOven;
using MicrowaveOven.Manufacturers;
using NUnit.Framework;
using System.Reflection;


namespace MicrowaveOvenTest
{
    [TestFixture]
    public class MicrowaveControllerTests
    {
        private MicrowaveOvenHW _microwaveMock;
        private MicrowaveController _controller;
        private StringWriter _consoleOutput;
        private TextWriter _originalOutput;

        [SetUp]
        public void Setup()
        {
            _microwaveMock = new MicrowaveOvenHW();
            _controller = new MicrowaveController(_microwaveMock);
            _originalOutput = Console.Out;
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
        }

        [TearDown]
        public void Teardown()
        {
            Console.SetOut(_originalOutput);
            _consoleOutput.Dispose();
        }

        [Test]
        public void WhenIOpenDoor_LightIsOn()
        {
            _controller.CloseDoor();
            _controller.StartButton();
            var beforLastStep = _consoleOutput.ToString();
            _controller.OpenDoor();

            string result = GetLastStep(beforLastStep);

            Assert.IsTrue(_consoleOutput.ToString().Contains(MicrowaveOvenHW.light_is_on));
            Assert.IsTrue(result.Contains(MicrowaveOvenHW.heater_turned_off));
        }

        [Test]
        public void WhenICloseDoor_LightIsOff()
        {
            _controller.OpenDoor();
            var beforLastStep = _consoleOutput.ToString();
            _controller.CloseDoor();

            string result = GetLastStep(beforLastStep);

            Assert.IsTrue(_consoleOutput.ToString().Contains(MicrowaveOvenHW.light_is_off));
            Assert.IsTrue(result.Contains(MicrowaveController.door_is_closed));
        }

        [Test]
        public void WhenIOpenDoor_HeaterStopsIfRunning()
        {
            _controller.CloseDoor();
            _controller.StartButton();
            var beforLastStep = _consoleOutput.ToString();
            _controller.OpenDoor();

            string result = GetLastStep(beforLastStep);

            Assert.IsTrue(result.Contains(MicrowaveOvenHW.heater_turned_off));
        }

        [Test]
        public void WhenIPressStartButtonWhenDoorIsOpen_NothingHappens()
        {
            _controller.OpenDoor();
            var beforLastStep = _consoleOutput.ToString();
            _controller.StartButton();

            string result = GetLastStep(beforLastStep);

            Assert.IsTrue(result.Equals(string.Empty));
        }

        [Test]
        public void WhenIPressStartButtonWhenDoorIsClosed_HeaterRunsFor1Minute()
        {
            int timeStep = GetNonPublicStaticValueFromController("_timeStep");
            _controller.OpenDoor();
            _controller.CloseDoor();
            var beforLastStep = _consoleOutput.ToString();
            _controller.StartButton();

            int remainingTime = GetNonPublicStaticValueFromController("_remainingTime");
            string result = GetLastStep(beforLastStep);

            Assert.IsTrue(result.Contains(MicrowaveOvenHW.heater_turned_on));
            Assert.IsTrue(remainingTime == timeStep);
        }

        [Test]
        public void WhenIPressStartButtonWhenDoorIsClosedAndAlreadyHeating_IncreaseRemainingTimeWith1Minute()
        {
            int timeStep = GetNonPublicStaticValueFromController("_timeStep");
            _controller.OpenDoor();
            _controller.CloseDoor();
            _controller.StartButton();
            var beforLastStep = _consoleOutput.ToString();
            Thread.Sleep(3000); // Simulation of working time
            int remainingTimeBeforeSecondStart = GetNonPublicStaticValueFromController("_remainingTime");
            _controller.StartButton();
            int remainingTimeAfterSecondStart = GetNonPublicStaticValueFromController("_remainingTime");

            string result = GetLastStep(beforLastStep);

            Assert.IsTrue(result.Contains(MicrowaveOvenHW.heater_turned_on));
            Assert.IsTrue(remainingTimeAfterSecondStart == remainingTimeBeforeSecondStart + timeStep);
        }

        /// <summary>
        /// Aditional tests ----------------------------------------------------------------------------------------------
        /// </summary>
        /// 

        [Test]
        public void WhenICloseDoor_HeaterStartsIfTimeRemaining()
        {
            _controller.CloseDoor();
            _controller.StartButton();
            _controller.OpenDoor();
            var beforLastStep = _consoleOutput.ToString();
            _controller.CloseDoor();

            string result = GetLastStep(beforLastStep);
            int remainingTime = GetNonPublicStaticValueFromController("_remainingTime");

            Assert.IsTrue(result.Contains(MicrowaveOvenHW.heater_turned_on)); 
            Assert.IsTrue(remainingTime > 0);
        }  

        #region Tools
        private string GetLastStep(string beforLastStepConsoleOutput)
        {
            return _consoleOutput.ToString().Replace(beforLastStepConsoleOutput, "");
        }

        private static int GetNonPublicStaticValueFromController(string fieldName)
        {
            Type myClassType = typeof(MicrowaveController);
            FieldInfo fieldInfo = myClassType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            return (int)fieldInfo.GetValue(null);
        }
        #endregion
    }
}

