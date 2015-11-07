namespace Xania.Steps
{
    public class AssignAgeStep : Step<Person>
    {
        private readonly int _age;

        public AssignAgeStep(int age)
        {
            _age = age;
        }

        public override void Execute(Person model)
        {
            model.Age = _age;
        }
    }
}