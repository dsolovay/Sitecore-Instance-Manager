using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;

namespace SIM.Specs
{
    [Binding]
    public class CommandLineSteps:IDisposable
    {
        ChromeDriver driver = new ChromeDriver();
        public void Dispose()
        {
            if (driver != null)
            {
                driver.Dispose();
                driver = null;
            }
        }


        [Given(@"No Sitecore intance named '(.*)' exists")]
        public void GivenNoSitecoreIntanceNamedExists(string siteName)
        {
            ThenDelete(siteName);
            Assert.IsFalse(SiteFound(siteName));
        }

        private bool SiteFound(string siteName)
        {
            driver.Navigate().GoToUrl($"http://{siteName}/");
            bool nameNotResolved = driver.PageSource.Contains("ERR_NAME_NOT_RESOLVED");

            // HACK There is a moment in the test execution where IIS handles the page not found, rather than chrome.
            bool iisPage = driver.FindElementsByTagName("a").Any(
                e => (e.GetAttribute("href") ?? "").Contains("go.microsoft.com/fwlink")); 

            return !nameNotResolved && !iisPage;
        }

        [When(@"I create '(.*)' with the command tool")]
        public void WhenICreateWithTheCommandTool(string siteName)
        {
            RunSimCommand($"install --name {siteName}");
        }
        

        [Then(@"I can navigate to '(.*)'")]
        public void ThenICanNavigateTo(string siteName)
        {
            Assert.IsTrue(SiteFound(siteName));
        }

        [Then(@"I see the Sitecore Welcome page")]
        public void ThenISeeTheSitecoreWelcomePage()
        {
            IWebElement element = driver.FindElement(By.TagName("h1"));
            Assert.AreEqual("Sitecore Experience Platform", element.Text);
        }

        [Then(@"Delete '(.*)'")]
        public void ThenDelete(string siteName)
        {
            RunSimCommand($"delete --name {siteName}");
            Assert.IsFalse(SiteFound(siteName));
        }

        #region Private Methods
        private static void RunSimCommand(string arguments)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = $@"{Environment.CurrentDirectory}\..\Sim.Client\bin\SIM.exe";
            p.StartInfo.Arguments = arguments;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            Console.WriteLine("Command output:");
            Console.WriteLine(output);
        }
        #endregion

    }
}
