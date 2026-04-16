<%@ Page Title="Boutique" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs" Inherits="E_comma.Views.Public.Shop" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        * {
            transition: none !important;
        }
        .filter-pills {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-bottom: 30px;
        }
        .filter-pill {
            padding: 8px 20px;
            border-radius: 25px;
            background: #f8f9fa;
            color: #333;
            text-decoration: none;
            border: 2px solid transparent;
        }
        .filter-pill:hover {
            background: #e9ecef;
            color: #007bff;
            text-decoration: none;
        }
        .filter-pill.active {
            background: #007bff;
            color: white;
            border-color: #007bff;
        }
        .product-card {
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            overflow: hidden;
            height: 100%;
        }
        .product-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }
        .product-image {
            width: 100%;
            height: 250px;
            object-fit: cover;
            background: #f5f5f5;
        }
        .product-body {
            padding: 15px;
        }
        .product-brand {
            font-size: 0.85rem;
            color: #666;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .product-title {
            font-size: 1.1rem;
            font-weight: 600;
            margin: 8px 0;
            color: #333;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }
        .product-price {
            font-size: 1.3rem;
            font-weight: bold;
            color: #007bff;
        }
        .star-rating i {
            font-size: 0.9rem;
        }
        .filter-panel {
            background: #fff;
            border: 1px solid #eadcc9;
            border-radius: 16px;
            padding: 16px;
            margin-bottom: 24px;
            box-shadow: 0 12px 24px rgba(67, 27, 0, 0.08);
        }
        .filter-panel .form-label {
            font-weight: 600;
            font-size: 0.9rem;
        }
        .filter-panel .form-control,
        .filter-panel .form-select {
            border-radius: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mb-5">
        <h1 class="section-title">
            <asp:Label ID="lblPageTitle" runat="server" Text="Tous les produits"></asp:Label>
        </h1>
        <p class="text-center text-muted mb-4">
            <asp:Label ID="lblProductCount" runat="server" Text="0"></asp:Label> produit(s) disponible(s)
        </p>
        <div class="filter-panel">
            <div class="row g-3 align-items-end">
                <div class="col-lg-4 col-md-6">
                    <label class="form-label">Recherche</label>
                    <asp:TextBox ID="SearchInput" runat="server" CssClass="form-control" placeholder="Produit ou marque" list="searchSuggestions"></asp:TextBox>
                </div>
                <div class="col-lg-2 col-md-3">
                    <label class="form-label">Prix min</label>
                    <asp:TextBox ID="MinPriceInput" runat="server" CssClass="form-control" TextMode="Number" placeholder="0"></asp:TextBox>
                </div>
                <div class="col-lg-2 col-md-3">
                    <label class="form-label">Prix max</label>
                    <asp:TextBox ID="MaxPriceInput" runat="server" CssClass="form-control" TextMode="Number" placeholder="9999"></asp:TextBox>
                </div>
                <div class="col-lg-4 col-md-6">
                    <label class="form-label">Marque</label>
                    <asp:DropDownList ID="BrandDropdown" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
            </div>
            <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mt-3">
                <div class="form-check">
                    <asp:CheckBox ID="InStockCheck" runat="server" CssClass="form-check-input" />
                    <label class="form-check-label" for="<%= InStockCheck.ClientID %>">Disponibilite: en stock</label>
                </div>
                <div class="d-flex gap-2">
                    <asp:Button ID="ApplyFiltersButton" runat="server" Text="Filtrer" CssClass="btn btn-primary" OnClick="ApplyFiltersButton_Click" />
                    <asp:Button ID="ClearFiltersButton" runat="server" Text="Reinitialiser" CssClass="btn btn-outline-secondary" OnClick="ClearFiltersButton_Click" CausesValidation="false" />
                </div>
            </div>
        </div>
        <!-- Filtres par catégorie (Pills) -->
        <div class="filter-pills">
            <a href="<%# GetCategoryLink(null) %>" class="filter-pill <%# GetActiveCategoryClass(null) %>">
                📦 Tous les produits
            </a>
            <asp:Repeater ID="CategoriesRepeater" runat="server">
                <ItemTemplate>
                    <a href="<%# GetCategoryLink(Eval("Id")) %>" 
                       class="filter-pill <%# GetActiveCategoryClass(Eval("Id")) %>">
                        <%# Eval("Name") %>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Grille de Produits -->
        <div class="row" id="productsGrid">
            <asp:Repeater ID="ProductsRepeater" runat="server">
                <ItemTemplate>
                    <div class="col-lg-3 col-md-4 col-sm-6 mb-4">
                        <div class="product-card">
                            <div style="position: relative;">
                                <%# GetProductBadge(Eval("IsFeatured")) %>
                                <a href="ProductDetail.aspx?id=<%# Eval("Id") %>">
                                    <img src="<%# Eval("MainImageUrl") %>" 
                                         alt="<%# Eval("Name") %>" 
                                         class="product-image"
                                         onerror="this.src='/images/no-image.jpg'">
                                </a>
                            </div>
                            <div class="product-body">
                                <div class="product-brand"><%# Eval("Brand") %></div>
                                <h5 class="product-title">
                                    <a href="ProductDetail.aspx?id=<%# Eval("Id") %>" 
                                       style="text-decoration: none; color: inherit;">
                                        <%# Eval("Name") %>
                                    </a>
                                </h5>
                                <div class="mb-2">
                                    <%# GetStarRating(Eval("AverageRating"), Eval("ReviewCount")) %>
                                </div>
                                <div class="product-price">
                                    <%# FormatPrice(Eval("BasePrice")) %>
                                </div>
                                <a href="ProductDetail.aspx?id=<%# Eval("Id") %>" 
                                   class="btn btn-primary btn-sm mt-3 w-100">
                                    Voir les détails
                                </a>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Message si aucun produit -->
        <asp:Panel ID="NoProductsPanel" runat="server" Visible="false">
            <div class="alert alert-info text-center py-5">
                <i class="fas fa-box-open fa-3x mb-3"></i>
                <h4>Aucun produit trouvé</h4>
                <p class="text-muted">Essayez de sélectionner une autre catégorie</p>
                <a href="Shop.aspx" class="btn btn-primary">Voir tous les produits</a>
            </div>
        </asp:Panel>
    </div>

    <script>
        // Animation douce au chargement
        document.addEventListener('DOMContentLoaded', function () {
            // Smooth scroll pour les liens de catégories (optionnel)
        });
    </script>
</asp:Content>
