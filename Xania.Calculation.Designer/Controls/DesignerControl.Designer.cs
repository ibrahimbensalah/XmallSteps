namespace Xania.Calculation.Designer.Controls
{
    partial class DesignerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // DesignerControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DesignerControl";
            this.Size = new System.Drawing.Size(497, 460);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DesignerControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DesignerControl_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
