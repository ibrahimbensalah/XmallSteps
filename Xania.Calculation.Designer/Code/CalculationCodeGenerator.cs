using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Code
{
    public class CalculationCodeGenerator
    {
        public string GenerateCode(LeafComponent leaf)
        {
            if (leaf.IsIdentityCall)
                return leaf.Type.ToString();
            if (leaf.IsFunctionCall)
                return string.Format("Leaf {0}", leaf.Fun);

            return string.Format("Leaf (fun input -> {0})", leaf.Fun);
        }

        public string GenerateCode(NodeComponent node, string newLine)
        {
            if (newLine == null)
                return "Node [ " + string.Join(" ; ",
                    node.Arguments.OfType<Branch>().Select(n => GenerateCode(n, null))) + " ]";
            
            return "Node [ " + newLine +"  "+ string.Join(newLine + "  ",
                node.Arguments.OfType<Branch>().Select(n => GenerateCode(n, newLine + "  "))) + newLine + "]";
        }

        public string GenerateCode(ConcatComponent node, string newLine)
        {
            return "concat [ " + newLine + string.Join(" ; ",
                node.Select(n => GenerateCode(n, newLine + "  "))) + " ]";
        }

        public string GenerateCode(Branch b, string newLine)
        {
            if (string.IsNullOrEmpty(b.Path))
                return string.Format("(\"{0}\", {1})", b.Name, GenerateCode(b.Tree, newLine)); ;

            return string.Format("(\"{0}\", {1} {2} {3} )", b.Name,
                b.Many ? "mapTrees" : "mapTree", b.Path, GenerateCode(b.Tree, newLine)); ;
        }

        public string GenerateCode(ITreeComponent tree, string newLine)
        {
            if (tree is LeafComponent)
                return GenerateCode(tree as LeafComponent);
            if (tree is NodeComponent)
                return GenerateCode(tree as NodeComponent, newLine);
            if (tree is TreeRefComponent)
                return GenerateCode(tree as TreeRefComponent);

            throw new NotSupportedException("type is not supported " + tree.GetType());
        }

        private string GenerateCode(TreeRefComponent treeRefComponent)
        {
            return treeRefComponent.Name;
        }
    }
}