using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.Steps.Monads
{
    public static class NullableMonadExtensions
    {

        public static void Test()
        {
            
        }
        public static Task<T> Unit<T>(this T value)
        {
            return Task.FromResult(value);
        }
    }
}
