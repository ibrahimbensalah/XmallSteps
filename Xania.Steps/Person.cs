using System;
using System.Collections;
using System.Collections.Generic;

namespace Xania.Steps
{
    public class Person
    {
        public int Age { get; set; }

        public ICollection<int> Grades { get; set; }

        public int GetAge()
        {
            return Age;
        }
    }
}