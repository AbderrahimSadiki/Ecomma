using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using E_comma.Models;
using E_comma.Helpers;

namespace E_comma.Views.Public
{
    public partial class Shop : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            LoadBrands();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // IMPORTANT : Charger uniquement lors de la première visite (pas sur PostBack)
            LoadCategories();

            if (!IsPostBack)
            {
                ApplyFilterValuesFromQuery();
                LoadProducts();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Désactiver le ViewState pour éviter les rechargements
            this.EnableViewState = false;
        }

        private void LoadCategories()
        {
            try
            {
                var categories = Category.GetAll();
                CategoriesRepeater.DataSource = categories;
                CategoriesRepeater.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur chargement catégories: " + ex.Message);
            }
        }

        private void LoadBrands()
        {
            try
            {
                var brands = ProductExtended.GetBrands();
                BrandDropdown.DataSource = brands;
                BrandDropdown.DataBind();
                BrandDropdown.Items.Insert(0, new ListItem("Toutes les marques", ""));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur chargement marques: " + ex.Message);
            }
        }

        private void LoadProducts()
        {
            try
            {
                int? categoryId = null;
                string search = (Request.QueryString["q"] ?? string.Empty).Trim();
                string brand = (Request.QueryString["brand"] ?? string.Empty).Trim();
                decimal? minPrice = ParseDecimal(Request.QueryString["min"]);
                decimal? maxPrice = ParseDecimal(Request.QueryString["max"]);
                bool inStock = IsInStockFilterActive();

                // Vérifier si une catégorie est sélectionnée
                string categoryValue = Request.QueryString["cat"];
                if (string.IsNullOrEmpty(categoryValue))
                {
                    categoryValue = Request.QueryString["categoryId"];
                }

                if (!string.IsNullOrEmpty(categoryValue))
                {
                    if (int.TryParse(categoryValue, out int catId))
                    {
                        categoryId = catId;

                        // Mettre à jour le titre de la page
                        var category = Category.GetById(catId);
                        if (category != null)
                        {
                            lblPageTitle.Text = category.Name;
                        }
                        else
                        {
                            lblPageTitle.Text = "Tous les produits";
                        }
                    }
                }
                else
                {
                    lblPageTitle.Text = "Tous les produits";
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    lblPageTitle.Text = "Resultats pour \"" + search + "\"";
                }

                // Charger les produits
                var products = ProductExtended.GetAllForShop(categoryId, search, minPrice, maxPrice, brand, inStock ? true : (bool?)null);

                if (products != null && products.Count > 0)
                {
                    ProductsRepeater.DataSource = products;
                    ProductsRepeater.DataBind();
                    lblProductCount.Text = products.Count.ToString();
                    NoProductsPanel.Visible = false;
                }
                else
                {
                    NoProductsPanel.Visible = true;
                    lblProductCount.Text = "0";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur chargement produits: " + ex.Message);
                NoProductsPanel.Visible = true;
                lblProductCount.Text = "0";
            }
        }

        // Helper methods pour le rendu dans les répéteurs
        private void ApplyFilterValuesFromQuery()
        {
            SearchInput.Text = Request.QueryString["q"] ?? string.Empty;
            MinPriceInput.Text = Request.QueryString["min"] ?? string.Empty;
            MaxPriceInput.Text = Request.QueryString["max"] ?? string.Empty;
            InStockCheck.Checked = IsInStockFilterActive();

            MinPriceInput.Attributes["step"] = "0.01";
            MaxPriceInput.Attributes["step"] = "0.01";
            MinPriceInput.Attributes["min"] = "0";
            MaxPriceInput.Attributes["min"] = "0";

            string selectedBrand = (Request.QueryString["brand"] ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(selectedBrand))
            {
                var item = BrandDropdown.Items.FindByValue(selectedBrand);
                if (item != null)
                {
                    BrandDropdown.SelectedValue = selectedBrand;
                }
            }
        }

        private bool IsInStockFilterActive()
        {
            string stockValue = (Request.QueryString["stock"] ?? string.Empty).Trim();
            return stockValue == "1" || stockValue.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal current))
                return current;

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal invariant))
                return invariant;

            return null;
        }

        protected void ApplyFiltersButton_Click(object sender, EventArgs e)
        {
            string category = Request.QueryString["cat"];
            if (string.IsNullOrEmpty(category))
            {
                category = Request.QueryString["categoryId"];
            }
            string search = (SearchInput.Text ?? string.Empty).Trim();
            string brand = BrandDropdown.SelectedValue ?? string.Empty;
            string minPrice = (MinPriceInput.Text ?? string.Empty).Trim();
            string maxPrice = (MaxPriceInput.Text ?? string.Empty).Trim();

            var query = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(category))
                query["cat"] = category;
            if (!string.IsNullOrEmpty(search))
                query["q"] = search;
            if (!string.IsNullOrEmpty(brand))
                query["brand"] = brand;
            if (!string.IsNullOrEmpty(minPrice))
                query["min"] = minPrice;
            if (!string.IsNullOrEmpty(maxPrice))
                query["max"] = maxPrice;
            if (InStockCheck.Checked)
                query["stock"] = "1";

            string url = "Shop.aspx";
            string qs = query.ToString();
            if (!string.IsNullOrEmpty(qs))
                url += "?" + qs;

            RedirectHelper.SafeRedirect(url);
        }

        protected void ClearFiltersButton_Click(object sender, EventArgs e)
        {
            RedirectHelper.SafeRedirect("Shop.aspx");
        }

        protected string GetCategoryLink(object categoryId)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (categoryId != null)
                query["cat"] = categoryId.ToString();

            string search = (Request.QueryString["q"] ?? string.Empty).Trim();
            string brand = (Request.QueryString["brand"] ?? string.Empty).Trim();
            string minPrice = (Request.QueryString["min"] ?? string.Empty).Trim();
            string maxPrice = (Request.QueryString["max"] ?? string.Empty).Trim();

            if (!string.IsNullOrEmpty(search))
                query["q"] = search;
            if (!string.IsNullOrEmpty(brand))
                query["brand"] = brand;
            if (!string.IsNullOrEmpty(minPrice))
                query["min"] = minPrice;
            if (!string.IsNullOrEmpty(maxPrice))
                query["max"] = maxPrice;
            if (IsInStockFilterActive())
                query["stock"] = "1";

            string qs = query.ToString();
            return string.IsNullOrEmpty(qs) ? "Shop.aspx" : "Shop.aspx?" + qs;
        }

        protected string GetActiveCategoryClass(object categoryId)
        {
            try
            {
                string selectedCat = Request.QueryString["cat"];
                if (string.IsNullOrEmpty(selectedCat))
                {
                    selectedCat = Request.QueryString["categoryId"];
                }

                // Si aucune catégorie n'est sélectionnée et categoryId est null (Tous les produits)
                if (string.IsNullOrEmpty(selectedCat) && categoryId == null)
                {
                    return "active";
                }

                // Si une catégorie est sélectionnée
                if (!string.IsNullOrEmpty(selectedCat) && categoryId != null)
                {
                    return selectedCat == categoryId.ToString() ? "active" : "";
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        protected string GetProductBadge(object isFeatured)
        {
            try
            {
                if (isFeatured != null && Convert.ToBoolean(isFeatured))
                {
                    return "<span class='badge bg-warning text-dark position-absolute top-0 start-0 m-2' style='z-index: 10;'>⭐ Vedette</span>";
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        protected string GetStarRating(object avgRating, object reviewCount)
        {
            try
            {
                double rating = avgRating != null ? Convert.ToDouble(avgRating) : 0;
                int count = reviewCount != null ? Convert.ToInt32(reviewCount) : 0;

                string stars = "<div class='star-rating d-inline-block'>";

                for (int i = 1; i <= 5; i++)
                {
                    if (i <= rating)
                    {
                        stars += "<i class='fas fa-star text-warning'></i>";
                    }
                    else if (i - rating < 1 && i - rating > 0)
                    {
                        stars += "<i class='fas fa-star-half-alt text-warning'></i>";
                    }
                    else
                    {
                        stars += "<i class='far fa-star text-warning'></i>";
                    }
                }

                stars += $"</div> <span class='text-muted ms-1'>({count})</span>";

                return stars;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur GetStarRating: " + ex.Message);
                return "<span class='text-muted'>(0)</span>";
            }
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
    }
}
