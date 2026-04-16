using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using E_comma.Models;
using E_comma.Helpers;

namespace E_comma.Views.Public
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCart();
            }
        }

        private void LoadCart()
        {
            if (Session["UserId"] == null)
            {
                RedirectHelper.SafeRedirect("~/Views/Auth/Login.aspx");
                return;
            }

            Guid userId = (Guid)Session["UserId"];
            List<CartItem> items = CartItem.GetItems(userId);

            if (items.Count > 0)
            {
                rptCartItems.DataSource = items;
                rptCartItems.DataBind();

                decimal total = CartItem.GetTotal(userId);
                cartTotalDisplay.InnerText = $"DH {total:N2}";

                pnlCartContent.Visible = true;
                pnlEmptyCart.Visible = false;
            }
            else
            {
                pnlCartContent.Visible = false;
                pnlEmptyCart.Visible = true;
            }
        }

        protected void rptCartItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int itemId = Convert.ToInt32(e.CommandArgument);
            
            if (Session["UserId"] != null)
            {
                Guid userId = (Guid)Session["UserId"];
                List<CartItem> items = CartItem.GetItems(userId);
                CartItem item = items.Find(i => i.Id == itemId);

                if (item != null)
                {
                    if (e.CommandName == "Increase")
                    {
                        CartItem.UpdateQuantity(itemId, item.Quantity + 1);
                    }
                    else if (e.CommandName == "Decrease")
                    {
                        if (item.Quantity > 1)
                        {
                            CartItem.UpdateQuantity(itemId, item.Quantity - 1);
                        }
                        else
                        {
                            CartItem.RemoveItem(itemId);
                        }
                    }
                    else if (e.CommandName == "Remove")
                    {
                        CartItem.RemoveItem(itemId);
                    }

                    LoadCart();
                }
            }
        }
    }
}
