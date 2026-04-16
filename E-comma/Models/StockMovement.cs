using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace E_comma.Models
{
    public class StockMovement
    {
        public long Id { get; set; }
        public int ProductVariantId { get; set; }
        public string MovementType { get; set; } // IN, OUT, ADJUSTMENT, ORDER, RETURN
        public int Quantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propriétés étendues
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }

        private static string ConnString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["DefaultConnection"].ConnectionString;

        // Récupérer l'historique des mouvements
        public static List<StockMovement> GetAll(int? productVariantId = null, int limit = 100)
        {
            List<StockMovement> movements = new List<StockMovement>();

            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string query = @"SELECT TOP (@Limit) sm.*, pv.SKU, pv.Attributes, p.Name AS ProductName
                                 FROM StockMovements sm
                                 INNER JOIN ProductVariants pv ON sm.ProductVariantId = pv.Id
                                 INNER JOIN Products p ON pv.ProductId = p.Id";

                if (productVariantId.HasValue)
                {
                    query += " WHERE sm.ProductVariantId = @ProductVariantId";
                }

                query += " ORDER BY sm.CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Limit", limit);
                    if (productVariantId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@ProductVariantId", productVariantId.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        movements.Add(new StockMovement
                        {
                            Id = Convert.ToInt64(row["Id"]),
                            ProductVariantId = (int)row["ProductVariantId"],
                            MovementType = row["MovementType"].ToString(),
                            Quantity = (int)row["Quantity"],
                            PreviousStock = (int)row["PreviousStock"],
                            NewStock = (int)row["NewStock"],
                            Reference = row["Reference"] != DBNull.Value ? row["Reference"].ToString() : "",
                            Notes = row["Notes"] != DBNull.Value ? row["Notes"].ToString() : "",
                            CreatedBy = row["CreatedBy"] != DBNull.Value ? (Guid?)row["CreatedBy"] : null,
                            CreatedAt = (DateTime)row["CreatedAt"],
                            SKU = row["SKU"].ToString(),
                            ProductName = row["ProductName"].ToString(),
                            VariantName = row["Attributes"].ToString()
                        });
                    }
                }
            }

            return movements;
        }

        // Enregistrer un mouvement et mettre à jour le stock
        public static bool Create(int productVariantId, string movementType, int quantity, string reference, string notes, Guid? createdBy)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // Récupérer le stock actuel
                    string getStockQuery = "SELECT StockQuantity FROM ProductVariants WHERE Id = @Id";
                    int currentStock = 0;

                    using (SqlCommand cmd = new SqlCommand(getStockQuery, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@Id", productVariantId);
                        object result = cmd.ExecuteScalar();
                        currentStock = result != DBNull.Value ? (int)result : 0;
                    }

                    // Calculer le nouveau stock
                    int newStock = currentStock;
                    if (movementType == "IN" || movementType == "RETURN")
                    {
                        newStock += quantity;
                    }
                    else if (movementType == "OUT" || movementType == "ORDER" || movementType == "ADJUSTMENT")
                    {
                        newStock -= quantity;
                    }

                    // Empêcher le stock négatif
                    if (newStock < 0) newStock = 0;

                    // Enregistrer le mouvement
                    string insertQuery = @"INSERT INTO StockMovements 
                                          (ProductVariantId, MovementType, Quantity, PreviousStock, NewStock, Reference, Notes, CreatedBy) 
                                          VALUES (@ProductVariantId, @MovementType, @Quantity, @PreviousStock, @NewStock, @Reference, @Notes, @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@ProductVariantId", productVariantId);
                        cmd.Parameters.AddWithValue("@MovementType", movementType);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@PreviousStock", currentStock);
                        cmd.Parameters.AddWithValue("@NewStock", newStock);
                        cmd.Parameters.AddWithValue("@Reference", reference ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Notes", notes ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    // Mettre à jour le stock dans ProductVariants
                    string updateStockQuery = "UPDATE ProductVariants SET StockQuantity = @NewStock WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(updateStockQuery, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@NewStock", newStock);
                        cmd.Parameters.AddWithValue("@Id", productVariantId);
                        cmd.ExecuteNonQuery();
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

        // Enregistrer un mouvement de commande (appelé lors de la création d'une commande)
        public static bool CreateOrderMovement(int productVariantId, int quantity, long orderId, Guid? userId)
        {
            return Create(
                productVariantId,
                "ORDER",
                quantity,
                "ORDER-" + orderId,
                "Commande #" + orderId,
                userId
            );
        }

        // Enregistrer un retour de stock (commande annulée)
        public static bool CreateReturnMovement(int productVariantId, int quantity, long orderId, Guid? userId)
        {
            return Create(
                productVariantId,
                "RETURN",
                quantity,
                "RETURN-ORDER-" + orderId,
                "Retour de commande #" + orderId,
                userId
            );
        }
    }
}