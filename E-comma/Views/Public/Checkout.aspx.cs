using E_comma.Models;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using E_comma.Helpers;

namespace E_comma.Views.Public
{
    public partial class Checkout : System.Web.UI.Page
    {
        private const decimal TAX_RATE = 0.20m;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    RedirectHelper.SafeRedirect("~/Views/Auth/Login.aspx");
                    return;
                }

                LoadDeliveryMethods();
                LoadCart();
                CalculateTotals();
            }
        }

        // =========================
        // CHARGEMENT
        // =========================

        private void LoadDeliveryMethods()
        {
            rptDeliveryMethods.DataSource = DeliveryMethod.GetActive();
            rptDeliveryMethods.DataBind();
        }

        private List<CartItem> LoadCart()
        {
            Guid userId = (Guid)Session["UserId"];
            List<CartItem> items = CartItem.GetItems(userId);

            if (items.Count == 0)
            {
                RedirectHelper.SafeRedirect("Cart.aspx");
                return new List<CartItem>();
            }

            rptOrderItems.DataSource = items;
            rptOrderItems.DataBind();

            return items;
        }

        // =========================
        // CALCUL DES TOTAUX
        // =========================

        protected void btnCalculateShipping_Click(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            Guid userId = (Guid)Session["UserId"];

            decimal subtotal = CartItem.GetTotal(userId);
            decimal shipping = 0;

            if (int.TryParse(hfDeliveryMethodId.Value, out int methodId))
            {
                shipping = DeliveryMethod.CalculateDeliveryPrice(methodId, txtCity.Text.Trim());
            }

            decimal tax = subtotal * TAX_RATE;
            decimal total = subtotal + shipping + tax;

            lblSubtotal.Text = subtotal.ToString("N2");
            lblShipping.Text = shipping.ToString("N2");
            lblTax.Text = tax.ToString("N2");
            lblTotal.Text = total.ToString("N2");
        }

        // =========================
        // CONFIRMATION COMMANDE
        // =========================

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!int.TryParse(hfDeliveryMethodId.Value, out int deliveryMethodId))
            {
                lblError.Text = "Veuillez sélectionner un mode de livraison.";
                lblError.Visible = true;
                return;
            }

            Guid userId = (Guid)Session["UserId"];
            var items = CartItem.GetItems(userId);

            if (items.Count == 0)
            {
                RedirectHelper.SafeRedirect("Cart.aspx");
                return;
            }

            try
            {
                decimal subtotal = CartItem.GetTotal(userId);
                decimal shipping = DeliveryMethod.CalculateDeliveryPrice(deliveryMethodId, txtCity.Text.Trim());
                decimal tax = subtotal * TAX_RATE;
                decimal total = subtotal + shipping + tax;

                long orderId = CreateOrderWithDelivery(
                    userId,
                    total,
                    tax,
                    shipping,
                    "CashOnDelivery",
                    deliveryMethodId,
                    txtFullName.Text.Trim(),
                    txtAddress.Text.Trim(),
                    txtCity.Text.Trim(),
                    txtPhone.Text.Trim()
                );

                foreach (var item in items)
                {
                    OrderItem.Create(orderId, item.ProductVariantId, item.Quantity, item.Price);
                    StockMovement.CreateOrderMovement(item.ProductVariantId, item.Quantity, orderId, userId);
                    CartItem.RemoveItem(item.Id);
                }

                RedirectHelper.SafeRedirect("~/Views/User/Orders.aspx?created=1");
            }
            catch (Exception ex)
            {
                lblError.Text = "Erreur lors de la création de la commande.";
                lblError.Visible = true;
            }
        }

        // =========================
        // CRÉATION COMMANDE
        // =========================

        private long CreateOrderWithDelivery(
            Guid userId,
            decimal total,
            decimal tax,
            decimal shipping,
            string paymentMethod,
            int deliveryMethodId,
            string fullName,
            string address,
            string city,
            string phone)
        {
            using (var con = new System.Data.SqlClient.SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var cmd = new System.Data.SqlClient.SqlCommand(@"
                    INSERT INTO Orders
                    (UserId, Total, Tax, Shipping, Status, PaymentStatus, PaymentMethod,
                     DeliveryMethodId, DeliveryFullName, DeliveryAddress, DeliveryCity, DeliveryPhone, CreatedAt)
                    OUTPUT INSERTED.Id
                    VALUES
                    (@UserId, @Total, @Tax, @Shipping, 'Pending', 'Pending', @PaymentMethod,
                     @DeliveryMethodId, @FullName, @Address, @City, @Phone, @CreatedAt)", con);

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@Tax", tax);
                cmd.Parameters.AddWithValue("@Shipping", shipping);
                cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                cmd.Parameters.AddWithValue("@DeliveryMethodId", deliveryMethodId);
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@City", city);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                con.Open();
                return (long)cmd.ExecuteScalar();
            }
        }
    }
}
