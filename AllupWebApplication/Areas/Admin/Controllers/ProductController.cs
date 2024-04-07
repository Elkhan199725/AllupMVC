using Microsoft.AspNetCore.Mvc;
using AllupWebApplication.Business.Interfaces;
using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllupWebApplication.Business.Implementations;
using AllupWebApplication.Data;
using AllupWebApplication.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AllupWebApplication.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly AllupDbContext _context;

    public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment, AllupDbContext context)
    {
        _productService = productService;
        _categoryService = categoryService;
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    //public async Task<IActionResult> Index()
    //{
    //    // Including related Category and ProductImages entities
    //    var products = await _productService.GetAllProductsAsync(null, "Category", "ProductImages");
    //    return View(products);
    //}
    public async Task<IActionResult> Index(int? pageNumber)
    {
        const int pageSize = 10; // Set your page size, which is the number of items per page
        var query = _context.Products.AsQueryable(); // Or whatever your queryable source is

        // Include related data as needed
        query = query.Include(p => p.Category).Include(p => p.ProductImages);

        var paginatedData = await PaginatedList<Product>.CreateAsync(query, pageNumber ?? 1, pageSize);
        return View(paginatedData);
    }


    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? posterImage, IFormFile? hoverImage, List<IFormFile> additionalImages)
    {
        if (ModelState.IsValid)
        {
            await _productService.CreateProductAsync(product, posterImage, hoverImage, additionalImages);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product, IFormFile? posterImage, IFormFile? hoverImage, List<IFormFile> additionalImages, List<int>? imageIdsToDelete)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetActiveCategoriesAsync();
            return View(product);
        }

        await _productService.UpdateProductAsync(product, posterImage, hoverImage, additionalImages, imageIdsToDelete);
        return RedirectToAction(nameof(Index));
    }

    // GET: Shows confirmation page for soft delete
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

    // POST: Performs the actual soft delete
    [HttpPost, ActionName("SoftDeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDeleteConfirmed(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product != null)
        {
            await _productService.SoftDeleteProductAsync(id);
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }
        return NotFound();
    }

    // GET: Shows confirmation page for hard delete
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

    // POST: Performs the actual hard delete
    [HttpPost, ActionName("HardDeleteConfirm")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDeleteConfirmed(int id)
    {
        var productExists = await _productService.GetProductByIdAsync(id) != null;
        if (!productExists)
        {
            return NotFound();
        }
        await _productService.HardDeleteProductAsync(id);
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }


}
