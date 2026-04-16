using System;
using System.Web.Security;
using E_comma.Helpers;

namespace E_comma.Views.Auth
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            RedirectHelper.SafeRedirect("~/Views/Auth/Login.aspx");
        }
    }
}
