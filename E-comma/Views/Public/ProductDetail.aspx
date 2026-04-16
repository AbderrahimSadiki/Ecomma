<%@ Page Title="Détail Produit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProductDetail.aspx.cs" Inherits="E_comma.Views.Public.ProductDetail" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <style>
            .product-main-image {
                width: 100%;
                height: 500px;
                object-fit: contain;
                border: 1px solid #e5e5e5;
                background: #f8f9fa;
            }

            .product-thumbnail {
                width: 100px;
                height: 100px;
                object-fit: cover;
                cursor: pointer;
                border: 2px solid transparent;
                transition: border-color 0.3s;
            }

            .product-thumbnail:hover,
            .product-thumbnail.active {
                border-color: #e1251a;
            }

            .variant-option {
                cursor: pointer;
                padding: 8px 15px;
                border: 1px solid #ddd;
                border-radius: 4px;
                margin-right: 10px;
                margin-bottom: 10px;
                display: inline-block;
                transition: all 0.3s;
            }

            .variant-option:hover {
                border-color: #e1251a;
                background-color: #fff5f5;
            }

            .variant-option.active {
                border-color: #e1251a;
                background-color: #e1251a;
                color: white;
            }

            .variant-option.out-of-stock {
                opacity: 0.5;
                cursor: not-allowed;
            }

            .review-card {
                border-left: 3px solid #e1251a;
            }

            .star-rating-large {
                font-size: 24px;
                color: #ffc107;
            }

            .review-stars {
                color: #ffc107;
            }

            .verified-badge {
                background-color: #28a745;
                color: white;
                padding: 2px 8px;
                border-radius: 3px;
                font-size: 11px;
            }
        </style>
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <asp:Panel ID="ProductNotFoundPanel" runat="server" Visible="false" CssClass="container py-5 text-center">
            <i class="fas fa-exclamation-triangle fa-5x text-warning mb-3"></i>
            <h3>Produit non trouvé</h3>
            <p class="text-muted">Le produit que vous recherchez n'existe pas ou a été supprimé.</p>
            <a href="Shop.aspx" class="btn btn-primary">Retour à la boutique</a>
        </asp:Panel>

        <asp:Panel ID="ProductDetailPanel" runat="server">
            <!-- Breadcrumb -->
            <div class="container-fluid py-3 bg-light">
                <div class="container">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-0">
                            <li class="breadcrumb-item"><a href="/">Accueil</a></li>
                            <li class="breadcrumb-item"><a href="Shop.aspx">Boutique</a></li>
                            <li class="breadcrumb-item">
                                <asp:Literal ID="ltCategoryLink" runat="server"></asp:Literal>
                            </li>
                            <li class="breadcrumb-item active">
                                <asp:Literal ID="ltProductName" runat="server"></asp:Literal>
                            </li>
                        </ol>
                    </nav>
                </div>
            </div>

            <!-- Product Detail -->
            <div class="container py-5">
                <div class="row">
                    <!-- Images Column -->
                    <div class="col-lg-6 mb-4">
                        <div class="mb-3">
                            <img id="mainImage" src="" class="product-main-image" alt="">
                        </div>
                        <div class="d-flex gap-2 flex-wrap">
                            <asp:Repeater ID="ImagesRepeater" runat="server">
                                <ItemTemplate>
                                    <img src="<%# Eval(" ImageUrl") %>"
                                    class="product-thumbnail <%# Container.ItemIndex==0 ? "active" : "" %>"
                                        alt="<%# Eval("AltText") %>"
                                            onclick="changeMainImage(this.src, this)">
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <!-- Product Info Column -->
                    <div class="col-lg-6">
                        <div class="mb-2">
                            <span class="badge bg-secondary">
                                <asp:Literal ID="ltCategory" runat="server"></asp:Literal>
                            </span>
                            <span class="badge bg-info ms-2">
                                <asp:Literal ID="ltBrand" runat="server"></asp:Literal>
                            </span>
                        </div>

                        <h2 class="mb-3">
                            <asp:Literal ID="ltProductTitle" runat="server"></asp:Literal>
                        </h2>

                        <!-- Rating -->
                        <div class="mb-3">
                            <asp:Literal ID="ltRating" runat="server"></asp:Literal>
                        </div>

                        <!-- Price -->
                        <div class="mb-4">
                            <h3 class="text-primary mb-0">
                                <span id="selectedPrice">
                                    <asp:Literal ID="ltPrice" runat="server"></asp:Literal>
                                </span> DH
                            </h3>
                        </div>

                        <!-- Description -->
                        <div class="mb-4">
                            <h5>Description</h5>
                            <p class="text-muted">
                                <asp:Literal ID="ltDescription" runat="server"></asp:Literal>
                            </p>
                        </div>

                        <!-- Variants -->
                        <asp:Panel ID="VariantsPanel" runat="server" CssClass="mb-4">
                            <h6 class="mb-3">Options disponibles</h6>
                            <div id="variantsContainer">
                                <asp:Repeater ID="VariantsRepeater" runat="server">
                                    <ItemTemplate>
                                        <div class="variant-option <%# (int)Eval(" StockQuantity")==0 ? "out-of-stock"
                                            : "" %>"
                                            data-variant-id="<%# Eval("Id") %>"
                                                data-price="<%# Eval("Price") %>"
                                                    data-stock="<%# Eval("StockQuantity") %>"
                                                        onclick="selectVariant(this)">
                                                        <%# Eval("Attributes") %>
                                                            <%# (int)Eval("StockQuantity")==0
                                                                ? "<small>(Rupture)</small>" : "" %>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:Panel>
                        <!-- Moved outside panel to ensure it exists even if no variants -->
                        <input type="hidden" id="selectedVariantId" value="" />

                        <!-- Stock -->
                        <div class="mb-4">
                            <span id="stockStatus" class="badge bg-success">En stock</span>
                        </div>

                        <!-- Quantity & Add to Cart -->
                        <div class="mb-4">
                            <div class="row g-3">
                                <div class="col-auto">
                                    <label class="form-label">Quantité</label>
                                    <div class="input-group" style="width: 150px;">
                                        <button class="btn btn-outline-secondary" type="button"
                                            onclick="decreaseQty()">-</button>
                                        <input type="number" id="quantity" class="form-control text-center" value="1"
                                            min="1">
                                        <button class="btn btn-outline-secondary" type="button"
                                            onclick="increaseQty()">+</button>
                                    </div>
                                </div>
                                <div class="col">
                                    <label class="form-label">&nbsp;</label>
                                    <button type="button" class="btn btn-primary btn-lg w-100" onclick="addToCart()">
                                        <i class="fas fa-shopping-cart me-2"></i>Ajouter au panier
                                    </button>
                                </div>
                            </div>
                        </div>

                        <!-- Actions -->
                        <div class="d-flex gap-2">
                            <button class="btn btn-outline-secondary" onclick="addToWishlist()">
                                <i class="far fa-heart me-2"></i>Liste de souhaits
                            </button>
                            <button class="btn btn-outline-secondary" onclick="shareProduct()">
                                <i class="fas fa-share-alt me-2"></i>Partager
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Reviews Section -->
                <div class="row mt-5">
                    <div class="col-12">
                        <ul class="nav nav-tabs" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active" data-bs-toggle="tab" href="#reviews">
                                    Avis clients (<asp:Literal ID="ltReviewCount" runat="server"></asp:Literal>)
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" data-bs-toggle="tab" href="#addreview">
                                    Laisser un avis
                                </a>
                            </li>
                        </ul>

                        <div class="tab-content p-4 border border-top-0">
                            <!-- Reviews List -->
                            <div id="reviews" class="tab-pane fade show active">
                                <asp:Repeater ID="ReviewsRepeater" runat="server">
                                    <ItemTemplate>
                                        <div class="card review-card mb-3">
                                            <div class="card-body">
                                                <div class="d-flex justify-content-between">
                                                    <div>
                                                        <h6 class="mb-1">
                                                            <%# Eval("UserName") %>
                                                                <%# (bool)Eval("IsVerifiedPurchase")
                                                                    ? "<span class='verified-badge ms-2'>✓ Achat vérifié</span>"
                                                                    : "" %>
                                                        </h6>
                                                        <div class="review-stars mb-2">
                                                            <%# GetStars((int)Eval("Rating")) %>
                                                        </div>
                                                    </div>
                                                    <small class="text-muted">
                                                        <%# ((DateTime)Eval("CreatedAt")).ToString("dd/MM/yyyy") %>
                                                    </small>
                                                </div>
                                                <p class="mb-0">
                                                    <%# Eval("Comment") %>
                                                </p>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <asp:Panel ID="NoReviewsPanel" runat="server" Visible="false"
                                    CssClass="text-center py-4">
                                    <p class="text-muted">Aucun avis pour le moment. Soyez le premier à donner votre
                                        avis!</p>
                                </asp:Panel>
                            </div>

                            <!-- Add Review Form -->
                            <div id="addreview" class="tab-pane fade">
                                <asp:Panel ID="LoginRequiredPanel" runat="server" Visible="false"
                                    CssClass="alert alert-warning">
                                    <i class="fas fa-info-circle me-2"></i>
                                    Vous devez être <a href="/Login.aspx">connecté</a> pour laisser un avis.
                                </asp:Panel>

                                <asp:Panel ID="ReviewFormPanel" runat="server" Visible="false">
                                    <div class="mb-3">
                                        <label class="form-label">Note</label>
                                        <div class="star-rating-large">
                                            <i class="far fa-star rating-star" data-rating="1"
                                                onclick="setRating(1)"></i>
                                            <i class="far fa-star rating-star" data-rating="2"
                                                onclick="setRating(2)"></i>
                                            <i class="far fa-star rating-star" data-rating="3"
                                                onclick="setRating(3)"></i>
                                            <i class="far fa-star rating-star" data-rating="4"
                                                onclick="setRating(4)"></i>
                                            <i class="far fa-star rating-star" data-rating="5"
                                                onclick="setRating(5)"></i>
                                        </div>
                                        <asp:HiddenField ID="hdnRating" runat="server" Value="0" />
                                    </div>
                                    <div class="mb-3">
                                        <label class="form-label">Votre commentaire</label>
                                        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="5"
                                            CssClass="form-control"
                                            placeholder="Partagez votre expérience avec ce produit..."></asp:TextBox>
                                    </div>
                                    <asp:Button ID="btnSubmitReview" runat="server" Text="Publier l'avis"
                                        CssClass="btn btn-primary" OnClick="btnSubmitReview_Click" />
                                    <asp:Label ID="lblReviewMessage" runat="server" CssClass="ms-3"></asp:Label>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Cart Sidebar -->
        <div id="cartOverlay" class="cart-overlay" onclick="closeCart()"></div>
        <div id="cartSidebar" class="cart-sidebar">
            <div class="cart-header">
                <h5 class="mb-0">Mon Panier</h5>
                <button type="button" class="btn-close" onclick="closeCart()"></button>
            </div>
            <div class="cart-body" id="cartItemsContainer">
                <!-- Items will be injected here -->
            </div>
            <div class="cart-footer">
                <div class="d-flex justify-content-between mb-3">
                    <span class="fw-bold">Total</span>
                    <span class="fw-bold text-primary" id="cartSidebarTotal">0.00 DH</span>
                </div>
                <a href="Checkout.aspx" class="btn btn-brown w-100">Commander</a>
            </div>
        </div>

        <style>
            .cart-overlay {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.5);
                z-index: 1040;
                display: none;
                opacity: 0;
                transition: opacity 0.3s;
            }

            .cart-overlay.show {
                display: block;
                opacity: 1;
            }

            .cart-sidebar {
                position: fixed;
                top: 0;
                right: -400px;
                width: 350px;
                height: 100%;
                background: white;
                z-index: 1050;
                transition: right 0.3s ease-in-out;
                display: flex;
                flex-direction: column;
                box-shadow: -2px 0 5px rgba(0, 0, 0, 0.1);
            }

            .cart-sidebar.show {
                right: 0;
            }

            .cart-header {
                padding: 20px;
                border-bottom: 1px solid #eee;
                display: flex;
                justify-content: space-between;
                align-items: center;
            }

            .cart-body {
                flex: 1;
                overflow-y: auto;
                padding: 20px;
            }

            .cart-footer {
                padding: 20px;
                border-top: 1px solid #eee;
                background: #f8f9fa;
            }

            .cart-item {
                display: flex;
                margin-bottom: 20px;
                border-bottom: 1px solid #f0f0f0;
                padding-bottom: 15px;
            }

            .cart-item:last-child {
                border-bottom: none;
            }

            .cart-item-img {
                width: 60px;
                height: 60px;
                object-fit: cover;
                border-radius: 4px;
                margin-right: 15px;
            }

            .cart-item-details {
                flex: 1;
            }

            .cart-item-title {
                font-size: 0.9rem;
                font-weight: 600;
                margin-bottom: 5px;
                display: block;
            }

            .cart-item-price {
                color: #e1251a;
                font-weight: 600;
                font-size: 0.9rem;
            }

            .cart-qty-control {
                display: flex;
                align-items: center;
                margin-top: 5px;
            }

            .btn-qty {
                width: 24px;
                height: 24px;
                padding: 0;
                display: flex;
                align-items: center;
                justify-content: center;
                border: 1px solid #ddd;
                background: white;
                border-radius: 50%;
                font-size: 12px;
                cursor: pointer;
            }

            .btn-qty:hover {
                background: #f0f0f0;
            }

            .qty-display {
                margin: 0 10px;
                font-size: 0.9rem;
                min-width: 20px;
                text-align: center;
            }

            .btn-brown {
                background-color: #8B4513;
                color: white;
                border: none;
                padding: 12px;
                text-transform: uppercase;
                font-weight: 600;
                letter-spacing: 1px;
                transition: background 0.3s;
            }

            .btn-brown:hover {
                background-color: #6F370F;
                color: white;
            }

            .remove-item {
                color: #999;
                cursor: pointer;
                font-size: 0.8rem;
                margin-left: auto;
            }

            .remove-item:hover {
                color: #dc3545;
            }
        </style>

        <script>
            let selectedRating = 0;

            function changeMainImage(src, thumb) {
                document.getElementById('mainImage').src = src;
                document.querySelectorAll('.product-thumbnail').forEach(t => t.classList.remove('active'));
                thumb.classList.add('active');
            }

            function selectVariant(element) {
                if (element.classList.contains('out-of-stock')) return;

                document.querySelectorAll('.variant-option').forEach(v => v.classList.remove('active'));
                element.classList.add('active');

                const price = element.dataset.price;
                const stock = element.dataset.stock;
                const variantId = element.dataset.variantId;

                document.getElementById('selectedPrice').textContent = parseFloat(price).toFixed(2);
                document.getElementById('selectedVariantId').value = variantId;

                const stockStatus = document.getElementById('stockStatus');
                if (stock > 0) {
                    stockStatus.className = 'badge bg-success';
                    stockStatus.textContent = `En stock (${stock} disponible${stock > 1 ? 's' : ''})`;
                } else {
                    stockStatus.className = 'badge bg-danger';
                    stockStatus.textContent = 'Rupture de stock';
                }
            }

            function increaseQty() {
                const input = document.getElementById('quantity');
                input.value = parseInt(input.value) + 1;
            }

            function decreaseQty() {
                const input = document.getElementById('quantity');
                if (parseInt(input.value) > 1) {
                    input.value = parseInt(input.value) - 1;
                }
            }

            function addToCart() {
                console.log("addToCart called");
                const productId = <%= ProductId %>;
                const variantInput = document.getElementById('selectedVariantId');
                const variantId = variantInput ? (variantInput.value || 0) : 0;
                const quantity = document.getElementById('quantity').value;

                console.log("ProductId:", productId, "VariantId:", variantId, "Quantity:", quantity);

                // Appel AJAX vers le serveur
                fetch('ProductDetail.aspx/AddToCart', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ productId: productId, variantId: parseInt(variantId), quantity: parseInt(quantity) })
                })
                    .then(response => {
                        console.log("Response status:", response.status);
                        if (!response.ok) {
                            throw new Error('Network response was not ok');
                        }
                        return response.json();
                    })
                    .then(data => {
                        console.log("Data received:", data);
                        if (data.d.success) {
                            updateSidebar(data.d);
                            openCart();
                        } else {
                            alert(data.d.message);
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                        alert('Une erreur est survenue lors de l\'ajout au panier.');
                    });
            }

            function updateCartItem(itemId, quantity) {
                fetch('ProductDetail.aspx/UpdateCartItem', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ itemId: itemId, quantity: quantity })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d.success) {
                            updateSidebar(data.d);
                        }
                    });
            }

            function removeCartItem(itemId) {
                fetch('ProductDetail.aspx/RemoveCartItem', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ itemId: itemId })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d.success) {
                            updateSidebar(data.d);
                        }
                    });
            }

            function updateSidebar(data) {
                const container = document.getElementById('cartItemsContainer');
                const totalEl = document.getElementById('cartSidebarTotal');

                totalEl.textContent = data.total + ' DH';
                container.innerHTML = '';

                if (data.items.length === 0) {
                    container.innerHTML = '<p class="text-center text-muted mt-5">Votre panier est vide</p>';
                    return;
                }

                data.items.forEach(item => {
                    const html = `
                    <div class="cart-item">
                        <img src="${item.ProductImage}" class="cart-item-img" alt="">
                        <div class="cart-item-details">
                            <span class="cart-item-title">${item.ProductName}</span>
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="cart-item-price">${item.Price.toFixed(2)} DH</span>
                                <i class="fas fa-trash-alt remove-item" onclick="removeCartItem(${item.Id})"></i>
                            </div>
                            <small class="text-muted d-block mb-1">${item.Attributes}</small>
                            <div class="cart-qty-control">
                                <button type="button" class="btn-qty" onclick="updateCartItem(${item.Id}, ${item.Quantity - 1})">-</button>
                                <span class="qty-display">${item.Quantity}</span>
                                <button type="button" class="btn-qty" onclick="updateCartItem(${item.Id}, ${item.Quantity + 1})">+</button>
                            </div>
                        </div>
                    </div>
                `;
                    container.insertAdjacentHTML('beforeend', html);
                });
            }

            function openCart() {
                document.getElementById('cartOverlay').classList.add('show');
                document.getElementById('cartSidebar').classList.add('show');
                document.body.style.overflow = 'hidden';
            }

            function closeCart() {
                document.getElementById('cartOverlay').classList.remove('show');
                document.getElementById('cartSidebar').classList.remove('show');
                document.body.style.overflow = '';
            }

            function setRating(rating) {
                selectedRating = rating;
                document.getElementById('<%= hdnRating.ClientID %>').value = rating;

                const stars = document.querySelectorAll('.rating-star');
                stars.forEach((star, index) => {
                    if (index < rating) {
                        star.classList.remove('far');
                        star.classList.add('fas');
                    } else {
                        star.classList.remove('fas');
                        star.classList.add('far');
                    }
                });
            }

            function addToWishlist() {
                const productId = <%= ProductId %>;

                // Appel AJAX vers le serveur
                fetch('ProductDetail.aspx/AddToWishlist', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ productId: productId })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d.success) {
                            alert('Produit ajouté à la liste de souhaits!');
                        } else {
                            alert(data.d.message);
                        }
                    });
            }

            // Init: Select first variant
            window.addEventListener('DOMContentLoaded', function () {
                const firstVariant = document.querySelector('.variant-option:not(.out-of-stock)');
                if (firstVariant) {
                    selectVariant(firstVariant);
                }

                const firstImage = document.querySelector('.product-thumbnail');
                if (firstImage) {
                    document.getElementById('mainImage').src = firstImage.src;
                }
            });
        </script>
    </asp:Content>