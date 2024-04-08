using System.ComponentModel.DataAnnotations;

namespace AllupWebApplication.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
}
