using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace E_comma.Views.Admin
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
                var orders = GetOrdersWithUsers();
                OrdersRepeater.DataSource = orders;
                OrdersRepeater.DataBind();
            }
            catch (Exception ex)
            {
                ShowStatus("Erreur chargement commandes : " + ex.Message, false);
            }
        }

        private List<OrderRow> GetOrdersWithUsers()
        {
            var list = new List<OrderRow>();
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                const string query = @"SELECT o.Id, o.UserId, o.Total, o.Tax, o.Shipping, o.Status, o.PaymentStatus, o.CreatedAt,
                                              u.Name, u.LastName, u.Email, u.Phone
                                       FROM Orders o
                                       INNER JOIN Users u ON o.UserId = u.Id
                                       ORDER BY o.CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new OrderRow
                            {
                                Id = rdr["Id"] != DBNull.Value ? Convert.ToInt64(rdr["Id"]) : 0,
                                UserId = rdr["UserId"] != DBNull.Value ? (Guid)rdr["UserId"] : Guid.Empty,
                                Total = rdr["Total"] != DBNull.Value ? (decimal)rdr["Total"] : 0,
                                Tax = rdr["Tax"] != DBNull.Value ? (decimal)rdr["Tax"] : 0,
                                Shipping = rdr["Shipping"] != DBNull.Value ? (decimal)rdr["Shipping"] : 0,
                                Status = rdr["Status"].ToString(),
                                PaymentStatus = rdr["PaymentStatus"].ToString(),
                                CreatedAt = rdr["CreatedAt"] != DBNull.Value ? (DateTime)rdr["CreatedAt"] : DateTime.Now,
                                CustomerName = ((rdr["Name"] != DBNull.Value ? rdr["Name"].ToString() : string.Empty) + " " + (rdr["LastName"] != DBNull.Value ? rdr["LastName"].ToString() : string.Empty)).Trim(),
                                CustomerEmail = rdr["Email"] != DBNull.Value ? rdr["Email"].ToString() : string.Empty,
                                CustomerPhone = rdr["Phone"] != DBNull.Value ? rdr["Phone"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            return list;
        }

        protected void OrdersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var order = (OrderRow)e.Item.DataItem;
            var ltStatus = (Literal)e.Item.FindControl("ltStatus");
            var itemsRepeater = (Repeater)e.Item.FindControl("ItemsRepeater");

            if (ltStatus != null)
            {
                string statusClass = GetStatusClass(order.Status);
                ltStatus.Text = "<span class='badge-status " + statusClass + "'>" + order.Status + "</span>";
            }

            if (itemsRepeater != null)
            {
                var details = GetOrderItems(order.Id);
                itemsRepeater.DataSource = details;
                itemsRepeater.DataBind();
            }
        }

        protected void OrdersRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            long orderId;
            if (!long.TryParse(e.CommandArgument.ToString(), out orderId))
            {
                return;
            }

            try
            {
                if (e.CommandName == "Confirm")
                {
                    UpdateOrderStatus(orderId, "Delivered", "Pending");
                    ShowStatus("Commande confirmee, livree.", true);
                }
                else if (e.CommandName == "Cancel")
                {
                    UpdateOrderStatus(orderId, "Cancelled", "Failed");
                    ShowStatus("Commande annulee.", true);
                }

                LoadOrders();
            }
            catch (Exception ex)
            {
                ShowStatus("Erreur de mise a jour : " + ex.Message, false);
            }
        }

        private void UpdateOrderStatus(long orderId, string status, string paymentStatus)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                const string query = "UPDATE Orders SET Status=@Status, PaymentStatus=@PaymentStatus WHERE Id=@Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                    cmd.Parameters.AddWithValue("@Id", orderId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private List<OrderItemRow> GetOrderItems(long orderId)
        {
            var items = new List<OrderItemRow>();
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                const string query = @"
                    SELECT oi.Id AS OrderItemId,
                           oi.ProductVariantId,
                           oi.Quantity,
                           oi.UnitPrice,
                           pv.ProductId,
                           pv.Attributes,
                           p.Name AS ProductName,
                           p.Brand,
                           (SELECT TOP 1 ImageUrl FROM ProductImages WHERE ProductId = p.Id AND IsMainImage = 1) AS MainImageUrl
                    FROM OrderItems oi
                    INNER JOIN ProductVariants pv ON oi.ProductVariantId = pv.Id
                    INNER JOIN Products p ON pv.ProductId = p.Id
                    WHERE oi.OrderId = @OrderId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            items.Add(new OrderItemRow
                            {
                                OrderItemId = rdr["OrderItemId"] != DBNull.Value ? Convert.ToInt64(rdr["OrderItemId"]) : 0,
                                ProductVariantId = rdr["ProductVariantId"] != DBNull.Value ? Convert.ToInt32(rdr["ProductVariantId"]) : 0,
                                ProductId = rdr["ProductId"] != DBNull.Value ? Convert.ToInt32(rdr["ProductId"]) : 0,
                                ProductName = rdr["ProductName"] != DBNull.Value ? rdr["ProductName"].ToString() : string.Empty,
                                Brand = rdr["Brand"] != DBNull.Value ? rdr["Brand"].ToString() : string.Empty,
                                Attributes = rdr["Attributes"] != DBNull.Value ? rdr["Attributes"].ToString() : string.Empty,
                                UnitPrice = rdr["UnitPrice"] != DBNull.Value ? (decimal)rdr["UnitPrice"] : 0,
                                Quantity = rdr["Quantity"] != DBNull.Value ? Convert.ToInt32(rdr["Quantity"]) : 0,
                                MainImageUrl = rdr["MainImageUrl"] != DBNull.Value ? rdr["MainImageUrl"].ToString() : "/images/no-image.jpg"
                            });
                        }
                    }
                }
            }
            return items;
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

        private class OrderRow
        {
            public long Id { get; set; }
            public Guid UserId { get; set; }
            public decimal Total { get; set; }
            public decimal Tax { get; set; }
            public decimal Shipping { get; set; }
            public string Status { get; set; }
            public string PaymentStatus { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
        }

        private class OrderItemRow
        {
            public long OrderItemId { get; set; }
            public int ProductVariantId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string Brand { get; set; }
            public string Attributes { get; set; }
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public string MainImageUrl { get; set; }
            public decimal LineTotal
            {
                get { return UnitPrice * Quantity; }
            }
        }
    }
}
