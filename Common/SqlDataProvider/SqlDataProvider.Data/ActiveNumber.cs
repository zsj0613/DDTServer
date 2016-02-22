using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ActiveNumberInfo
    {
        public string AwardID
        {
            get;
            set;
        }
        public int ActiveID
        {
            get;
            set;
        }
        public bool PullDown
        {
            get;
            set;
        }
        public DateTime GetDate
        {
            get;
            set;
        }
        public int UserID
        {
            get;
            set;
        }
        public int Mark
        {
            get;
            set;
        }
    }
}
