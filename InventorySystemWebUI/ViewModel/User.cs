using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventorySystemWebUI.ViewModel;

public class User
{
    [Required(ErrorMessage = "The user name is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "The Email Address is required")]
    [EmailAddress(ErrorMessage = "Enter a valid Email Address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The Password is required")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }


    [Required(ErrorMessage = "The Confirm Password is required")]
    [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }


    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(11, ErrorMessage = "The phone number must not exceed 11 digits.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "The phone number must be exactly 11 digits.")]
    public string Phonenumber { get; set; }
}
