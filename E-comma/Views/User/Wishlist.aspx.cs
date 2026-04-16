using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using E_comma.Models;
using E_comma.Helpers;

namespace E_comma.Views.User
{
    public partial class Wishlist : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Panel LoginRequiredPanel;
        protected System.Web.UI.WebControls.Panel WishlistPanel;
        protected System.Web.UI.WebControls.Repeater WishlistRepeater;
        protected System.Web.UI.WebControls.Panel EmptyWishlistPanel;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadWishlist();
            }
        }

        private void LoadWishlist()
        {
            try
            {
                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    LoginRequiredPanel.Visible = true;
                    WishlistPanel.Visible = false;
                    return;
                }

                Guid userId = (Guid)Session["UserId"];

                // Get user's wishlist
                var wishlistItems = Models.Wishlist.GetUserWishlist(userId);

                if (wishlistItems != null && wishlistItems.Count > 0)
                {
                    WishlistRepeater.DataSource = wishlistItems;
                    WishlistRepeater.DataBind();
                    WishlistRepeater.Visible = true;
                    EmptyWishlistPanel.Visible = false;
                }
                else
                {
                    WishlistRepeater.Visible = false;
                    EmptyWishlistPanel.Visible = true;
                }

                LoginRequiredPanel.Visible = false;
                WishlistPanel.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur chargement wishlist: " + ex.Message);
                EmptyWishlistPanel.Visible = true;
                WishlistRepeater.Visible = false;
            }
        }

        protected void WishlistRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                try
                {
                    if (Session["UserId"] == null)
                    {
                        RedirectHelper.SafeRedirect("../Auth/Login.aspx");
                        return;
                    }

                    Guid userId = (Guid)Session["UserId"];
                    int productId = Convert.ToInt32(e.CommandArgument);

                    // Attempt to remove
                    Models.Wishlist.RemoveFromWishlist(userId, productId);

                    // Always reload the wishlist to reflect the current state (empty or not)
                    LoadWishlist();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur ItemCommand: " + ex.Message);
                }
            }
        }

        [WebMethod]
        public static object RemoveFromWishlist(int productId)
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return new { success = false, message = "Vous devez être connecté" };
                }

                Guid userId = (Guid)System.Web.HttpContext.Current.Session["UserId"];

                bool success = Models.Wishlist.RemoveFromWishlist(userId, productId);

                if (success)
                {
                    return new { success = true, message = "Produit retiré de la liste de souhaits" };
                }
                else
                {
                    return new { success = false, message = "Erreur lors de la suppression" };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Erreur: " + ex.Message };
            }
        }

        protected string GetStarRating(double avgRating, int reviewCount)
        {
            string stars = "<div class='star-rating d-inline-block'>";

            for (int i = 1; i <= 5; i++)
            {
                if (i <= avgRating)
                {
                    stars += "<i class='fas fa-star text-warning'></i>";
                }
                else if (i - avgRating < 1 && i - avgRating > 0)
                {
                    stars += "<i class='fas fa-star-half-alt text-warning'></i>";
                }
                else
                {
                    stars += "<i class='far fa-star text-warning'></i>";
                }
            }

            stars += $"</div> <span class='text-muted ms-1'>({reviewCount})</span>";

            return stars;
        }
    }
}
