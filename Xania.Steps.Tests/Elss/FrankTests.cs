using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests.Elss
{
    public class FrankTests
    {
        [Test]
        public void HandlingFeeTest()
        {
            var md = new MasterDataRepository();

            var creditFeePercentageFunc = Calc.Constant(() => new TariffProposalResult
            {
                CreditFeePercentage = md.GetCreditFee(),
                CreditFeeOnObligoPercentage = md.GetCreditFeeOnObligo(),
                AdvicedCreditFeePercentage = md.GetCreditFee() + md.GetCreditFeeOnObligo()
            });

            var advicedCreditFeeFunc =
                from relation in Function.Query<CommercialRelation>()
                from pa in relation.ProductAgreements
                select creditFeePercentageFunc & Calc.Constant(() => new TariffProposalResult
                {
                    AdvicedCreditFeeAmount = pa.ObligoAmount + md.GetCreditFee() + md.GetCreditFeeOnObligo()
                });

            var result = advicedCreditFeeFunc.Execute(new CommercialRelation() {ProductAgreements = {new ProductAgreement()}}).Single().Execute(Unit.Any);

            result.CreditFeeOnObligoPercentage.Should().Be(2);
            result.CreditFeePercentage.Should().Be(1);
            result.AdvicedCreditFeePercentage.Should().Be(3);
            result.AdvicedCreditFeeAmount.Should().Be(3);

            var q =
                from a in (new [] { new CommercialRelation() }).AsQueryable()
                from pa in a.ProductAgreements
                select new TariffProposalResult
                {
                    AdvicedCreditFeeAmount = 123
                };

            Console.WriteLine(q.Expression);
        }

        class Assessment
        {
            public ClientGroup ClientGroup { get; set; }
        }
        class ClientGroup
        {
            public IList<CommercialRelation> CommercialRelations { get; set; }
        }

        class CommercialRelation
        {
            public CommercialRelation()
            {
                ProductAgreements = new List<ProductAgreement>();
            }
            public IList<ProductAgreement> ProductAgreements { get; private set; }
            public PDDeterminingClient PDDeterminingClient { get; set; }
        }

        class PDDeterminingClient
        {
            public decimal CreditObligoAmount { get; set; }
            public string ExpectedLossClass { get; set; }
        }

        class ProductAgreement
        {
            List<TariffProposal> TariffProposals { get; set; }
            public decimal ObligoAmount { get; set; }
        }

        class TariffProposal
        {
            public ProductAgreement ProductAgreement { get; set; }
        }

        class TariffProposalResult
        {
            public decimal CreditFeePercentage { get; set; }
            public decimal CreditFeeOnObligoPercentage { get; set; }
            public decimal AdvicedCreditFeePercentage { get; set; }
            public decimal AdvicedCreditFeeAmount { get; set; }
            public ProductAgreement PA { get; set; }
        }
    }

    public class MasterDataRepository
    {
        public decimal GetCreditFeePercentage(decimal creditObligoAmount, string expectedLossClass)
        {
            return creditObligoAmount;
        }

        public decimal GetCreditFee()
        {
            return 1;
        }

        public decimal GetCreditFeeOnObligo()
        {
            return 2;
        }
    }
}
