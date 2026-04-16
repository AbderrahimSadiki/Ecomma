using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public string Attributes { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static List<ProductVariant> GetByProductId(int productId)
        {
            List<ProductVariant> variants = new List<ProductVariant>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM ProductVariants WHERE ProductId = @ProductId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        variants.Add(new ProductVariant
                        {
                            Id = (int)row["Id"],
                            ProductId = (int)row["ProductId"],
                            SKU = row["SKU"].ToString(),
                            Attributes = row["Attributes"].ToString(),
                            Price = (decimal)row["Price"],
                            StockQuantity = (int)row["StockQuantity"]
                        });
                    }
                }
            }

            return variants;
        }

        public static ProductVariant GetById(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM ProductVariants WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    DataRow row = dt.Rows[0];
                    return new ProductVariant
                    {
                        Id = (int)row["Id"],
                        ProductId = (int)row["ProductId"],
                        SKU = row["SKU"].ToString(),
                        Attributes = row["Attributes"].ToString(),
                        Price = (decimal)row["Price"],
                        StockQuantity = (int)row["StockQuantity"]
                    };
                }
            }
        }
        public static int Create(int productId, string sku, string attributes, decimal price, int stockQuantity)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"INSERT INTO ProductVariants (ProductId, SKU, Attributes, Price, StockQuantity) 
                                 OUTPUT INSERTED.Id
                                 VALUES (@ProductId, @SKU, @Attributes, @Price, @StockQuantity)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Attributes", attributes);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@StockQuantity", stockQuantity);

                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
