using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration; // Nécessaire pour ConfigurationManager

namespace E_comma.Models
{
    public class OrderItem
    {
        // Propriétés
        public long Id { get; set; }
        public long OrderId { get; set; }
        // ATTENTION : La DB stocke ProductVariantId, mais le modèle veut ProductId pour la récupération d'infos produit.
        // On utilisera ProductId dans le modèle C# et ProductVariantId dans la DB.
        public int ProductId { get; set; } 
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; } // Calculé dans GetDetailsByOrderId

        // Propriétés étendues pour l'affichage (récupérées via jointure)
        public string Brand { get; set; }
        public string MainImageUrl { get; set; }
        public string Attributes { get; set; } // Récupéré de ProductVariants
        public decimal LineTotal { get { return TotalPrice; } } // Alias pour TotalPrice

        // Connexion à la base
        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        // ➤ 1) Récupérer tous les articles d'une commande
        public static List<OrderItem> GetByOrderId(long orderId)
        {
            return GetDetailsByOrderId(orderId);
        }

        // ➤ 1b) Récupérer les détails complets (avec image, marque, etc.)
        public static List<OrderItem> GetDetailsByOrderId(long orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                // REQUÊTE CORRIGÉE :
                // - Jointure sur ProductVariants (pv) pour obtenir ProductId et Attributes
                // - Jointure sur Products (p) pour obtenir ProductName et Brand
                // - Calcul de TotalPrice dans le SELECT
                string query = @"
                    SELECT 
                        oi.Id, 
                        oi.OrderId, 
                        oi.Quantity, 
                        oi.UnitPrice, 
                        (oi.Quantity * oi.UnitPrice) AS TotalPrice, -- 🎯 TotalPrice calculé ici
                        pv.ProductId, -- 🎯 ProductId récupéré du ProductVariant
                        pv.Attributes, -- 🎯 Attributes récupéré du ProductVariant
                        p.Name AS ProductName, -- 🎯 ProductName récupéré de Products
                        p.Brand,
                        pi.ImageUrl
                    FROM OrderItems oi
                    INNER JOIN ProductVariants pv ON oi.ProductVariantId = pv.Id
                    INNER JOIN Products p ON pv.ProductId = p.Id
                    LEFT JOIN ProductImages pi ON p.Id = pi.ProductId AND pi.IsMainImage = 1
                    WHERE oi.OrderId = @OrderId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new OrderItem
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                OrderId = Convert.ToInt64(reader["OrderId"]),
                                ProductId = (int)reader["ProductId"], // Mappé depuis pv.ProductId
                                ProductName = reader["ProductName"].ToString(), // Mappé depuis p.Name
                                Quantity = (int)reader["Quantity"],
                                UnitPrice = (decimal)reader["UnitPrice"],
                                TotalPrice = (decimal)reader["TotalPrice"], // Mappé depuis le calcul
                                Brand = reader["Brand"] != DBNull.Value ? reader["Brand"].ToString() : "",
                                MainImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : "/Content/img/no-image.png",
                                Attributes = reader["Attributes"] != DBNull.Value ? reader["Attributes"].ToString() : ""
                            });
                        }
                    }
                }
            }
            return items;
        }

        // ➤ 2) Créer un article de commande
        // ATTENTION : La méthode doit prendre ProductVariantId et non ProductId. 
        // Les paramètres ProductName et TotalPrice ne sont pas utilisés dans l'INSERT car ils n'existent pas dans la table OrderItems SQL.
        public static bool Create(long orderId, int productVariantId, int quantity, decimal unitPrice)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                // REQUÊTE CORRIGÉE : N'insère que les colonnes existantes dans la DB OrderItems :
                // OrderId, ProductVariantId, Quantity, UnitPrice.
                string query = @"INSERT INTO OrderItems (OrderId, ProductVariantId, Quantity, UnitPrice) 
                                 VALUES (@OrderId, @ProductVariantId, @Quantity, @UnitPrice)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@ProductVariantId", productVariantId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                
                // Les paramètres ProductName et TotalPrice ne sont pas nécessaires pour l'INSERT
                // L'ancien code les utilisait : cmd.Parameters.AddWithValue("@ProductName", productName);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}