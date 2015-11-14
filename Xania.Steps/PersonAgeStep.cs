namespace Xania.Steps
{
    public class PersonAgeStep : Step<Person, int>
    {
        public override int Execute(Person model)
        {
            return model.Age;
        }

        public override void Execute(Person model, IStepVisitor<int> stepVisitor)
        {
            stepVisitor.Visit(model.Age);
        }
    }
}