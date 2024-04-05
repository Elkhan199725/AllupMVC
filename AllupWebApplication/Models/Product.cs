using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AllupWebApplication.Models;
using Microsoft.AspNetCore.Http;

namespace AllupWebApplication.Models;

public class Product : BaseEntity
{
    public int CategoryId { get; set; }

    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(350)]
    public string Description { get; set; }
    public string ProductCode { get; set; }
    public double CostPrice { get; set; }
    public double SalePrice { get; set; }
    public double DiscountPercent { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsAvailable { get; set; }
    public int StockCount { get; set; }
    // Relationship to Category
    public virtual Category Category { get; set; }

    // Relationship to ProductImage
    public virtual List<ProductImage> ProductImages { get; set; }

    [NotMapped]
    public IFormFile? PosterImageFile { get; set; }

    [NotMapped]
    public IFormFile? HoverImageFile { get; set; }

    [NotMapped]
    public List<IFormFile>? ImageFiles { get; set; }

    [NotMapped]
    public List<int>? ProductImageIds { get; set; }
}
