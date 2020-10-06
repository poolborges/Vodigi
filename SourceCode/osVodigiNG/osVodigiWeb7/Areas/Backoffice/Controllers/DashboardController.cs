using System;
using Microsoft.AspNetCore.Mvc;

namespace Application.WebsiteCore.Areas.Backoffice.Controllers
{
    [Area("Backoffice")]
    [Route("Backoffice/[controller]")]
    public class DashboardController : Controller
    {
        public DashboardController()
        {
            //
        }

        [Route("{page:int?}")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
