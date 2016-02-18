using Bussiness.CenterService;
using Bussiness.Managers;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class PlayerBussiness : BaseBussiness
    {

        public bool CreateUsername(string username, int inviteid)
        {
            SqlParameter[] para = new SqlParameter[]
            {
                new SqlParameter("@UserName", username),
                new SqlParameter("@Inviteid", inviteid),
                new SqlParameter("@Result", SqlDbType.Int)
            };
            para[2].Direction = ParameterDirection.ReturnValue;
            bool result = this.db.RunProcedure("Mem_Users_CreateUser", para);
            if (result)
            {
                result = ((int)para[2].Value > 0);
            }
            return result;
        }
        public int GetUserType(string username)
        {
            SqlParameter[] array = new SqlParameter[]
            {
                new SqlParameter("@UserName", username),
                new SqlParameter("@result", SqlDbType.Int)
            };
            array[1].Direction = ParameterDirection.ReturnValue;
            bool flag = this.db.RunProcedure("Mem_Users_GetType", array);
            int num = 0;
            int.TryParse(array[1].Value.ToString(), out num);
            return num;
        }
        public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, int giftToken, string IP, string site)
        {
            LogProvider.Default.Debug("ActivePlayer");
            bool result = false;
            //try
            //{
            player = new PlayerInfo();
            player.Agility = 0;
            player.Attack = 0;
            player.Colors = ",,,,,,";
            player.Skin = "";
            player.ConsortiaID = 0;
            player.Defence = 0;
            player.Gold = gold;
            player.GP = 1;
            player.Grade = 1;
            player.ID = 0;
            player.Luck = 0;
            player.Money = money;
            player.GiftToken = giftToken;
            player.NickName = "";
            player.Sex = sex;
            player.State = 0;
            player.Style = ",,,,,,";
            player.Hide = 1111111111;
            SqlParameter[] para = new SqlParameter[22];
            para[0] = new SqlParameter("@UserID", SqlDbType.Int);
            para[0].Direction = ParameterDirection.Output;
            para[1] = new SqlParameter("@Attack", player.Attack);
            para[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
            para[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
            para[4] = new SqlParameter("@Defence", player.Defence);
            para[5] = new SqlParameter("@Gold", player.Gold);
            para[6] = new SqlParameter("@GP", player.GP);
            para[7] = new SqlParameter("@Grade", player.Grade);
            para[8] = new SqlParameter("@Luck", player.Luck);
            para[9] = new SqlParameter("@Money", player.Money);
            para[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
            para[11] = new SqlParameter("@Agility", player.Agility);
            para[12] = new SqlParameter("@State", player.State);
            para[13] = new SqlParameter("@UserName", userName);
            para[14] = new SqlParameter("@PassWord", passWord);
            para[15] = new SqlParameter("@Sex", sex);
            para[16] = new SqlParameter("@Hide", player.Hide);
            para[17] = new SqlParameter("@ActiveIP", IP);
            para[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
            para[19] = new SqlParameter("@Result", SqlDbType.Int);
            para[19].Direction = ParameterDirection.ReturnValue;
            para[20] = new SqlParameter("@Site", site);
            para[21] = new SqlParameter("@GiftToken", player.GiftToken);
            if (this.db.RunProcedure("SP_Users_Active", para))
            {
                player.ID = int.Parse(para[0].Value.ToString());

                result = ((int)para[19].Value == 0);

            }
            else
            {
 
            }
            //}
            //catch (Exception e)
            //{
            //if (true)
            //{
            //	BaseBussiness.log.Error("Init", e);
            //}
            //}
            return result;
        }
        public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, int sex, ref string msg, int validDate)
        {
            bool result = false;
            try
            {
                string[] bStyles = bStyle.Split(new char[]
                {
                    ','
                });
                string[] gStyles = gStyle.Split(new char[]
                {
                    ','
                });
                SqlParameter[] para = new SqlParameter[18];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@PassWord", passWord);
                para[2] = new SqlParameter("@NickName", nickName);
                para[3] = new SqlParameter("@BArmID", bStyles[0]);
                para[4] = new SqlParameter("@BHairID", bStyles[1]);
                para[5] = new SqlParameter("@BFaceID", bStyles[2]);
                para[6] = new SqlParameter("@BClothID", bStyles[3]);
                para[7] = new SqlParameter("@GArmID", gStyles[0]);
                para[8] = new SqlParameter("@GHairID", gStyles[1]);
                para[9] = new SqlParameter("@GFaceID", gStyles[2]);
                para[10] = new SqlParameter("@GClothID", gStyles[3]);
                para[11] = new SqlParameter("@ArmColor", (armColor == null) ? "" : armColor);
                para[12] = new SqlParameter("@HairColor", (hairColor == null) ? "" : hairColor);
                para[13] = new SqlParameter("@FaceColor", (faceColor == null) ? "" : faceColor);
                para[14] = new SqlParameter("@ClothColor", (clothColor == null) ? "" : clothColor);
                para[15] = new SqlParameter("@Sex", sex);
                para[16] = new SqlParameter("@Result", SqlDbType.Int);
                para[16].Direction = ParameterDirection.ReturnValue;
                para[17] = new SqlParameter("@StyleDate", validDate);
                result = this.db.RunProcedure("SP_Users_RegisterNotValidate", para);
                int returnValue = (int)para[16].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2", new object[0]);
                        break;
                    case 3:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool RenameNick(string userName, string nickName, string newNickName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", userName),
                    new SqlParameter("@NickName", nickName),
                    new SqlParameter("@NewNickName", newNickName),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Users_RenameNick", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("RenameNick", e);
                }
            }
            return result;
        }
        public bool RenameByCard(string userName, string nickName, string newNickName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", userName),
                    new SqlParameter("@NickName", nickName),
                    new SqlParameter("@NewNickName", newNickName),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Users_RenameByCard", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("RenameNick", e);
                }
            }
            return result;
        }
        public bool RenameConsortiaName(string userName, string nickName, string consortiaName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", userName),
                    new SqlParameter("@NickName", nickName),
                    new SqlParameter("@ConsortiaName", consortiaName),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Users_RenameConsortiaName", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.SP_Users_RenameConsortiaName.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("RenameNick", e);
                }
            }
            return result;
        }
        public bool RenameConsortiaNameByCard(string userName, string nickName, string consortiaName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", userName),
                    new SqlParameter("@NickName", nickName),
                    new SqlParameter("@ConsortiaName", consortiaName),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Users_RenameConsortiaNameByCard", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.SP_Users_RenameConsortiaName.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("RenameNick", e);
                }
            }
            return result;
        }
        public bool UpdatePassWord(int userID, string password)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userID),
                    new SqlParameter("@Password", password)
                };
                result = this.db.RunProcedure("SP_Users_UpdatePassword", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userID),
                    new SqlParameter("@PasswordQuestion1", PasswordQuestion1),
                    new SqlParameter("@PasswordAnswer1", PasswordAnswer1),
                    new SqlParameter("@PasswordQuestion2", PasswordQuestion2),
                    new SqlParameter("@PasswordAnswer2", PasswordAnswer2),
                    new SqlParameter("@FailedPasswordAttemptCount", Count)
                };
                result = this.db.RunProcedure("SP_Users_Password_Add", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userID)
                };
                this.db.GetReader(ref reader, "SP_Users_PasswordInfo", para);
                while (reader.Read())
                {
                    PasswordQuestion1 = ((reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString());
                    PasswordAnswer1 = ((reader["PasswordAnswer1"] == null) ? "" : reader["PasswordAnswer1"].ToString());
                    PasswordQuestion2 = ((reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString());
                    PasswordAnswer2 = ((reader["PasswordAnswer2"] == null) ? "" : reader["PasswordAnswer2"].ToString());
                    DateTime Today = (DateTime)reader["LastFindDate"];
                    if (Today == DateTime.Today)
                    {
                        Count = (int)reader["FailedPasswordAttemptCount"];
                    }
                    else
                    {
                        Count = 5;
                    }
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
        }
        public bool UpdatePasswordTwo(int userID, string passwordTwo)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userID),
                    new SqlParameter("@PasswordTwo", passwordTwo)
                };
                result = this.db.RunProcedure("SP_Users_UpdatePasswordTwo", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public PlayerInfo[] GetUserLoginList(string userName)
        {
            List<PlayerInfo> infos = new List<PlayerInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
                };
                para[0].Value = userName;
                this.db.GetReader(ref reader, "SP_Users_LoginList", para);
                while (reader.Read())
                {
                    infos.Add(this.InitPlayerInfo(reader));
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
        public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname)
        {
            SqlDataReader reader = null;
            PlayerInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", username),
                    new SqlParameter("@Password", ""),
                    new SqlParameter("@FirstValidate", firstValidate),
                    new SqlParameter("@Nickname", nickname)
                };
                if (this.db.GetReader(ref reader, "SP_Users_LoginWeb", para) && reader.Read())
                {
                    isFirst = (int)reader["IsFirst"];
                    isExist = (bool)reader["IsExist"];
                    forbidDate = (DateTime)reader["ForbidDate"];
                    if (isFirst > 1)
                    {
                        isFirst--;
                    }
                    result = this.InitPlayerInfo(reader);
                    return result;
                }
            }
            catch (Exception e)
            {
                isError = true;
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
        public PlayerInfo LoginGame(string username, string password)
        {
            SqlDataReader reader = null;
            PlayerInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", username),
                    new SqlParameter("@Password", password)
                };
                if (this.db.GetReader(ref reader, "SP_Users_Login", para) && reader.Read())
                {
                    result = this.InitPlayerInfo(reader);
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
        public bool UpdatePlayer(PlayerInfo player)
        {
            bool result = false;
            bool result2;
            try
            {
                if (player.Grade < 1)
                {
                    result2 = result;
                    return result2;
                }
                SqlParameter[] para = new SqlParameter[44];
                para[0] = new SqlParameter("@UserID", player.ID);
                para[1] = new SqlParameter("@Attack", player.Attack);
                para[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
                para[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
                para[4] = new SqlParameter("@Defence", player.Defence);
                para[5] = new SqlParameter("@Gold", player.Gold);
                para[6] = new SqlParameter("@GP", player.GP);
                para[7] = new SqlParameter("@Grade", player.Grade);
                para[8] = new SqlParameter("@Luck", player.Luck);
                para[9] = new SqlParameter("@Money", player.Money);
                para[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
                para[11] = new SqlParameter("@Agility", player.Agility);
                para[12] = new SqlParameter("@State", player.State);
                para[13] = new SqlParameter("@Hide", player.Hide);
                para[14] = new SqlParameter("@ExpendDate", (!player.ExpendDate.HasValue) ? "" : player.ExpendDate.ToString());
                para[15] = new SqlParameter("@Win", player.Win);
                para[16] = new SqlParameter("@Total", player.Total);
                para[17] = new SqlParameter("@Escape", player.Escape);
                para[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
                para[19] = new SqlParameter("@Offer", player.Offer);
                para[20] = new SqlParameter("@AntiAddiction", player.AntiAddiction);
                para[20].Direction = ParameterDirection.InputOutput;
                para[21] = new SqlParameter("@Result", SqlDbType.Int);
                para[21].Direction = ParameterDirection.ReturnValue;
                para[22] = new SqlParameter("@RichesOffer", player.RichesOffer);
                para[23] = new SqlParameter("@RichesRob", player.RichesRob);
                para[24] = new SqlParameter("@CheckCount", player.CheckCount);
                para[24].Direction = ParameterDirection.InputOutput;
                para[25] = new SqlParameter("@MarryInfoID", player.MarryInfoID);
                para[26] = new SqlParameter("@DayLoginCount", player.DayLoginCount);
                para[27] = new SqlParameter("@Nimbus", player.Nimbus);
                para[28] = new SqlParameter("@LastAward", player.LastAward);
                para[29] = new SqlParameter("@GiftToken", player.GiftToken);
                para[30] = new SqlParameter("@QuestSite", player.QuestSite);
                para[31] = new SqlParameter("@PvePermission", player.PvePermission);
                para[32] = new SqlParameter("@FightPower", player.FightPower);
                para[33] = new SqlParameter("@AnswerSite", player.AnswerSite);
                para[34] = new SqlParameter("@LastAuncherAward", player.LastAuncherAward);
                para[35] = new SqlParameter("@ChatCount", player.ChatCount);
                para[36] = new SqlParameter("@SpaPubGoldRoomLimit", player.SpaPubGoldRoomLimit);
                para[37] = new SqlParameter("@LastSpaDate", player.LastSpaDate);
                para[38] = new SqlParameter("@FightLabPermission", player.FightLabPermission);
                para[39] = new SqlParameter("@SpaPubMoneyRoomLimit", player.SpaPubMoneyRoomLimit);
                para[40] = new SqlParameter("@IsInSpaPubGoldToday", player.IsInSpaPubGoldToday);
                para[41] = new SqlParameter("@IsInSpaPubMoneyToday", player.IsInSpaPubMoneyToday);
                para[42] = new SqlParameter("@VIPLevel", player.VipLevel);
                para[43] = new SqlParameter("@VIPGiftLevel", player.VIPGiftLevel);
                if (this.db.RunProcedure("SP_Users_Update", para))
                {
                    result = ((int)para[21].Value == 0);
                    if (result)
                    {
                        player.AntiAddiction = (int)para[20].Value;
                        player.CheckCount = (int)para[24].Value;
                    }
                    player.IsDirty = false;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            result2 = result;
            return result2;
        }
        public bool UpdatePlayerMarry(PlayerInfo player)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", player.ID),
                    new SqlParameter("@IsMarried", player.IsMarried),
                    new SqlParameter("@SpouseID", player.SpouseID),
                    new SqlParameter("@SpouseName", player.SpouseName),
                    new SqlParameter("@IsCreatedMarryRoom", player.IsCreatedMarryRoom),
                    new SqlParameter("@SelfMarryRoomID", player.SelfMarryRoomID),
                    new SqlParameter("@IsGotRing", player.IsGotRing)
                };
                result = this.db.RunProcedure("SP_Users_Marry", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdatePlayerMarry", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerLastAward(int id)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", id)
                };
                result = this.db.RunProcedure("SP_Users_LastAward", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdatePlayerAward", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerLastAuncherAward(int id)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", id)
                };
                result = this.db.RunProcedure("SP_Users_LastAuncherAward", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdatePlayerLastAuncherAward", e);
                }
            }
            return result;
        }
        public PlayerInfo GetUserSingleByUserID(int UserID)
        {
            SqlDataReader reader = null;
            PlayerInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_SingleByUserID", para);
                if (reader.Read())
                {
                    result = this.InitPlayerInfo(reader);
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
        public PlayerInfo GetUserSingleAllUserID(int UserID)
        {
            SqlDataReader reader = null;
            PlayerInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_SingleAllUserID", para);
                if (reader.Read())
                {
                    result = this.InitPlayerInfo(reader);
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
        public PlayerInfo[] GetUserSingleByUserName(string userName)
        {
            SqlDataReader reader = null;
            List<PlayerInfo> result = new List<PlayerInfo>();
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", SqlDbType.VarChar, 200)
                };
                para[0].Value = userName;
                this.db.GetReader(ref reader, "SP_Users_SingleByUserName", para);
                if (reader.Read())
                {                    
                      result.Add(this.InitPlayerInfo(reader));
                }
                return result.ToArray();
            }
            catch
            {
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public PlayerInfo GetUserSingleByNickName(string nickName)
        {
            SqlDataReader reader = null;
            PlayerInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
                };
                para[0].Value = nickName;
                this.db.GetReader(ref reader, "SP_Users_SingleByNickName", para);
                if (reader.Read())
                {
                    result = this.InitPlayerInfo(reader);
                    return result;
                }
            }
            catch
            {
                throw new Exception();
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
        public bool GetUserCheckByNickName(string nickName)
        {
            bool result = false;
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
                };
                para[0].Value = nickName;
                if (this.db.GetReader(ref reader, "SP_Users_CheckByNickName", para))
                {
                    while (reader.Read())
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = true;
                throw new Exception();
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return result;
        }
        public bool GetConsortiaCheckByConsortiaName(string consortiaName)
        {
            bool result = false;
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ConsortiaName", SqlDbType.NVarChar, 200)
                };
                para[0].Value = consortiaName;
                if (this.db.GetReader(ref reader, "SP_Consortia_CheckByConsortiaName", para))
                {
                    while (reader.Read())
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = true;
                throw new Exception();
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return result;
        }
        public PlayerInfo InitPlayerInfo(SqlDataReader reader)
        {
            PlayerInfo player = new PlayerInfo();
            player.IsConsortia = (bool)reader["IsConsortia"];
            player.Agility = (int)reader["Agility"];
            player.Attack = (int)reader["Attack"];
            player.Colors = ((reader["Colors"] == null) ? "" : reader["Colors"].ToString());
            player.ConsortiaID = (int)reader["ConsortiaID"];
            player.Defence = (int)reader["Defence"];
            player.Gold = (int)reader["Gold"];
            player.GP = (int)reader["GP"];
            player.Grade = (int)reader["Grade"];
            player.ID = (int)reader["UserID"];
            player.Luck = (int)reader["Luck"];
            player.Money = (int)reader["Money"];
            player.NickName = ((reader["NickName"] == null) ? "" : reader["NickName"].ToString());
            player.Sex = (bool)reader["Sex"];
            player.State = (int)reader["State"];
            player.Style = ((reader["Style"] == null) ? "" : reader["Style"].ToString());
            player.Hide = (int)reader["Hide"];
            player.Repute = (int)reader["Repute"];
            player.UserName = ((reader["UserName"] == null) ? "" : reader["UserName"].ToString());
            player.ConsortiaName = ((reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString());
            player.Offer = (int)reader["Offer"];
            player.Win = (int)reader["Win"];
            player.Total = (int)reader["Total"];
            player.Escape = (int)reader["Escape"];
            player.Skin = ((reader["Skin"] == null) ? "" : reader["Skin"].ToString());
            player.IsBanChat = (bool)reader["IsBanChat"];
            player.ReputeOffer = (int)reader["ReputeOffer"];
            player.ConsortiaRepute = (int)reader["ConsortiaRepute"];
            player.ConsortiaLevel = (int)reader["ConsortiaLevel"];
            player.StoreLevel = (int)reader["StoreLevel"];
            player.ShopLevel = (int)reader["ShopLevel"];
            player.SmithLevel = (int)reader["SmithLevel"];
            player.ConsortiaHonor = (int)reader["ConsortiaHonor"];
            player.RichesOffer = (int)reader["RichesOffer"];
            player.RichesRob = (int)reader["RichesRob"];
            player.AntiAddiction = (int)reader["AntiAddiction"];
            player.DutyLevel = (int)reader["DutyLevel"];
            player.DutyName = ((reader["DutyName"] == null) ? "" : reader["DutyName"].ToString());
            player.Right = (int)reader["Right"];
            player.ChairmanName = ((reader["ChairmanName"] == null) ? "" : reader["ChairmanName"].ToString());
            player.AddDayGP = (int)reader["AddDayGP"];
            player.AddDayOffer = (int)reader["AddDayOffer"];
            player.AddWeekGP = (int)reader["AddWeekGP"];
            player.AddWeekOffer = (int)reader["AddWeekOffer"];
            player.ConsortiaRiches = (int)reader["ConsortiaRiches"];
            player.CheckCount = (int)reader["CheckCount"];
            player.IsMarried = (bool)reader["IsMarried"];
            player.SpouseID = (int)reader["SpouseID"];
            player.SpouseName = ((reader["SpouseName"] == null) ? "" : reader["SpouseName"].ToString());
            player.MarryInfoID = (int)reader["MarryInfoID"];
            player.IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"];
            player.DayLoginCount = (int)reader["DayLoginCount"];
            player.PasswordTwo = ((reader["PasswordTwo"] == null) ? "" : reader["PasswordTwo"].ToString());
            player.SelfMarryRoomID = (int)reader["SelfMarryRoomID"];
            player.IsGotRing = (bool)reader["IsGotRing"];
            player.Rename = (bool)reader["Rename"];
            player.ConsortiaRename = (bool)reader["ConsortiaRename"];
            player.IsDirty = false;
            player.IsFirst = (int)reader["IsFirst"];
            player.Nimbus = (int)reader["Nimbus"];
            player.LastAward = (DateTime)reader["LastAward"];
            player.LastAuncherAward = (DateTime)reader["LastAuncherAward"];
            player.GiftToken = (int)reader["GiftToken"];
            player.QuestSite = ((reader["QuestSite"] == null) ? new byte[200] : ((byte[])reader["QuestSite"]));
            player.PvePermission = ((reader["PvePermission"] == null) ? "" : reader["PvePermission"].ToString());
            player.FightLabPermission = ((reader["FightLabPermission"] == null) ? "" : reader["FightLabPermission"].ToString());
            player.FightPower = (int)reader["FightPower"];
            player.PasswordQuest1 = ((reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString());
            player.PasswordQuest2 = ((reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString());
            player.LastDate = ((reader["LastDate"] == null || reader["LastDate"].ToString() == "") ? DateTime.Now : ((DateTime)reader["LastDate"]));
            player.ChatCount = ((reader["ChatCount"] == null) ? 0 : ((int)reader["ChatCount"]));
            player.OnlineTime = (int)reader["OnlineTime"];
            player.BoxProgression = (int)reader["BoxProgression"];
            player.GetBoxLevel = (int)reader["GetBoxLevel"];
            player.SpaPubGoldRoomLimit = (int)reader["SpaPubGoldRoomLimit"];
            player.LastSpaDate = (DateTime)reader["LastSpaDate"];
            player.SpaPubMoneyRoomLimit = (int)reader["SpaPubMoneyRoomLimit"];
            player.AddGPLastDate = (DateTime)reader["AddGPLastDate"];
            player.BoxGetDate = (DateTime)reader["BoxGetDate"];
            player.IsInSpaPubGoldToday = (bool)reader["IsInSpaPubGoldToday"];
            player.IsInSpaPubMoneyToday = (bool)reader["IsInSpaPubMoneyToday"];
            player.AlreadyGetBox = (int)reader["AlreadyGetBox"];

            player.ChargedMoney = (int)reader["ChargedMoney"];

            player.VipLevel = (int)reader["VIPLevel"];
            player.InviteID = (int)reader["InviteID"];
            player.VIPGiftLevel = (int)reader["VIPGiftLevel"];

            if ((DateTime)reader["LastFindDate"] != DateTime.Today.Date)
            {
                player.FailedPasswordAttemptCount = 5;
            }
            else
            {
                player.FailedPasswordAttemptCount = (int)reader["FailedPasswordAttemptCount"];
            }
            player.AnswerSite = (int)reader["AnswerSite"];
            return player;
        }
        public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
        {
            List<PlayerInfo> infos = new List<PlayerInfo>();
            try
            {
                string sWhere = " IsExist=1 and IsFirst<> 0 ";
                if (userID != -1)
                {
                    object obj = sWhere;
                    sWhere = string.Concat(new object[]
                    {
                        obj,
                        " and UserID =",
                        userID,
                        " "
                    });
                }
                string sOrder = "GP desc,AddGPLastDate asc";
                switch (order)
                {
                    case 1:
                        sOrder = "Offer desc";
                        break;
                    case 2:
                        sOrder = "AddDayGP desc";
                        break;
                    case 3:
                        sOrder = "AddWeekGP desc";
                        break;
                    case 4:
                        sOrder = "AddDayOffer desc";
                        break;
                    case 5:
                        sOrder = "AddWeekOffer desc";
                        break;
                    case 6:
                        sOrder = "FightPower desc";
                        break;
                }
                sOrder += ",UserID";
                DataTable dt = base.GetPage("V_Sys_Users_Detail", sWhere, page, size, "*", sOrder, "UserID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    infos.Add(new PlayerInfo
                    {
                        Agility = (int)dr["Agility"],
                        Attack = (int)dr["Attack"],
                        Colors = (dr["Colors"] == null) ? "" : dr["Colors"].ToString(),
                        ConsortiaID = (int)dr["ConsortiaID"],
                        Defence = (int)dr["Defence"],
                        Gold = (int)dr["Gold"],
                        GP = (int)dr["GP"],
                        Grade = (int)dr["Grade"],
                        ID = (int)dr["UserID"],
                        Luck = (int)dr["Luck"],
                        Money = (int)dr["Money"],
                        NickName = (dr["NickName"] == null) ? "" : dr["NickName"].ToString(),
                        Sex = (bool)dr["Sex"],
                        State = (int)dr["State"],
                        Style = (dr["Style"] == null) ? "" : dr["Style"].ToString(),
                        Hide = (int)dr["Hide"],
                        Repute = (int)dr["Repute"],
                        UserName = (dr["UserName"] == null) ? "" : dr["UserName"].ToString(),
                        ConsortiaName = (dr["ConsortiaName"] == null) ? "" : dr["ConsortiaName"].ToString(),
                        Offer = (int)dr["Offer"],
                        Skin = (dr["Skin"] == null) ? "" : dr["Skin"].ToString(),
                        IsBanChat = (bool)dr["IsBanChat"],
                        ReputeOffer = (int)dr["ReputeOffer"],
                        ConsortiaRepute = (int)dr["ConsortiaRepute"],
                        ConsortiaLevel = (int)dr["ConsortiaLevel"],
                        StoreLevel = (int)dr["StoreLevel"],
                        ShopLevel = (int)dr["ShopLevel"],
                        SmithLevel = (int)dr["SmithLevel"],
                        ConsortiaHonor = (int)dr["ConsortiaHonor"],
                        RichesOffer = (int)dr["RichesOffer"],
                        RichesRob = (int)dr["RichesRob"],
                        DutyLevel = (int)dr["DutyLevel"],
                        DutyName = (dr["DutyName"] == null) ? "" : dr["DutyName"].ToString(),
                        Right = (int)dr["Right"],
                        ChairmanName = (dr["ChairmanName"] == null) ? "" : dr["ChairmanName"].ToString(),
                        Win = (int)dr["Win"],
                        Total = (int)dr["Total"],
                        Escape = (int)dr["Escape"],
                        AddDayGP = (int)dr["AddDayGP"],
                        AddDayOffer = (int)dr["AddDayOffer"],
                        AddWeekGP = (int)dr["AddWeekGP"],
                        AddWeekOffer = (int)dr["AddWeekOffer"],
                        ConsortiaRiches = (int)dr["ConsortiaRiches"],
                        CheckCount = (int)dr["CheckCount"],
                        Nimbus = (int)dr["Nimbus"],
                        GiftToken = (int)dr["GiftToken"],
                        QuestSite = (dr["QuestSite"] == null) ? new byte[200] : ((byte[])dr["QuestSite"]),
                        PvePermission = (dr["PvePermission"] == null) ? "" : dr["PvePermission"].ToString(),
                        //PvePermission = (dr["FightLabPermission"] == null) ? "" : dr["FightLabPermission"].ToString(),
                        FightPower = (int)dr["FightPower"],
                        ChatCount = (dr["ChatCount"] == null) ? 0 : ((int)dr["ChatCount"]),
                        SpaPubGoldRoomLimit = (int)dr["SpaPubGoldRoomLimit"],
                        SpaPubMoneyRoomLimit = (int)dr["SpaPubMoneyRoomLimit"]
                    });
                }
                resultValue = true;
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
            }
            return infos.ToArray();
        }
        public ItemInfo[] GetUserItem(int UserID)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Items_All", para);
                while (reader.Read())
                {
                    items.Add(this.InitItem(reader));
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
            return items.ToArray();
        }
        public ItemInfo[] GetUserBagByType(int UserID, int bagType)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                para[1] = new SqlParameter("@BagType", bagType);
                this.db.GetReader(ref reader, "SP_Users_BagByType", para);
                while (reader.Read())
                {
                    items.Add(this.InitItem(reader));
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
            return items.ToArray();
        }
        public List<ItemInfo> GetUserEuqip(int UserID)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = UserID;
                if (this.db.GetReader(ref reader, "SP_Users_Items_Equip", para))
                {
                    while (reader.Read())
                    {
                        items.Add(this.InitItem(reader));
                    }
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
            return items;
        }
        public ItemInfo GetUserItemSingle(int itemID)
        {
            SqlDataReader reader = null;
            ItemInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", SqlDbType.Int, 4)
                };
                para[0].Value = itemID;
                this.db.GetReader(ref reader, "SP_Users_Items_Single", para);
                if (reader.Read())
                {
                    result = this.InitItem(reader);
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
        public ItemInfo InitItem(SqlDataReader reader)
        {
            ItemInfo item = ItemInfo.CreateWithoutInit(ItemMgr.FindItemTemplate((int)reader["TemplateID"]));
            item.AgilityCompose = (int)reader["AgilityCompose"];
            item.AttackCompose = (int)reader["AttackCompose"];
            item.Color = reader["Color"].ToString();
            item.Count = (int)reader["Count"];
            item.DefendCompose = (int)reader["DefendCompose"];
            item.ItemID = (int)reader["ItemID"];
            item.LuckCompose = (int)reader["LuckCompose"];
            item.Place = (int)reader["Place"];
            item.Hole1 = (int)reader["Hole1"];
            item.Hole2 = (int)reader["Hole2"];
            item.Hole3 = (int)reader["Hole3"];
            item.Hole4 = (int)reader["Hole4"];
            item.Hole5 = (int)reader["Hole5"];
            item.Hole6 = (int)reader["Hole6"];
            item.StrengthenLevel = (int)reader["StrengthenLevel"];
            item.TemplateID = (int)reader["TemplateID"];
            item.UserID = (int)reader["UserID"];
            item.ValidDate = (int)reader["ValidDate"];
            item.IsDirty = false;
            item.IsExist = (bool)reader["IsExist"];
            item.IsBinds = (bool)reader["IsBinds"];
            item.IsUsed = (bool)reader["IsUsed"];
            item.BeginDate = (DateTime)reader["BeginDate"];
            item.IsJudge = (bool)reader["IsJudge"];
            item.BagType = (int)reader["BagType"];
            item.Skin = reader["Skin"].ToString();
            item.RemoveDate = (DateTime)reader["RemoveDate"];
            item.RemoveType = (int)reader["RemoveType"];
            item.IsDirty = false;
            return item;
        }
        public bool AddGoods(ItemInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[26];
                para[0] = new SqlParameter("@ItemID", item.ItemID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@TemplateID", item.TemplateID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                para[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                para[6] = new SqlParameter("@BeginDate", item.BeginDate);
                para[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                para[8] = new SqlParameter("@Count", item.Count);
                para[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                para[10] = new SqlParameter("@IsBinds", item.IsBinds);
                para[11] = new SqlParameter("@IsExist", item.IsExist);
                para[12] = new SqlParameter("@IsJudge", item.IsJudge);
                para[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                para[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                para[15] = new SqlParameter("@ValidDate", item.ValidDate);
                para[16] = new SqlParameter("@BagType", item.BagType);
                para[17] = new SqlParameter("@Skin", (item.Skin == null) ? "" : item.Skin);
                para[18] = new SqlParameter("@IsUsed", item.IsUsed);
                para[19] = new SqlParameter("@RemoveType", item.RemoveType);
                para[20] = new SqlParameter("@Hole1", item.Hole1);
                para[21] = new SqlParameter("@Hole2", item.Hole2);
                para[22] = new SqlParameter("@Hole3", item.Hole3);
                para[23] = new SqlParameter("@Hole4", item.Hole4);
                para[24] = new SqlParameter("@Hole5", item.Hole5);
                para[25] = new SqlParameter("@Hole6", item.Hole6);
                result = this.db.RunProcedure("SP_Users_Items_Add", para);
                item.ItemID = (int)para[0].Value;
                item.IsDirty = false;
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
            }
            return result;
        }
        public bool UpdateGoods(ItemInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ItemID", item.ItemID),
                    new SqlParameter("@UserID", item.UserID),
                    new SqlParameter("@TemplateID", item.TemplateID),
                    new SqlParameter("@Place", item.Place),
                    new SqlParameter("@AgilityCompose", item.AgilityCompose),
                    new SqlParameter("@AttackCompose", item.AttackCompose),
                    new SqlParameter("@BeginDate", item.BeginDate),
                    new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
                    new SqlParameter("@Count", item.Count),
                    new SqlParameter("@DefendCompose", item.DefendCompose),
                    new SqlParameter("@IsBinds", item.IsBinds),
                    new SqlParameter("@IsExist", item.IsExist),
                    new SqlParameter("@IsJudge", item.IsJudge),
                    new SqlParameter("@LuckCompose", item.LuckCompose),
                    new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
                    new SqlParameter("@ValidDate", item.ValidDate),
                    new SqlParameter("@BagType", item.BagType),
                    new SqlParameter("@Skin", item.Skin),
                    new SqlParameter("@IsUsed", item.IsUsed),
                    new SqlParameter("@RemoveDate", item.RemoveDate),
                    new SqlParameter("@RemoveType", item.RemoveType),
                    new SqlParameter("@Hole1", item.Hole1),
                    new SqlParameter("@Hole2", item.Hole2),
                    new SqlParameter("@Hole3", item.Hole3),
                    new SqlParameter("@Hole4", item.Hole4),
                    new SqlParameter("@Hole5", item.Hole5),
                    new SqlParameter("@Hole6", item.Hole6)
                };
                result = this.db.RunProcedure("SP_Users_Items_Update", para);
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool DeleteGoods(int itemID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", itemID)
                };
                result = this.db.RunProcedure("SP_Users_Items_Delete", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public BestEquipInfo[] GetCelebByDayBestEquip()
        {
            List<BestEquipInfo> infos = new List<BestEquipInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_Users_BestEquip");
                while (reader.Read())
                {
                    infos.Add(new BestEquipInfo
                    {
                        Date = (DateTime)reader["RemoveDate"],
                        GP = (int)reader["GP"],
                        Grade = (int)reader["Grade"],
                        ItemName = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
                        NickName = (reader["NickName"] == null) ? "" : reader["NickName"].ToString(),
                        Sex = (bool)reader["Sex"],
                        Strengthenlevel = (int)reader["Strengthenlevel"],
                        UserName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString()
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
        public MailInfo InitMail(SqlDataReader reader)
        {
            return new MailInfo
            {
                Annex1 = reader["Annex1"].ToString(),
                Annex2 = reader["Annex2"].ToString(),
                Content = reader["Content"].ToString(),
                Gold = (int)reader["Gold"],
                ID = (int)reader["ID"],
                IsExist = (bool)reader["IsExist"],
                Money = (int)reader["Money"],
                Receiver = reader["Receiver"].ToString(),
                ReceiverID = (int)reader["ReceiverID"],
                Sender = reader["Sender"].ToString(),
                SenderID = (int)reader["SenderID"],
                Title = reader["Title"].ToString(),
                Type = (int)reader["Type"],
                ValidDate = (int)reader["ValidDate"],
                IsRead = (bool)reader["IsRead"],
                SendTime = (DateTime)reader["SendTime"],
                Annex1Name = (reader["Annex1Name"] == null) ? "" : reader["Annex1Name"].ToString(),
                Annex2Name = (reader["Annex2Name"] == null) ? "" : reader["Annex2Name"].ToString(),
                Annex3 = reader["Annex3"].ToString(),
                Annex4 = reader["Annex4"].ToString(),
                Annex5 = reader["Annex5"].ToString(),
                Annex3Name = (reader["Annex3Name"] == null) ? "" : reader["Annex3Name"].ToString(),
                Annex4Name = (reader["Annex4Name"] == null) ? "" : reader["Annex4Name"].ToString(),
                Annex5Name = (reader["Annex5Name"] == null) ? "" : reader["Annex5Name"].ToString(),
                AnnexRemark = (reader["AnnexRemark"] == null) ? "" : reader["AnnexRemark"].ToString(),
                GiftToken = (int)reader["GiftToken"]
            };
        }
        public MailInfo[] GetMailByUserID(int userID)
        {
            List<MailInfo> items = new List<MailInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_Mail_ByUserID", para);
                while (reader.Read())
                {
                    items.Add(this.InitMail(reader));
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
            return items.ToArray();
        }
        public MailInfo[] GetMailBySenderID(int userID)
        {
            List<MailInfo> items = new List<MailInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_Mail_BySenderID", para);
                while (reader.Read())
                {
                    items.Add(this.InitMail(reader));
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
            return items.ToArray();
        }
        public MailInfo GetMailSingle(int UserID, int mailID)
        {
            SqlDataReader reader = null;
            MailInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", mailID),
                    new SqlParameter("@UserID", UserID)
                };
                this.db.GetReader(ref reader, "SP_Mail_Single", para);
                if (reader.Read())
                {
                    result = this.InitMail(reader);
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
        public bool SendMail(MailInfo mail)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[29];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", true);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", false);
                para[16] = new SqlParameter("@SendTime", DateTime.Now);
                para[17] = new SqlParameter("@Type", mail.Type);
                para[18] = new SqlParameter("@Annex1Name", (mail.Annex1Name == null) ? "" : mail.Annex1Name);
                para[19] = new SqlParameter("@Annex2Name", (mail.Annex2Name == null) ? "" : mail.Annex2Name);
                para[20] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
                para[21] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
                para[22] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
                para[23] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
                para[24] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
                para[25] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
                para[26] = new SqlParameter("@ValidDate", mail.ValidDate);
                para[27] = new SqlParameter("@AnnexRemark", (mail.AnnexRemark == null) ? "" : mail.AnnexRemark);
                para[28] = new SqlParameter("@GiftToken", mail.GiftToken);
                result = this.db.RunProcedure("SP_Mail_Send", para);
                mail.ID = (int)para[0].Value;
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
            }
            return result;
        }
        public bool DeleteMail(int UserID, int mailID, out int senderID)
        {
            bool result = false;
            senderID = 0;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ID", mailID);
                para[1] = new SqlParameter("@UserID", UserID);
                para[2] = new SqlParameter("@SenderID", SqlDbType.Int);
                para[2].Value = senderID;
                para[2].Direction = ParameterDirection.InputOutput;
                para[3] = new SqlParameter("@Result", SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Mail_Delete", para);
                int returnValue = (int)para[3].Value;
                if (returnValue == 0)
                {
                    result = true;
                    senderID = (int)para[2].Value;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateMail(MailInfo mail, int oldMoney)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[30];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", mail.IsExist);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", mail.IsRead);
                para[16] = new SqlParameter("@SendTime", mail.SendTime);
                para[17] = new SqlParameter("@Type", mail.Type);
                para[18] = new SqlParameter("@OldMoney", oldMoney);
                para[19] = new SqlParameter("@ValidDate", mail.ValidDate);
                para[20] = new SqlParameter("@Annex1Name", mail.Annex1Name);
                para[21] = new SqlParameter("@Annex2Name", mail.Annex2Name);
                para[22] = new SqlParameter("@Result", SqlDbType.Int);
                para[22].Direction = ParameterDirection.ReturnValue;
                para[23] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
                para[24] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
                para[25] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
                para[26] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
                para[27] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
                para[28] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
                para[29] = new SqlParameter("@GiftToken", mail.GiftToken);
                this.db.RunProcedure("SP_Mail_Update", para);
                int returnValue = (int)para[22].Value;
                result = (returnValue == 0);
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
            }
            return result;
        }
        public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@userid", userid);
                para[1] = new SqlParameter("@mailID", mailID);
                para[2] = new SqlParameter("@senderID", SqlDbType.Int);
                para[2].Value = senderID;
                para[2].Direction = ParameterDirection.InputOutput;
                para[3] = new SqlParameter("@Result", SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Mail_PaymentCancel", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                if (result)
                {
                    senderID = (int)para[2].Value;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool ScanMail(ref string noticeUserID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
                };
                para[0].Direction = ParameterDirection.Output;
                this.db.RunProcedure("SP_Mail_Scan", para);
                noticeUserID = para[0].Value.ToString();
                result = true;
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
            }
            return result;
        }
        public bool SendMailAndItem(MailInfo mail, ItemInfo item, ref int returnValue)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[34];
                para[0] = new SqlParameter("@ItemID", item.ItemID);
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@TemplateID", item.TemplateID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                para[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                para[6] = new SqlParameter("@BeginDate", item.BeginDate);
                para[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                para[8] = new SqlParameter("@Count", item.Count);
                para[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                para[10] = new SqlParameter("@IsBinds", item.IsBinds);
                para[11] = new SqlParameter("@IsExist", item.IsExist);
                para[12] = new SqlParameter("@IsJudge", item.IsJudge);
                para[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                para[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                para[15] = new SqlParameter("@ValidDate", item.ValidDate);
                para[16] = new SqlParameter("@BagType", item.BagType);
                para[17] = new SqlParameter("@ID", mail.ID);
                para[17].Direction = ParameterDirection.Output;
                para[18] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[19] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[20] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[21] = new SqlParameter("@Gold", mail.Gold);
                para[22] = new SqlParameter("@Money", mail.Money);
                para[23] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[24] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[25] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[26] = new SqlParameter("@SenderID", mail.SenderID);
                para[27] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[28] = new SqlParameter("@IfDelS", false);
                para[29] = new SqlParameter("@IsDelete", false);
                para[30] = new SqlParameter("@IsDelR", false);
                para[31] = new SqlParameter("@IsRead", false);
                para[32] = new SqlParameter("@SendTime", DateTime.Now);
                para[33] = new SqlParameter("@Result", SqlDbType.Int);
                para[33].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Admin_SendUserItem", para);
                returnValue = (int)para[33].Value;
                result = (returnValue == 0);
                if (result)
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        client.MailNotice(mail.ReceiverID);
                    }
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
            }
            return result;
        }
        public bool SendMailAndMoney(MailInfo mail, ref int returnValue)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[18];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", true);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", false);
                para[16] = new SqlParameter("@SendTime", DateTime.Now);
                para[17] = new SqlParameter("@Result", SqlDbType.Int);
                para[17].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Admin_SendUserMoney", para);
                returnValue = (int)para[17].Value;
                result = (returnValue == 0);
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
            }
            return result;
        }
        public int SendMailAndItem(string title, string content, int UserID, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            MailInfo message = new MailInfo();
            message.Annex1 = "";
            message.Content = title;
            message.Gold = gold;
            message.Money = money;
            message.Receiver = "";
            message.ReceiverID = UserID;
            message.Sender = "Administrators";
            message.SenderID = 0;
            message.Title = content;
            ItemInfo userGoods = ItemInfo.CreateWithoutInit(null);
            userGoods.TemplateID = templateID;
            userGoods.AgilityCompose = AgilityCompose;
            userGoods.AttackCompose = AttackCompose;
            userGoods.BeginDate = DateTime.Now;
            userGoods.Color = "";
            userGoods.DefendCompose = DefendCompose;
            userGoods.IsDirty = false;
            userGoods.IsExist = true;
            userGoods.IsJudge = true;
            userGoods.LuckCompose = LuckCompose;
            userGoods.StrengthenLevel = StrengthenLevel;
            userGoods.ValidDate = validDate;
            userGoods.Count = count;
            userGoods.IsBinds = isBinds;
            int returnValue = 1;
            this.SendMailAndItem(message, userGoods, ref returnValue);
            return returnValue;
        }
       // public int SendMailAndItemByUserName(string title, string content, string userName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
      //  {
      //      PlayerInfo player = this.GetUserSingleByUserName(userName);
      //      int result;
      //      if (player != null)
      //      {
      //          result = this.SendMailAndItem(title, content, player.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
      //      }
      //      else
      //      {
      //          result = 2;
      //      }
      //      return result;
      //  }
        public int SendMailAndItemByNickName(string title, string content, string NickName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo player = this.GetUserSingleByNickName(NickName);
            int result;
            if (player != null)
            {
                result = this.SendMailAndItem(title, content, player.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            }
            else
            {
                result = 2;
            }
            return result;
        }
        public int SendMailAndItem(string title, string content, int userID, int gold, int money, int giftToken, string param)
        {
            int returnValue = 1;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Content", content),
                    new SqlParameter("@UserID", userID),
                    new SqlParameter("@Gold", gold),
                    new SqlParameter("@Money", money),
                    new SqlParameter("@GiftToken", giftToken),
                    new SqlParameter("@Param", param),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[7].Direction = ParameterDirection.ReturnValue;
                bool result = this.db.RunProcedure("SP_Admin_SendAllItem", para);
                returnValue = (int)para[7].Value;
                result = (returnValue == 0);
                if (result)
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        client.MailNotice(userID);
                    }
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
            }
            return returnValue;
        }
      //  public int SendMailAndItemByUserName(string title, string content, string userName, int gold, int money, int giftToken, string param)
      //  {
      //      PlayerInfo player = this.GetUserSingleByUserName(userName);
      //      int result;
      //      if (player != null)
       //     {
        //        result = this.SendMailAndItem(title, content, player.ID, gold, money, giftToken, param);
      //      }
      //      else
      //      {
      //          result = 2;
      //      }
      //      return result;
      //  }
        public int SendMailAndItemByNickName(string title, string content, string nickName, int gold, int money, int giftToken, string param)
        {
            PlayerInfo player = this.GetUserSingleByNickName(nickName);
            int result;
            if (player != null)
            {
                result = this.SendMailAndItem(title, content, player.ID, gold, money, giftToken, param);
            }
            else
            {
                result = 2;
            }
            return result;
        }
        public Dictionary<int, int> GetFriendsIDAll(int UserID)
        {
            Dictionary<int, int> info = new Dictionary<int, int>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Friends_All", para);
                while (reader.Read())
                {
                    if (!info.ContainsKey((int)reader["FriendID"]))
                    {
                        info.Add((int)reader["FriendID"], (int)reader["Relation"]);
                    }
                    else
                    {
                        info[(int)reader["FriendID"]] = (int)reader["Relation"];
                    }
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
            return info;
        }
        public bool AddFriends(FriendInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", info.ID),
                    new SqlParameter("@AddDate", DateTime.Now),
                    new SqlParameter("@FriendID", info.FriendID),
                    new SqlParameter("@IsExist", true),
                    new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark),
                    new SqlParameter("@UserID", info.UserID),
                    new SqlParameter("@Relation", info.Relation)
                };
                result = this.db.RunProcedure("SP_Users_Friends_Add", para);
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
            }
            return result;
        }
        public bool DeleteFriends(int UserID, int FriendID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", FriendID),
                    new SqlParameter("@UserID", UserID)
                };
                result = this.db.RunProcedure("SP_Users_Friends_Delete", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public FriendInfo[] GetFriendsAll(int UserID)
        {
            List<FriendInfo> infos = new List<FriendInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Friends", para);
                while (reader.Read())
                {
                    infos.Add(new FriendInfo
                    {
                        AddDate = (DateTime)reader["AddDate"],
                        Colors = (reader["Colors"] == null) ? "" : reader["Colors"].ToString(),
                        FriendID = (int)reader["FriendID"],
                        Grade = (int)reader["Grade"],
                        Hide = (int)reader["Hide"],
                        ID = (int)reader["ID"],
                        IsExist = (bool)reader["IsExist"],
                        NickName = (reader["NickName"] == null) ? "" : reader["NickName"].ToString(),
                        Remark = (reader["Remark"] == null) ? "" : reader["Remark"].ToString(),
                        Sex = ((bool)reader["Sex"]) ? 1 : 0,
                        State = (int)reader["State"],
                        Style = (reader["Style"] == null) ? "" : reader["Style"].ToString(),
                        UserID = (int)reader["UserID"],
                        ConsortiaName = (reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString(),
                        Offer = (int)reader["Offer"],
                        Win = (int)reader["Win"],
                        Total = (int)reader["Total"],
                        Escape = (int)reader["Escape"],
                        Relation = (int)reader["Relation"],
                        Repute = (int)reader["Repute"],
                        UserName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString(),
                        DutyName = (reader["DutyName"] == null) ? "" : reader["DutyName"].ToString(),
                        Nimbus = (int)reader["Nimbus"],
                        FightPower = (int)reader["FightPower"]
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
        public ArrayList GetFriendsGood(string UserName)
        {
            ArrayList friends = new ArrayList();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserName", SqlDbType.NVarChar)
                };
                para[0].Value = UserName;
                this.db.GetReader(ref reader, "SP_Users_Friends_Good", para);
                while (reader.Read())
                {
                    friends.Add((reader["UserName"] == null) ? "" : reader["UserName"].ToString());
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
            return friends;
        }
        public FriendInfo[] GetFriendsBbs(string condictArray)
        {
            List<FriendInfo> infos = new List<FriendInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 4000)
                };
                para[0].Value = condictArray;
                this.db.GetReader(ref reader, "SP_Users_FriendsBbs", para);
                while (reader.Read())
                {
                    infos.Add(new FriendInfo
                    {
                        NickName = (reader["NickName"] == null) ? "" : reader["NickName"].ToString(),
                        UserID = (int)reader["UserID"],
                        UserName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString(),
                        IsExist = (int)reader["UserID"] > 0
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
        public QuestDataInfo[] GetUserQuest(int userID)
        {
            List<QuestDataInfo> infos = new List<QuestDataInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_QuestData_All", para);
                while (reader.Read())
                {
                    infos.Add(new QuestDataInfo
                    {
                        CompletedDate = (DateTime)reader["CompletedDate"],
                        IsComplete = (bool)reader["IsComplete"],
                        Condition1 = (int)reader["Condition1"],
                        Condition2 = (int)reader["Condition2"],
                        Condition3 = (int)reader["Condition3"],
                        Condition4 = (int)reader["Condition4"],
                        QuestID = (int)reader["QuestID"],
                        UserID = (int)reader["UserId"],
                        IsExist = (bool)reader["IsExist"],
                        RandDobule = (int)reader["RandDobule"],
                        RepeatFinish = (int)reader["RepeatFinish"],
                        IsDirty = false
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
        public bool UpdateDbQuestDataInfo(QuestDataInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", info.UserID),
                    new SqlParameter("@QuestID", info.QuestID),
                    new SqlParameter("@CompletedDate", info.CompletedDate),
                    new SqlParameter("@IsComplete", info.IsComplete),
                    new SqlParameter("@Condition1", info.Condition1),
                    new SqlParameter("@Condition2", info.Condition2),
                    new SqlParameter("@Condition3", info.Condition3),
                    new SqlParameter("@Condition4", info.Condition4),
                    new SqlParameter("@IsExist", info.IsExist),
                    new SqlParameter("@RepeatFinish", info.RepeatFinish),
                    new SqlParameter("@RandDobule", info.RandDobule)
                };
                result = this.db.RunProcedure("SP_QuestData_Add", para);
                info.IsDirty = false;
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
            }
            return result;
        }
        public List<AchievementDataInfo> GetUserAchievementData(int userID)
        {
            List<AchievementDataInfo> infos = new List<AchievementDataInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_Achievement_Data_All", para);
                while (reader.Read())
                {
                    infos.Add(new AchievementDataInfo
                    {
                        UserID = (int)reader["UserID"],
                        AchievementID = (int)reader["AchievementID"],
                        IsComplete = (bool)reader["IsComplete"],
                        CompletedDate = (DateTime)reader["CompletedDate"],
                        IsDirty = false
                    });
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init_GetUserAchievement", e);
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
        public bool UpdateDbAchievementDataInfo(AchievementDataInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", info.UserID),
                    new SqlParameter("@AchievementID", info.AchievementID),
                    new SqlParameter("@IsComplete", info.IsComplete),
                    new SqlParameter("@CompletedDate", info.CompletedDate)
                };
                result = this.db.RunProcedure("SP_Achievement_Data_Add", para);
                info.IsDirty = false;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init_UpdateDbAchievementDataInfo", e);
                }
            }
            finally
            {
            }
            return result;
        }
        public List<UsersRecordInfo> GetUserRecord(int userID)
        {
            List<UsersRecordInfo> infos = new List<UsersRecordInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_Users_Record_All", para);
                while (reader.Read())
                {
                    infos.Add(new UsersRecordInfo
                    {
                        UserID = (int)reader["UserID"],
                        RecordID = (int)reader["RecordID"],
                        Total = (int)reader["Total"],
                        IsDirty = false
                    });
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init_GetUserRecord", e);
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
        public bool UpdateDbUserRecord(UsersRecordInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", info.UserID),
                    new SqlParameter("@RecordID", info.RecordID),
                    new SqlParameter("@Total", info.Total)
                };
                result = this.db.RunProcedure("SP_Users_Record_Add", para);
                info.IsDirty = false;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init_UpdateDbUserRecord", e);
                }
            }
            finally
            {
            }
            return result;
        }
        public BufferInfo[] GetUserBuffer(int userID)
        {
            List<BufferInfo> infos = new List<BufferInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SqlDbType.Int, 4)
                };
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_User_Buff_All", para);
                while (reader.Read())
                {
                    infos.Add(new BufferInfo
                    {
                        BeginDate = (DateTime)reader["BeginDate"],
                        Data = (reader["Data"] == null) ? "" : reader["Data"].ToString(),
                        Type = (int)reader["Type"],
                        UserID = (int)reader["UserID"],
                        ValidDate = (int)reader["ValidDate"],
                        Value = (int)reader["Value"],
                        IsExist = (bool)reader["IsExist"],
                        IsDirty = false
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
        public bool SaveBuffer(BufferInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", info.UserID),
                    new SqlParameter("@Type", info.Type),
                    new SqlParameter("@BeginDate", info.BeginDate),
                    new SqlParameter("@Data", (info.Data == null) ? "" : info.Data),
                    new SqlParameter("@IsExist", info.IsExist),
                    new SqlParameter("@ValidDate", info.ValidDate),
                    new SqlParameter("@Value", info.Value)
                };
                result = this.db.RunProcedure("SP_User_Buff_Add", para);
                info.IsDirty = false;
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
            }
            return result;
        }
        public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, decimal needMoney, out int userID, ref int isResult, DateTime date, string IP, string nickName)
        {
            bool result = false;
            userID = 0;
            try
            {
                SqlParameter[] para = new SqlParameter[10];
                para[0] = new SqlParameter("@ChargeID", chargeID);
                para[1] = new SqlParameter("@UserName", userName);
                para[2] = new SqlParameter("@Money", money);
                para[3] = new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                para[4] = new SqlParameter("@PayWay", payWay);
                para[5] = new SqlParameter("@NeedMoney", needMoney);
                para[6] = new SqlParameter("@UserID", userID);
                para[6].Direction = ParameterDirection.InputOutput;
                para[7] = new SqlParameter("@Result", SqlDbType.Int);
                para[7].Direction = ParameterDirection.ReturnValue;
                para[8] = new SqlParameter("@IP", IP);
                para[9] = new SqlParameter("@NickName", nickName);
                result = this.db.RunProcedure("SP_Charge_Money_Add", para);
                userID = (int)para[6].Value;
                isResult = (int)para[7].Value;
                result = (isResult == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool AddChargeMoneyForId(string chargeID, string userName, int money, string payWay, decimal needMoney, out int userID, ref int isResult, DateTime date, string IP, int sourceUserId)
        {
            bool result = false;
            userID = 0;
            try
            {
                SqlParameter[] para = new SqlParameter[10];
                para[0] = new SqlParameter("@ChargeID", chargeID);
                para[1] = new SqlParameter("@UserName", userName);
                para[2] = new SqlParameter("@Money", money);
                para[3] = new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                para[4] = new SqlParameter("@PayWay", payWay);
                para[5] = new SqlParameter("@NeedMoney", needMoney);
                para[6] = new SqlParameter("@UserID", userID);
                para[6].Direction = ParameterDirection.InputOutput;
                para[7] = new SqlParameter("@Result", SqlDbType.Int);
                para[7].Direction = ParameterDirection.ReturnValue;
                para[8] = new SqlParameter("@IP", IP);
                para[9] = new SqlParameter("@SourceUserID", sourceUserId);
                if (this.db.RunProcedure("SP_Charge_Money_UserId_Add", para))
                {
                    userID = (int)para[6].Value;
                    isResult = (int)para[7].Value;
                    result = (isResult == 0);
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool ChargeToUser(string userName, ref int money, string nickName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@money", SqlDbType.Int);
                para[1].Direction = ParameterDirection.Output;
                para[2] = new SqlParameter("@NickName", nickName);
                result = this.db.RunProcedure("SP_Charge_To_User", para);
                money = (int)para[1].Value;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public ChargeRecordInfo[] GetChargeRecordInfo(DateTime date, int SaveRecordSecond)
        {
            List<ChargeRecordInfo> list = new List<ChargeRecordInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
                    new SqlParameter("@Second", SaveRecordSecond)
                };
                this.db.GetReader(ref reader, "SP_Charge_Record", para);
                while (reader.Read())
                {
                    list.Add(new ChargeRecordInfo
                    {
                        BoyTotalPay = (int)reader["BoyTotalPay"],
                        GirlTotalPay = (int)reader["GirlTotalPay"],
                        PayWay = (reader["PayWay"] == null) ? "" : reader["PayWay"].ToString(),
                        TotalBoy = (int)reader["TotalBoy"],
                        TotalGirl = (int)reader["TotalGirl"]
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
            return list.ToArray();
        }
        public AuctionInfo GetAuctionSingle(int auctionID)
        {
            SqlDataReader reader = null;
            AuctionInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@AuctionID", auctionID)
                };
                this.db.GetReader(ref reader, "SP_Auction_Single", para);
                if (reader.Read())
                {
                    result = this.InitAuctionInfo(reader);
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
        public bool AddAuction(AuctionInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[17];
                para[0] = new SqlParameter("@AuctionID", info.AuctionID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
                para[2] = new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName);
                para[3] = new SqlParameter("@BeginDate", info.BeginDate);
                para[4] = new SqlParameter("@BuyerID", info.BuyerID);
                para[5] = new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName);
                para[6] = new SqlParameter("@IsExist", info.IsExist);
                para[7] = new SqlParameter("@ItemID", info.ItemID);
                para[8] = new SqlParameter("@Mouthful", info.Mouthful);
                para[9] = new SqlParameter("@PayType", info.PayType);
                para[10] = new SqlParameter("@Price", info.Price);
                para[11] = new SqlParameter("@Rise", info.Rise);
                para[12] = new SqlParameter("@ValidDate", info.ValidDate);
                para[13] = new SqlParameter("@TemplateID", info.TemplateID);
                para[14] = new SqlParameter("Name", info.Name);
                para[15] = new SqlParameter("Category", info.Category);
                para[16] = new SqlParameter("Random", info.Random);
                result = this.db.RunProcedure("SP_Auction_Add", para);
                info.AuctionID = (int)para[0].Value;
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
            }
            return result;
        }
        public bool UpdateAuction(AuctionInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@AuctionID", info.AuctionID),
                    new SqlParameter("@AuctioneerID", info.AuctioneerID),
                    new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName),
                    new SqlParameter("@BeginDate", info.BeginDate),
                    new SqlParameter("@BuyerID", info.BuyerID),
                    new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName),
                    new SqlParameter("@IsExist", info.IsExist),
                    new SqlParameter("@ItemID", info.ItemID),
                    new SqlParameter("@Mouthful", info.Mouthful),
                    new SqlParameter("@PayType", info.PayType),
                    new SqlParameter("@Price", info.Price),
                    new SqlParameter("@Rise", info.Rise),
                    new SqlParameter("@ValidDate", info.ValidDate),
                    new SqlParameter("Name", info.Name),
                    new SqlParameter("Category", info.Category),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[15].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Auction_Update", para);
                int returnValue = (int)para[15].Value;
                result = (returnValue == 0);
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
            }
            return result;
        }
        public bool DeleteAuction(int auctionID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@AuctionID", auctionID),
                    new SqlParameter("@UserID", userID),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Auction_Delete", para);
                int returnValue = (int)para[2].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 0:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1", new object[0]);
                        break;
                    case 1:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2", new object[0]);
                        break;
                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3", new object[0]);
                        break;
                    default:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string AuctionIDs)
        {
            List<AuctionInfo> infos = new List<AuctionInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (!string.IsNullOrEmpty(name))
                {
                    sWhere = sWhere + " and Name like '%" + name + "%' ";
                }
                if (type != -1)
                {
                    switch (type)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                            {
                                object obj = sWhere;
                                sWhere = string.Concat(new object[]
                                {
                            obj,
                            " and Category =",
                            type,
                            " "
                                });
                                break;
                            }
                        case 18:
                        case 19:
                        case 20:
                            break;
                        case 21:
                            sWhere += " and Category in(1,2,5,8,9) ";
                            break;
                        case 22:
                            sWhere += " and Category in(13,15,6,4,3) ";
                            break;
                        case 23:
                            sWhere += " and Category in(16,11,10) ";
                            break;
                        case 24:
                            sWhere += " and Category in(8,9) ";
                            break;
                        case 25:
                            sWhere += " and Category in (7,17) ";
                            break;
                        case 26:
                            sWhere += " and TemplateId>=311000 and TemplateId<=313999";
                            break;
                        case 27:
                            sWhere += " and TemplateId>=311000 and TemplateId<=311999 ";
                            break;
                        case 28:
                            sWhere += " and TemplateId>=313000 and TempLateId<=313999";
                            break;
                        case 29:
                            sWhere += " and TemplateId>=312000 and TemplateId<=312999 ";
                            break;
                        default:
                            switch (type)
                            {
                                case 1100:
                                    sWhere += " and TemplateID in (11019,11021,11022,11023,11024) ";
                                    break;
                                case 1101:
                                    sWhere += " and TemplateID='11019' ";
                                    break;
                                case 1102:
                                    sWhere += " and TemplateID='11021' ";
                                    break;
                                case 1103:
                                    sWhere += " and TemplateID='11022' ";
                                    break;
                                case 1104:
                                    sWhere += " and TemplateID='11023' ";
                                    break;
                                case 1105:
                                    sWhere += " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";
                                    break;
                                case 1106:
                                    sWhere += " and TemplateID in (11001,11002,11003,11004) ";
                                    break;
                                case 1107:
                                    sWhere += " and TemplateID in (11005,11006,11007,11008) ";
                                    break;
                                case 1108:
                                    sWhere += " and TemplateID in (11009,11010,11011,11012) ";
                                    break;
                                case 1109:
                                    sWhere += " and TemplateID in (11013,11014,11015,11016) ";
                                    break;
                                case 1110:
                                    sWhere += " and TemplateID='11024' ";
                                    break;
                            }
                            break;
                    }
                }
                if (pay != -1)
                {
                    object obj = sWhere;
                    sWhere = string.Concat(new object[]
                    {
                        obj,
                        " and PayType =",
                        pay,
                        " "
                    });
                }
                if (userID != -1)
                {
                    object obj = sWhere;
                    sWhere = string.Concat(new object[]
                    {
                        obj,
                        " and AuctioneerID =",
                        userID,
                        " "
                    });
                }
                if (buyID != -1)
                {
                    object obj = sWhere;
                    sWhere = string.Concat(new object[]
                    {
                        obj,
                        " and (BuyerID =",
                        buyID,
                        " or AuctionID in (",
                        AuctionIDs,
                        ")) "
                    });
                }
                string sOrder = "Category,Name,Price,dd,AuctioneerID";
                switch (order)
                {
                    case 0:
                        sOrder = "Name";
                        break;
                    case 2:
                        sOrder = "dd";
                        break;
                    case 3:
                        sOrder = "AuctioneerName";
                        break;
                    case 4:
                        sOrder = "Price";
                        break;
                    case 5:
                        sOrder = "BuyerName";
                        break;
                }
                sOrder += (sort ? " desc" : "");
                sOrder += ",AuctionID ";
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@QueryStr", "V_Auction_Scan"),
                    new SqlParameter("@QueryWhere", sWhere),
                    new SqlParameter("@PageSize", size),
                    new SqlParameter("@PageCurrent", page),
                    new SqlParameter("@FdShow", "*"),
                    new SqlParameter("@FdOrder", sOrder),
                    new SqlParameter("@FdKey", "AuctionID"),
                    new SqlParameter("@TotalRow", total)
                };
                para[7].Direction = ParameterDirection.Output;
                DataTable dt = this.db.GetDataTable("Auction", "SP_CustomPage", para);
                total = (int)para[7].Value;
                foreach (DataRow dr in dt.Rows)
                {
                    infos.Add(new AuctionInfo
                    {
                        AuctioneerID = (int)dr["AuctioneerID"],
                        AuctioneerName = dr["AuctioneerName"].ToString(),
                        AuctionID = (int)dr["AuctionID"],
                        BeginDate = (DateTime)dr["BeginDate"],
                        BuyerID = (int)dr["BuyerID"],
                        BuyerName = dr["BuyerName"].ToString(),
                        Category = (int)dr["Category"],
                        IsExist = (bool)dr["IsExist"],
                        ItemID = (int)dr["ItemID"],
                        Name = dr["Name"].ToString(),
                        Mouthful = (int)dr["Mouthful"],
                        PayType = (int)dr["PayType"],
                        Price = (int)dr["Price"],
                        Rise = (int)dr["Rise"],
                        ValidDate = (int)dr["ValidDate"]
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
            }
            return infos.ToArray();
        }
        public AuctionInfo InitAuctionInfo(SqlDataReader reader)
        {
            return new AuctionInfo
            {
                AuctioneerID = (int)reader["AuctioneerID"],
                AuctioneerName = (reader["AuctioneerName"] == null) ? "" : reader["AuctioneerName"].ToString(),
                AuctionID = (int)reader["AuctionID"],
                BeginDate = (DateTime)reader["BeginDate"],
                BuyerID = (int)reader["BuyerID"],
                BuyerName = (reader["BuyerName"] == null) ? "" : reader["BuyerName"].ToString(),
                IsExist = (bool)reader["IsExist"],
                ItemID = (int)reader["ItemID"],
                Mouthful = (int)reader["Mouthful"],
                PayType = (int)reader["PayType"],
                Price = (int)reader["Price"],
                Rise = (int)reader["Rise"],
                ValidDate = (int)reader["ValidDate"],
                Name = reader["Name"].ToString(),
                Category = (int)reader["Category"]
            };
        }
        public bool ScanAuction(ref string noticeUserID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
                };
                para[0].Direction = ParameterDirection.Output;
                this.db.RunProcedure("SP_Auction_Scan", para);
                noticeUserID = para[0].Value.ToString();
                result = true;
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
            }
            return result;
        }
        public bool AddMarryInfo(MarryInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@UserID", info.UserID);
                para[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
                para[3] = new SqlParameter("@Introduction", info.Introduction);
                para[4] = new SqlParameter("@RegistTime", info.RegistTime);
                result = this.db.RunProcedure("SP_MarryInfo_Add", para);
                info.ID = (int)para[0].Value;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("AddMarryInfo", e);
                }
            }
            return result;
        }
        public bool DeleteMarryInfo(int ID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", ID),
                    new SqlParameter("@UserID", userID),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_MarryInfo_Delete", para);
                int returnValue = (int)para[2].Value;
                result = (returnValue == 0);
                if (returnValue == 0)
                {
                    msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed", new object[0]);
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("DeleteAuction", e);
                }
            }
            return result;
        }
        public MarryInfo GetMarryInfoSingle(int ID)
        {
            SqlDataReader reader = null;
            MarryInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", ID)
                };
                this.db.GetReader(ref reader, "SP_MarryInfo_Single", para);
                if (reader.Read())
                {
                    result = new MarryInfo
                    {
                        ID = (int)reader["ID"],
                        UserID = (int)reader["UserID"],
                        IsPublishEquip = (bool)reader["IsPublishEquip"],
                        Introduction = reader["Introduction"].ToString(),
                        RegistTime = (DateTime)reader["RegistTime"]
                    };
                    return result;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("GetMarryInfoSingle", e);
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
        public bool UpdateMarryInfo(MarryInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", info.ID),
                    new SqlParameter("@UserID", info.UserID),
                    new SqlParameter("@IsPublishEquip", info.IsPublishEquip),
                    new SqlParameter("@Introduction", info.Introduction),
                    new SqlParameter("@RegistTime", info.RegistTime),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_MarryInfo_Update", para);
                int returnValue = (int)para[5].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
        {
            List<MarryInfo> infos = new List<MarryInfo>();
            try
            {
                string sWhere;
                if (sex)
                {
                    sWhere = " IsExist=1 and Sex=1 and UserExist=1";
                }
                else
                {
                    sWhere = " IsExist=1 and Sex=0 and UserExist=1";
                }
                if (!string.IsNullOrEmpty(name))
                {
                    sWhere = sWhere + " and NickName like '%" + name + "%' ";
                }
                string sOrder = "State desc,IsMarried";
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@QueryStr", "V_Sys_Marry_Info"),
                    new SqlParameter("@QueryWhere", sWhere),
                    new SqlParameter("@PageSize", size),
                    new SqlParameter("@PageCurrent", page),
                    new SqlParameter("@FdShow", "*"),
                    new SqlParameter("@FdOrder", sOrder),
                    new SqlParameter("@FdKey", "ID"),
                    new SqlParameter("@TotalRow", total)
                };
                para[7].Direction = ParameterDirection.Output;
                DataTable dt = this.db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", para);
                total = (int)para[7].Value;
                foreach (DataRow dr in dt.Rows)
                {
                    infos.Add(new MarryInfo
                    {
                        ID = (int)dr["ID"],
                        UserID = (int)dr["UserID"],
                        IsPublishEquip = (bool)dr["IsPublishEquip"],
                        Introduction = dr["Introduction"].ToString(),
                        NickName = dr["NickName"].ToString(),
                        IsConsortia = (bool)dr["IsConsortia"],
                        ConsortiaID = (int)dr["ConsortiaID"],
                        Sex = (bool)dr["Sex"],
                        Win = (int)dr["Win"],
                        Total = (int)dr["Total"],
                        Escape = (int)dr["Escape"],
                        GP = (int)dr["GP"],
                        Honor = dr["Honor"].ToString(),
                        Style = dr["Style"].ToString(),
                        Colors = dr["Colors"].ToString(),
                        Hide = (int)dr["Hide"],
                        Grade = (int)dr["Grade"],
                        State = (int)dr["State"],
                        Repute = (int)dr["Repute"],
                        Skin = dr["Skin"].ToString(),
                        Offer = (int)dr["Offer"],
                        IsMarried = (bool)dr["IsMarried"],
                        ConsortiaName = dr["ConsortiaName"].ToString(),
                        DutyName = dr["DutyName"].ToString(),
                        Nimbus = (int)dr["Nimbus"],
                        FightPower = (int)dr["FightPower"]
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
            }
            return infos.ToArray();
        }
        public MarryInfo[] GetMarryInfoPages(int page, string name, bool sex, int size, ref int total)
        {
            List<MarryInfo> infos = new List<MarryInfo>();
            try
            {
                string sWhere;
                if (sex)
                {
                    sWhere = " IsExist=1 and Sex=1 and UserExist=1";
                }
                else
                {
                    sWhere = " IsExist=1 and Sex=0 and UserExist=1";
                }
                if (!string.IsNullOrEmpty(name))
                {
                    sWhere = sWhere + " and NickName like '%" + name + "%' ";
                }
                string sOrder = "State desc,IsMarried";
                DataTable dt = base.GetFetch_List(page, size, sOrder, sWhere, "V_Sys_Marry_Info", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    infos.Add(new MarryInfo
                    {
                        ID = (int)dr["ID"],
                        UserID = (int)dr["UserID"],
                        IsPublishEquip = (bool)dr["IsPublishEquip"],
                        Introduction = dr["Introduction"].ToString(),
                        NickName = dr["NickName"].ToString(),
                        IsConsortia = (bool)dr["IsConsortia"],
                        ConsortiaID = (int)dr["ConsortiaID"],
                        Sex = (bool)dr["Sex"],
                        Win = (int)dr["Win"],
                        Total = (int)dr["Total"],
                        Escape = (int)dr["Escape"],
                        GP = (int)dr["GP"],
                        Honor = dr["Honor"].ToString(),
                        Style = dr["Style"].ToString(),
                        Colors = dr["Colors"].ToString(),
                        Hide = (int)dr["Hide"],
                        Grade = (int)dr["Grade"],
                        State = (int)dr["State"],
                        Repute = (int)dr["Repute"],
                        Skin = dr["Skin"].ToString(),
                        Offer = (int)dr["Offer"],
                        IsMarried = (bool)dr["IsMarried"],
                        ConsortiaName = dr["ConsortiaName"].ToString(),
                        DutyName = dr["DutyName"].ToString(),
                        Nimbus = (int)dr["Nimbus"],
                        FightPower = (int)dr["FightPower"]
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
            }
            return infos.ToArray();
        }
        public bool InsertPlayerMarryApply(MarryApplyInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", info.UserID),
                    new SqlParameter("@ApplyUserID", info.ApplyUserID),
                    new SqlParameter("@ApplyUserName", info.ApplyUserName),
                    new SqlParameter("@ApplyType", info.ApplyType),
                    new SqlParameter("@ApplyResult", info.ApplyResult),
                    new SqlParameter("@LoveProclamation", info.LoveProclamation),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[6].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Apply", para);
                result = ((int)para[6].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("InsertPlayerMarryApply", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", UserID),
                    new SqlParameter("@LoveProclamation", loveProclamation),
                    new SqlParameter("@isExist", isExist),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Apply", para);
                result = ((int)para[3].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdatePlayerMarryApply", e);
                }
            }
            return result;
        }
        public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
        {
            SqlDataReader reader = null;
            List<MarryApplyInfo> infos = new List<MarryApplyInfo>();
            MarryApplyInfo[] result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", UserID)
                };
                this.db.GetReader(ref reader, "SP_Get_Marry_Apply", para);
                while (reader.Read())
                {
                    infos.Add(new MarryApplyInfo
                    {
                        UserID = (int)reader["UserID"],
                        ApplyUserID = (int)reader["ApplyUserID"],
                        ApplyUserName = reader["ApplyUserName"].ToString(),
                        ApplyType = (int)reader["ApplyType"],
                        ApplyResult = (bool)reader["ApplyResult"],
                        LoveProclamation = reader["LoveProclamation"].ToString(),
                        ID = (int)reader["Id"]
                    });
                }
                result = infos.ToArray();
                return result;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("GetPlayerMarryApply", e);
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
        public bool InsertMarryRoomInfo(MarryRoomInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[20];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@Name", info.Name);
                para[2] = new SqlParameter("@PlayerID", info.PlayerID);
                para[3] = new SqlParameter("@PlayerName", info.PlayerName);
                para[4] = new SqlParameter("@GroomID", info.GroomID);
                para[5] = new SqlParameter("@GroomName", info.GroomName);
                para[6] = new SqlParameter("@BrideID", info.BrideID);
                para[7] = new SqlParameter("@BrideName", info.BrideName);
                para[8] = new SqlParameter("@Pwd", info.Pwd);
                para[9] = new SqlParameter("@AvailTime", info.AvailTime);
                para[10] = new SqlParameter("@MaxCount", info.MaxCount);
                para[11] = new SqlParameter("@GuestInvite", info.GuestInvite);
                para[12] = new SqlParameter("@MapIndex", info.MapIndex);
                para[13] = new SqlParameter("@BeginTime", info.BeginTime);
                para[14] = new SqlParameter("@BreakTime", info.BreakTime);
                para[15] = new SqlParameter("@RoomIntroduction", info.RoomIntroduction);
                para[16] = new SqlParameter("@ServerID", info.ServerID);
                para[17] = new SqlParameter("@IsHymeneal", info.IsHymeneal);
                para[18] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
                para[19] = new SqlParameter("@Result", SqlDbType.Int);
                para[19].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Room_Info", para);
                result = ((int)para[19].Value == 0);
                if (result)
                {
                    info.ID = (int)para[0].Value;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("InsertMarryRoomInfo", e);
                }
            }
            return result;
        }
        public bool UpdateMarryRoomInfo(MarryRoomInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", info.ID),
                    new SqlParameter("@AvailTime", info.AvailTime),
                    new SqlParameter("@BreakTime", info.BreakTime),
                    new SqlParameter("@roomIntroduction", info.RoomIntroduction),
                    new SqlParameter("@isHymeneal", info.IsHymeneal),
                    new SqlParameter("@Name", info.Name),
                    new SqlParameter("@Pwd", info.Pwd),
                    new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Room_Info", para);
                result = ((int)para[8].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdateMarryRoomInfo", e);
                }
            }
            return result;
        }
        public bool DisposeMarryRoomInfo(int ID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", ID),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Dispose_Marry_Room_Info", para);
                result = ((int)para[1].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("DisposeMarryRoomInfo", e);
                }
            }
            return result;
        }
        public MarryRoomInfo[] GetMarryRoomInfo()
        {
            SqlDataReader reader = null;
            List<MarryRoomInfo> infos = new List<MarryRoomInfo>();
            MarryRoomInfo[] result;
            try
            {
                this.db.GetReader(ref reader, "SP_Get_Marry_Room_Info");
                while (reader.Read())
                {
                    infos.Add(new MarryRoomInfo
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"].ToString(),
                        PlayerID = (int)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        GroomID = (int)reader["GroomID"],
                        GroomName = reader["GroomName"].ToString(),
                        BrideID = (int)reader["BrideID"],
                        BrideName = reader["BrideName"].ToString(),
                        Pwd = reader["Pwd"].ToString(),
                        AvailTime = (int)reader["AvailTime"],
                        MaxCount = (int)reader["MaxCount"],
                        GuestInvite = (bool)reader["GuestInvite"],
                        MapIndex = (int)reader["MapIndex"],
                        BeginTime = (DateTime)reader["BeginTime"],
                        BreakTime = (DateTime)reader["BreakTime"],
                        RoomIntroduction = reader["RoomIntroduction"].ToString(),
                        ServerID = (int)reader["ServerID"],
                        IsHymeneal = (bool)reader["IsHymeneal"],
                        IsGunsaluteUsed = (bool)reader["IsGunsaluteUsed"]
                    });
                }
                result = infos.ToArray();
                return result;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("GetMarryRoomInfo", e);
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
        public MarryRoomInfo GetMarryRoomInfoSingle(int id)
        {
            SqlDataReader reader = null;
            MarryRoomInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", id)
                };
                this.db.GetReader(ref reader, "SP_Get_Marry_Room_Info_Single", para);
                if (reader.Read())
                {
                    result = new MarryRoomInfo
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"].ToString(),
                        PlayerID = (int)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        GroomID = (int)reader["GroomID"],
                        GroomName = reader["GroomName"].ToString(),
                        BrideID = (int)reader["BrideID"],
                        BrideName = reader["BrideName"].ToString(),
                        Pwd = reader["Pwd"].ToString(),
                        AvailTime = (int)reader["AvailTime"],
                        MaxCount = (int)reader["MaxCount"],
                        GuestInvite = (bool)reader["GuestInvite"],
                        MapIndex = (int)reader["MapIndex"],
                        BeginTime = (DateTime)reader["BeginTime"],
                        BreakTime = (DateTime)reader["BreakTime"],
                        RoomIntroduction = reader["RoomIntroduction"].ToString(),
                        ServerID = (int)reader["ServerID"],
                        IsHymeneal = (bool)reader["IsHymeneal"],
                        IsGunsaluteUsed = (bool)reader["IsGunsaluteUsed"]
                    };
                    return result;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("GetMarryRoomInfo", e);
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
        public bool UpdateBreakTimeWhereServerStop()
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[0].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", para);
                result = ((int)para[0].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdateBreakTimeWhereServerStop", e);
                }
            }
            return result;
        }
        public MarryProp GetMarryProp(int id)
        {
            SqlDataReader reader = null;
            MarryProp result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@UserID", id)
                };
                this.db.GetReader(ref reader, "SP_Select_Marry_Prop", para);
                if (reader.Read())
                {
                    result = new MarryProp
                    {
                        IsMarried = (bool)reader["IsMarried"],
                        SpouseID = (int)reader["SpouseID"],
                        SpouseName = reader["SpouseName"].ToString(),
                        IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"],
                        SelfMarryRoomID = (int)reader["SelfMarryRoomID"],
                        IsGotRing = (bool)reader["IsGotRing"]
                    };
                    return result;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("GetMarryProp", e);
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
        public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@UserID", info.UserID);
                para[1] = new SqlParameter("@ApplyUserID", info.ApplyUserID);
                para[2] = new SqlParameter("@ApplyUserName", info.ApplyUserName);
                para[3] = new SqlParameter("@ApplyType", info.ApplyType);
                para[4] = new SqlParameter("@ApplyResult", info.ApplyResult);
                para[5] = new SqlParameter("@LoveProclamation", info.LoveProclamation);
                para[6] = new SqlParameter("@AnswerId", answerId);
                para[7] = new SqlParameter("@ouototal", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Output;
                para[8] = new SqlParameter("@Result", SqlDbType.Int);
                para[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Notice", para);
                id = (int)para[7].Value;
                result = ((int)para[8].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("SavePlayerMarryNotice", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerGotRingProp(int groomID, int brideID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@GroomID", groomID),
                    new SqlParameter("@BrideID", brideID),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_GotRing_Prop", para);
                result = ((int)para[2].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdatePlayerGotRingProp", e);
                }
            }
            return result;
        }
        public RebateChargeInfo[] GetChargeInfo(string UserName, string NickName, ref DateTime firstChargeDate)
        {
            List<SqlParameter> para = new List<SqlParameter>();
            List<RebateChargeInfo> allChargeInfo = new List<RebateChargeInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter outPara = new SqlParameter("FirstChargeDate", SqlDbType.DateTime);
                outPara.Direction = ParameterDirection.Output;
                para.Add(outPara);
                para.Add(new SqlParameter("UserName", UserName));
                para.Add(new SqlParameter("NickName", NickName));
                if (this.db.GetReader(ref reader, "SP_Charge_Reward_ChargeInfo", para.ToArray()))
                {
                    while (reader.Read())
                    {
                        allChargeInfo.Add(new RebateChargeInfo
                        {
                            ChargeID = (string)reader["ChargeID"],
                            UserName = (string)reader["UserName"],
                            NickName = (string)reader["NickName"],
                            Money = (int)reader["Money"],
                            Date = (DateTime)reader["Date"]
                        });
                    }
                    reader.Close();
                    firstChargeDate = (DateTime)outPara.Value;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return allChargeInfo.ToArray();
        }
        public RebateItemInfo[] GetChargeRewardItemsInfo(int chargeMoney, DateTime chargeDate)
        {
            List<SqlParameter> para = new List<SqlParameter>();
            List<RebateItemInfo> allItemsInfo = new List<RebateItemInfo>();
            SqlDataReader reader = null;
            try
            {
                para.Add(new SqlParameter("Money", chargeMoney));
                para.Add(new SqlParameter("ChargeDate", chargeDate));
                if (this.db.GetReader(ref reader, "SP_Charge_Reward_RewardInfo", para.ToArray()))
                {
                    while (reader.Read())
                    {
                        allItemsInfo.Add(new RebateItemInfo
                        {
                            GiftPackageID = (int)reader["GiftPackageID"],
                            RewardItemID = (int)reader["RewardItemID"],
                            Money = (int)reader["Money"],
                            Gold = (int)reader["Gold"],
                            GiftToken = (int)reader["GiftToken"],
                            ItemTemplateID = (int)reader["ItemTemplateID"],
                            Count = (int)reader["Count"],
                            ValidDate = (int)reader["ValidDate"],
                            StrengthLevel = (int)reader["StrengthLevel"],
                            AttackCompose = (int)reader["AttackCompose"],
                            DefendCompose = (int)reader["DefendCompose"],
                            LuckCompose = (int)reader["LuckCompose"],
                            AgilityCompose = (int)reader["AgilityCompose"],
                            IsBind = (bool)reader["IsBind"],
                            NeedSex = (int)reader["NeedSex"],
                            FirstChargeNeeded = (bool)reader["FirstChargeNeeded"]
                        });
                    }
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return allItemsInfo.ToArray();
        }
        public bool DoChargeReward(int userID, RebateChargeInfo chargeInfo, int rewardMoney, int rewardGold, int rewardGiftToken, string rewardITems, string rewardInfo)
        {
            bool result = false;
            try
            {
                List<SqlParameter> para = new List<SqlParameter>();
                string title = LanguageMgr.GetTranslation("PlayerBussiness.DoChargeReward.Title", new object[0]);
                string content = LanguageMgr.GetTranslation("PlayerBussiness.DoChargeReward.Content", new object[0]);
                para.Add(new SqlParameter("Title", title));
                para.Add(new SqlParameter("Content", content));
                para.Add(new SqlParameter("UserID", userID));
                para.Add(new SqlParameter("ChargeID", chargeInfo.ChargeID));
                para.Add(new SqlParameter("ChargeMoney", chargeInfo.Money));
                para.Add(new SqlParameter("Money", rewardMoney));
                para.Add(new SqlParameter("Gold", rewardGold));
                para.Add(new SqlParameter("GiftToken", rewardGiftToken));
                para.Add(new SqlParameter("RewardInfo", rewardInfo));
                para.Add(new SqlParameter("RewardItems", rewardITems));
                result = this.db.RunProcedure("SP_Charge_Reward_DoReward", para.ToArray());
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool Test(string DutyName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@DutyName", DutyName)
                };
                result = this.db.RunProcedure("SP_Test1", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public SpaRoomInfo GetSpaRoomInfoSingle(int id)
        {
            SqlDataReader reader = null;
            SpaRoomInfo result;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@RoomID", id)
                };
                this.db.GetReader(ref reader, "SP_Get_Spa_Room_Info_Single", para);
                if (reader.Read())
                {
                    result = new SpaRoomInfo
                    {
                        RoomID = (int)reader["RoomID"],
                        RoomName = reader["RoomName"].ToString(),
                        PlayerID = (int)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        Pwd = reader["Pwd"].ToString(),
                        AvailTime = (int)reader["AvailTime"],
                        MaxCount = (int)reader["MaxCount"],
                        MapIndex = (int)reader["MapIndex"],
                        BeginTime = (DateTime)reader["BeginTime"],
                        BreakTime = (DateTime)reader["BreakTime"],
                        RoomIntroduction = reader["RoomIntroduction"].ToString(),
                        RoomType = (int)reader["RoomType"],
                        ServerID = (int)reader["ServerID"],
                        RoomNumber = (int)reader["RoomNumber"]
                    };
                    return result;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("GetSpaRoomInfo", e);
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
        public bool UpdateSpaRoomInfo(SpaRoomInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@RoomID", info.RoomID),
                    new SqlParameter("@RoomName", info.RoomName),
                    new SqlParameter("@Pwd", info.Pwd),
                    new SqlParameter("@AvailTime", info.AvailTime),
                    new SqlParameter("@BreakTime", info.BreakTime),
                    new SqlParameter("@roomIntroduction", info.RoomIntroduction),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[6].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Spa_Room_Info", para);
                result = ((int)para[6].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("UpdateSpaRoomInfo", e);
                }
            }
            return result;
        }
        public bool DisposeSpaRoomInfo(int RoomID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@RoomID", RoomID),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Dispose_Spa_Room_Info", para);
                result = ((int)para[1].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("DisposeSpaRoomInfo", e);
                }
            }
            return result;
        }
        public SpaRoomInfo[] GetSpaRoomInfo(int id)
        {
            return null;
        }
        public SpaRoomInfo[] GetSpaRoomInfo()
        {
            SqlDataReader reader = null;
            List<SpaRoomInfo> infos = new List<SpaRoomInfo>();
            SpaRoomInfo[] result;
            try
            {
                this.db.GetReader(ref reader, "SP_Get_Spa_Room_Info");
                while (reader.Read())
                {
                    infos.Add(new SpaRoomInfo
                    {
                        RoomID = (int)reader["RoomID"],
                        RoomName = (reader["RoomName"] == null) ? "" : reader["RoomName"].ToString(),
                        PlayerID = (int)reader["PlayerID"],
                        PlayerName = (reader["PlayerName"] == null) ? "" : reader["PlayerName"].ToString(),
                        Pwd = (reader["Pwd"].ToString() == null) ? "" : reader["Pwd"].ToString(),
                        AvailTime = (int)reader["AvailTime"],
                        MaxCount = (int)reader["MaxCount"],
                        BeginTime = (DateTime)reader["BeginTime"],
                        BreakTime = (DateTime)reader["BreakTime"],
                        RoomIntroduction = (reader["RoomIntroduction"] == null) ? "" : reader["RoomIntroduction"].ToString(),
                        RoomType = (int)reader["RoomType"],
                        ServerID = (int)reader["ServerID"],
                        RoomNumber = (int)reader["RoomNumber"]
                    });
                }
                result = infos.ToArray();
                return result;
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("GetSpaRoomInfo", e);
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
        public bool InsertSpaPubRoomInfo(SpaRoomInfo info)
        {
            bool result2;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@RoomName", info.RoomName),
                    new SqlParameter("@AvailTime", info.AvailTime),
                    new SqlParameter("@MaxCount", info.MaxCount),
                    new SqlParameter("@MapIndex", info.MapIndex),
                    new SqlParameter("@BeginTime", info.BeginTime),
                    new SqlParameter("@BreakTime", info.BreakTime),
                    new SqlParameter("@RoomIntroduction", info.RoomIntroduction),
                    new SqlParameter("@RoomType", info.RoomType),
                    new SqlParameter("@ServerID", info.ServerID),
                    new SqlParameter("@RoomNumber", info.RoomNumber),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[10].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Spa_PubRoom_Info", para);
                int result = (int)para[10].Value;
                if (result > 0)
                {
                    info.RoomID = (int)para[10].Value;
                    result2 = true;
                    return result2;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("InsertSpaRoomInfo", e);
                }
            }
            result2 = false;
            return result2;
        }
        public bool InsertSpaRoomInfo(SpaRoomInfo info)
        {
            bool result2;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@RoomName", info.RoomName),
                    new SqlParameter("@PlayerID", info.PlayerID),
                    new SqlParameter("@PlayerName", info.PlayerName),
                    new SqlParameter("@Pwd", info.Pwd),
                    new SqlParameter("@AvailTime", info.AvailTime),
                    new SqlParameter("@MaxCount", info.MaxCount),
                    new SqlParameter("@MapIndex", info.MapIndex),
                    new SqlParameter("@BeginTime", info.BeginTime),
                    new SqlParameter("@BreakTime", info.BreakTime),
                    new SqlParameter("@RoomIntroduction", info.RoomIntroduction),
                    new SqlParameter("@RoomType", info.RoomType),
                    new SqlParameter("@ServerID", info.ServerID),
                    new SqlParameter("@RoomNumber", info.RoomNumber),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[13].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Spa_Room_Info", para);
                int result = (int)para[13].Value;
                if (result > 0)
                {
                    info.RoomID = (int)para[13].Value;
                    result2 = true;
                    return result2;
                }
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("InsertSpaRoomInfo", e);
                }
            }
            result2 = false;
            return result2;
        }
        public bool UpdateBreakTimeWhereSpaServerStop()
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@Result", SqlDbType.Int)
                };
                para[0].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Spa_Room_Info_Sever_Stop", para);
                result = ((int)para[0].Value == 0);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("Spa_UpdateBreakTimeWhereServerStop", e);
                }
            }
            return result;
        }


        public ChargeInfo[] GetUserChargeList()
        {
            List<ChargeInfo> infos = new List<ChargeInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                };
                this.db.GetReader(ref reader, "SP_Users_ChargeList", para);
                while (reader.Read())
                {
                    ChargeInfo a = new ChargeInfo();
                    a.ID = (int)reader["ID"];
                    a.UserID = (int)reader["UserID"];
                    a.Money = (int)reader["money"];
                    infos.Add(a);
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
        public bool DoUserCharge(int ID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@ID", ID)
                };
                result = this.db.RunProcedure("SP_Users_DoCharge", para);
            }
            catch (Exception e)
            {
                if (true)
                {
                    BaseBussiness.log.Error("DoUserCharge", e);
                }
            }
            return result;
        }

    }
}
