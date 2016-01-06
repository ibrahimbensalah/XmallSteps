﻿using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Effort;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests
{
    public class EntityFrameworkTests
    {
        [Test]
        public void ShouldSendSingleQueryTest()
        {
            // arrange
            var connection = DbConnectionFactory.CreateTransient();
            var context = new MyDbContext(connection);
            context.Organisations.Add(new Organisation
            {
                Persons = { new Person { Age = 40 } }
            });
            context.SaveChanges();

            // arrange
            var getAges = from o in Function.Query<Organisation>()
                from p in o.Persons
                where p.Age >= 18
                select p.Age;

            context.Database.Log += Console.Out.WriteLine;

            getAges.Execute(context.Organisations).Should().BeEquivalentTo(40);
        }

        class MyDbContext : DbContext
        {
            public MyDbContext(DbConnection conn)
                : base(conn, true)
            {
            }

            public IDbSet<Person> People { get; set; }

            public IDbSet<Organisation> Organisations { get; set; }
        }
    }
}
