using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// UserInfo 的摘要说明
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        
        public int UserType { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
        public int InviteID { get; set; }
        public string NickName { get; set; }
        public DateTime Date { get; set; }

        public int Money { get; set; }

        public int Grade { get; set; }
        public string ActiveIP { get; set; }
        public int ChargedMoney { get; set; }
        public bool Sex { get; set; }
        public int State { get; set; }
        public bool IsExist { get; set; }
    }
}