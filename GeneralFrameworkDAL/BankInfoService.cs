using GeneralFrameworkBLLModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using GeneralFrameworkDAL.JSON;

namespace GeneralFrameworkDAL
{
    public class BankInfoService
    {
        public string UpdateBankInfo(Bank bank)
        {
            var sql = @"update Bank set Name=@name,[Address]=@address,ParentBankId=@ParentBankId,iszzd=@iszzd,BankType=@BankType
where Id=@bankid";
            var efc = DBHelper.Execute(sql,
                new SqlParameter("@bankid", bank.ID),
                new SqlParameter("@name", bank.Name.Trim()),
                new SqlParameter("@address", bank.Address.Trim()),
                //new SqlParameter("@connector", bank.Connector.Trim()),
                //new SqlParameter("@ConnectorPhone", bank.ConnectorPhone.Trim()),
                 new SqlParameter("@BankType", bank.BankType),
                new SqlParameter("@ParentBankId", bank.ParentBankId),
                new SqlParameter("@iszzd", bank.iszzd)) as int?;
            //if (bank.logo1 != null)
            //{
            //    sql = @"update Bank set Logo=@logo where Id=@bankid";
            //    DBHelper.Execute(sql,
            //    new SqlParameter("@bankid", bank.ID),
            //    new SqlParameter("@logo", bank.logo1));
            //}
            //if (bank.logo2 != null)
            //{
            //    sql = @"update Bank set Logo2=@logo2 where Id=@bankid";
            //    DBHelper.Execute(sql,
            //    new SqlParameter("@bankid", bank.ID),
            //    new SqlParameter("@logo2", bank.logo2));
            //}
            return bank.ID.ToString();
        }

        public string SaveBankInfo(Bank bank)
        {
            if (bank.ID > 0) return UpdateBankInfo(bank);
            string sql = string.Empty;
            int? id = 0;
            //if (bank.iszzd == 1)
            //{
            //    sql = @"insert into Bank(Name,[Address],Connector,ConnectorPhone,MainBankId,Logo,Logo2,[Desc],iszzd)values(@name,@address,@connector,@ConnectorPhone,@MainBankId,@logo,@logo2,@Desc,@iszzd)";
            //    id = DBHelper.Execute(sql, new SqlParameter("@name", bank.Name.Trim()),
            //        new SqlParameter("@address", bank.Address.Trim()),
            //        new SqlParameter("@connector", bank.Connector.Trim()),
            //        new SqlParameter("@ConnectorPhone", bank.ConnectorPhone.Trim()),
            //        new SqlParameter("@logo", bank.logo1),
            //        new SqlParameter("@logo2", bank.logo2),
            //        new SqlParameter("@Desc", bank.BankDesc),
            //        new SqlParameter("@MainBankId", bank.MainBankId),
            //        new SqlParameter("@iszzd", bank.iszzd)) as int?;
            //}
            //else
            //{
            sql = @"insert into Bank(Name,[Address],ParentBankId,iszzd,BankType)values(@name,@address,@ParentBankId,@iszzd,@BankType)";
            id = DBHelper.Execute(sql, new SqlParameter("@name", bank.Name.Trim()),
                new SqlParameter("@address", bank.Address.Trim()),
                //new SqlParameter("@connector", bank.Connector.Trim()),
                //new SqlParameter("@ConnectorPhone", bank.ConnectorPhone.Trim()),
                new SqlParameter("@BankType", bank.BankType),
                new SqlParameter("@ParentBankId", bank.ParentBankId),
                new SqlParameter("@iszzd", bank.iszzd)) as int?;
            //}

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
            var sql = @"select a.Id, Name,[Address],Connector,ConnectorPhone,ParentBankId,c.BankName,a.[Desc],a.EditDate,c.Leader,C.Phone,iszzd,BankType from Bank a 
left join CooperativeBank c on a.ParentBankId = c.id where a.Id = " + BankId + "";
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.SerializeObject(dt);
            return reply;
        }

        public string GetBankDg(int page, int rows)
        {
            var sql = @"select a.Id, Name,[Address],Connector,ConnectorPhone,ParentBankId,c.BankName,c.Leader,C.Phone,iszzd,a.EditDate,a.BankType from Bank a 
left join CooperativeBank c on a.ParentBankId = c.id";
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
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
            var sql = @"select Id,Name,MainBankId from Bank where iszzd = 1 order by sort asc";
            var dt1 = DBHelper.GetDataSet(sql);
            DataTable dt = new DataTable();
            dt = GetDistinctSelf(dt1, "MainBankId");
            var reply = JSON.JsonHelper.SerializeObject(dt1);
            return reply;
        }

        public DataTable GetDistinctSelf(DataTable SourceDt, string filedName)
        {
            for (int i = SourceDt.Rows.Count - 2; i > 0; i--)
            {
                DataRow[] rows = SourceDt.Select(string.Format("{0}='{1}'", filedName, SourceDt.Rows[i][filedName]));
                if (rows.Length > 1)
                {
                    SourceDt.Rows.RemoveAt(i);
                }
            }
            return SourceDt;
        }

        public byte[] GetLogo2ImgForId(string Id)
        {
            var sql = "select b.logo2 from Bank a left join CooperativeBank b on a.ParentBankId = b.Id where a.Id = '" + Id + "'";
            return (byte[])DBHelper.GetScalar(sql);
        }

        public byte[] GetLogoImgForId(string Id)
        {
            var sql = "select b.logo1 from Bank a left join CooperativeBank b on a.ParentBankId = b.Id where a.Id = '" + Id + "'";
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

        public bool AddLiaisonanMan(LiaisonanMan lman)
        {
            string sql = "insert into LiaisonanMan (BankId,Name,Post,Phone)values(@BankId,@Name,@Post,@Phone)";

            var sid = DBHelper.Execute(sql,
                new SqlParameter("@BankId", lman.BankId),
                new SqlParameter("@Name", lman.Name),
                new SqlParameter("@Post", lman.Post),
                new SqlParameter("@Phone", lman.Phone));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetLiaisonanManDG(int BankId, int page, int rows)
        {
            var sql = "select a.ID,a.Name,a.Post,a.Phone,b.Name as BankName from LiaisonanMan a left join Bank b on a.BankId = b.Id where a.BankId = '" + BankId + "' and IsDeleteed != 1";
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }

        public bool EditLiaisonanMan(LiaisonanMan lman)
        {
            var sql = "update LiaisonanMan set Name = @Name,Post= @Post,Phone=@Phone where ID = @Id";
            var sid = DBHelper.Execute(sql,
               new SqlParameter("@Id", lman.Id),
               new SqlParameter("@Name", lman.Name),
               new SqlParameter("@Post", lman.Post),
               new SqlParameter("@Phone", lman.Phone));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DelLiaisonanMan(LiaisonanMan lman)
        {
            var sql = "Update LiaisonanMan set IsDeleteed = 1 where ID = @Id";
            var sid = DBHelper.Execute(sql,
              new SqlParameter("@Id", lman.Id));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //        public string GetBankDg(int page, int rows)
        //        {
        //            var sql = @"select a.Id, Name,[Address],Connector,ConnectorPhone,ParentBankId,c.BankName,a.EditDate from Bank a 
        //left join CooperativeBank c on a.ParentBankId = c.id";
        //            DataTable dt = DBHelper.GetDataSet(sql);
        //            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
        //            return reply;
        //        }
    }
}
