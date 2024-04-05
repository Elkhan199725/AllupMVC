using Microsoft.EntityFrameworkCore;
using AllupWebApplication.Models;
using AllupWebApplication.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AllupWebApplication.Helpers.Extensions;
using AllupWebApplication.Data; // Ensure FileManager is included here.

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

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(Expression<Func<Product, bool>>? filter = null, params string[] includes)
    {
        filter = filter == null ? p => p.IsActive : CombineExpressions(p => p.IsActive, filter);

        return await GetAllProductsAsync(filter, includes);
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) throw new ArgumentException($"Product with ID {id} not found.");
        return product;
    }

    public async Task CreateProductAsync(Product product, IFormFile? posterImage, IFormFile? hoverImage, IEnumerable<IFormFile>? additionalImages = null)
    {
        if (posterImage != null)
        {
            var posterImageUrl = await FileManager.SaveFileAsync(posterImage, _webHostEnvironment.WebRootPath, "uploads/products");
            product.ProductImages.Add(new ProductImage { ImageUrl = posterImageUrl, IsPoster = true });
        }

        if (hoverImage != null)
        {
            var hoverImageUrl = await FileManager.SaveFileAsync(hoverImage, _webHostEnvironment.WebRootPath, "uploads/products");
            product.ProductImages.Add(new ProductImage { ImageUrl = hoverImageUrl, IsPoster = false });
        }

        if (additionalImages != null)
        {
            foreach (var image in additionalImages)
            {
                var imageUrl = await image.SaveFileAsync(_webHostEnvironment.WebRootPath, "uploads/products");
                product.ProductImages.Add(new ProductImage { ImageUrl = imageUrl });
            }
        }
        product.SetCreatedDate();
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product, IFormFile? posterImage = null, IFormFile? hoverImage = null, IEnumerable<IFormFile>? additionalImages = null, IEnumerable<int>? imageIdsToDelete = null)
    {
        var existingProduct = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existingProduct == null) throw new KeyNotFoundException($"Product with ID {product.Id} not found.");

        // Update basic properties
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        // Update additional properties as needed

        // Update Poster and Hover Images if provided
        if (posterImage != null)
        {
            // Assuming existingProduct.ProductImages is initialized
            var existingPoster = existingProduct.ProductImages.FirstOrDefault(img => img.IsPoster == true);
            if (existingPoster != null)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", existingPoster.ImageUrl);
            }
            existingPoster.ImageUrl = await FileManager.SaveFileAsync(posterImage, _webHostEnvironment.WebRootPath, "uploads/products");
        }

        if (hoverImage != null)
        {
            var existingHover = existingProduct.ProductImages.FirstOrDefault(img => img.IsPoster == false);
            if (existingHover != null)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", existingHover.ImageUrl);
            }
            existingHover.ImageUrl = await FileManager.SaveFileAsync(hoverImage, _webHostEnvironment.WebRootPath, "uploads/products");
        }

        // Handle additional images
        if (additionalImages != null)
        {
            foreach (var image in additionalImages)
            {
                var imageUrl = await image.SaveFileAsync(_webHostEnvironment.WebRootPath, "uploads/products");
                existingProduct.ProductImages.Add(new ProductImage { ImageUrl = imageUrl });
            }
        }

        // Remove selected images
        if (imageIdsToDelete != null)
        {
            var imagesToRemove = existingProduct.ProductImages.Where(img => imageIdsToDelete.Contains(img.Id)).ToList();
            foreach (var image in imagesToRemove)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", image.ImageUrl);
                _context.ProductImages.Remove(image);
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
        if (product == null) throw new KeyNotFoundException($"Product with ID {id} not found.");

        product.IsActive = false;
        await _context.SaveChangesAsync();
    }

    public async Task HardDeleteProductAsync(int id)
    {
        var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) throw new KeyNotFoundException($"Product with ID {id} not found.");

        foreach (var image in product.ProductImages)
        {
            FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", image.ImageUrl);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    // Helper method to combine expressions with AND
    private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }
}
