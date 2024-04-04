using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AllupWebApplication.Models;

public class SliderItem : BaseEntity
{
    [Required]
    [StringLength(30)]
    public string Title { get; set; }

    [StringLength(50)]
    public string? Subtitle { get; set; }

    [StringLength(250)]
    public string? ImageUrl { get; set; } // This will store the file path after the file is saved

    // This property will be used to upload files from the form
    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; set; }

    [StringLength(40)]
    public string? ButtonText { get; set; }

    [StringLength(250)]
    public string? ButtonUrl { get; set; }
}
