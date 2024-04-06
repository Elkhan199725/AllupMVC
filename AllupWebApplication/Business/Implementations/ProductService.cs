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

        // Dynamically include specified related entities
        foreach (var include in includes ?? Array.Empty<string>())
        {
            query = query.Include(include);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

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
        // Initialize ProductImages if it's null to avoid NullReferenceException
        if (product.ProductImages == null)
        {
            product.ProductImages = new List<ProductImage>();
        }

        // Handle the poster image
        if (posterImage != null && posterImage.Length > 0)
        {
            var posterImageUrl = await FileManager.SaveFileAsync(posterImage, _webHostEnvironment.WebRootPath, "uploads/products");
            product.ProductImages.Add(new ProductImage
            {
                ImageUrl = posterImageUrl,
                IsPoster = true
            });
        }

        // Handle the hover image
        if (hoverImage != null && hoverImage.Length > 0)
        {
            var hoverImageUrl = await FileManager.SaveFileAsync(hoverImage, _webHostEnvironment.WebRootPath, "uploads/products");
            product.ProductImages.Add(new ProductImage
            {
                ImageUrl = hoverImageUrl,
                IsPoster = false
            });
        }

        // Handle additional images
        if (additionalImages != null)
        {
            foreach (var image in additionalImages)
            {
                if (image.Length > 0)
                {
                    var imageUrl = await FileManager.SaveFileAsync(image, _webHostEnvironment.WebRootPath, "uploads/products");
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        IsPoster = null // Not a poster or hover image
                    });
                }
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

        // Handle poster image update
        if (posterImage != null)
        {
            var existingPosterImage = existingProduct.ProductImages.FirstOrDefault(img => img.IsPoster == true);
            if (existingPosterImage != null)
            {
                // If there is an existing poster image, delete it
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", existingPosterImage.ImageUrl);
                _context.ProductImages.Remove(existingPosterImage);
            }
            // Add the new poster image
            var posterImageUrl = await FileManager.SaveFileAsync(posterImage, _webHostEnvironment.WebRootPath, "uploads/products");
            existingProduct.ProductImages.Add(new ProductImage { ImageUrl = posterImageUrl, IsPoster = true });
        }

        // Handle hover image update
        if (hoverImage != null)
        {
            var existingHoverImage = existingProduct.ProductImages.FirstOrDefault(img => img.IsPoster == false);
            if (existingHoverImage != null)
            {
                // If there is an existing hover image, delete it
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", existingHoverImage.ImageUrl);
                _context.ProductImages.Remove(existingHoverImage);
            }
            // Add the new hover image
            var hoverImageUrl = await FileManager.SaveFileAsync(hoverImage, _webHostEnvironment.WebRootPath, "uploads/products");
            existingProduct.ProductImages.Add(new ProductImage { ImageUrl = hoverImageUrl, IsPoster = false });
        }

        // Handle additional images
        if (additionalImages != null)
        {
            foreach (var image in additionalImages)
            {
                // Prevent adding the poster or hover image as an additional image
                if (posterImage != null && image.FileName == posterImage.FileName ||
                    hoverImage != null && image.FileName == hoverImage.FileName)
                {
                    continue;
                }
                var imageUrl = await FileManager.SaveFileAsync(image, _webHostEnvironment.WebRootPath, "uploads/products");
                existingProduct.ProductImages.Add(new ProductImage { ImageUrl = imageUrl, IsPoster = null });
            }
        }

        // Handle image deletions
        if (imageIdsToDelete != null)
        {
            var imagesToRemove = existingProduct.ProductImages.Where(img => imageIdsToDelete.Contains(img.Id)).ToList();
            foreach (var imageToRemove in imagesToRemove)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", imageToRemove.ImageUrl);
                _context.ProductImages.Remove(imageToRemove);
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
            throw new KeyNotFoundException("Product not found.");
        }
        if (product.IsActive)
        {
            throw new InvalidOperationException("Cannot hard delete an active product. Please deactivate the product first.");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
