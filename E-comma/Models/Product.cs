using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static List<Product> GetAll()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Products ORDER BY CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        products.Add(MapProduct(row));
                    }
                }
            }

            return products;
        }

        public static Product GetById(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Products WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    return MapProduct(dt.Rows[0]);
                }
            }
        }

        public static List<Product> GetByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Products WHERE CategoryId = @CategoryId ORDER BY CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        products.Add(MapProduct(row));
                    }
                }
            }

            return products;
        }

        private static Product MapProduct(DataRow row)
        {
            return new Product
            {
                Id = (int)row["Id"],
                CategoryId = (int)row["CategoryId"],
                Name = row["Name"].ToString(),
                Slug = row["Slug"] != DBNull.Value ? row["Slug"].ToString() : "",
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : "",
                Brand = row["Brand"] != DBNull.Value ? row["Brand"].ToString() : "",
                BasePrice = row["BasePrice"] != DBNull.Value ? (decimal)row["BasePrice"] : 0,
                IsFeatured = row["IsFeatured"] != DBNull.Value ? (bool)row["IsFeatured"] : false,
                CreatedAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.Now
            };
        }
        public static int Create(Product p, ProductImage mainImage)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    string query = @"INSERT INTO Products (CategoryId, Name, Slug, Description, Brand, BasePrice, IsFeatured) 
                                     VALUES (@CategoryId, @Name, @Slug, @Description, @Brand, @BasePrice, @IsFeatured); 
                                     SELECT SCOPE_IDENTITY();";
                    
                    int newId = 0;
                    using (SqlCommand cmd = new SqlCommand(query, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", p.CategoryId);
                        cmd.Parameters.AddWithValue("@Name", p.Name);
                        cmd.Parameters.AddWithValue("@Slug", p.Slug);
                        cmd.Parameters.AddWithValue("@Description", p.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Brand", p.Brand ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BasePrice", p.BasePrice);
                        cmd.Parameters.AddWithValue("@IsFeatured", p.IsFeatured);
                        newId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (mainImage != null)
                    {
                        string imgQuery = @"INSERT INTO ProductImages (ProductId, ImageUrl, AltText, IsMainImage, DisplayOrder)
                                            VALUES (@ProductId, @ImageUrl, @AltText, 1, @DisplayOrder)";
                        using (SqlCommand cmd = new SqlCommand(imgQuery, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@ProductId", newId);
                            cmd.Parameters.AddWithValue("@ImageUrl", mainImage.ImageUrl);
                            cmd.Parameters.AddWithValue("@AltText", mainImage.AltText ?? "");
                            cmd.Parameters.AddWithValue("@DisplayOrder", mainImage.DisplayOrder);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    return newId;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public static bool Update(Product p, ProductImage mainImage)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    string query = @"UPDATE Products SET CategoryId=@CategoryId, Name=@Name, Slug=@Slug, 
                                     Description=@Description, Brand=@Brand, BasePrice=@BasePrice, IsFeatured=@IsFeatured 
                                     WHERE Id=@Id";
                    
                    using (SqlCommand cmd = new SqlCommand(query, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", p.Id);
                        cmd.Parameters.AddWithValue("@CategoryId", p.CategoryId);
                        cmd.Parameters.AddWithValue("@Name", p.Name);
                        cmd.Parameters.AddWithValue("@Slug", p.Slug);
                        cmd.Parameters.AddWithValue("@Description", p.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Brand", p.Brand ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BasePrice", p.BasePrice);
                        cmd.Parameters.AddWithValue("@IsFeatured", p.IsFeatured);
                        cmd.ExecuteNonQuery();
                    }

                    if (mainImage != null)
                    {
                        // Reset existing main image
                        string resetQuery = "UPDATE ProductImages SET IsMainImage=0 WHERE ProductId=@ProductId";
                        using (SqlCommand cmd = new SqlCommand(resetQuery, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@ProductId", p.Id);
                            cmd.ExecuteNonQuery();
                        }

                        // Insert new or update logic could be complex, for simplicity we insert new main image
                        // or if we want to just update the 'main' one, we might need more logic.
                        // Here we assume we just add a new image record as main for simplicity or update if we had ID.
                        // But the Dashboard code passes a new object. Let's just insert it as main.
                        string imgQuery = @"INSERT INTO ProductImages (ProductId, ImageUrl, AltText, IsMainImage, DisplayOrder)
                                            VALUES (@ProductId, @ImageUrl, @AltText, 1, @DisplayOrder)";
                        using (SqlCommand cmd = new SqlCommand(imgQuery, con, trans))
                        {
                            cmd.Parameters.AddWithValue("@ProductId", p.Id);
                            cmd.Parameters.AddWithValue("@ImageUrl", mainImage.ImageUrl);
                            cmd.Parameters.AddWithValue("@AltText", mainImage.AltText ?? "");
                            cmd.Parameters.AddWithValue("@DisplayOrder", mainImage.DisplayOrder);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public static bool Delete(int id, out string error)
        {
            error = string.Empty;
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try
                {
                    // 1. Delete Images
                    string cmdImages = "DELETE FROM ProductImages WHERE ProductId=@Id";
                    using (SqlCommand cmd = new SqlCommand(cmdImages, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Delete Product
                    string query = "DELETE FROM Products WHERE Id=@Id";
                    using (SqlCommand cmd = new SqlCommand(query, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        int rows = cmd.ExecuteNonQuery();
                        trans.Commit();
                        return rows > 0;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    error = ex.Message;
                    return false;
                }
            }
        }
    }
}