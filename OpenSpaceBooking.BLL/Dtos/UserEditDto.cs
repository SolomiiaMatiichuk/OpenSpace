using System.ComponentModel.DataAnnotations;

namespace OpenSpaceBooking.BLL.Dtos;

public class UserEditDto
{
    [Required(ErrorMessage = "Last name is required!")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "First name is required!")]
    public string FirstName { get; set; }
}