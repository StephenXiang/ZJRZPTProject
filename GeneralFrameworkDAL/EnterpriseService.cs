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
values(@name,@license,@code,@rt,@pi,@et,@rri,@hpi,@regfin,@regfinmt,@business,@mp,@ct,@jp,@cp,@ct,@desc,@os)";

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
                       new SqlParameter("@ct", ins.ConnectionTelephone),
                       new SqlParameter("@desc", ins.Desc),
                       new SqlParameter("@os", false)) > 0;
        }
    }
}
