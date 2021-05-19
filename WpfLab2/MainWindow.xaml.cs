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
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            RandomFigures((Canvas)sender);
        }
        
        private void RandomFigures(Canvas canvas)
        {
            var rand = new Random();

            for (int i = 0; i < 4; i++)
            {
                var width = rand.Next(10, 400);
                var height = rand.Next(10, 400);
                var color = new SolidColorBrush(Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)));

                if (rand.Next() % 2 == 0)
                {
                    Rectangle figure = new Rectangle
                    {
                        Width = width,
                        Height = height,
                        Fill = color,
                        Cursor = Cursors.Hand,
                    };

                    figure.MouseDown += figure_MouseDown;
                    figure.GotFocus += figure_GotFocus;
                    Canvas.SetLeft(figure, rand.Next((int)this.Width - width));
                    Canvas.SetTop(figure, rand.Next((int)this.Height - height));
                    canvas.Children.Add(figure);
                }
                else
                {
                    Ellipse figure = new Ellipse
                    {
                        Width = width,
                        Height = height,
                        Fill = color,
                        Cursor = Cursors.Hand,
                    };

                    figure.MouseDown += figure_MouseDown;
                    figure.GotFocus += figure_GotFocus;
                    figure.Focusable = true;
                    Canvas.SetLeft(figure, rand.Next((int)this.Width - width));
                    Canvas.SetTop(figure, rand.Next((int)this.Height - height));
                    canvas.Children.Add(figure);
                }
            }
        }

        private void figure_GotFocus(object sender, RoutedEventArgs e)
        {
            Ellipse figure = new Ellipse
            {
                Width = 100,
                Height = 100,
                Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Cursor = Cursors.Hand,
            };
            mainCanvas.Children.Add(figure);
        }

        private void figure_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
