using System;
using System.Net.Mail;
using E_comma.Models;

namespace E_comma.Views.Auth
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            E_comma.Models.User user = E_comma.Models.User.GetByEmail(email);

            if (user != null)
            {
                Guid token = Guid.NewGuid();
                // Expiry set to 1 hour from now
                PasswordResetToken.Create(user.Id, token, DateTime.Now.AddHours(1));

                string resetLink = Request.Url.GetLeftPart(UriPartial.Authority) + "/Views/Auth/ResetPassword.aspx?token=" + token.ToString();
                
                try
                {
                    SendEmail(user.Email, resetLink);
                    lblMessage.Text = "Un email de réinitialisation a été envoyé à " + user.Email;
                    lblMessage.Visible = true;
                    lblError.Visible = false;
                }
                catch (Exception ex)
                {
                    lblError.Text = "Erreur lors de l'envoi de l'email: " + ex.Message;
                    lblError.Visible = true;
                    lblMessage.Visible = false;
                }
            }
            else
            {
                // For security, generic message
                lblMessage.Text = "Si cette adresse existe, un email a été envoyé.";
                lblMessage.Visible = true;
                lblError.Visible = false;
            }
        }

        private void SendEmail(string toEmail, string link)
        {
            string smtpHost = System.Web.Configuration.WebConfigurationManager.AppSettings["SMTP_Host"];
            int smtpPort = int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["SMTP_Port"]);
            string smtpUser = System.Web.Configuration.WebConfigurationManager.AppSettings["SMTP_User"];
            string smtpPass = System.Web.Configuration.WebConfigurationManager.AppSettings["SMTP_Pass"];
            bool enableSsl = bool.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["SMTP_EnableSsl"]);

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(smtpUser, "E-comma Security");
                mail.To.Add(toEmail);
                mail.Subject = "Réinitialisation de votre mot de passe";
                mail.Body = $@"
                    <h2>Réinitialisation de mot de passe</h2>
                    <p>Vous avez demandé une réinitialisation de votre mot de passe.</p>
                    <p>Cliquez sur le lien ci-dessous pour créer un nouveau mot de passe :</p>
                    <p><a href='{link}'>Réinitialiser mon mot de passe</a></p>
                    <p>Ce lien est valide pour 1 heure.</p>
                    <hr>
                    <p>Si vous n'êtes pas à l'origine de cette demande, veuillez ignorer cet email.</p>";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = enableSsl;
                    smtp.Send(mail);
                }
            }
        }
    }
}
