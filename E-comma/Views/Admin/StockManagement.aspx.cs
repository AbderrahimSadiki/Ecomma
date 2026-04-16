using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using E_comma.Models;
using E_comma.Helpers;

namespace E_comma.Views.Admin
{
    public partial class StockManagement : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStockData();
                LoadStockAlerts();
                LoadRecentMovements();
                LoadProductVariantsForStock();
            }
        }

        private void LoadStockData()
        {
            var variants = ProductVariant.GetByProductId(0); // Récupère toutes les variantes
            rptStock.DataSource = GetAllVariantsWithStock();
            rptStock.DataBind();
        }

        private List<object> GetAllVariantsWithStock()
        {
            var list = new List<object>();
            using (var con = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = @"SELECT pv.Id, pv.SKU, pv.Attributes, pv.StockQuantity, pv.Price,
                                 p.Id AS ProductId, p.Name AS ProductName, p.Brand,
                                 sa.ThresholdQuantity, sa.IsActive AS AlertActive
                                 FROM ProductVariants pv
                                 INNER JOIN Products p ON pv.ProductId = p.Id
                                 LEFT JOIN StockAlerts sa ON pv.Id = sa.ProductVariantId
                                 ORDER BY p.Name, pv.Attributes";

                using (var cmd = new System.Data.SqlClient.SqlCommand(query, con))
                {
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new
                            {
                                Id = (int)reader["Id"],
                                SKU = reader["SKU"].ToString(),
                                Attributes = reader["Attributes"].ToString(),
                                StockQuantity = (int)reader["StockQuantity"],
                                Price = (decimal)reader["Price"],
                                ProductId = (int)reader["ProductId"],
                                ProductName = reader["ProductName"].ToString(),
                                Brand = reader["Brand"] != DBNull.Value ? reader["Brand"].ToString() : "",
                                ThresholdQuantity = reader["ThresholdQuantity"] != DBNull.Value ? (int)reader["ThresholdQuantity"] : 0,
                                AlertActive = reader["AlertActive"] != DBNull.Value ? (bool)reader["AlertActive"] : false
                            });
                        }
                    }
                }
            }
            return list;
        }

        private void LoadStockAlerts()
        {
            var alerts = StockAlert.GetActiveAlerts();
            rptAlerts.DataSource = alerts;
            rptAlerts.DataBind();

            lblAlertCount.Text = alerts.Count.ToString();
            AlertsPanel.Visible = alerts.Count > 0;
            NoAlertsPanel.Visible = alerts.Count == 0;
        }

        private void LoadRecentMovements()
        {
            var movements = StockMovement.GetAll(null, 50);
            rptMovements.DataSource = movements;
            rptMovements.DataBind();
        }

        private void LoadProductVariantsForStock()
        {
            ddlVariant.Items.Clear();
            ddlVariant.Items.Add(new ListItem("-- Sélectionner un produit --", ""));

            var list = GetAllVariantsWithStock();
            foreach (dynamic item in list)
            {
                string text = $"{item.ProductName} - {item.Attributes} (SKU: {item.SKU})";
                ddlVariant.Items.Add(new ListItem(text, item.Id.ToString()));
            }
        }

        protected void btnAddStock_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlVariant.SelectedValue))
            {
                SetStatus(lblStockStatus, "Veuillez sélectionner un produit.", false);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                SetStatus(lblStockStatus, "Quantité invalide.", false);
                return;
            }

            try
            {
                int variantId = int.Parse(ddlVariant.SelectedValue);
                string movementType = ddlMovementType.SelectedValue;
                string notes = txtNotes.Text.Trim();

                Guid? userId = Session["UserId"] != null ? (Guid?)Session["UserId"] : null;

                bool success = StockMovement.Create(variantId, movementType, quantity, null, notes, userId);

                if (success)
                {
                    SetStatus(lblStockStatus, "Stock mis à jour avec succès.", true);
                    LoadStockData();
                    LoadStockAlerts();
                    LoadRecentMovements();

                    // Reset form
                    ddlVariant.SelectedIndex = 0;
                    txtQuantity.Text = "";
                    txtNotes.Text = "";
                }
                else
                {
                    SetStatus(lblStockStatus, "Échec de la mise à jour du stock.", false);
                }
            }
            catch (Exception ex)
            {
                SetStatus(lblStockStatus, "Erreur : " + ex.Message, false);
            }
        }

        protected void rptStock_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int variantId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "UpdateAlert")
            {
                // Logique pour mettre à jour l'alerte
                RedirectHelper.SafeRedirect("StockManagement.aspx");
            }
        }



        protected string GetMovementTypeLabel(object type)
        {
            string movementType = type?.ToString() ?? "";
            switch (movementType.ToUpper())
            {
                case "IN": return "<span class='badge bg-success'>Entrée</span>";
                case "OUT": return "<span class='badge bg-danger'>Sortie</span>";
                case "ORDER": return "<span class='badge bg-primary'>Commande</span>";
                case "RETURN": return "<span class='badge bg-info'>Retour</span>";
                case "ADJUSTMENT": return "<span class='badge bg-warning'>Ajustement</span>";
                default: return "<span class='badge bg-secondary'>" + movementType + "</span>";
            }
        }

        private void SetStatus(Label target, string message, bool success)
        {
            target.Text = message;
            target.CssClass = success ? "alert alert-success" : "alert alert-danger";
            target.Visible = true;
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
        protected string GetStockStatusClass(object stockObj, object thresholdObj)
        {
            int stock = Convert.ToInt32(stockObj);
            int threshold = Convert.ToInt32(thresholdObj);


            if (stock <= 0) return "badge bg-danger";
            if (stock <= threshold) return "badge bg-warning";
            return "badge bg-success";
        }




        protected string GetStatusBadge(object statusObj)
        {
            string status = statusObj?.ToString();
            switch (status)
            {
                case "Pending": return "badge bg-warning";
                case "Completed": return "badge bg-success";
                case "Cancelled": return "badge bg-danger";
                default: return "badge bg-secondary";
            }
        }

    }
}
