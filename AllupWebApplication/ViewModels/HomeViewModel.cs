using AllupWebApplication.Models;
using Humanizer.Localisation;

namespace AllupWebApplication.ViewModels;

public class HomeViewModel
{
    public IEnumerable<SliderItem> SliderItems { get; set; }
    public IEnumerable<Category> Categories { get; set; }
    public IEnumerable<Product> Products { get; set; }
}
