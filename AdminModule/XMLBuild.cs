using Bussiness;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using zlib;

namespace Web.Server.Module
{
    public class XMLBuild
    {
        public static byte[] Compress(string str)
        {
            byte[] src = Encoding.UTF8.GetBytes(str);
            return Compress(src);
        }
        public static byte[] Compress(byte[] src)
        {
            return Compress(src, 0, src.Length);
        }
        public static byte[] Compress(byte[] src, int offset, int length)
        {
            MemoryStream ms = new MemoryStream();
            Stream s = new ZOutputStream(ms, 9);
            s.Write(src, offset, length);
            s.Close();
            return ms.ToArray();
        }
        public static string CreateCompressXml(string path, XElement result, string file, bool isCompress, string message)
        {
            string result2;
            try
            {
                file += ".xml";
                path += file;
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    if (isCompress)
                    {
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            writer.Write(Compress(result.ToString(false)));
                        }
                    }
                    else
                    {
                        using (StreamWriter wirter = new StreamWriter(fs))
                        {
                            wirter.Write(result.ToString(false));
                        }
                    }
                }
                result2 = "Build:" + file + "," + message;
            }
            catch(Exception ex)
            {
                WebServer.log.Error("CreateCompressXml " + file + " is fail!", ex);
                result2 = "Build:" + file + ",Fail!";
            }
            return result2;
        }
        public static string CreateCompressXml(string path, XElement result, string file, bool isCompress)
        {
            string result2;
            try
            {
                file += ".xml";
                path += file;
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    if (isCompress)
                    {
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            writer.Write(Compress(result.ToString(false)));
                        }
                    }
                    else
                    {
                        using (StreamWriter wirter = new StreamWriter(fs))
                        {
                            wirter.Write(result.ToString(false));
                        }
                    }
                }
                result2 = "Build:" + file + ", Success!";
            }
            catch(Exception ex)
            {
                WebServer.log.Error("CreateCompressXml " + file + " is fail!", ex);
                result2 = "Build:" + file + ",Fail!";
            }
            return result2;
        }
        public static string LoadBoxTempBuild(string path)
        {
            bool value = false;
            string message = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    ItemBoxInfo[] infos = db.GetItemBoxInfos();
                    if (infos != null && infos.Length > 0)
                    {
                        ItemBoxInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            ItemBoxInfo info = array[i];
                            result.Add(FlashUtils.CreateItemBox(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "BoxTemplateList", false, message);
        }
        public static string FightLabDropItemListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, 11021, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, 11002, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, 11006, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, 11010, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, 11014, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, 11408, 1));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, 11107, 2000));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 0, -300, 100));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, 11022, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, 11003, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, 11007, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, 11011, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, 11015, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, 11408, 3));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, 11107, 5000));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 1, -300, 300));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, 11023, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, 11004, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, 11008, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, 11012, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, 11016, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, 11408, 5));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, 11107, 8000));
                result.Add(FlashUtils.CreateFightLabDropItem(5, 2, -300, 500));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, 11021, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, 11002, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, 11006, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, 11010, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, 11014, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, 11408, 1));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, 11107, 2000));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 0, -300, 100));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, 11022, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, 11003, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, 11007, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, 11011, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, 11015, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, 11408, 3));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, 11107, 5000));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 1, -300, 300));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, 11023, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, 11004, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, 11008, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, 11012, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, 11016, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, 11408, 5));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, 11107, 8000));
                result.Add(FlashUtils.CreateFightLabDropItem(6, 2, -300, 500));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, 11021, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, 11002, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, 11006, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, 11010, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, 11014, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, 11408, 1));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, 11107, 2000));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 0, -300, 100));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, 11022, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, 11003, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, 11007, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, 11011, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, 11015, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, 11408, 3));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, 11107, 5000));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 1, -300, 300));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, 11023, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, 11004, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, 11008, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, 11012, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, 11016, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, 11408, 5));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, 11107, 8000));
                result.Add(FlashUtils.CreateFightLabDropItem(7, 2, -300, 500));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, 11021, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, 11002, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, 11006, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, 11010, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, 11014, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, 11408, 1));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, 11107, 2000));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 0, -300, 100));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, 11022, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, 11003, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, 11007, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, 11011, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, 11015, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, 11408, 3));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, 11107, 5000));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 1, -300, 300));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, 11023, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, 11004, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, 11008, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, 11012, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, 11016, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, 11408, 5));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, 11107, 8000));
                result.Add(FlashUtils.CreateFightLabDropItem(8, 2, -300, 500));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, 11021, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, 11002, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, 11006, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, 11010, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, 11014, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, 11408, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, 11107, 2000));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 0, -300, 100));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, 11022, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, 11003, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, 11007, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, 11011, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, 11015, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, 11408, 3));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, 11107, 5000));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 1, -300, 300));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, 11023, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, 11004, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, 11008, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, 11012, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, 11016, 4));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, 11408, 5));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, 11107, 8000));
                result.Add(FlashUtils.CreateFightLabDropItem(9, 2, -300, 500));
                value = true;
                message = "Success!";
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "FightLabDropItemList", true, message);
        }

        public static string LoadUserBoxBuild(string path)
        {
            bool value = false;
            string message = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    List<BoxInfo> infos = db.GetAllBoxInfo();
                    if (infos != null && infos.Count<BoxInfo>() > 0)
                    {
                        foreach (BoxInfo info in infos)
                        {
                            result.Add(FlashUtils.CreateUserBox(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "LoadUserBox", false, message);
        }
        public static string AchievementListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    AchievementInfo[] achievements = db.GetALlAchievement();
                    AchievementConditionInfo[] achievementConditions = db.GetALlAchievementCondition();
                    AchievementRewardInfo[] achievementRewards = db.GetALlAchievementReward();
                    if (achievements != null && achievementConditions != null && achievementRewards != null && achievements.Length > 0 && achievementConditions.Length > 0 && achievementRewards.Length > 0)
                    {
                        AchievementInfo[] array = achievements;
                        AchievementInfo achievement;
                        for (int i = 0; i < array.Length; i++)
                        {
                            achievement = array[i];
                            XElement temp_xml = FlashUtils.CreateAchievement(achievement);
                            IEnumerable temp_achievementConditions =
                                from s in achievementConditions
                                where s.AchievementID == achievement.ID
                                select s;
                            foreach (AchievementConditionInfo achievementCondition in temp_achievementConditions)
                            {
                                temp_xml.Add(FlashUtils.CreateAchievementCondition(achievementCondition));
                            }
                            IEnumerable temp_achievementRewards =
                                from s in achievementRewards
                                where s.AchievementID == achievement.ID
                                select s;
                            foreach (AchievementRewardInfo achievementReward in temp_achievementRewards)
                            {
                                temp_xml.Add(FlashUtils.CreateAchievementReward(achievementReward));
                            }
                            result.Add(temp_xml);
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "AchievementList", true, message);
        }
        public static string DropItemForNewRegisterBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness pb = new ProduceBussiness())
                {
                    DropItem[] infos = pb.GetDropItemForNewRegister();
                    if (infos != null && infos.Length > 0)
                    {
                        DropItem[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            DropItem info = array[i];
                            result.Add(FlashUtils.CreateDropItemForNewRegister(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "DropList", true, message);
        }

        public static string NPCInfoListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    NpcInfo[] infos = db.GetAllNPCInfo();
                    if (infos != null && infos.Length > 0)
                    {
                        NpcInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            NpcInfo info = array[i];
                            result.Add(FlashUtils.CreatNPCInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "NPCInfoList", true, message);
        }
        public static string DailyAwardListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    DailyAwardInfo[] infos = db.GetAllDailyAward();
                    if (infos != null && infos.Length > 0)
                    {
                        DailyAwardInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            DailyAwardInfo info = array[i];
                            result.Add(FlashUtils.CreateActiveInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "DailyAwardList", true, message);
        }
        public static string ConsortiaLevelListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    ConsortiaLevelInfo[] infos = db.GetAllConsortiaLevel();
                    if (infos != null && infos.Length > 0)
                    {
                        ConsortiaLevelInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            ConsortiaLevelInfo info = array[i];
                            result.Add(FlashUtils.CreateConsortiLevelInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "ConsortiaLevelList", true, message);
        }
        public static string MapServerListBulid(string path)
        {
            bool value = false;
            string message = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (MapBussiness db = new MapBussiness())
                {
                    ServerMapInfo[] infos = db.GetAllServerMap();
                    if (infos != null && infos.Length > 0)
                    {
                        ServerMapInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            ServerMapInfo info = array[i];
                            result.Add(FlashUtils.CreateMapServer(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "MapServerList", true, message);
        }
        public static string ItemStrengthenListBulid(string path)
        {
            bool value = false;
            string message = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    StrengthenInfo[] infos = db.GetAllStrengthen();
                    if (infos != null && infos.Length > 0)
                    {
                        StrengthenInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            StrengthenInfo info = array[i];
                            result.Add(FlashUtils.CreateStrengthenInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "ItemStrengthenList", true, message);
        }
        public static string LoadItemsCategoryBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    CategoryInfo[] infos = db.GetAllCategory();
                    if (infos != null && infos.Length > 0)
                    {
                        CategoryInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            CategoryInfo info = array[i];
                            result.Add(FlashUtils.CreateCategoryInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "ItemsCategory", true, message);
        }
        public static string ShopItemListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    XElement Store = new XElement("Store");
                    ShopItemInfo[] shop = db.GetALllShop();
                    if (shop != null && shop.Length > 0)
                    {
                        ShopItemInfo[] array = shop;
                        for (int i = 0; i < array.Length; i++)
                        {
                            ShopItemInfo s = array[i];
                            Store.Add(FlashUtils.CreateShopInfo(s));
                        }
                        result.Add(Store);
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "ShopItemList", true, message);
        }
        public static string TemplateAllListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    XElement template = new XElement("ItemTemplate");
                    ItemTemplateInfo[] items = db.GetAllGoods();
                    if (items != null && items.Length > 0)
                    {
                        ItemTemplateInfo[] array = items;
                        for (int i = 0; i < array.Length; i++)
                        {
                            ItemTemplateInfo g = array[i];
                            template.Add(FlashUtils.CreateItemInfo(g));
                        }
                        result.Add(template);
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "TemplateAlllist", true, message);
        }
        public static string QuestListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    QuestInfo[] quests = db.GetALlQuest();
                    QuestAwardInfo[] questgoods = db.GetAllQuestGoods();
                    QuestConditionInfo[] questcondiction = db.GetAllQuestCondiction();
                    if (quests != null && questgoods != null && questcondiction != null && quests.Length > 0 && questgoods.Length > 0 && questcondiction.Length > 0)
                    {
                        QuestInfo[] array = quests;
                        QuestInfo quest;
                        for (int i = 0; i < array.Length; i++)
                        {
                            quest = array[i];
                            XElement temp_xml = FlashUtils.CreateQuestInfo(quest);
                            IEnumerable temp_questcondiction =
                                from s in questcondiction
                                where s.QuestID == quest.ID
                                select s;
                            foreach (QuestConditionInfo item in temp_questcondiction)
                            {
                                temp_xml.Add(FlashUtils.CreateQuestCondiction(item));
                            }
                            IEnumerable temp_questgoods =
                                from s in questgoods
                                where s.QuestID == quest.ID
                                select s;
                            foreach (QuestAwardInfo item2 in temp_questgoods)
                            {
                                temp_xml.Add(FlashUtils.CreateQuestGoods(item2));
                            }
                            result.Add(temp_xml);
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "QuestList", true, message);
        }
        private static void AppendAttribute(XmlDocument doc, XmlNode node, string attr, string value)
        {
            XmlAttribute at = doc.CreateAttribute(attr);
            at.Value = value;
            node.Attributes.Append(at);
        }
        public static string LoadPVEItemsBuild(string path)
        {
            bool value = false;
            string message = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (PveBussiness db = new PveBussiness())
                {
                    PveInfo[] infos = db.GetAllPveInfos();
                    if (infos != null && infos.Length > 0)
                    {
                        PveInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            PveInfo info = array[i];
                            if (info.Type == 5)
                            {
                            }
                            result.Add(FlashUtils.CreatePveInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "PVEList", true, message);
        }
        public static string LoadMapsItemsBulid(string path)
        {
            bool value = false;
            string message = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (MapBussiness db = new MapBussiness())
                {
                    MapInfo[] infos = db.GetAllMap();
                    if (infos != null && infos.Length > 0)
                    {
                        MapInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            MapInfo info = array[i];
                            result.Add(FlashUtils.CreateMapInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "MapList", true, message);
        }
        public static string BallListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    BallInfo[] infos = db.GetAllBall();
                    if (infos != null && infos.Length > 0)
                    {
                        BallInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            BallInfo info = array[i];
                            result.Add(FlashUtils.CreateBallInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "BallList", true, message);
        }
        public static string ActiveListBulid(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {                
                using (ActiveBussiness db = new ActiveBussiness())
                {
                    ActiveInfo[] infos = db.GetAllActives();
                    if (infos != null && infos.Length > 0)
                    {
                        ActiveInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            ActiveInfo info = array[i];
                            result.Add(FlashUtils.CreateActiveInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                    else
                    {
                        value = false;
                        message = "Fail!";
                    }
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error(ex);
            }

            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "ActiveList", true, message);
        }


        public static string CelebByDayBestEquipBuild(string path)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    BestEquipInfo[] infos = db.GetCelebByDayBestEquip();
                    BestEquipInfo[] array = infos;
                    for (int i = 0; i < array.Length; i++)
                    {
                        BestEquipInfo info = array[i];
                        result.Add(FlashUtils.CreateBestEquipInfo(info));
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception ex)
            {

            }
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            return CreateCompressXml(path, result, "CelebForBestEquip", false);
        }

        public static string BuildCelebConsortia(string path, string file, int order)
        {
            return BuildCelebConsortia(path, file, order, "");
        }
        public static string BuildCelebConsortia(string path, string file, int order, string fileNotCompress)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            int total = 0;
            try
            {
                int page = 1;
                int size = 100;
                int consortiaID = -1;
                string name = "";
                int level = -1;
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    ConsortiaInfo[] infos = db.GetConsortiaPage(page, size, ref total, order, name, consortiaID, level, -1);
                    ConsortiaInfo[] array = infos;
                    for (int i = 0; i < array.Length; i++)
                    {
                        ConsortiaInfo info = array[i];
                        XElement node = FlashUtils.CreateConsortiaInfo(info);
                        if (info.ChairmanID != 0)
                        {
                            using (PlayerBussiness pb = new PlayerBussiness())
                            {
                                PlayerInfo player = pb.GetUserSingleByUserID(info.ChairmanID);
                                if (player != null)
                                {
                                    node.Add(FlashUtils.CreateCelebInfo(player));
                                }
                            }
                        }
                        result.Add(node);
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch(Exception ex)
            {
                WebServer.log.Error(file + " is fail!", ex);
            }
            result.Add(new XAttribute("total", total));
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            result.Add(new XAttribute("date", DateTime.Now.ToString("MM-dd HH:mm")));
            if (!string.IsNullOrEmpty(fileNotCompress))
            {
                CreateCompressXml(path, result, fileNotCompress, false);
            }
            return CreateCompressXml(path, result, file, true);
        }
        public static string BuildCelebUsers(string path, string file, int order)
        {
            return BuildCelebUsers(path, file, order, "");
        }
        public static string BuildCelebUsers(string path, string file, int order, string fileNotCompress)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                int page = 1;
                int size = 100;
                int userID = -1;
                int total = 0;
                bool resultValue = false;
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    PlayerInfo[] infos = db.GetPlayerPage(page, size, ref total, order, userID, ref resultValue);
                    if (resultValue)
                    {
                        PlayerInfo[] array = infos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            PlayerInfo info = array[i];
                            result.Add(FlashUtils.CreateCelebInfo(info));
                        }
                        value = true;
                        message = "Success!";
                    }
                }
            }
            catch(Exception ex)
            {
                WebServer.log.Error(file + " is fail!", ex);
            }
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            result.Add(new XAttribute("date", DateTime.Now.ToString("MM-dd HH:mm")));
            if (!string.IsNullOrEmpty(fileNotCompress))
            {
                CreateCompressXml(path, result, fileNotCompress, false);
            }
            return CreateCompressXml(path, result, file, true);
        }
    }
}
