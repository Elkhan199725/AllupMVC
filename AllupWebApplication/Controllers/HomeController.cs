
using AllupWebApplication.Business.Interfaces;
using AllupWebApplication.Data;
using AllupWebApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AllupWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AllupDbContext _context;
        private readonly ISliderService _slider;
        private readonly ICategoryService _category;

        public HomeController(
                AllupDbContext context,
                ISliderService slider,
                ICategoryService category)
        {
            _context = context;
            _slider = slider;
            _category = category;
        }

        public async Task<IActionResult> Index()
        {
            HomeViewModel homeVM = new HomeViewModel()
            {
                SliderItems = await _slider.GetAllSlidersAsync(),
                Categories = await _category.GetAllCategoriesAsync(),
                //FeaturedBooks = await _bookService.GetAllAsync(x => x.IsFeatured == true, "BookImages", "Author", "Genre"),
                //NewBooks = await _bookService.GetAllAsync(x => x.IsNew == true, "BookImages", "Author", "Genre"),
                //BestSellerBooks = await _bookService.GetAllAsync(x => x.IsBestSeller == true, "BookImages", "Author", "Genre"),
            };
            return View(homeVM);
        }
    }
}
