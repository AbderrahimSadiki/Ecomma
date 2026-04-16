using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace E_comma.Models
{
    public class ProductExtended
    {
        // Propriétés de base du produit
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propriétés étendues
        public string CategoryName { get; set; }
        public string MainImageUrl { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ProductVariant> Variants { get; set; }
        public List<ProductImage> Images { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        public static List<ProductExtended> GetAllForShop(
            int? categoryId = null,
            string search = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string brand = null,
            bool? inStock = null)
        {
            List<ProductExtended> products = new List<ProductExtended>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT p.*, c.Name as CategoryName,
                                 (SELECT TOP 1 ImageUrl FROM ProductImages WHERE ProductId = p.Id AND IsMainImage = 1) as MainImage
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 WHERE 1=1";

                if (categoryId.HasValue)
                {
                    query += " AND p.CategoryId = @CategoryId";
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query += " AND (p.Name LIKE @Search OR p.Brand LIKE @Search OR p.Description LIKE @Search)";
                }

                if (minPrice.HasValue)
                {
                    query += " AND p.BasePrice >= @MinPrice";
                }

                if (maxPrice.HasValue)
                {
                    query += " AND p.BasePrice <= @MaxPrice";
                }

                if (!string.IsNullOrWhiteSpace(brand))
                {
                    query += " AND p.Brand = @Brand";
                }

                if (inStock.HasValue && inStock.Value)
                {
                    query += " AND EXISTS (SELECT 1 FROM ProductVariants pv WHERE pv.ProductId = p.Id AND pv.StockQuantity > 0)";
                }

                query += " ORDER BY p.IsFeatured DESC, p.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (categoryId.HasValue)
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId.Value);

                    if (!string.IsNullOrWhiteSpace(search))
                        cmd.Parameters.AddWithValue("@Search", "%" + search + "%");

                    if (minPrice.HasValue)
                        cmd.Parameters.AddWithValue("@MinPrice", minPrice.Value);

                    if (maxPrice.HasValue)
                        cmd.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);

                    if (!string.IsNullOrWhiteSpace(brand))
                        cmd.Parameters.AddWithValue("@Brand", brand);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        int productId = (int)row["Id"];

                        products.Add(new ProductExtended
                        {
                            Id = productId,
                            CategoryId = (int)row["CategoryId"],
                            Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "",
                            Slug = row["Slug"] != DBNull.Value ? row["Slug"].ToString() : "",
                            Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : "",
                            Brand = row["Brand"] != DBNull.Value ? row["Brand"].ToString() : "",
                            BasePrice = row["BasePrice"] != DBNull.Value ? (decimal)row["BasePrice"] : 0,
                            IsFeatured = row["IsFeatured"] != DBNull.Value ? (bool)row["IsFeatured"] : false,
                            CreatedAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.Now,
                            CategoryName = row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : "",
                            MainImageUrl = row["MainImage"] != DBNull.Value ? row["MainImage"].ToString() : "/images/no-image.jpg",
                            AverageRating = ProductReview.GetAverageRating(productId),
                            ReviewCount = ProductReview.GetReviewCount(productId)
                        });
                    }
                }
            }

            return products;
        }

        public static List<ProductExtended> GetCatalogSnapshot(int maxResults = 30)
        {
            var products = new List<ProductExtended>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT TOP (@MaxResults) p.Id, p.CategoryId, p.Name, p.Brand, p.BasePrice,
                                 p.IsFeatured, p.CreatedAt, c.Name as CategoryName
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 ORDER BY p.IsFeatured DESC, p.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MaxResults", maxResults);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        products.Add(new ProductExtended
                        {
                            Id = (int)row["Id"],
                            CategoryId = (int)row["CategoryId"],
                            Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "",
                            Brand = row["Brand"] != DBNull.Value ? row["Brand"].ToString() : "",
                            BasePrice = row["BasePrice"] != DBNull.Value ? (decimal)row["BasePrice"] : 0,
                            IsFeatured = row["IsFeatured"] != DBNull.Value ? (bool)row["IsFeatured"] : false,
                            CreatedAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.Now,
                            CategoryName = row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : ""
                        });
                    }
                }
            }

            return products;
        }

        public static List<ProductExtended> SearchForChatbot(IEnumerable<string> keywords, int maxResults = 6)
        {
            var products = new List<ProductExtended>();

            var terms = (keywords ?? Enumerable.Empty<string>())
                .Select(t => t == null ? string.Empty : t.Trim())
                .Where(t => t.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(6)
                .ToList();

            if (terms.Count == 0)
            {
                return products;
            }

            if (maxResults <= 0)
            {
                maxResults = 6;
            }

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                var conditions = new List<string>();
                for (int i = 0; i < terms.Count; i++)
                {
                    conditions.Add("(p.Name LIKE @Term" + i + " OR p.Brand LIKE @Term" + i +
                                   " OR p.Description LIKE @Term" + i + " OR c.Name LIKE @Term" + i + ")");
                }

                string query = @"SELECT TOP (@MaxResults) p.Id, p.CategoryId, p.Name, p.Brand, p.BasePrice,
                                 p.IsFeatured, p.CreatedAt, c.Name as CategoryName
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 WHERE " + string.Join(" OR ", conditions) + @"
                                 ORDER BY p.IsFeatured DESC, p.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MaxResults", maxResults);
                    for (int i = 0; i < terms.Count; i++)
                    {
                        cmd.Parameters.AddWithValue("@Term" + i, "%" + terms[i] + "%");
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        products.Add(new ProductExtended
                        {
                            Id = (int)row["Id"],
                            CategoryId = (int)row["CategoryId"],
                            Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "",
                            Brand = row["Brand"] != DBNull.Value ? row["Brand"].ToString() : "",
                            BasePrice = row["BasePrice"] != DBNull.Value ? (decimal)row["BasePrice"] : 0,
                            IsFeatured = row["IsFeatured"] != DBNull.Value ? (bool)row["IsFeatured"] : false,
                            CreatedAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.Now,
                            CategoryName = row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : ""
                        });
                    }
                }
            }

            return products;
        }

        public static ProductExtended GetByNameOrLike(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT TOP 1 p.*, c.Name as CategoryName
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 WHERE p.Name = @Name OR p.Name LIKE @NameLike
                                 ORDER BY CASE WHEN p.Name = @Name THEN 0 ELSE 1 END,
                                          p.IsFeatured DESC, p.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@NameLike", "%" + name + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        return null;
                    }

                    DataRow row = dt.Rows[0];

                    return new ProductExtended
                    {
                        Id = (int)row["Id"],
                        CategoryId = (int)row["CategoryId"],
                        Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "",
                        Slug = row["Slug"] != DBNull.Value ? row["Slug"].ToString() : "",
                        Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : "",
                        Brand = row["Brand"] != DBNull.Value ? row["Brand"].ToString() : "",
                        BasePrice = row["BasePrice"] != DBNull.Value ? (decimal)row["BasePrice"] : 0,
                        IsFeatured = row["IsFeatured"] != DBNull.Value ? (bool)row["IsFeatured"] : false,
                        CreatedAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.Now,
                        CategoryName = row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : ""
                    };
                }
            }
        }


        public static List<string> GetBrands()
        {
            var brands = new List<string>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT DISTINCT Brand
                                 FROM Products
                                 WHERE Brand IS NOT NULL AND LTRIM(RTRIM(Brand)) <> ''
                                 ORDER BY Brand";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        brands.Add(row["Brand"].ToString());
                    }
                }
            }

            return brands;
        }

        public static List<string> GetSearchSuggestions(int maxResults = 12)
        {
            var suggestions = new List<string>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT DISTINCT TOP (@MaxResults) Value
                                 FROM (
                                     SELECT Name AS Value FROM Products
                                     UNION
                                     SELECT Brand AS Value FROM Products
                                 ) AS suggestions
                                 WHERE Value IS NOT NULL AND LTRIM(RTRIM(Value)) <> ''
                                 ORDER BY Value";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MaxResults", maxResults);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        suggestions.Add(row["Value"].ToString());
                    }
                }
            }

            return suggestions;
        }

        public static ProductExtended GetDetailedById(int id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT p.*, c.Name as CategoryName
                                 FROM Products p
                                 INNER JOIN Categories c ON p.CategoryId = c.Id
                                 WHERE p.Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    DataRow row = dt.Rows[0];

                    var product = new ProductExtended
                    {
                        Id = (int)row["Id"],
                        CategoryId = (int)row["CategoryId"],
                        Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "",
                        Slug = row["Slug"] != DBNull.Value ? row["Slug"].ToString() : "",
                        Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : "",
                        Brand = row["Brand"] != DBNull.Value ? row["Brand"].ToString() : "",
                        BasePrice = row["BasePrice"] != DBNull.Value ? (decimal)row["BasePrice"] : 0,
                        IsFeatured = row["IsFeatured"] != DBNull.Value ? (bool)row["IsFeatured"] : false,
                        CreatedAt = row["CreatedAt"] != DBNull.Value ? (DateTime)row["CreatedAt"] : DateTime.Now,
                        CategoryName = row["CategoryName"] != DBNull.Value ? row["CategoryName"].ToString() : "",
                        AverageRating = ProductReview.GetAverageRating(id),
                        ReviewCount = ProductReview.GetReviewCount(id),
                        Variants = ProductVariant.GetByProductId(id),
                        Images = ProductImage.GetByProductId(id)
                    };

                    if (product.Images != null && product.Images.Count > 0)
                        product.MainImageUrl = product.Images[0].ImageUrl;
                    else
                        product.MainImageUrl = "/images/no-image.jpg";

                    return product;
                }
            }
        }
    }
}
