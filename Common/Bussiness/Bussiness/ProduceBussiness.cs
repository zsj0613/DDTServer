using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ProduceBussiness : BaseBussiness
	{
		public ItemTemplateInfo[] GetAllGoods()
		{
			List<ItemTemplateInfo> infos = new List<ItemTemplateInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Items_All");
				while (reader.Read())
				{
					infos.Add(this.InitItemTemplateInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ItemTemplateInfo GetSingleGoods(int goodsID)
		{
			SqlDataReader reader = null;
			ItemTemplateInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				para[0].Value = goodsID;
				this.db.GetReader(ref reader, "SP_Items_Single", para);
				if (reader.Read())
				{
					result = this.InitItemTemplateInfo(reader);
					return result;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = null;
			return result;
		}
		public ItemTemplateInfo[] GetSingleCategory(int CategoryID)
		{
			List<ItemTemplateInfo> infos = new List<ItemTemplateInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@CategoryID", SqlDbType.Int, 4)
				};
				para[0].Value = CategoryID;
				this.db.GetReader(ref reader, "SP_Items_Category_Single", para);
				while (reader.Read())
				{
					infos.Add(this.InitItemTemplateInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ItemTemplateInfo InitItemTemplateInfo(SqlDataReader reader)
		{
			return new ItemTemplateInfo
			{
				AddTime = reader["AddTime"].ToString(),
				Agility = (int)reader["Agility"],
				Attack = (int)reader["Attack"],
				CanDelete = (bool)reader["CanDelete"],
				CanDrop = (bool)reader["CanDrop"],
				CanEquip = (bool)reader["CanEquip"],
				CanUse = (bool)reader["CanUse"],
				CategoryID = (int)reader["CategoryID"],
				Colors = reader["Colors"].ToString(),
				Defence = (int)reader["Defence"],
				Description = reader["Description"].ToString(),
				Level = (int)reader["Level"],
				Luck = (int)reader["Luck"],
				MaxCount = (int)reader["MaxCount"],
				Name = reader["Name"].ToString(),
				NeedSex = (int)reader["NeedSex"],
				Pic = reader["Pic"].ToString(),
				Data = (reader["Data"] == null) ? "" : reader["Data"].ToString(),
				Property1 = (int)reader["Property1"],
				Property2 = (int)reader["Property2"],
				Property3 = (int)reader["Property3"],
				Property4 = (int)reader["Property4"],
				Property5 = (int)reader["Property5"],
				Property6 = (int)reader["Property6"],
				Property7 = (int)reader["Property7"],
				Property8 = (int)reader["Property8"],
				Quality = (int)reader["Quality"],
				Script = reader["Script"].ToString(),
				TemplateID = (int)reader["TemplateID"],
				CanCompose = (bool)reader["CanCompose"],
				CanStrengthen = (bool)reader["CanStrengthen"],
				NeedLevel = (int)reader["NeedLevel"],
				BindType = (int)reader["BindType"],
				FusionType = (int)reader["FusionType"],
				FusionRate = (int)reader["FusionRate"],
				FusionNeedRate = (int)reader["FusionNeedRate"],
				Hole = (reader["Hole"] == null) ? "" : reader["Hole"].ToString(),
				RefineryLevel = (int)reader["RefineryLevel"],
				IsDirty = false,
				ReclaimValue = (string)reader["ReclaimValue"],
				ReclaimType = (int)reader["ReclaimType"],
                IsOnly = (bool)reader["IsOnly"]
			};
		}
		public ItemBoxInfo[] GetItemBoxInfos()
		{
			List<ItemBoxInfo> infos = new List<ItemBoxInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_ItemsBox_All");
				while (reader.Read())
				{
					infos.Add(this.InitItemBoxInfo(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init@Shop_Goods_Box：" + e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ItemBoxInfo InitItemBoxInfo(SqlDataReader reader)
		{
			return new ItemBoxInfo
			{
				Id = (int)reader["id"],
				DataId = (int)reader["DataId"],
				TemplateId = (int)reader["TemplateId"],
				IsSelect = (bool)reader["IsSelect"],
				IsBind = (bool)reader["IsBind"],
				ItemValid = (int)reader["ItemValid"],
				ItemCount = (int)reader["ItemCount"],
				StrengthenLevel = (int)reader["StrengthenLevel"],
				AttackCompose = (int)reader["AttackCompose"],
				DefendCompose = (int)reader["DefendCompose"],
				AgilityCompose = (int)reader["AgilityCompose"],
				LuckCompose = (int)reader["LuckCompose"],
				Random = (int)reader["Random"],
				IsTips = (bool)reader["IsTips"],
				IsLogs = (bool)reader["IsLogs"]
			};
		}
		public DropItem[] GetDropItemForNewRegister()
		{
			List<DropItem> infos = new List<DropItem>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Drop_Item_NewRegister");
				while (reader.Read())
				{
					infos.Add(new DropItem
					{
						Id = (int)reader["Id"],
						DropId = (int)reader["DropId"],
						ItemId = (int)reader["ItemId"],
						ValueDate = (int)reader["ValueDate"],
						IsBind = (bool)reader["IsBind"],
						Random = (int)reader["Random"],
						BeginData = (int)reader["BeginData"],
						EndData = (int)reader["EndData"],
						IsTips = (bool)reader["IsTips"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public CategoryInfo[] GetAllCategory()
		{
			List<CategoryInfo> infos = new List<CategoryInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Items_Category_All");
				while (reader.Read())
				{
					infos.Add(new CategoryInfo
					{
						ID = (int)reader["ID"],
						Name = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
						Place = (int)reader["Place"],
						Remark = (reader["Remark"] == null) ? "" : reader["Remark"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public PropInfo[] GetAllProp()
		{
			List<PropInfo> infos = new List<PropInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Prop_All");
				while (reader.Read())
				{
					infos.Add(new PropInfo
					{
						AffectArea = (int)reader["AffectArea"],
						AffectTimes = (int)reader["AffectTimes"],
						AttackTimes = (int)reader["AttackTimes"],
						BoutTimes = (int)reader["BoutTimes"],
						BuyGold = (int)reader["BuyGold"],
						BuyMoney = (int)reader["BuyMoney"],
						Category = (int)reader["Category"],
						Delay = (int)reader["Delay"],
						Description = reader["Description"].ToString(),
						Icon = reader["Icon"].ToString(),
						ID = (int)reader["ID"],
						Name = reader["Name"].ToString(),
						Parameter = (int)reader["Parameter"],
						Pic = reader["Pic"].ToString(),
						Property1 = (int)reader["Property1"],
						Property2 = (int)reader["Property2"],
						Property3 = (int)reader["Property3"],
						Random = (int)reader["Random"],
						Script = reader["Script"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public BallInfo[] GetAllBall()
		{
			List<BallInfo> infos = new List<BallInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Ball_All");
				while (reader.Read())
				{
					infos.Add(new BallInfo
					{
						Amount = (int)reader["Amount"],
						ID = (int)reader["ID"],
						Name = reader["Name"].ToString(),
						Power = (double)reader["Power"],
						Radii = (int)reader["Radii"],
						BombPartical = reader["BombPartical"].ToString(),
						FlyingPartical = reader["FlyingPartical"].ToString(),
						IsSpin = (bool)reader["IsSpin"],
						Mass = (int)reader["Mass"],
						SpinV = (int)reader["SpinV"],
						SpinVA = (double)reader["SpinVA"],
						Wind = (int)reader["Wind"],
						DragIndex = (int)reader["DragIndex"],
						Weight = (int)reader["Weight"],
						Shake = (bool)reader["Shake"],
						Delay = (int)reader["Delay"],
						ShootSound = (reader["ShootSound"] == null) ? "" : reader["ShootSound"].ToString(),
						BombSound = (reader["BombSound"] == null) ? "" : reader["BombSound"].ToString(),
						ActionType = (int)reader["ActionType"],
						HasTunnel = (bool)reader["HasTunnel"]
					});
				}
			}
			catch (Exception e)
			{
				
					BaseBussiness.log.Error("Init", e);
				
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ShopItemInfo[] GetALllShop()
		{
			List<ShopItemInfo> infos = new List<ShopItemInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Shop_All");
				while (reader.Read())
				{
					infos.Add(new ShopItemInfo
					{
						ID = (int)reader["ID"],
						ShopID = (int)reader["ShopID"],
						GroupID = (int)reader["GroupID"],
						TemplateID = (int)reader["TemplateID"],
						BuyType = (int)reader["BuyType"],
						Sort = (int)reader["Sort"],
						IsBind = (int)reader["IsBind"],
						IsVouch = (int)reader["IsVouch"],
						Label = (float)((int)reader["Label"]),
						Beat = (decimal)reader["Beat"],
						AUnit = (int)reader["AUnit"],
						APrice1 = (int)reader["APrice1"],
						AValue1 = (int)reader["AValue1"],
						APrice2 = (int)reader["APrice2"],
						AValue2 = (int)reader["AValue2"],
						APrice3 = (int)reader["APrice3"],
						AValue3 = (int)reader["AValue3"],
						BUnit = (int)reader["BUnit"],
						BPrice1 = (int)reader["BPrice1"],
						BValue1 = (int)reader["BValue1"],
						BPrice2 = (int)reader["BPrice2"],
						BValue2 = (int)reader["BValue2"],
						BPrice3 = (int)reader["BPrice3"],
						BValue3 = (int)reader["BValue3"],
						CUnit = (int)reader["CUnit"],
						CPrice1 = (int)reader["CPrice1"],
						CValue1 = (int)reader["CValue1"],
						CPrice2 = (int)reader["CPrice2"],
						CValue2 = (int)reader["CValue2"],
						CPrice3 = (int)reader["CPrice3"],
						CValue3 = (int)reader["CValue3"],
						LimitCount = (int)reader["LimitCount"],
						IsCheap = (bool)reader["IsCheap"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public FusionInfo[] GetAllFusion()
		{
			List<FusionInfo> infos = new List<FusionInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Fusion_All");
				while (reader.Read())
				{
					infos.Add(new FusionInfo
					{
						FusionID = (int)reader["FusionID"],
						Item1 = (int)reader["Item1"],
						Item2 = (int)reader["Item2"],
						Item3 = (int)reader["Item3"],
						Item4 = (int)reader["Item4"],
						Formula = (int)reader["Formula"],
						Reward = (int)reader["Reward"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllFusion", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public StrengthenInfo[] GetAllStrengthen()
		{
			List<StrengthenInfo> infos = new List<StrengthenInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Item_Strengthen_All");
				while (reader.Read())
				{
					infos.Add(new StrengthenInfo
					{
						StrengthenLevel = (int)reader["StrengthenLevel"],
						Random = (int)reader["Random"],
						Rock = (int)reader["Rock"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllStrengthen", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public StrengthenGoodsInfo[] GetAllStrengthenGoodsInfo()
		{
			List<StrengthenGoodsInfo> infos = new List<StrengthenGoodsInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Item_StrengthenGoodsInfo_All");
				while (reader.Read())
				{
					infos.Add(new StrengthenGoodsInfo
					{
						ID = (int)reader["ID"],
						Level = (int)reader["Level"],
						CurrentEquip = (int)reader["CurrentEquip"],
						GainEquip = (int)reader["GainEquip"],
						OrginEquip = (int)reader["OrginEquip"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllStrengthenGoodsInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public StrengthenInfo[] GetAllRefineryStrengthen()
		{
			List<StrengthenInfo> infos = new List<StrengthenInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Item_Refinery_Strengthen_All");
				while (reader.Read())
				{
					infos.Add(new StrengthenInfo
					{
						StrengthenLevel = (int)reader["StrengthenLevel"],
						Rock = (int)reader["Rock"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllRefineryStrengthen", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public List<RefineryInfo> GetAllRefineryInfo()
		{
			List<RefineryInfo> infos = new List<RefineryInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Item_Refinery_All");
				while (reader.Read())
				{
					infos.Add(new RefineryInfo
					{
						RefineryID = (int)reader["RefineryID"],
						m_Equip = 
						{
							(int)reader["Equip1"],
							(int)reader["Equip2"],
							(int)reader["Equip3"],
							(int)reader["Equip4"]
						},
						Item1 = (int)reader["Item1"],
						Item2 = (int)reader["Item2"],
						Item3 = (int)reader["Item3"],
						Item1Count = (int)reader["Item1Count"],
						Item2Count = (int)reader["Item2Count"],
						Item3Count = (int)reader["Item3Count"],
						m_Reward = 
						{
							(int)reader["Material1"],
							(int)reader["Operate1"],
							(int)reader["Reward1"],
							(int)reader["Material2"],
							(int)reader["Operate2"],
							(int)reader["Reward2"],
							(int)reader["Material3"],
							(int)reader["Operate3"],
							(int)reader["Reward3"],
							(int)reader["Material4"],
							(int)reader["Operate4"],
							(int)reader["Reward4"]
						}
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllRefineryInfo", e);
				}
			}
			finally
			{
				if (reader != null && reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos;
		}
		public QuestInfo[] GetALlQuest()
		{
			List<QuestInfo> infos = new List<QuestInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Quest_All");
				while (reader.Read())
				{
					infos.Add(this.InitQuest(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public QuestAwardInfo[] GetAllQuestGoods()
		{
			List<QuestAwardInfo> infos = new List<QuestAwardInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Quest_Goods_All");
				while (reader.Read())
				{
					infos.Add(this.InitQuestGoods(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public QuestConditionInfo[] GetAllQuestCondiction()
		{
			List<QuestConditionInfo> infos = new List<QuestConditionInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Quest_Condiction_All");
				while (reader.Read())
				{
					infos.Add(this.InitQuestCondiction(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public QuestInfo GetSingleQuest(int questID)
		{
			SqlDataReader reader = null;
			QuestInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@QuestID", SqlDbType.Int, 4)
				};
				para[0].Value = questID;
				this.db.GetReader(ref reader, "SP_Quest_Single", para);
				if (reader.Read())
				{
					result = this.InitQuest(reader);
					return result;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = null;
			return result;
		}
		public QuestInfo InitQuest(SqlDataReader reader)
		{
			return new QuestInfo
			{
				ID = (int)reader["ID"],
				QuestID = (int)reader["QuestID"],
				Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(),
				Detail = (reader["Detail"] == null) ? "" : reader["Detail"].ToString(),
				Objective = (reader["Objective"] == null) ? "" : reader["Objective"].ToString(),
				NeedMinLevel = (int)reader["NeedMinLevel"],
				NeedMaxLevel = (int)reader["NeedMaxLevel"],
				PreQuestID = (reader["PreQuestID"] == null) ? "" : reader["PreQuestID"].ToString(),
				NextQuestID = (reader["NextQuestID"] == null) ? "" : reader["NextQuestID"].ToString(),
				IsOther = (int)reader["IsOther"],
				CanRepeat = (bool)reader["CanRepeat"],
				RepeatInterval = (int)reader["RepeatInterval"],
				RepeatMax = (int)reader["RepeatMax"],
				RewardGP = (int)reader["RewardGP"],
				RewardGold = (int)reader["RewardGold"],
				RewardGiftToken = (int)reader["RewardGiftToken"],
				RewardOffer = (int)reader["RewardOffer"],
				RewardRiches = (int)reader["RewardRiches"],
				RewardBuffID = (int)reader["RewardBuffID"],
				RewardBuffDate = (int)reader["RewardBuffDate"],
				RewardMoney = (int)reader["RewardMoney"],
				Rands = (decimal)reader["Rands"],
				RandDouble = (int)reader["RandDouble"],
				TimeMode = (bool)reader["TimeMode"],
				StartDate = (DateTime)reader["StartDate"],
				EndDate = (DateTime)reader["EndDate"]
			};
		}
		public QuestAwardInfo InitQuestGoods(SqlDataReader reader)
		{
			return new QuestAwardInfo
			{
				QuestID = (int)reader["QuestID"],
				RewardItemID = (int)reader["RewardItemID"],
				IsSelect = (bool)reader["IsSelect"],
				RewardItemValid = (int)reader["RewardItemValid"],
				RewardItemCount = (int)reader["RewardItemCount"],
				StrengthenLevel = (int)reader["StrengthenLevel"],
				AttackCompose = (int)reader["AttackCompose"],
				DefendCompose = (int)reader["DefendCompose"],
				AgilityCompose = (int)reader["AgilityCompose"],
				LuckCompose = (int)reader["LuckCompose"],
				IsCount = (bool)reader["IsCount"]
			};
		}
		public QuestConditionInfo InitQuestCondiction(SqlDataReader reader)
		{
			return new QuestConditionInfo
			{
				QuestID = (int)reader["QuestID"],
				CondictionID = (int)reader["CondictionID"],
				CondictionTitle = (reader["CondictionTitle"] == null) ? "" : reader["CondictionTitle"].ToString(),
				CondictionType = (int)reader["CondictionType"],
				Para1 = (int)reader["Para1"],
				Para2 = (int)reader["Para2"]
			};
		}
		public DropCondiction[] GetAllDropCondictions()
		{
			List<DropCondiction> infos = new List<DropCondiction>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Drop_Condiction_All");
				while (reader.Read())
				{
					infos.Add(this.InitDropCondiction(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public DropItem[] GetAllDropItems()
		{
			List<DropItem> infos = new List<DropItem>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Drop_Item_All");
				while (reader.Read())
				{
					infos.Add(this.InitDropItem(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public DropCondiction InitDropCondiction(SqlDataReader reader)
		{
			return new DropCondiction
			{
				DropId = (int)reader["DropID"],
				CondictionType = (int)reader["CondictionType"],
				Para1 = (string)reader["Para1"],
				Para2 = (string)reader["Para2"]
			};
		}
		public DropItem InitDropItem(SqlDataReader reader)
		{
			return new DropItem
			{
				Id = (int)reader["Id"],
				DropId = (int)reader["DropId"],
				ItemId = (int)reader["ItemId"],
				ValueDate = (int)reader["ValueDate"],
				IsBind = (bool)reader["IsBind"],
				Random = (int)reader["Random"],
				BeginData = (int)reader["BeginData"],
				EndData = (int)reader["EndData"],
				IsTips = (bool)reader["IsTips"],
				IsLogs = (bool)reader["IsLogs"]
			};
		}
		public AASInfo[] GetAllAASInfo()
		{
			List<AASInfo> infos = new List<AASInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_AASInfo_All");
				while (reader.Read())
				{
					infos.Add(new AASInfo
					{
						UserID = (int)reader["ID"],
						Name = reader["Name"].ToString(),
						IDNumber = reader["IDNumber"].ToString(),
						State = (int)reader["State"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllAASInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public bool AddAASInfo(AASInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Name", info.Name),
					new SqlParameter("@IDNumber", info.IDNumber),
					new SqlParameter("@State", info.State),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[4].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ASSInfo_Add", para);
				result = ((int)para[4].Value == 0);
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("UpdateAASInfo", e);
				}
			}
			return result;
		}
		public bool AddAreaBigBugleRecord(int userid, int areaid, string nickname, string message, string ip)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userid),
					new SqlParameter("@AreaID", areaid),
					new SqlParameter("@NickName", nickname),
					new SqlParameter("@Message", message),
					new SqlParameter("@IP", ip),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[5].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_AreaBigBugle_Record", para);
				result = ((int)para[5].Value == 0);
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("AddAreaBigBugleRecord", e);
				}
			}
			return result;
		}
		public List<BigBugleInfo> GetAllAreaBigBugleRecord()
		{
			SqlDataReader reader = null;
			List<BigBugleInfo> list = new List<BigBugleInfo>();
			try
			{
				this.db.GetReader(ref reader, "SP_Get_AreaBigBugle_Record");
				while (reader.Read())
				{
					list.Add(new BigBugleInfo
					{
						ID = (int)reader["ID"],
						UserID = (int)reader["UserID"],
						AreaID = (int)reader["AreaID"],
						NickName = (reader["NickName"] == null) ? "" : reader["NickName"].ToString(),
						Message = (reader["Message"] == null) ? "" : reader["Message"].ToString(),
						State = (bool)reader["State"],
						IP = (reader["IP"] == null) ? "" : reader["IP"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllAreaBigBugleRecord", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return list;
		}
		public bool UpdateAreaBigBugleRecord(int id)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", id),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[1].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Update_AreaBigBugle_Record", para);
				result = ((int)para[1].Value == 0);
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("SP_Update_AreaBigBugle_Record", e);
				}
			}
			return result;
		}
		public string GetASSInfoSingle(int UserID)
		{
			SqlDataReader reader = null;
			string result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", UserID)
				};
				this.db.GetReader(ref reader, "SP_ASSInfo_Single", para);
				if (reader.Read())
				{
					string ID = reader["IDNumber"].ToString();
					result = ID;
					return result;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetASSInfoSingle", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = "";
			return result;
		}
		public DailyAwardInfo[] GetAllDailyAward()
		{
			List<DailyAwardInfo> infos = new List<DailyAwardInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Daily_Award_All");
				while (reader.Read())
				{
					infos.Add(new DailyAwardInfo
					{
						Count = (int)reader["Count"],
						ID = (int)reader["ID"],
						IsBinds = (bool)reader["IsBinds"],
						TemplateID = (int)reader["TemplateID"],
						Type = (int)reader["Type"],
						ValidDate = (int)reader["ValidDate"],
						Sex = (int)reader["Sex"],
						Remark = (reader["Remark"] == null) ? "" : reader["Remark"].ToString(),
						CountRemark = (reader["CountRemark"] == null) ? "" : reader["CountRemark"].ToString(),
						GetWay = (int)reader["GetWay"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllDaily", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public NpcInfo[] GetAllNPCInfo()
		{
			List<NpcInfo> infos = new List<NpcInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_NPC_Info_All");
				while (reader.Read())
				{
					infos.Add(new NpcInfo
					{
						ID = (int)reader["ID"],
						Name = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
						Level = (int)reader["Level"],
						Camp = (int)reader["Camp"],
						Type = (int)reader["Type"],
						Blood = (int)reader["Blood"],
						X = (int)reader["X"],
						Y = (int)reader["Y"],
						Width = (int)reader["Width"],
						Height = (int)reader["Height"],
						MoveMin = (int)reader["MoveMin"],
						MoveMax = (int)reader["MoveMax"],
						BaseDamage = (int)reader["BaseDamage"],
						BaseGuard = (int)reader["BaseGuard"],
						Attack = (int)reader["Attack"],
						Defence = (int)reader["Defence"],
						Agility = (int)reader["Agility"],
						Lucky = (int)reader["Lucky"],
						ModelID = (reader["ModelID"] == null) ? "" : reader["ModelID"].ToString(),
						ResourcesPath = (reader["ResourcesPath"] == null) ? "" : reader["ResourcesPath"].ToString(),
						DropRate = (reader["DropRate"] == null) ? 2 : Convert.ToInt32(reader["DropRate"].ToString()),
						Experience = (int)reader["Experience"],
						Delay = (int)reader["Delay"],
						Immunity = (int)reader["Immunity"],
						Alert = (int)reader["Alert"],
						Range = (int)reader["Range"],
						Preserve = (int)reader["Preserve"],
						Script = (reader["Script"] == null) ? "" : reader["Script"].ToString(),
						FireX = (int)reader["FireX"],
						FireY = (int)reader["FireY"],
						DropId = (int)reader["DropId"],
						MaxBeatDis = (int)reader["MaxBeatDis"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllNPCInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public MissionInfo[] GetAllMissionInfo()
		{
			List<MissionInfo> infos = new List<MissionInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Mission_Info_All");
				while (reader.Read())
				{
					infos.Add(new MissionInfo
					{
						Id = (int)reader["ID"],
						Name = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
						TotalCount = (int)reader["TotalCount"],
						TotalTurn = (int)reader["TotalTurn"],
						Script = (reader["Script"] == null) ? "" : reader["Script"].ToString(),
						Success = (reader["Success"] == null) ? "" : reader["Success"].ToString(),
						Failure = (reader["Failure"] == null) ? "" : reader["Failure"].ToString(),
						Description = (reader["Description"] == null) ? "" : reader["Description"].ToString(),
						IncrementDelay = (int)reader["IncrementDelay"],
						Delay = (int)reader["Delay"],
						Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(),
						Param1 = (int)reader["Param1"],
						Param2 = (int)reader["Param2"],
						TakeCard = (int)reader["TakeCard"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllMissionInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ItemRecordTypeInfo[] GetAllItemRecordType()
		{
			List<ItemRecordTypeInfo> infos = new List<ItemRecordTypeInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Item_Record_Type_All");
				while (reader.Read())
				{
					infos.Add(this.InitItemRecordType(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllItemRecordType:", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public AchievementInfo[] GetALlAchievement()
		{
			List<AchievementInfo> infos = new List<AchievementInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Achievement_All");
				while (reader.Read())
				{
					infos.Add(this.InitAchievement(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetALlAchievement:", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public AchievementConditionInfo[] GetALlAchievementCondition()
		{
			List<AchievementConditionInfo> infos = new List<AchievementConditionInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Achievement_Condition_All");
				while (reader.Read())
				{
					infos.Add(this.InitAchievementCondition(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetALlAchievementCondition:", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public AchievementRewardInfo[] GetALlAchievementReward()
		{
			List<AchievementRewardInfo> infos = new List<AchievementRewardInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Achievement_Reward_All");
				while (reader.Read())
				{
					infos.Add(this.InitAchievementReward(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetALlAchievementReward", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public AchievementDataInfo[] GetAllAchievementData(int userID)
		{
			List<AchievementDataInfo> infos = new List<AchievementDataInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID)
				};
				this.db.GetReader(ref reader, "SP_Achievement_Data_All", para);
				while (reader.Read())
				{
					infos.Add(this.InitAchievementData(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllAchievementData", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public UsersRecordInfo[] GetAllUsersRecord(int userID)
		{
			List<UsersRecordInfo> infos = new List<UsersRecordInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID)
				};
				this.db.GetReader(ref reader, "SP_Users_Record_All", para);
				while (reader.Read())
				{
					infos.Add(this.InitUsersRecord(reader));
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllUsersRecord", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ItemRecordTypeInfo InitItemRecordType(SqlDataReader reader)
		{
			return new ItemRecordTypeInfo
			{
				RecordID = (int)reader["RecordID"],
				Name = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
				Description = (reader["Description"] == null) ? "" : reader["Description"].ToString()
			};
		}
		public AchievementInfo InitAchievement(SqlDataReader reader)
		{
			return new AchievementInfo
			{
				ID = (int)reader["ID"],
				PlaceID = (int)reader["PlaceID"],
				Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(),
				Detail = (reader["Detail"] == null) ? "" : reader["Detail"].ToString(),
				NeedMinLevel = (int)reader["NeedMinLevel"],
				NeedMaxLevel = (int)reader["NeedMaxLevel"],
				PreAchievementID = (reader["PreAchievementID"] == null) ? "" : reader["PreAchievementID"].ToString(),
				IsOther = (int)reader["IsOther"],
				AchievementType = (int)reader["AchievementType"],
				CanHide = (bool)reader["CanHide"],
				StartDate = (DateTime)reader["StartDate"],
				EndDate = (DateTime)reader["EndDate"],
				AchievementPoint = (int)reader["AchievementPoint"]
			};
		}
		public AchievementConditionInfo InitAchievementCondition(SqlDataReader reader)
		{
			return new AchievementConditionInfo
			{
				AchievementID = (int)reader["AchievementID"],
				CondictionID = (int)reader["CondictionID"],
				CondictionType = (int)reader["CondictionType"],
				Condiction_Para1 = (reader["Condiction_Para1"] == null) ? "" : reader["Condiction_Para1"].ToString(),
				Condiction_Para2 = (int)reader["Condiction_Para2"]
			};
		}
		public AchievementRewardInfo InitAchievementReward(SqlDataReader reader)
		{
			return new AchievementRewardInfo
			{
				AchievementID = (int)reader["AchievementID"],
				RewardType = (int)reader["RewardType"],
				RewardPara = (reader["RewardPara"] == null) ? "" : reader["RewardPara"].ToString(),
				RewardValueId = (int)reader["RewardValueId"],
				RewardCount = (int)reader["RewardCount"]
			};
		}
		public AchievementDataInfo InitAchievementData(SqlDataReader reader)
		{
			return new AchievementDataInfo
			{
				UserID = (int)reader["UserID"],
				AchievementID = (int)reader["AchievementID"],
				IsComplete = (bool)reader["IsComplete"],
				CompletedDate = (DateTime)reader["CompletedDate"]
			};
		}
		public UsersRecordInfo InitUsersRecord(SqlDataReader reader)
		{
			return new UsersRecordInfo
			{
				UserID = (int)reader["UserID"],
				RecordID = (int)reader["RecordID"],
				Total = (int)reader["Total"]
			};
		}
		public List<BoxInfo> GetAllBoxInfo()
		{
			List<BoxInfo> infos = new List<BoxInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Box_Info");
				while (reader.Read())
				{
					infos.Add(new BoxInfo
					{
						ID = (int)reader["ID"],
						Type = (int)reader["Type"],
						Level = (int)reader["Level"],
						Condition = (int)reader["Condition"],
						Template = (reader["Template"] == null) ? "" : reader["Template"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllBoxInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos;
		}
		public bool UpdateBoxProgression(int userid, int boxProgression, int getBoxLevel, DateTime addGPLastDate, DateTime BoxGetDate, int alreadyBox)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userid),
					new SqlParameter("@BoxProgression", boxProgression),
					new SqlParameter("@GetBoxLevel", getBoxLevel),
					new SqlParameter("@AddGPLastDate", addGPLastDate),
					new SqlParameter("@BoxGetDate", BoxGetDate),
					new SqlParameter("@AlreadyGetBox", alreadyBox),
					new SqlParameter("@OutPut", SqlDbType.Int)
				};
				para[6].Direction = ParameterDirection.Output;
				this.db.RunProcedure("SP_User_Update_BoxProgression", para);
				result = ((int)para[6].Value == 1);
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("User_Update_BoxProgression", e);
				}
			}
			return result;
		}
		public ActiveConditionInfo[] GetAllActiveConditionInfo()
		{
			List<ActiveConditionInfo> infos = new List<ActiveConditionInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Active_Condition");
				while (reader.Read())
				{
					infos.Add(new ActiveConditionInfo
					{
						ID = (int)reader["ID"],
						ActiveID = (int)reader["ActiveID"],
						Conditiontype = (int)reader["Conditiontype"],
						Condition = (int)reader["Condition"],
						LimitGrade = (reader["LimitGrade"].ToString() == null) ? "" : reader["LimitGrade"].ToString(),
						AwardId = (reader["AwardId"].ToString() == null) ? "" : reader["AwardId"].ToString(),
						IsMult = (bool)reader["IsMult"],
						StartTime = (DateTime)reader["StartTime"],
						EndTime = (DateTime)reader["EndTime"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllActiveConditionInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ActiveAwardInfo[] GetAllActiveAwardInfo()
		{
			List<ActiveAwardInfo> infos = new List<ActiveAwardInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Active_Award");
				while (reader.Read())
				{
					infos.Add(new ActiveAwardInfo
					{
						ID = (int)reader["ID"],
						ActiveID = (int)reader["ActiveID"],
						AgilityCompose = (int)reader["AgilityCompose"],
						AttackCompose = (int)reader["AttackCompose"],
						Count = (int)reader["Count"],
						DefendCompose = (int)reader["DefendCompose"],
						Gold = (int)reader["Gold"],
						ItemID = (int)reader["ItemID"],
						LuckCompose = (int)reader["LuckCompose"],
						Mark = (int)reader["Mark"],
						Money = (int)reader["Money"],
						Sex = (int)reader["Sex"],
						StrengthenLevel = (int)reader["StrengthenLevel"],
						ValidDate = (int)reader["ValidDate"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllActiveAwardInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public Dictionary<int, int> GetShopLimitCount()
		{
			Dictionary<int, int> infos = new Dictionary<int, int>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Shop_ByLimitCount");
				while (reader.Read())
				{
					infos.Add((int)reader["ID"], (int)reader["LimitCount"]);
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllMissionInfo", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos;
		}
	}
}
