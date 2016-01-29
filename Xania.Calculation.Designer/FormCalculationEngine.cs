using System.Linq;
using System.Windows.Forms;
using Xania.Calculation.Designer.Code;
using Xania.Calculation.Designer.Components;
using Xania.Calculation.Designer.Controls;

namespace Xania.Calculation.Designer
{
    public partial class FormCalculationEngine : Form
    {
        public FormCalculationEngine()
        {
            InitializeComponent();
        }

        private void FormCalculationEngine_Load(object sender, System.EventArgs e)
        {
            designerControl1.SelectionChanged += designerControl1_SelectionChanged;
        }

        void designerControl1_SelectionChanged(object sender, Components.ITreeComponent[] e)
        {
            propertyGrid1.SelectedObjects = e.OfType<object>().ToArray();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            designerControl1.Invalidate();
        }

        private void generateCodeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var generator = new CalculationCodeGenerator();

            var codeBlocks = designerControl1.Items.OfType<NodeComponent>()
                .Where(node => !string.IsNullOrEmpty(node.Name))
                .Select(node => generator.GenerateCode(node));

            var viewer = new FormCodeViewer
            {
                Code = string.Join("\r\n", codeBlocks)
            };

            viewer.Show(this);

        }
    }
}
