<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryManagement.aspx.cs" 
    Inherits="E_comma.Views.Admin.DeliveryManagement" MasterPageFile="~/Admin.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <h1 class="mb-4">Gestion de la Livraison</h1>

        <!-- Formulaire d'ajout/modification -->
        <div class="card mb-4">
            <div class="card-header">
                <h5 class="mb-0">Ajouter / Modifier un mode de livraison</h5>
            </div>
            <div class="card-body">
                <asp:Label ID="lblDeliveryStatus" runat="server" Visible="false" CssClass="alert" />
                <asp:HiddenField ID="hfDeliveryMethodId" runat="server" />

                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label">Nom *</label>
                        <asp:TextBox ID="txtMethodName" runat="server" CssClass="form-control" 
                            placeholder="Ex: Livraison Standard" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Prix (DH) *</label>
                        <asp:TextBox ID="txtMethodPrice" runat="server" CssClass="form-control" 
                            TextMode="Number" step="0.01" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Délai (jours) *</label>
                        <asp:TextBox ID="txtEstimatedDays" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="3" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Description</label>
                        <asp:TextBox ID="txtMethodDescription" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="2" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Ordre d'affichage</label>
                        <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" />
                    </div>
                    <div class="col-md-3">
                        <div class="form-check mt-4">
                            <asp:CheckBox ID="chkMethodActive" runat="server" CssClass="form-check-input" 
                                Checked="true" />
                            <label class="form-check-label">Actif</label>
                        </div>
                    </div>
                    <div class="col-12">
                        <asp:Button ID="btnSaveDeliveryMethod" runat="server" Text="Enregistrer" 
                            CssClass="btn btn-primary" OnClick="btnSaveDeliveryMethod_Click" />
                        <asp:Button ID="btnResetDeliveryMethod" runat="server" Text="Annuler" 
                            CssClass="btn btn-secondary" OnClick="btnResetDeliveryMethod_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Liste des modes de livraison -->
        <div class="card">
            <div class="card-header">
                <h5 class="mb-0">Modes de livraison</h5>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table table-hover">
                        <thead>
                            <tr>
                                <th>Ordre</th>
                                <th>Nom</th>
                                <th>Prix</th>
                                <th>Délai</th>
                                <th>Statut</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptDeliveryMethods" runat="server" OnItemCommand="rptDeliveryMethods_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("DisplayOrder") %></td>
                                        <td>
                                            <strong><%# Eval("Name") %></strong>
                                            <br />
                                            <small class="text-muted"><%# Eval("Description") %></small>
                                        </td>
                                        <td><%# FormatPrice(Eval("Price")) %></td>
                                        <td><%# FormatDays(Eval("EstimatedDays")) %></td>
                                        <td><%# GetStatusBadge(Eval("IsActive")) %></td>
                                        <td>
                                            <asp:LinkButton ID="btnEdit" runat="server" 
                                                CommandName="EditMethod" 
                                                CommandArgument='<%# Eval("Id") %>' 
                                                CssClass="btn btn-sm btn-primary">
                                                <i class="fas fa-edit"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnToggle" runat="server" 
                                                CommandName="ToggleMethod" 
                                                CommandArgument='<%# Eval("Id") %>' 
                                                CssClass="btn btn-sm btn-warning"
                                                OnClientClick="return confirm('Changer le statut ?');">
                                                <i class="fas fa-power-off"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnDelete" runat="server" 
                                                CommandName="DeleteMethod" 
                                                CommandArgument='<%# Eval("Id") %>' 
                                                CssClass="btn btn-sm btn-danger"
                                                OnClientClick="return confirm('Supprimer ce mode de livraison ?');">
                                                <i class="fas fa-trash"></i>
                                            </asp:LinkButton>
                                        </td>
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