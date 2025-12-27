using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PEMS.Web.Models;

namespace PEMS.Web.Models
{
    public class AlertViewModel
    {
        public string Message { get; set; }
        public string Type { get; set; } = "info"; // info, success, warning, danger
        public bool Dismissible { get; set; } = false;

    }
}