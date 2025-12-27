
using System;
using System.Collections.Generic;
using PEMS.Web.Models;

namespace PEMS.Web.DAL.Interfaces
{
    // Filter for listing transactions (Index)
    public class TransactionFilter
    {
        public string UserId { get; set; }                // required for scoping
        public string TransactionType { get; set; }       // "Expense" or "Income"
        public DateTime? From { get; set; }               // date range
        public DateTime? To { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string PaymentMethod { get; set; }
    }

    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetTransactions(TransactionFilter filter);
        Transaction GetById(string userId, int id);

        int Create(Transaction t);                        // t.UserId required
        bool Update(Transaction t);                       // ownership enforced
        bool Delete(string userId, int id);               // ownership enforced
    }
}
