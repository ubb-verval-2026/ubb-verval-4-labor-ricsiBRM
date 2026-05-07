namespace DatesAndStuff.Tests
{
    public sealed class SimulationTimeTests
    {
        [OneTimeSetUp]
        public void OneTimeSetupStuff()
        {
            // 
        }

        [SetUp]
        public void Setup()
        {
            // minden teszt felteheti, hogz elotte lefutott ez
        }

        [TearDown]
        public void TearDown()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [Test]
        // Default time is not current time.
        public void SimulationTime_Construction()
        {
            // throw new NotImplementedException();
        }

        [Test]
        // equal
        // not equal
        // <
        // >
        // <= different
        // >= different 
        // <= same
        // >= same
        // max
        // min
        public void SimulationTime_Op()
        {
            // throw new NotImplementedException();
        }

        private class TimeSpanArithmeticTests
        {

            [Test]
            // TimeSpanArithmetic
            // add
            // substract
            // Given_When_Then
            public void Addition_SimulationTimeIsShifted()
            {
                // UserSignedIn_OrderSent_OrderIsRegistered
                // DBB, specflow, cucumber, gherkin

                // Arrange
                DateTime baseDate = new DateTime(2010, 8, 23, 9, 4, 49);
                SimulationTime sut = new SimulationTime(baseDate);

                var ts = TimeSpan.FromMilliseconds(4544313);

                // Act
                var result = sut + ts;

                // Assert
                var expectedDateTime = baseDate + ts;
                //Assert.AreEqual(expectedDateTime, result.ToAbsoluteDateTime());
            }

            [Test]
            //Method_Should_Then
            public void Subtracttion_SimulationTimeShifted()
            {
                // code kozelibb
                // RegisterOrder_SignedInUserSendsOrder_OrderIsRegistered
                // throw new NotImplementedException();
            }
        }


        [Test]
        // simulation difference timespane and datetimetimespan is the same
        public void SimulationTime_SubtractSimulationTime()
        {
            // throw new NotImplementedException();
        }

        [Test]
        // millisecond representation works
        public void SimulationTime_1msPerTick()
        {
            //var t1 = SimulationTime.MinValue.AddMilliseconds(10);
            // throw new NotImplementedException();
        }

        [Test]
        // next millisec calculation works
        public void SimulationTime_NextMillisec()
        {
            //Assert.AreEqual(t1.TotalMilliseconds + 1, t1.NextMillisec.TotalMilliseconds);
            // throw new NotImplementedException();
        }

        [Test]
        // creat a SimulationTime from a DateTime, add the same milliseconds to both and check if they are still equal
        public void SimulationTime_AddMilliSeconds()
        {
            // throw new NotImplementedException();
        }

        [Test]
        // the same as before just with seconds
        public void SimulationTime_AddSeconds()
        {
            // throw new NotImplementedException();
        }

        [Test]
        // same as before just with timespan
        public void SimulationTime_AddTimeSpan()
        {
            // throw new NotImplementedException();
        }

        [Test]
        // check string representation given by ToString
        public void SimulationTime_ToString()
        {
            // throw new NotImplementedException();
        }
    }
}