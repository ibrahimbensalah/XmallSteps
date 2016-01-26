using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xania.Calculation.Designer.Components;

namespace Xania.Calculation.Designer.Controls
{
    public partial class ToolBoxControl : UserControl
    {
        public ToolBoxControl()
        {
            InitializeComponent();
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is ListViewItem)
            {
                var listViewItem = e.Item as ListViewItem;

                if (!string.IsNullOrEmpty(listViewItem.Text))
                    DoDragDrop(new DragItem(listViewItem.Text), DragDropEffects.Copy);
            }
        }
    }

    internal class DragItem
    {
        public string Type { get; private set; }

        public DragItem(string type)
        {
            Type = type;
        }
    }
}
