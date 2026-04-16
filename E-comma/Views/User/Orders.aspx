<%@ Page Title="Mes commandes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Orders.aspx.cs" Inherits="E_comma.Views.User.Orders" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .orders-hero {
            background: linear-gradient(120deg, #fdf7ec 0%, #f1e3cf 100%);
            border-radius: 18px;
            padding: 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
        }
        .order-card {
            border: 1px solid #f0e8dc;
            border-radius: 14px;
            box-shadow: 0 8px 28px rgba(0,0,0,0.04);
            padding: 16px;
            background: #fff;
        }
        .order-header { display: flex; justify-content: space-between; align-items: center; }
        .order-items { margin-top: 10px; }
        .order-item { display: grid; grid-template-columns: 80px 1fr 100px 90px; gap: 10px; align-items: center; padding: 10px 0; border-bottom: 1px solid #f3eee6; }
        .order-item:last-child { border-bottom: 0; }
        .order-item img { width: 80px; height: 80px; object-fit: cover; border-radius: 10px; border: 1px solid #f0e8dc; }
        .order-item .name { margin: 0; font-weight: 700; color: #2c1c10; }
        .order-item .attrs { margin: 0; color: #9a8565; font-size: 0.9rem; }
        .order-item .brand { margin: 0; color: #7c674b; font-size: 0.9rem; }
        .badge-status { border-radius: 999px; padding: 6px 10px; font-size: 0.85rem; }
        .status-pending { background: #fff3cd; color: #856404; }
        .status-processing { background: #d1ecf1; color: #0c5460; }
        .status-delivered { background: #d4edda; color: #155724; }
        .status-cancelled { background: #f8d7da; color: #721c24; }
        .cart-empty {
            background: #fff;
            border: 1px dashed #d2b07a;
            border-radius: 14px;
            padding: 32px;
            text-align: center;
            color: #7a6140;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <div class="orders-hero mb-4">
            <div>
                <h2>Mes commandes</h2>
                <p class="mb-0">Suivez l'etat de vos achats.</p>
            </div>
            <div class="badge bg-dark text-white px-3 py-2">
                <asp:Label ID="lblOrderCount" runat="server" Text="0"></asp:Label> commande(s)
            </div>
        </div>

        <asp:Label ID="lblStatus" runat="server" Visible="false"></asp:Label>

        <asp:Panel ID="OrdersPanel" runat="server">
            <asp:Repeater ID="OrdersRepeater" runat="server" OnItemCommand="OrdersRepeater_ItemCommand" OnItemDataBound="OrdersRepeater_ItemDataBound">
                <ItemTemplate>
                    <div class="order-card mb-3">
                        <div class="order-header">
                            <div>
                                <div class="fw-bold">Commande #<%# Eval("Id") %></div>
                                <small class="text-muted"><%# ((DateTime)Eval("CreatedAt")).ToString("dd/MM/yyyy HH:mm") %></small>
                            </div>
                            <div class="text-end">
                                <asp:Literal ID="ltStatus" runat="server"></asp:Literal>
                                <asp:Button ID="btnCancel" runat="server" Text="Annuler" CommandName="Cancel" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-outline-danger btn-sm mt-2" Visible="false" CausesValidation="false" />
                            </div>
                        </div>
                        <div class="text-muted mt-1">Total : <span class="fw-bold text-dark">DH <%# string.Format("{0:N2}", Eval("Total")) %></span></div>
                        <div class="order-items">
                            <asp:Repeater ID="ItemsRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="order-item">
                                        <img src="<%# Eval("MainImageUrl") %>" alt="<%# Eval("ProductName") %>" />
                                        <div>
                                            <p class="name"><%# Eval("ProductName") %></p>
                                            <p class="brand"><%# Eval("Brand") %></p>
                                            <p class="attrs"><%# Eval("Attributes") %></p>
                                        </div>
                                        <div class="text-end">
                                            <div class="fw-bold">DH <%# string.Format("{0:N2}", Eval("UnitPrice")) %></div>
                                            <small class="text-muted">x <%# Eval("Quantity") %></small>
                                        </div>
                                        <div class="text-end fw-bold text-dark">DH <%# string.Format("{0:N2}", Eval("LineTotal")) %></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>

        <asp:Panel ID="EmptyPanel" runat="server" Visible="false" CssClass="cart-empty mt-3">
            <i class="fas fa-box-open fa-2x mb-2"></i>
            <h4>Aucune commande</h4>
            <p>Vous n'avez pas encore passe de commande.</p>
            <a href="/Views/Public/Shop.aspx" class="btn btn-primary mt-2">Aller à la boutique</a>
        </asp:Panel>
    </div>
</asp:Content>
