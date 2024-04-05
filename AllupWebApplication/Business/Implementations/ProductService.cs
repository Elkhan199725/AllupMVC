using AllupWebApplication.Data;
using AllupWebApplication.Helpers.Extensions;
using AllupWebApplication.Models;
using AllupWebApplication.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AllupWebApplication.Business.Implementations
{
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

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
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
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new ArgumentException($"Product with ID {id} not found.");
            return product;
        }

        public async Task CreateProductAsync(Product product, IFormFile posterImage, IEnumerable<IFormFile>? additionalImages = null)
        {
            // Handle the poster image
            if (posterImage != null)
            {
                var posterImageUrl = await posterImage.SaveFileAsync(_webHostEnvironment.WebRootPath, "uploads/products");
                product.ProductImages.Add(new ProductImage { ImageUrl = posterImageUrl, IsPoster = true });
            }

            // Handle the hover image if it's separate from the poster and additional images
            if (product.HoverImageFile != null)
            {
                var hoverImageUrl = await product.HoverImageFile.SaveFileAsync(_webHostEnvironment.WebRootPath, "uploads/products");
                product.ProductImages.Add(new ProductImage { ImageUrl = hoverImageUrl, IsPoster = false });
            }

            // Handle additional images
            if (additionalImages != null)
            {
                foreach (var image in additionalImages)
                {
                    var imageUrl = await image.SaveFileAsync(_webHostEnvironment.WebRootPath, "uploads/products");
                    product.ProductImages.Add(new ProductImage { ImageUrl = imageUrl });
                }
            }

            // Since ProductImages is a relational property, ensure it's initialized to avoid a NullReferenceException
            if (product.ProductImages == null) product.ProductImages = new List<ProductImage>();

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateProductAsync(Product product, IFormFile? posterImage = null, IEnumerable<IFormFile>? additionalImages = null)
        {
            var existingProduct = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct == null) throw new ArgumentException($"Product with ID {product.Id} not found.");

            // Update properties
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.CostPrice = product.CostPrice;
            existingProduct.SalePrice = product.SalePrice;
            existingProduct.DiscountPercent = product.DiscountPercent;
            existingProduct.IsNew = product.IsNew;
            existingProduct.IsFeatured = product.IsFeatured;
            existingProduct.IsBestSeller = product.IsBestSeller;
            // Continue updating other properties as necessary

            // Assume we decide which image is the poster based on some property or position
            var existingPosterImage = existingProduct.ProductImages.FirstOrDefault(img => img.IsPoster == true);
            // Handle poster image update
            if (posterImage != null && existingPosterImage != null)
            {
                // Assuming FileManager.DeleteFile handles null fileName gracefully
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", existingPosterImage.ImageUrl);
                existingPosterImage.ImageUrl = await posterImage.SaveFileAsync(_webHostEnvironment.WebRootPath, "uploads/products");
            }

            // Optionally clear existing additional images and replace with new ones
            // This step depends on your requirements. You might also want to just add new images or allow users to select images to delete.
            var additionalImagesToDelete = existingProduct.ProductImages.Where(img => img.IsPoster == null || img.IsPoster == false).ToList();
            foreach (var img in additionalImagesToDelete)
            {
                FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/products", img.ImageUrl);
                _context.ProductImages.Remove(img);
            }

            if (additionalImages != null)
            {
                foreach (var image in additionalImages)
                {
                    var imageUrl = await image.SaveFileAsync(_webHostEnvironment.WebRootPath, "uploads/products");
                    existingProduct.ProductImages.Add(new ProductImage { ImageUrl = imageUrl, ProductId = existingProduct.Id });
                }
            }

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();
        }


        public async Task SoftDeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            product.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task HardDeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // Helper method to combine two expressions with an AND operation
        private static Expression<Func<Product, bool>> CombineExpressions(Expression<Func<Product, bool>> expr1, Expression<Func<Product, bool>>? expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<Product, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
