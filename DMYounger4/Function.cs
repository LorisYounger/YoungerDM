using LinePutScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMYounger4
{
    public static class Function
    {
        public static Display mainwin;
        /// <summary>
        /// 开播 前9个说话的人可以多拿点积分啥的
        /// </summary>
        static int FirstPerson = 10;
        static Random Rnd = new Random();
        /// <summary>
        /// 用户列表
        /// </summary>
        public static List<Users> BiliUsers;

        //public delegate void func(string str);

        public static readonly string SoftPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\LBSoft\DMYounger";

        /// <summary>
        /// 找到或者生成一个新用户
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="uname">用户名</param>
        /// <returns></returns>
        public static Users GetUser(int uid, string uname)
        {
            Users usr = BiliUsers.Find(x => x.Uid == uid);
            if (usr == null)
            {
                usr = new Users(uid, uname);
                BiliUsers.Add(usr);
            }
            else
                usr.Name = uname;
            return usr;
        }

        /// <summary>
        /// 接受弹幕消息
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="uname">用户名</param>
        /// <param name="text">用户说的话</param>
        public static void DanmuMessage(int uid, string uname, string text)
        {
            Users usr = GetUser(uid, uname);
            //杜绝刷屏赚积分现象,每分钟只能积分一次
            if (DateTime.Now.Ticks - usr.LastLogin.Ticks >= 600000000)
            {
                usr.AddPoint(1);//给这个用户一点活跃分
                usr.LastLogin = DateTime.Now;
                if (FirstPerson > 0)//前9个发话的人多给点积分
                {
                    usr.AddPoint(--FirstPerson);
                }
                //2%发消息幸运儿,多拿10积分
                if (Rnd.Next(50) == 0)
                {
                    usr.AddPoint(10);
                }
            }
            switch (text)
            {
                case "我的积分":
                case "我的活跃点数"://TODO:换算成k等复杂单位
                    sendDM(usr, $"我的活跃点数: {usr.TempPoint}/{usr.Point}");
                    break;
                //或许可以出一些功能用活跃点数购买小功能,比如说超级消息 100积分=1次
                default:
                    sendDM(usr, text);
                    break;
            }
        }
        /// <summary>
        /// 礼物信息
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="uname">用户名</param>
        public static void GiftMessage(int uid, string uname, string giftname, int gifttimes)
        {
            Users usr = GetUser(uid, uname);
            int getpoint = 10;
            switch (giftname)
            {
                case "辣条":
                case "小心心":
                case "PK票":
                    getpoint = 2;
                    break;                
                case "吃瓜":
                case "凉了":
                case "牛哇牛哇":
                case "粉丝团灯牌":
                case "小花花":
                case "i了i了":
                case "PK值卡":
                    getpoint = 10;
                    break;
                case "flag":
                    getpoint = 20;
                    break;
                case "咩咩羊":
                    getpoint = 60;
                    break;
                case "打call":
                    getpoint = 50;
                    break;               
                case "冰阔落":
                case "这个好诶":
                case "白银宝盒":
                case "幸运之钥":
                    getpoint = 100;
                    break;
                case "白金手柄":
                case "打榜":
                case "给大佬递茶":
                    getpoint = 200;
                    break;
                case "泡泡机":
                    getpoint = 500;
                    break;
                case "情书":
                    getpoint = 520;
                    break;
                case "干杯":
                    getpoint = 660;
                    break;
                case "B坷垃":
                    getpoint = 990;
                    break;
                case "紫金宝盒":
                    getpoint = 1000;
                    break;
                case "守护之翼":
                    getpoint = 2000;
                    break;
                case "告白花束":
                    getpoint = 2200;
                    break;
                case "礼花":
                    getpoint = 2800;
                    break;
                case "花式夸夸":
                    getpoint = 3300;
                    break;
                case "为你加冕":
                    getpoint = 4500;
                    break;
                case "疯狂打call":
                case "疯狂心动":
                    getpoint = 5200;
                    break;               
                case "撒花":
                    getpoint = 6600;
                    break;
                case "天空之翼":
                case "节奏风暴":
                    getpoint = 10000;
                    break;
                case "摩天大楼":
                    getpoint = 45000;
                    break;
                case "小电视飞船":
                    getpoint = 124500;
                    break;

                default:
                    //sendDM($"杨远插件:意料之外的礼物名称{giftname}\n在直播结束后查看日志并修改程序和修正用户'{uname}'的积分数据");
                    sendLog($"Info:意料之外的礼物名称'{giftname}'");
                    getpoint = 5;
                    break;
            }
            usr.AddPoint(getpoint * gifttimes);
            getpoint = (int)Math.Sqrt(gifttimes * getpoint);
            sendDM(usr, $"{giftname}{(gifttimes == 1 ? "" : "*" + gifttimes)}", getpoint * 2);//原本想写得到了多少活跃点数,但是感觉有点骗氪,就没写了
        }

      
        public static Action<string> sendLog;
        public static event Action Handle;
        public static void Smtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Handle();
        }
        public static void sendDM(Users usr, string dm, int showtime = 20)
        {
            mainwin.Main.Dispatcher.BeginInvoke(new Action(() => mainwin.Main.Children.Add(new UserTalk(usr, dm, showtime, mainwin.Main.ActualWidth))));
        }

        /// <summary>
        /// 超级消息 根据价格可以10秒显示n次(timer) 又或者别的专门搞个位置放这个超级消息
        /// </summary>
        public static void SuperMessage(int uid, string uname, int price, string message)
        {    //                               //如果我自己搞的话
             //查了下bili的价格            //换算成活跃点数   倍率  次数     时间
             //30=60s        =>*2            3000            *1      30      300s
             //50=120s       =>*2.4          5000            *1      50      500s
             //100=300s      =>*3            10000           *1      100     1000s
             //500=1800s     =>*3.6          50000           *1      500     5000s
             //1000=3600s    =>*3.6          10000           *1      1000    10000s
             //2000=2*3600s  =>*3.6          20000           *1      2000    20000s
            Users usr = GetUser(uid, uname);
            //usr.AddPoint(price * 100);//价格*100=活跃积分
            ////输出次数=价格
            //SuperChats.Add(new SuperChat(uname, message, price));
            //smtimer.Start();
        }
        public static System.Timers.Timer smtimer;
        public static System.Timers.Timer autosave;

    }

}
