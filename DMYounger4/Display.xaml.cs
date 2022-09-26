using LinePutScript;
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

namespace DMYounger4
{
    /// <summary>
    /// Display.xaml 的交互逻辑
    /// </summary>
    public partial class Display : Window
    {
        public Display()
        {
            InitializeComponent();
            Width = DMYounger.Saves["Display"].GetDouble("Width", 300);
            Height = DMYounger.Saves["Display"].GetDouble("Height", 400);
        }
        public void RemoveUIE(UIElement obj) => Main.Dispatcher.BeginInvoke(new Action(() => Main.Children.Remove(obj)));

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DMYounger.Saves["Display"][(gdbe)"Width"] = Width;
            DMYounger.Saves["Display"][(gdbe)"Height"] = Height;
        }
    }
}
