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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RandomFigures();
        }

        private void RandomFigures()
        {
            var rand = new Random();
            RectangleFunc1(rand);
            RectangleFunc2(rand);
            ElipseFunc1(rand);
            ElipseFunc2(rand);
        }

        private void RectangleFunc1(Random rand)
        {
            Rectangle1.Width = rand.Next(100, 400);
            Rectangle1.Height = rand.Next(100, 400);
            Canvas.SetLeft(Rectangle1, rand.Next((int)this.Width));
            Canvas.SetTop(Rectangle1, rand.Next((int)this.Height));
        }
        private void RectangleFunc2(Random rand)
        {
            Rectangle2.Width = rand.Next(100, 400);
            Rectangle2.Height = rand.Next(100, 400);
            Canvas.SetLeft(Rectangle2, rand.Next((int)this.Width));
            Canvas.SetTop(Rectangle2, rand.Next((int)this.Height));
        }
        private void ElipseFunc1(Random rand)
        {
            Ellipse1.Width = rand.Next(100, 400);
            Ellipse1.Height = rand.Next(100, 400);
            Canvas.SetLeft(Ellipse1, rand.Next((int)this.Width));
            Canvas.SetTop(Ellipse1, rand.Next((int)this.Height));
        }
        private void ElipseFunc2(Random rand)
        {
            Ellipse2.Width = rand.Next(100, 400);
            Ellipse2.Height = rand.Next(100, 400);
            Canvas.SetLeft(Ellipse2, rand.Next((int)this.Width));
            Canvas.SetTop(Ellipse2, rand.Next((int)this.Height));
        }

        private void Rectangle1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle1.Stroke = new SolidColorBrush(Color.FromRgb(255,255,255));
            Rectangle1.StrokeThickness = 10;
        }

        private void Rectangle2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle2.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Rectangle2.StrokeThickness = 10;
        }

        private void Ellipse1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse1.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Ellipse1.StrokeThickness = 10;
        }

        private void Ellipse2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse2.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Ellipse2.StrokeThickness = 10;
        }

        private void Rectangle1_MouseUp(object sender, MouseEventArgs e)
        {
            Rectangle1.StrokeThickness = 0;
        }

        private void Rectangle2_MouseUp(object sender, MouseEventArgs e)
        {
            Rectangle2.StrokeThickness = 0;
        }

        private void Ellipse1_MouseUp(object sender, MouseEventArgs e)
        {
            Ellipse1.StrokeThickness = 0;
        }

        private void Ellipse2_MouseUp(object sender, MouseEventArgs e)
        {
            Ellipse2.StrokeThickness = 0;
        }

    }
}
