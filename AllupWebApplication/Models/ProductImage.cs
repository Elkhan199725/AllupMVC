using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllupWebApplication.Models;

public class ProductImage : BaseEntity
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Image URL is required")]
    [StringLength(250, ErrorMessage = "ImageUrl must be less than 250 characters")]
    public string ImageUrl { get; set; }

    // true: Poster, false: Hover, null: Additional detail image
    public bool? IsPoster { get; set; }

    // Relationship to Product
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
