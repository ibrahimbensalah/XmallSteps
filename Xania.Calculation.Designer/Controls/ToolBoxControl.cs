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

                var item = CreateTreeComponent(listViewItem.Text);
                if (item != null)
                    DoDragDrop(new DragItem(item), DragDropEffects.Copy);
            }
        }

        private ITreeComponent CreateTreeComponent(string text)
        {
            switch (text)
            {
                case "Leaf":
                    return new LeafComponent { };
                case "Node":
                    return new NodeComponent { };
                default:
                    return null;
            }
        }
    }

    internal class DragItem
    {
        public ITreeComponent Tree { get; private set; }

        public DragItem(ITreeComponent tree)
        {
            Tree = tree;
        }
    }
}
