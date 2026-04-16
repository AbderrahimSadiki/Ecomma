using System;
using System.Data;
using System.Data.SqlClient;
using E_comma.Helpers; // pour PasswordHelper

namespace E_comma.Models
{
    public class User
    {
        // propriétés correspondant aux colonnes
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } // Ajout de la propriété Role
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // connexion à la base (lue dans web.config)
        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        // ➤ 1) Récupérer un utilisateur par email
        public static User GetByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Users WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0) return null;

                    DataRow row = dt.Rows[0];

                    return new User
                    {
                        Id = Guid.Parse(row["Id"].ToString()),
                        Email = row["Email"].ToString(),
                        Phone = row["Phone"].ToString(),
                        Name = row["Name"].ToString(),
                        LastName = row["LastName"].ToString(),
                        Role = row.Table.Columns.Contains("Role") ? row["Role"].ToString() : "Client", // Gestion sécure
                        PasswordHash = row["PasswordHash"].ToString(),
                        IsActive = (bool)row["IsActive"],
                        CreatedAt = (DateTime)row["CreatedAt"]
                    };
                }
            }
        }

        // ➤ 2) Inscrire un utilisateur
        public static bool Register(string email, string phone, string firstName, string lastName, string password)
        {
            string hash = PasswordHelper.HashPassword(password);

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"INSERT INTO Users (Email, Phone, Name, LastName, PasswordHash, Role)
                                 VALUES (@Email, @Phone, @Name, @LastName, @PasswordHash, 'Client')";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Name", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@PasswordHash", hash);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ➤ 3) Vérifier Login
        public static bool CheckLogin(string email, string password)
        {
            User user = GetByEmail(email);

            if (user == null) return false;

            return PasswordHelper.VerifyPassword(password, user.PasswordHash);
        }

        // ➤ 4) Mettre à jour le mot de passe
        public static bool UpdatePassword(string email, string newPassword)
        {
            string hash = PasswordHelper.HashPassword(newPassword);

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PasswordHash", hash);
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool UpdatePassword(Guid userId, string newPassword)
        {
            string hash = PasswordHelper.HashPassword(newPassword);

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PasswordHash", hash);
                    cmd.Parameters.AddWithValue("@Id", userId);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        // ➤ 5) Récupérer tous les utilisateurs
        public static System.Collections.Generic.List<User> GetAll()
        {
            var list = new System.Collections.Generic.List<User>();
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Users ORDER BY CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new User
                            {
                                Id = (Guid)rdr["Id"],
                                Email = rdr["Email"].ToString(),
                                Phone = rdr["Phone"] != DBNull.Value ? rdr["Phone"].ToString() : "",
                                Name = rdr["Name"] != DBNull.Value ? rdr["Name"].ToString() : "",
                                LastName = rdr["LastName"] != DBNull.Value ? rdr["LastName"].ToString() : "",
                                Role = HasColumn(rdr, "Role") ? rdr["Role"].ToString() : "Client",
                                IsActive = (bool)rdr["IsActive"],
                                CreatedAt = (DateTime)rdr["CreatedAt"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        private static bool HasColumn(SqlDataReader dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static User GetById(Guid id)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "SELECT * FROM Users WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            return new User
                            {
                                Id = (Guid)rdr["Id"],
                                Email = rdr["Email"].ToString(),
                                Phone = rdr["Phone"] != DBNull.Value ? rdr["Phone"].ToString() : "",
                                Name = rdr["Name"] != DBNull.Value ? rdr["Name"].ToString() : "",
                                LastName = rdr["LastName"] != DBNull.Value ? rdr["LastName"].ToString() : "",
                                IsActive = (bool)rdr["IsActive"],
                                CreatedAt = (DateTime)rdr["CreatedAt"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static bool Create(string email, string phone, string firstName, string lastName, string password, bool isActive, out string error)
        {
            error = string.Empty;
            string hash = PasswordHelper.HashPassword(password);
            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    string query = @"INSERT INTO Users (Email, Phone, Name, LastName, PasswordHash, IsActive) 
                                     VALUES (@Email, @Phone, @Name, @LastName, @PasswordHash, @IsActive)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Name", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@PasswordHash", hash);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool Update(Guid id, string email, string phone, string firstName, string lastName, bool isActive, string password, out string error)
        {
            error = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    string query = @"UPDATE Users SET Email=@Email, Phone=@Phone, Name=@Name, LastName=@LastName, IsActive=@IsActive" +
                                   (!string.IsNullOrEmpty(password) ? ", PasswordHash=@PasswordHash" : "") +
                                   " WHERE Id=@Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Name", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        if (!string.IsNullOrEmpty(password))
                            cmd.Parameters.AddWithValue("@PasswordHash", PasswordHelper.HashPassword(password));

                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool Delete(Guid id, out string error)
        {
            error = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnString))
                {
                    // Check dependencies if needed, or cascade delete
                    string query = "DELETE FROM Users WHERE Id=@Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool ToggleActive(Guid id, bool isActive)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = "UPDATE Users SET IsActive=@IsActive WHERE Id=@Id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
