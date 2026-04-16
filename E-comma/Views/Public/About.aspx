<%@ Page Title="A propos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="~/Content/css/about.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="about-hero">
        <div class="container">
            <div class="row align-items-center g-4">
                <div class="col-lg-6">
                    <div class="about-kicker">E-comma</div>
                    <h1 class="about-title">A propos de notre cooperative</h1>
                    <p class="about-lead">
                        Nous selectionnons des produits inspires du rituel marocain, avec une attention
                        particuliere a la qualite, aux ingredients, et a l'experience client.
                    </p>
                    <div class="d-flex flex-wrap gap-2">
                        <a class="btn btn-primary rounded-pill px-4 py-2" href="/Views/Public/Shop.aspx">Explorer la boutique</a>
                        <a class="btn btn-outline-dark btn-ghost rounded-pill px-4 py-2" href="/Views/Public/Contact.aspx">Nous contacter</a>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="about-highlight bg-white border shadow-sm">
                        <div class="row g-3">
                            <div class="col-6">
                                <div class="stat-card text-center">
                                    <div class="stat-value">200+</div>
                                    <div class="stat-label">Produits artisanaux</div>
                                </div>
                            </div>
                            <div class="col-6">
                                <div class="stat-card text-center">
                                    <div class="stat-value">15</div>
                                    <div class="stat-label">Ateliers partenaires</div>
                                </div>
                            </div>
                            <div class="col-6">
                                <div class="stat-card text-center">
                                    <div class="stat-value">48h</div>
                                    <div class="stat-label">Preparation de commande</div>
                                </div>
                            </div>
                            <div class="col-6">
                                <div class="stat-card text-center">
                                    <div class="stat-value">4.8/5</div>
                                    <div class="stat-label">Avis clients</div>
                                </div>
                            </div>
                        </div>
                        <div class="about-quote">
                            "Nous voulons offrir l'authenticite d'un savoir-faire local, avec un service en ligne moderne."
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="about-section">
        <div class="container">
            <div class="text-center">
                <div class="about-kicker">Nos engagements</div>
                <h2 class="section-title">Une experience soignee du debut a la fin</h2>
                <p class="section-subtitle">
                    Nous travaillons avec des artisans et des producteurs pour garantir des produits
                    authentiques, un emballage propre, et une livraison soignee.
                </p>
            </div>
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="about-card">
                        <div class="about-icon"><i class="bi bi-gem"></i></div>
                        <h3>Qualite verifiee</h3>
                        <p>Chaque produit est selectionne pour sa composition, son origine et sa regularite.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="about-card">
                        <div class="about-icon"><i class="bi bi-leaf"></i></div>
                        <h3>Formules naturelles</h3>
                        <p>Nous privilegions des ingredients d'origine naturelle et des recettes traditionnelles.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="about-card">
                        <div class="about-icon"><i class="bi bi-people"></i></div>
                        <h3>Relation humaine</h3>
                        <p>Des partenaires de confiance et une equipe disponible pour vous guider.</p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="about-section pt-0">
        <div class="container">
            <div class="row align-items-center g-4">
                <div class="col-lg-6">
                    <img class="img-fluid story-image" src="https://cooperativeyacout.com/wp-content/uploads/2023/07/IMG_2327.jpg" alt="Artisanat marocain" loading="lazy" />
                </div>
                <div class="col-lg-6">
                    <div class="about-kicker">Notre histoire</div>
                    <h2 class="section-title">Une boutique nee d'une passion pour le bien-etre</h2>
                    <p class="about-lead">
                        E-comma est ne d'une envie simple : rendre accessibles des produits authentiques,
                        inspires des rituels de soins, sans compromis sur la qualite.
                    </p>
                    <ul class="about-timeline list-unstyled">
                        <li>
                            <h4>2019 - Creation de la cooperative</h4>
                            <p>Selection de produits artisanaux et premiers partenaires locaux.</p>
                        </li>
                        <li>
                            <h4>2021 - Lancement en ligne</h4>
                            <p>Ouverture de la boutique et mise en place d'un service client dedie.</p>
                        </li>
                        <li>
                            <h4>2024 - Expansion</h4>
                            <p>Nouvelle gamme bien-etre et livraison acceleree au niveau national.</p>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </section>

    <section class="about-section pt-0">
        <div class="container">
            <div class="about-cta">
                <div class="row align-items-center g-3">
                    <div class="col-lg-8">
                        <h3>Envie de decouvrir nos produits ?</h3>
                        <p>Parcourez notre selection et trouvez votre prochain rituel de soin.</p>
                    </div>
                    <div class="col-lg-4 text-lg-end">
                        <a class="btn btn-primary rounded-pill px-4 py-2" href="/Views/Public/Shop.aspx">Voir la boutique</a>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
