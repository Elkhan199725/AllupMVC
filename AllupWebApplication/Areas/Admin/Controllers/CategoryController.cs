﻿using Microsoft.AspNetCore.Mvc;

namespace AllupWebApplication.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
