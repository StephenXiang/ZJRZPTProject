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
                sql = @"select a.Id,a.EnterpriseId,c.Name,b.Quota,a.PublishDate,a.Status,a.BankIds from RZFlow a 
left join RZDemandInfo b on a.DemandId = b.Id
left join Enterprise c on a.EnterpriseId = c.ID where BankIds like '%" + BankId + "%'";
                var dt1 = DBHelper.GetDataSet(sql);
                if (dt1.Rows.Count == 0)
                {
                    return "";
                }
                else
                {
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
                                if (int.Parse(bankids[i]) == BankId)
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
            }
            else
            {
                return "";
            }
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
                var efi = new RzInfoReply
                {
                    RZED = dr[0].ToString(),
                    RZYH = GetBankNamesByUserName(UserName),
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
            var sql = string.Format(@"select Name from Bank where Id in({0})", BankIDs);
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

        public bool EditRZStatus(string status, string rzid, string UserName, int i = 0)
        {
            string sql = string.Empty;
            if (status == "2")
            {
                sql = @"update  RZFlow set [Status] = @status,BankIds = @BankId where Id = @rzid";
            }
            else
            {
                sql = @"update  RZFlow set [Status] = @status where Id = @rzid";
            }
            var sid = DBHelper.Execute(sql, new SqlParameter("@status", status), new SqlParameter("@rzid", rzid), new SqlParameter("@BankId", GetBankIdByUserName(UserName)));
            if (sid > 0)
            {
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
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
