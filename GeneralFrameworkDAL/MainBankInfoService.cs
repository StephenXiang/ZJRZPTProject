using GeneralFrameworkDAL.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkDAL
{
    public class MainBankInfoService
    {
        public string GetMainBankCmb()
        {
            var sql = string.Format(" select Id as ID,BankName as TypeName from CooperativeBank where IsDesplay = 0 order by Sort asc ");
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }
    }
}
