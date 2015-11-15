using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Xania.Steps.Design
{
    public class StepComponent: IStepComponent
    {
        public StepComponent(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

        public string InputType { get; set; }

        public string OutputType { get; set; }

        public StepComponent Output { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0}<{1}, {2}>()", Name, InputType, OutputType);
        }

        public void Accept(IStepVisitor stepVisitor)
        {
        }
    }

    public interface IStepCodeGenerator
    {
        void Generate(IStepComponent component, TextWriter writer);
    }

    public class CsharpStepCodeGenerator : IStepCodeGenerator
    {
        public void Generate(IStepComponent component, TextWriter writer)
        {
            new StepVisitor(this, writer).Visit(component);
        }

        public void Visit(StepComponent component)
        {
        }

        class StepVisitor : IStepVisitor
        {
            private readonly IStepCodeGenerator _codeGenerator;
            private readonly TextWriter _writer;

            public StepVisitor(IStepCodeGenerator codeGenerator, TextWriter writer)
            {
                _codeGenerator = codeGenerator;
                _writer = writer;
            }

            public void Visit(IStepComponent component)
            {
                component.Accept(this);
            }

            public void Call(string method)
            {
                _writer.Write(".{0}()", method);
            }
        }
    }

    public interface IStepVisitor
    {
        void Visit(IStepComponent component);
        void Call(string method);
    }

    public static class StepCodeGeneratorExtensions
    {
        public static string Generate(this IStepCodeGenerator codeGenerator, IStepComponent component)
        {
            var writer = new StringWriter();
            codeGenerator.Generate(component, writer);

            return writer.ToString();
        }
    }

    public class ComposeComponent : IStepComponent, IEnumerable<IStepComponent>
    {
        private readonly ICollection<IStepComponent> _items;

        public ComposeComponent()
        {
            _items = new List<IStepComponent>();
        }

        public void Accept(IStepVisitor visitor)
        {
            foreach (var item in _items)
                visitor.Visit(item);
        }

        public void Add(IStepComponent component)
        {
            _items.Add(component);
        }

        public IEnumerator<IStepComponent> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("");
        }
    }

    public class RootComponent : IStepComponent
    {
        private readonly string _type;

        public RootComponent(string type)
        {
            _type = type;
        }

        public void Accept(IStepVisitor stepVisitor)
        {
            stepVisitor.Call(string.Format("Root<{0}>", _type));
        }
    }

    public class SelectComponent : IStepComponent
    {
        public string Member { get; set; }

        public void Accept(IStepVisitor stepVisitor)
        {
            stepVisitor.Call("Select");
        }

        public override string ToString()
        {
            return string.Format("Select(r => r.{0})", this.Member);
        }
    }

    public class ForEachComponent : IStepComponent
    {
        public void Accept(IStepVisitor stepVisitor)
        {
            stepVisitor.Call("ForEach");
        }
    }

    public interface IStepComponent
    {
        void Accept(IStepVisitor stepVisitor);
    }
}
