using System;
using System.IO;

namespace Xania.Steps.Design
{
    public class NodeComponent: ICalculationComponent
    {
        public NodeComponent(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

        public string InputType { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }

    public interface ICalculationCodeGenerator
    {
        void Generate(ICalculationComponent component, TextWriter writer);
    }

    public static class CalculationCodeGeneratorExtensions
    {
        public static string Generate(this ICalculationCodeGenerator codeGenerator, ICalculationComponent component)
        {
            var writer = new StringWriter();
            codeGenerator.Generate(component, writer);

            return writer.ToString();
        }
    }

    public interface ICalculationComponent
    {
    }
}
