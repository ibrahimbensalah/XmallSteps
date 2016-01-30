using System.Collections.Generic;
using System.Linq;

namespace Xania.Calculation.Designer.Components
{
    public class DesignerHelper
    {
        public static string GetNewVariableName(string format, IEnumerable<string> existingNames)
        {
            var enumerable = existingNames as string[] ?? existingNames.ToArray();
            for (int i = 0; ; i++)
            {
                var n = string.Format(format, i);
                if (!enumerable.Contains(n))
                    return n;
            }
        }
    }
}