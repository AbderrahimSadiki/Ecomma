<%@ Page Title="Connexion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="E_comma.Views.Auth.Login" %>

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
                                <h2 class="panel-title">Bienvenue</h2>
                                <p class="panel-text">Rejoins notre communaute et decouvre des offres exclusives.</p>
                                <ul class="panel-list">
                                    <li>Catalogue riche et mis a jour</li>
                                    <li>Paiement securise</li>
                                    <li>Suivi de commande simple</li>
                                </ul>
                                <a class="btn btn-secondary rounded-pill px-4" href="Register.aspx">Creer un compte</a>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <div class="auth-card">
                                <div class="auth-header">
                                    <h2 class="auth-title">Connexion</h2>
                                    <p class="auth-subtitle">Connecte-toi pour acceder a ton compte.</p>
                                </div>

                                <div class="message-container">
                                    <asp:Label ID="lblMessage" runat="server" CssClass="error-message" Visible="false">
                                    </asp:Label>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Adresse email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" placeholder="exemple@mail.com"
                                        CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="mb-3">
                                    <label class="form-label">Mot de passe</label>
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                                        placeholder="Votre mot de passe" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-4">
                                    <div class="form-check">
                                        <input type="checkbox" id="chkRemember" class="form-check-input" />
                                        <label class="form-check-label" for="chkRemember">Se souvenir</label>
                                    </div>
                                    <a href="ForgotPassword.aspx" class="auth-link">Mot de passe oublie?</a>
                                </div>

                                <asp:Button ID="btnLogin" runat="server" Text="Se connecter"
                                    CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />

                                <div class="auth-links text-center mt-3">
                                    Pas encore de compte ? <a href="Register.aspx">S'inscrire</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </asp:Content>