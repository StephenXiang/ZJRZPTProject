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
    public class ZZDExamineApproveService
    {
        public string GetZZDDataTable(string UserName, int page, int rows)
        {
            var sql = @"select BankId,RolesID from SysUser where UserName = '" + UserName + "'";
            int BankId = 0;
            int RoleId = 0;
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                BankId = dr["BankId"] == DBNull.Value ? 0 : int.Parse(dr["BankId"].ToString());
                RoleId = dr["RolesID"] == DBNull.Value ? 0 : int.Parse(dr["RolesID"].ToString());
            }
            else
            {
                return "";
            }
            if (BankId != 0)
            {
                sql = @"select a.Id,b.Id as EnterpriseId,b.Name as EnterpriseName,a.OriginalQuota,c.Name as BankName,a.ThisQuota,CONVERT(varchar(100), a.PublishDate, 23) as PublishDate,a.[Status],a.[Status] as cz,a.Feedback from ZZDFlow a 
left join Enterprise b on a.EnterpriseId = b.ID
left join Bank c on a.BankId = c.Id
where  a.BankId = '" + BankId + "' and a.IsDeleted=0 order by a.Id Desc";
                var dt1 = DBHelper.GetDataSet(sql);
                return JsonHelper.TableToJson(dt1.Rows.Count, JsonHelper.GetPagedTable(dt1, page, rows));
            }
            if (RoleId == 1 || RoleId == 2)    // 系统用户或政府部门
            {
                sql = @"select a.Id,b.Id as EnterpriseId,b.Name as EnterpriseName,a.OriginalQuota,c1.Name as Bank1,c2.Name as BankName,a.ThisQuota,CONVERT(varchar(100), a.PublishDate, 23) as PublishDate,a.[Status],a.[Status] as cz,a.Feedback from ZZDFlow a 
left join Enterprise b on a.EnterpriseId = b.ID
left join Bank c1 on a.MastBankId = c1.Id
left join Bank c2 on a.BankId = c2.Id where a.IsDeleted=0 order by a.Id Desc";
                var dt1 = DBHelper.GetDataSet(sql);
                return JsonHelper.TableToJson(dt1.Rows.Count, JsonHelper.GetPagedTable(dt1, page, rows));
            }
            return "";
        }

        public string GetBankInfoForUserName(string UserName)
        {
            var sql = @"select BankId,RolesID from SysUser where UserName = '" + UserName + "'";
            int BankId = 0;
            int RoleId = 0;
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                BankId = dr["BankId"] == DBNull.Value ? 0 : int.Parse(dr["BankId"].ToString());
                RoleId = dr["RolesID"] == DBNull.Value ? 0 : int.Parse(dr["RolesID"].ToString());
            }
            else
            {
                return "";
            }
            sql = @"select a.Id, Name,[Address],Connector,ConnectorPhone,ParentBankId,c.BankName,a.[Desc],a.EditDate,c.Leader,C.Phone,iszzd,BankType from Bank a 
