using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps
{
    public class Organisation
    {
        public Organisation()
        {
            Persons = new List<Person>();
        }

        public int Id { get; set; }
        public ICollection<Person> Persons { get; private set; }
        public int TotalAge { get; set; }

        public void Init()
        {
            this.TotalAge = Persons.Sum(p => p.Age);
        }

        public override string ToString()
        {
            return "organisation";
        }
    }

    public class Contract
    {
        public Contract()
        {
            Organisations = new List<Organisation>();
        }
        public ICollection<Organisation> Organisations { get; private set; }
    }
}