using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class Order
    {
        // Propriétés
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public string Status { get; set; } // "Pending", "Processing", "Shipped", "Delivered", "Cancelled"
        public string PaymentStatus { get; set; } // "Pending", "Paid", "Failed", "Refunded"
        public string PaymentMethod { get; set; } // "CashOnDelivery", "CreditCard", "BankTransfer"
        public DateTime CreatedAt { get; set; }

        // Connexion à la base
        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        // ➤ 1) Récupérer toutes les commandes d'un utilisateur
        public static List<Order> GetByUserId(Guid userId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Orders WHERE UserId = @UserId ORDER BY CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        orders.Add(MapOrder(row));
                    }
                }
            }

            return orders;
        }

        // ➤ 2) Récupérer une commande par ID
        public static Order GetById(long id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Orders WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    return MapOrder(dt.Rows[0]);
                }
            }
        }

        // ➤ 3) Créer une nouvelle commande
        public static long Create(Guid userId, decimal total, decimal tax, decimal shipping, string paymentMethod)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"INSERT INTO Orders (UserId, Total, Tax, Shipping, Status, PaymentStatus, PaymentMethod, CreatedAt) 
                                 OUTPUT INSERTED.Id
                                 VALUES (@UserId, @Total, @Tax, @Shipping, 'Pending', 'Pending', @PaymentMethod, @CreatedAt)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@Tax", tax);
                cmd.Parameters.AddWithValue("@Shipping", shipping);
                cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod ?? "CashOnDelivery");
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                con.Open();
                return (long)cmd.ExecuteScalar();
            }
        }

        // ➤ 4) Mettre à jour le statut d'une commande
        public static bool UpdateStatus(long id, string status)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE Orders SET Status = @Status WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ➤ 5) Générer un numéro de commande unique (Optionnel si non stocké en base, ou pour affichage)
        public static string GenerateOrderNumber()
        {
            return "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        // Helper: Mapper DataRow vers Order
        private static Order MapOrder(DataRow row)
        {
            return new Order
            {
                Id = Convert.ToInt64(row["Id"]),
                UserId = Guid.Parse(row["UserId"].ToString()),
                Total = (decimal)row["Total"],
                Tax = row["Tax"] != DBNull.Value ? (decimal)row["Tax"] : 0,
                Shipping = row["Shipping"] != DBNull.Value ? (decimal)row["Shipping"] : 0,
                Status = row["Status"].ToString(),
                PaymentStatus = row["PaymentStatus"] != DBNull.Value ? row["PaymentStatus"].ToString() : "Pending",
                PaymentMethod = row["PaymentMethod"] != DBNull.Value ? row["PaymentMethod"].ToString() : "CashOnDelivery",
                CreatedAt = (DateTime)row["CreatedAt"]
            };
        }
    }
}
