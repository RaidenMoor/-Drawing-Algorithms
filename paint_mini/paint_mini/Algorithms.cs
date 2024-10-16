using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paint_mini
{
    internal class Algorithms
    {
        private Bitmap bitmap;
        private Graphics g;
        public  Algorithms (Bitmap _bitmap, Graphics g)
        {
            bitmap = _bitmap;
            this.g = g;
        }
        public void DrawEllipseParametric(int centerX, int centerY, int radiusX, int radiusY, Color color)
        {
            // Параметрические уравнения эллипса:
            // x = centerX + radiusX * cos(t)
            // y = centerY + radiusY * sin(t)

          
            List<Point> line_points = new List<Point>();
            double angle = 0; 
            for (int i = 0; i <= 1000; i++)
            {
                angle = 2 * Math.PI * i / 1000;
                int x = (int)(centerX + radiusX * Math.Cos(angle));
                int y = (int)(centerY + radiusY * Math.Sin(angle));
                line_points.Add(new Point(x, y));
                if (line_points.Count() == 2)
                {
                    BrezenhamLine(line_points[0].X, line_points[0].Y, line_points[1].X, line_points[1].Y, color);
                    line_points.Remove(line_points[0]);
                }
            }
        }
        public void DrawRectangle(Point start, Point end, Color color)
        {
            // Рисуем линии по алгоритму Брезенхама
            BrezenhamLine(start.X, start.Y, end.X, start.Y, color); // Верхняя линия
            BrezenhamLine(end.X, start.Y, end.X, end.Y, color); // Правая линия
            BrezenhamLine(end.X, end.Y, start.X, end.Y, color); // Нижняя линия
            BrezenhamLine(start.X, end.Y, start.X, start.Y, color); // Левая линия
        }
      
        public void BrezenhamLine(int x0, int y0, int x1, int y1, Color color)
        {
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                DrawPoint(steep ? y : x, steep ? x : y, color);
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }


        }
        // Вспомогательный метод для обмена значениями двух переменных
        void Swap(ref int x0, ref int x2)
        {
            var bin = x0;
            x0 = x2;
            x2 = bin;
        }
        void DrawPoint(int x, int y, Color color)
        {
            if (x > 0 & x < bitmap.Width & y > 0 & y < bitmap.Height)
                bitmap.SetPixel(x, y, color);
        }
        public void DrawBezierCurve(List<Point> controlPoints, Color color)
        {
            // Проверяем, что количество контрольных точек не менее 2
            if (controlPoints.Count < 2)
            {
                return;
            }

            // Делим кривую на сегменты
            const int segmentsCount = 1000; // Количество сегментов кривой
            double t = 0;
            List<Point> line_points = new List<Point>();
            for (int i = 0; i <= segmentsCount; i++)
            {
               
                t = (double)i / segmentsCount;

                // Вычисляем координаты точки на кривой
                int x = (int)BezierCurvePoint(t, controlPoints)[0];
                int y = (int)BezierCurvePoint(t, controlPoints)[1];

                line_points.Add(new Point(x, y));
                if (line_points.Count() == 2)
                {
                    BrezenhamLine(line_points[0].X, line_points[0].Y, line_points[1].X, line_points[1].Y, color);
                    line_points.Remove(line_points[0]);
                }

                
            }
        }

        // Вычисляет координату точки на кривой Безье по параметру t
         double[] BezierCurvePoint(double t, List<Point> controlPoints)
        {
            int n = controlPoints.Count - 1; // Степень кривой
            double x = 0, y = 0;

            for (int i = 0; i <= n; i++)
            {
                double bernstein = BernsteinPolynomial(n, i, t);
                x += controlPoints[i].X * bernstein;
                y += controlPoints[i].Y * bernstein;
            }

            return new double[] { x, y };
        }

        // Вычисляет полином Бернштейна
        double BernsteinPolynomial(int n, int i, double t)
        {
            return (double)Factorial(n) / (Factorial(i) * Factorial(n - i)) * Math.Pow(t, i) * Math.Pow(1 - t, n - i);
        }

        // Вычисляет факториал числа
        double Factorial(int n)
        {
            if (n == 0)
            {
                return 1;
            }
            else
            {
                return n * Factorial(n - 1);
            }
        }
        public void FillColor(int x, int y, Color fill_color)
        {
            Color backcolor = bitmap.GetPixel(x, y);
            var xLeft = x;
            var xRight = x + 1;
            while ((xRight < bitmap.Width - 1) && (bitmap.GetPixel(xRight, y) == backcolor))
            {
                bitmap.SetPixel(xRight, y, fill_color);
                xRight++;
            }
            xRight--;
            while ((xLeft > 0) && (bitmap.GetPixel(xLeft, y) == backcolor))
            {
                bitmap.SetPixel(xLeft, y, fill_color);
                xLeft--;
            }
            xLeft++;
            var xLeft1 = xLeft;
            while ((xLeft1 < xRight) && (y != 0))
            {
                while ((xLeft1 < xRight) && (bitmap.GetPixel(xLeft1, y - 1) != backcolor))
                    xLeft1++;
                if (xLeft1 < xRight) FillColor(xLeft1, y - 1, fill_color);
                xLeft1++;
            }
            xLeft1 = xLeft;
            while ((xLeft1 < xRight) && (y != bitmap.Height - 1))
            {
                while ((xLeft1 < xRight) && (bitmap.GetPixel(xLeft1, y + 1) != backcolor))
                    xLeft1++;
                if (xLeft1 < xRight) FillColor(xLeft1, y + 1, fill_color);
                xLeft1++;
            }
        }
        public void FillPatternColor(int x, int y, Color fill_color, Color pattern_color, byte[][] pattern)
        {
            Color backcolor = bitmap.GetPixel(x, y);
           
            if (pattern == null) FillColor(x, y, fill_color);
            else
            {
                Point point;
                Stack<Point> stack = new Stack<Point>();
                stack.Push(new Point(x, y));
                while(stack.Count() > 0)
                {
                    point = stack.Pop();
                    x = point.X;
                    y = point.Y;
                    bitmap.SetPixel(x, y, SetColor(x, y, fill_color, pattern_color, pattern));

                    if (( x + 1 > 0 && x + 1 < bitmap.Width && y > 0 && y < bitmap.Height) && (bitmap.GetPixel(x + 1, y) == backcolor))
                    {
                        stack.Push(new Point((int)x + 1, (int)y));
                    }
                    if ((x > 0 && x < bitmap.Width && y - 1> 0 && y - 1 < bitmap.Height) && (bitmap.GetPixel(x, y - 1) == backcolor))
                    {
                        stack.Push(new Point((int)x, (int)y - 1));
                    }
                    if ((x - 1 > 0 && x - 1 < bitmap.Width && y > 0 && y < bitmap.Height) && (bitmap.GetPixel(x - 1, y) == backcolor))
                    {
                        stack.Push(new Point((int)x - 1, (int)y));
                    }
                    if ((x > 0 && x < bitmap.Width && y + 1 > 0 && y + 1 < bitmap.Height) && (bitmap.GetPixel(x , y + 1) == backcolor))
                    {
                        stack.Push(new Point((int)x, (int)y + 1));
                    }
                }
            }
        }
       
        public void FillPatternColor2(int x, int y, Color fill_color, Color pattern_color, byte[][] pattern)
        {
            Color backcolor = bitmap.GetPixel(x, y);
            if (pattern == null) FillColor(x, y, fill_color);
            else
            {
                var xLeft = x;
                var xRight = x + 1;
                while ((xRight < bitmap.Width - 1) && (bitmap.GetPixel(xRight, y) == backcolor))
                {
                    bitmap.SetPixel(xRight, y, SetColor(xRight, y, fill_color, pattern_color, pattern));
                    xRight++;
                }
                xRight--;
                while ((xLeft > 0) && (bitmap.GetPixel(xLeft, y) == backcolor))
                {
                    bitmap.SetPixel(xLeft, y, SetColor(xLeft, y, fill_color, pattern_color, pattern));
                    xLeft--;
                }
                xLeft++;
                var xLeft1 = xLeft;
                while ((xLeft1 < xRight) && (y != 0))
                {
                    while ((xLeft1 < xRight) && (bitmap.GetPixel(xLeft1, y - 1) != backcolor))
                        xLeft1++;
                    if (xLeft1 < xRight) FillPatternColor(xLeft1, y - 1, fill_color, pattern_color, pattern);
                    xLeft1++;
                }
                xLeft1 = xLeft;
                while ((xLeft1 < xRight) && (y != bitmap.Height - 1))
                {
                    while ((xLeft1 < xRight) && (bitmap.GetPixel(xLeft1, y + 1) != backcolor))
                        xLeft1++;
                    if (xLeft1 < xRight) FillPatternColor(xLeft1, y + 1, fill_color, pattern_color, pattern);
                    xLeft1++;
                }
            }
        }
        private Color SetColor(int x, int y, Color fill_color, Color pattern_color, byte[][] pattern)
        {
            Color[] colors = { fill_color, pattern_color };
            byte m = pattern[x % pattern[0].Length][y % pattern.Length];
            return colors[m];
        }


    }
}
