using AventStack.ExtentReports;
using BackEndAutomation.Rest.Calls;
using BackEndAutomation.Rest.DataManagement;
using NUnit.Framework;
using Reqnroll;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEndAutomation.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackEndAutomation.Tests.BBDTests
{
    [Binding]
    public class DemoStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private ExtentTest _test;
        private List<int> numbers = new List<int>();

        public DemoStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _test = scenarioContext.Get<ExtentTest>("ExtentTest");
        }

        [Given("open the calculator")]
        public void GivenOpenTheCalculator()
        {
            //Console.WriteLine("Calculator is open");
            _test.Log(Status.Info, "Calculator is open");
        }

        [When("click {int} number")]
        public void WhenClickNumber(int number)
        {
            numbers.Add(number);
            //Console.WriteLine("Click on number: " + number);
            _test.Log(Status.Info, "Click on number: " + number);
        }

        [When("click + action")]
        public void WhenClickSumAction()
        {
            Console.WriteLine("Click on + action on calculator");
        }

        [When("click = action")]
        public void WhenClickEndAction()
        {
            Console.WriteLine("Click on = action on calculator");
        }

        [Then("the calculator shows \"(.*)\"")]
        public void ThenTheCalculatorShows(string p0)
        {
            int sum = numbers.Sum();
            //Assert.That(sum.ToString, Is.EqualTo(p0), "The sum is not as expected, please check the input values");
            UtilitiesMethods.AssertEqual(
                actual: sum.ToString(),
                expected: p0,
                message: "The sum is not as expected, please check the input values",
                scenarioContext: _scenarioContext);
        }

        [Then("the calculator shows \"(.*)\", or show the \"(.*)\" error message")]
        public void ThenTheCalculatorShows(string number, string errorMessage)
        {
            int sum = numbers.Sum();
            string sumString = sum.ToString();
            //Assert.That(sum.ToString, Is.EqualTo(number), errorMessage.Replace("REPLACEWITHACTUALSUM",sumString));
            UtilitiesMethods.AssertEqual(
                actual: sum.ToString(),
                expected: number,
                message: errorMessage.Replace("REPLACEWITHACTUALSUM",sumString),
                scenarioContext: _scenarioContext);
        }

    }
}
