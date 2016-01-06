using System;
using System.Collections;
using System.Collections.Generic;

namespace Xania.Steps
{
    public class Person
    {
        public Person()
        {
            Grades = new List<int>();
        }

        public int Id { get; set; }

        public int Age { get; set; }

        public ICollection<int> Grades { get; private set; }

        public int GetAge()
        {
            return Age;
        }
    }
}