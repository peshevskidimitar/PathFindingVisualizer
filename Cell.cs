using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PathfindingVisualizer
{
    public class Cell
    {
        public Point TopLeft { get; set; }
        public Size Size { get; set; }
        public bool IsObstacle { get; set; }
        public bool IsStart { get; set; }
        public bool IsFinish { get; set; }
        public bool IsVisited { get; set; }
        public bool IsPath { get; set; }

        public Cell(Point topLeft, Size size)
        {
            TopLeft = topLeft;
            Size = size;
        }

        private Color ChooseColor()
        {
            if (IsStart)
                return Color.Green;
            else if (IsFinish)
                return Color.Red;
            else if (IsObstacle)
                return Color.Black;
            else if (IsPath)
                return Color.Purple;
            else if (IsVisited)
                return Color.LightGreen;
            else
                return Color.Gray;
        }

        public void Draw(Graphics graphics)
        {
            Brush brush = new SolidBrush(ChooseColor());
            graphics.FillRectangle(brush, TopLeft.X, TopLeft.Y, Size.Width, Size.Height);
            brush.Dispose();
        }

        public bool IsClicked(Point point)
        {
            if (!IsStart && !IsFinish)
            {
                Rectangle rectangle = new Rectangle(TopLeft, Size);
                return rectangle.Contains(point);
            }

            return false;
        }
    }
}
