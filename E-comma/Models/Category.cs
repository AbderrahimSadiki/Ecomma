using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static List<Category> GetAll()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Categories ORDER BY Name";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        categories.Add(new Category
                        {
                            Id = (int)row["Id"],
                            Name = row["Name"].ToString(),
                            Slug = row["Slug"].ToString(),
                            ParentId = row["ParentId"] != DBNull.Value ? (int?)row["ParentId"] : null
                        });
                    }
                }
            }

            return categories;
        }

        public static Category GetById(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Categories WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    DataRow row = dt.Rows[0];

                    return new Category
                    {
                        Id = (int)row["Id"],
                        Name = row["Name"].ToString(),
                        Slug = row["Slug"].ToString(),
                        ParentId = row["ParentId"] != DBNull.Value ? (int?)row["ParentId"] : null
                    };
                }
            }
        }
        public static int Create(string name, string slug, int? parentId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "INSERT INTO Categories (Name, Slug, ParentId) VALUES (@Name, @Slug, @ParentId); SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Slug", slug);
                    cmd.Parameters.AddWithValue("@ParentId", parentId ?? (object)DBNull.Value);
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static bool Update(int id, string name, string slug, int? parentId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE Categories SET Name=@Name, Slug=@Slug, ParentId=@ParentId WHERE Id=@Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Slug", slug);
                    cmd.Parameters.AddWithValue("@ParentId", parentId ?? (object)DBNull.Value);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                try 
                {
                    // 1. Get Products to delete their images/dependencies if strict, 
                    // OR just delete Products if we rely on Product.Delete handling it?
                    // Product.Delete isn't static transaction aware easily unless we refactor.
                    // For "ON DELETE CASCADE" in SQL simulation:
                    
                    // A. Delete ProductImages for all products in this category
                    string cmdImages = @"DELETE FROM ProductImages WHERE ProductId IN (SELECT Id FROM Products WHERE CategoryId=@Id)";
                    using (SqlCommand cmd = new SqlCommand(cmdImages, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // B. Delete Products
                    string cmdProducts = @"DELETE FROM Products WHERE CategoryId=@Id";
                    using (SqlCommand cmd = new SqlCommand(cmdProducts, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // C. Delete Category
                    string query = "DELETE FROM Categories WHERE Id=@Id";
                    using (SqlCommand cmd = new SqlCommand(query, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        int rows = cmd.ExecuteNonQuery();
                        
                        trans.Commit();
                        return rows > 0;
                    }
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}