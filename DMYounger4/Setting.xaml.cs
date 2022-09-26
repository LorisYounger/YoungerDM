using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static DMYounger4.Function;

namespace DMYounger4
{
    /// <summary>
    /// Display.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        DMYounger master;
        public Setting(DMYounger host)
        {
            InitializeComponent();
            this.master = host;
        }

        private void create_thanks_click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder("感谢");
            foreach (Users usr in BiliUsers.FindAll(x => x.TempPoint > 0).OrderBy(x => x.TempPoint).Reverse())
            {
                sb.Append(usr.Name);
                sb.Append(',');
                usr.TempPoint = 0;
            }
            sb.Append("和其他观看的小伙伴的支持");
            Outputbox.Text = sb.ToString();
        }

        private void Outputbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Outputbox.SelectAll();
        }

        private void Outputbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Outputbox.Visibility = Outputbox.Text == "" ? Visibility.Hidden : Visibility.Visible;
        }

        private void open_path_click(object sender, RoutedEventArgs e)
        {
            Process.Start(SoftPath);
        }

        private void update_image_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
