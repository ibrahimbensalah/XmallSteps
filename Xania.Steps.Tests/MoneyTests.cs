using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests
{
    public class MoneyTests
    {
        [Test]
        public void PersonSalaryTest()
        {
            var person = Function.Id<Person>();
            var m = person.Money(p => p.Salary) + person.Money(p => p.Salary);

            m.Execute(new Person { Salary = 123 }).Should().Be(123 + 123);
        }

        public void LinqSalaryTest()
        {
            var organisations = Function.Each<Organisation>();

            var q =
                from o in organisations
                from m in o.Employees.Where(e => e.Age >= 18).Select(e => e.Salary)
                select new EmployeeViewModel
                {
                    Salary = m
                };


        }
    }

    public class EmployeeRequest
    {
        public int OrganisationId { get; set; }
    }
    public class EmployeeViewModel
    {
        public decimal Salary { get; set; }
    }
}
