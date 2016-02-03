using System.IO;
using System.Text;
using Microsoft.FSharp.Compiler.Interactive;
using Microsoft.FSharp.Core;

namespace Xania.Calculation.Designer.Interactive
{
    public class InteractiveService
    {
        public void Execute(string expression, StringWriter output)
        {
            var sbOut = new StringBuilder();
            var sbErr = new StringBuilder();
            var inStream = new StringReader("");
            var outStream = new StringWriter(sbOut);
            var errStream = new StringWriter(sbErr);

            // Build command line arguments & start FSI session
            var allArgs = new[] { "C:\\fsi.exe", "--noninteractive" };

            var fsiConfig = Shell.FsiEvaluationSession.GetDefaultConfiguration();

            FSharpOption<bool> collectible = new FSharpOption<bool>(false);
            var fsiSession = Shell.FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream, collectible);
        }
    }
}