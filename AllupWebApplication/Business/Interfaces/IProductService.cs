using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;

namespace AllupWebApplication.Business.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(Expression<Func<Product, bool>>? filter = null, params string[] includes);
        Task<Product> GetProductByIdAsync(int id);
        Task CreateProductAsync(Product product, IFormFile? posterImage, IFormFile? hoverImage, IEnumerable<IFormFile>? additionalImages = null);
        Task UpdateProductAsync(Product product, IFormFile? posterImage = null, IFormFile? hoverImage = null, IEnumerable<IFormFile>? additionalImages = null, IEnumerable<int>? imageIdsToDelete = null);
        Task SoftDeleteProductAsync(int id);
        Task HardDeleteProductAsync(int id);
    }
}
