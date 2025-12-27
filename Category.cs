
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PEMS.Web.Models;

namespace PEMS.Web.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, RegularExpression("Income|Expense", ErrorMessage = "Type must be Income or Expense.")]
        public string CategoryType { get; set; }

        public bool IsActive { get; set; } = true;

        // Owner of the category; NULL means global (read-only in UI)
        public string UserId { get; set; }
        public IEnumerable<AlertViewModel> Alerts { get; set; } = new List<AlertViewModel>();
    }
}
