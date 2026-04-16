<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockManagement.aspx.cs" 
    Inherits="E_comma.Views.Admin.StockManagement" MasterPageFile="~/Admin.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container py-4">
        <h1 class="mb-4">Gestion des Stocks</h1>

        <!-- Alertes de stock -->
        <div class="card mb-4">
            <div class="card-header bg-warning">
                <h5 class="mb-0">
                    <i class="fas fa-exclamation-triangle"></i>
                    Alertes de stock (<asp:Label ID="lblAlertCount" runat="server" Text="0" />)
                </h5>
            </div>
            <div class="card-body">
                <asp:Panel ID="AlertsPanel" runat="server" Visible="false">
                    <asp:Repeater ID="rptAlerts" runat="server">
                        <ItemTemplate>
                            <div class="alert alert-warning mb-2">
                                <strong><%# Eval("ProductName") %></strong> - <%# Eval("VariantName") %>
                                <br />
                                Stock actuel: <strong><%# Eval("CurrentStock") %></strong> 
                                (Seuil: <%# Eval("ThresholdQuantity") %>)
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:Panel>
                <asp:Panel ID="NoAlertsPanel" runat="server" Visible="true">
                    <p class="text-muted mb-0">Aucune alerte de stock.</p>
                </asp:Panel>
            </div>
        </div>

        <!-- Ajustement de stock -->
        <div class="card mb-4">
            <div class="card-header">
                <h5 class="mb-0">Ajuster le stock</h5>
            </div>
            <div class="card-body">
                <asp:Label ID="lblStockStatus" runat="server" Visible="false" CssClass="alert" />

                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label">Produit / Variante</label>
                        <asp:DropDownList ID="ddlVariant" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Type de mouvement</label>
                        <asp:DropDownList ID="ddlMovementType" runat="server" CssClass="form-select">
                            <asp:ListItem Value="IN">Entrée (IN)</asp:ListItem>
                            <asp:ListItem Value="OUT">Sortie (OUT)</asp:ListItem>
                            <asp:ListItem Value="ADJUSTMENT">Ajustement</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Quantité</label>
                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" 
                            TextMode="Number" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Notes</label>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="col-12">
                        <asp:Button ID="btnAddStock" runat="server" Text="Enregistrer" 
                            CssClass="btn btn-primary" OnClick="btnAddStock_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Liste des stocks -->
        <div class="card mb-4">
            <div class="card-header">
                <h5 class="mb-0">État des stocks</h5>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead>
                            <tr>
                                <th>Produit</th>
                                <th>Variante</th>
                                <th>SKU</th>
                                <th>Stock</th>
                                <th>Seuil</th>
                                <th>Prix</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptStock" runat="server" OnItemCommand="rptStock_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("ProductName") %></td>
                                        <td><%# Eval("Attributes") %></td>
                                        <td><code><%# Eval("SKU") %></code></td>
                                        <td>
                                            <span class="<%# GetStockStatusClass(Eval("StockQuantity"), Eval("ThresholdQuantity")) %>">
                                                <%# Eval("StockQuantity") %>
                                            </span>
                                        </td>
                                        <td><%# Eval("ThresholdQuantity") %></td>
                                        <td><%# String.Format("{0:N2} DH", Eval("Price")) %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Historique des mouvements -->
        <div class="card">
            <div class="card-header">
                <h5 class="mb-0">Mouvements récents</h5>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-sm">
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>Produit</th>
                                <th>Type</th>
                                <th>Quantité</th>
                                <th>Stock avant</th>
                                <th>Stock après</th>
                                <th>Référence</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptMovements" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# String.Format("{0:dd/MM/yyyy HH:mm}", Eval("CreatedAt")) %></td>
                                        <td>
                                            <%# Eval("ProductName") %>
                                            <small class="text-muted">- <%# Eval("VariantName") %></small>
                                        </td>
                                        <td><%# GetMovementTypeLabel(Eval("MovementType")) %></td>
                                        <td><%# Eval("Quantity") %></td>
                                        <td><%# Eval("PreviousStock") %></td>
                                        <td><%# Eval("NewStock") %></td>
                                        <td><small><%# Eval("Reference") %></small></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>