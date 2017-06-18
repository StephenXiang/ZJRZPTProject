using System;
using System.Data;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;
using System.Collections.Generic;

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
select ROW_NUMBER() over (order by Createdate desc) as rowId,* from NewsInFo where NewsType='{2}' and IsDeleted=0) as r where r.rowId>{1}
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
            var sql = "update NewsInFo set IsDeleted=1 , DeleteAt='" + DateTime.Now + "' where ID = '" + newsId + "'";
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

        public string GetIndexNewsInfo(string NewsType)
        {
            int count = 0;
            if (NewsType == "zc")
            {
                count = 6;
            }
            else
            {
                count = 8;
            }
            List<IndexNewsInfo> list = new List<IndexNewsInfo>();
            var sql = "select top(" + count + ") ID,NewsTitle, CONVERT(varchar(5), Createdate, 110) as createdate from NewsInFo where NewsType='" + NewsType + "' and IsDeleted=0 order by ID desc";
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    IndexNewsInfo ini = new IndexNewsInfo()
                    {
                        id = int.Parse(dr["ID"].ToString()),
                        Title = dr["NewsTitle"].ToString(),
                        date = dr["createdate"].ToString()
                    };
                    list.Add(ini);
                }
            }
            return JsonHelper.SerializeObject(list);
        }

        public string GetZCNewsList(string newstype, int PageIndex, int PageSize)
        {
            var sql = "select Id,NewsTitle,NewsContent,Createdate from NewsInFo where NewsType ='" + newstype + "' and IsDeleted=0 order by Createdate desc";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.SerializeObject(dt);
        }

        public string GetZCNewsDetail(int Id)
        {
            var sql = "select Id,NewsTitle,NewsContent,Createdate from NewsInFo where ID ='" + Id + "'";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.SerializeObject(dt);
        }
    }
}
