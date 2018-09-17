using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseClaim.Controllers
{
    public class HomeController:Controller
    {
        public IActionResult Index()
        {
            CultureInfo invC = CultureInfo.InvariantCulture;
            DateTime date = DateTime.Now;
            string demo = date.ToString("f", invC);

            string dt = "Tuesday 27 april 2017";

            return Content( "Hello from Home Controller");
        }
    }
}
