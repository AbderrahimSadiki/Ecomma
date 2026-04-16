using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace E_comma.Models
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }

        private static string ConnString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static void Create(Guid userId, Guid token, DateTime expiry)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "INSERT INTO PasswordResetTokens (UserId, Token, ExpiryDate) VALUES (@UserId, @Token, @ExpiryDate)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiry);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static PasswordResetToken GetByToken(Guid token)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM PasswordResetTokens WHERE Token = @Token AND IsUsed = 0 AND ExpiryDate > GETDATE()";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PasswordResetToken
                            {
                                Id = (Guid)reader["Id"],
                                UserId = (Guid)reader["UserId"],
                                Token = (Guid)reader["Token"],
                                ExpiryDate = (DateTime)reader["ExpiryDate"],
                                IsUsed = (bool)reader["IsUsed"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static void MarkAsUsed(Guid token)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE PasswordResetTokens SET IsUsed = 1 WHERE Token = @Token";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
