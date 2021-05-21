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
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

/// https://social.msdn.microsoft.com/Forums/en-US/78be7989-b931-4479-ab43-7bc57f07d6f1/wpf-user-controlimage-display-with-drawing-functionality?forum=wpf
/// https://stackoverflow.com/questions/14125771/calculate-angle-from-matrix-transform?fbclid=IwAR2G6K07JPRBM6SXlCJ3jrM0f_urADPS3xwlhMFa2OHzUXMUKqdA95X6YYw
/// https://teusje.wordpress.com/2012/05/01/c-save-a-canvas-as-an-image/?fbclid=IwAR38GD2ubVKiLyByJ4nzeQekOgF1jHNh8_E6o8TH_w7lrvMA8H-BHsNnkzI
/// https://docs.microsoft.com/pl-pl/dotnet/api/microsoft.win32.savefiledialog?view=net-5.0&fbclid=IwAR2oArhYbj3bRdGsiir-iM0irMr1ilhiAEkBiaZ87bIn7F5Z8qbcAuXjUHk

namespace WpfLab2
{
    public partial class MainWindow : Window
    {
        Shape selectedShape;
        string creatingFigure = "";
        Point startingPosition;
        bool isCreating = false;
        bool isFigureClicked = false;
        bool isShapeFocused = false;
        bool movingFigure = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            RandomFigures((Canvas)sender);
            LoadColors();
            disableButtons();
        }

        private void LoadColors()
        {
            var color_query =
                from PropertyInfo property in typeof(Colors).GetProperties()
                orderby property.Name
                where property.Name != "Transparent"
                select new ColorInfo(
                    property.Name,
                    (Color)property.GetValue(null, null));
            colorComboBox.ItemsSource = color_query;
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
            figure.MouseLeftButtonDown += Figure_MouseLeftClickDown;
            figure.GotFocus += Figure_GotFocus;
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
            isShapeFocused = true;
            FigureFocusView(figure);
            enableButtons();
        }

        private void FigureFocusView(Shape figure)
        {
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

        private void RemoveFigureFocusView(Shape figure)
        {
            figure.Effect = null;
        }

        private void Figure_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isCreating)
                return;
            var figure = (Shape)sender;
            if (!figure.IsFocused || Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (selectedShape != null && isShapeFocused)
                    RemoveFigureFocusView(selectedShape);
                isFigureClicked = true;
                figure.Focus();
            }
            else
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                figure.MoveFocus(tRequest);
                isShapeFocused = false;
                RemoveFigureFocusView(figure);
                disableButtons();
            }
        }
        private void Figure_MouseLeftClickDown(object sender, MouseButtonEventArgs e)
        {
            if (isCreating)
                return;
            var figure = (Shape)sender;
            figure.Cursor = Cursors.ScrollAll;
            movingFigure = true;
            isShapeFocused = true;
            Point location = e.MouseDevice.GetPosition(mainCanvas);
            startingPosition = new Point(location.X - Canvas.GetLeft(figure), location.Y - Canvas.GetTop(figure));
            figure.CaptureMouse();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedShape != null && isShapeFocused)
            {
                mainCanvas.Children.Remove(selectedShape);
                disableButtons();
            }
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            if (selectedShape != null && isShapeFocused)
            {
                var rand = new Random();
                selectedShape.Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)));
                selectedShape.Focus();
            }
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

            if (creatingFigure != "")
            {
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
            else if (!isFigureClicked && selectedShape != null)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                selectedShape.MoveFocus(tRequest);
                isShapeFocused = false;
                RemoveFigureFocusView(selectedShape);
                disableButtons();
            }
            else
            {
                isFigureClicked = false;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mainCanvas.ReleaseMouseCapture();
            if (selectedShape != null)
            {
                selectedShape.ReleaseMouseCapture();
                selectedShape.Cursor = Cursors.Hand;
            }
            mainCanvas.Cursor = Cursors.Arrow;
            isCreating = false;
            movingFigure = false;
            creatingFigure = "";
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {

            if (isCreating && mainCanvas.IsMouseCaptured)
            {
                Point location = e.MouseDevice.GetPosition(mainCanvas);

                double minX = Math.Min(location.X, startingPosition.X);
                double minY = Math.Min(location.Y, startingPosition.Y);
                double maxX = Math.Max(location.X, startingPosition.X);
                double maxY = Math.Max(location.Y, startingPosition.Y);

                Canvas.SetLeft(selectedShape, minX);
                Canvas.SetTop(selectedShape, minY);

                double height = maxY - minY;
                double width = maxX - minX;

                selectedShape.Height = Math.Abs(height);
                selectedShape.Width = Math.Abs(width);
            }
            else if (movingFigure && selectedShape.IsMouseCaptured)
            {
                if (movingFigure)
                {
                    var figure = selectedShape;
                    Point location = e.MouseDevice.GetPosition(mainCanvas);
                    Canvas.SetLeft(figure, location.X - startingPosition.X);
                    Canvas.SetTop(figure, location.Y - startingPosition.Y);
                }
            }
        }

        private void angleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectedShape != null)
            {
                RotateTransform rotateTransform2 = new RotateTransform(angleSlider.Value);
                rotateTransform2.CenterX = selectedShape.Width / 2;
                rotateTransform2.CenterY = selectedShape.Height / 2;
                selectedShape.RenderTransform = rotateTransform2;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Color_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            if (selectedShape != null && isShapeFocused)
                selectedShape.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(textBlock.Text);
        }

        private void heightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var TextBox = (TextBox)sender;
            if (selectedShape != null && isShapeFocused)
                selectedShape.Height = Convert.ToInt32(TextBox.Text);
        }

        private void widthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var TextBox = (TextBox)sender;
            if (selectedShape != null && isShapeFocused)
                selectedShape.Width = Convert.ToInt32(TextBox.Text);
        }

        private void enableButtons()
        {
            widthTextBox.IsEnabled = true;
            heightTextBox.IsEnabled = true;
            colorComboBox.IsEnabled = true;
            angleSlider.IsEnabled = true;
            widthTextBox.Text = selectedShape.Width.ToString();
            heightTextBox.Text = selectedShape.Height.ToString();
            colorComboBox.SelectedItem = selectedShape.Fill;
            //angleSlider.Value = 0;
            if (selectedShape != null)
            {
                var mT = selectedShape.RenderTransform;
                if (mT != null)
                {
                    var x = new Vector(1, 0);
                    Vector rotated = Vector.Multiply(x, mT.Value);
                    angleSlider.Value = Vector.AngleBetween(x, rotated);
                }
            }

            deleteButton.IsEnabled = true;
            randomColorsButton.IsEnabled = true;
        }

        private void disableButtons()
        {
            if (selectedShape != null)
                selectedShape = null;
            widthTextBox.IsEnabled = false;
            heightTextBox.IsEnabled = false;
            colorComboBox.IsEnabled = false;
            angleSlider.IsEnabled = false;
            widthTextBox.Text = null;
            heightTextBox.Text = null;
            colorComboBox.SelectedItem = null;
            angleSlider.Value = 0;
            deleteButton.IsEnabled = false;
            randomColorsButton.IsEnabled = false;
        }

        private void CreateSaveBitmap(Canvas canvas)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.ActualWidth, (int)canvas.ActualHeight,
             96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Canvas"; 
            dlg.DefaultExt = ".png"; 
            dlg.Filter = "Text documents (.png)|*.png"; 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                using (FileStream file = File.Create(filename))
                {
                    encoder.Save(file);
                }
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSaveBitmap(mainCanvas);
        }
    }
}
