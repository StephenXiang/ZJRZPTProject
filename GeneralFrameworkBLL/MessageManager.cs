using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;

namespace GeneralFrameworkBLL
{
    public class MessageManager
    {
        readonly MessageService _ms = new MessageService();

        public bool Reply(MessageInfo mi)
        {
            return _ms.Reply(mi);
        }

        public string LoadMessages(string username, int page, int rows)
        {
            return _ms.GetMessages(page, rows, 0);
        }

        public string LoadHandledMessages(string username, int page, int rows)
        {
            return _ms.GetMessages(page, rows, 1);
        }

        public bool AddMsg(MessageInfo mi)
        {
            return _ms.AddMessage(mi);
        }

        public bool AddMsg(string title, string content, string username, string userphone)
        {
            return _ms.AddMessage(new MessageInfo
            {
                Title = title,
                Content = content,
                UserName = username,
                UserPhone = userphone
            });
        }

        public string GetUserMessages(int page, int rows, string username)
        {
            return _ms.GetUserMessages(page, rows, username);
        }

        public bool LeaveMesage(string username, string title, string content)
        {
            return _ms.LeaveMesage(username, title, content);
        }

        public bool Delete(int id)
        {
            return _ms.DelMessage(id);
        }
    }
}
