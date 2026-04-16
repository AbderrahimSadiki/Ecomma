<%@ Page Title="Mon Panier" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Cart.aspx.cs" Inherits="E_comma.Views.Public.Cart" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <link href="../../Content/css/cart-custom.css" rel="stylesheet" />

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="container py-5">
            <!-- Header Section -->
            <div class="cart-header-section">
                <div>
                    <h1 class="cart-header-title">Votre panier</h1>
                    <p class="cart-header-subtitle">Retrouvez ici tous les articles ajoutés depuis la boutique.</p>
                </div>
                <span class="cart-item-count">Panier</span>
            </div>

            <asp:Panel ID="pnlEmptyCart" runat="server" Visible="false" CssClass="text-center">
                <div class="alert alert-info">
                    <h4>Votre panier est vide.</h4>
                    <p>Découvrez nos produits et commencez vos achats !</p>
                    <a href="Shop.aspx" class="btn btn-primary mt-3">Retour à la boutique</a>
                </div>
            </asp:Panel>

            <asp:UpdatePanel ID="updCartContent" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnlCartContent" runat="server">
                        <div class="cart-container">
                            <!-- Left Column: Items -->
                            <div class="cart-items-column">
                                <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
                                    <ItemTemplate>
                                        <div class="cart-item-card">
                                            <img src='<%# Eval("ProductImage") %>' class="cart-item-image"
                                                alt="Product Image">
                                            <div class="cart-item-details">
                                                <h3 class="cart-item-title">
                                                    <%# Eval("ProductName") %>
                                                </h3>
                                                <p class="cart-item-brand">
                                                    <%# Eval("Attributes") %>
                                                </p>
                                                <span class="cart-item-status">En stock</span>
                                                <div class="mt-2">
                                                    <span class="cart-item-price">
                                                        <%# Eval("Price", "{0:N2} DH" ) %>
                                                    </span>
                                                </div>
                                            </div>

                                            <!-- Quantity and Actions -->
                                            <div class="cart-item-actions">
                                                <div class="d-flex align-items-center mb-2">
                                                    <!-- Minus Button -->
                                                    <asp:LinkButton ID="btnMinus" runat="server" CommandName="Decrease"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        CssClass="btn btn-sm btn-light border rounded-circle me-2">
                                                        <i class="fa fa-minus"></i>
                                                    </asp:LinkButton>

                                                    <input type="text" class="cart-quantity-input"
                                                        value='<%# Eval("Quantity") %>' readonly>

                                                    <!-- Plus Button -->
                                                    <asp:LinkButton ID="btnPlus" runat="server" CommandName="Increase"
                                                        CommandArgument='<%# Eval("Id") %>'
                                                        CssClass="btn btn-sm btn-light border rounded-circle ms-2">
                                                        <i class="fa fa-plus"></i>
                                                    </asp:LinkButton>
                                                </div>

                                                <!-- Remove Link -->
                                                <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove"
                                                    CommandArgument='<%# Eval("Id") %>' CssClass="btn-remove">
                                                    Supprimer
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <!-- Right Column: Summary -->
                            <div class="cart-summary-column">
                                <div class="cart-summary-card">
                                    <div class="summary-row">
                                        <span>Sous-total</span>
                                        <!-- We use a placeholder here because the backend only updates the total. 
                                         Ideally we would bind this too, but we can just assume subtotal = total for now 
                                         or let the user know. The backend updates 'cartTotalDisplay'. 
                                         I'll just duplicate the ID logic? No, ID must be unique.
                                         I will just put a static text or JS to copy it? 
                                         Let's just leave it static or remove it if it's confusing.
                                         Actually, the image has "Sous-total" and "Total".
                                         I will just use the Total for both visually or hide subtotal if it's redundant.
                                         But to match the image, I'll put a placeholder.
                                    -->
                                        <span>--</span>
                                    </div>
                                    <div class="summary-row">
                                        <span>Livraison</span>
                                        <span>DH 0.00</span>
                                    </div>
                                    <div class="summary-total-row">
                                        <span>Total</span>
                                        <span runat="server" id="cartTotalDisplay">DH 0.00</span>
                                    </div>
                                    <p class="summary-note">Les taxes et frais seront calculés à l'étape de paiement.
                                    </p>

                                    <a href="Checkout.aspx" class="btn-checkout">Procéder au paiement</a>
                                    <a href="Shop.aspx" class="btn-continue">Continuer mes achats</a>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Content>