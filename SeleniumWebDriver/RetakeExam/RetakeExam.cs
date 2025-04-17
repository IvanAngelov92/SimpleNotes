using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace RetakeExam
{
    public class RetakeExam
    {
        private IWebDriver driver;

        private readonly string BaseUrl = "https://d5wfqm7y6yb3q.cloudfront.net/";

        Random random;

        string lastCreatedNoteTitle = "";
        string lastCreatedNoteDescription = "";

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            
            driver.Navigate().GoToUrl($"{BaseUrl}User/LoginRegister");

            driver.FindElement(By.XPath("//a[@id=\"tab-login\"]")).Click();

            driver.FindElement(By.XPath("//input[@id=\"loginName\"]")).SendKeys("ivantesting1@gmail.com");
            driver.FindElement(By.XPath("//input[@id=\"loginPassword\"]")).SendKeys("ivantesting123");
            driver.FindElement(By.XPath("//button[@class=\"btn btn-primary btn-block mb-4\"]")).Click();
        }

        [Test, Order(1)]
        public void AddNoteWithInvalidData_Test()
        {
            string invalidNoteTitle= "";
            string invalidNoteDescription = "";

            driver.Navigate().GoToUrl(BaseUrl + "Note/New");

            driver.FindElement(By.XPath("//a[@href=\"/Note/Create\"]")).Click();

            driver.FindElement(By.XPath("//input[@id=\"form4Example1\"]")).SendKeys(invalidNoteTitle);
            driver.FindElement(By.XPath("//textarea[@id=\"form4Example3\"]")).SendKeys(invalidNoteDescription);

            driver.FindElement(By.XPath("//button[@type=\"submit\"]")).Click();

            Assert.That(driver.Url, Is.EqualTo(BaseUrl + "Note/Create"));

            var errorMessage = driver.FindElement(By.XPath("//div[@class=\"toast-message\"]")).Text;

            Assert.That(errorMessage.Trim(), Is.EqualTo("The Title field is required. The Description field is required."));
        }

        [Test, Order(2)]
        public void AddRandomNote_Test()
        {
            lastCreatedNoteTitle = GenerateRandomString("title");
            lastCreatedNoteDescription = GenerateRandomString("description1231331362694213213213");

            driver.Navigate().GoToUrl(BaseUrl + "Note/New");

            driver.FindElement(By.XPath("//a[@href=\"/Note/Create\"]")).Click();

            driver.FindElement(By.XPath("//input[@id=\"form4Example1\"]")).SendKeys(lastCreatedNoteTitle);
            driver.FindElement(By.XPath("//textarea[@id=\"form4Example3\"]")).SendKeys(lastCreatedNoteDescription);

            driver.FindElement(By.XPath("//select[@name=\"Status\"]")).SendKeys("New");

            driver.FindElement(By.XPath("//button[@type=\"submit\"]")).Click();

            var succesMessage = driver.FindElement(By.XPath("//div[@class=\"toast-message\"]")).Text;

            Assert.That(succesMessage.Trim(), Is.EqualTo("Note created successfully!"));
        }

        [Test, Order(3)]
        public void EditLastAddedNote_Test()
        {
            driver.Navigate().GoToUrl(BaseUrl + "Note/New");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var notes = wait.Until(driver => driver.FindElements(By.XPath("//section[@class=\"p-4 d-flex justify-content-center text-center w-100\"]")));

            Assert.IsTrue(notes.Count > 0, "No notes were found on the page.");

            var lastNotes = notes.Last();
            var editButton = lastNotes.FindElement(By.CssSelector("a[href*=\"/Note/Edit\"]"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(editButton).Click().Perform();

            var titleInput = driver.FindElement(By.XPath("//input[@id=\"form4Example1\"]"));
            string newTitle = "Changed Title: " + lastCreatedNoteTitle;
            titleInput.Clear();
            titleInput.SendKeys(newTitle);

            var editSubmitButton = driver.FindElement(By.XPath("//button[@class=\"btn btn-info btn-block mb-4 col-6\"]"));
            editSubmitButton.Click();

            driver.Navigate().GoToUrl(BaseUrl + "Note/New");

            var notesTitleElement = driver.FindElement(By.XPath("//div[@class=\"card-body\"]//h5[@class=\"card-title\"]"));
            string notesTitle = notesTitleElement.Text.Trim();
            
            Assert.That(notesTitle, Is.EqualTo(newTitle), "The title of the note does not match the expected value.");
        }

        [Test, Order(4)]
        public void MoveEditedNotetoDone_Test()
        {
            driver.Navigate().GoToUrl(BaseUrl + "Note/New");

            driver.FindElement(By.XPath("//a[@class=\"btn btn-primary\"]")).Click();

            var succesMessage = driver.FindElement(By.XPath("//div[@class=\"toast-message\"]")).Text;

            Assert.That(succesMessage.Trim(), Is.EqualTo("Note status changed successfully!"));
        }

        [Test, Order(5)]
        public void DeleteEditedNote_Test()
        {
            driver.Navigate().GoToUrl(BaseUrl + "Note/Done");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var notes = wait.Until(driver => driver.FindElements(By.XPath("//section[@class=\"p-4 d-flex justify-content-center text-center w-100\"]")));

            Assert.IsTrue(notes.Count > 0, "No notes were found on the page.");

            var lastNotes = notes.Last();
            var deleteButton = lastNotes.FindElement(By.XPath("//a[@class=\"btn btn-danger\"]"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(deleteButton).Click().Perform();

            driver.FindElement(By.XPath("//button[@type=\"submit\"]")).Click();

            var deleteMessage = driver.FindElement(By.XPath("//div[@class=\"toast-message\"]")).Text;

            Assert.That(deleteMessage.Trim(), Is.EqualTo("Note deleted successfully!"));
        }

        [Test, Order(6)]
        public void Logout_Test()
        {
            driver.FindElement(By.XPath("//span[text()=\"Logout\"]")).Click();

            Assert.That(driver.Url, Is.EqualTo(BaseUrl));

            driver.Navigate().GoToUrl(BaseUrl + "Note/New");

            var accesDeniedMessage = driver.FindElement(By.XPath("//pre[text()=\"Access Denied\"]")).Text;

            Assert.That(accesDeniedMessage.Trim(), Is.EqualTo("Access Denied"));
        }

        private string GenerateRandomString(string prefix)
        {
            random = new Random();

            return prefix + random.Next(999, 99999).ToString();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}