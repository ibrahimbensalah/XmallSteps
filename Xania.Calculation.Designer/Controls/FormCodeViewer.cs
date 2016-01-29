using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xania.Calculation.Designer.Controls
{
    public partial class FormCodeViewer : Form
    {
        public FormCodeViewer()
        {
            InitializeComponent();
        }

        public string Code
        {
            get { return this.richTextBox1.Text; }
            set { this.richTextBox1.Text = value; }
        }
    }
}
