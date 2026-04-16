<%@ Page Title="Creer un compte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="E_comma.Views.Auth.Register" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="~/Content/css/auth-theme.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="auth-page">
        <div class="container">
            <div class="auth-shell">
                <div class="row g-0">
                    <div class="col-lg-5">
                        <div class="auth-panel h-100">
                            <div class="panel-kicker">E-comma</div>
                            <h2 class="panel-title">Creer un compte</h2>
                            <p class="panel-text">Inscris-toi pour profiter d'une experience shopping personnalisee.</p>
                            <ul class="panel-list">
                                <li>Offres exclusives et promos</li>
                                <li>Historique des commandes</li>
                                <li>Assistance rapide</li>
                            </ul>
                            <a class="btn btn-secondary rounded-pill px-4" href="Login.aspx">Deja un compte</a>
                        </div>
                    </div>
                    <div class="col-lg-7">
                        <div class="auth-card">
                            <div class="auth-header">
                                <h2 class="auth-title">Inscription</h2>
                                <p class="auth-subtitle">Remplis les informations pour creer ton compte.</p>
                            </div>

                            <div class="message-container">
                                <asp:Label ID="lblMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                            </div>

                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Prenom</label>
                                    <asp:TextBox ID="txtName" runat="server" placeholder="Votre prenom" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Nom</label>
                                    <asp:TextBox ID="txtLastName" runat="server" placeholder="Votre nom" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Adresse email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" placeholder="exemple@mail.com" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Telephone</label>
                                    <asp:TextBox ID="txtPhone" runat="server" placeholder="06XXXXXXXX ou +212 6XX XXX XXX" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Mot de passe</label>
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Creer un mot de passe" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Confirmer le mot de passe</label>
                                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder="Confirmez votre mot de passe" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mt-4">
                                <asp:Button ID="btnRegister" runat="server" Text="Creer le compte" CssClass="btn btn-primary w-100" OnClick="btnRegister_Click" />
                            </div>

                            <div class="auth-links text-center mt-3">
                                Vous avez deja un compte ? <a href="Login.aspx">Se connecter</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
