using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AllupWebApplication.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(Expression<Func<Category, bool>>? filter = null, params string[] includes);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync(); // This can remain as is if you're only fetching active categories without additional filters.
        Task<Category> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(Category category, IFormFile imageFile);
        Task UpdateCategoryAsync(Category category, IFormFile? imageFile = null);
        Task SoftDeleteCategoryAsync(int id);
        Task HardDeleteCategoryAsync(int id);
    }
}
