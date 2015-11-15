namespace Xania.Steps
{
    public class PersonAgeStep : Step<Person, int>
    {
        public override void Execute(Person model, IStepVisitor<int> stepVisitor)
        {
            stepVisitor.Visit(model.Age);
        }
    }
}