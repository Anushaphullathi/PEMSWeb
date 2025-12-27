
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PEMS.Web.DAL.Interfaces;
using PEMS.Web.Models;

namespace PEMS.Web.DAL.Sql
{
    public class SqlTransactionRepository : ITransactionRepository
    {
        private readonly string _conn = ConfigurationManager.ConnectionStrings["PEMSDb"].ConnectionString;

        public IEnumerable<Transaction> GetTransactions(TransactionFilter f)
        {
            var list = new List<Transaction>();
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT t.TransactionId, t.UserId, t.CategoryId, t.Amount, t.TransactionType,
                         t.PaymentMethod, t.Notes, t.TxnDate, t.CreatedOn
                  FROM dbo.Transactions t
                  WHERE t.UserId = @UserId
                    AND (@Type IS NULL OR t.TransactionType = @Type)
                    AND (@From IS NULL OR t.TxnDate >= @From)
                    AND (@To   IS NULL OR t.TxnDate <= @To)
                    AND (@CategoryId IS NULL OR t.CategoryId = @CategoryId)
                    AND (@MinAmt IS NULL OR t.Amount >= @MinAmt)
                    AND (@MaxAmt IS NULL OR t.Amount <= @MaxAmt)
                    AND (@PayMethod IS NULL OR t.PaymentMethod = @PayMethod)
                  ORDER BY t.TxnDate DESC, t.TransactionId DESC;", cn))
            {
                cmd.Parameters.AddWithValue("@UserId", f.UserId);
                cmd.Parameters.AddWithValue("@Type", (object)f.TransactionType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@From", (object)f.From ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@To", (object)f.To ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryId", (object)f.CategoryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MinAmt", (object)f.MinAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxAmt", (object)f.MaxAmount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PayMethod", (object)f.PaymentMethod ?? DBNull.Value);

                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Transaction
                        {
                            TransactionId = r.GetInt32(0),
                            UserId = r.GetString(1),
                            CategoryId = r.GetInt32(2),
                            Amount = r.GetDecimal(3),
                            TransactionType = r.GetString(4),
                            PaymentMethod = r.IsDBNull(5) ? null : r.GetString(5),
                            Notes = r.IsDBNull(6) ? null : r.GetString(6),
                            TxnDate = r.GetDateTime(7),
                            CreatedOn = r.GetDateTime(8)
                        });
                    }
                }
            }
            return list;
        }

        public Transaction GetById(string userId, int id)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT TransactionId, UserId, CategoryId, Amount, TransactionType,
                         PaymentMethod, Notes, TxnDate, CreatedOn
                  FROM dbo.Transactions
                  WHERE TransactionId = @Id AND UserId = @UserId;", cn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new Transaction
                    {
                        TransactionId = r.GetInt32(0),
                        UserId = r.GetString(1),
                        CategoryId = r.GetInt32(2),
                        Amount = r.GetDecimal(3),
                        TransactionType = r.GetString(4),
                        PaymentMethod = r.IsDBNull(5) ? null : r.GetString(5),
                        Notes = r.IsDBNull(6) ? null : r.GetString(6),
                        TxnDate = r.GetDateTime(7),
                        CreatedOn = r.GetDateTime(8)
                    };
                }
            }
        }

        public int Create(Transaction t)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"INSERT INTO dbo.Transactions
                  (UserId, CategoryId, Amount, TransactionType, PaymentMethod, Notes, TxnDate, CreatedOn)
                  OUTPUT INSERTED.TransactionId
                  VALUES (@UserId, @CategoryId, @Amount, @Type, @PayMethod, @Notes, @TxnDate, GETDATE());", cn))
            {
                cmd.Parameters.AddWithValue("@UserId", t.UserId);
                cmd.Parameters.AddWithValue("@CategoryId", t.CategoryId);
                cmd.Parameters.AddWithValue("@Amount", t.Amount);
                cmd.Parameters.AddWithValue("@Type", t.TransactionType);
                cmd.Parameters.AddWithValue("@PayMethod", (object)t.PaymentMethod ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)t.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TxnDate", t.TxnDate);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(Transaction t)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"UPDATE dbo.Transactions
                      SET CategoryId = @CategoryId,
                          Amount = @Amount,
                          TransactionType = @Type,
                          PaymentMethod = @PayMethod,
                          Notes = @Notes,
                          TxnDate = @TxnDate
                  WHERE TransactionId = @Id AND UserId = @UserId;", cn))
            {
                cmd.Parameters.AddWithValue("@Id", t.TransactionId);
                cmd.Parameters.AddWithValue("@UserId", t.UserId);
                cmd.Parameters.AddWithValue("@CategoryId", t.CategoryId);
                cmd.Parameters.AddWithValue("@Amount", t.Amount);
                cmd.Parameters.AddWithValue("@Type", t.TransactionType);
                cmd.Parameters.AddWithValue("@PayMethod", (object)t.PaymentMethod ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)t.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TxnDate", t.TxnDate);
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public bool Delete(string userId, int id)
        {
            using (var cn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"DELETE FROM dbo.Transactions
                  WHERE TransactionId = @Id AND UserId = @UserId;", cn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }
    }
}
