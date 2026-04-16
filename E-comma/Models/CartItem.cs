using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }

        // Propriétés étendues pour l'affichage
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public string Attributes { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static bool AddToCart(Guid userId, int variantId, int quantity)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();
                
                // Récupérer le prix de la variante
                string priceQuery = "SELECT Price FROM ProductVariants WHERE Id = @VariantId";
                SqlCommand priceCmd = new SqlCommand(priceQuery, con);
                priceCmd.Parameters.AddWithValue("@VariantId", variantId);
                object priceObj = priceCmd.ExecuteScalar();
                
                if (priceObj == null)
                {
                    return false; // Variante introuvable
                }
                
                decimal unitPrice = (decimal)priceObj;
                
                // Vérifier si l'item existe déjà
                string checkQuery = "SELECT Id, Quantity FROM CartItems WHERE UserId = @UserId AND ProductVariantId = @VariantId";

                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@VariantId", variantId);

                SqlDataReader reader = checkCmd.ExecuteReader();

                if (reader.Read())
                {
                    int existingId = (int)reader["Id"];
                    int existingQty = (int)reader["Quantity"];
                    reader.Close();

                    string updateQuery = "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@Id", existingId);
                    updateCmd.Parameters.AddWithValue("@Quantity", existingQty + quantity);

                    return updateCmd.ExecuteNonQuery() > 0;
                }
                else
                {
                    reader.Close();

                    string insertQuery = @"INSERT INTO CartItems (UserId, ProductVariantId, Quantity, UnitPrice) 
                                           VALUES (@UserId, @VariantId, @Quantity, @UnitPrice)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                    insertCmd.Parameters.AddWithValue("@VariantId", variantId);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                    insertCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);

                    return insertCmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<CartItem> GetItems(Guid userId)
        {
            List<CartItem> items = new List<CartItem>();
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"
                    SELECT ci.Id, ci.Quantity, ci.ProductVariantId, ci.AddedAt,
                           p.Name, pv.Price, pv.Attributes,
                           (SELECT TOP 1 ImageUrl FROM ProductImages WHERE ProductId = p.Id AND IsMainImage = 1) as ImageUrl
                    FROM CartItems ci
                    JOIN ProductVariants pv ON ci.ProductVariantId = pv.Id
                    JOIN Products p ON pv.ProductId = p.Id
                    WHERE ci.UserId = @UserId
                    ORDER BY ci.AddedAt DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new CartItem
                    {
                        Id = (int)reader["Id"],
                        UserId = userId,
                        ProductVariantId = (int)reader["ProductVariantId"],
                        Quantity = (int)reader["Quantity"],
                        AddedAt = (DateTime)reader["AddedAt"],
                        ProductName = reader["Name"].ToString(),
                        Price = (decimal)reader["Price"],
                        Attributes = reader["Attributes"].ToString(),
                        ProductImage = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : "/images/no-image.jpg"
                    });
                }
            }
            return items;
        }

        public static bool UpdateQuantity(int itemId, int quantity)
        {
            if (quantity <= 0) return RemoveItem(itemId);

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", itemId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool RemoveItem(int itemId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "DELETE FROM CartItems WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", itemId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static decimal GetTotal(Guid userId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"
                    SELECT SUM(ci.Quantity * pv.Price)
                    FROM CartItems ci
                    JOIN ProductVariants pv ON ci.ProductVariantId = pv.Id
                    WHERE ci.UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? (decimal)result : 0;
            }
        }

        public static int GetCartCount(Guid userId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT SUM(Quantity) FROM CartItems WHERE UserId = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}