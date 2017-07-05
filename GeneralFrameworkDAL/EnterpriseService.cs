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

        public bool EditEnterprise(Enterprise ent)
        {
            string sql = "";
            int? id;
            if (ent.BusinessLicense.Length > 0)
            {
                sql = @"update Enterprise set Name=@name,BusinessLicense=@BusinessLicense,Code=@Code,RegistTypeId= @RegistTypeId,ProfessionId=@ProfessionId,
                        EnterpriseTypeId=@EnterpriseTypeId,RegistRegionId=@RegistRegionId,HuanpingId=@HuanpingId,RegFinance=@RegFinance,RegFinanceMt=@RegFinanceMt,Business=@Business,
                        MainProduction=@MainProduction,CreateTime=@CreateTime,JuridicalPerson=@JuridicalPerson,ConectionPerson=@ConectionPerson,ConnectionTelephone=@ConnectionTelephone,
                        [Desc] =@Desc where ID = @ID";

                id = DBHelper.Execute(sql,
                       new SqlParameter("@name", ent.Name),
                       new SqlParameter("@BusinessLicense", ent.BusinessLicense),
                       new SqlParameter("@Code", ent.Code),
                       new SqlParameter("@RegistTypeId", ent.RegistTypeId),
                       new SqlParameter("@ProfessionId", ent.ProfessionId),
                       new SqlParameter("@EnterpriseTypeId", ent.EnterpriseTypeId),
                       new SqlParameter("@RegistRegionId", ent.RegistRegionId),
                       new SqlParameter("@HuanpingId", ent.HuanpingId),
                       new SqlParameter("@RegFinance", ent.RegFinance),
                       new SqlParameter("@RegFinanceMt", ent.RegFinanceMt),
                       new SqlParameter("@Business", ent.Business),
                       new SqlParameter("@MainProduction", ent.MainProduction),
                       new SqlParameter("@CreateTime", ent.CreateTime),
                       new SqlParameter("@JuridicalPerson", ent.JuridicalPerson),
                       new SqlParameter("@ConectionPerson", ent.ConectionPerson),
                       new SqlParameter("@ConnectionTelephone", ent.ConnectionTelephone),
                       new SqlParameter("@Desc", ent.Desc),
                       new SqlParameter("@ID", ent.ID));

            }
            else
            {
                sql = @"update Enterprise set Name=@name,Code=@Code,RegistTypeId= @RegistTypeId,ProfessionId=@ProfessionId,
EnterpriseTypeId=@EnterpriseTypeId,RegistRegionId=@RegistRegionId,HuanpingId=@HuanpingId,RegFinance=@RegFinance,RegFinanceMt=@RegFinanceMt,Business=@Business,
MainProduction=@MainProduction,CreateTime=@CreateTime,JuridicalPerson=@JuridicalPerson,ConectionPerson=@ConectionPerson,ConnectionTelephone=@ConnectionTelephone,
[Desc] =@Desc where ID = @ID";
                id = DBHelper.Execute(sql,
                       new SqlParameter("@name", ent.Name),
                       new SqlParameter("@Code", ent.Code),
                       new SqlParameter("@RegistTypeId", ent.RegistTypeId),
                       new SqlParameter("@ProfessionId", ent.ProfessionId),
                       new SqlParameter("@EnterpriseTypeId", ent.EnterpriseTypeId),
                       new SqlParameter("@RegistRegionId", ent.RegistRegionId),
                       new SqlParameter("@HuanpingId", ent.HuanpingId),
                       new SqlParameter("@RegFinance", ent.RegFinance),
                       new SqlParameter("@RegFinanceMt", ent.RegFinanceMt),
                       new SqlParameter("@Business", ent.Business),
                       new SqlParameter("@MainProduction", ent.MainProduction),
                       new SqlParameter("@CreateTime", ent.CreateTime),
                       new SqlParameter("@JuridicalPerson", ent.JuridicalPerson),
                       new SqlParameter("@ConectionPerson", ent.ConectionPerson),
                       new SqlParameter("@ConnectionTelephone", ent.ConnectionTelephone),
                       new SqlParameter("@Desc", ent.Desc),
                       new SqlParameter("@ID", ent.ID));
            }
            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            return false;
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

        public string GetEnterpriseDg(int page, int rows)
        {
            var sql = @"select a.ID,a.Name,a.Code,c.[Desc] as RegistType,d.[Desc] as Profession,e.[Desc] as EnterpriseType,
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
                            left join (select Id,[Desc],[Type] from Lookup where [Type] = 7) j on a.Business = j.Id";
            DataTable dt = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
            return reply;
        }

        public string GetEnterpriseInfoForUserName(string UserName, int? enterpriseId = null)
        {
            string reply = null;
            string sql = @"select a.Id,a.Name,a.BusinessLicense,a.Code,a.RegistTypeId,c.[Desc] as RegistType, a.ProfessionId,d.[Desc] as Profession,a.EnterpriseTypeId,e.[Desc] as EnterpriseType,
                            a.RegistRegionId,f.[Desc] as RegistRegion,a.HuanpingId,g.[Desc] as Huanping,a.RegFinance as RegFinanceId,h.[Desc] as RegFinance,a.RegFinanceMt as RegFinanceMtId,i.[Desc] as RegFinanceMt,j.[Desc] as Business,
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
                            where " + (enterpriseId == null ? ("b.UserName = '" + UserName + "'") : ("a.ID=" + enterpriseId));
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                reply = JsonHelper.SerializeObject(new
                {
                    DataCount = dt.Rows.Count.ToString(),
                    Id = dr["Id"].ToString(),
                    Name = dr["Name"].ToString(),
                    BusinessLicense = dr["BusinessLicense"].ToString(),
                    Code = dr["Code"].ToString(),
                    RegistTypeId = dr["RegistTypeId"].ToString(),
                    RegistType = dr["RegistType"].ToString(),
                    ProfessionId = dr["ProfessionId"].ToString(),
                    Profession = dr["Profession"].ToString(),
                    EnterpriseTypeId = dr["EnterpriseTypeId"].ToString(),
                    EnterpriseType = dr["EnterpriseType"].ToString(),
                    RegistRegionId = dr["RegistRegionId"].ToString(),
                    RegistRegion = dr["RegistRegion"].ToString(),
                    HuanpingId = dr["HuanpingId"].ToString(),
                    Huanping = dr["Huanping"].ToString(),
                    RegFinanceId = dr["RegFinanceId"].ToString(),
                    RegFinance = dr["RegFinance"].ToString(),
                    RegFinanceMtId = dr["RegFinanceMtId"].ToString(),
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

        public Enterprise GetEnterpriseInfoForUserNameTwo(string UserName)
        {
            string reply = null;
            string sql = @"select b.ID,b.Name,b.ConectionPerson,b.ConnectionTelephone from SysUser a right join Enterprise b on a.EnterpriseId = b.ID where a.UserName = '" + UserName + "'";
            DataTable dt = DBHelper.GetDataSet(sql);
            Enterprise ent = new Enterprise();
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                ent.ID = int.Parse(dr["ID"].ToString());
                ent.Name = dr["Name"].ToString();
                ent.ConectionPerson = dr["ConectionPerson"].ToString();
                ent.ConnectionTelephone = dr["ConnectionTelephone"].ToString();
            }
            return ent;

        }

        public byte[] GetImgForCode(string Code)
        {
            var sql = "select BusinessLicense from Enterprise where code='" + Code + "' ";
            return (byte[])DBHelper.GetScalar(sql);
        }

        public string GetEnterpriseFinanceInfoByUserName(string userName, int? enterpriseId = null)
        {
            string reply = null;
            //            var sql = string.Format(@"select
            //ty.Total as thiszcze,ty.Liabilities as thisfzze,ty.SaleIncome as thisxssr,ty.Receivable as thisyszk,ty.RetainedProfits as thissyzqy,ty.[Year] as thisyear,ty.Jinlirun as thisjlr,
            //ly.Total as lzcze,ly.Liabilities as lfzze,ly.SaleIncome as lxssr,ly.Receivable as lyszk,ly.RetainedProfits as lsyzqy,ly.[Year] as lyear,ly.Jinlirun as ljlr,
            //lly.Total as llzcze,lly.Liabilities as llfzze,lly.SaleIncome as llxssr,lly.Receivable as llyszk,lly.RetainedProfits as llsyzqy,lly.[Year] as llyear,lly.Jinlirun as lljlr
            //from RZFinance rzf
            //join SysUser u on u.EnterpriseId=rzf.EnterpriseId
            //left join RZFinanceYear ty on ty.Id=rzf.Finance
            //left join RZFinanceYear ly on ly.Id=rzf.FinanceLY
            //left join RZFinanceYear lly on lly.Id=rzf.FinanceLLY
            //where {0}", enterpriseId == null ? string.Format("u.UserName='{0}'", userName.Trim()) : string.Format("rzf.EnterpriseId={0}", enterpriseId));

            var sql = string.Format(@"select a.Total as zcze,a.Liabilities as fzze,a.SaleIncome as xssr,a.Receivable as yszk,a.RetainedProfits as syzqy,
a.Jinlirun as jlr,a.[Year] as [year]
 from RZFinanceYear a left join SysUser u on a.EnterpriseId = u.EnterpriseId 
where {0}", enterpriseId == null ? string.Format("u.UserName='{0}' order  by a.[Year] desc", userName.Trim()) : string.Format("a.EnterpriseId={0} order  by a.[Year] desc", enterpriseId));
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                var efi = new EnterpriseFinanceInfo();
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (i == 0)
                    {
                        efi.thisyear = dr["year"].ToString();
                        efi.thiszcze = dr["zcze"].ToString();
                        efi.thisfzze = dr["fzze"].ToString();
                        efi.thisxssr = dr["xssr"].ToString();
                        efi.thisyszk = dr["yszk"].ToString();
                        efi.thissyzqy = dr["syzqy"].ToString();
                        efi.thisjlr = dr["jlr"].ToString();
                    }
                    else if (i == 1)
                    {
                        efi.lyear = dr["year"].ToString();
                        efi.lzcze = dr["zcze"].ToString();
                        efi.lfzze = dr["fzze"].ToString();
                        efi.lxssr = dr["xssr"].ToString();
                        efi.lyszk = dr["yszk"].ToString();
                        efi.lsyzqy = dr["syzqy"].ToString();
                        efi.ljlr = dr["jlr"].ToString();
                    }
                    else
                    {
                        efi.llyear = dr["year"].ToString();
                        efi.llzcze = dr["zcze"].ToString();
                        efi.llfzze = dr["fzze"].ToString();
                        efi.llxssr = dr["xssr"].ToString();
                        efi.llyszk = dr["yszk"].ToString();
                        efi.llsyzqy = dr["syzqy"].ToString();
                        efi.lljlr = dr["jlr"].ToString();
                    }
                }
                //var dr = dt.Rows[0];
                //var efi = new EnterpriseFinanceInfo
                //{
                //    llzcze = dr["llzcze"].ToString(),
                //    lzcze = dr["lzcze"].ToString(),
                //    thiszcze = dr["thiszcze"].ToString(),
                //    llfzze = dr["llfzze"].ToString(),
                //    lfzze = dr["lfzze"].ToString(),
                //    thisfzze = dr["thisfzze"].ToString(),
                //    llxssr = dr["llxssr"].ToString(),
                //    lxssr = dr["lxssr"].ToString(),
                //    thisxssr = dr["thisxssr"].ToString(),
                //    llyszk = dr["llyszk"].ToString(),
                //    lyszk = dr["lyszk"].ToString(),
                //    thisyszk = dr["thisyszk"].ToString(),
                //    lljlr = dr["lljlr"].ToString(),
                //    ljlr = dr["ljlr"].ToString(),
                //    thisjlr = dr["thisjlr"].ToString(),
                //    llsyzqy = dr["llsyzqy"].ToString(),
                //    lsyzqy = dr["lsyzqy"].ToString(),
                //    thissyzqy = dr["thissyzqy"].ToString(),
                //    llyear = dr["llyear"].ToString(),
                //    lyear = dr["lyear"].ToString(),
                //    thisyear = dr["thisyear"].ToString()
                //};
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

        public string GetEnterpriseInfoList()
        {
            var sql = @"select ID as Id,NewsTitle as Name from NewsInFo where NewsType = 'qy' and IsDeleted != 1";
            var dt1 = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.SerializeObject(dt1);
            return reply;
        }

        public string GetEnterpriseInfoById(string id)
        {
            var sql = @"select  a.Name,a.MainProduction,c.[Desc] as zczj,b.[Desc] as yyfw,a.CreateTime,a.ConectionPerson,a.ConnectionTelephone,a.[Desc] from Enterprise a 
left join (select * from Lookup where Name='营业范围' ) b on a.Business = b.Id
left join (select * from Lookup where Name='注册资金额度' ) c on a.RegFinance = c.Id where a.Id ='" + id + "'";
            var dt1 = DBHelper.GetDataSet(sql);
            var reply = JSON.JsonHelper.SerializeObject(dt1);
            return reply;
        }
    }
}
