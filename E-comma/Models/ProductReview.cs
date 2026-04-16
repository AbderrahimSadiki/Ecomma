using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static List<ProductReview> GetByProductId(int productId)
        {
            List<ProductReview> reviews = new List<ProductReview>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT pr.*, u.Name + ' ' + u.LastName AS UserName 
                                 FROM ProductReviews pr 
                                 INNER JOIN Users u ON pr.UserId = u.Id 
                                 WHERE pr.ProductId = @ProductId 
                                 ORDER BY pr.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        reviews.Add(new ProductReview
                        {
                            Id = (int)row["Id"],
                            ProductId = (int)row["ProductId"],
                            UserId = Guid.Parse(row["UserId"].ToString()),
                            Rating = (int)row["Rating"],
                            Comment = row["Comment"].ToString(),
                            IsVerifiedPurchase = (bool)row["IsVerifiedPurchase"],
                            CreatedAt = (DateTime)row["CreatedAt"],
                            UserName = row["UserName"].ToString()
                        });
                    }
                }
            }

            return reviews;
        }

        public static double GetAverageRating(int productId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT AVG(CAST(Rating AS FLOAT)) FROM ProductReviews WHERE ProductId = @ProductId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }
            }
        }

        public static int GetReviewCount(int productId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT COUNT(*) FROM ProductReviews WHERE ProductId = @ProductId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    con.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public static bool Create(int productId, Guid userId, int rating, string comment)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                // Vérifier si l'utilisateur a acheté le produit
                string checkQuery = @"SELECT COUNT(*) FROM OrderItems oi 
                                      INNER JOIN Orders o ON oi.OrderId = o.Id 
                                      INNER JOIN ProductVariants pv ON oi.ProductVariantId = pv.Id 
                                      WHERE o.UserId = @UserId AND pv.ProductId = @ProductId 
                                      AND o.Status = 'Delivered'";

                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);

                con.Open();
                int purchaseCount = (int)checkCmd.ExecuteScalar();
                bool isVerified = purchaseCount > 0;

                string query = @"INSERT INTO ProductReviews (ProductId, UserId, Rating, Comment, IsVerifiedPurchase) 
                                 VALUES (@ProductId, @UserId, @Rating, @Comment, @IsVerified)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Rating", rating);
                cmd.Parameters.AddWithValue("@Comment", comment ?? "");
                cmd.Parameters.AddWithValue("@IsVerified", isVerified);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}