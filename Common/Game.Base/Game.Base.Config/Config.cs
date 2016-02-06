using Lsj.Util.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base.Config
{
    public class Config:XmlConfigFile
    {
        public Config() : base("ddt.config")
        {
        }
        [ConfigElementName(Name = "WebIP")]
        private ConfigElement webip = new ConfigElement("127.0.0.1");
        [ConfigElementName(Name ="WebPort")]
        private ConfigElement webport = new ConfigElement("80");

        public string WebIP => webip.Value;
        public int WebPort => webport.IntValue;
    }
}
