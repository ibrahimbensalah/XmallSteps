using System;
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
            var creditFee = Calc.Select((TariffProposal a) => a.A + a.B);
            var calcD = (creditFee + Calc.Select((TariffProposal a) => a.B));
            var calcE = (calcD - Calc.Select((TariffProposal a) => a.B));

            var result = calcE.Execute(new TariffProposal
            {
                A = 1,
                B = 2
            });

            result.Should().Be(3);
        }

        [Test]
        public void Test2()
        {
            var calcA = Calc.Select((TariffProposal a) => new TarifProposalOutput { A = 1 });
            var calcB = Calc.Select((TariffProposal a) => new TarifProposalOutput { B = a.B });

            var calc = calcA & calcB;

            var result = calc.Execute(new TariffProposal { A = 10, B = 2 });
            result.A.Should().Be(1);
            result.B.Should().Be(2);
        }

        [Test]
        public void Test3()
        {
            var id = Calc.Id<TariffProposal>();

            var calcA = id.Select(a => new TarifProposalOutput { A = a.A });
            var calcB = id.Select(a => new TarifProposalOutput { B = a.B });

            var calc = calcA & calcB;

            var result = calc.Execute(new TariffProposal { A = 1, B = 2 });
            result.A.Should().Be(1);
            result.B.Should().Be(2);
        }

        [Test]
        public void Test4()
        {
            var id = Calc.Id<TariffProposal>();
            var calcA = Calc.Constant(() => new TarifProposalOutput { A = 1 });
            var calcMd = id.Select(a => new TarifProposalOutput { Md = a.B });

            var calc = calcMd.SelectMany(calcA);

            var result = calc.Execute(new TariffProposal { B = 2 });
            result.A.Should().Be(1);
            result.Md.Should().Be(2m);
        }

        [Test]
        public void Should_pack_result()
        {
            var productAgreementFunc = 
                from a in Function.Id<TariffProposal>()
                from pa in a.NestedInputs
                select new NestedOutput
                {
                    DoubleObligo = pa.Obligo * 2
                };

            var productAgreementPack = 
                from productAgreementOutputs in Function.Query<TariffProposal>().Select(productAgreementFunc)
                select new TarifProposalOutput
                {
                    ProductAgreements = productAgreementOutputs
                };

            var result = productAgreementPack.Execute(new TariffProposal
            {
                NestedInputs = { new ProductAgreement { Obligo = 2 } }
            });

            result.SelectMany(e => e.ProductAgreements)
                .Select(e => e.DoubleObligo).Should().BeEquivalentTo(4m);
        }

        private class TariffProposal
        {
            public TariffProposal()
            {
                NestedInputs = new List<ProductAgreement>();
            }
            public decimal A { get; set; }
            public decimal B { get; set; }
            public decimal? C { get; set; }
            public decimal D { get; set; }

            public ICollection<ProductAgreement> NestedInputs { get; private set; }
        }

        class ProductAgreement
        {
            public decimal Obligo { get; set; }
        }

        internal class NestedOutput
        {
            public decimal Obligo { get; set; }
            public decimal DoubleObligo { get; set; }
        }

        private class MasterData
        {
            public decimal Fee { get; set; }
        }

        internal class TarifProposalOutput
        {
            public TarifProposalOutput()
            {
                ProductAgreements = new List<NestedOutput>();
            }
            public int Id { get; set; }
            public decimal A { get; set; }
            public decimal B { get; set; }
            public object Md { get; set; }

            public override string ToString()
            {
                return String.Format(@"{{ A = {0}, B = {1}, Md = {2} }}", A, B, Md);
            }

            public IEnumerable<NestedOutput> ProductAgreements { get; set; }
        }
    }


/*
 * C = A + B
 * D = C + D
 */

}