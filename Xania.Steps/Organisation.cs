﻿using System.Collections.Generic;
using System.Linq;

namespace Xania.Steps
{
    public class Organisation
    {
        public Organisation()
        {
            Persons = new List<Person>();
        }

        public ICollection<Person> Persons { get; private set; }
        public int TotalAge { get; set; }

        public void Init()
        {
            this.TotalAge = Persons.Sum(p => p.Age);
        }
    }
}