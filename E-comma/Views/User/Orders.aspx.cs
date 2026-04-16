using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrderModel = E_comma.Models.Order;
using OrderItemModel = E_comma.Models.OrderItem;
using E_comma.Helpers;

namespace E_comma.Views.User
{
    public partial class Orders : Page
    {
        private string ConnString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    RedirectHelper.SafeRedirect("/Views/Auth/Login.aspx");
                    return;
                }

                Guid userId = (Guid)Session["UserId"];
                var orders = OrderModel.GetByUserId(userId);

                if (orders != null && orders.Count > 0)
                {
                    lblOrderCount.Text = orders.Count.ToString();
                    OrdersRepeater.DataSource = orders;
                    OrdersRepeater.DataBind();

                    OrdersPanel.Visible = true;
                    EmptyPanel.Visible = false;
                }
                else
                {
                    OrdersPanel.Visible = false;
                    EmptyPanel.Visible = true;
                }

                if (Request.QueryString["created"] == "1")
                {
                    ShowStatus("Commande enregistree. Notre equipe vous contactera pour confirmation.", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Erreur lors du chargement des commandes : " + ex.Message, false);
                OrdersPanel.Visible = false;
                EmptyPanel.Visible = true;
            }
        }

        protected void OrdersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var order = (OrderModel)e.Item.DataItem;
            var statusLiteral = (Literal)e.Item.FindControl("ltStatus");
            var itemsRepeater = (Repeater)e.Item.FindControl("ItemsRepeater");
            var cancelButton = (Button)e.Item.FindControl("btnCancel");

            if (statusLiteral != null)
            {
                string statusClass = GetStatusClass(order.Status);
                statusLiteral.Text = "<span class='badge-status " + statusClass + "'>" + order.Status + "</span>";
            }

            if (cancelButton != null)
            {
                cancelButton.Visible = string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase);
            }

            if (itemsRepeater != null)
            {
                var details = OrderItemModel.GetDetailsByOrderId(order.Id);
                itemsRepeater.DataSource = details;
                itemsRepeater.DataBind();
            }
        }

        protected void OrdersRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Cancel")
                return;

            if (Session["UserId"] == null)
            {
                RedirectHelper.SafeRedirect("/Views/Auth/Login.aspx");
                return;
            }

            long orderId;
            if (!long.TryParse(e.CommandArgument.ToString(), out orderId))
            {
                ShowStatus("Commande introuvable.", false);
                return;
            }

            try
            {
                Guid userId = (Guid)Session["UserId"];
                bool updated = CancelPendingOrder(orderId, userId);
                ShowStatus(updated ? "Commande annulee." : "Impossible d'annuler cette commande.", updated);
                LoadOrders();
            }
            catch (Exception ex)
            {
                ShowStatus("Erreur lors de l'annulation : " + ex.Message, false);
            }
        }

        private string GetStatusClass(string status)
        {
            switch ((status ?? string.Empty).ToLowerInvariant())
            {
                case "processing": return "status-processing";
                case "delivered": return "status-delivered";
                case "cancelled": return "status-cancelled";
                default: return "status-pending";
            }
        }

        private void ShowStatus(string message, bool success)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = success ? "alert alert-success" : "alert alert-danger";
            lblStatus.Visible = true;
        }

        private bool CancelPendingOrder(long orderId, Guid userId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                const string query = "UPDATE Orders SET Status=@Status, PaymentStatus=@PaymentStatus WHERE Id=@Id AND UserId=@UserId AND Status='Pending'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", "Cancelled");
                    cmd.Parameters.AddWithValue("@PaymentStatus", "Failed");
                    cmd.Parameters.AddWithValue("@Id", orderId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
