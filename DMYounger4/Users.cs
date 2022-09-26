using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BilibiliDM_PluginFramework;
using LinePutScript;
using System.IO;

namespace DMYounger4
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class Users
    {
        /// <summary>
        /// 用户id,储存已这个为准
        /// </summary>
        public long Uid;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name = "";

        /// <summary>
        /// 用户积分,仅仅本次直播
        /// </summary>
        public int TempPoint = 0;

        /// <summary>
        /// 用户积分,包括送的礼物和说话互动啥的
        /// </summary>
        public int Point = 0;
        /// <summary>
        /// 给用户添加积分
        /// </summary>
        /// <param name="point">添加的积分数量</param>
        public void AddPoint(int point)
        {
            Point += point;
            TempPoint += point;
        }
        /// <summary>
        /// 上次活跃在我直播间时间 
        /// </summary>
        public DateTime LastLogin;
        /// <summary>
        /// 头像链接
        /// </summary>
        public Uri HeadImage = null;

        public bool HaveHeadImage
        {
            get
            {
                if (haveheadimage == null)
                {
                    haveheadimage = false;
                    if (File.Exists(Function.SoftPath + $"\\usr\\{Uid}.png"))
                    {
                        HeadImage = new Uri(Function.SoftPath + $"\\usr\\{Uid}.png");
                        haveheadimage = true;
                    }
                    else if (File.Exists(Function.SoftPath + $"\\usr\\{Uid}.jpg"))
                    {
                        HeadImage = new Uri(Function.SoftPath + $"\\usr\\{Uid}.jpg");
                        haveheadimage = true;
                    }
                }
                return haveheadimage == true;
            }
        }
        public bool? haveheadimage = null;
        /// <summary>
        /// 新建一个用户
        /// </summary>
        /// <param name="Uid">用户id</param>
        /// <param name="Name">用户名</param>
        public Users(int uid, string name)
        {
            LastLogin = DateTime.Now;
            Uid = uid;
            Name = name;
        }
        public Users(DanmakuModel user)
        {
            LastLogin = DateTime.Now;
            Uid = user.UserID;
            Name = user.UserName;
        }
        /// <summary>
        /// 从数据库读取的用户
        /// </summary>
        /// <param name="data">数据</param>
        public Users(Line data)
        {
            Uid = data.InfoToInt64;
            Sub tmp = data.Find("name");
            if (tmp != null)
                Name = tmp.Info;
            else
                Name = "ERROR";

            tmp = data.Find("point");
            if (tmp != null)
                Point = tmp.InfoToInt;
            else
                Point = 0;

            tmp = data.Find("tpoint");
            if (tmp != null)
                TempPoint = tmp.InfoToInt;
            else
                TempPoint = 0;

            tmp = data.Find("lastlogin");
            if (tmp != null)
                LastLogin = new DateTime(tmp.InfoToInt64);
            else
                LastLogin = DateTime.MinValue;
        }
        private bool isVIP = false;
        public bool IsVIP { get => isVIP; set => isVIP = value; }

        /// <summary>
        /// 生成Line用户储存
        /// </summary>
        /// <returns>生成的储存数据</returns>
        public Line ToLine() => new Line("user", Uid.ToString(), "", new Sub("name", Name), new Sub("point", Point.ToString()), new Sub("tpoint", TempPoint.ToString()), new Sub("lastlogin", LastLogin.Ticks.ToString()));
    }
}
