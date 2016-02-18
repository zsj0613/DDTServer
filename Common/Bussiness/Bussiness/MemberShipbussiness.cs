
using SqlDataProvider.BaseClass;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
namespace Bussiness
{
	public class MemberShipbussiness : BaseBussiness
	{
        public MemberShipbussiness()
        {
            this.db = new Sql_DbObject("AppConfig", "conmemString");
        }
		public bool ExistsUsername(string username)
		{
			SqlParameter[] para = new SqlParameter[]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			para[1].Direction = ParameterDirection.ReturnValue;
			bool result = this.db.RunProcedure("Mem_Users_CheckByUsername", para);
            if (result)
            { return (int)para[1].Value > 0; }
            else return false;
		}
        public bool CreateUsername(string username, string password,int inviteid)
		{
			SqlParameter[] para = new SqlParameter[]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Password", password),
				new SqlParameter("@Inviteid", inviteid),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			para[3].Direction = ParameterDirection.ReturnValue;
			bool result = this.db.RunProcedure("Mem_Users_CreateUser", para);
			if (result)
			{
				result = ((int)para[3].Value > 0);
			}
			return result;
		}
        public bool CheckUser(string username, string password)
        {
            SqlParameter[] array = new SqlParameter[]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Password", password),
				new SqlParameter("@UserId", SqlDbType.Int)
			};
            array[2].Direction = ParameterDirection.ReturnValue;
            bool flag = this.db.RunProcedure("Mem_Users_Check", array);
            int num = 0;
            int.TryParse(array[2].Value.ToString(), out num);
            return num > 0;
        }

    }
}
