using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using CsvHelper;
using CsvHelper.Configuration;
using Xania.Calculation.Designer.Controls;

namespace Xania.Calculation.Designer.Components
{
    internal class CsvRepositoryComponent : ITreeComponent
    {
        private Dictionary<string, string>[] _rows;

        public CsvRepositoryComponent(FileInfo file)
        {
            _rows = GetRows(file).ToArray();
            Layout = new ComponentLayout();
        }

        private IEnumerable<Dictionary<string, string>> GetRows(FileInfo file)
        {
            using (var stream = file.OpenText())
            {
                var parser = new CsvParser(stream, new CsvConfiguration{ Delimiter = ";"});
                Fields = parser.Read();
                string[] current;
                while ((current = parser.Read()) != null)
                {
                    var row = new Dictionary<string, string>();
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        row.Add(Fields[i], current[i]);
                    }
                    yield return row;
                }
                stream.Close();
            }
        }

        public ComponentLayout Layout { get; private set; }

        public bool Connect(ITreeComponent fromComponent)
        {
            return false;
        }

        public void Paint(Graphics graphics, Font font, Func<ITreeComponent, bool> isSelected)
        {
            var x = Layout.X - Images.db_10_128.Width / 2;
            var y = Layout.Y - Images.db_10_128.Height / 2;

            graphics.DrawImage(Images.db_10_128, new Point(x, y));
        }

        public void UnConnect(ITreeComponent treeComponent)
        {
        }

        public string[] Fields { get; private set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name ?? "{ repository }";
        }
    }
}