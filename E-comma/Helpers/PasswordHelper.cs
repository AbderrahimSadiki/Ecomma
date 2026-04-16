using System;
using System.Security.Cryptography;
using System.Text;

namespace E_comma.Helpers
{
    public static class PasswordHelper
    {
        // Fonction pour hasher un mot de passe (SHA256)
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        // Vérifier si le mot de passe entré correspond au hash stocké
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string hashOfInput = HashPassword(enteredPassword);
            return hashOfInput.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
