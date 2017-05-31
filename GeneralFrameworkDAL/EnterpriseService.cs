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
                            left join (select [Desc],[Type] from Lookup where [Type] = 1) c on a.RegistTypeId = c.Type
                            left join (select [Desc],[Type] from Lookup where [Type] = 2) d on a.ProfessionId = d.Type
                            left join (select [Desc],[Type] from Lookup where [Type] = 3) e on a.EnterpriseTypeId = e.Type
                            left join (select [Desc],[Type] from Lookup where [Type] = 4) f on a.RegistRegionId = f.Type
                            left join (select [Desc],[Type] from Lookup where [Type] = 5) g on a.HuanpingId = g.Type
                            left join (select [Desc],[Type] from Lookup where [Type] = 10) h on a.RegFinance = h.Type
                            left join (select [Desc],[Type] from Lookup where [Type] = 6) i on a.RegFinanceMt = i.Type
                            left join (select [Desc],[Type] from Lookup where [Type] = 7) j on a.Business = j.Type
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

    }
}
