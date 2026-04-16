using System;
using System.Web;
using System.Net.Mail;
using E_comma.Models;

namespace E_comma.Views.Auth
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        private Guid _token;

        protected void Page_Load(object sender, EventArgs e)
        {
            string tokenStr = Request.QueryString["token"];
            if (string.IsNullOrEmpty(tokenStr) || !Guid.TryParse(tokenStr, out _token))
            {
                lblError.Text = "Token invalide ou manquant.";
                lblError.Visible = true;
                return;
            }

            if (!IsPostBack)
            {
                PasswordResetToken resetToken = PasswordResetToken.GetByToken(_token);
                if (resetToken == null)
                {
                    lblError.Text = "Ce lien a expiré ou a déjà été utilisé.";
                    lblError.Visible = true;
                }
                else
                {
                    pnlReset.Visible = true;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text;
            string confirm = txtConfirmPassword.Text;

            if (password != confirm)
            {
                lblError.Text = "Les mots de passe ne correspondent pas.";
                lblError.Visible = true;
                return;
            }

            PasswordResetToken resetToken = PasswordResetToken.GetByToken(_token);
            if (resetToken != null)
            {
                // Get User to update password
                // We need to get the user email or ID. The token has UserId.
                // But User.UpdatePassword takes email. Let's add UpdatePasswordById or get email first.
                // For now, let's fetch user by ID to get email, then update.
                // Wait, User model doesn't have GetById. I should add it or just add UpdatePasswordById.
                // Let's add UpdatePasswordById to User model or just use a direct SQL here for simplicity?
                // No, better to keep logic in User model.
                // Actually, I can just fetch the user email if I had GetById.
                
                // Let's assume I can add a method to User model or just do it here since it's "code-behind" but User model is better.
                // I'll add a quick helper method here or modify User model again?
                // Modifying User model is cleaner. I'll do that in a separate step if needed.
                // For now, let's try to see if I can get the email easily.
                // I'll add `User.GetById` or `User.UpdatePassword(Guid userId, string password)`.
                
                // Let's modify User.cs to add UpdatePassword(Guid id, string password)
                // But I can't do that in this file write.
                // I will assume I will add it.
                
                if (E_comma.Models.User.UpdatePassword(resetToken.UserId, password))
                {
                    PasswordResetToken.MarkAsUsed(_token);
                    
                    // Retrieve user to send confirmation email
                    E_comma.Models.User user = E_comma.Models.User.GetById(resetToken.UserId);
                    if (user != null)
                    {
                        try
                        {
                            SendConfirmationEmail(user.Email);
                        }
                        catch (Exception)
                        {
                            // Log error? For now, we don't block the success message if email fails.
                        }
                    }

                    lblMessage.Text = "Votre mot de passe a été réinitialisé avec succès. Vous pouvez maintenant vous connecter.";
                    lblMessage.Visible = true;
                    lblError.Visible = false;
                    pnlReset.Visible = false;
                }
                else
                {
                    lblError.Text = "Erreur lors de la mise à jour du mot de passe.";
                    lblError.Visible = true;
                    lblMessage.Visible = false;
                }
            }
            else
            {
                lblError.Text = "Token invalide.";
                lblError.Visible = true;
                lblMessage.Visible = false;
            }
        }

        private void SendConfirmationEmail(string toEmail)
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
                mail.Subject = "Confirmation de changement de mot de passe";
                mail.Body = $@"
                    <h2>Mot de passe modifié</h2>
                    <p>Votre mot de passe a été modifié avec succès.</p>
                    <p>Si vous n'êtes pas à l'origine de cette modification, veuillez contacter le support immédiatement.</p>
                    <hr>
                    <p>E-comma Security Team</p>";
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
