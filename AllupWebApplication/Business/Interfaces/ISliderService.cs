using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;

namespace AllupWebApplication.Business.Interfaces
{
    public interface ISliderService
    {
        Task<IEnumerable<SliderItem>> GetAllSlidersAsync();
        Task<IEnumerable<SliderItem>> GetActiveSlidersAsync();
        Task<SliderItem> GetSliderByIdAsync(int id);

     
        Task CreateSliderAsync(SliderItem slider, IFormFile imageFile);
        Task UpdateSliderAsync(SliderItem slider, IFormFile? imageFile = null);

        Task SoftDeleteSliderAsync(int id);
        Task HardDeleteSliderAsync(int id);
    }
}
