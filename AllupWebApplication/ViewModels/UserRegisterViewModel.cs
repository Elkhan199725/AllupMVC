using System.ComponentModel.DataAnnotations;

namespace AllupWebApplication.ViewModels;

public class UserRegisterViewModel
{
    [DataType(DataType.Text)]
    public string FullName { get; set; }
    [DataType(DataType.Text)]
    public string UserName { get; set; }
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword")]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; } = string.Empty;

}
