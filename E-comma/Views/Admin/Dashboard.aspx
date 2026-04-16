<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="E_comma.Views.Admin.Dashboard" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
        <link href="~/Content/css/admin.css" rel="stylesheet" />
    </asp:Content>

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container-fluid admin-dashboard py-4">

            <!-- VIEW: DASHBOARD (Stats & Quick Links) -->
            <asp:HiddenField ID="hfActiveTab" runat="server" Value="dashboard" />
            <div id="view-dashboard" class="admin-view active-view">
                <h3 class="mb-4 fw-bold" style="color: var(--primary-dark);">Tableau de bord</h3>
                <div class="row g-3 admin-metrics mb-4">
                    <div class="col-lg-3 col-sm-6">
                        <div class="metric-card">
                            <div class="metric-label">Categories</div>
                            <div class="metric-value">
                                <asp:Label ID="lblCategoryCount" runat="server" Text="0" />
                            </div>
                            <div class="metric-subtext">Total enregistrees</div>
                            <i class="bi bi-collection metric-icon"></i>
                        </div>
                    </div>
                    <div class="col-lg-3 col-sm-6">
                        <div class="metric-card">
                            <div class="metric-label">Produits</div>
                            <div class="metric-value">
                                <asp:Label ID="lblProductCount" runat="server" Text="0" />
                            </div>
                            <div class="metric-subtext">Catalog complet</div>
                            <i class="bi bi-box-seam metric-icon"></i>
                        </div>
                    </div>
                    <div class="col-lg-3 col-sm-6">
                        <div class="metric-card">
                            <div class="metric-label">En vedette</div>
                            <div class="metric-value">
                                <asp:Label ID="lblFeaturedCount" runat="server" Text="0" />
                            </div>
                            <div class="metric-subtext">Produits mis en avant</div>
                            <i class="bi bi-star metric-icon"></i>
                        </div>
                    </div>
                    <div class="col-lg-3 col-sm-6">
                        <div class="metric-card">
                            <div class="metric-label">Valeur catalogue</div>
                            <div class="metric-value">DH
                                <asp:Label ID="lblCatalogValue" runat="server" Text="0" />
                            </div>
                            <div class="metric-subtext">Prix de base cumules</div>
                            <i class="bi bi-cash-stack metric-icon"></i>
                        </div>
                    </div>
                </div>

                <!-- Quick Links to other modules -->
                <div class="row g-3 mb-4">
                    <div class="col-lg-4 col-md-6">
                        <div class="metric-card" style="border-left-color: #667eea;">
                            <div class="metric-label">Gestion Stock</div>
                            <div class="metric-value"><i class="bi bi-boxes"></i></div>
                            <div class="mt-2">
                                <a href="StockManagement.aspx" class="btn btn-sm btn-outline-primary">Acceder au
                                    Stock</a>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6">
                        <div class="metric-card" style="border-left-color: #f093fb;">
                            <div class="metric-label">Alertes Stock</div>
                            <div class="metric-value">
                                <asp:Label ID="lblStockAlertCount" runat="server" Text="0" />
                            </div>
                            <div class="mt-2">
                                <span class="text-danger small">
                                    <asp:Label ID="lblAlertBadge" runat="server" Text="0" /> Critique(s)
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6">
                        <div class="metric-card" style="border-left-color: #4facfe;">
                            <div class="metric-label">Livraison</div>
                            <div class="metric-value"><i class="bi bi-truck"></i></div>
                            <div class="mt-2">
                                <a href="DeliveryManagement.aspx" class="btn btn-sm btn-outline-primary">Gerer
                                    Livraisons</a>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Stock Alerts Panel (Dash only) -->
                <div class="card shadow-sm admin-section mb-4">
                    <div class="card-header d-flex align-items-center justify-content-between bg-danger text-white">
                        <h5 class="mb-0 text-white"><i class="bi bi-exclamation-triangle-fill me-2"></i>Alertes Stock
                            Prioritaires</h5>
                    </div>
                    <div class="card-body p-0">
                        <asp:Panel ID="pnlStockAlerts" runat="server" Visible="false">
                            <div class="table-responsive">
                                <table class="table admin-table align-middle mb-0">
                                    <thead>
                                        <tr>
                                            <th>Produit</th>
                                            <th>Stock</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptStockAlerts" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%# Eval("ProductName") %>
                                                    </td>
                                                    <td><span class="badge bg-danger">
                                                            <%# Eval("CurrentStock") %>
                                                        </span></td>
                                                    <td>
                                                        <a href='StockManagement.aspx?variant=<%# Eval("ProductVariantId") %>'
                                                            class="btn btn-sm btn-light">Voir</a>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnlNoStockAlerts" runat="server" Visible="true" CssClass="p-4 text-center">
                            <small class="text-muted">Tout est en ordre.</small>
                        </asp:Panel>
                    </div>
                </div>
            </div>

            <!-- VIEW: CATEGORIES -->
            <div id="view-categories" class="admin-view">
                <h3 class="mb-4 fw-bold" style="color: var(--primary-dark);">Gestion des Categories</h3>

                <div class="card shadow-sm admin-section">
                    <div class="card-body">
                        <asp:Label ID="lblCategoryStatus" runat="server" Visible="false"></asp:Label>
                        <div class="row g-4">
                            <!-- Form -->
                            <div class="col-lg-4 border-end">
                                <h5 class="mb-3">Ajouter / Modifier</h5>
                                <asp:HiddenField ID="hfCategoryId" runat="server" />
                                <div class="mb-3">
                                    <label class="form-label">Nom</label>
                                    <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control"
                                        placeholder="Ex: Soins"></asp:TextBox>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">Slug</label>
                                    <asp:TextBox ID="txtCategorySlug" runat="server" CssClass="form-control"
                                        placeholder="auto-generated"></asp:TextBox>
                                </div>
                                <div class="mb-3">
                                    <label class="form-label">Parent</label>
                                    <asp:DropDownList ID="ddlParentCategory" runat="server" CssClass="form-select">
                                    </asp:DropDownList>
                                </div>
                                <div class="d-grid gap-2">
                                    <asp:Button ID="btnSaveCategory" runat="server" Text="Enregistrer"
                                        CssClass="btn btn-primary" OnClick="btnSaveCategory_Click" />
                                    <asp:Button ID="btnResetCategory" runat="server" Text="Annuler"
                                        CssClass="btn btn-outline-secondary" OnClick="btnResetCategory_Click"
                                        CausesValidation="false" />
                                </div>
                            </div>

                            <!-- List -->
                            <div class="col-lg-8">
                                <div class="filter-container">
                                    <i class="bi bi-search"></i>
                                    <input type="text" id="params-filter-cat" class="filter-input"
                                        placeholder="Rechercher une categorie..."
                                        onkeyup="filterTable('params-filter-cat', 'table-categories')">
                                </div>

                                <asp:Repeater ID="rptCategories" runat="server"
                                    OnItemCommand="rptCategories_ItemCommand">
                                    <HeaderTemplate>
                                        <div class="table-responsive" style="max-height: 500px; overflow-y:auto;">
                                            <table class="table admin-table align-middle mb-0" id="table-categories">
                                                <thead>
                                                    <tr>
                                                        <th>Nom</th>
                                                        <th>Slug</th>
                                                        <th>Parent</th>
                                                        <th class="text-end">Actions</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%# Eval("Name") %>
                                            </td>
                                            <td><small class="text-muted">
                                                    <%# Eval("Slug") %>
                                                </small></td>
                                            <td>
                                                <%# Eval("ParentName") %>
                                            </td>
                                            <td class="text-end">
                                                <asp:LinkButton ID="btnEditCategory" runat="server"
                                                    CssClass="btn btn-sm btn-link text-primary"
                                                    CommandName="EditCategory" CommandArgument='<%# Eval("Id") %>'><i
                                                        class="bi bi-pencil"></i></asp:LinkButton>
                                                <asp:LinkButton ID="btnDeleteCategory" runat="server"
                                                    CssClass="btn btn-sm btn-link text-danger"
                                                    CommandName="DeleteCategory" CommandArgument='<%# Eval("Id") %>'
                                                    OnClientClick="return confirm('Confirmer suppression ?');"><i
                                                        class="bi bi-trash"></i></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody>
                                        </table>
                            </div>
                            </FooterTemplate>
                            </asp:Repeater>
                            <asp:Panel ID="pnlCategoryEmpty" runat="server" Visible="false" CssClass="text-center p-4">
                                <p class="text-muted">Aucune categorie.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- VIEW: PRODUCTS -->
        <div id="view-products" class="admin-view">
            <h3 class="mb-4 fw-bold" style="color: var(--primary-dark);">Catalogue Produits</h3>

            <div class="card shadow-sm admin-section">
                <div class="card-body">
                    <asp:Label ID="lblProductStatus" runat="server" Visible="false"></asp:Label>

                    <div class="row g-4">
                        <!-- Form -->
                        <div class="col-lg-5 border-end"> <!-- Slightly wider for product form -->
                            <h5 class="mb-3">Ajouter / Modifier</h5>
                            <div class="card card-body bg-light border-0 p-3">
                                <asp:HiddenField ID="hfProductId" runat="server" />
                                <asp:HiddenField ID="hfCurrentImageUrl" runat="server" />
                                <div class="row g-3">
                                    <div class="col-12">
                                        <label class="form-label">Nom</label>
                                        <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Slug</label>
                                        <asp:TextBox ID="txtProductSlug" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Marque</label>
                                        <asp:TextBox ID="txtBrand" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Prix (DH)</label>
                                        <asp:TextBox ID="txtBasePrice" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Categorie</label>
                                        <asp:DropDownList ID="ddlProductCategory" runat="server" CssClass="form-select">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Description</label>
                                        <asp:TextBox ID="txtProductDescription" runat="server" CssClass="form-control"
                                            TextMode="MultiLine" Rows="3" />
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Image</label>
                                        <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-12">
                                        <label class="form-label">Alt Text</label>
                                        <asp:TextBox ID="txtImageAlt" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-12">
                                        <div class="form-check">
                                            <asp:CheckBox ID="chkFeatured" runat="server" CssClass="form-check-input" />
                                            <label class="form-check-label">Mettre en avant</label>
                                        </div>
                                    </div>
                                    <div class="col-12 d-grid gap-2">
                                        <asp:Button ID="btnSaveProduct" runat="server" Text="Enregistrer"
                                            CssClass="btn btn-primary" OnClick="btnSaveProduct_Click" />
                                        <asp:Button ID="btnResetProduct" runat="server" Text="Annuler"
                                            CssClass="btn btn-outline-secondary" OnClick="btnResetProduct_Click"
                                            CausesValidation="false" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- List -->
                        <div class="col-lg-7">
                            <div class="filter-container mb-3">
                                <i class="bi bi-search"></i>
                                <input type="text" id="params-filter-prod" class="filter-input"
                                    placeholder="Rechercher un produit..."
                                    onkeyup="filterTable('params-filter-prod', 'table-products')">
                            </div>

                            <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
                                <HeaderTemplate>
                                    <div class="table-responsive" style="max-height: 800px; overflow-y:auto;">
                                        <table class="table admin-table align-middle mb-0" id="table-products">
                                            <thead>
                                                <tr>
                                                    <th>Info</th>
                                                    <th>Categorie</th>
                                                    <th>Prix</th>
                                                    <th>Statut</th>
                                                    <th class="text-end">Actions</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <div class="d-flex align-items-center">
                                                <span class="rounded me-2"
                                                    style='<%# String.Format("width:40px;height:40px;background-size:cover;background-position:center;background-image:url({0}); display:inline-block; border:1px solid #eee;", Eval("ImageUrl")) %>'></span>
                                                <div>
                                                    <div class="fw-bold text-dark">
                                                        <%# Eval("Name")%>
                                                    </div>
                                                    <small class="text-muted">
                                                        <%# Eval("Brand") %>
                                                    </small>
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            <%# Eval("CategoryName") %>
                                        </td>
                                        <td><span class="fw-bold" style="color:var(--primary-dark);">DH <%#
                                                    string.Format("{0:N2}", Eval("BasePrice")) %></span></td>
                                        <td>
                                            <%# (bool)Eval("IsFeatured")
                                                ? "<span class='badge badge-primary'>Vedette</span>" : "" %>
                                        </td>
                                        <td class="text-end">
                                            <asp:LinkButton ID="btnEditProduct" runat="server"
                                                CssClass="btn btn-sm btn-link text-primary" CommandName="EditProduct"
                                                CommandArgument='<%# Eval("Id") %>'><i class="bi bi-pencil"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnDeleteProduct" runat="server"
                                                CssClass="btn btn-sm btn-link text-danger" CommandName="DeleteProduct"
                                                CommandArgument='<%# Eval("Id") %>'
                                                OnClientClick="return confirm('Confirmer ?');"><i
                                                    class="bi bi-trash"></i>
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                                    </table>
                        </div>
                        </FooterTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlProductEmpty" runat="server" Visible="false" CssClass="text-center p-4">
                            <p class="text-muted">Aucun produit.</p>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
        </div>

        <!-- VIEW: USERS -->
        <div id="view-users" class="admin-view">
            <h3 class="mb-4 fw-bold" style="color: var(--primary-dark);">Gestion des Utilisateurs</h3>
            <asp:Label ID="lblUserStatus" runat="server" Visible="false" CssClass="alert alert-info d-block mb-3">
            </asp:Label>

            <div class="row">
                <div class="col-lg-4">
                    <div class="card shadow-sm admin-section">
                        <div class="card-body">
                            <h5 class="card-title mb-3">edition Rapide</h5>
                            <asp:HiddenField ID="hfUserId" runat="server" />
                            <div class="mb-3">
                                <label class="form-label">Email</label>
                                <asp:TextBox ID="txtUserEmail" runat="server" CssClass="form-control" />
                            </div>
                            <div class="row g-2 mb-3">
                                <div class="col-6">
                                    <label class="form-label">Prenom</label>
                                    <asp:TextBox ID="txtUserFirstName" runat="server" CssClass="form-control" />
                                </div>
                                <div class="col-6">
                                    <label class="form-label">Nom</label>
                                    <asp:TextBox ID="txtUserLastName" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Telephone</label>
                                <asp:TextBox ID="txtUserPhone" runat="server" CssClass="form-control" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Mot de passe (Modif.)</label>
                                <asp:TextBox ID="txtUserPassword" runat="server" CssClass="form-control"
                                    TextMode="Password" />
                            </div>
                            <div class="form-check mb-3">
                                <asp:CheckBox ID="chkUserActive" runat="server" CssClass="form-check-input"
                                    Checked="true" />
                                <label class="form-check-label">Compte Actif</label>
                            </div>
                            <div class="d-grid gap-2">
                                <asp:Button ID="btnSaveUser" runat="server" Text="Sauvegarder"
                                    CssClass="btn btn-primary" OnClick="btnSaveUser_Click" />
                                <asp:Button ID="btnResetUser" runat="server" Text="Annuler"
                                    CssClass="btn btn-outline-secondary" OnClick="btnResetUser_Click"
                                    CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-8">
                    <div class="card shadow-sm admin-section">
                        <div class="card-body">
                            <div class="filter-container">
                                <i class="bi bi-search"></i>
                                <input type="text" id="params-filter-users" class="filter-input"
                                    placeholder="Chercher par nom, email..."
                                    onkeyup="filterTable('params-filter-users', 'table-users')">
                            </div>
                            <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand">
                                <HeaderTemplate>
                                    <div class="table-responsive">
                                        <table class="table admin-table align-middle mb-0" id="table-users">
                                            <thead>
                                                <tr>
                                                    <th>Identite</th>
                                                    <th>Contact</th>
                                                    <th>Statut</th>
                                                    <th class="text-end">Actions</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <div class="fw-bold">
                                                <%# Eval("Name") %>
                                                    <%# Eval("LastName") %>
                                            </div>
                                        </td>
                                        <td>
                                            <div class="small">
                                                <%# Eval("Email") %>
                                            </div>
                                            <div class="small text-muted">
                                                <%# Eval("Phone") %>
                                            </div>
                                        </td>
                                        <td>
                                            <%# (bool)Eval("IsActive") ? "<span class='badge bg-success'>Actif</span>"
                                                : "<span class='badge bg-secondary'>Inactif</span>" %>
                                        </td>
                                        <td class="text-end">
                                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-link text-primary"
                                                CommandName="EditUser" CommandArgument='<%# Eval("Id") %>'><i
                                                    class="bi bi-pencil"></i></asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-link text-warning"
                                                CommandName="ToggleUser"
                                                CommandArgument='<%# Eval("Id") + ";" + Eval("IsActive") %>'><i
                                                    class="bi bi-power"></i></asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-link text-danger"
                                                CommandName="DeleteUser" CommandArgument='<%# Eval("Id") %>'
                                                OnClientClick="return confirm('Confirmer ?');"><i
                                                    class="bi bi-trash"></i></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                                    </table>
                        </div>
                        </FooterTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlUserEmpty" runat="server" Visible="false" CssClass="text-center p-4">
                            <p class="text-muted">Aucun utilisateur.</p>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
        </div>

        </div>

        <script type="text/javascript">
            (function () {
                // --- View Selection Logic ---
                var hfActiveTab = document.getElementById('<%= hfActiveTab.ClientID %>');

                function showView(viewName) {
                    if (!viewName) viewName = "dashboard";

                    // Normalize viewName (remove # or view- prefix if present for clean storage)
                    viewName = viewName.replace("#", "").replace("view-", "");

                    // Hide all views
                    document.querySelectorAll('.admin-view').forEach(function (el) {
                        el.classList.remove('active-view');
                    });

                    // Show target view
                    var targetId = "view-" + viewName;
                    var target = document.getElementById(targetId);
                    if (target) {
                        target.classList.add('active-view');
                        // Update HiddenField for PostBack persistence
                        if (hfActiveTab) hfActiveTab.value = viewName;
                        // Update Hash (optional, avoids page jump if handled carefully)
                        // window.location.hash = viewName; 
                    }
                }

                function initView() {
                    // Priority: HiddenField (PostBack) > Hash (Direct Link) > Default
                    var currentView = "dashboard";

                    if (hfActiveTab && hfActiveTab.value) {
                        currentView = hfActiveTab.value;
                    } else if (window.location.hash) {
                        currentView = window.location.hash.substring(1); // remove #
                    }

                    showView(currentView);
                }

                // Listen for hash changes (important for sidebar links)
                window.addEventListener('hashchange', function () {
                    var hash = window.location.hash;
                    if (hash) showView(hash.substring(1));
                    else showView("dashboard");
                });

                // Listen for sidebar or internal links that should switch views
                // Assuming links are like <a href="#products">...</a>
                document.body.addEventListener('click', function (e) {
                    // Traverse up to find anchor
                    var target = e.target.closest('a');
                    if (target && target.getAttribute('href') && target.getAttribute('href').startsWith('#')) {
                        var hash = target.getAttribute('href').substring(1);
                        // internal view switch
                        showView(hash);
                    }
                });

                // Initial load
                initView();

                // --- Slug Generation ---
                var slugify = function (value) {
                    return value.toString().toLowerCase().trim()
                        .normalize('NFD').replace(/[\u0300-\u036f]/g, "") // Remove accents
                        .replace(/[^a-z0-9\s-]/g, "") // Remove non-alphanumeric chars
                        .replace(/\s+/g, "-"); // Replace spaces with -
                };
                var syncSlug = function (sourceId, targetId) {
                    var source = document.getElementById(sourceId);
                    var target = document.getElementById(targetId);
                    if (!source || !target) return;
                    if (target.value) target.dataset.locked = "true";

                    source.addEventListener("input", function () {
                        if (target.dataset.locked === "true") return;
                        target.value = slugify(source.value);
                    });
                };
                syncSlug("<%= txtCategoryName.ClientID %>", "<%= txtCategorySlug.ClientID %>");
                syncSlug("<%= txtProductName.ClientID %>", "<%= txtProductSlug.ClientID %>");

            })();

            // --- Global Filter Function ---
            function filterTable(inputId, tableId) {
                var input = document.getElementById(inputId);
                var filter = input.value.toLowerCase();
                var table = document.getElementById(tableId);
                var tr = table.getElementsByTagName("tr");

                for (var i = 1; i < tr.length; i++) {
                    var visible = false;
                    var tds = tr[i].getElementsByTagName("td");
                    for (var j = 0; j < tds.length; j++) {
                        if (tds[j] && tds[j].innerText.toLowerCase().indexOf(filter) > -1) {
                            visible = true;
                            break;
                        }
                    }
                    tr[i].style.display = visible ? "" : "none";
                }
            }
        </script>

    </asp:Content>