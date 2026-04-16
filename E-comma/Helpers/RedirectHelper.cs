using System.Web;

namespace E_comma.Helpers
{
    public static class RedirectHelper
    {
        public static void SafeRedirect(string url)
        {
            HttpContext context = HttpContext.Current;
            if (context == null) return;

            context.Response.Redirect(url, false);
            context.ApplicationInstance.CompleteRequest();
        }
    }
}
