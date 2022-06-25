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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PathfindingVisualizer
{
    public partial class Form : System.Windows.Forms.Form
    {
        private Scene Scene { get; set; }
        private bool IsMouseClick { get; set; }
        private Point LastMouseDown { get; set; }

        private string FileName { get; set; } = "untitled";
        private bool IsModified { get; set; } = true;

        public Form()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Scene = new Scene(ClientSize.Width, ClientSize.Height - toolStrip1.Height);
            SetCaption();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Scene.Draw(e.Graphics);
            SetCaption();
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (Scene != null)
            {
                IsModified = true;
                Scene.UpdateSize(ClientSize.Width, ClientSize.Height - toolStrip1.Height);
                Invalidate();
            }
        }

        private void Form_MouseClick(object sender, MouseEventArgs e)
        {
            IsModified = true;
            Scene.Click(e.Location, e.Button == MouseButtons.Left, LastMouseDown);
            Invalidate();
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            LastMouseDown = e.Location;
            IsMouseClick = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseClick)
            {
                IsModified = true;
                Scene.Click(e.Location, e.Button == MouseButtons.Left, LastMouseDown);
                Invalidate();
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseClick = false;
            Scene.IsStartCellSelected = false;
            Scene.IsEndCellSelected = false;
        }

        private void SetCaption()
        {
            Text = string.Format("Pathfinding Visualizer - {0}{1}", FileName, IsModified ? "*" : "");
        }

        private void AskToSave()
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to save last changes?", "Save the current visualization", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                SaveFile(false);
        }

        private void NewFile()
        {
            if (IsModified)
                AskToSave();

            Scene = new Scene(ClientSize.Width, ClientSize.Height - toolStrip1.Height);
            tssLblReport.Text = "";
            nudCellSize.Value = Scene.CellSize;
            FileName = "untitled";
            SetCaption();
            Invalidate();
        }

        private void SaveFile(bool IsSaveAs)
        {
            if (FileName == "untitled" || IsSaveAs)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pathfinding Visualizer (*.pathvis)|*.pathvis";
                saveFileDialog.Title = "Save Pathfinding Visualization";
                saveFileDialog.FileName = FileName;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    FileName = saveFileDialog.FileName;
            }

            if (FileName != null)
            {
                using (FileStream fileStream = new FileStream(FileName, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, Scene);
                }

                IsModified = false;
                SetCaption();
            }
        }

        private void OpenFile()
        {
            if (IsModified)
                AskToSave();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pathfinding Visualizer (*.pathvis)|*.pathvis";
            openFileDialog.Title = "Open Pathfinding Visualization";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = FileName;
                FileName = openFileDialog.FileName;
                try
                {
                    using (FileStream fileStream = new FileStream(FileName, FileMode.Open))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        Scene = (Scene)formatter.Deserialize(fileStream);
                    }

                    SetCaption();
                    Invalidate();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Could not read file: " + FileName);
                    FileName = fileName;
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(true);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void tsBtnBFS_Click(object sender, EventArgs e)
        {
            IsModified = true;
            Enabled = false;
            Scene.BFS(this, tssLblReport);
            Enabled = true;
        }

        private void tsBtnClear_Click(object sender, EventArgs e)
        {
            Scene.ClearVisitedAndPathFlags();
            tssLblReport.Text = "";
            Invalidate();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsModified)
                AskToSave();
        }

        private void nudCellSize_ValueChanged(object sender, EventArgs e)
        {
            IsModified = true;
            Scene.UpdateCellSize((int)nudCellSize.Value);
            Invalidate();
        }

        private void runBtn_Click(object sender, EventArgs e)
        {
            IsModified = true;
            Enabled = false;
            if (cbAlgorithm.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an algorithm", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(cbAlgorithm.SelectedIndex == 0)
            {
                Scene.BFS(this, tssLblReport);
            }
            else if(cbAlgorithm.SelectedIndex == 1)
            {
                Scene.DFS(this, tssLblReport);
            }
            else if (cbAlgorithm.SelectedIndex == 2)
            {
                Scene.Greedy(this, tssLblReport);
            }
            else if (cbAlgorithm.SelectedIndex == 3)
            {
                Scene.AStar(this, tssLblReport);
            }
            Enabled = true;
        }

        private void instantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Scene.GenerateMaze(this, false);
        }
        private void showAlgorithmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Scene.GenerateMaze(this, true);
        }

        private void tsBtnEmpty_Click(object sender, EventArgs e)
        {
            Scene.GenerateCells();
            Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
