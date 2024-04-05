using Microsoft.AspNetCore.Mvc;
using AllupWebApplication.Business.Interfaces;
using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;
using AllupWebApplication.Data;

namespace AllupWebApplication.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly AllupDbContext _context;

    public ProductController(IProductService productService, ICategoryService categoryService,AllupDbContext context, IWebHostEnvironment env)
    {
        _productService = productService;
        _categoryService = categoryService;
        
    }
        private readonly IWebHostEnvironment _env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        ViewBag.Categories = await _context.Genres.ToListAsync();
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile posterImage, List<IFormFile> additionalImages)
    {
        if (ModelState.IsValid)
        {
            await _productService.CreateProductAsync(product, posterImage, additionalImages);
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
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? posterImage, List<IFormFile> additionalImages)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(product, posterImage, additionalImages);
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }
        return View(product);
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
