using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathfindingVisualizer
{
    public partial class Form : System.Windows.Forms.Form
    {
        private Scene Scene { get; set; }
        private bool IsMouseClick { get; set; }

        public Form()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Scene = new Scene(ClientSize.Width, ClientSize.Height - toolStrip1.Height);
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Scene.Draw(e.Graphics);
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            Scene.UpdateSize(ClientSize.Width, ClientSize.Height - toolStrip1.Height);
            Invalidate();
        }

        private void Form_MouseClick(object sender, MouseEventArgs e)
        {
            Scene.Click(e.Location, e.Button == MouseButtons.Left);
            Invalidate();
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseClick = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseClick)
            {
                Scene.Click(e.Location, e.Button == MouseButtons.Left);
                Invalidate();
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseClick = false;
        }

        private void NewFile()
        {
            throw new NotImplementedException();
        }

        private void SaveFile(bool IsSaveAs)
        {
            throw new NotImplementedException();
        }

        private void OpenFile()
        {
            throw new NotImplementedException();
        }

        private void tssBtnDFS_Click(object sender, EventArgs e)
        {
            Scene.DFS(this);
        }

        private void tsBtnBFS_Click(object sender, EventArgs e)
        {
            Scene.BFS(this);
        }
    }
}
