<%@ Page Title="Réinitialiser le mot de passe" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ResetPassword.aspx.cs" Inherits="E_comma.Views.Auth.ResetPassword" %>

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
                                <div class="panel-kicker">Nouveau Depart</div>
                                <h2 class="panel-title">Sécurité</h2>
                                <p class="panel-text">Choisissez un nouveau mot de passe pour protéger votre compte.</p>
                                <ul class="panel-list">
                                    <li>Mise à jour immédiate</li>
                                    <li>Confirmation par email</li>
                                    <li>Connexion automatique</li>
                                </ul>
                            </div>
                        </div>
                        <div class="col-lg-7">
                            <div class="auth-card">
                                <div class="auth-header">
                                    <h2 class="auth-title">Réinitialisation</h2>
                                    <p class="auth-subtitle">Créez votre nouveau mot de passe sécurisé.</p>
                                </div>

                                <div class="message-container">
                                    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success d-block"
                                        Visible="false"></asp:Label>
                                    <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger d-block"
                                        Visible="false"></asp:Label>
                                </div>

                                <asp:Panel ID="pnlReset" runat="server" Visible="false">
                                    <div class="mb-3">
                                        <label class="form-label">Nouveau mot de passe</label>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                                            placeholder="Entrez votre nouveau mot de passe" CssClass="form-control">
                                        </asp:TextBox>
                                    </div>

                                    <div class="mb-4">
                                        <label class="form-label">Confirmer le mot de passe</label>
                                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password"
                                            placeholder="Confirmez votre mot de passe" CssClass="form-control">
                                        </asp:TextBox>
                                    </div>

                                    <asp:Button ID="btnReset" runat="server" Text="Réinitialiser"
                                        CssClass="btn btn-primary w-100" OnClick="btnReset_Click" />
                                </asp:Panel>

                                <div class="auth-links text-center mt-3">
                                    <a href="Login.aspx">Retour à la connexion</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </asp:Content>