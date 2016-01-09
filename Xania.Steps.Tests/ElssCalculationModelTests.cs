using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Xania.Steps.Core;

namespace Xania.Steps.Tests
{
    public class ElssCalculationModelTests
    {
        [Test]
        public void Test()
        {
        }

        private class Assessment
        {
            public decimal A { get; set; }
            public decimal B { get; set; }
            public decimal C { get; set; }
            public decimal D { get; set; }
        }

        private class AssessmentCalculation : Calculation<Assessment>
        {
            public AssessmentCalculation(Expression<Func<Assessment, decimal>> expression) : base(expression)
            {
            }

            public AssessmentCalculation Test()
            {
                var calcC = Unit(a => a.A + a.B).Set(a => a.C);
                var calcD = (calcC + Unit(a => a.B)).Set(a => a.D);

                var calcAll = calcC & calcD;

                var query =
                    from a in Function.Query<Assessment>()
                    select calcAll.Execute(a);

                return null;
            }
        }

        private struct MasterData
        {
        }
    }

    public class Calculation<TInput>: IFunction<TInput, decimal>
    {
        protected Calculation(Expression<Func<TInput, decimal>> expression)
        {
        }

        public static Calculation<TInput> Unit(Expression<Func<TInput, decimal>> expression)
        {
            return new Calculation<TInput>(expression);
        }

        public static Calculation<TInput> operator +(Calculation<TInput> calc, decimal value)
        {
            return calc;
        }

        public static Calculation<TInput> operator &(Calculation<TInput> calc, Calculation<TInput> value)
        {
            throw new NotImplementedException();
        }

        public static Calculation<TInput> operator +(Calculation<TInput> calc, Calculation<TInput> value)
        {
            return new Calculation<TInput>(a => calc.Execute(a) + value.Execute(a));
        }

        public Calculation<TInput> Set<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression)
        {
            throw new NotImplementedException();
        }

        public decimal Execute(TInput root)
        {
            throw new NotImplementedException();
        }
    }
}

/*
 * C = A + B
 * D = C + D
 */

