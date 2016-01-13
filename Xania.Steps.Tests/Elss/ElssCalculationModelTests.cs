using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests.Elss
{
    public class ElssCalculationModelTests
    {
        [Test]
        public void Test()
        {
            var calcC = Calc.Select((Assessment a) => a.A + a.B);
            var calcD = (calcC + Calc.Select((Assessment a) => a.B));
            var calcE = (calcD - Calc.Select((Assessment a) => a.B));

            var result = calcE.Execute(new Assessment
            {
                A = 1,
                B = 2
            });

            result.Should().Be(3);
        }

        [Test]
        public void Test2()
        {
            var calcA = Calc.Select((Assessment a) => new AssessmentOutput { A = 1 });
            var calcB = Calc.Select((Assessment a) => new AssessmentOutput { B = a.B });

            var calc = calcA & calcB;

            var result = calc.Execute(new Assessment { A = 1, B = 2 });
            result.A.Should().Be(1);
            result.B.Should().Be(2);
        }

        [Test]
        public void Test3()
        {
            var id = Calc.Id<Assessment>();

            var calcA = id.Select(a => new AssessmentOutput { A = a.A });
            var calcB = id.Select(a => new AssessmentOutput { B = a.B });

            var calc = calcA & calcB;

            var result = calc.Execute(new Assessment { A = 1, B = 2 });
            result.A.Should().Be(1);
            result.B.Should().Be(2);
        }

        [Test]
        public void Test4()
        {
            var id = Calc.Id<Assessment>();
            var calcA = Calc.Constant(() => new AssessmentOutput { A = 1 });
            var calcMd = id.Select(a => new AssessmentOutput { Md = a.B });

            var calc = calcMd.Merge(calcA);

            var result = calc.Execute(new Assessment { B = 2 });
            result.A.Should().Be(1);
            result.Md.Should().Be(2m);

            //var calc = 
            //    from a in calcA
            //    join b in calcFee on a.Id equals b.Id
            //    select a;
        }

        [Test]
        public void Should_combine_results()
        {
            var productAgreementFunc = 
                from pa in Function.Id<ProductAgreement>()
                select new ProductAgreementOutput
                {
                    DoubleObligo = pa.Obligo * 2
                };

            var fullPath = Function.Query<Assessment>().Select(e => e.ProductAgreements).Select(e => e.ElementType);
            var q = 
                from i in Function.Id<Assessment>()
                from pa in i.ProductAgreements

                select new AssessmentOutput
                {
                    ProductAgreements = {i}
                };

            var result = q.Execute(new Assessment
            {
                ProductAgreements = { new ProductAgreement {  Obligo = 2 } }
            });

            result.SelectMany(e => e.ProductAgreements).Select(e => e.DoubleObligo).Should().BeEquivalentTo(4m);
        }

        private class Assessment
        {
            public Assessment()
            {
                ProductAgreements = new List<ProductAgreement>();
            }
            public decimal A { get; set; }
            public decimal B { get; set; }
            public decimal? C { get; set; }
            public decimal D { get; set; }

            public ICollection<ProductAgreement> ProductAgreements { get; private set; }
        }

        class ProductAgreement
        {
            public decimal Obligo { get; set; }
        }

        internal class ProductAgreementOutput
        {
            public decimal Obligo { get; set; }
            public decimal DoubleObligo { get; set; }
        }

        private class MasterData
        {
            public decimal Fee { get; set; }
        }

        internal class AssessmentOutput
        {
            public AssessmentOutput()
            {
                ProductAgreements = new List<ProductAgreementOutput>();
            }
            public int Id { get; set; }
            public decimal A { get; set; }
            public decimal B { get; set; }
            public object Md { get; set; }

            public override string ToString()
            {
                return String.Format(@"{{ A = {0}, B = {1} }}", A, B);
            }

            public ICollection<ProductAgreementOutput> ProductAgreements { get; private set; }
        }
    }


/*
 * C = A + B
 * D = C + D
 */

}