using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace paint_mini
{
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        Graphics graphics;
        Algorithms drawing;
        bool isPaint = false;
        Point px, py;
        Pen pen = new Pen(Color.Black, 1);
        int index;
        int x, y, sX, sY, cX, cY;
        List<Point> points = new List<Point>();
        Color currColor = Color.Black;
        ColorDialog colorDialog = new ColorDialog();
        Color patternColor = Color.White;
        DrawingType drawingType = DrawingType.FreeDrawing;

        byte[][] pattern = new byte[][]
    {
        new byte[] { 0, 0, 0, 0, 0 },
        new byte[] { 0, 0, 0, 0, 0 },
        new byte[] { 0, 0, 1, 0, 0 },
        new byte[] { 0, 0, 0, 0, 0 },
        new byte[] { 0, 0, 0, 0, 0 }
    };

        private enum DrawingType
        {
            Line,
            Rectangle,         
            Ellipse,           
            BezierCurve,
            FreeDrawing,
            Fill,
            PatternFill
        }
        private void circleBtn_Click(object sender, EventArgs e)
        {
            drawingType = DrawingType.Ellipse;
        }

        private void lineBtn_Click(object sender, EventArgs e)
        {
            drawingType = DrawingType.Line;
        }

        private void rectangleBtn_Click(object sender, EventArgs e)
        {
            drawingType = DrawingType.Rectangle;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
           
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (drawingType)
            {
                case DrawingType.BezierCurve:
                    points.Add(e.Location);
                    graphics.FillRectangle(new SolidBrush(currColor), e.X, e.Y, 2, 2);
                    break;
                case DrawingType.Fill:
                    drawing.FillColor(e.X, e.Y, currColor); 
                    break;
                case DrawingType.PatternFill:
                    drawing.FillPatternColor(e.X, e.Y, currColor, patternColor, pattern); 
                    break;

            }          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            drawingType = DrawingType.BezierCurve;
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (drawingType == DrawingType.BezierCurve)
            {
                if (points.Count > 1)
                {
                    drawing.DrawBezierCurve( points, currColor);
                    points.Clear();
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            pictureBox1.Image = bitmap;
            index = 0;
        }

        private void chooseColorBtn_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
             currColor = colorDialog.Color;
            colorBtn.BackColor = colorDialog.Color;
            pen.Color = colorDialog.Color;
        }

        private void fillBtn_Click(object sender, EventArgs e)
        {
            drawingType = DrawingType.Fill;
        }

        private void patternBtn_Click(object sender, EventArgs e)
        {
            drawingType = DrawingType.PatternFill;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {

        }

       
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isPaint = true;
            py = e.Location;

            cX = e.X; cY = e.Y;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPaint)
            {
                if(drawingType == DrawingType.FreeDrawing)
                {
                    px = e.Location;
                    graphics.DrawLine(pen, px, py);
                    py = px;
                }
            }
            pictureBox1.Refresh();

            x = e.X; y = e.Y;
          
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isPaint = false;

            sX = x - cX; sY = y - cY;
            switch (drawingType)
            {
                case DrawingType.Ellipse:
                    drawing.DrawEllipseParametric(cX, cY, sX, sY, currColor);
                    break;
                    case DrawingType.Rectangle:
                    drawing.DrawRectangle(py, new Point(x, y), currColor);
                    break;
                case DrawingType.Line:
                    drawing.BrezenhamLine(py.X, py.Y, x, y, currColor);
                    break;
            }
         
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            drawingType = DrawingType.FreeDrawing;
        }

        public Form1()
        {
            InitializeComponent();

            this.Width = 900;
            this.Height = 700;
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);      
            graphics.Clear(Color.White);
            drawing = new Algorithms(bitmap, graphics);
            pictureBox1.Image = bitmap;
        }
    }
}
