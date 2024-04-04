using Microsoft.EntityFrameworkCore;
using AllupWebApplication.Models;
using AllupWebApplication.Business.Interfaces;
using AllupWebApplication.Data;

public class SliderService(AllupDbContext context) : ISliderService
{
    private readonly AllupDbContext _context = context;

    public async Task<IEnumerable<SliderItem>> GetAllSlidersAsync()
    {
        // Fetches all sliders, regardless of their IsActive status.
        return await _context.SliderItems.ToListAsync();
    }

    public async Task<IEnumerable<SliderItem>> GetActiveSlidersAsync()
    {
        // Fetches only sliders that are active (IsActive == true).
        return await _context.SliderItems.Where(s => s.IsActive).ToListAsync();
    }

    public async Task<SliderItem> GetSliderByIdAsync(Guid id)
    {
        // Retrieves a single slider by its Id.
        return await _context.SliderItems.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task CreateSliderAsync(SliderItem slider)
    {
        ArgumentNullException.ThrowIfNull(slider);

        slider.SetCreatedDate(); // Sets the CreatedDate at the time of creation
        await _context.SliderItems.AddAsync(slider);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSliderAsync(SliderItem slider)
    {
        ArgumentNullException.ThrowIfNull(slider);

        var existingSlider = await _context.SliderItems.FindAsync(slider.Id) ?? throw new InvalidOperationException("Slider not found.");

        // Update properties here, for example:
        existingSlider.Title = slider.Title;
        existingSlider.Subtitle = slider.Subtitle;
        existingSlider.ImageUrl = slider.ImageUrl;
        existingSlider.ButtonText = slider.ButtonText;
        existingSlider.ButtonUrl = slider.ButtonUrl;
        existingSlider.UpdateModifiedDate(); // Updates the ModifiedDate on each update

        _context.SliderItems.Update(existingSlider);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteSliderAsync(Guid id)
    {
        var slider = await _context.SliderItems.FindAsync(id) ?? throw new InvalidOperationException("Slider not found.");
        slider.IsActive = false; // Soft delete by setting IsActive to false
        slider.UpdateModifiedDate(); // Update the ModifiedDate upon soft deletion
        await _context.SaveChangesAsync();
    }

    public async Task HardDeleteSliderAsync(Guid id)
    {
        var slider = await _context.SliderItems.FindAsync(id) ?? throw new InvalidOperationException("Slider not found.");
        _context.SliderItems.Remove(slider);
        await _context.SaveChangesAsync();
    }
}
