using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xania.Calculation.Designer.Code;
using Xania.Calculation.Designer.Components;
using Xania.Calculation.Designer.Controls;

namespace Xania.Calculation.Designer
{
    public partial class FormCalculationEngine : Form
    {
        private readonly ICalculationEngine _engine;

        public FormCalculationEngine(ICalculationEngine engine)
        {
            _engine = engine;
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

            const string newLine = "\r\n  ";
            var codeBlocks = designerControl1.Items.OfType<NodeComponent>()
                .Where(node => !string.IsNullOrEmpty(node.Name))
                .Select(node => string.Format("let {0} = {1}{2}", node.Name, newLine, generator.GenerateCode(node, newLine)));

            var viewer = new FormCodeViewer
            {
                Code = string.Join("\r\n", codeBlocks)
            };

            viewer.Show(this);

        }

        private void executeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                var writer = new StringWriter();

                foreach (var item in designerControl1.Items)
                {
                    var json = _engine.Run(item);
                    writer.WriteLine(json);
                }

                var viewer = new FormCodeViewer
                {
                    Code = writer.ToString()
                };

                viewer.Show(this);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, @"engine error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public interface ICalculationEngine
    {
        string Run(ITreeComponent item);
    }
}
