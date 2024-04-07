using System.ComponentModel.DataAnnotations;

namespace AllupWebApplication.Areas.Admin.ViewModels;

public class AdminLoginViewModel
{
    [DataType(DataType.Text)]
    [StringLength(20)]
    public string UserName { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
