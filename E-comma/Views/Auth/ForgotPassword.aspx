<%@ Page Title="Mot de passe oublié" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ForgotPassword.aspx.cs" Inherits="E_comma.Views.Auth.ForgotPassword" %>

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
                                <div class="panel-kicker">Securite</div>
                                <h2 class="panel-title">Recuperation</h2>
                                <p class="panel-text">Nous vous aidons a securiser de nouveau votre acces.</p>
                                <ul class="panel-list">
                                    <li>Processus securise</li>
                                    <li>Verification d'identite</li>
                                    <li>Support 24/7</li>
                                </ul>
                                <a class="btn btn-secondary rounded-pill px-4" href="Login.aspx">Retour à la
                                    connexion</a>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <div class="auth-card">
                                <div class="auth-header">
                                    <h2 class="auth-title">Mot de passe oublie ?</h2>
                                    <p class="auth-subtitle">Entrez votre email pour recevoir un lien de
                                        reinitialisation.</p>
                                </div>

                                <div class="message-container">
                                    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success d-block"
                                        Visible="false"></asp:Label>
                                    <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger d-block"
                                        Visible="false"></asp:Label>
                                </div>

                                <div class="mb-4">
                                    <label class="form-label">Adresse email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" placeholder="exemple@mail.com"
                                        CssClass="form-control"></asp:TextBox>
                                </div>

                                <asp:Button ID="btnSubmit" runat="server" Text="Envoyer le lien"
                                    CssClass="btn btn-primary w-100" OnClick="btnSubmit_Click" />

                                <div class="auth-links text-center mt-3">
                                    <a href="Login.aspx">Se connecter</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </asp:Content>