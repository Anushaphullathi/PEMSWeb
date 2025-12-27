
using System;
using System.ComponentModel.DataAnnotations;

namespace PEMS.Web.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime PeriodStart { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime PeriodEnd { get; set; }

        [Required, Range(0.0, 100000000)]
        public decimal LimitAmount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
