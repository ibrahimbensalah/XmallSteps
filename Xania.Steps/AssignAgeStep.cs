namespace Xania.Steps
{
    public class AssignAgeStep : Step<Person, Person>
    {
        private readonly int _age;

        public AssignAgeStep(int age)
        {
            _age = age;
        }

        public override void Execute(Person model, IStepVisitor<Person> stepVisitor)
        {
            model.Age = _age;
            stepVisitor.Visit(model);
        }
    }
}