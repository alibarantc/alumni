using System.ComponentModel.DataAnnotations;

namespace Konekto.API.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [Display(Name = "E-Posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [MaxLength(75, ErrorMessage = "Ad en fazla 75 karakter olabilir.")]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [MaxLength(75, ErrorMessage = "Soyad en fazla 75 karakter olabilir.")]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [Display(Name = "E-Posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler birbiriyle eşleşmiyor.")]
    [Display(Name = "Şifre Tekrar")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı rolü seçilmelidir.")]
    [Display(Name = "Hesap Rolü")]
    public string Role { get; set; } = "Alumni"; // "Alumni" or "Student"

    // Optional Alumni/Student details
    [Display(Name = "Mezuniyet / Giriş Yılı")]
    [Range(1970, 2030, ErrorMessage = "Geçerli bir yıl giriniz.")]
    public int? GraduationYear { get; set; } = DateTime.UtcNow.Year;

    [Display(Name = "Bölüm")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Çalıştığı Şirket")]
    [MaxLength(150)]
    public string? CurrentCompany { get; set; }

    [Display(Name = "Unvan / Pozisyon")]
    [MaxLength(100)]
    public string? CurrentTitle { get; set; }
}

public class ProfileViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Alumni";
    public DateTime CreatedAt { get; set; }

    // Alumni details if role is Alumni
    public int? GraduationYear { get; set; }
    public string? CurrentCompany { get; set; }
    public string? CurrentTitle { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Bio { get; set; }
    public string? DepartmentName { get; set; }
}

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    public string LastName { get; set; } = string.Empty;

    public string? CurrentCompany { get; set; }
    public string? CurrentTitle { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Bio { get; set; }
}

public class ChangeUserRoleRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string NewRole { get; set; } = "Alumni"; // "Admin", "Alumni", "Student"
}
