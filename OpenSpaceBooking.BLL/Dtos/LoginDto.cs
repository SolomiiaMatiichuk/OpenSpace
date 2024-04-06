using System.ComponentModel.DataAnnotations;

namespace OpenSpaceBooking.BLL.Dtos;

public class LoginDto
{
    [EmailAddress(ErrorMessage = "Email must be in correct format!")]
    [Required(ErrorMessage = "That field is required!")]
    public string Email { get; set; }
    [Required(ErrorMessage = "That field is required!")]
    public string Password { get; set; }
}