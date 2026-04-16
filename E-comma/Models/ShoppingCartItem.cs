using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class ShoppingCartItem
    {
        // Propriétés
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedAt { get; set; }

        // Connexion à la base
        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        // ➤ 1) Récupérer tous les articles d'un panier
        public static List<ShoppingCartItem> GetByCartId(Guid cartId)
        {
            List<ShoppingCartItem> items = new List<ShoppingCartItem>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM ShoppingCartItems WHERE ShoppingCartId = @CartId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CartId", cartId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        items.Add(new ShoppingCartItem
                        {
                            Id = Guid.Parse(row["Id"].ToString()),
                            ShoppingCartId = Guid.Parse(row["ShoppingCartId"].ToString()),
                            ProductId = Guid.Parse(row["ProductId"].ToString()),
                            Quantity = (int)row["Quantity"],
                            Price = (decimal)row["Price"],
                            AddedAt = (DateTime)row["AddedAt"]
                        });
                    }
                }
            }

            return items;
        }

        // ➤ 2) Ajouter un article au panier
        public static bool AddItem(Guid cartId, Guid productId, int quantity, decimal price)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                // Vérifier si le produit existe déjà dans le panier
                string checkQuery = "SELECT Id, Quantity FROM ShoppingCartItems WHERE ShoppingCartId = @CartId AND ProductId = @ProductId";
                
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@CartId", cartId);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);

                con.Open();
                SqlDataReader reader = checkCmd.ExecuteReader();

                if (reader.Read())
                {
                    // Produit existe déjà, mettre à jour la quantité
                    Guid itemId = (Guid)reader["Id"];
                    int existingQty = (int)reader["Quantity"];
                    reader.Close();

                    return UpdateQuantity(itemId, existingQty + quantity);
                }
                else
                {
                    reader.Close();

                    // Nouveau produit, l'ajouter
                    string insertQuery = @"INSERT INTO ShoppingCartItems (ShoppingCartId, ProductId, Quantity, Price) 
                                           VALUES (@CartId, @ProductId, @Quantity, @Price)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@CartId", cartId);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                    insertCmd.Parameters.AddWithValue("@Price", price);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ➤ 3) Mettre à jour la quantité d'un article
        public static bool UpdateQuantity(Guid itemId, int quantity)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE ShoppingCartItems SET Quantity = @Quantity WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", itemId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ➤ 4) Supprimer un article du panier
        public static bool RemoveItem(Guid itemId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "DELETE FROM ShoppingCartItems WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", itemId);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ➤ 5) Calculer le total du panier
        public static decimal GetCartTotal(Guid cartId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT SUM(Quantity * Price) FROM ShoppingCartItems WHERE ShoppingCartId = @CartId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CartId", cartId);

                con.Open();
                object result = cmd.ExecuteScalar();

                return result != DBNull.Value ? (decimal)result : 0;
            }
        }
    }
}
