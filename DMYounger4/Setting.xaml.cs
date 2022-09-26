using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LinePutScript;
using System.Security.Policy;

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
            UserID.Text = DMYounger.Saves["image"][(gstr)"Image"];
            UserCookies.Text = DMYounger.Saves["image"][(gstr)"Cookies"];
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
            DMYounger.Saves["image"][(gstr)"Image"] = UserID.Text;
            DMYounger.Saves["image"][(gstr)"Cookies"] = UserCookies.Text;
            MessageBox.Show("本次成功新增 " + GetUImage(UserID.Text, UserCookies.Text) + "个头像", "头像更新");
        }
        public string GetUImage(string uid, string cookies)
        {
            try
            {
                int same = 0;
                int success = 0;
                CookieContainer cookieContainer = new CookieContainer();
                foreach (var item in cookies.Replace(",", "%2C").Trim(';').Split(';'))
                {
                    var sp = item.Split('=');
                    if (sp.Length != 2 || string.IsNullOrWhiteSpace(item) || string.IsNullOrWhiteSpace(sp[0].Trim()) || string.IsNullOrWhiteSpace(sp[1].Trim()))
                        continue;
                    cookieContainer.Add(new Cookie(sp[0].Trim(), sp[1].Trim(), "", "api.bilibili.com"));
                }

                JObject json = Get(uid, 1, cookieContainer);
                int totalfan = (int)(json["data"]["total"]);
                int pages = totalfan / 50 + 1; // 取整加一是最大的页数
                for (int i = 1; i <= 20 && i <= pages; i++)
                {
                    json = Get(uid, i, cookieContainer);  // 获取一页数据，一页最多50个粉丝数据
                    foreach (var item in json["data"]["list"])
                    {
                        if (item["mid"] == null || item["face"] == null)
                            continue;
                        string fuid = (string)item["mid"]; //用户id
                        string face = (string)item["face"]; //头像URL
                        string path = SoftPath + @"\usr\" + fuid + face.Substring(face.Length - 4);
                        same++;
                        if (File.Exists(path))
                            continue;
                        using (var web = new WebClient())                        
                            web.DownloadFile(face, path);
                        
                        success++;
                    }
                }
                return $"({success}/{same})[{totalfan}]";
            }
            catch (Exception e)
            {
                MessageBox.Show("头像更新失败，请检查网络连接或者cookies是否正确\n" + e.ToString(), "头像失败");
                return "0";
            }
        }

        /// <summary>
        /// 获取页面html
        /// </summary>
        /// <returns></returns>
        public JObject Get(string uid, int page, CookieContainer cookieContainer)
        {
            string html = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                $"https://api.bilibili.com/x/relation/followers?vmid={uid}&pn={page}");
            request.ContentType = "text/html;";
            request.Method = "Get";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.26";
            request.CookieContainer = cookieContainer;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream streamResponse = response.GetResponseStream())
                {
                    using (StreamReader streamResponseReader = new StreamReader(streamResponse, Encoding.UTF8))
                    {
                        html = streamResponseReader.ReadToEnd();
                    }
                }
            }
            return JObject.Parse(html);
        }
    }
}
