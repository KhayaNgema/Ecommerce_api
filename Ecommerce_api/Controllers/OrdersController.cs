using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_api.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
