using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using UserModel = E_comma.Models.User;
using E_comma.Helpers;

namespace E_comma.Views.User
{
    public partial class UserProfile : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    RedirectHelper.SafeRedirect("/Views/Auth/Login.aspx");
                    return;
                }

                Guid userId = (Guid)Session["UserId"];
                var user = UserModel.GetById(userId);
                if (user == null) return;

                txtFirstName.Text = user.Name;
                txtLastName.Text = user.LastName;
                txtPhone.Text = user.Phone;
                txtEmail.Text = user.Email;
            }
            catch (Exception ex)
            {
                ShowStatus("Erreur lors du chargement du profil : " + ex.Message, false);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                RedirectHelper.SafeRedirect("/Views/Auth/Login.aspx");
                return;
            }

            Guid userId = (Guid)Session["UserId"];
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text.Trim();
            string passwordConfirm = txtPasswordConfirm.Text.Trim();

            if (!string.IsNullOrWhiteSpace(password) && password != passwordConfirm)
            {
                ShowStatus("Les mots de passe ne correspondent pas.", false);
                return;
            }

            try
            {
                var user = UserModel.GetById(userId);
                if (user == null)
                {
                    ShowStatus("Utilisateur introuvable.", false);
                    return;
                }

                string error;
                bool updated = UserModel.Update(userId, user.Email, phone, firstName, lastName, user.IsActive, password, out error);

                ShowStatus(updated ? "Profil mis à jour." : (string.IsNullOrEmpty(error) ? "Mise à jour impossible." : error), updated);
                if (updated)
                {
                    txtPassword.Text = string.Empty;
                    txtPasswordConfirm.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Erreur : " + ex.Message, false);
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            LoadProfile();
            txtPassword.Text = string.Empty;
            txtPasswordConfirm.Text = string.Empty;
            lblStatus.Visible = false;
        }

        private void ShowStatus(string message, bool success)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = success ? "alert alert-success mb-0" : "alert alert-danger mb-0";
            lblStatus.Visible = true;
        }
    }
}
