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
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); // Növeljük 10-re a türelmi időt

        // Kattintsunk a navigációra
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-test='PersonPageNavigation']"))).Click();

        // Megkeressük az inputot, kitöröljük, beírjuk az értéket
        var input = wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-test='SalaryIncreasePercentageInput']")));
        input.Clear();
        input.SendKeys(invalidPercentage);

        // Kattintunk a gombra
        var submitButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@data-test='SalaryIncreaseSubmitButton']")));

        if (invalidPercentage == "-10" && submitButton.Enabled)
        {
            Console.WriteLine("Inkonzisztencia: A gomb aktív -10-nél, pedig a backend tiltja!");
        }

        submitButton.Click();

        // --- Itt jön a kritikus rész ---
        // Ellenőrizzük, hogy láthatóvá válik-e a hibaüzenet
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