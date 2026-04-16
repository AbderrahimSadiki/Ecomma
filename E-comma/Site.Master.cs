using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace E_comma
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                LoadSearchSuggestions();
            }
            
            UpdateCartTotal();
        }

        private void LoadCategories()
        {
            try
            {
                // Récupérer toutes les catégories
                List<Models.Category> categories = Models.Category.GetAll();

                if (categories != null && categories.Count > 0)
                {
                    // Préparer les données avec compteur de produits
                    var categoriesWithCount = new List<dynamic>();
                    
                    foreach (var cat in categories)
                    {
                        int productCount = Models.Product.GetByCategory(cat.Id).Count;
                        categoriesWithCount.Add(new
                        {
                            Id = cat.Id,
                            Name = cat.Name,
                            ProductCount = productCount
                        });
                    }

                    // Bind dropdown dans la barre de recherche
                    CategoryDropdown.DataSource = categories;
                    CategoryDropdown.DataTextField = "Name";
                    CategoryDropdown.DataValueField = "Id";
                    CategoryDropdown.DataBind();
                    CategoryDropdown.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Category", ""));

                    // Bind repeater catégories desktop
                    CategoriesRepeater.DataSource = categoriesWithCount;
                    CategoriesRepeater.DataBind();

                    // Bind repeater catégories mobile
                    MobileCategoriesRepeater.DataSource = categoriesWithCount;
                    MobileCategoriesRepeater.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur lors du chargement des catégories: " + ex.Message);
            }
        }

        private void LoadSearchSuggestions()
        {
            try
            {
                var suggestions = Models.ProductExtended.GetSearchSuggestions(12);
                SearchSuggestionsRepeater.DataSource = suggestions;
                SearchSuggestionsRepeater.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur lors du chargement des suggestions: " + ex.Message);
            }
        }

        private void UpdateCartTotal()
        {
            try
            {
                if (cartTotal == null)
                {
                    return; // Le contrôle n'est pas disponible sur cette page
                }

                if (Request.IsAuthenticated && Session["UserId"] != null)
                {
                    Guid userId = (Guid)Session["UserId"];
                    decimal total = Models.CartItem.GetTotal(userId);
                    cartTotal.InnerText = $"DH {total:N2}";
                }
                else
                {
                    cartTotal.InnerText = "DH 0.00";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur UpdateCartTotal: " + ex.Message);
                if (cartTotal != null)
                {
                    cartTotal.InnerText = "DH 0.00";
                }
            }
        }
    }
}
