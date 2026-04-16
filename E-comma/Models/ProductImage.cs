using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMainImage { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static List<ProductImage> GetByProductId(int productId)
        {
            List<ProductImage> images = new List<ProductImage>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM ProductImages WHERE ProductId = @ProductId ORDER BY DisplayOrder";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        images.Add(new ProductImage
                        {
                            Id = (int)row["Id"],
                            ProductId = (int)row["ProductId"],
                            ImageUrl = row["ImageUrl"].ToString(),
                            AltText = row["AltText"].ToString(),
                            DisplayOrder = (int)row["DisplayOrder"],
                            IsMainImage = (bool)row["IsMainImage"]
                        });
                    }
                }
            }

            return images;
        }

        public static string GetMainImage(int productId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT TOP 1 ImageUrl FROM ProductImages WHERE ProductId = @ProductId AND IsMainImage = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "/images/no-image.jpg";
                }
            }
        }

        public static ProductImage GetMainImageRecord(int productId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT TOP 1 * FROM ProductImages WHERE ProductId = @ProductId AND IsMainImage = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    DataRow row = dt.Rows[0];
                    return new ProductImage
                    {
                        Id = (int)row["Id"],
                        ProductId = (int)row["ProductId"],
                        ImageUrl = row["ImageUrl"].ToString(),
                        AltText = row["AltText"].ToString(),
                        DisplayOrder = (int)row["DisplayOrder"],
                        IsMainImage = (bool)row["IsMainImage"]
                    };
                }
            }
        }
    }
}
