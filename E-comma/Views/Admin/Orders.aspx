<%@ Page Title="Commandes" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeFile="Orders.aspx.cs" Inherits="E_comma.Views.Admin.Orders" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .order-hero {
            background: linear-gradient(135deg, #f7f2ff 0%, #eef7ff 100%);
            border-radius: 16px;
            padding: 18px 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            border: 1px solid #e7e3f3;
        }
        .order-hero h3 { margin: 0; color: #1d1b2f; }
        .order-hero .meta { color: #6b6a7e; margin: 0; }
        .orders-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); gap: 16px; }
        .order-admin-card {
            border: 1px solid #ecebf2;
            border-radius: 14px;
            padding: 14px;
            background: #fff;
            box-shadow: 0 10px 26px rgba(25, 21, 51, 0.05);
            transition: transform 0.15s ease, box-shadow 0.15s ease;
        }
        .order-admin-card:hover { transform: translateY(-2px); box-shadow: 0 14px 30px rgba(25, 21, 51, 0.08); }
        .badge-status { border-radius: 999px; padding: 6px 10px; font-size: 0.85rem; text-transform: capitalize; }
        .status-pending { background: #fff3cd; color: #856404; }
        .status-processing { background: #d1ecf1; color: #0c5460; }
        .status-delivered { background: #d4edda; color: #155724; }
        .status-cancelled { background: #f8d7da; color: #721c24; }
        .order-items ul { margin: 0; padding-left: 18px; color: #5a556d; }
        .order-actions .btn { min-width: 110px; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid py-3">
        <div class="order-hero mb-3">
            <div>
                <h3 class="mb-1">Commandes</h3>
                <p class="meta">Suivi des paiements et validation manuelle.</p>
            </div>
            <asp:Label ID="lblStatus" runat="server" Visible="false"></asp:Label>
        </div>

        <asp:Repeater ID="OrdersRepeater" runat="server" OnItemCommand="OrdersRepeater_ItemCommand" OnItemDataBound="OrdersRepeater_ItemDataBound">
            <HeaderTemplate><div class="orders-grid"></HeaderTemplate>
            <ItemTemplate>
                <div class="order-admin-card">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <div class="fw-bold">Commande #<%# Eval("Id") %></div>
                            <small class="text-muted"><%# ((DateTime)Eval("CreatedAt")).ToString("dd/MM/yyyy HH:mm") %></small>
                            <div class="mt-2 small text-muted">
                                <div><strong>Client :</strong> <%# Eval("CustomerName") %></div>
                                <div><strong>Email :</strong> <%# Eval("CustomerEmail") %></div>
                                <div><strong>Telephone :</strong> <%# Eval("CustomerPhone") %></div>
                            </div>
                        </div>
                        <div class="text-end">
                            <asp:Literal ID="ltStatus" runat="server"></asp:Literal>
                            <div class="fw-bold text-dark mt-2">DH <%# string.Format("{0:N2}", Eval("Total")) %></div>
                        </div>
                    </div>
                    <div class="order-items mt-3">
                        <asp:Repeater ID="ItemsRepeater" runat="server">
                            <HeaderTemplate><ul class="mb-2"></HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <%# Eval("ProductName") %> (<%# Eval("Attributes") %>) x <%# Eval("Quantity") %>
                                    <span class="text-muted">— DH <%# string.Format("{0:N2}", Eval("LineTotal")) %></span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate></ul></FooterTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="order-actions mt-3 d-flex gap-2">
                        <asp:Button ID="btnConfirm" runat="server" Text="Confirmer" CommandName="Confirm" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-success btn-sm" />
                        <asp:Button ID="btnCancel" runat="server" Text="Annuler" CommandName="Cancel" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-danger btn-sm" />
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
