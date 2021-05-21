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

namespace WpfLab2
{
    public class ColorInfo
    {
        public string ColorName { get; set; }
        public Color Color { get; set; }

        public SolidColorBrush SampleBrush
        {
            get { return new SolidColorBrush(Color); }
        }

        public ColorInfo(string color_name, Color color)
        {
            ColorName = color_name;
            Color = color;
        }
    }
}
