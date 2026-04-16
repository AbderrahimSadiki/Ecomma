<%@ Page Title="Caisse" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Checkout.aspx.cs" Inherits="E_comma.Views.Public.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid py-5">
        <div class="container py-5">
            <h1 class="mb-4">Details de facturation</h1>

            <!-- Message d'erreur -->
            <asp:Label ID="lblError" runat="server" Visible="false" CssClass="alert alert-danger"></asp:Label>

            <div class="row g-5">
                <!-- Formulaire de facturation -->
                <div class="col-md-12 col-lg-6 col-xl-7">
                    <div class="row">
                        <div class="col-md-12 col-lg-6">
                            <div class="form-item w-100">
                                <label class="form-label my-3">Nom complet<sup>*</sup></label>
                                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" 
                                    placeholder="Votre nom complet"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvFullName" runat="server"
                                    ControlToValidate="txtFullName" 
                                    ErrorMessage="Le nom est requis."
                                    CssClass="text-danger" 
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="col-md-12 col-lg-6">
                            <div class="form-item w-100">
                                <label class="form-label my-3">Email<sup>*</sup></label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                    TextMode="Email" 
                                    placeholder="votre@email.com"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                    ControlToValidate="txtEmail" 
                                    ErrorMessage="L'email est requis."
                                    CssClass="text-danger" 
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>

                    <div class="form-item w-100">
                        <label class="form-label my-3">Adresse<sup>*</sup></label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="3" 
                            placeholder="Votre adresse complete"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvAddress" runat="server" 
                            ControlToValidate="txtAddress"
                            ErrorMessage="L'adresse est requise." 
                            CssClass="text-danger" 
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>

                    <div class="row">
                        <div class="col-md-12 col-lg-6">
                            <div class="form-item w-100">
                                <label class="form-label my-3">Ville<sup>*</sup></label>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" 
                                    placeholder="Tetouan, Tanger, Casablanca..."></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCity" runat="server" 
                                    ControlToValidate="txtCity"
                                    ErrorMessage="La ville est requise." 
                                    CssClass="text-danger" 
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="col-md-12 col-lg-6">
                            <div class="form-item w-100">
                                <label class="form-label my-3">Telephone<sup>*</sup></label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" 
                                    placeholder="+212 6XX XXX XXX"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" 
                                    ControlToValidate="txtPhone"
                                    ErrorMessage="Le telephone est requis." 
                                    CssClass="text-danger" 
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                    </div>

                    <!-- Section Mode de livraison -->
                    <div class="card my-4">
                        <div class="card-header bg-light">
                            <h5 class="mb-0">
                                <i class="fas fa-truck"></i> Mode de livraison<sup>*</sup>
                            </h5>
                        </div>
                        <div class="card-body">
                            <asp:HiddenField ID="hfDeliveryMethodId" runat="server" />
                            
                            <asp:Repeater ID="rptDeliveryMethods" runat="server">
                                <ItemTemplate>
                                    <div class="form-check mb-3 p-3 border rounded delivery-option">
                                        <input type="radio" 
                                               name="deliveryMethod" 
                                               id="delivery_<%# Eval("Id") %>"
                                               value="<%# Eval("Id") %>" 
                                               class="form-check-input delivery-method-radio"
                                               data-price="<%# Eval("Price") %>"
                                               data-days="<%# Eval("EstimatedDays") %>" />
                                        <label class="form-check-label w-100" for="delivery_<%# Eval("Id") %>">
                                            <div class="d-flex justify-content-between align-items-start">
                                                <div>
                                                    <strong><%# Eval("Name") %></strong>
                                                    <br />
                                                    <small class="text-muted">
                                                        <%# Eval("Description") %>
                                                    </small>
                                                </div>
                                                <div class="text-end">
                                                    <strong class="text-primary">
                                                        <%# String.Format("{0:N2} DH", Eval("Price")) %>
                                                    </strong>
                                                    <br />
                                                    <small class="text-muted">
                                                        <%# Convert.ToInt32(Eval("EstimatedDays")) == 0 ? "Immediat" : 
                                                            Convert.ToInt32(Eval("EstimatedDays")) + " jour(s)" %>
                                                    </small>
                                                </div>
                                            </div>
                                        </label>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:CustomValidator ID="cvDeliveryMethod" runat="server" 
                                ErrorMessage="Veuillez selectionner un mode de livraison." 
                                CssClass="text-danger d-block mt-2"
                                Display="Dynamic"
                                ClientValidationFunction="validateDeliveryMethod"></asp:CustomValidator>
                        </div>
                    </div>
                </div>

                <!-- Resume de la commande -->
                <div class="col-md-12 col-lg-6 col-xl-5">
                    <div class="card">
                        <div class="card-header bg-light">
                            <h5 class="mb-0">
                                <i class="fas fa-shopping-cart"></i> Resume de la commande
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="table-responsive">
                                <table class="table table-sm">
                                    <thead>
                                        <tr>
                                            <th>Produit</th>
                                            <th class="text-center">Qte</th>
                                            <th class="text-end">Prix</th>
                                            <th class="text-end">Total</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptOrderItems" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <div class="d-flex align-items-center">
                                                            <img src='<%# Eval("ProductImage") %>'
                                                                class="img-fluid rounded me-2"
                                                                style="width: 40px; height: 40px; object-fit: cover;" 
                                                                alt="">
                                                            <div>
                                                                <small class="fw-bold"><%# Eval("ProductName") %></small>
                                                                <br />
                                                                <small class="text-muted"><%# Eval("Attributes") %></small>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td class="text-center align-middle">
                                                        <span class="badge bg-secondary"><%# Eval("Quantity") %></span>
                                                    </td>
                                                    <td class="text-end align-middle">
                                                        <small><%# Eval("Price", "{0:N2}" ) %> DH</small>
                                                    </td>
                                                    <td class="text-end align-middle">
                                                        <strong><%# ((decimal)Eval("Price") * (int)Eval("Quantity")).ToString("N2") %> DH</strong>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>

                            <!-- Totaux -->
                            <div class="border-top pt-3 mt-3">
                                <div class="d-flex justify-content-between mb-2">
                                    <span>Sous-total:</span>
                                    <strong>
                                        <asp:Label ID="lblSubtotal" runat="server" Text="0.00"></asp:Label> DH
                                    </strong>
                                </div>
                                <div class="d-flex justify-content-between mb-2">
                                    <span>
                                        Livraison:
                                        <small class="text-muted" id="deliveryInfo"></small>
                                    </span>
                                    <strong>
                                        <asp:Label ID="lblShipping" runat="server" Text="0.00"></asp:Label> DH
                                    </strong>
                                </div>
                                <div class="d-flex justify-content-between mb-2">
                                    <span>TVA:</span>
                                    <strong>
                                        <asp:Label ID="lblTax" runat="server" Text="0.00"></asp:Label> DH
                                    </strong>
                                </div>
                                <div class="d-flex justify-content-between pt-3 border-top">
                                    <h5>Total:</h5>
                                    <h5 class="text-primary">
                                        <asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label> DH
                                    </h5>
                                </div>
                            </div>

                            <!-- Boutons d'action -->
                            <div class="mt-4">
                                <asp:Button ID="btnPlaceOrder" runat="server" 
                                    Text="Confirmer la commande"
                                    CssClass="btn btn-primary w-100 py-3"
                                    OnClick="btnPlaceOrder_Click" />
                                
                                <a href="Cart.aspx" class="btn btn-outline-secondary w-100 mt-2">
                                    <i class="fas fa-arrow-left"></i> Retour au panier
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- JavaScript pour gestion du mode de livraison -->
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function() {
            // Gerer la selection du mode de livraison
            const deliveryRadios = document.querySelectorAll('.delivery-method-radio');
            const hiddenField = document.getElementById('<%= hfDeliveryMethodId.ClientID %>');
            const deliveryInfo = document.getElementById('deliveryInfo');
            
            deliveryRadios.forEach(radio => {
                radio.addEventListener('change', function() {
                    // Mettre à jour le champ cache
                    hiddenField.value = this.value;
                    
                    // Mettre à jour l'info de livraison
                    const price = parseFloat(this.getAttribute('data-price'));
                    const days = this.getAttribute('data-days');
                    
                    if (days == '0') {
                        deliveryInfo.textContent = '(Immediat)';
                    } else {
                        deliveryInfo.textContent = '(' + days + ' jour(s))';
                    }
                    
                    // Mettre en surbrillance l'option selectionnee
                    document.querySelectorAll('.delivery-option').forEach(opt => {
                        opt.classList.remove('border-primary', 'bg-light');
                    });
                    this.closest('.delivery-option').classList.add('border-primary', 'bg-light');
                    
                    // Declencher le recalcul (postback)
                    __doPostBack('<%= btnCalculateShipping.UniqueID %>', '');
                });
            });
        });

        // Validation côte client pour le mode de livraison
        function validateDeliveryMethod(source, args) {
            const hiddenField = document.getElementById('<%= hfDeliveryMethodId.ClientID %>');
            args.IsValid = (hiddenField.value !== '' && hiddenField.value !== '0');
        }
    </script>

    <!-- Bouton cache pour le calcul des frais -->
    <asp:Button ID="btnCalculateShipping" runat="server" 
        style="display:none;" 
        OnClick="btnCalculateShipping_Click" 
        CausesValidation="false" />

    <style>
        .delivery-option {
            cursor: pointer;
            transition: all 0.3s ease;
        }
        .delivery-option:hover {
            border-color: #0d6efd !important;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .delivery-option.border-primary {
            background-color: #f8f9fa;
        }
    </style>
</asp:Content>