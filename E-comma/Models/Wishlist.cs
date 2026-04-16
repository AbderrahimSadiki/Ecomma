using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime AddedAt { get; set; }

        // Joined data
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }
        public decimal BasePrice { get; set; }
        public string MainImageUrl { get; set; }
        public string Brand { get; set; }
    }

    public class Wishlist
    {
        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static bool AddToWishlist(Guid userId, int productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    string query = @"IF NOT EXISTS (SELECT 1 FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId)
                                     INSERT INTO Wishlist (UserId, ProductId) VALUES (@UserId, @ProductId)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@ProductId", productId);

                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur AddToWishlist: " + ex.Message);
                return false;
            }
        }

        public static bool RemoveFromWishlist(Guid userId, int productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    string query = "DELETE FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@ProductId", productId);

                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur RemoveFromWishlist: " + ex.Message);
                return false;
            }
        }

        public static List<WishlistItem> GetUserWishlist(Guid userId)
        {
            List<WishlistItem> wishlist = new List<WishlistItem>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    string query = @"SELECT w.*, p.Name as ProductName, p.Slug as ProductSlug, p.BasePrice, p.Brand,
                                     (SELECT TOP 1 ImageUrl FROM ProductImages WHERE ProductId = p.Id AND IsMainImage = 1) as MainImageUrl
                                     FROM Wishlist w
                                     INNER JOIN Products p ON w.ProductId = p.Id
                                     WHERE w.UserId = @UserId
                                     ORDER BY w.AddedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            wishlist.Add(new WishlistItem
                            {
                                Id = (int)row["Id"],
                                UserId = (Guid)row["UserId"],
                                ProductId = (int)row["ProductId"],
                                AddedAt = (DateTime)row["AddedAt"],
                                ProductName = row["ProductName"].ToString(),
                                ProductSlug = row["ProductSlug"].ToString(),
                                BasePrice = (decimal)row["BasePrice"],
                                Brand = row["Brand"].ToString(),
                                MainImageUrl = row["MainImageUrl"] != DBNull.Value ? row["MainImageUrl"].ToString() : "/images/no-image.jpg"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur GetUserWishlist: " + ex.Message);
            }

            return wishlist;
        }

        public static bool IsInWishlist(Guid userId, int productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    string query = "SELECT COUNT(*) FROM Wishlist WHERE UserId = @UserId AND ProductId = @ProductId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@ProductId", productId);

                        con.Open();
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur IsInWishlist: " + ex.Message);
                return false;
            }
        }
    }
}