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
        public int EditTpNews(NewsInfo news)
        {
            return _ns.EditTpNews(news);
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

        public string GetZCNewsList(string newstype, int PageIndex = 1, int PageSize = 7)
        {
            return _ns.GetZCNewsList(newstype, PageIndex, PageSize);
        }

        public string GetZCNewsDetail(int Id)
        {
            return _ns.GetZCNewsDetail(Id);
        }

        public byte[] GetNewsImage(int Newsid)
        {
            return _ns.GetNewsImage(Newsid);
        }

        public string GetDefaultNewsImage()
        {
            return _ns.GetDefaultNewsImage();
        }
    }
}
