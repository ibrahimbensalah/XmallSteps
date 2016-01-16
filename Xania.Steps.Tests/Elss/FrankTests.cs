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
        public void GetAdvicedCreditAmountTest()
        {
            var tariffProposalCalculator = new TariffProposalCalculator();

            var result = tariffProposalCalculator.GetAdvicedCreditAmount(new ProductAgreement());

            result.Value.CreditFeeOnObligoPercentage.Should().Be(2);
            result.Value.CreditFeePercentage.Should().Be(1);
            result.Value.AdvicedCreditFeePercentage.Should().Be(3);
            result.Value.AdvicedCreditFeeAmount.Should().Be(3);
        }

        [Test]
        public void GetBankGaranteePercentageTest()
        {
            var calculator = new TariffProposalCalculator();

            var result = calculator.AdvicedBankGaranteeAmount(new ProductAgreement { ObligoAmount = 2 });

            result.Value.BankGaranteeOnObligoPercentage.Should().Be(2);
            result.Value.BankGaranteePercentage.Should().Be(1);
            result.Value.AdvicedBankGaranteePercentage.Should().Be(3);
            result.Value.AdvicedBankGaranteeAmount.Should().Be(6);
        }

        [Test]
        public void GetTariffProposalTest()
        {
            var calculator = new TariffProposalCalculator();
            var pa = new ProductAgreement {ObligoAmount = 2};

            var result = calculator.AdvicedBankGaranteeAmount(pa) & calculator.GetAdvicedCreditAmount(pa);

            result.Value.BankGaranteeOnObligoPercentage.Should().Be(2);
            result.Value.CreditFeeOnObligoPercentage.Should().Be(2);

            result.Value.AdvicedBankGaranteeAmount.Should().Be(6);
            result.Value.AdvicedCreditFeeAmount.Should().Be(5);
        }

        private class Assessment
        {
            public ClientGroup ClientGroup { get; set; }
        }

        private class ClientGroup
        {
            public IList<CommercialRelation> CommercialRelations { get; set; }
        }

        private class CommercialRelation
        {
            public CommercialRelation()
            {
                ProductAgreements = new List<ProductAgreement>();
            }

            public IList<ProductAgreement> ProductAgreements { get; private set; }
            public PDDeterminingClient PDDeterminingClient { get; set; }
        }

        private class PDDeterminingClient
        {
            public decimal CreditObligoAmount { get; set; }
            public string ExpectedLossClass { get; set; }
        }

        internal class ProductAgreement
        {
            private List<TariffProposal> TariffProposals { get; set; }
            public decimal ObligoAmount { get; set; }
            public decimal BankGaranteeCostsCorrectionAmount { get; set; }
        }

        private class TariffProposal
        {
            public ProductAgreement ProductAgreement { get; set; }
        }

        internal class TariffProposalResult
        {
            public decimal CreditFeePercentage { get; set; }
            public decimal CreditFeeOnObligoPercentage { get; set; }
            public decimal AdvicedCreditFeePercentage { get; set; }
            public decimal? AdvicedCreditFeeAmount { get; set; }
            public ProductAgreement PA { get; set; }
            public decimal BankGaranteePercentage { get; set; }
            public decimal BankGaranteeOnObligoPercentage { get; set; }
            public decimal AdvicedBankGaranteePercentage { get; set; }
            public decimal AdvicedBankGaranteeAmount { get; set; }
        }


        public class TariffProposalCalculator
        {
            public decimal GetCreditFeePercentage(decimal creditObligoAmount, string expectedLossClass)
            {
                return creditObligoAmount;
            }

            public decimal CreditFee()
            {
                return 1;
            }

            public decimal CreditFeeOnObligo()
            {
                return 2;
            }

            private decimal AdvicedCreditFeePercentage()
            {
                return CreditFee() + CreditFeeOnObligo();
            }

            internal Calc<TariffProposalResult> GetAdvicedCreditAmount(ProductAgreement pa)
            {
                return Calc.Create(() => new TariffProposalResult
                {
                    CreditFeePercentage = CreditFee(),
                    CreditFeeOnObligoPercentage = CreditFeeOnObligo(),
                    AdvicedCreditFeePercentage = AdvicedCreditFeePercentage(),
                    AdvicedCreditFeeAmount = IncludeAdvicedCreditFeeAmount() ? pa.ObligoAmount + AdvicedCreditFeePercentage() : (decimal?)null
                });
            }

            internal Calc<TariffProposalResult> AdvicedBankGaranteeAmount(ProductAgreement pa)
            {
                return Calc.Create(() => new TariffProposalResult
                {
                    BankGaranteePercentage = CreditFee(),
                    BankGaranteeOnObligoPercentage = CreditFeeOnObligo(),
                    AdvicedBankGaranteePercentage = AdvicedCreditFeePercentage(),
                    AdvicedBankGaranteeAmount = AdvicedCreditFeePercentage() * pa.ObligoAmount - pa.BankGaranteeCostsCorrectionAmount
                });
            }

            public bool IncludeAdvicedCreditFeeAmount()
            {
                return true;
            }
        }
    }
}
