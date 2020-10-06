using System;
using Microsoft.AspNetCore.Mvc;

namespace Application.WebsiteCore.Areas.Backoffice.Controllers
{
    [Area("Backoffice")]
    [Route("Backoffice/[controller]")]
    public class ProductsController : Controller
    {
        public ProductsController()
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
