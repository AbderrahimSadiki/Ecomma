<%@ Page Title="Ma Liste de Souhaits" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Wishlist.aspx.cs" Inherits="E_comma.Views.User.Wishlist" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <style>
            .wishlist-item {
                border: 1px solid #e0e0e0;
                border-radius: 8px;
                padding: 15px;
                margin-bottom: 15px;
                background: #fff;
            }

            .wishlist-item:hover {
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            }

            .product-image {
                width: 80px;
                height: 80px;
                object-fit: cover;
                border-radius: 4px;
            }

            .remove-btn {
                color: #dc3545;
                border: none;
                background: none;
                font-size: 18px;
                cursor: pointer;
            }

            .remove-btn:hover {
                color: #c82333;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container mt-4">
            <h2 class="mb-4">Ma Liste de Souhaits</h2>

            <asp:Panel ID="LoginRequiredPanel" runat="server" Visible="false" CssClass="alert alert-warning">
                <h4>Connexion requise</h4>
                <p>Vous devez être connecté pour voir votre liste de souhaits.</p>
                <a href="../Auth/Login.aspx" class="btn btn-primary">Se connecter</a>
            </asp:Panel>

            <asp:Panel ID="WishlistPanel" runat="server">
                <asp:Repeater ID="WishlistRepeater" runat="server" OnItemCommand="WishlistRepeater_ItemCommand">
                    <ItemTemplate>
                        <div class="wishlist-item d-flex align-items-center">
                            <img src='<%# Eval("MainImageUrl") %>' alt='<%# Eval("ProductName") %>'
                                class="product-image me-3" />
                            <div class="flex-grow-1">
                                <h5 class="mb-1">
                                    <%# Eval("ProductName") %>
                                </h5>
                                <p class="text-muted mb-1">
                                    <%# Eval("Brand") %>
                                </p>
                                <p class="mb-1"><strong>
                                        <%# Eval("BasePrice", "{0:N2}" ) %> DH
                                    </strong></p>
                            </div>
                            <div class="d-flex flex-column align-items-end">
                                <asp:Button ID="btnRemove" runat="server" CommandName="Remove"
                                    CommandArgument='<%# Eval("ProductId") %>' Text="✕" CssClass="remove-btn mb-2"
                                    ToolTip="Retirer de la liste" />
                                <a href='../Public/ProductDetail.aspx?id=<%# Eval("ProductId") %>'
                                    class="btn btn-primary btn-sm">Voir le produit</a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="EmptyWishlistPanel" runat="server" Visible="false" CssClass="text-center py-5">
                    <i class="fas fa-heart-broken fa-3x text-muted mb-3"></i>
                    <h4>Votre liste de souhaits est vide</h4>
                    <p>Découvrez nos produits et ajoutez-les à votre liste de souhaits !</p>
                    <a href="../Public/Shop.aspx" class="btn btn-primary">Voir les produits</a>
                </asp:Panel>
            </asp:Panel>
        </div>

        <script type="text/javascript">
            function removeFromWishlist(productId) {
                if (confirm('Êtes-vous sûr de vouloir retirer ce produit de votre liste de souhaits ?')) {
                    // AJAX call to remove from wishlist
                    $.ajax({
                        type: "POST",
                        url: "Wishlist.aspx/RemoveFromWishlist",
                        data: JSON.stringify({ productId: productId }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response.d.success) {
                                location.reload();
                            } else {
                                alert('Erreur: ' + response.d.message);
                            }
                        },
                        error: function () {
                            alert('Erreur lors de la suppression');
                        }
                    });
                }
            }
        </script>
    </asp:Content>