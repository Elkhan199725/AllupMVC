using AllupWebApplication.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllupWebApplication.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;

    public HeaderViewComponent(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryService.GetAllCategoriesAsync(null);

        return View(categories);
    }
}