left join CooperativeBank c on a.ParentBankId = c.id where a.Id = " + BankId + "";
            dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.SerializeObject(dt);
            return reply;
        }

        public string GetMastBankZZDDT(string UserName, int page, int rows)
        {
            var sql = @"select BankId,RolesID from SysUser where UserName = '" + UserName + "'";
            int BankId = 0;
            int RoleId = 0;
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                BankId = dr["BankId"] == DBNull.Value ? 0 : int.Parse(dr["BankId"].ToString());
                RoleId = dr["RolesID"] == DBNull.Value ? 0 : int.Parse(dr["RolesID"].ToString());
            }
            else
            {
                return "";
            }
            if (BankId != 0)
            {
                sql = @"select a.Id,b.Id as EnterpriseId,b.Name as EnterpriseName,a.OriginalQuota,c.Name as BankName,a.ThisQuota,CONVERT(varchar(100), a.PublishDate, 23) as PublishDate,a.[Status],a.[Status] as cz,a.Feedback from ZZDFlow a 
left join Enterprise b on a.EnterpriseId = b.ID
left join Bank c on a.BankId = c.Id
where  a.MastBankId = '" + BankId + "' and a.IsDeleted=0 order by a.Id Desc";
                var dt1 = DBHelper.GetDataSet(sql);
                return JsonHelper.TableToJson(dt1.Rows.Count, JsonHelper.GetPagedTable(dt1, page, rows));
            }
            else
            {
                return "";
            }
        }

        public bool EditZZDStatus(string status, string zzdid)
        {
            string sql = string.Empty;
            sql = @"update  ZZDFlow set [Status] = @status where Id = @zzdid";
            var sid = DBHelper.Execute(sql, new SqlParameter("@status", status), new SqlParameter("@zzdid", zzdid));
            if (sid > 0)
            {
                if (status == "3")
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
                    sql = string.Format(@"select b.ConnectionTelephone from ZZDFlow a left join Enterprise b on a.EnterpriseId = b.ID where a.Id = {0}", zzdid);
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
                        //sms.Send(phonelist[h].ToString(), "有一笔周转贷申请受理成功，请及时登陆镇江融资平台查看详细信息");
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetEnterpriseInfoForEnterpriseId(string enterpriseId)
        {
            string reply = null;
            string sql = @"select a.Name,a.BusinessLicense,a.Code,c.[Desc] as RegistType,d.[Desc] as Profession,e.[Desc] as EnterpriseType,
                            f.[Desc] as RegistRegion,g.[Desc] as Huanping,h.[Desc] as RegFinance,i.[Desc] as RegFinanceMt,j.[Desc] as Business,
                            a.MainProduction,CONVERT(varchar(100),a.CreateTime, 23) as CreateTime,a.JuridicalPerson,a.ConectionPerson,a.ConnectionTelephone,a.[Desc] from Enterprise a 
                            left join SysUser b on a.ID = b.EnterpriseId 
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 1) c on a.RegistTypeId = c.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 2) d on a.ProfessionId = d.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 3) e on a.EnterpriseTypeId = e.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 4) f on a.RegistRegionId = f.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 5) g on a.HuanpingId = g.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 12) h on a.RegFinance = h.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 6) i on a.RegFinanceMt = i.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 7) j on a.Business = j.Id
                            where a.Id = '" + enterpriseId + "'";
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                reply = JsonHelper.SerializeObject(new
                {
                    DataCount = dt.Rows.Count.ToString(),
                    Name = dr["Name"].ToString(),
                    BusinessLicense = dr["BusinessLicense"].ToString(),
                    Code = dr["Code"].ToString(),
                    RegistType = dr["RegistType"].ToString(),
                    Profession = dr["Profession"].ToString(),
                    EnterpriseType = dr["EnterpriseType"].ToString(),
                    RegistRegion = dr["RegistRegion"].ToString(),
                    Huanping = dr["Huanping"].ToString(),
                    RegFinance = dr["RegFinance"].ToString(),
                    RegFinanceMt = dr["RegFinanceMt"].ToString(),
                    Business = dr["Business"].ToString(),
                    MainProduction = dr["MainProduction"].ToString(),
                    CreateTime = dr["CreateTime"].ToString(),
                    JuridicalPerson = dr["JuridicalPerson"].ToString(),
                    ConectionPerson = dr["ConectionPerson"].ToString(),
                    ConnectionTelephone = dr["ConnectionTelephone"].ToString(),
                    Desc = dr["Desc"].ToString()
                });
            }
            else
            {
                reply = JsonHelper.SerializeObject(new
                {
                    DataCount = dt.Rows.Count.ToString()
                });
            }
            return reply;
        }

        public string GetEnterpriseFinanceInfoByEnterpriseId(string enterpriseId)
        {
            string reply = null;
            var sql = string.Format(@"select
ty.Total as thiszcze,ty.Liabilities as thisfzze,ty.SaleIncome as thisxssr,ty.Receivable as thisyszk,ty.RetainedProfits as thissyzqy,ty.[Year] as thisyear,ty.Jinlirun as thisjlr,
ly.Total as lzcze,ly.Liabilities as lfzze,ly.SaleIncome as lxssr,ly.Receivable as lyszk,ly.RetainedProfits as lsyzqy,ly.[Year] as lyear,ly.Jinlirun as ljlr,
lly.Total as llzcze,lly.Liabilities as llfzze,lly.SaleIncome as llxssr,lly.Receivable as llyszk,lly.RetainedProfits as llsyzqy,lly.[Year] as llyear,lly.Jinlirun as lljlr
from RZFinance rzf
join SysUser u on u.EnterpriseId=rzf.EnterpriseId
left join RZFinanceYear ty on ty.Id=rzf.Finance
left join RZFinanceYear ly on ly.Id=rzf.FinanceLY
left join RZFinanceYear lly on lly.Id=rzf.FinanceLLY
where u.EnterpriseId='{0}'", enterpriseId);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                var efi = new EnterpriseFinanceInfo
                {
                    llzcze = dr["llzcze"].ToString(),
                    lzcze = dr["lzcze"].ToString(),
                    thiszcze = dr["thiszcze"].ToString(),
                    llfzze = dr["llfzze"].ToString(),
                    lfzze = dr["lfzze"].ToString(),
                    thisfzze = dr["thisfzze"].ToString(),
                    llxssr = dr["llxssr"].ToString(),
                    lxssr = dr["lxssr"].ToString(),
                    thisxssr = dr["thisxssr"].ToString(),
                    llyszk = dr["llyszk"].ToString(),
                    lyszk = dr["lyszk"].ToString(),
                    thisyszk = dr["thisyszk"].ToString(),
                    lljlr = dr["lljlr"].ToString(),
                    ljlr = dr["ljlr"].ToString(),
                    thisjlr = dr["thisjlr"].ToString(),
                    llsyzqy = dr["llsyzqy"].ToString(),
                    lsyzqy = dr["lsyzqy"].ToString(),
                    thissyzqy = dr["thissyzqy"].ToString(),
                    llyear = dr["llyear"].ToString(),
                    lyear = dr["lyear"].ToString(),
                    thisyear = dr["thisyear"].ToString()
                };
                reply = JsonHelper.SerializeObject(efi);
            }
            else
            {
                reply = "";
            }
            return reply;
        }

        public string GetZZDInfoByZZDID(string zzdid)
        {
            string reply = null;
            var sql = string.Format(@"select b.Name,a.Manager,a.ManagerPhone,a.OriginalQuota,CONVERT(varchar(100), a.ExpirationDate, 23) as ExpirationDate,c.Name,a.ThisQuota from ZZDFlow a
left join Bank b on a.BankId = b.Id
left join Bank c on a.MastBankId = c.Id where a.Id = {0}", zzdid);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                var efi = new ZzdInfoReply
                {
                    ydkyh = dr[0].ToString(),
                    khjlxm = dr[1].ToString(),
                    khjllxdh = dr[2].ToString(),
                    ydkje = dr[3].ToString(),
                    dkdqsj = dr[4].ToString(),
                    zbh = dr[5].ToString(),
                    bcdkje = dr[6].ToString(),
                };
                reply = JsonHelper.SerializeObject(efi);
            }
            else
            {
                reply = "";
            }
            return reply;
        }

        public bool EditZZDStatus(string status, string zzdid, string liyou)
        {
            string sql = string.Empty;
            sql = @"update ZZDFlow set [Status] = @status,Feedback = @Feedback where Id = @zzdid";
            var sid = DBHelper.Execute(sql, new SqlParameter("@status", status), new SqlParameter("@zzdid", zzdid), new SqlParameter("@Feedback", liyou));
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
                sql = string.Format(@"select b.ConnectionTelephone from ZZDFlow a left join Enterprise b on a.EnterpriseId = b.ID where a.Id = {0}", zzdid);
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
                    sms.Send(phonelist[h].ToString(), "有一笔周转贷申请受理不成功，请及时登陆镇江融资平台查看详细信息");
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
