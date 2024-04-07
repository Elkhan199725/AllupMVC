using System.ComponentModel.DataAnnotations;

namespace AllupWebApplication.ViewModels;

public class UserLoginViewModel
{
    [DataType(DataType.Text)]
    [StringLength(20)]
    public string UserName { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
