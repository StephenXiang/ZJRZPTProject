using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GeneralFrameworkDAL
{
    public class SearchService
    {
        public string GetSearchJson(string str)
        {
            DataTable dt = new DataTable();
            if (str.Trim() != "")
            {
                string sql = string.Format(@"select Id,title,[Type] as datatyle,Createdate from 
(select ID as Id,NewsTitle as title,NewsType as Type,CONVERT(varchar(100), Createdate, 23) as Createdate  from NewsInFo where IsDeleted = 0 and NewsTitle like '%{0}%')a
union all 
(select Id,Title  as title,'jrcp' as Type,CONVERT(varchar(100), PublishDate, 23) as Createdate from JRCPFlow where IsDeleted = 0 and Status = 1 and Title like '%{1}%') 
union all
(select Id,BankName  as title,'bank' as Type,CONVERT(varchar(100), Createdate, 23) as Createdate from CooperativeBank where 1=1 and BankName like '%{2}%' )
order by Createdate desc", str, str, str);
                dt = DBHelper.GetDataSet(sql);
            }
            var reply = JSON.JsonHelper.SerializeObject(dt);
            return reply;
        }
    }
}
