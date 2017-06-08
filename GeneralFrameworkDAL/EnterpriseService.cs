using System.Data;
using System.Data.SqlClient;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;

namespace GeneralFrameworkDAL
{
    public class EnterpriseService
    {
        public string LookUp(string key)
        {
            var sql = string.Format("select Id as ID,[Desc] as TypeName from [Lookup] where [Name]=N'{0}'", key);
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public string GetUserTypeCmb()
        {
            var sql = "select ID,TypeName from LookUp where TypeCls = 'UsersType'";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public bool Save(Enterprise ins, out string err)
        {
            err = "";
            if (DBHelper.GetDataSet("select ID from Enterprise where [Name]='{0}'").Rows.Count > 0)
            {
                err = "企业名称已经存在";
                return false;
            }
            var sql = @"insert into Enterprise([Name],BusinessLicense,Code,RegistTypeId,ProfessionId,EnterpriseTypeId,RegistRegionId,HuanpingId,RegFinance,RegFinanceMt
,Business,MainProduction,CreateTime,JuridicalPerson,ConectionPerson,ConnectionTelephone,[Desc],Outside)
values(@name,@license,@code,@rt,@pi,@et,@rri,@hpi,@regfin,@regfinmt,@business,@mp,@ct,@jp,@cp,@ctp,@desc,@os)";

            return DBHelper.Execute(sql,
                       new SqlParameter("@name", ins.Name),
                       new SqlParameter("@license", ins.BusinessLicense),
                       new SqlParameter("@code", ins.Code),
                       new SqlParameter("@rt", ins.RegistTypeId),
                       new SqlParameter("@pi", ins.ProfessionId),
                       new SqlParameter("@et", ins.EnterpriseTypeId),
                       new SqlParameter("@rri", ins.RegistRegionId),
                       new SqlParameter("@hpi", ins.HuanpingId),
                       new SqlParameter("@regfin", ins.RegFinance),
                       new SqlParameter("@regfinmt", ins.RegFinanceMt),
                       new SqlParameter("@business", ins.Business),
                       new SqlParameter("@mp", ins.MainProduction),
                       new SqlParameter("@ct", ins.CreateTime),
                       new SqlParameter("@jp", ins.JuridicalPerson),
                       new SqlParameter("@cp", ins.ConectionPerson),
                       new SqlParameter("@ctp", ins.ConnectionTelephone),
                       new SqlParameter("@desc", ins.Desc),
                       new SqlParameter("@os", false)) > 0;
        }


        public int GetEntetpriseIdForName(string EnterpriseName, string UserName)
        {
            string sql = @"select ID from Enterprise where Name = '" + EnterpriseName + "' ";
            DataTable dt = DBHelper.GetDataSet(sql);
            int EnterpriseId = 0;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                EnterpriseId = int.Parse(dr[0].ToString());
            }
            sql = "update SysUser set EnterpriseId = '" + EnterpriseId + "' where UserName = '" + UserName + "' ";
            return DBHelper.GetSingle(sql);
        }

        public string GetEnterpriseInfoForUserName(string UserName)
        {
            string reply = null;
            string sql = @"select a.Name,a.BusinessLicense,a.Code,c.[Desc] as RegistType,d.[Desc] as Profession,e.[Desc] as EnterpriseType,
                            f.[Desc] as RegistRegion,g.[Desc] as Huanping,h.[Desc] as RegFinance,i.[Desc] as RegFinanceMt,j.[Desc] as Business,
                            a.MainProduction,a.CreateTime,a.JuridicalPerson,a.ConectionPerson,a.ConnectionTelephone,a.[Desc] from Enterprise a 
                            left join SysUser b on a.ID = b.EnterpriseId 
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 1) c on a.RegistTypeId = c.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 2) d on a.ProfessionId = d.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 3) e on a.EnterpriseTypeId = e.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 4) f on a.RegistRegionId = f.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 5) g on a.HuanpingId = g.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 12) h on a.RegFinance = h.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 6) i on a.RegFinanceMt = i.Id
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 7) j on a.Business = j.Id
                            where b.UserName = '" + UserName + "'";
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

        public byte[] GetImgForCode(string Code)
        {
            var sql = "select BusinessLicense from Enterprise where code='" + Code + "' ";
            return (byte[])DBHelper.GetScalar(sql);
        }

        public string GetEnterpriseFinanceInfoByUserName(string userName)
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
where u.UserName='{0}'", userName.Trim());
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

        public bool SaveEnterpriseFianceInfo(EnterpriseFinanceInfo efi)
        {
            var sql = string.Format(@"select EnterpriseId from SysUser where UserName='{0}'", efi.username.Trim());
            var ent = DBHelper.GetScalar(sql) as int?;
            if (ent == null) return false;
            sql = @"
insert into RZFinanceYear(EnterpriseId,Total,Liabilities,SaleIncome,Receivable,RetainedProfits,[Year],Jinlirun)
values (@eid,@tt,@lia,@sal,@rec,@ret,@yr,@jlr)
SELECT @@IDENTITY";
            var thisid = DBHelper.GetScalar(sql,
                new SqlParameter("@eid", ent),
                new SqlParameter("@tt", efi.thiszcze.Trim()),
                new SqlParameter("@lia", efi.thisfzze.Trim()),
                new SqlParameter("@sal", efi.thisxssr.Trim()),
                new SqlParameter("@rec", efi.thisyszk.Trim()),
                new SqlParameter("@ret", efi.thissyzqy.Trim()),
                new SqlParameter("@yr", efi.thisyear.Trim()),
                new SqlParameter("@jlr", efi.thisjlr.Trim()));
            if (thisid == null) return false;
            var lid = DBHelper.GetScalar(sql,
                new SqlParameter("@eid", ent),
                new SqlParameter("@tt", efi.lzcze.Trim()),
                new SqlParameter("@lia", efi.lfzze.Trim()),
                new SqlParameter("@sal", efi.lxssr.Trim()),
                new SqlParameter("@rec", efi.lyszk.Trim()),
                new SqlParameter("@ret", efi.lsyzqy.Trim()),
                new SqlParameter("@yr", efi.lyear.Trim()),
                new SqlParameter("@jlr", efi.ljlr.Trim()));
            if (lid == null) return false;
            var llid = DBHelper.GetScalar(sql,
                new SqlParameter("@eid", ent),
                new SqlParameter("@tt", efi.llzcze.Trim()),
                new SqlParameter("@lia", efi.llfzze.Trim()),
                new SqlParameter("@sal", efi.llxssr.Trim()),
                new SqlParameter("@rec", efi.llyszk.Trim()),
                new SqlParameter("@ret", efi.llsyzqy.Trim()),
                new SqlParameter("@yr", efi.llyear.Trim()),
                new SqlParameter("@jlr", efi.lljlr.Trim()));
            if (llid == null) return false;
            sql = @"insert into RZFinance(EnterpriseId,FinanceYear,Finance,FinanceLY,FinanceLLY)
values(@eid,@fy,@fn,@fnl,@fnll)";
            var id = DBHelper.Execute(sql, new SqlParameter("@eid", ent),
                new SqlParameter("@fy", efi.thisyear.Trim()),
                new SqlParameter("@fn", thisid),
                new SqlParameter("@fnl", lid),
                new SqlParameter("@fnll", llid));
            return id > 0;
        }
    }
}
