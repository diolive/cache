using System.ComponentModel.DataAnnotations;

namespace DioRed.Cache.WebApp.Models.AccountViewModels;

public class RegisterVM
{
    [Required(ErrorMessage = "The Email field is required")]
    [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "The Password field is required")]
    [StringLength(100, ErrorMessage = "The Password must be at least {1} and at max {2} characters long", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
    public required string ConfirmPassword { get; set; }
}