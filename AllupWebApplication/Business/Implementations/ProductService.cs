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
using AllupWebApplication.Helpers.Extensions;

public class ProductService : IProductService
{
    private readonly AllupDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductService(AllupDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(Expression<Func<Product, bool>>? filter = null, params string[] includes)
    {
        IQueryable<Product> query = _context.Products.AsQueryable();
        if (filter != null) query = query.Where(filter);
        foreach (var include in includes ?? new string[] { }) query = query.Include(include);
        return await query.ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context.Products
                             .Include(p => p.ProductImages)
                             .FirstOrDefaultAsync(p => p.Id == id)
               ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
    }

    public async Task CreateProductAsync(Product product, IFormFile? posterImage, IFormFile? hoverImage, IEnumerable<IFormFile>? additionalImages = null)
    {
        if (posterImage != null)
        {
            product.ProductImages.Add(new ProductImage
            {
                ImageUrl = await FileManager.SaveFileAsync(posterImage, _webHostEnvironment.WebRootPath, "uploads/products"),
                IsPoster = true
            });
        }

        if (hoverImage != null)
        {
            product.ProductImages.Add(new ProductImage
            {
                ImageUrl = await FileManager.SaveFileAsync(hoverImage, _webHostEnvironment.WebRootPath, "uploads/products"),
                IsPoster = false
            });
        }

        if (additionalImages != null)
        {
            foreach (var image in additionalImages)
            {
                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = await FileManager.SaveFileAsync(image, _webHostEnvironment.WebRootPath, "uploads/products"),
                    IsPoster = null
                });
            }
        }

        // Set creation date
        product.SetCreatedDate();

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product, IFormFile? posterImage = null, IFormFile? hoverImage = null, IEnumerable<IFormFile>? additionalImages = null, IEnumerable<int>? imageIdsToDelete = null)
    {
        var existingProduct = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existingProduct == null) throw new ArgumentException($"Product with ID {product.Id} not found.");

        // Update product fields here
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.SalePrice = product.SalePrice;
        existingProduct.CostPrice = product.CostPrice;
        existingProduct.DiscountPercent = product.DiscountPercent;
        existingProduct.ProductCode = product.ProductCode;
        existingProduct.StockCount = product.StockCount;
        existingProduct.IsActive = product.IsActive;
        existingProduct.Category = product.Category;
        existingProduct.IsBestSeller = product.IsBestSeller;
        existingProduct.IsNew = product.IsNew;
        existingProduct.IsFeatured = product.IsFeatured;
        existingProduct.IsAvailable = product.IsAvailable;
        // Add other fields as necessary

        // Update poster and hover images if provided
        if (posterImage != null)
        {
            var existingPoster = existingProduct.ProductImages.FirstOrDefault(img => img.IsPoster == true);
            if (existingPoster != null)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", existingPoster.ImageUrl);
                existingProduct.ProductImages.Remove(existingPoster);
            }
            existingProduct.ProductImages.Add(new ProductImage
            {
                ImageUrl = await FileManager.SaveFileAsync(posterImage, _webHostEnvironment.WebRootPath, "uploads/products"),
                IsPoster = true
            });
        }

        if (hoverImage != null)
        {
            var existingHover = existingProduct.ProductImages.FirstOrDefault(img => img.IsPoster == false);
            if (existingHover != null)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", existingHover.ImageUrl);
                existingProduct.ProductImages.Remove(existingHover);
            }
            existingProduct.ProductImages.Add(new ProductImage
            {
                ImageUrl = await FileManager.SaveFileAsync(hoverImage, _webHostEnvironment.WebRootPath, "uploads/products"),
                IsPoster = false
            });
        }

        // Handle additional images
        if (additionalImages != null)
        {
            foreach (var image in additionalImages)
            {
                existingProduct.ProductImages.Add(new ProductImage
                {
                    ImageUrl = await FileManager.SaveFileAsync(image, _webHostEnvironment.WebRootPath, "uploads/products"),
                    IsPoster = null
                });
            }
        }

        // Handle image deletions
        if (imageIdsToDelete != null && imageIdsToDelete.Any())
        {
            var imagesToDelete = existingProduct.ProductImages.Where(img => imageIdsToDelete.Contains(img.Id)).ToList();
            foreach (var image in imagesToDelete)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", image.ImageUrl);
                existingProduct.ProductImages.Remove(image);
            }
        }
        // Update modified date
        existingProduct.UpdateModifiedDate();

        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null && product.IsActive == true)
        {
            product.IsActive = false;
            product.UpdateModifiedDate();
            await _context.SaveChangesAsync();
        }
    }

    public async Task HardDeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }
        if (product.IsActive)
        {
            throw new InvalidOperationException("Cannot hard delete an active category. Please deactivate the category first.");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
