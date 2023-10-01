using ProjectOversight.API.Data.Model;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;

namespace ProjectOversight.API.Dto;

public class EmployeeDto
{
    public int UserId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string PhoneNumber { get; set; }
 
    public IFormFile ProfilePhotoBase64 { get; set; }
}
public class EmployeeLoginDto
{
    [Required(ErrorMessage = "Employee Id is required")]
    public string? EmployeeId { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Phone number is not valid.")]
    public string? PhoneNumber { get; set; }
}

public class VerifyEmployeePhoneDto
{
    [Required(ErrorMessage = "Employee Id is required")]
    public string? EmployeeId { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Phone number is not valid.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "OTP is required")]
    public int? OTP { get; set; }
}