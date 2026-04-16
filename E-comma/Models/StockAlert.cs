using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class StockAlert
    {
        // Champs réels de la table StockAlerts
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public int ThresholdQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastAlertDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Champs pour affichage (JOIN)
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int CurrentStock { get; set; }

        private static string ConnString =>
            System.Configuration.ConfigurationManager
                .ConnectionStrings["DefaultConnection"].ConnectionString;

        // =====================================================
        // Récupérer les alertes actives
        // =====================================================
        public static List<StockAlert> GetActiveAlerts()
        {
            List<StockAlert> alerts = new List<StockAlert>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"
                    SELECT 
                        sa.Id,
                        sa.ProductVariantId,
                        sa.ThresholdQuantity,
                        sa.IsActive,
                        sa.LastAlertDate,
                        sa.CreatedAt,
                        pv.SKU,
                        pv.Attributes AS VariantName,
                        pv.StockQuantity,
                        p.Name AS ProductName
                    FROM StockAlerts sa
                    INNER JOIN ProductVariants pv ON sa.ProductVariantId = pv.Id
                    INNER JOIN Products p ON pv.ProductId = p.Id
                    WHERE sa.IsActive = 1
                      AND pv.StockQuantity <= sa.ThresholdQuantity
                    ORDER BY sa.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        alerts.Add(new StockAlert
                        {
                            Id = (int)rd["Id"],
                            ProductVariantId = (int)rd["ProductVariantId"],
                            ThresholdQuantity = (int)rd["ThresholdQuantity"],
                            IsActive = (bool)rd["IsActive"],
                            LastAlertDate = rd["LastAlertDate"] == DBNull.Value
                                ? null
                                : (DateTime?)rd["LastAlertDate"],
                            CreatedAt = (DateTime)rd["CreatedAt"],
                            SKU = rd["SKU"].ToString(),
                            ProductName = rd["ProductName"].ToString(),
                            VariantName = rd["VariantName"].ToString(),
                            CurrentStock = (int)rd["StockQuantity"]
                        });
                    }
                }
            }

            return alerts;
        }

        // =====================================================
        // Créer une alerte de stock
        // =====================================================
        public static bool Create(int productVariantId, int threshold)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"
                    INSERT INTO StockAlerts
                    (ProductVariantId, ThresholdQuantity, IsActive, LastAlertDate, CreatedAt)
                    VALUES
                    (@ProductVariantId, @ThresholdQuantity, 1, @LastAlertDate, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ProductVariantId", productVariantId);
                    cmd.Parameters.AddWithValue("@ThresholdQuantity", threshold);
                    cmd.Parameters.AddWithValue("@LastAlertDate", DateTime.Now);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // =====================================================
        // Désactiver une alerte (résolue)
        // =====================================================
        public static bool Deactivate(int alertId)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"
                    UPDATE StockAlerts
                    SET IsActive = 0
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", alertId);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // =====================================================
        // Vérifier et créer automatiquement une alerte
        // =====================================================
        public static void CheckAndCreateAlerts(int productVariantId, int newStock)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string checkQuery = @"
                    SELECT COUNT(*)
                    FROM StockAlerts
                    WHERE ProductVariantId = @ProductVariantId
                      AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ProductVariantId", productVariantId);
                    con.Open();

                    int count = (int)cmd.ExecuteScalar();

                    if (count == 0 && newStock <= 10)
                    {
                        Create(productVariantId, 10);
                    }
                }
            }
        }
    }
}
