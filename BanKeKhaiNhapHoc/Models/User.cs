using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanKeKhaiNhapHoc.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string? Username { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string? Role { get; set; }
    }
}