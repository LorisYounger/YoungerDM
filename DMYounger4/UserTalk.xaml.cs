using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static DMYounger4.Function;
namespace DMYounger4
{
    /// <summary>
    /// UserTalk.xaml 的交互逻辑
    /// </summary>
    public partial class UserTalk : UserControl
    {

        public UserTalk(Users usr, string Text, int showtime, double width)
        {
            InitializeComponent();
            TText.Text = Text;
            this.Width = width - 4;
            TUserName.Text = usr.Name;
            if (usr.HaveHeadImage)
                EImage.Fill = new ImageBrush(new BitmapImage(usr.HeadImage));
            else
                EImage.Fill = new ImageBrush(new BitmapImage(new Uri(SoftPath + @"\default.png")));
            ShowTime = showtime;
            BorderTime.Width = Width;
            bsec = Width / (showtime + 1);
            Handle += TimeRels;
            Margin = new Thickness(1);
        }
        public int ShowTime;//s
        readonly double bsec;
        public void TimeRels()
        {
            if (ShowTime-- <= 0)
            {
                mainwin.RemoveUIE(this);
                Handle -= TimeRels;
            }
            else
            {
                UTMain.Dispatcher.BeginInvoke(new Action(() =>
                {
                    BorderTime.Width -= bsec;
                    if (ShowTime < 3)
                    {
                        UTMain.Opacity -= 0.2;
                    }
                }));
            }

        }
    }
}
