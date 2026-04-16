using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace E_comma
{
    public class Global : System.Web.HttpApplication
    {


protected void Application_Start(object sender, EventArgs e)
    {
        ScriptManager.ScriptResourceMapping.AddDefinition(
            "jquery",
            new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-3.6.0.min.js",
                DebugPath = "~/Scripts/jquery-3.6.0.js",
                CdnPath = "https://code.jquery.com/jquery-3.6.0.min.js",
                CdnDebugPath = "https://code.jquery.com/jquery-3.6.0.js",
                CdnSupportsSecureConnection = true,
                LoadSuccessExpression = "window.jQuery"
            }
        );
    }


    protected void Session_Start(object sender, EventArgs e)
        {
            // Force logout if user is authenticated but session is new/empty (e.g. browser restart)
            // This ensures strict session expiry on browser close.
            if (User != null && User.Identity.IsAuthenticated && Session["UserId"] == null)
            {
                FormsAuthentication.SignOut();
            }
        }
    }
}