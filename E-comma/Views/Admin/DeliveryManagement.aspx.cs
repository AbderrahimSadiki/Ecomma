using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using E_comma.Models;

namespace E_comma.Views.Admin
{
    public partial class DeliveryManagement : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDeliveryMethods();
            }
        }

        private void LoadDeliveryMethods()
        {
            var methods = DeliveryMethod.GetAll();
            rptDeliveryMethods.DataSource = methods;
            rptDeliveryMethods.DataBind();
        }

        protected void btnSaveDeliveryMethod_Click(object sender, EventArgs e)
        {
            string name = txtMethodName.Text.Trim();
            string description = txtMethodDescription.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                SetStatus(lblDeliveryStatus, "Le nom est obligatoire.", false);
                return;
            }

            if (!decimal.TryParse(txtMethodPrice.Text.Trim(), out decimal price))
            {
                SetStatus(lblDeliveryStatus, "Prix invalide.", false);
                return;
            }

            if (!int.TryParse(txtEstimatedDays.Text.Trim(), out int estimatedDays))
            {
                SetStatus(lblDeliveryStatus, "Délai invalide.", false);
                return;
            }

            if (!int.TryParse(txtDisplayOrder.Text.Trim(), out int displayOrder))
            {
                displayOrder = 0;
            }

            bool isActive = chkMethodActive.Checked;

            try
            {
                int methodId = 0;
                int.TryParse(hfDeliveryMethodId.Value, out methodId);

                bool success;
                if (methodId > 0)
                {
                    success = DeliveryMethod.Update(methodId, name, description, price, estimatedDays, isActive, displayOrder);
                    SetStatus(lblDeliveryStatus, success ? "Mode de livraison mis à jour." : "Mise à jour impossible.", success);
                }
                else
                {
                    int newId = DeliveryMethod.Create(name, description, price, estimatedDays, isActive, displayOrder);
                    success = newId > 0;
                    SetStatus(lblDeliveryStatus, success ? "Mode de livraison ajouté." : "Ajout impossible.", success);
                }

                if (success)
                {
                    LoadDeliveryMethods();
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                SetStatus(lblDeliveryStatus, "Erreur : " + ex.Message, false);
            }
        }

        protected void btnResetDeliveryMethod_Click(object sender, EventArgs e)
        {
            ResetForm();
            lblDeliveryStatus.Visible = false;
        }

        protected void rptDeliveryMethods_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int methodId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditMethod")
            {
                LoadMethodForEdit(methodId);
            }
            else if (e.CommandName == "DeleteMethod")
            {
                DeleteMethod(methodId);
            }
            else if (e.CommandName == "ToggleMethod")
            {
                ToggleMethod(methodId);
            }
        }

        private void LoadMethodForEdit(int methodId)
        {
            var method = DeliveryMethod.GetById(methodId);
            if (method == null)
            {
                SetStatus(lblDeliveryStatus, "Mode de livraison introuvable.", false);
                return;
            }

            hfDeliveryMethodId.Value = method.Id.ToString();
            txtMethodName.Text = method.Name;
            txtMethodDescription.Text = method.Description;
            txtMethodPrice.Text = method.Price.ToString("0.00");
            txtEstimatedDays.Text = method.EstimatedDays.ToString();
            txtDisplayOrder.Text = method.DisplayOrder.ToString();
            chkMethodActive.Checked = method.IsActive;
        }

        private void DeleteMethod(int methodId)
        {
            try
            {
                bool success = DeliveryMethod.Delete(methodId);
                SetStatus(lblDeliveryStatus,
                    success ? "Mode de livraison supprimé." : "Suppression impossible (utilisé dans des commandes).",
                    success);

                if (success)
                {
                    LoadDeliveryMethods();
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                SetStatus(lblDeliveryStatus, "Erreur : " + ex.Message, false);
            }
        }

        private void ToggleMethod(int methodId)
        {
            try
            {
                var method = DeliveryMethod.GetById(methodId);
                if (method == null)
                {
                    SetStatus(lblDeliveryStatus, "Mode de livraison introuvable.", false);
                    return;
                }

                bool success = DeliveryMethod.Update(
                    method.Id,
                    method.Name,
                    method.Description,
                    method.Price,
                    method.EstimatedDays,
                    !method.IsActive,
                    method.DisplayOrder
                );

                SetStatus(lblDeliveryStatus,
                    success ? "Statut mis à jour." : "Mise à jour impossible.",
                    success);

                if (success)
                {
                    LoadDeliveryMethods();
                }
            }
            catch (Exception ex)
            {
                SetStatus(lblDeliveryStatus, "Erreur : " + ex.Message, false);
            }
        }

        private void ResetForm()
        {
            hfDeliveryMethodId.Value = "";
            txtMethodName.Text = "";
            txtMethodDescription.Text = "";
            txtMethodPrice.Text = "";
            txtEstimatedDays.Text = "3";
            txtDisplayOrder.Text = "0";
            chkMethodActive.Checked = true;
        }

        private void SetStatus(Label target, string message, bool success)
        {
            target.Text = message;
            target.CssClass = success ? "alert alert-success" : "alert alert-danger";
            target.Visible = true;
        }

        protected string GetStatusBadge(object isActive)
        {
            bool active = isActive != null && (bool)isActive;
            return active
                ? "<span class='badge bg-success'>Actif</span>"
                : "<span class='badge bg-secondary'>Inactif</span>";
        }

        protected string FormatPrice(object price)
        {
            if (price == null) return "0.00 DH";
            return ((decimal)price).ToString("N2") + " DH";
        }

        protected string FormatDays(object days)
        {
            if (days == null) return "-";
            int d = (int)days;
            if (d == 0) return "Immédiat";
            if (d == 1) return "1 jour";
            return d + " jours";
        }
    }
}