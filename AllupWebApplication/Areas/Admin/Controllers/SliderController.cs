using Microsoft.AspNetCore.Mvc;
using AllupWebApplication.Business.Interfaces;
using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using AllupWebApplication.Data;
using Microsoft.AspNetCore.Hosting;
using AllupWebApplication.Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace AllupWebApplication.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public class SliderController : Controller
{
    private readonly ISliderService _sliderService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly AllupDbContext _context;

    public SliderController(ISliderService sliderService, IWebHostEnvironment webHostEnvironment, AllupDbContext context)
    {
        _sliderService = sliderService;
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    // Display the list of sliders
    public async Task<IActionResult> Index(int? pageNumber)
    {
        const int pageSize = 10; // The number of items per page
        var query = _context.SliderItems.AsQueryable(); // Assuming _context is your DbContext and Sliders is your DbSet

        // If you have related data to include, do it here
        // query = query.Include(s => ...);

        var paginatedData = await PaginatedList<SliderItem>.CreateAsync(query, pageNumber ?? 1, pageSize);
        return View(paginatedData);
    }


    // Show the form to create a new slider
    public IActionResult Create()
    {
        return View();
    }

    // Process the creation of a new slider
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SliderItem slider, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            await _sliderService.CreateSliderAsync(slider, imageFile);
            return RedirectToAction(nameof(Index));
        }
        return View(slider);
    }

    // Show the form to edit an existing slider
    public async Task<IActionResult> Edit(int id)
    {
        var slider = await _sliderService.GetSliderByIdAsync(id);
        if (slider == null)
        {
            return NotFound();
        }
        return View(slider);
    }

    // Process the update of an existing slider
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SliderItem slider, IFormFile? imageFile = null)
    {
        if (id != slider.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _sliderService.UpdateSliderAsync(slider, imageFile);
            return RedirectToAction(nameof(Index));
        }
        return View(slider);
    }

    // GET: Shows confirmation page for soft delete
    [HttpGet]
    public async Task<IActionResult> SoftDeleteConfirm(int id)
    {
        var slider = await _sliderService.GetSliderByIdAsync(id);
        if (slider == null)
        {
            return NotFound();
        }
        return View(slider);
    }

    // POST: Performs the actual soft delete
    [HttpPost, ActionName("SoftDeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDeleteConfirmed(int id)
    {
        var slider = await _sliderService.GetSliderByIdAsync(id);
        if (slider != null)
        {
            await _sliderService.SoftDeleteSliderAsync(id);
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }
        return NotFound();
    }

    // GET: Shows confirmation page for hard delete
    [HttpGet]
    public async Task<IActionResult> HardDeleteConfirm(int id)
    {
        var slider = await _sliderService.GetSliderByIdAsync(id);
        if (slider == null)
        {
            return NotFound();
        }
        return View(slider);
    }

    // POST: Performs the actual hard delete
    [HttpPost, ActionName("HardDeleteConfirm")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDeleteConfirmed(int id)
    {
        var sliderExists = await _sliderService.GetSliderByIdAsync(id) != null;
        if (!sliderExists)
        {
            return NotFound();
        }
        await _sliderService.HardDeleteSliderAsync(id);
        return RedirectToAction(nameof(Index), new { area = "Admin" });
    }


}
