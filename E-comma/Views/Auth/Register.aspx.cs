using System;
using System.Text.RegularExpressions;
using System.Web;
using E_comma.Models;

namespace E_comma.Views.Auth
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // 1) Vérifier champs vides
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                lblMessage.Text = "Veuillez remplir tous les champs obligatoires.";
                lblMessage.Visible = true;
                lblMessage.CssClass = "error-message";
                return;
            }

            // 2) Vérifier confirmation mot de passe
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblMessage.Text = "Les mots de passe ne correspondent pas.";
                lblMessage.Visible = true;
                lblMessage.CssClass = "error-message";
                return;
            }

            // 3) Vérifier si email existe déjà
            var existingUser = Models.User.GetByEmail(txtEmail.Text);
            if (existingUser != null)
            {
                lblMessage.Text = "Cet email est déjà utilisé.";
                lblMessage.Visible = true;
                lblMessage.CssClass = "error-message";
                return;
            }

            // 4) Normaliser le numéro de téléphone
            string normalizedPhone = NormalizePhoneNumber(txtPhone.Text);

            // 5) Créer l'utilisateur
            bool ok = Models.User.Register(
                txtEmail.Text,
                normalizedPhone,
                txtName.Text,
                txtLastName.Text,
                txtPassword.Text
            );

            if (ok)
            {
                // Succès → afficher message et redirection
                lblMessage.Text = "Compte créé avec succès ! Redirection vers la page de connexion...";
                lblMessage.Visible = true;
                lblMessage.CssClass = "success-message";
                
                // Redirection après 2 secondes
                Response.AddHeader("REFRESH", "2;URL=Login.aspx");
            }
            else
            {
                lblMessage.Text = "Erreur lors de la création du compte.";
                lblMessage.Visible = true;
                lblMessage.CssClass = "error-message";
            }
        }

        /// <summary>
        /// Normalise les numéros de téléphone marocains au format international +212
        /// </summary>
        private string NormalizePhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            // Supprimer tous les espaces, tirets et parenthèses
            phone = Regex.Replace(phone, @"[\s\-\(\)]", "");

            // Si le numéro commence déjà par +212, le retourner tel quel
            if (phone.StartsWith("+212"))
                return phone;

            // Si le numéro commence par 212 sans le +, ajouter le +
            if (phone.StartsWith("212"))
                return "+" + phone;

            // Si le numéro commence par 0 (format local marocain)
            if (phone.StartsWith("0") && phone.Length == 10)
            {
                // Remplacer le 0 initial par +212
                return "+212" + phone.Substring(1);
            }

            // Si le numéro a 9 chiffres (sans le 0), ajouter +212
            if (phone.Length == 9 && Regex.IsMatch(phone, @"^[5-7]\d{8}$"))
            {
                return "+212" + phone;
            }

            // Cas par défaut : retourner le numéro tel quel
            return phone;
        }
    }
}
