using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PathfindingVisualizer
{
    [Serializable]
    public class Cell
    {
        public enum State
        {
            Normal,
            Obstacle,
            Start,
            End
        }

        public Point TopLeft { get; set; }
        public Size Size { get; set; }
        public State PreviousState { get; set; }
        public State CurrentState { get; set; }
        public bool IsVisited { get; set; }
        public bool IsPath { get; set; }

        public Cell(Point topLeft, Size size)
        {
            TopLeft = topLeft;
            Size = size;
            PreviousState = State.Normal;
            CurrentState = State.Normal;
        }

        public void ChangeState(State newState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
        }

        public void UndoState()
        {
            CurrentState = PreviousState;
            PreviousState = State.Normal;
        }

        private Color ChooseColor()
        {
            if (CurrentState == State.Start)
                return Color.Green;
            else if (CurrentState == State.End)
                return Color.Red;

            if (IsPath)
                return Color.Purple;
            else if (IsVisited)
                return Color.LightGreen;

            if (CurrentState == State.Normal)
                return Color.Gray;
            else if (CurrentState == State.Obstacle)
                return Color.Black;

            return Color.White;
        }

        public void Draw(Graphics graphics)
        {
            Brush brush = new SolidBrush(ChooseColor());
            graphics.FillRectangle(brush, TopLeft.X, TopLeft.Y, Size.Width, Size.Height);
            brush.Dispose();
        }

        public bool IsClicked(Point point)
        {
            Rectangle rectangle = new Rectangle(TopLeft, Size);
            return rectangle.Contains(point);
        }
    }
}
