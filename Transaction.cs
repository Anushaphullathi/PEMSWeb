
using System;
using System.ComponentModel.DataAnnotations;

namespace PEMS.Web.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, Range(0.0, 100000000)]
        public decimal Amount { get; set; }

        [Required, RegularExpression("Income|Expense", ErrorMessage = "Type must be Income or Expense.")]
        public string TransactionType { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime TxnDate { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
