using Game.Base.Config;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Reflection;
namespace Bussiness
{
	public abstract class GameProperties
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		[ConfigProperty("Edition", "当前游戏版本", "11000")]
		public static readonly string EDITION;
		[ConfigProperty("MustComposeGold", "合成消耗金币价格", 1000)]
		public static readonly int PRICE_COMPOSE_GOLD;
		[ConfigProperty("MustFusionGold", "熔炼消耗金币价格", 1000)]
		public static readonly int PRICE_FUSION_GOLD;
		[ConfigProperty("MustStrengthenGold", "强化金币消耗价格", 1000)]
		public static readonly int PRICE_STRENGHTN_GOLD;
		[ConfigProperty("CheckRewardItem", "验证码奖励物品", 11001)]
		public static readonly int CHECK_REWARD_ITEM;
		[ConfigProperty("CheckCount", "多少场出一次验证码", 20)]
		public static readonly int CHECKCODE_PER_GAME_COUNT;
		[ConfigProperty("HymenealMoney", "求婚的价格", 1000)]
		public static readonly int PRICE_PROPOSE;
		[ConfigProperty("DivorcedMoney", "离婚的价格", 1000)]
		public static readonly int PRICE_DIVORCED;
		[ConfigProperty("MarryRoomCreateMoney", "结婚房间的价格,2小时、3小时、4小时用逗号分隔", "2000,2700,3400")]
		public static readonly string PRICE_MARRY_ROOM;
		[ConfigProperty("BoxAppearCondition", "箱子物品提示的等级", 4)]
		public static readonly int BOX_APPEAR_CONDITION;
		[ConfigProperty("DisableCommands", "禁止使用的命令", "")]
		public static readonly string DISABLED_COMMANDS;
		[ConfigProperty("AssState", "防沉迷系统的开关,True打开,False关闭", false)]
		public static bool ASS_STATE;
		[ConfigProperty("DailyAwardState", "每日奖励开关,True打开,False关闭", true)]
		public static bool DAILY_AWARD_STATE;
		[ConfigProperty("Cess", "交易扣税", 0.1)]
		public static readonly double Cess;
		[ConfigProperty("BeginAuction", "拍买时起始随机时间", 20)]
		public static int BeginAuction;
		[ConfigProperty("EndAuction", "拍买时结束随机时间", 40)]
		public static int EndAuction;
		[ConfigProperty("Equip", "始化装备", "3101,6101,5101,7001|3201,6201,5201,7001")]
		public static string Equip;
		[ConfigProperty("CopyInviteLevelLimit", "邀请进入副本最低等级", 9)]
		public static int CopyInviteLevelLimit;
		[ConfigProperty("SpaRoomCreateMoney", "温泉房间的价格,2小时", "800,1600")]
		public static readonly string PRICE_SPA_ROOM;
		[ConfigProperty("SpaPubRoomCount", "温泉公共房间数量", "1,2")]
		public static readonly string COUNT_SPA_PUBROOM;
		[ConfigProperty("SpaPriRoomCount", "温泉私有房间数量上限", 2000)]
		public static int COUNT_SPA_PRIROOM;
		[ConfigProperty("RefreshLimitShopCount", "限量商品数量更新时间", "20:00")]
		public static string RefreshLimitShopCount;
		[ConfigProperty("LimitShopState", "限量购买开关", true)]
		public static bool LimitShopState;
		[ConfigProperty("GoHomeState", "回老家活动的开关", true)]
		public static bool GoHomeState;
		[ConfigProperty("SpaPubRoomServerID", "温泉公共房间的频道id", "1|10")]
		public static string SERVERID_SPA_PUBROOM;
		[ConfigProperty("SeizeNpc", "夺宝奇兵活动开关", true)]
		public static bool SEIZENPC;
		[ConfigProperty("SpaPubRoomLoginPay", "登录温泉公共房间的扣费", "10000,200")]
		public static string LOGIN_PAY_SPA_PUBROOM;
		[ConfigProperty("SpaPriRoomInitTime", "温泉私有房间的默认分钟", 60)]
		public static int INIT_TIME_SPA_PRIROOM;
		[ConfigProperty("SpaPriRoomContinueTime", "温泉私有房间的每次续费的延长分钟", 60)]
		public static int CONTINUE_TIME_SPA_PRIROOM;
		[ConfigProperty("SpaPubRoomTimeLimit", "玩家每日在公共房间的时间上限", "60,60")]
		public static string PLAYER_TIMELIMIT_SPA_PUBROOM;
		[ConfigProperty("SpaPubRoomPlayerMaxCount", "温泉公共房间最大人数", "10,10")]
		public static string PLAYER_MAXCOUNT_SPA_PUBROOM;




        private static void Load(Type type)
		{
			using (ServiceBussiness sb = new ServiceBussiness())
			{
				FieldInfo[] fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo f = fields[i];
					if (f.IsStatic)
					{
						object[] attribs = f.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
						if (attribs.Length != 0)
						{
							ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)attribs[0];
							f.SetValue(null, GameProperties.LoadProperty(attrib, sb));
						}
					}
				}
			}
		}
		private static void Save(Type type)
		{
			using (ServiceBussiness sb = new ServiceBussiness())
			{
				FieldInfo[] fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo f = fields[i];
					if (f.IsStatic)
					{
						object[] attribs = f.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
						if (attribs.Length != 0)
						{
							ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)attribs[0];
							GameProperties.SaveProperty(attrib, sb, f.GetValue(null));
						}
					}
				}
			}
		}
		private static object LoadProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb)
		{
			string key = attrib.Key;
			ServerProperty property = sb.GetServerPropertyByKey(key);
			if (property == null)
			{
				property = new ServerProperty();
				property.Key = key;
				property.Value = attrib.DefaultValue.ToString();
				GameProperties.log.Error("Cannot find server property " + key + ",keep it default value!");
			}
			GameProperties.log.Debug("Loading " + key + " Value is " + property.Value);
			object result;
			try
			{
				result = Convert.ChangeType(property.Value, attrib.DefaultValue.GetType());
			}
			catch (Exception e)
			{
				GameProperties.log.Error("Exception in GameProperties Load: ", e);
				result = null;
			}
			return result;
		}
		private static void SaveProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb, object value)
		{
			try
			{
				sb.UpdateServerPropertyByKey(attrib.Key, value.ToString());
			}
			catch (Exception ex)
			{
				GameProperties.log.Error("Exception in GameProperties Save: ", ex);
			}
		}
		public static void Refresh()
		{
			GameProperties.log.Info("Refreshing game properties!");
			GameProperties.Load(typeof(GameProperties));
		}
		public static void Save()
		{
			GameProperties.log.Info("Saving game properties into db!");
			GameProperties.Save(typeof(GameProperties));
		}
	}
}
