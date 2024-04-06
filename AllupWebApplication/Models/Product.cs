using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace AllupWebApplication.Models;

public class Product : BaseEntity
{
    [Required]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50, ErrorMessage = "Product name must be less than 50 characters")]
    public string Name { get; set; }

    [StringLength(350, ErrorMessage = "Description must be less than 350 characters")]
    public string? Description { get; set; }

    [StringLength(50, ErrorMessage = "Product code must be less than 50 characters")]
    public string? ProductCode { get; set; }
    public double? CostPrice { get; set; }
    public double? SalePrice { get; set; }
    public double? DiscountPercent { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsNew { get; set; } = false;
    public bool IsBestSeller { get; set; } = false;
    public bool IsAvailable { get; set; } = true;
    public int? StockCount { get; set; }

    // Relationship to Category
    public virtual Category? Category { get; set; }

    // Relationship to ProductImage
    public virtual List<ProductImage>? ProductImages { get; set; } = new List<ProductImage>();

    [NotMapped]
    public IFormFile? PosterImageFile { get; set; }

    [NotMapped]
    public IFormFile? HoverImageFile { get; set; }

    [NotMapped]
    public List<IFormFile>? ImageFiles { get; set; }

    // For handling deletions or updates of specific images
    [NotMapped]
    public List<int>? ProductImageIdsToDelete { get; set; }
}
