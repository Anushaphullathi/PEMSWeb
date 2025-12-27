using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PEMS.Web.Models; // for AlertViewModel and Category


namespace PEMS.Web.Models
{
    public class CategoriesIndexViewModel
    {

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<AlertViewModel> Alerts { get; set; } = new List<AlertViewModel>();


    }
}