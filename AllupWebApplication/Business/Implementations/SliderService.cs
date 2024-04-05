using AllupWebApplication.Business.Interfaces;
using AllupWebApplication.Models;
using Microsoft.EntityFrameworkCore;
using AllupWebApplication.Helpers.Extensions;
using AllupWebApplication.Data;

namespace AllupWebApplication.Business.Implementations
{
    public class SliderService : ISliderService
    {
        private readonly AllupDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderService(AllupDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        
        public async Task<IEnumerable<SliderItem>> GetAllSlidersAsync()
        {
            return await _context.SliderItems.ToListAsync();
        }

        public async Task<IEnumerable<SliderItem>> GetActiveSlidersAsync()
        {
            return await _context.SliderItems
                                 .Where(s => s.IsActive)
                                 .ToListAsync();
        }

        public async Task<SliderItem> GetSliderByIdAsync(int id)
        {
            return await _context.SliderItems
                                 .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task CreateSliderAsync(SliderItem slider, IFormFile imageFile)
        {
            // Handling file upload
            slider.ImageUrl = await FileManager.SaveFileAsync(imageFile, _webHostEnvironment.WebRootPath, "uploads/sliders");

            // Set creation date
            slider.SetCreatedDate();

            await _context.SliderItems.AddAsync(slider);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSliderAsync(SliderItem slider, IFormFile? imageFile = null)
        {
            var existingSlider = await _context.SliderItems.FindAsync(slider.Id);
            if (existingSlider != null)
            {
                existingSlider.Title = slider.Title;
                existingSlider.Subtitle = slider.Subtitle;
                existingSlider.ButtonText = slider.ButtonText;
                existingSlider.ButtonUrl = slider.ButtonUrl;

                // Handle image update
                if (imageFile != null)
                {
                    FileManager.DeleteFile(_webHostEnvironment.WebRootPath, "uploads/sliders", existingSlider.ImageUrl);
                    existingSlider.ImageUrl = await FileManager.SaveFileAsync(imageFile, _webHostEnvironment.WebRootPath, "uploads/sliders");
                }

                // Update modified date
                existingSlider.UpdateModifiedDate();

                _context.SliderItems.Update(existingSlider);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteSliderAsync(int id)
        {
            var slider = await _context.SliderItems.FindAsync(id);
            if (slider != null)
            {
                slider.IsActive = false;
                slider.UpdateModifiedDate();
                await _context.SaveChangesAsync();
            }
        }

        public async Task HardDeleteSliderAsync(int id)
        {
            var slider = await _context.SliderItems.FindAsync(id);
            if (slider == null)
            {
                throw new KeyNotFoundException("Slider not found.");
            }
            if (slider.IsActive)
            {
                throw new InvalidOperationException("Cannot hard delete an active slider. Please deactivate the slider first.");
            }

            _context.SliderItems.Remove(slider);
            await _context.SaveChangesAsync();
        }
    }
}
