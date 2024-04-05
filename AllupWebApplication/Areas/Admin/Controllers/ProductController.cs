using Microsoft.AspNetCore.Mvc;
using AllupWebApplication.Business.Interfaces;
using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllupWebApplication.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile posterImage, List<IFormFile> additionalImages)
    {
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        if (ModelState.IsValid)
        {
            // No casting needed here; additionalImages is directly passed as it matches the expected parameter type
            await _productService.CreateProductAsync(product, posterImage, (IFormFile?)additionalImages);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }


    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? posterImage, IFormFile? hoverImage, List<IFormFile> additionalImages)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(product, posterImage, hoverImage, additionalImages);
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }
        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> SoftDeleteConfirm(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost, ActionName("SoftDeleteConfirm")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDeleteConfirmed(int id)
    {
        await _productService.SoftDeleteProductAsync(id);
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> HardDeleteConfirm(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost, ActionName("HardDeleteConfirm")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDeleteConfirmed(int id)
    {
        await _productService.HardDeleteProductAsync(id);
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }
}
