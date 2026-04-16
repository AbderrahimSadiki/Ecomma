<%@ Page Title="Mon profil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="E_comma.Views.User.UserProfile" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .profile-card {
            background: #fff;
            border: 1px solid #e9e5dd;
            border-radius: 14px;
            box-shadow: 0 10px 28px rgba(0,0,0,0.05);
            padding: 18px;
        }
        .profile-hero {
            background: linear-gradient(135deg, #fdf6eb 0%, #f0e0c8 100%);
            border-radius: 16px;
            padding: 18px 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <div class="profile-hero mb-4">
            <div>
                <h3 class="mb-1">Mon profil</h3>
                <p class="mb-0 text-muted">Mettez a jour vos informations personnelles (email non modifiable).</p>
            </div>
            <asp:Label ID="lblStatus" runat="server" Visible="false"></asp:Label>
        </div>

        <div class="profile-card">
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Prenom</label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Nom</label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Telephone</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Email (non modifiable)</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Nouveau mot de passe (optionnel)</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Confirmer le mot de passe</label>
                    <asp:TextBox ID="txtPasswordConfirm" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
            </div>
            <div class="d-flex gap-2 mt-3">
                <asp:Button ID="btnSave" runat="server" Text="Enregistrer" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Annuler" CssClass="btn btn-outline-secondary" OnClick="btnReset_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</asp:Content>
