<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="~/Content/css/contact.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="contact-hero">
        <div class="container">
            <div class="row align-items-center g-4">
                <div class="col-lg-6">
                    <div class="contact-kicker">Contact</div>
                    <h1 class="contact-title">Parlons de votre projet</h1>
                    <p class="contact-lead">
                        Une question sur un produit ou une commande ? Ecrivez-nous et notre equipe vous repondra rapidement.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <a class="btn btn-primary rounded-pill px-4 py-2" href="tel:+212780085121">Appeler maintenant</a>
                        <a class="btn btn-secondary rounded-pill px-4 py-2" href="mailto:info@example.com">Envoyer un email</a>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="contact-card">
                        <div class="contact-icon"><i class="bi bi-headset"></i></div>
                        <h3>Service client</h3>
                        <p>Disponible du lundi au samedi pour vos questions et demandes.</p>
                        <div class="contact-hours">
                            <strong>Horaires :</strong> 09:00 - 19:00
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="contact-section">
        <div class="container">
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="contact-card">
                        <div class="contact-icon"><i class="bi bi-geo-alt"></i></div>
                        <h3>Adresse</h3>
                        <p>1er etage, Mhanech, 277 Av. Casablanca, Tetouan</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="contact-card">
                        <div class="contact-icon"><i class="bi bi-telephone"></i></div>
                        <h3>Telephone</h3>
                        <p>(+212)-780085121</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="contact-card">
                        <div class="contact-icon"><i class="bi bi-envelope"></i></div>
                        <h3>Email</h3>
                        <p>info@example.com</p>
                    </div>
                </div>
            </div>

            <div class="row g-4 mt-2">
                <div class="col-lg-6">
                    <div class="contact-form">
                        <h3 class="mb-3">Envoyer un message</h3>
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label">Nom</label>
                                <input type="text" class="form-control" placeholder="Votre nom" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Email</label>
                                <input type="email" class="form-control" placeholder="exemple@mail.com" />
                            </div>
                            <div class="col-12">
                                <label class="form-label">Sujet</label>
                                <input type="text" class="form-control" placeholder="Sujet du message" />
                            </div>
                            <div class="col-12">
                                <label class="form-label">Message</label>
                                <textarea class="form-control" rows="4" placeholder="Ecrivez votre message"></textarea>
                            </div>
                            <div class="col-12">
                                <button type="button" class="btn btn-primary w-100">Envoyer</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="contact-map">
                        <iframe title="Localisation" loading="lazy" referrerpolicy="no-referrer-when-downgrade"
                            src="https://www.openstreetmap.org/export/embed.html?bbox=-5.3834%2C35.5753%2C-5.3548%2C35.5888&amp;layer=mapnik&amp;marker=35.5820%2C-5.3690"></iframe>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
