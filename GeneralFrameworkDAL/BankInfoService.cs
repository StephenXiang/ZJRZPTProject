using GeneralFrameworkBLLModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GeneralFrameworkDAL
{
    public class BankInfoService
    {
        public string SaveBankInfo(Bank bank)
        {
            var sql = @"insert into Bank(Name,[Address],Connector,ConnectorPhone,MainBankId,Logo,Logo2,[Desc])values(@name,@address,@connector,@ConnectorPhone,@MainBankId,@logo,@logo2,@Desc)";
            var id = DBHelper.Execute(sql, new SqlParameter("@name", bank.Name.Trim()),
                new SqlParameter("@address", bank.Address.Trim()),
                new SqlParameter("@connector", bank.Connector.Trim()),
                new SqlParameter("@ConnectorPhone", bank.ConnectorPhone.Trim()),
                new SqlParameter("@logo", bank.logo1),
                new SqlParameter("@logo2", bank.logo2),
                new SqlParameter("@Desc", bank.BankDesc),
                new SqlParameter("@MainBankId", bank.MainBankId)) as int?;
            if (id > 0)
            {
                sql = string.Format(@"select Id from Bank where Name = '{0}'", bank.Name);
                var ent = DBHelper.GetScalar(sql) as int?;
                if (ent == -1) return "";
                sql = @"update SysUser set BankId = @BankId where UserName =@userName";
                var sid = DBHelper.Execute(sql, new SqlParameter("@BankId", ent),
                    new SqlParameter("@userName", bank.UserName));
                if (sid > 0)
                {
                    return ent.ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }

        }

        public string GetBankId(string userName)
        {
            string sql = @"select BankId from SysUser where UserName = '" + userName + "' ";
            DataTable dt = DBHelper.GetDataSet(sql);
            string bankID = string.Empty;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                bankID = dr[0].ToString();
                return bankID;
            }
            else
            {
                return "";
            }
        }

        public string GetBankInfolbForBankId(string BankId)
        {
            var sql = @"select a.Id, Name,[Address],Connector,ConnectorPhone,MainBankId,c.BankName,a.[Desc],a.EditDate  from Bank a 
left join MainBank c on a.MainBankId = c.id where a.Id = " + BankId + "";
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.SerializeObject(dt);
            return reply;
        }

        // 获取所有银行名称
        public static Dictionary<int, string> GetBanks()
        {
            var dic = new Dictionary<int, string>();
            var sql = @"select Id,Name from Bank";
            var dt1 = DBHelper.GetDataSet(sql);
            foreach (DataRow dr in dt1.Rows)
            {
                dic.Add(dr.Field<int>("Id"), dr["Name"].ToString());
            }
            return dic;
        }

        public string GetBankInfoList()
        {
            var sql = @"select Id,Name from Bank";
            var dt1 = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.SerializeObject(dt1);
            return reply;
        }

        public byte[] GetLogo2ImgForId(string Id)
        {
            var sql = "select Logo2 from Bank where Id = '" + Id + "'";
            return (byte[])DBHelper.GetScalar(sql);
        }

        public byte[] GetLogoImgForId(string Id)
        {
            var sql = "select Logo from Bank where Id = '" + Id + "'";
            return (byte[])DBHelper.GetScalar(sql);
        }


        public string GetMainBank(int page, int rows)
        {
            var sql = @"select Id,BankName from MainBank order by Id Desc";
            var dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JSON.JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }

        public bool AddMainBank(string bankname)
        {
            var sql = @"select Id,BankName from MainBank where BankName ='" + bankname + "'";
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                return false;
            }
            else
            {
                sql = "insert into MainBank(BankName)values('" + bankname + "')";
                var ent = DBHelper.GetScalar(sql) as int?;
                if (ent == -1) return false;
            }
            return true;
        }

        public bool DelMainBank(int mainBankId)
        {
            var sql = "delete MainBank where Id = '" + mainBankId + "'";
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
