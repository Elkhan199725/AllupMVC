using AllupWebApplication.Models;
using System.Linq.Expressions;

namespace AllupWebApplication.Business.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync(Expression<Func<Product, bool>>? filter = null, params string[] includes);
    Task<IEnumerable<Product>> GetActiveProductsAsync(Expression<Func<Product, bool>>? filter = null, params string[] includes);
    Task<Product> GetProductByIdAsync(int id);
    Task CreateProductAsync(Product product, IFormFile posterImage, IEnumerable<IFormFile>? additionalImages = null);
    Task UpdateProductAsync(Product product, IFormFile? posterImage = null, IEnumerable<IFormFile>? additionalImages = null);
    Task SoftDeleteProductAsync(int id);
    Task HardDeleteProductAsync(int id);
}