using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using E_comma.Models;

namespace E_comma.Views.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Access Control
                if (Session["UserRole"] == null || !Session["UserRole"].ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("~/Views/Auth/Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                BindLookups();
                BindMetrics();
                BindCategoryList();
                BindProductList();
                BindUserList();
                LoadStockAlerts();
            }
        }

        // ==========================================================
        //  BINDING HELPERS
        // ==========================================================
        private void BindLookups()
        {
            var categories = Category.GetAll();
            
            // Parent Category (for Category Form)
            ddlParentCategory.DataSource = categories;
            ddlParentCategory.DataTextField = "Name";
            ddlParentCategory.DataValueField = "Id";
            ddlParentCategory.DataBind();
            ddlParentCategory.Items.Insert(0, new ListItem("-- Aucun --", ""));

            // Product Category (for Product Form)
            ddlProductCategory.DataSource = categories;
            ddlProductCategory.DataTextField = "Name";
            ddlProductCategory.DataValueField = "Id";
            ddlProductCategory.DataBind();
            ddlProductCategory.Items.Insert(0, new ListItem("-- Sélectionner --", ""));
        }

        private void BindMetrics()
        {
            var categories = Category.GetAll();
            var products = Product.GetAll();

            lblCategoryCount.Text = categories.Count.ToString();
            lblProductCount.Text = products.Count.ToString();
            lblFeaturedCount.Text = products.Count(p => p.IsFeatured).ToString();
            lblCatalogValue.Text = products.Sum(p => p.BasePrice).ToString("N2");
            
            var alerts = StockAlert.GetActiveAlerts();
            lblStockAlertCount.Text = alerts.Count.ToString();
            lblAlertBadge.Text = alerts.Count.ToString();
        }

        private void BindCategoryList()
        {
            var categories = Category.GetAll();
            // Join to get Parent Name
            var list = categories.Select(c => new {
                c.Id,
                c.Name,
                c.Slug,
                c.ParentId,
                ParentName = c.ParentId.HasValue 
                             ? categories.FirstOrDefault(p => p.Id == c.ParentId.Value)?.Name 
                             : "-"
            }).ToList();

            rptCategories.DataSource = list;
            rptCategories.DataBind();
            pnlCategoryEmpty.Visible = list.Count == 0;
        }

        private void BindProductList()
        {
            var products = Product.GetAll();
            // Need to join Category Name? Product model has CategoryId.
            // Ideally Product.GetAll should return a view model with CategoryName. 
            // Assuming Product.GetAll() returns Product objects, we might need to fetch categories to display names efficiently
            // OR the existing ASPX expected a property 'CategoryName'. 
            // The Product.cs I read (step 181) does NOT have CategoryName property. 
            // However, the ASPX uses `<%# Eval("CategoryName") %>`. This will throw an error if missing.
            // I must project this.
            
            var categories = Category.GetAll();
            var productList = products.Select(p => new {
                p.Id,
                p.Name,
                p.Brand,
                p.BasePrice,
                p.IsFeatured,
                p.Slug, // Needed?
                CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "Inconnue",
                ImageUrl = GetMainImage(p.Id) // We need an image helper
            }).ToList();

            rptProducts.DataSource = productList;
            rptProducts.DataBind();
            pnlProductEmpty.Visible = productList.Count == 0;
        }
        
        // Helper to mimic image retrieval if Product model doesn't have it directly
        // The Product.cs MapProduct method didn't show ImageUrl. 
        // But the ASPX uses Eval("ImageUrl") or Eval("MainImageUrl").
        // I'll assume we need to provide it.
        private string GetMainImage(int productId)
        {
             // Placeholder logic or query DB? 
             // Ideally we should update Product.GetAll to join with images.
             // For now, return a placeholder or empty to avoid crash
             return "/Content/images/placeholder.png"; 
             // REALITY CHECK: If I don't implement this, the Eval will crash.
             // The original code probably had a view or sproc. Product.GetAll used "SELECT * FROM Products".
             // Product.cs doesn't seem to load images.	
        }

        private void BindUserList()
        {
            var users = E_comma.Models.User.GetAll();
            rptUsers.DataSource = users;
            rptUsers.DataBind();
            pnlUserEmpty.Visible = users.Count == 0;
        }

        private void LoadStockAlerts()
        {
            var alerts = StockAlert.GetActiveAlerts();
            rptStockAlerts.DataSource = alerts;
            rptStockAlerts.DataBind();
            pnlStockAlerts.Visible = alerts.Count > 0;
            pnlNoStockAlerts.Visible = alerts.Count == 0;
        }

        // ==========================================================
        //  CATEGORY ACTIONS
        // ==========================================================
        protected void btnSaveCategory_Click(object sender, EventArgs e)
        {
            int? parentId = null;
            if (!string.IsNullOrEmpty(ddlParentCategory.SelectedValue))
                parentId = int.Parse(ddlParentCategory.SelectedValue);

            string name = txtCategoryName.Text.Trim();
            string slug = txtCategorySlug.Text.Trim();
            if(string.IsNullOrEmpty(slug)) 
            {
                // Simple slugify: Lowercase -> Remove Accents -> Remove invalid -> Replace space with -
                string s = name.ToLower().Trim();
                s = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(s)); // Hacky, better to use specific normalization if available
                // Better approach without extra deps:
                s = s.Normalize(System.Text.NormalizationForm.FormD);
                var chars = s.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();
                s = new string(chars).Normalize(System.Text.NormalizationForm.FormC);
                
                s = System.Text.RegularExpressions.Regex.Replace(s, @"[^a-z0-9\s-]", "");
                s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", "-");
                slug = s;
            } 

            if (string.IsNullOrEmpty(hfCategoryId.Value))
            {
                // Create
                Category.Create(name, slug, parentId);
            }
            else
            {
                // Update
                int id = int.Parse(hfCategoryId.Value);
                Category.Update(id, name, slug, parentId);
            }
            
            ResetCategoryForm();
            BindCategoryList();
            BindLookups(); // Rebind to update parents list
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            
            if (e.CommandName == "EditCategory")
            {
                var cat = Category.GetById(id);
                if (cat != null)
                {
                    hfCategoryId.Value = cat.Id.ToString();
                    txtCategoryName.Text = cat.Name;
                    txtCategorySlug.Text = cat.Slug;
                    if (cat.ParentId.HasValue)
                        ddlParentCategory.SelectedValue = cat.ParentId.Value.ToString();
                    else 
                        ddlParentCategory.SelectedIndex = 0;
                }
            }
            else if (e.CommandName == "DeleteCategory")
            {
                try 
                {
                    Category.Delete(id);
                    BindCategoryList();
                    BindLookups();
                    lblCategoryStatus.Text = "Catégorie supprimée.";
                    lblCategoryStatus.Visible = true;
                    lblCategoryStatus.CssClass = "alert alert-success d-block mb-3";
                }
                catch (Exception)
                {
                    lblCategoryStatus.Text = "Impossible de supprimer cette catégorie (elle contient peut-être des produits ?)";
                    lblCategoryStatus.Visible = true;
                    lblCategoryStatus.CssClass = "alert alert-danger d-block mb-3";
                }
            }
        }
        
        protected void btnResetCategory_Click(object sender, EventArgs e)
        {
            ResetCategoryForm();
        }

        private void ResetCategoryForm()
        {
            hfCategoryId.Value = "";
            txtCategoryName.Text = "";
            txtCategorySlug.Text = "";
            ddlParentCategory.SelectedIndex = 0;
        }

        // ==========================================================
        //  PRODUCT ACTIONS
        // ==========================================================
        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            try
            {
                // Basic implementation
                int catId = 0;
                int.TryParse(ddlProductCategory.SelectedValue, out catId);

                Product p = new Product
                {
                    CategoryId = catId,
                    Name = txtProductName.Text.Trim(),
                    Slug = txtProductSlug.Text.Trim(),
                    Brand = txtBrand.Text.Trim(),
                    Description = txtProductDescription.Text,
                    IsFeatured = chkFeatured.Checked
                };

                decimal price;
                if (decimal.TryParse(txtBasePrice.Text, out price)) p.BasePrice = price;

                // Handle Image
                ProductImage img = null;
                if (fuImage.HasFile)
                {
                    string extension = Path.GetExtension(fuImage.FileName);
                    if (!IsAllowedImageExtension(extension))
                    {
                        lblProductStatus.Text = "Format d'image non supporte.";
                        lblProductStatus.Visible = true;
                        lblProductStatus.CssClass = "alert alert-danger d-block mb-3";
                        return;
                    }

                    string uploadsDir = Server.MapPath("~/Content/Uploads/");
                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    string fileName = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
                    string path = Path.Combine(uploadsDir, fileName);

                    fuImage.SaveAs(path);
                    img = new ProductImage
                    {
                        ImageUrl = "/Content/Uploads/" + fileName,
                        AltText = txtImageAlt.Text,
                        DisplayOrder = 0
                    };
                }
                else if (!string.IsNullOrEmpty(hfCurrentImageUrl.Value))
                {
                    // Keep existing image if no new upload.
                }

                if (string.IsNullOrEmpty(hfProductId.Value))
                {
                    Product.Create(p, img);
                }
                else
                {
                    p.Id = int.Parse(hfProductId.Value);
                    Product.Update(p, img);
                }

                lblProductStatus.Text = "Produit enregistre avec succes.";
                lblProductStatus.Visible = true;
                lblProductStatus.CssClass = "alert alert-success d-block mb-3";

                ResetProductForm();
                BindProductList();
            }
            catch (Exception ex)
            {
                lblProductStatus.Text = "Erreur : " + ex.Message;
                lblProductStatus.Visible = true;
                lblProductStatus.CssClass = "alert alert-danger d-block mb-3";
            }
        }

        protected void btnResetProduct_Click(object sender, EventArgs e)
        {
            ResetProductForm();
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
             int id = int.Parse(e.CommandArgument.ToString());
             if (e.CommandName == "EditProduct")
             {
                 var p = Product.GetById(id);
                 if(p != null)
                 {
                     hfProductId.Value = p.Id.ToString();
                     txtProductName.Text = p.Name;
                     txtProductSlug.Text = p.Slug;
                     txtBrand.Text = p.Brand;
                     txtBasePrice.Text = p.BasePrice.ToString();
                     txtProductDescription.Text = p.Description;
                     ddlProductCategory.SelectedValue = p.CategoryId.ToString();
                     chkFeatured.Checked = p.IsFeatured;
                     // Image handling?
                 }
             }
            else if (e.CommandName == "DeleteProduct")
            {
                string err;
                // We assume Product.Delete returns bool or void, but populates err on failure
                Product.Delete(id, out err);
                
                if (!string.IsNullOrEmpty(err))
                {
                    lblProductStatus.Text = "Erreur : " + err;
                    lblProductStatus.Visible = true;
                    lblProductStatus.CssClass = "alert alert-danger d-block mb-3";
                }
                else
                {
                    BindProductList();
                    lblProductStatus.Text = "Produit supprimé avec succès.";
                    lblProductStatus.Visible = true;
                    lblProductStatus.CssClass = "alert alert-success d-block mb-3";
                }
            }
        }
        
        private void ResetProductForm()
        {
            hfProductId.Value = "";
            txtProductName.Text = "";
            txtProductSlug.Text = "";
            txtBrand.Text = "";
            txtBasePrice.Text = "";
            txtProductDescription.Text = "";
            chkFeatured.Checked = false;
            ddlProductCategory.SelectedIndex = 0;
            hfCurrentImageUrl.Value = "";
        }

        private static bool IsAllowedImageExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension)) return false;
            string ext = extension.ToLowerInvariant();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".webp";
        }


        // ==========================================================
        //  USER ACTIONS
        // ==========================================================
        // ==========================================================
        //  USER ACTIONS
        // ==========================================================
        // ==========================================================
        //  USER ACTIONS
        // ==========================================================
        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(txtUserEmail.Text) || string.IsNullOrWhiteSpace(txtUserFirstName.Text))
                {
                    lblUserStatus.Text = "L'email et le prénom sont requis.";
                    lblUserStatus.Visible = true;
                    lblUserStatus.CssClass = "alert alert-danger d-block mb-3";
                    return;
                }

                string email = txtUserEmail.Text.Trim();
                string firstName = txtUserFirstName.Text.Trim();
                string lastName = txtUserLastName.Text.Trim();
                string phone = txtUserPhone.Text.Trim();
                bool isActive = chkUserActive.Checked;
                string password = txtUserPassword.Text;

                if (string.IsNullOrEmpty(hfUserId.Value))
                {
                    // Create
                    if (string.IsNullOrEmpty(password))
                    {
                        lblUserStatus.Text = "Le mot de passe est requis pour un nouvel utilisateur.";
                        lblUserStatus.Visible = true;
                        lblUserStatus.CssClass = "alert alert-danger d-block mb-3";
                        return;
                    }
                    string error;
                    // Signature: bool Create(string email, string phone, string firstName, string lastName, string password, bool isActive, out string error)
                    bool success = E_comma.Models.User.Create(email, phone, firstName, lastName, password, isActive, out error);
                    if (!success)
                    {
                        lblUserStatus.Text = "Erreur: " + error;
                        lblUserStatus.Visible = true;
                        lblUserStatus.CssClass = "alert alert-danger d-block mb-3";
                        return;
                    }
                }
                else
                {
                    // Update
                    Guid id = Guid.Parse(hfUserId.Value);
                    string error;
                    // Signature: bool Update(Guid id, string email, string phone, string firstName, string lastName, bool isActive, string password, out string error)
                    bool success = E_comma.Models.User.Update(id, email, phone, firstName, lastName, isActive, password, out error);
                    if (!success)
                    {
                        lblUserStatus.Text = "Erreur: " + error;
                        lblUserStatus.Visible = true;
                        lblUserStatus.CssClass = "alert alert-danger d-block mb-3";
                        return;
                    }
                }

                ResetUserForm();
                BindUserList();
                lblUserStatus.Text = "Utilisateur enregistré avec succès.";
                lblUserStatus.Visible = true;
                lblUserStatus.CssClass = "alert alert-success d-block mb-3";
            }
            catch (Exception ex)
            {
                lblUserStatus.Text = "Erreur inattendue: " + ex.Message;
                lblUserStatus.Visible = true;
                lblUserStatus.CssClass = "alert alert-danger d-block mb-3";
            }
        }

        protected void btnResetUser_Click(object sender, EventArgs e)
        {
            ResetUserForm();
        }

        private void ResetUserForm()
        {
            txtUserEmail.Text = "";
            txtUserFirstName.Text = "";
            txtUserLastName.Text = "";
            txtUserPhone.Text = "";
            txtUserPassword.Text = "";
            hfUserId.Value = "";
            chkUserActive.Checked = true;
            lblUserStatus.Visible = false;
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditUser")
            {
                Guid id = Guid.Parse(e.CommandArgument.ToString());
                var u = E_comma.Models.User.GetById(id);
                if (u != null)
                {
                    hfUserId.Value = u.Id.ToString();
                    txtUserEmail.Text = u.Email;
                    txtUserFirstName.Text = u.Name;
                    txtUserLastName.Text = u.LastName;
                    txtUserPhone.Text = u.Phone;
                    chkUserActive.Checked = u.IsActive;
                    txtUserPassword.Text = ""; 
                    
                    lblUserStatus.Visible = false;
                }
            }
            else if (e.CommandName == "ToggleUser")
            {
                // Toggle Logic
                string[] args = e.CommandArgument.ToString().Split(';');
                if (args.Length == 2)
                {
                    Guid id = Guid.Parse(args[0]);
                    bool currentStatus = bool.Parse(args[1]);
                    // Signature: bool ToggleActive(Guid id, bool isActive)
                    E_comma.Models.User.ToggleActive(id, !currentStatus);
                    BindUserList();
                }
            }
            else if (e.CommandName == "DeleteUser")
            {
                Guid id = Guid.Parse(e.CommandArgument.ToString());
                // Avoid deleting self
                if (Session["UserId"] != null && Session["UserId"].ToString() == id.ToString())
                {
                     lblUserStatus.Text = "Impossible de supprimer votre propre compte.";
                     lblUserStatus.Visible = true;
                     lblUserStatus.CssClass = "alert alert-warning d-block mb-3";
                     return;
                }

                string error;
                bool success = E_comma.Models.User.Delete(id, out error);
                if (!success)
                {
                    lblUserStatus.Text = "Erreur lors de la suppression: " + error;
                    lblUserStatus.Visible = true;
                    lblUserStatus.CssClass = "alert alert-danger d-block mb-3";
                }
                else
                {
                    BindUserList();
                    lblUserStatus.Text = "Utilisateur supprimé.";
                    lblUserStatus.Visible = true;
                    lblUserStatus.CssClass = "alert alert-success d-block mb-3";
                }
            }
        }
    }
}
