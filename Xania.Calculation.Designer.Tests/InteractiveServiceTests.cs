using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Xania.Calculation.Designer.Components;
using Xania.Calculation.Designer.Interactive;

namespace Xania.Calculation.Designer.Tests
{
    public class InteractiveServiceTests
    {
        private InteractiveService _interactiveService;

        [SetUp]
        public void StartInteractive()
        {
            _interactiveService = new InteractiveService();
        }

        [Test]
        public void HelloWorldTest()
        {
            // arrange
            var output = new StringWriter();

            // act
            _interactiveService.Execute("printfn \"Hello Interactive!\"", output);

            // assert
            output.Should().Be("Hello Interactive!");
        }
    }
}
