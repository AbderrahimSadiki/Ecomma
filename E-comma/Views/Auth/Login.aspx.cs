using System;
using System.Web;
using System.Web.Security;
using E_comma.Models;
using E_comma.Helpers;

namespace E_comma.Views.Auth
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                RedirectHelper.SafeRedirect("~/Views/Public/Home.aspx"); // Or default page
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (E_comma.Models.User.CheckLogin(email, password))
            {
                // Get user details
                var user = E_comma.Models.User.GetByEmail(email);
                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["UserEmail"] = user.Email;
                    Session["UserName"] = user.Name;
                    Session["UserRole"] = user.Role; // Stocker le rôle en session
                }

                FormsAuthentication.SetAuthCookie(email, false);


                
                // Redirect to original requested page or default
                string returnUrl = Request.QueryString["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    RedirectHelper.SafeRedirect(returnUrl);
                }
                else
                {
                    RedirectHelper.SafeRedirect("~/Views/Public/Home.aspx"); // Adjust as needed
                }
            }
            else
            {
                lblMessage.Text = "Email ou mot de passe incorrect.";
                lblMessage.Visible = true;
            }
        }
    }
}
