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
    public class RZExamineApproveService
    {
        public string GetRZDataTable(string UserName, int page, int rows)
        {
            var sql = @"select BankId,RolesID from SysUser where UserName = '" + UserName + "'";
            int BankId = 0;
            int RoleId = 0;
            int ParentBankId = 0;
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                BankId = dr["BankId"] == DBNull.Value ? 0 : int.Parse(dr["BankId"].ToString());
                RoleId = dr["RolesID"] == DBNull.Value ? 0 : int.Parse(dr["RolesID"].ToString());
                if (BankId != 0)
                {
                    sql = "select ParentBankId from Bank where Id = '" + BankId + "'";
                    dt = DBHelper.GetDataSet(sql);
                    dr = dt.Rows[0];
                    ParentBankId = dr["ParentBankId"] == DBNull.Value ? 0 : int.Parse(dr["ParentBankId"].ToString());
                }
            }
            else
            {
                return "";
            }

            if (ParentBankId != 0)
            {
                sql = @"select a.Id,a.EnterpriseId,c.Name,b.Quota,CONVERT(varchar(100), PublishDate, 23) as PublishDate,a.Status,a.BankIds,a.SLBankId,d.Name as SLBankName,b.CreditAmount,CONVERT(varchar(100), b.CreditDate, 23) as CreditDate,a.Status as cz from RZFlow a 
left join RZDemandInfo b on a.DemandId = b.Id
left join Enterprise c on a.EnterpriseId = c.ID
left join Bank d on a.SLBankId = d.Id 
where BankIds like '%" + ParentBankId + "%' and a.IsDeleted=0 order by PublishDate desc";
                var dt1 = DBHelper.GetDataSet(sql);
                if (dt1.Rows.Count == 0)
                {
                    return "";
                }
                DataTable dtclone = new DataTable();
                dtclone = dt1.Copy();
                int dt1index = 0;
                foreach (DataRow dr in dt1.Rows)
                {
                    string[] bankids = dr["BankIds"].ToString().Split(',');
                    bool iscz = false;
                    if (bankids.Length > 0)
                    {
                        for (int i = 0; i < bankids.Length; i++)
                        {
                            if (int.Parse(bankids[i]) == ParentBankId)
                            {
                                iscz = true;
                                break;
                            }
                        }
                        if (iscz == false)
                        {
                            dtclone.Rows.RemoveAt(dt1index);
                        }
                    }
                    dt1index++;
                }
                return JsonHelper.TableToJson(dtclone.Rows.Count, GetPagedTable(dtclone, page, rows));
            }
            if (RoleId == 1 || RoleId == 2)    // 系统用户或政府部门
            {
                sql = @"select a.Id,a.EnterpriseId,c.Name,b.Quota,CONVERT(varchar(100), a.PublishDate, 23) as PublishDate,a.Status,a.BankIds,d.Name as SLBankName,b.CreditAmount,CONVERT(varchar(100), b.CreditDate, 23) as CreditDate from RZFlow a 
left join RZDemandInfo b on a.DemandId = b.Id
left join Enterprise c on a.EnterpriseId = c.ID
left join Bank d on a.SLBankId = d.Id 
where a.IsDeleted=0 order by PublishDate desc";
                var banks = BankInfoService.GetBanks();
                var dt1 = DBHelper.GetDataSet(sql);
                if (dt1.Rows.Count == 0)
                {
                    return "";
                }
                dt1.Columns.Add("Banks");
                foreach (DataRow dr in dt1.Rows)
                {
                    //dr["Banks"] = string.Join(",",
                    //    dr["BankIds"].ToString().Split(',').Select(int.Parse)
                    //        .Select(p => banks.ContainsKey(p) ? banks[p] : ""));
                    dr["Banks"] = GetBankNamesByBankIds(dr["BankIds"].ToString());
                }
                return JsonHelper.TableToJson(dt1.Rows.Count, GetPagedTable(dt1, page, rows));
            }
            return "";
        }

        public DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)//PageIndex表示第几页，PageSize表示每页的记录数
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy dt的框架

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;//源数据记录数小于等于要显示的记录，直接返回dt

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }

        public string GetRZInfoByRZID(string RZID, string UserName)
        {
            string reply = null;
            var sql = string.Format(@"select b.Quota,a.BankIds,e.[Desc],d.[Desc],(case b.HadCollateral when '0' then'无' when '1' then '有' else '无' end) dyw,b.CollateralDesc  from RZFlow a 
left join RZDemandInfo b on a.DemandId = b.Id
left join Enterprise c on a.EnterpriseId = c.ID
left join (select Id,[Type],[Desc] from Lookup where Type = 9) d on b.PurposeId = d.Id
left join (select Id,[Type],[Desc] from Lookup where Type = 8) e on b.TermId = e.Id
where a.Id = {0}", RZID);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                var banks = BankInfoService.GetBanks();
                var efi = new RzInfoReply
                {
                    RZED = dr[0].ToString(),
                    //RZYH = string.Join(",",
                    //    dr["BankIds"].ToString().Split(',').Select(int.Parse)
                    //        .Select(p => banks.ContainsKey(p) ? banks[p] : "")),
                    RZYH = GetBankNamesByBankIds(dr["BankIds"].ToString()),
                    RZQX = dr[2].ToString(),
                    RZQT = dr[3].ToString(),
                    isdyw = dr[4].ToString(),
                    dywdesc = dr[5].ToString() == "" ? "无" : dr[5].ToString()

                };
                reply = JsonHelper.SerializeObject(efi);
            }
            else
            {
                reply = "";
            }
            return reply;
        }

        public string GetBankNamesByUserName(string UserName)
        {
            var sql = @"select BankId from SysUser where UserName = '" + UserName + "'";
            int BankId = 0;
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                BankId = int.Parse(dr[0].ToString());
            }
            else
            {
                return "";
            }
            if (BankId != 0)
            {
                sql = string.Format(@"select Name from Bank where Id in({0})", BankId);
                string BankName = (string)DBHelper.GetScalar(sql);
                return BankName;

            }
            else
            {
                return "";
            }
        }

        public string GetBankIdByUserName(string UserName)
        {
            var sql = @"select BankId from SysUser where UserName = '" + UserName + "'";
            int BankId = 0;
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                BankId = int.Parse(dr[0].ToString());
                return BankId.ToString();
            }
            else
            {
                return "";
            }

        }


        public string GetBankNamesByBankIds(string BankIDs)
        {
            string BankNames = string.Empty;
            var sql = string.Format(@"select BankName as Name from CooperativeBank where Id in({0})", BankIDs);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (i == 0)
                    {
                        BankNames = dr[0].ToString();
                    }
                    else
                    {
                        BankNames = BankNames + "," + dr[0].ToString();
                    }
                }
                return BankNames;
            }
            else
            {
                return "";
            }
        }



        public bool EditRZStatus(string status, string rzid, string UserName, string slje = null, string slrq = null, int i = 0)
        {
            string sql = string.Empty;
            if (status == "2")
            {
                sql = @"update  RZFlow set [Status] = @status,SLBankId = @BankId where Id = @rzid";
            }
            else
            {
                sql = @"update  RZFlow set [Status] = @status where Id = @rzid";
            }
            var sid = DBHelper.Execute(sql, new SqlParameter("@status", status), new SqlParameter("@rzid", rzid), new SqlParameter("@BankId", GetBankIdByUserName(UserName)));
            if (sid > 0)
            {
                if (status == "3")
                {
                    sql = "select DemandId from RZFlow where Id = '" + rzid + "'";
                    DataTable demandiddt = DBHelper.GetDataSet(sql);
                    DataRow dr = demandiddt.Rows[0];
                    sql = "update RZDemandInfo set CreditAmount = @slje,CreditDate=@slrq where Id=@id";
                    sid = DBHelper.Execute(sql, new SqlParameter("@slje", slje), new SqlParameter("@slrq", slrq), new SqlParameter("@id", dr[0].ToString()));
                    List<string> phonelist = new List<string>();
                    sql = string.Format(@"select b.ConnectionTelephone from RZFlow a left join Enterprise b on a.EnterpriseId = b.ID where a.Id = {0}", rzid);
                    DataTable yhdt = DBHelper.GetDataSet(sql);
                    if (yhdt.Rows.Count > 0)
                    {
                        foreach (DataRow yhdr in yhdt.Rows)
                        {
                            string phone = yhdr[0].ToString();
                            phonelist.Add(phone);
                        }
                    }
                    SmsService sms = new SmsService();
                    for (int h = 0; h < phonelist.Count; h++)
                    {
                        sms.Send(phonelist[h].ToString(), "有一笔融资申请受理成功，请及时登陆镇江融资平台查看详细信息");
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool EditRZStatus(string status, string rzid, string liyou)
        {
            string sql = string.Empty;
            sql = @"update RZFlow set [Status] = @status,Feedback = @Feedback where Id = @rzid";
            var sid = DBHelper.Execute(sql, new SqlParameter("@status", status), new SqlParameter("@rzid", rzid), new SqlParameter("@Feedback", liyou));
            if (sid > 0)
            {
                List<string> phonelist = new List<string>();
                sql = "select Phone from SysUser where RolesID = 2";
                DataTable dt = DBHelper.GetDataSet(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string phone = dr[0].ToString();
                        phonelist.Add(phone);
                    }
                }
                sql = string.Format(@"select b.ConnectionTelephone from RZFlow a left join Enterprise b on a.EnterpriseId = b.ID where a.Id = {0}", rzid);
                DataTable yhdt = DBHelper.GetDataSet(sql);
                if (yhdt.Rows.Count > 0)
                {
                    foreach (DataRow yhdr in yhdt.Rows)
                    {
                        string phone = yhdr[0].ToString();
                        phonelist.Add(phone);
                    }
                }
                SmsService sms = new SmsService();
                for (int h = 0; h < phonelist.Count; h++)
                {
                    sms.Send(phonelist[h].ToString(), "有一笔融资申请受理不成功，请及时登陆镇江融资平台查看详细信息");
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
