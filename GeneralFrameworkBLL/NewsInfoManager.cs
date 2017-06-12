using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;

namespace GeneralFrameworkBLL
{
    public class NewsInfoManager
    {
        private readonly NewsInfoService _ns = new NewsInfoService();

        public string GetNewsDg(string type, int page, int rows)
        {
            return _ns.GetNewsDg(type, page, rows);
        }

        public int AddNews(NewsInfo news)
        {
            return _ns.AddNews(news);
        }

        public int EditNews(NewsInfo news)
        {
            return _ns.EditNews(news);
        }

        public int DelNews(int newsId)
        {
            return _ns.DelNews(newsId);
        }

        public NewsInfo GetNewContent(int newsId)
        {
            return _ns.GetNewsContent(newsId);
        }
        public string GetIndexNewsInfo(string newstype)
        {
            return _ns.GetIndexNewsInfo(newstype);
        }
    }
}
