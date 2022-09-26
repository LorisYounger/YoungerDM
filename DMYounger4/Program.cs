using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;
using static DMYounger4.Function;
namespace DMYounger4
{
    public class DMYounger : BilibiliDM_PluginFramework.DMPlugin
    {
        public static LpsDocument Saves;
        public DMYounger()
        {
            this.PluginAuth = "洛里斯杨远";
            this.PluginName = "杨远弹幕插件4";
            this.PluginCont = "zoujin.dev@exlb.org";
            this.PluginVer = "v4.0.0";
            this.PluginDesc = "自己直播的弹幕姬插件当然得自己写";
            //this.Connected += DMYounger_Connected;
            //this.Disconnected += DMYounger_Disconnected;
            this.ReceivedDanmaku += DMYounger_ReceivedDanmaku;
            this.ReceivedRoomCount += DMYounger_ReceivedRoomCount;


            //读取数据
            BiliUsers = new List<Users>();
            if (!Directory.Exists(SoftPath))
                Directory.CreateDirectory(SoftPath);
            if (!Directory.Exists(SoftPath + @"\usr"))
                Directory.CreateDirectory(SoftPath + @"\usr");
            if (!File.Exists(SoftPath + @"\main.png"))
                Properties.Resources.Younger.Save(SoftPath + @"\default.png");
            if (File.Exists(SoftPath + @"\users.lpt"))
            {
                var data = new LpsDocument(File.ReadAllText(SoftPath + @"\users.lpt"));
                foreach (Line dt in data)
                    BiliUsers.Add(new Users(dt));
            }
            if (File.Exists(SoftPath + @"\saves.lpt"))
                Saves = new LpsDocument(File.ReadAllText(SoftPath + @"\saves.lpt"));
            else
                Saves = new LpsDocument("setting#YoungerDM:|");


            mainwin = new Display();
            smtimer = new System.Timers.Timer();
            autosave = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = 60000
            };
            ////把方法传递下
            sendLog = this.Log;
            //sendDM = this.AddDM;
            
            //請勿使用任何阻塞方法
        }

        public override void Start()
        {
            base.Start();
            mainwin.Show();
            smtimer.AutoReset = true;
            smtimer.Elapsed += Smtimer_Elapsed;
            smtimer.Interval = 1000;
            smtimer.Start();
            autosave.Elapsed += Autosave_Elapsed;
            autosave.Start();
            this.AddDM("杨远弹幕插件启动成功\n自己直播的插件当然得自己写!", true);
        }

        private void Autosave_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                LpsDocument lps = new LpsDocument();
                foreach (Users usr in BiliUsers)
                    lps.AddLine(usr.ToLine());
                File.WriteAllText(SoftPath + @"\users.lpt", lps.ToString());
                File.WriteAllText(SoftPath + @"\saves.lpt", Saves.ToString());
            }
            catch (Exception ex)
            {
                this.AddDM(ex.ToString());
            }
        }

        private void DMYounger_ReceivedRoomCount(object sender, BilibiliDM_PluginFramework.ReceivedRoomCountArgs e)
        {
            Users usrfist = null;
            foreach (Users usr in BiliUsers.OrderByDescending(x => x.TempPoint))
            {
                if (usr.Name != "")
                {
                    usrfist = usr;
                    break;
                }
            }
            Dispatcher.BeginInvoke(new Action(() => mainwin.LableTitle.Content = $"当前人气值{e.UserCount}   当前活跃榜第一为'{(usrfist == null ? "没有人呢,主播哭哭,发句话互动下吧" : usrfist.Name)}'"));
        }
        private void DMYounger_ReceivedDanmaku(object sender, BilibiliDM_PluginFramework.ReceivedDanmakuArgs e)
        {
            switch (e.Danmaku.MsgType)
            {
                case BilibiliDM_PluginFramework.MsgTypeEnum.Comment:
                    DanmuMessage(e.Danmaku.UserID, e.Danmaku.UserName, e.Danmaku.CommentText);
                    break;
                case BilibiliDM_PluginFramework.MsgTypeEnum.GiftSend:
                    GiftMessage(e.Danmaku.UserID, e.Danmaku.UserName, e.Danmaku.GiftName, e.Danmaku.GiftCount);
                    break;
                case BilibiliDM_PluginFramework.MsgTypeEnum.SuperChat:
                    break;
            }
        }
        public override void Stop()
        {
            base.Stop();
            //DMYounger_Disconnected(null, null);
            //储存数据
            LpsDocument lps = new LpsDocument();
            foreach (Users usr in BiliUsers)
                lps.AddLine(usr.ToLine());
            File.WriteAllText(SoftPath + @"\users.lpt", lps.ToString());
            File.WriteAllText(SoftPath + @"\saves.lpt", Saves.ToString());
            //請勿使用任何阻塞方法
            this.AddDM("杨远弹幕插件已关闭\n程序数据已存档\n下播啦啦啦!", true);
        }
        public override void Admin()
        {
            base.Admin();
            Setting set = new Setting(this);
            set.Show();
        }
    }
}
