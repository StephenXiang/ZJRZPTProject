using System;
using System.Data;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;

namespace GeneralFrameworkDAL
{
    public class NewsInfoService
    {
        public string GetNewsDg(string type, int page, int rows)
        {
            var sqlCount = "select * from NewsInFo";
            var countDt = DBHelper.GetDataSet(sqlCount);
            var sql = string.Format(@"
select top {0} ID,NewsTitle,NewsLink,NewsType,Relation_Firm,Createdate,CreateUser from (
select ROW_NUMBER() over (order by Createdate desc) as rowId,* from NewsInFo where NewsType='{2}' and IsDelete=0) as r where r.rowId>{1}
", rows, (page - 1) * rows, type);
            var dt = DBHelper.GetDataSet(sql);
            var json = JsonHelper.TableToJson(countDt.Rows.Count, dt);
            return json;
        }

        public int AddNews(NewsInfo news)
        {
            var sql = "insert into NewsInFo(NewsTitle,NewsContent,NewsLink,NewsType,Relation_Firm,CreateUser)values('" +
                      news.NewsTitle + "','" + news.NewsContent + "','" + news.NewsLink + "','" + news.NewsType + "','" + news.Relation_Firm + "','" + news.CreateUser + "')";
            var i = DBHelper.GetNonQuery(sql);
            return i;
        }

        public int EditNews(NewsInfo news)
        {
            var sql = "update NewsInFo set NewsTitle = '" + news.NewsTitle + "',NewsLink = '" +
                      news.NewsLink + "',NewsType = '" + news.NewsType + "',NewsContent = '" + news.NewsContent + "',Relation_Firm='" + news.Relation_Firm + "' where ID = '" + news.NewsID + "'";
            var i = DBHelper.GetNonQuery(sql);
            return i;
        }

        public int DelNews(int newsId)
        {
            var sql = "update NewsInFo set IsDelete=1 , DeleteAt='" + DateTime.Now + "' where ID = '" + newsId + "'";
            var i = DBHelper.GetNonQuery(sql);
            return i;
        }

        public NewsInfo GetNewsContent(int newsId)
        {
            var sql = "select NewsTitle,NewsContent,Createdate,CreateUser" +
                      " from NewsInFo where ID = '" + newsId + "'";
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count == 0) return null;
            var dr = dt.Rows[0];
            var newsContent = new NewsInfo
            {
                CreateUser = dr["CreateUser"].ToString(),
                Createdate = dr.Field<DateTime>("Createdate"),
                NewsTitle = dr["NewsTitle"].ToString(),
                NewsContent = dr["NewsContent"].ToString(),
            };
            return newsContent;
        }
    }
}
