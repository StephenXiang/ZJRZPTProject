using System.Data.SqlClient;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;
using System.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace GeneralFrameworkDAL
{
    public class PublishRzService
    {
        public string GetBanks()
        {
            var sql = "select Id as ID,[Name] as TypeName from [Bank]";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public bool SaveRzInfo(RzInfo rzi)
        {
            bool isDyw = false;
            if (rzi.dywdesc.Trim() != "" && rzi.dywdesc != null)
            {
                isDyw = true;
            }
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", rzi.UserName.Trim());
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return false;
            sql = @"
insert into RZDemandInfo (Quota,TermId,PurposeId,HadCollateral,CollateralDesc)
values(@qua,@tem,@pup,@hc,@cd)
SELECT @@IDENTITY";
            var demid = DBHelper.GetScalar(sql,
                new SqlParameter("@qua", rzi.RZED),
                new SqlParameter("@tem", rzi.RZQX),
                new SqlParameter("@pup", rzi.RZQT),
                new SqlParameter("@hc", isDyw),
                new SqlParameter("@cd", rzi.dywdesc.Trim()));
            if (demid == null) return false;
            sql = @"
insert into RZFlow(EnterpriseId,BankIds,FinanceId,DemandId,[Status])
values(@ent,@bks,(select top 1 Id from RZFinance where EnterpriseId=@ent),@dm,0)";
            var efc = DBHelper.Execute(sql,
                new SqlParameter("@ent", ent),
                new SqlParameter("@bks", rzi.RZYH),
                new SqlParameter("@dm", demid));
            if (efc > 0)
            {
                List<PhoneList> phonelist = new List<PhoneList>();
                sql = "select Phone from SysUser where RolesID = 2";
                DataTable dt = DBHelper.GetDataSet(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        PhoneList phone = new PhoneList();
                        phone.Phone = dr[0].ToString();
                        phonelist.Add(phone);
                    }
                }
                sql = string.Format(@"select ConnectorPhone from Bank where Id in({0})", rzi.RZYH);
                DataTable yhdt = DBHelper.GetDataSet(sql);
                if (yhdt.Rows.Count > 0)
                {
                    foreach (DataRow yhdr in yhdt.Rows)
                    {
                        PhoneList phone = new PhoneList();
                        phone.Phone = yhdr[0].ToString();
                        phonelist.Add(phone);
                    }
                }
                SmsService sms = new SmsService();
                for (int h = 0; h < phonelist.Count; h++)
                {
                    sms.Send(phonelist[h].ToString(), "有新的融资申请，请及时登陆镇江融资平台查看详细信息");
                }
            }
            return efc > 0;
        }

        struct PhoneList
        {
            public string Phone;
        }

        public string GetRZLBJson(string UserName, int page, int rows)
        {
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", UserName);
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return "";
            sql = string.Format(@"select b.Id as ID,a.Quota as RZED,c.[Desc] as RZQX,b.BankIds as SXYH, d.[Desc] as RZYT,a.CollateralDesc as DYW,b.PublishDate,b.[Status] from RZDemandInfo a 
left join RZFlow b on a.Id = b.DemandId
left join (select * from Lookup where Type = 8) c on a.TermId = c.Id
left join (select * from Lookup where Type = 9) d on a.PurposeId = d.Id
where b.EnterpriseId = {0} order by b.Id Desc", ent);
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }

        public string GetRZBankstr(string BankIds)
        {
            string Bankstr = string.Empty;
            var sql = string.Format(@"select Name from Bank where Id in({0})", BankIds);
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (i == 0)
                    {
                        Bankstr = dr["Name"].ToString();
                    }
                    else
                    {
                        Bankstr = Bankstr + "," + dr["Name"].ToString();
                    }

                }
                return Bankstr;
            }
            else
            {
                return "无";
            }
        }

        public string GetIndexTJJson()
        {
            DataTable rzdt = new DataTable();
            DataTable zzddt = new DataTable();
            DataRow dr;
            int fbrzsl = 0;
            double fbrzje = 0;
            int djrzsl = 0;
            double djrzje = 0;
            int djcgsl = 0;
            double djcgje = 0;

            int fbzzdsl = 0;
            double fbzzdje = 0;
            int djzzdsl = 0;
            double djzzdje = 0;
            int djzzdcgsl = 0;
            double djzzdcgje = 0;

            var RZsql = "select * from RZFlow";
            rzdt = DBHelper.GetDataSet(RZsql);
            fbrzsl = rzdt.Rows.Count;
            RZsql = "select SUM(Quota) from RZDemandInfo";
            rzdt = DBHelper.GetDataSet(RZsql);
            dr = rzdt.Rows[0];
            fbrzje = double.Parse(dr[0].ToString() == "" ? "0.0" : dr[0].ToString());

            RZsql = "select * from RZFlow a left join RZDemandInfo b on a.DemandId = b.Id where a.Status in (1,2)";
            rzdt = DBHelper.GetDataSet(RZsql);
            djrzsl = rzdt.Rows.Count;
            RZsql = "select SUM(Quota) from RZFlow a left join RZDemandInfo b on a.DemandId = b.Id where a.Status in (1,2)";
            rzdt = DBHelper.GetDataSet(RZsql);
            dr = rzdt.Rows[0];
            djrzje = double.Parse(dr[0].ToString() == "" ? "0.0" : dr[0].ToString());

            RZsql = "select * from RZFlow a left join RZDemandInfo b on a.DemandId = b.Id where a.Status = 3";
            rzdt = DBHelper.GetDataSet(RZsql);
            djcgsl = rzdt.Rows.Count;
            RZsql = "select SUM(Quota) from RZFlow a left join RZDemandInfo b on a.DemandId = b.Id where a.Status = 3";
            rzdt = DBHelper.GetDataSet(RZsql);
            dr = rzdt.Rows[0];
            djcgje = double.Parse(dr[0].ToString() == "" ? "0.0" : dr[0].ToString());

            var ZZDsql = "select * from ZZDFlow";
            zzddt = DBHelper.GetDataSet(ZZDsql);
            fbzzdsl = zzddt.Rows.Count;
            ZZDsql = "select SUM(ThisQuota) from ZZDFlow";
            zzddt = DBHelper.GetDataSet(ZZDsql);
            dr = zzddt.Rows[0];
            fbzzdje = double.Parse(dr[0].ToString() == "" ? "0.0" : dr[0].ToString());

            ZZDsql = "select * from ZZDFlow where Status in (1,2)";
            zzddt = DBHelper.GetDataSet(ZZDsql);
            djzzdsl = zzddt.Rows.Count;
            ZZDsql = "select SUM(ThisQuota) from ZZDFlow where Status in (1,2)";
            zzddt = DBHelper.GetDataSet(ZZDsql);
            dr = zzddt.Rows[0];
            djzzdje = double.Parse(dr[0].ToString() == "" ? "0.0" : dr[0].ToString());

            ZZDsql = "select * from ZZDFlow where Status = 3";
            zzddt = DBHelper.GetDataSet(ZZDsql);
            djzzdcgsl = zzddt.Rows.Count;
            ZZDsql = "select SUM(ThisQuota) from ZZDFlow where Status = 3";
            zzddt = DBHelper.GetDataSet(ZZDsql);
            dr = zzddt.Rows[0];
            djzzdcgje = double.Parse(dr[0].ToString() == "" ? "0.0" : dr[0].ToString());

            var jsonData = new IndexTJInfo
            {
                fbrzsl = fbrzsl.ToString(),
                fbrzje = fbrzje.ToString("C"),
                djrzsl = djrzsl.ToString(),
                djrzje = djrzje.ToString("C"),
                djcgsl = djcgsl.ToString(),
                djcgje = djcgje.ToString("C"),
                fbzzdsl = fbzzdsl.ToString(),
                fbzzdje = fbzzdje.ToString("C"),
                djzzdsl = djzzdsl.ToString(),
                djzzdje = djzzdje.ToString("C"),
                djzzdcgsl = djzzdcgsl.ToString(),
                djzzdcgje = djzzdcgje.ToString("C")
            };

            return JsonHelper.SerializeObject(jsonData);
        }


    }
}
