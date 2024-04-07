using AllupWebApplication.Data;
using AllupWebApplication.Models;
using AllupWebApplication.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AllupWebApplication.Helpers.Extensions; //FileManager is in this namespace

public class CategoryService : ICategoryService
{
    private readonly AllupDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CategoryService(AllupDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    private IQueryable<Category> _getIncludes(IQueryable<Category> query, params string[] includes)
    {
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(Expression<Func<Category, bool>>? filter = null, params string[] includes)
    {
        IQueryable<Category> query = _context.Categories;
        query = _getIncludes(query, includes);

        if (filter != null)
        {   
            query = query.Where(filter);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _context.Categories
                             .Where(c => c.IsActive)
                             .ToListAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task CreateCategoryAsync(Category category, IFormFile imageFile)
    {
        string folderPath = "uploads/categories";
        category.ImageUrl = await FileManager.SaveFileAsync(imageFile, _webHostEnvironment.WebRootPath, folderPath);

        // Set creation date
        category.SetCreatedDate();

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category, IFormFile? imageFile = null)
    {
        var existingCategory = await _context.Categories.FindAsync(category.Id);
        if (existingCategory != null)
        {
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.IsActive = category.IsActive;

            if (imageFile != null)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/categories", existingCategory.ImageUrl);
                existingCategory.ImageUrl = await FileManager.SaveFileAsync(imageFile, _webHostEnvironment.WebRootPath, "uploads/categories");
            }

            // Update modified date
            existingCategory.UpdateModifiedDate();

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SoftDeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null && category.IsActive == true)
        {
            category.IsActive = false;
            category.UpdateModifiedDate();
            await _context.SaveChangesAsync();
        }
    }

    public async Task HardDeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }
        if (category.IsActive)
        {
            throw new InvalidOperationException("Cannot hard delete an active category. Please deactivate the category first.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
