using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class DeliveryMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDays { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        private static string CS =>
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // ===================== GET ALL =====================
        public static List<DeliveryMethod> GetAll()
        {
            var list = new List<DeliveryMethod>();

            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT * FROM DeliveryMethods ORDER BY DisplayOrder", con))
            {
                con.Open();
                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(Map(r));
                }
            }
            return list;
        }

        // ===================== GET ACTIVE =====================
        public static List<DeliveryMethod> GetActive()
        {
            var list = new List<DeliveryMethod>();

            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(
                "SELECT * FROM DeliveryMethods WHERE IsActive = 1 ORDER BY DisplayOrder", con))
            {
                con.Open();
                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(Map(r));
                }
            }
            return list;
        }

        // ===================== GET BY ID =====================
        public static DeliveryMethod GetById(int id)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(
                "SELECT * FROM DeliveryMethods WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                var r = cmd.ExecuteReader();
                if (r.Read())
                    return Map(r);
            }
            return null;
        }

        // ===================== CREATE =====================
        public static int Create(string name, string description, decimal price,
                                 int estimatedDays, bool isActive, int displayOrder)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
                INSERT INTO DeliveryMethods
                (Name, Description, Price, EstimatedDays, IsActive, DisplayOrder)
                OUTPUT INSERTED.Id
                VALUES (@N,@D,@P,@E,@A,@O)", con))
            {
                cmd.Parameters.AddWithValue("@N", name);
                cmd.Parameters.AddWithValue("@D", description);
                cmd.Parameters.AddWithValue("@P", price);
                cmd.Parameters.AddWithValue("@E", estimatedDays);
                cmd.Parameters.AddWithValue("@A", isActive);
                cmd.Parameters.AddWithValue("@O", displayOrder);

                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // ===================== UPDATE =====================
        public static bool Update(int id, string name, string description,
                                  decimal price, int estimatedDays,
                                  bool isActive, int displayOrder)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
                UPDATE DeliveryMethods SET
                    Name=@N,
                    Description=@D,
                    Price=@P,
                    EstimatedDays=@E,
                    IsActive=@A,
                    DisplayOrder=@O
                WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@N", name);
                cmd.Parameters.AddWithValue("@D", description);
                cmd.Parameters.AddWithValue("@P", price);
                cmd.Parameters.AddWithValue("@E", estimatedDays);
                cmd.Parameters.AddWithValue("@A", isActive);
                cmd.Parameters.AddWithValue("@O", displayOrder);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ===================== DELETE =====================
        public static bool Delete(int id)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(
                "DELETE FROM DeliveryMethods WHERE Id=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ===================== MAPPER =====================
        private static DeliveryMethod Map(SqlDataReader r)
        {
            return new DeliveryMethod
            {
                Id = (int)r["Id"],
                Name = r["Name"].ToString(),
                Description = r["Description"].ToString(),
                Price = (decimal)r["Price"],
                EstimatedDays = (int)r["EstimatedDays"],
                DisplayOrder = (int)r["DisplayOrder"],
                IsActive = (bool)r["IsActive"]
            };
        }
        public static decimal CalculateDeliveryPrice(int methodId, string city)
        {
            var method = GetById(methodId);
            if (method == null)
                return 0;

            // Plus tard tu peux adapter selon la ville
            return method.Price;
        }

    }
}
