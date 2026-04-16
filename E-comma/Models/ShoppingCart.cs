using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class ShoppingCart
    {
        // Propriétés
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Connexion à la base
        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        // ➤ 1) Récupérer le panier d'un utilisateur
        public static ShoppingCart GetByUserId(Guid userId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM ShoppingCarts WHERE UserId = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    DataRow row = dt.Rows[0];

                    return new ShoppingCart
                    {
                        Id = Guid.Parse(row["Id"].ToString()),
                        UserId = Guid.Parse(row["UserId"].ToString()),
                        CreatedAt = (DateTime)row["CreatedAt"]
                    };
                }
            }
        }

        // ➤ 2) Créer un panier pour un utilisateur
        public static Guid Create(Guid userId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"INSERT INTO ShoppingCarts (UserId) 
                                 OUTPUT INSERTED.Id
                                 VALUES (@UserId)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                return (Guid)cmd.ExecuteScalar();
            }
        }

        // ➤ 3) Récupérer ou créer le panier d'un utilisateur
        public static ShoppingCart GetOrCreate(Guid userId)
        {
            ShoppingCart cart = GetByUserId(userId);

            if (cart == null)
            {
                Guid cartId = Create(userId);
                cart = new ShoppingCart
                {
                    Id = cartId,
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };
            }

            return cart;
        }

        // ➤ 4) Vider le panier (supprimer tous les items)
        public static bool Clear(Guid cartId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "DELETE FROM ShoppingCartItems WHERE ShoppingCartId = @CartId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CartId", cartId);

                con.Open();
                return cmd.ExecuteNonQuery() >= 0;
            }
        }
    }
}
