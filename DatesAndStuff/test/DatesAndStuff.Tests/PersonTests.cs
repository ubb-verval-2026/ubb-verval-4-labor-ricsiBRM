using AutoFixture;
using FluentAssertions;

namespace DatesAndStuff.Tests;

public class PersonTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GotMerried_First_NameShouldChange()
    {
        // Arrange
        var sut = PersonFactory.CreateTestPerson();

        string newName = "Test-Eleso Pista";
        double salaryBeforeMarriage = sut.Salary;
        var beforeChanges = Person.Clone(sut);

        // Act
        sut.GotMarried(newName);

        // Assert
        Assert.That(sut.Name, Is.EqualTo(newName)); // act = exp

        sut.Name.Should().Be(newName);
        sut.Should().BeEquivalentTo(beforeChanges, o => o.Excluding(p => p.Name));

        //sut.Salary.Should().Be(salaryBeforeMarriage);

        //Assert.AreEqual(newName, sut.Name); // = (exp, act)
        //Assert.AreEqual(salaryBeforeMarriage, sut.Salary);
    }

    [Test]
    public void GotMerried_Second_ShouldFail()
    {
        // Arrange
        var fixture = new AutoFixture.Fixture();
        fixture.Customize<IPaymentService>(c => c.FromFactory(() => new TestPaymentService()));

        var sut = fixture.Create<Person>();

        string newName = "Test-Eleso-Felallo Pista";
        sut.GotMarried("");

        // Act
        var task = Task.Run(() => sut.GotMarried(""));
        try { task.Wait(); } catch { }

        // Assert
        //Assert.IsTrue(task.IsFaulted);
    }

    [Test]
    [CustomPersonCreationAutodataAttribute]
    public void IncreaseSalary_ReasonableValue_ShouldModifySalary(Person sut, double salaryIncreasePercentage)
    {
        // Arrange
        double initialSalary = sut.Salary;

        // Act
        sut.IncreaseSalary(salaryIncreasePercentage);

        // Assert
        sut.Salary.Should().BeApproximately(initialSalary * (100 + salaryIncreasePercentage) / 100, Math.Pow(10, -8), because: "numerical salary calculation might be rounded to conform legal stuff");
    }

    [Test]
    public void Constructor_DefaultParams_ShouldBeAbleToEatChocolate()
    {
        // Arrange

        // Act
        Person sut = PersonFactory.CreateTestPerson();

        // Assert
        sut.CanEatChocolate.Should().BeTrue();
    }

    [Test]
    public void Constructor_DontLikeChocolate_ShouldNotBeAbleToEatChocolate()
    {
        // Arrange

        // Act
        Person sut = PersonFactory.CreateTestPerson(fp => fp.CanEatChocolate = false);

        // Assert
        sut.CanEatChocolate.Should().BeFalse();
    }

    [Test]
    public void IncreaseSalary_ZeroPercentIncrease_ShouldNotChange()
    {
        // throw new NotImplementedException();
    }

    [Test]
    public void IncreaseSalary_NegativeIncrease_ShouldDecrease()
    {
        // throw new NotImplementedException();
    }

    [Test]
    public void IncreaseSalary_SmallerThanMinusTenPerc_ShouldFail()
    {
        // throw new NotImplementedException();
    }
}