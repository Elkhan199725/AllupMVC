using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllupWebApplication.Models;

namespace AllupWebApplication.Business.Interfaces;

public interface ISliderService
{
    Task<IEnumerable<SliderItem>> GetAllSlidersAsync();
    Task<IEnumerable<SliderItem>> GetActiveSlidersAsync();
    Task<SliderItem> GetSliderByIdAsync(int id);
    Task CreateSliderAsync(SliderItem slider);
    Task UpdateSliderAsync(SliderItem slider);
    Task SoftDeleteSliderAsync(int id);
    Task HardDeleteSliderAsync(int id);
}