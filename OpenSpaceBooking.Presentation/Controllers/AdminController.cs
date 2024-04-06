using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenSpaceBooking.BLL.Interfaces;

namespace OpenSpaceBooking.Presentation.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}