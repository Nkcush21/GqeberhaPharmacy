using Microsoft.AspNetCore.Mvc;

namespace GqeberhaPharmacy.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Manager"))
                    return RedirectToAction("Index", "Manager");
                if (User.IsInRole("Pharmacist"))
                    return RedirectToAction("Index", "Pharmacist");
                if (User.IsInRole("Customer"))
                    return RedirectToAction("Index", "Customer");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
