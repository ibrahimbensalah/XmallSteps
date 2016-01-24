using System.Linq;
using System.Windows.Forms;

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
    }
}
