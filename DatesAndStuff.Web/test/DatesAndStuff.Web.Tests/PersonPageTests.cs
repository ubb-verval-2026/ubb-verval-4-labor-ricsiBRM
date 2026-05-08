using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DatesAndStuff.Web.Tests;

[TestFixture]
public class PersonPageTests
{
    private IWebDriver driver;
    private StringBuilder verificationErrors;
    private const string BaseURL = "http://localhost:5091";
    private bool acceptNextAlert = true;

    private Process? _blazorProcess;

    [OneTimeSetUp]
    public void StartBlazorServer()
    {
        var webProjectPath = Path.GetFullPath(Path.Combine(
            Assembly.GetExecutingAssembly().Location,
            "../../../../../../src/DatesAndStuff.Web/DatesAndStuff.Web.csproj"
            ));

        var webProjFolderPath = Path.GetDirectoryName(webProjectPath);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            //Arguments = $"run --project \"{webProjectPath}\"",
            Arguments = "dotnet run --no-build",
            WorkingDirectory = webProjFolderPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        _blazorProcess = Process.Start(startInfo);

        // Wait for the app to become available
        var client = new HttpClient();
        var timeout = TimeSpan.FromSeconds(30);
        var start = DateTime.Now;

        while (DateTime.Now - start < timeout)
        {
            try
            {
                var result = client.GetAsync(BaseURL).Result;
                if (result.IsSuccessStatusCode)
                {
                    break;
                }
            }
            catch (Exception e)
            {
                Thread.Sleep(1000);
            }
        }
    }

    [OneTimeTearDown]
    public void StopBlazorServer()
    {
        if (_blazorProcess != null && !_blazorProcess.HasExited)
        {
            _blazorProcess.Kill(true);
            _blazorProcess.Dispose();
        }
    }

    [SetUp]
    public void SetupTest()
    {
        driver = new ChromeDriver();
        verificationErrors = new StringBuilder();
    }

    [TearDown]
    public void TeardownTest()
    {
        try
        {
            driver.Quit();
            driver.Dispose();
        }
        catch (Exception)
        {
            // Ignore errors if unable to close the browser
        }
        Assert.That(verificationErrors.ToString(), Is.EqualTo(""));
    }

    //[Test]
    [TestCase("5", 5250)]
    [TestCase("10", 5500)]
    [TestCase("20", 6000)]
    public void Person_SalaryIncrease_ShouldIncrease(string percentage, double expectedSalary)
    {
        // Arrange
        driver.Navigate().GoToUrl(BaseURL);

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        var navButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-test='PersonPageNavigation']")));
        navButton.Click();

        var input = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='SalaryIncreasePercentageInput']")));
        input.Clear();
        input.SendKeys(percentage);

        // Act
        var submitButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-test='SalaryIncreaseSubmitButton']")));
        submitButton.Click();

        // Assert
        var salaryLabel = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='DisplayedSalary']")));

        // Érdemes parse-olás előtt ránézni, hogy ne dobjon hibát, ha üres a szöveg
        var salaryAfterSubmission = double.Parse(salaryLabel.Text);

        salaryAfterSubmission.Should().BeApproximately(expectedSalary, 0.001);
    }

    [TestCase("-10")]
    public void Person_SalaryIncrease_UnderMinusTen_ShouldShowErrors(string invalidPercentage)
    {
        driver.Navigate().GoToUrl(BaseURL);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); 

        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-test='PersonPageNavigation']"))).Click();

        var input = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='SalaryIncreasePercentageInput']")));
        input.Clear();
        input.SendKeys(invalidPercentage);

        var submitButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-test='SalaryIncreaseSubmitButton']")));

        if (invalidPercentage == "-10" && submitButton.Enabled)
        {
            Console.WriteLine("Inkonzisztencia: A gomb aktív -10-nél, pedig a backend tiltja!");
        }

        submitButton.Click();

        try
        {
            var fieldError = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@data-test='SalaryIncreaseFieldError']")));
            fieldError.Text.Should().NotBeNullOrEmpty();
            Console.WriteLine("Siker! A hibaüzenet megjelent: " + fieldError.Text);
        }
        catch (WebDriverTimeoutException)
        {
            Assert.Fail("A hibaüzenet nem jelent meg 10 másodperc után sem, pedig rossz értéket adtunk meg!");
        }
    }


    [Test]
    public void BlazeDemo_MexicoCityToDublin_ShouldHaveAtLeastThreeFlights()
    {
        // Arrange
        driver.Navigate().GoToUrl("https://blazedemo.com");
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        var departureSelect = new SelectElement(driver.FindElement(By.Name("fromPort")));
        departureSelect.SelectByValue("Mexico City");

        var destinationSelect = new SelectElement(driver.FindElement(By.Name("toPort")));
        destinationSelect.SelectByValue("Dublin");

        // Act
        driver.FindElement(By.CssSelector("input[type='submit']")).Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("table")));

        // Assert
        var flightRows = driver.FindElements(By.XPath("//table[@class='table']/tbody/tr"));

        int flightCount = flightRows.Count;

        flightCount.Should().BeGreaterThanOrEqualTo(3,
            $"Mivel Mexico City és Dublin között legalább 3 járatot vártunk, de csak {flightCount} található.");

        Console.WriteLine($"Sikeres teszt: {flightCount} járatot találtunk.");
    }




    private bool IsElementPresent(By by)
    {
        try
        {
            driver.FindElement(by);
            return true;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    private bool IsAlertPresent()
    {
        try
        {
            driver.SwitchTo().Alert();
            return true;
        }
        catch (NoAlertPresentException)
        {
            return false;
        }
    }

    private string CloseAlertAndGetItsText()
    {
        try
        {
            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;
            if (acceptNextAlert)
            {
                alert.Accept();
            }
            else
            {
                alert.Dismiss();
            }
            return alertText;
        }
        finally
        {
            acceptNextAlert = true;
        }
    }
}