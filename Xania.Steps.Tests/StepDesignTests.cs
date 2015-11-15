using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Design;

namespace Xania.Steps.Tests
{
    public class StepDesignTests
    {
        private CsharpStepCodeGenerator _generator;

        [SetUp]
        public void Setup()
        {
            _generator = new CsharpStepCodeGenerator();
        }

        [Test]
        public void ComponentTest()
        {
            var component = new StepComponent("Step")
            {
                InputType = "Person",
                OutputType = "Int32"
            };

            _generator.Generate(component).Should().Be(".Step<Person, Int32>()");
        }

        [Test]
        public void ComposeComponentTest()
        {
            var component = new ComposeComponent()
            {
                new RootComponent("Organisation"),
                new SelectComponent()
                {
                    Member = "Persons"
                },
                new ForEachComponent()
                {
                }
            };

            _generator.Generate(component).Should().Be(".Root<Organisation>().Select(r => r.Persons).ForEach(r => r)");
        }
    }
}
