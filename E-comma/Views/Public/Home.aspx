<%@ Page Title="Accueil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs"
    Inherits="E_comma.Views.Public.Home" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <style>
            .section-title {
                text-align: center;
                font-size: 2rem;
                margin-bottom: 40px;
                color: #2c3e50;
            }

            .products-grid {
                display: grid;
                grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
                gap: 30px;
                margin-bottom: 60px;
            }

            .product-card {
                background: white;
                border-radius: 10px;
                overflow: hidden;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                transition: transform 0.3s, box-shadow 0.3s;
            }

            .product-card:hover {
                transform: translateY(-5px);
                box-shadow: 0 8px 15px rgba(0, 0, 0, 0.2);
            }

            .product-image {
                width: 100%;
                height: 200px;
                background: #ecf0f1;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 3rem;
            }

            .product-info {
                padding: 20px;
            }

            .product-name {
                font-size: 1.2rem;
                font-weight: 600;
                margin-bottom: 10px;
                color: #2c3e50;
            }

            .product-price {
                font-size: 1.5rem;
                color: #e74c3c;
                font-weight: bold;
                margin-bottom: 15px;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <!-- Carousel Start -->
        <div class="container-fluid carousel bg-light px-0">
            <div class="row g-0 justify-content-end">
                <div class="col-12 col-lg-7 col-xl-9">
                    <div class="header-carousel bg-light py-5">
                        <div class="header-carousel-track">
                            <div class="row g-0 header-carousel-item align-items-center">
                                <div class="col-xl-6 carousel-img">
                                    <img src="https://cooperativeyacout.com/wp-content/uploads/2023/10/5H5A0099-scaled.jpg"
                                        class="img-fluid w-100" alt="Image">
                                </div>
                                <div class="col-xl-6 carousel-content p-4">
                                    <h4 class="text-uppercase fw-bold mb-4" style="letter-spacing: 3px;">Save Up To A
                                        100 DH</h4>
                                    <h1 class="display-3 text-capitalize mb-4">Nos Produits <br> Hamame</h1>
                                    <p class="text-dark">Terms and Condition Apply</p>
                                    <a class="btn btn-primary rounded-pill py-3 px-5"
                                        href="/Views/Public/Shop.aspx">Shop Now</a>
                                </div>
                            </div>
                            <div class="row g-0 header-carousel-item align-items-center">
                                <div class="col-xl-6 carousel-img">
                                    <img src="https://cooperativeyacout.com/wp-content/uploads/2016/01/hammam-SAVON-NOIR-EUCALYPTUS-BLACK-SOAP-ARGANOIL-YACOUT-COOPERATIVE-MAROC-FEMMES-MAROCAINES-FABRIQUE-ARTISANAT-MADE-IN-MOROCCO-MAGHRIB.png"
                                        class="img-fluid w-100" alt="Image">
                                </div>
                                <div class="col-xl-6 carousel-content p-4">
                                    <h4 class="text-uppercase fw-bold mb-4" style="letter-spacing: 3px;">Save Up To A DH
                                        200</h4>
                                    <h1 class="display-3 text-capitalize mb-4">Nos Produits <br> Hamame</h1>
                                    <p class="text-dark">Terms and Condition Apply</p>
                                    <a class="btn btn-primary rounded-pill py-3 px-5"
                                        href="/Views/Public/Shop.aspx">Shop Now</a>
                                </div>
                            </div>
                        </div>
                        <!-- Navigation -->
                        <div class="hero-nav" aria-hidden="false">
                            <button type="button" class="hero-prev" aria-label="Previous slide"><i
                                    class="bi bi-arrow-left"></i></button>
                            <button type="button" class="hero-next" aria-label="Next slide"><i
                                    class="bi bi-arrow-right"></i></button>
                        </div>
                    </div>
                </div>
                <div class="col-12 col-lg-5 col-xl-3">
                    <div class="carousel-header-banner h-100">
                        <img src="https://cooperativeyacout.com/wp-content/uploads/2023/05/PARFUM-LAVANDE-CANNELLE-THE-VERT-VERVEINE-LA-ROSE-ORIENT-BOUGIE-TRADITIONNELLE-FLEUR-DORANGER-COOPERATIVE-YACOUT-SENTEUR-PARFUM-JAUNE-ROUGE-MAROC-PRODUIT-MAROCAIN-MADE-IN-MOROCCO-copie.png"
                            class="img-fluid w-100 h-100" style="object-fit: cover;" alt="Image">
                        <div class="carousel-banner-offer">
                            <p class="text-white rounded fs-5 py-2 px-4 mb-0 me-3" style="background-color: #c4a066;">
                                Save DH 75.00</p>
                            <p class="text-primary fs-5 fw-bold mb-0">Special Offer</p>
                        </div>
                        <div class="carousel-banner">
                            <div class="carousel-banner-content text-center p-4">
                                <a href="#" class="d-block mb-2">Parfum d'intérieur Cannelle et orange</a>
                                <a href="#" class="d-block text-white fs-3">Nouveau Parfum<br></a>
                                <del class="me-2 text-white fs-5">DH 149.00</del>
                                <span class="text-primary fs-5">DH 75.00</span>
                            </div>
                            <a href="#" class="btn btn-primary rounded-pill py-2 px-4"><i
                                    class="fas fa-shopping-cart me-2"></i> Add To Cart</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Carousel End -->

        <!-- Services Start -->
        <div class="container-fluid px-0">
            <div class="row g-0">
                <div class="col-6 col-md-4 col-lg-2 border-start border-end wow fadeInUp" data-wow-delay="0.1s">
                    <div class="p-4">
                        <div class="d-inline-flex align-items-center">
                            <i class="fa fa-sync-alt fa-2x text-primary"></i>
                            <div class="ms-4">
                                <h6 class="text-uppercase mb-2">Free Return</h6>
                                <p class="mb-0">30 days money back guarantee!</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-6 col-md-4 col-lg-2 border-end wow fadeInUp" data-wow-delay="0.2s">
                    <div class="p-4">
                        <div class="d-flex align-items-center">
                            <i class="fab fa-telegram-plane fa-2x text-primary"></i>
                            <div class="ms-4">
                                <h6 class="text-uppercase mb-2">Free Shipping</h6>
                                <p class="mb-0">Free shipping on all order</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-6 col-md-4 col-lg-2 border-end wow fadeInUp" data-wow-delay="0.3s">
                    <div class="p-4">
                        <div class="d-flex align-items-center">
                            <i class="fas fa-life-ring fa-2x text-primary"></i>
                            <div class="ms-4">
                                <h6 class="text-uppercase mb-2">Support 24/7</h6>
                                <p class="mb-0">We support online 24 hrs a day</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-6 col-md-4 col-lg-2 border-end wow fadeInUp" data-wow-delay="0.4s">
                    <div class="p-4">
                        <div class="d-flex align-items-center">
                            <i class="fas fa-credit-card fa-2x text-primary"></i>
                            <div class="ms-4">
                                <h6 class="text-uppercase mb-2">Receive Gift Card</h6>
                                <p class="mb-0">Recieve gift all over oder DH 50</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-6 col-md-4 col-lg-2 border-end wow fadeInUp" data-wow-delay="0.5s">
                    <div class="p-4">
                        <div class="d-flex align-items-center">
                            <i class="fas fa-lock fa-2x text-primary"></i>
                            <div class="ms-4">
                                <h6 class="text-uppercase mb-2">Secure Payment</h6>
                                <p class="mb-0">We Value Your Security</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-6 col-md-4 col-lg-2 border-end wow fadeInUp" data-wow-delay="0.6s">
                    <div class="p-4">
                        <div class="d-flex align-items-center">
                            <i class="fas fa-blog fa-2x text-primary"></i>
                            <div class="ms-4">
                                <h6 class="text-uppercase mb-2">Online Service</h6>
                                <p class="mb-0">Free return products in 30 days</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Services End -->

        <!-- Products Categories Start -->
        <section class="custom-section">
            <div class="custom-container">
                <div class="custom-column" onclick="window.location.href='/Views/Public/Shop.aspx?category=cadeaux'">
                    <div class="custom-banner banner-1">
                        <img loading="lazy" decoding="async"
                            src="https://cooperativeyacout.com/wp-content/uploads/2023/07/IMG_2327.jpg"
                            alt="Image de cadeaux" />
                        <div class="image-overlay"></div>
                        <div class="shimmer-effect"></div>
                        <div class="banner-content banner-content-1">
                            <h2 class="banner-title"><span>Nos Cadeaux</span></h2>
                            <div class="button-wrap banner-button-wrap">
                                <div class="banner-button">Je découvre</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="custom-column" onclick="window.location.href='/Views/Public/Shop.aspx?category=solaire'">
                    <div class="custom-banner banner-2">
                        <img loading="lazy" decoding="async"
                            src="https://cooperativeyacout.com/wp-content/uploads/2023/08/CADEAUX-GIFT-OIL-BODY-SUMMER-ETE-2023-YACOUT-COOPERATIVE-MAROC-FEMMES-MAROCAINES-FABRIQUE-ARTISANAT-MADE-IN-MOROCCO-MAGHRIB-ARAB-2.png"
                            alt="Image de produits solaires" />
                        <div class="image-overlay"></div>
                        <div class="shimmer-effect"></div>
                        <div class="banner-content">
                            <h2 class="banner-title"><span>Sélection Solaire</span></h2>
                            <div class="button-wrap">
                                <div class="banner-button">Je découvre</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="custom-column" onclick="window.location.href='/Views/Public/Shop.aspx?category=parfum'">
                    <div class="custom-banner banner-3">
                        <img loading="lazy" decoding="async"
                            src="https://cooperativeyacout.com/wp-content/uploads/2023/07/PARFUM-DIFUSEUR-ROSE-KALAAT-MGOUNA-KALAA-GOUNA-TRADITIONNELLE-FLEUR-DORANGER-COOPERATIVE-YACOUT-SENTEUR-PARFUM-JAUNE-ROUGE-MAROC-PRODUIT-MAROCAIN-MADE-IN-MOROCCO.png"
                            alt="Image de parfum" />
                        <div class="image-overlay"></div>
                        <div class="shimmer-effect"></div>
                        <div class="banner-content">
                            <h2 class="banner-title"><span>Nouveau Parfum</span></h2>
                            <div class="button-wrap">
                                <div class="banner-button">Je découvre</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <!-- Products Categories End -->

        <!-- Promotions Start -->
        <section class="promotions-section">
            <div class="container">
                <div class="section-header">
                    <h4>Promotions</h4>
                    <p>Ne ratez pas nos promotions exclusives</p>
                </div>

                <div class="promo-product-grid">
                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/06/COFFRET-GOURMET-TERROIR-DARGAN-AGANIA-SPINOSA-KERNEL-OIL-SOAP-DE-GRAINES-DE-FESSI-YACOUT-COOPERATIVE-MAROC-FEMMES-MAROCAINES-FABRIQUE-ARTISANAT-MADE-IN-MOROCCO-MAGHRIB-9-768x764.png"
                                    alt="Coffret douceur du Maroc">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">ALIMENTAIRE</div>
                            <div class="promo-product-title">Coffret douceur du Maroc</div>
                            <div class="promo-product-price">
                                <span class="old-price">295.00 DH</span>
                                <span class="new-price">265.00 DH</span>
                            </div>
                        </div>
                    </div>

                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/06/HUILE-DE-BAIN-50-ML-AMANDE-ARGAN-ALMONDES-PETALES-DAMANDIER-MASSAGE-COOPERATIVE-YACOUT-BERBER-600x600.png"
                                    alt="Huile de bain pétale d'amandier">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">HUILES</div>
                            <div class="promo-product-title">Huile de bain pétale d'amandier</div>
                            <div class="promo-product-price">
                                <span class="old-price">150.00 DH</span>
                                <span class="new-price">119.00 DH</span>
                            </div>
                        </div>
                    </div>

                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/07/MASQUE-MASK-AKER-FASSI-ARGAN-AKKER-EL-FASI-ECLAIRCISSANT-CORPS-BODY-COOPERATIVE-YACOUT-PRODUIT-NATUREL-MAROCAIN-MADE-IN-MOROCCO-RABAT-CASABLANCA-ARGAN-OIL-600x599.png"
                                    alt="Masque éclaircissant Aker el Fassi">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">TONIFIER VOTRE CORPS</div>
                            <div class="promo-product-title">Masque éclaircissant Aker el Fassi</div>
                            <div class="promo-product-price">
                                <span class="old-price">140.00 DH</span>
                                <span class="new-price">99.00 DH</span>
                            </div>
                        </div>
                    </div>

                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/06/CREME-EXFOLIANTE-ARGILE-ROUGE-CREAM-SCRUB-RED-CLAY-ARGANOIL-YACOUT-COOPERATIVE-MAROC-FEMMES-MAROCAINES-FABRIQUE-ARTISANAT-MADE-IN-MOROCCO-MAGHRIB-RED-ROUGE-600x600.png"
                                    alt="Crème exfoliante Argile rouge et cannelle">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">CRÈMES</div>
                            <div class="promo-product-title">Crème exfoliante Argile rouge et cannelle</div>
                            <div class="promo-product-price">
                                <span class="old-price">95.00 DH</span>
                                <span class="new-price">72.00 DH</span>
                            </div>
                        </div>
                    </div>

                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/07/PARFUM-ORANGE-BLOSSOM-VERT-VERVEINE-LA-ROSE-ORIENT-BOUGIE-TRADITIONNELLE-FLEUR-DORANGER-COOPERATIVE-YACOUT-SENTEUR-PARFUM-JAUNE-ROUGE-MAROC-PRODUIT-MAROCAIN-MADE-IN-MOROCCO-600x600.png"
                                    alt="Parfum d'intérieur Fleur d'oranger">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">PARFUM D'INTÉRIEUR</div>
                            <div class="promo-product-title">Parfum d'intérieur Fleur d'oranger</div>
                            <div class="promo-product-price">~62.00 DH – 149.00 DH</div>
                        </div>
                    </div>

                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/10/1202386C-7817-4D7D-82F7-C485B2591257-600x600.jpg"
                                    alt="Coffret Fleur d'oranger">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">COSMÉTIQUE</div>
                            <div class="promo-product-title">Coffret Fleur d'oranger</div>
                            <div class="promo-product-price">
                                <span class="old-price">395.00 DH</span>
                                <span class="new-price">360.00 DH</span>
                            </div>
                        </div>
                    </div>

                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/10/4EADB896-26E3-4044-A1F2-C0DBD548BFFD-600x600.jpg"
                                    alt="Coffret Rose">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">COSMÉTIQUE</div>
                            <div class="promo-product-title">Coffret Rose</div>
                            <div class="promo-product-price">
                                <span class="old-price">390.00 DH</span>
                                <span class="new-price">365.00 DH</span>
                            </div>
                        </div>
                    </div>

                    <div class="promo-product-card">
                        <div class="promo-product-image-wrapper">
                            <a href="#">
                                <img loading="lazy" class="promo-product-image"
                                    src="https://cooperativeyacout.com/wp-content/uploads/2023/10/CBCD36C3-3A5F-451F-998C-41BB35FECFFB-600x600.jpg"
                                    alt="Coffret Verveine">
                            </a>
                        </div>
                        <div class="promo-product-details">
                            <div class="promo-product-category">COSMÉTIQUE</div>
                            <div class="promo-product-title">Coffret Verveine</div>
                            <div class="promo-product-price">
                                <span class="old-price">395.00 DH</span>
                                <span class="new-price">360.00 DH</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <!-- Promotions End -->



        <!-- Scripts -->
        <script>
            document.addEventListener('DOMContentLoaded', function () {
                // Carousel
                var carousel = document.querySelector('.header-carousel');
                if (carousel) {
                    var track = carousel.querySelector('.header-carousel-track');
                    var slides = track.querySelectorAll('.header-carousel-item');
                    var prevBtn = carousel.querySelector('.hero-prev');
                    var nextBtn = carousel.querySelector('.hero-next');
                    var index = 0;
                    var slideCount = slides.length;
                    var autoplaySpeed = 4000;
                    var interval;

                    function updateSlide() {
                        track.style.transform = 'translateX(-' + (index * 100) + '%)';
                    }

                    function nextSlide() { index = (index + 1) % slideCount; updateSlide(); }
                    function prevSlide() { index = (index - 1 + slideCount) % slideCount; updateSlide(); }

                    if (nextBtn) nextBtn.addEventListener('click', function () { nextSlide(); resetInterval(); });
                    if (prevBtn) prevBtn.addEventListener('click', function () { prevSlide(); resetInterval(); });

                    function resetInterval() {
                        if (interval) clearInterval(interval);
                        interval = setInterval(nextSlide, autoplaySpeed);
                    }

                    if (slideCount > 1) {
                        interval = setInterval(nextSlide, autoplaySpeed);
                        carousel.addEventListener('mouseenter', function () { clearInterval(interval); });
                        carousel.addEventListener('mouseleave', resetInterval);
                    }
                    updateSlide();
                }

                // Products Categories Animation
                var columns = document.querySelectorAll('.custom-column');
                var observerOptions = { root: null, rootMargin: '0px', threshold: 0.1 };
                var observerCallback = function (entries, observer) {
                    entries.forEach(function (entry) {
                        if (entry.isIntersecting) {
                            entry.target.classList.add('is-visible');
                            observer.unobserve(entry.target);
                        }
                    });
                };
                var observer = new IntersectionObserver(observerCallback, observerOptions);
                columns.forEach(function (column) { observer.observe(column); });
                // Promo Cards Staggered Fade-in Animation
                var promoCards = document.querySelectorAll('.promo-product-card');
                var promoObserver = new IntersectionObserver(observerCallback, observerOptions);
                promoCards.forEach(function (card) { promoObserver.observe(card); });
            });
        </script>
    </asp:Content>