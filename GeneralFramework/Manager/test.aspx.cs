using GeneralFrameworkBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeneralFramework.Manager
{
    public partial class test : System.Web.UI.Page
    {
        EnterpriseManager _em = new EnterpriseManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string code = Request.Params["Code"];

                var bytes = _em.GetImgForCode(code);
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }
    }
}