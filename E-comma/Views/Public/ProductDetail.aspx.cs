using E_comma.Models;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace E_comma.Views.Public
{
    public partial class ProductDetail : System.Web.UI.Page
    {
        protected int ProductId
        {
            get { return ViewState["ProductId"] != null ? (int)ViewState["ProductId"] : 0; }
            set { ViewState["ProductId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProductDetail();
            }
        }

        private void LoadProductDetail()
        {
            try
            {
                // Récupérer l'ID du produit depuis l'URL
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    if (int.TryParse(Request.QueryString["id"], out int productId))
                    {
                        ProductId = productId;
                        var product = ProductExtended.GetDetailedById(productId);

                        if (product != null)
                        {
                            DisplayProduct(product);
                            LoadReviews(productId);
                            SetupReviewForm();
                        }
                        else
                        {
                            ShowProductNotFound();
                        }
                    }
                    else
                    {
                        ShowProductNotFound();
                    }
                }
                else
                {
                    ShowProductNotFound();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur chargement produit: " + ex.Message);
                ShowProductNotFound();
            }
        }

        private void DisplayProduct(ProductExtended product)
        {
            // Breadcrumb
            ltCategoryLink.Text = $"<a href='Shop.aspx?cat={product.CategoryId}'>{product.CategoryName}</a>";
            ltProductName.Text = product.Name;

            // Product Info
            ltCategory.Text = product.CategoryName;
            ltBrand.Text = product.Brand;
            ltProductTitle.Text = product.Name;
            ltDescription.Text = product.Description;
            ltPrice.Text = product.BasePrice.ToString("N2") + " DH";

            // Rating
            ltRating.Text = GetStarRatingHTML(product.AverageRating, product.ReviewCount);
            ltReviewCount.Text = product.ReviewCount.ToString();

            // Images
            if (product.Images != null && product.Images.Count > 0)
            {
                ImagesRepeater.DataSource = product.Images;
                ImagesRepeater.DataBind();
            }
            else
            {
                // Image par défaut si aucune image
                var defaultImages = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/no-image.jpg", AltText = "No image available" }
                };
                ImagesRepeater.DataSource = defaultImages;
                ImagesRepeater.DataBind();
            }

            // Variants
            if (product.Variants != null && product.Variants.Count > 0)
            {
                VariantsPanel.Visible = true;
                VariantsRepeater.DataSource = product.Variants;
                VariantsRepeater.DataBind();
            }
            else
            {
                VariantsPanel.Visible = false;
            }

            ProductDetailPanel.Visible = true;
            ProductNotFoundPanel.Visible = false;
        }

        private void LoadReviews(int productId)
        {
            var reviews = ProductReview.GetByProductId(productId);

            if (reviews != null && reviews.Count > 0)
            {
                ReviewsRepeater.DataSource = reviews;
                ReviewsRepeater.DataBind();
                NoReviewsPanel.Visible = false;
            }
            else
            {
                NoReviewsPanel.Visible = true;
            }
        }

        private void SetupReviewForm()
        {
            // Vérifier si l'utilisateur est connecté
            if (Session["UserId"] != null)
            {
                ReviewFormPanel.Visible = true;
                LoginRequiredPanel.Visible = false;
            }
            else
            {
                ReviewFormPanel.Visible = false;
                LoginRequiredPanel.Visible = true;
            }
        }

        private void ShowProductNotFound()
        {
            ProductDetailPanel.Visible = false;
            ProductNotFoundPanel.Visible = true;
        }

        // Soumettre un avis
        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    lblReviewMessage.Text = "<span class='text-danger'>Vous devez être connecté</span>";
                    lblReviewMessage.CssClass = "ms-3 text-danger";
                    return;
                }

                int rating = 0;
                if (!string.IsNullOrEmpty(hdnRating.Value))
                {
                    int.TryParse(hdnRating.Value, out rating);
                }

                string comment = txtComment.Text.Trim();

                if (rating == 0)
                {
                    lblReviewMessage.Text = "<span class='text-danger'>Veuillez sélectionner une note</span>";
                    lblReviewMessage.CssClass = "ms-3 text-danger";
                    return;
                }

                if (string.IsNullOrEmpty(comment))
                {
                    lblReviewMessage.Text = "<span class='text-danger'>Veuillez saisir un commentaire</span>";
                    lblReviewMessage.CssClass = "ms-3 text-danger";
                    return;
                }

                Guid userId = (Guid)Session["UserId"];
                bool success = ProductReview.Create(ProductId, userId, rating, comment);

                if (success)
                {
                    lblReviewMessage.Text = "<span class='text-success'>✓ Avis publié avec succès!</span>";
                    lblReviewMessage.CssClass = "ms-3 text-success";
                    txtComment.Text = "";
                    hdnRating.Value = "0";

                    // Recharger les avis
                    LoadReviews(ProductId);
                }
                else
                {
                    lblReviewMessage.Text = "<span class='text-danger'>Erreur lors de la publication</span>";
                    lblReviewMessage.CssClass = "ms-3 text-danger";
                }
            }
            catch (Exception ex)
            {
                lblReviewMessage.Text = "<span class='text-danger'>Erreur: " + ex.Message + "</span>";
                lblReviewMessage.CssClass = "ms-3 text-danger";
            }
        }

        // WebMethod pour ajouter au panier (AJAX)
        [WebMethod]
        public static object AddToCart(int productId, int variantId, int quantity)
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return new { success = false, message = "Vous devez être connecté" };
                }

                Guid userId = (Guid)System.Web.HttpContext.Current.Session["UserId"];

                // Si pas de variante sélectionnée, essayer d'en trouver une ou d'en créer une par défaut
                if (variantId <= 0)
                {
                    var variants = ProductVariant.GetByProductId(productId);
                    if (variants != null && variants.Count > 0)
                    {
                        // Utiliser la première variante disponible
                        variantId = variants[0].Id;
                    }
                    else
                    {
                        // Aucune variante n'existe, créer une variante par défaut
                        var product = ProductExtended.GetDetailedById(productId);
                        if (product != null)
                        {
                            // Créer une variante par défaut avec les infos du produit
                            variantId = ProductVariant.Create(
                                productId, 
                                "SKU-" + productId, 
                                "Standard", 
                                product.BasePrice, 
                                100 // Stock par défaut
                            );
                        }
                        else
                        {
                            return new { success = false, message = "Produit introuvable" };
                        }
                    }
                }

                // Vérifier le stock
                var variant = ProductVariant.GetById(variantId);
                if (variant == null)
                {
                    return new { success = false, message = "Variante introuvable" };
                }

                if (variant.StockQuantity < quantity)
                {
                    return new { success = false, message = "Stock insuffisant" };
                }

                // Ajouter au panier
                bool success = CartItem.AddToCart(userId, variantId, quantity);

                if (success)
                {
                    // Retourner les données du panier pour mise à jour immédiate
                    return GetCartData(userId);
                }
                else
                {
                    return new { success = false, message = "Erreur lors de l'ajout" };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Erreur: " + ex.Message };
            }
        }

        [WebMethod]
        public static object GetCart()
        {
            if (System.Web.HttpContext.Current.Session["UserId"] == null) return new { success = false };
            Guid userId = (Guid)System.Web.HttpContext.Current.Session["UserId"];
            return GetCartData(userId);
        }

        [WebMethod]
        public static object UpdateCartItem(int itemId, int quantity)
        {
            if (System.Web.HttpContext.Current.Session["UserId"] == null) return new { success = false };
            Guid userId = (Guid)System.Web.HttpContext.Current.Session["UserId"];
            
            if (CartItem.UpdateQuantity(itemId, quantity))
            {
                return GetCartData(userId);
            }
            return new { success = false };
        }

        [WebMethod]
        public static object RemoveCartItem(int itemId)
        {
            if (System.Web.HttpContext.Current.Session["UserId"] == null) return new { success = false };
            Guid userId = (Guid)System.Web.HttpContext.Current.Session["UserId"];

            if (CartItem.RemoveItem(itemId))
            {
                return GetCartData(userId);
            }
            return new { success = false };
        }

        private static object GetCartData(Guid userId)
        {
            var items = CartItem.GetItems(userId);
            var total = CartItem.GetTotal(userId);
            var count = CartItem.GetCartCount(userId);

            return new
            {
                success = true,
                items = items,
                total = total.ToString("N2"),
                count = count
            };
        }

        // WebMethod pour ajouter à la liste de souhaits (AJAX)
        [WebMethod]
        public static object AddToWishlist(int productId)
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["UserId"] == null)
                {
                    return new { success = false, message = "Vous devez être connecté" };
                }

                Guid userId = (Guid)System.Web.HttpContext.Current.Session["UserId"];

                // Vérifier si déjà dans la liste
                if (Wishlist.IsInWishlist(userId, productId))
                {
                    return new { success = false, message = "Ce produit est déjà dans votre liste de souhaits." };
                }

                // Ajouter à la wishlist
                bool success = Wishlist.AddToWishlist(userId, productId);

                if (success)
                {
                    return new { success = true, message = "Produit ajouté à la liste de souhaits" };
                }
                else
                {
                    return new { success = false, message = "Erreur lors de l'ajout" };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Erreur: " + ex.Message };
            }
        }

        // Helper: Générer HTML des étoiles pour les avis
        protected string GetStars(object rating)
        {
            try
            {
                int ratingValue = rating != null ? Convert.ToInt32(rating) : 0;
                string stars = "";

                for (int i = 1; i <= 5; i++)
                {
                    if (i <= ratingValue)
                    {
                        stars += "<i class='fas fa-star text-warning'></i>";
                    }
                    else
                    {
                        stars += "<i class='far fa-star text-warning'></i>";
                    }
                }
                return stars;
            }
            catch
            {
                return "";
            }
        }

        private string GetStarRatingHTML(double avgRating, int reviewCount)
        {
            string stars = "<div class='star-rating-large mb-2'>";

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

            stars += $"</div><p class='text-muted mb-0'>{avgRating:F1} sur 5 ({reviewCount} avis)</p>";

            return stars;
        }

        protected string FormatPrice(object price)
        {
            try
            {
                if (price != null)
                {
                    decimal priceValue = Convert.ToDecimal(price);
                    return priceValue.ToString("N2") + " DH";
                }
                return "0.00 DH";
            }
            catch
            {
                return "0.00 DH";
            }
        }

        protected string FormatDate(object date)
        {
            try
            {
                if (date != null && date != DBNull.Value)
                {
                    DateTime dateValue = Convert.ToDateTime(date);
                    return dateValue.ToString("dd/MM/yyyy");
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}