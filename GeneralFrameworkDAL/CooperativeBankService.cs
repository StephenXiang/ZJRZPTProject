using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GeneralFrameworkDAL
{
    public class CooperativeBankService
    {
        public bool AddCooperativeBank(CooperativeBank bank)
        {
            bool IsDesplay = false;
            if (bank.IsDesplay == 0)
            {
                IsDesplay = false;
            }
            else
            {
                IsDesplay = true;
            }
            string sql = @"insert into CooperativeBank(BankName,logo1,logo2,BankDesc,Sort,Leader,Phone,IsDesplay)values(@BankName,@logo1,@logo2,@BankDesc,@sort,@Leader,@Phone,@IsDesplay)";
            int? id = DBHelper.Execute(sql,
                new SqlParameter("@BankName", bank.BankName.Trim()),
                new SqlParameter("@logo1", bank.logo1),
                new SqlParameter("@logo2", bank.logo2),
                new SqlParameter("@BankDesc", bank.BankDesc),
                new SqlParameter("@sort", bank.sort),
                new SqlParameter("@Leader", bank.Leader),
                new SqlParameter("@Phone", bank.Phone), new SqlParameter("@IsDesplay", IsDesplay)) as int?;
            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EditCooperativeBank(CooperativeBank bank)
        {
            int? id = 0;
            bool IsDesplay = false;
            if (bank.IsDesplay == 0)
            {
                IsDesplay = false;
            }
            else
            {
                IsDesplay = true;
            }
            if (bank.logo1.Length == 0 && bank.logo2.Length == 0)
            {
                string sql = @"update CooperativeBank set BankName=@BankName,BankDesc=@BankDesc,Sort=@sort,Leader=@Leader,Phone=@Phone,IsDesplay =@IsDesplay where Id = @Id";
                id = DBHelper.Execute(sql,
                   new SqlParameter("@Id", bank.Id),
                   new SqlParameter("@BankName", bank.BankName.Trim()),
                   new SqlParameter("@BankDesc", bank.BankDesc),
                   new SqlParameter("@sort", bank.sort),
                   new SqlParameter("@Leader", bank.Leader),
                   new SqlParameter("@Phone", bank.Phone), new SqlParameter("@IsDesplay", IsDesplay)) as int?;
            }
            else if (bank.logo1.Length > 0 && bank.logo2.Length == 0)
            {
                string sql = @"update CooperativeBank set BankName=@BankName,logo1=@logo1,BankDesc=@BankDesc,Sort=@sort,Leader=@Leader,Phone=@Phone,IsDesplay =@IsDesplay where Id = @Id";
                id = DBHelper.Execute(sql,
                   new SqlParameter("@Id", bank.Id),
                   new SqlParameter("@BankName", bank.BankName.Trim()),
                   new SqlParameter("@logo1", bank.logo1),
                   new SqlParameter("@BankDesc", bank.BankDesc),
                   new SqlParameter("@sort", bank.sort),
                   new SqlParameter("@Leader", bank.Leader),
                   new SqlParameter("@Phone", bank.Phone), new SqlParameter("@IsDesplay", IsDesplay)) as int?;
            }
            else if (bank.logo2.Length > 0 && bank.logo1.Length == 0)
            {
                string sql = @"update CooperativeBank set BankName=@BankName,logo2=@logo2,BankDesc=@BankDesc,Sort=@sort,Leader=@Leader,Phone=@Phone,IsDesplay =@IsDesplay where Id = @Id";
                id = DBHelper.Execute(sql,
                   new SqlParameter("@Id", bank.Id),
                   new SqlParameter("@BankName", bank.BankName.Trim()),
                   new SqlParameter("@logo2", bank.logo2),
                   new SqlParameter("@BankDesc", bank.BankDesc),
                   new SqlParameter("@sort", bank.sort),
                   new SqlParameter("@Leader", bank.Leader),
                   new SqlParameter("@Phone", bank.Phone), new SqlParameter("@IsDesplay", IsDesplay)) as int?;
            }
            else if (bank.logo2.Length > 0 && bank.logo1.Length > 0)
            {
                string sql = @"update CooperativeBank set BankName=@BankName,logo1=@logo1,logo2=@logo2,BankDesc=@BankDesc,Sort=@sort,Leader=@Leader,Phone=@Phone,IsDesplay =@IsDesplay where Id = @Id";
                id = DBHelper.Execute(sql,
                   new SqlParameter("@Id", bank.Id),
                   new SqlParameter("@BankName", bank.BankName.Trim()),
                   new SqlParameter("@logo1", bank.logo1),
                   new SqlParameter("@logo2", bank.logo2),
                   new SqlParameter("@BankDesc", bank.BankDesc),
                   new SqlParameter("@sort", bank.sort),
                   new SqlParameter("@Leader", bank.Leader),
                   new SqlParameter("@Phone", bank.Phone), new SqlParameter("@IsDesplay", IsDesplay)) as int?;
            }

            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetCooperativeBankDG(int page, int rows)
        {
            var sql = @"select Id,BankName,BankDesc,Sort,Leader,Phone,IsDesplay from CooperativeBank order by sort asc";
            var dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JSON.JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }

        public string GetBankInfoList()
        {
            var sql = @"select Id,BankName as Name from CooperativeBank where IsDesplay = 0 order by sort asc";
            var dt1 = DBHelper.GetDataSet(sql);
            DataTable dt = new DataTable();
            var reply = JSON.JsonHelper.SerializeObject(dt1);
            return reply;
        }

        public bool DelCooperativeBank(int Id)
        {
            var sql = "delete CooperativeBank where Id = @Id";
            int? id = DBHelper.Execute(sql, new SqlParameter("@Id", Id)) as int?;
            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetBankInfolbForBankId(int BankId)
        {
            var sql = @"select Id,BankName,BankDesc,CreateDate from CooperativeBank where Id='" + BankId + "'";
            var dt1 = DBHelper.GetDataSet(sql);
            DataTable dt = new DataTable();
            var reply = JSON.JsonHelper.SerializeObject(dt1);
            return reply;
        }
        public byte[] GetLogoImgForId(string Id)
        {
            var sql = "select Logo1 from CooperativeBank where Id = '" + Id + "'";
            return (byte[])DBHelper.GetScalar(sql);
        }
        public string GetMainBankCmb()
        {
            var sql = string.Format(" select Id as ID,BankName as TypeName from CooperativeBank order by Sort asc ");
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }
    }

}
