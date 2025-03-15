using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanKeKhaiNhapHoc.Models
{
    [Table("LopHoc")]
    public class LopHoc
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tên lớp")]
        [Required(ErrorMessage = "Tên lớp không được để trống")]
        [StringLength(100, ErrorMessage = "Tên lớp không được vượt quá 100 ký tự")]
        public string? TenLop { get; set; }

        [Display(Name = "Mô tả lớp")]
        public string? MoTaLop { get; set; }

        [Display(Name = "Tên khóa học")]
        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [StringLength(200, ErrorMessage = "Tên khóa học không được vượt quá 200 ký tự")]
        public string? TenKhoaHoc { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime? NgayBatDau { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        [Display(Name = "Mô tả khóa học")]
        public string? MoTaKhoaHoc { get; set; }

        public virtual ICollection<DanhSachLop>? DanhSachLops { get; set; }
    }
}
