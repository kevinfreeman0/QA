using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WalmartSurvey
{
    [TestFixture]
    class AutoFill
    {
        IWebDriver driver;
        const int ClickTimeoutSeconds = 5;
        const int SendKeysTimeoutSeconds = 5;

        [SetUp]
        public void startBrowser()
        {
            //ChromeOptions chromeOptions = new ChromeOptions();
            //driver = new ChromeDriver(Inputs.ChromeDriverDirectory, chromeOptions, new TimeSpan(0, 0, 0, 10));

            driver = new ChromeDriver(Inputs.ChromeDriverDirectory);
            //driver = new ChromeDriver(@"\\MAIN\Portal\Kevin\Selenium");
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void closeBrowser()
        {
            driver.Close();
        }

        [Test]
        public void Survey_AutoFillsOk()
        {
            driver.Url = Inputs.Url;

            // Validate inputs that change frequently
            Assert.That(Inputs.TCNumber.Replace(" ", "").Length == Inputs.TCNumberLength,
                String.Format("TCNumber value must be {0} digits.", Inputs.TCNumberLength));
            DateTime visitDateOverrideValue;
            Assert.That(String.IsNullOrWhiteSpace(Inputs.VisitDateOverride) ||
                DateTime.TryParse(Inputs.VisitDateOverride, out visitDateOverrideValue),
                "VisitDateOverride value must be blank or in 'MM/dd/YYYY' format.");
            Assert.That(Inputs.VisitTime.EndsWith(":00 AM") || Inputs.VisitTime.EndsWith(":00 PM"),
                "VisitTime value must be whole hour in 'h:mm AM' / 'h:mm PM' format.");

            // Click a language label on language page
            Click(driver.FindElement(By.XPath(String.Format(@"//label[text()='{0}']", Inputs.Language))));
            Click(driver.FindElement(By.Id("btn_continue")));

            // Click past intro page
            Click(driver.FindElement(By.Id("btn_continue")));

            // Enter postal code
            SendKeys(driver.FindElement(By.Name("ans121.0.0")), Inputs.PostalCode);
            Click(driver.FindElement(By.Id("btn_continue")));

            // Ensure we're on the page asking about Walmart employee status, then choose 'No'.
            IWebElement employeeQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'family') and contains(text(),'employee')]"));
            Click(driver.FindElement(By.XPath(@"//span[normalize-space(text())='No']")));
            Click(driver.FindElement(By.Id("btn_continue")));

            // Ensure we're on the province page, then choose province.
            IWebElement provinceQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'province')]"));
            Click(driver.FindElement(By.XPath(String.Format(@"//span[normalize-space(text())='{0}']", Inputs.Province))));
            Click(driver.FindElement(By.Id("btn_continue")));

            // Enter birth year and indicate age of majority.
            SendKeys(driver.FindElement(By.Id("ans7876.0.0")), Inputs.BirthYear.ToString() + Keys.Tab);
            SendKeys(driver.FindElement(By.Id("ans203.0.0")), " ");
            Click(driver.FindElement(By.Id("btn_continue")));

            // Ensure we're on the skill-testing question page, then provide answer.
            IWebElement skillQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'skill')]"));
            SendKeys(driver.FindElement(By.Id("ans6839.0.0")), Inputs.SkillTestAnswer.ToString());
            Click(driver.FindElement(By.Id("btn_continue")));

            // Ensure we're on the receipt availability confirmation page, then choose 'Yes'.
            IWebElement receiptQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'receipt')]"));
            Click(driver.FindElement(By.XPath(@"//span[normalize-space(text())='Yes']")));
            Click(driver.FindElement(By.Id("btn_continue")));

            // Ensure we're on the store/TC/date page, then enter values.
            #region Store/TC/Date
            IWebElement storeTcDateQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'Store #')]"));
            SendKeys(driver.FindElement(By.Id("ans2968.0.0")), Inputs.StoreNumber.ToString());

            string[] tcParts = SplitTCNumber();
            SendKeys(driver.FindElement(By.Id("ans7310.0.0")), tcParts[0]);
            SendKeys(driver.FindElement(By.Id("ans7316.0.0")), tcParts[1]);
            SendKeys(driver.FindElement(By.Id("ans7321.0.0")), tcParts[2]);
            SendKeys(driver.FindElement(By.Id("ans7326.0.0")), tcParts[3]);
            SendKeys(driver.FindElement(By.Id("ans7331.0.0")), tcParts[4] + Keys.Tab);

            DateTime visitDate = GetVisitDate();
            IWebElement chosenYear = driver.FindElement(By.XPath(@"//div[contains(@class,'ui-datepicker-year')]//descendant::span[@class='select2-chosen']"));
            if (chosenYear.Text != visitDate.Year.ToString())
            {
                // Choose different year
                Click(chosenYear);
                IWebElement newYearToChoose = driver.FindElement(By.XPath(String.Format(@"//*[@id='select2-drop']//descendant::div[text()='{0}']", visitDate.Year.ToString())));
                Click(newYearToChoose);
            }
            IWebElement chosenMonthShortName = driver.FindElement(By.XPath(@"//div[contains(@class,'ui-datepicker-month')]//descendant::span[@class='select2-chosen']"));
            if (chosenMonthShortName.Text != visitDate.ToString("MMM"))
            {
                // Choose different month
                Click(chosenMonthShortName);
                IWebElement newMonthToChoose = driver.FindElement(By.XPath(String.Format(@"//*[@id='select2-drop']//descendant::div[text()='{0}']", visitDate.ToString("MMM"))));
                Click(newMonthToChoose);
            }
            IWebElement newDayToChoose = driver.FindElement(By.XPath(String.Format(@"//td[@data-month='{0}' and @data-year='{1}']//descendant::a[text()='{2}']", (visitDate.Month-1).ToString(), visitDate.Year.ToString(), visitDate.Day.ToString())));
            Click(newDayToChoose);
            Click(driver.FindElement(By.Id("btn_continue")));
            #endregion Store/TC/Date

            // Choose arrival time.
            IWebElement timeDropDown = driver.FindElement(By.Id("ans3395.0.0"));
            SelectElement selectElement = new SelectElement(timeDropDown);
            selectElement.SelectByText(Inputs.VisitTime);
            Click(driver.FindElement(By.Id("btn_continue")));

            // Ensure we're on the page asking about using self-checkout, then choose 'No'.
            IWebElement selfCheckoutQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'self check-out')]"));
            Click(driver.FindElement(By.XPath(@"//span[normalize-space(text())='No']")));
            Click(driver.FindElement(By.Id("btn_continue")));
            
            // Ensure we're on the page asking about overall satisfaction, then choose '9' (0-based, so use 8).
            IWebElement overallSatisfactionQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'Overall') and contains(text(),'satisfied')]"));
            Click(driver.FindElement(By.XPath(@"//td[contains(@class,'walrating') and @data-col='8']")));
            Click(driver.FindElement(By.Id("btn_continue")));

            // Ensure we're on the page asking about satisfaction in specific areas, then choose '9' (0-based, so use 8) for all.
            IWebElement specificSatisfactionQuestionText = driver.FindElement(By.XPath(@"//*[contains(text(),'satisfied') and contains(text(),'areas')]"));
            ReadOnlyCollection<IWebElement> ratingRowsValue9 = driver.FindElements(By.XPath(@"//td[contains(@class,'walrating') and @data-col='8']"));
            ratingRowsValue9.ToList<IWebElement>().ForEach(Click);
            Click(driver.FindElement(By.Id("btn_continue")));


        }

        /// <summary>
        /// Tries clicking once per second until success or timeout occurs
        /// </summary>
        /// <param name="webElement"></param>
        /// <returns></returns>
        private void Click(IWebElement webElement)
        {
            int tries = 0;
            while(true)
            {
                try
                {
                    tries++;
                    webElement.Click();
                    return;
                }
                catch (ElementClickInterceptedException)
                {
                    if (tries == ClickTimeoutSeconds)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        /// <summary>
        /// Checks once per second if element is enabled for SendKeys or timeout occurs
        /// </summary>
        /// <param name="webElement"></param>
        private void SendKeys(IWebElement webElement, string text)
        {
            int tries = 0;
            while (!webElement.Enabled && tries < SendKeysTimeoutSeconds)
            {
                tries++;
                Thread.Sleep(1000);
            }
            webElement.SendKeys(text);


            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SendKeysTimeoutSeconds));
            //IWebElement element = wait.Until(driver => driver.FindElement(By.Name("q")));
            
        }

        /// <summary>
        /// Normalizes TC # and returns it as 5 chunks of lengths 4, 4, 4, 4, and 5.
        /// </summary>
        /// <returns></returns>
        private string[] SplitTCNumber()
        {
            string tcNumberNoSpaces = Inputs.TCNumber.Replace(" ", "");
            string[] tcParts = new string[5];
            tcParts[0] = tcNumberNoSpaces.Substring(0, 4);
            tcParts[1] = tcNumberNoSpaces.Substring(4, 4);
            tcParts[2] = tcNumberNoSpaces.Substring(8, 4);
            tcParts[3] = tcNumberNoSpaces.Substring(12, 4);
            tcParts[4] = tcNumberNoSpaces.Substring(16, 5);
            return tcParts;
        }

        /// <summary>
        /// Returns overridden visit date in mm/dd/yyyy format if provided; otherwise defaults to today
        /// </summary>
        private DateTime GetVisitDate()
        {
            if (String.IsNullOrWhiteSpace(Inputs.VisitDateOverride)) // not overridden
            {
                return DateTime.Today;
            }
            else
            {
                return DateTime.Parse(Inputs.VisitDateOverride);
            }
        }

    }
}
