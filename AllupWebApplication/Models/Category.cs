using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace AllupWebApplication.Models;

public class Category : BaseEntity
{
    [Required]
    [StringLength(100, ErrorMessage = "The Name must be less than 100 characters long.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "The Description must be less than 500 characters long.")]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    [NotMapped]
    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; set; }

    // Navigation property to Products
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
