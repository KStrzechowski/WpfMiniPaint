using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// https://social.msdn.microsoft.com/Forums/en-US/78be7989-b931-4479-ab43-7bc57f07d6f1/wpf-user-controlimage-display-with-drawing-functionality?forum=wpf

namespace WpfLab2
{
    public partial class MainWindow : Window
    {
        Shape selectedShape;
        string creatingFigure = "";
        Point startingPosition;
        bool isCreating = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            RandomFigures((Canvas)sender);
        }

        private void RandomFigures(Canvas canvas)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            Shape figure;
            for (int i = 0; i < 4; i++)
            {
                var width = rand.Next(10, 400);
                var height = rand.Next(10, 400);
                var color = new SolidColorBrush(Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)));

                if (rand.Next() % 2 == 0)
                    figure = NewRectangle(width, height, color);
                else
                    figure = NewEllipse(width, height, color);

                AddControls(figure);
                SetPosition(figure, rand.Next((int)this.Width - (int)figure.Width), rand.Next((int)this.Height - (int)figure.Height));
                canvas.Children.Add(figure);
            }
        }
        private Shape NewRectangle(int width, int height, Brush color)
        {
            var figure = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = color,
                Cursor = Cursors.Hand,
            };
            return figure;
        }
        private Shape NewEllipse(int width, int height, Brush color)
        {
            var figure = new Ellipse
            {
                Width = width,
                Height = height,
                Fill = color,
                Cursor = Cursors.Hand,
            };
            return figure;
        }

        private void AddControls(Shape figure)
        {
            figure.MouseDown += Figure_MouseDown;
            figure.GotFocus += Figure_GotFocus;
            figure.LostFocus += Figure_LostFocus;
            figure.Focusable = true;
        }
        private void SetPosition(Shape figure, int xPosition, int yPosition)
        {
            Canvas.SetLeft(figure, xPosition);
            Canvas.SetTop(figure, yPosition);
        }

        private void Figure_GotFocus(object sender, RoutedEventArgs e)
        {
            var figure = (Shape)sender;
            selectedShape = figure;

            figure.Effect = new DropShadowEffect
            {
                Direction = 270,
                BlurRadius = 50,
                Color = Color.FromRgb(255, 255, 255)
            };

            int figureZIndex = Panel.GetZIndex(figure);
            if (figureZIndex == 0)
            {
                figureZIndex++;
                Panel.SetZIndex(figure, figureZIndex);
            }

            for (int i = 0; i < mainCanvas.Children.Count; i++)
            {
                UIElement child = mainCanvas.Children[i];
                if (!child.Equals(figure) && Panel.GetZIndex(child) <= figureZIndex)
                    Panel.SetZIndex(child, figureZIndex - 1);
            }
        }

        private void Figure_LostFocus(object sender, RoutedEventArgs e)
        {
            var figure = (Shape)sender;
            figure.Effect = null;
        }

        private void Figure_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (creatingFigure != "")
                return;
            var figure = (Shape)sender;
            if (!figure.IsFocused)
                figure.Focus();
            else
                mainCanvas.Focus();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedShape != null)
                mainCanvas.Children.Remove(selectedShape);
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            if (selectedShape != null)
                selectedShape.Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)));
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            creatingFigure = "Rectangle";
            mainCanvas.Cursor = Cursors.Cross;
        }

        private void Ellipse_Click(object sender, RoutedEventArgs e)
        {
            creatingFigure = "Ellipse";
            mainCanvas.Cursor = Cursors.Cross;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = mainCanvas;
            Shape figure;
            var rand = new Random(Guid.NewGuid().GetHashCode());
            Brush color = new SolidColorBrush(Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)));

            if (creatingFigure == "")
                return;

            canvas.CaptureMouse();
            startingPosition = Mouse.GetPosition(canvas);
            switch (creatingFigure)
            {
                case "Rectangle":
                    figure = NewRectangle(10, 10, color);
                    break;
                case "Ellipse":
                    figure = NewEllipse(10, 10, color);
                    break;
                default:
                    return;
            }
            isCreating = true;
            Canvas.SetTop(figure, startingPosition.Y);
            Canvas.SetLeft(figure, startingPosition.X);

            AddControls(figure);
            canvas.Children.Add(figure);
            selectedShape = figure;
            creatingFigure = "";
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mainCanvas.ReleaseMouseCapture();
            mainCanvas.Cursor = Cursors.Arrow;
            isCreating = false;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mainCanvas.IsMouseCaptured || !isCreating)
                return;

            Point location = e.MouseDevice.GetPosition(mainCanvas);

            double minX = Math.Min(location.X, startingPosition.X);
            double minY = Math.Min(location.Y, startingPosition.Y);
            double maxX = Math.Max(location.X, startingPosition.X);
            double maxY = Math.Max(location.Y, startingPosition.Y);

            Canvas.SetTop(selectedShape, minY);
            Canvas.SetLeft(selectedShape, minX);

            double height = maxY - minY;
            double width = maxX - minX;

            selectedShape.Height = Math.Abs(height);
            selectedShape.Width = Math.Abs(width);
        }
    }
}
