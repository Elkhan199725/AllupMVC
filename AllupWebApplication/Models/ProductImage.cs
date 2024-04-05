using AllupWebApplication.Models;

namespace AllupWebApplication.Models;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }

    // true: Poster, false: Hover, null: Additional detail image
    public bool? IsPoster { get; set; }

    // Relationship to Product
    public virtual Product Product { get; set; }
}
