using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanKeKhaiNhapHoc.Models
{
    [Table("HocVien")]
    public class HocVien
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100)]
        public string? HoTen { get; set; }

        [Display(Name = "Giới tính")]
        public bool? GioiTinh { get; set; }

        [Display(Name = "Ngày tháng năm sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Nơi sinh")]
        [StringLength(200)]
        public string? NoiSinh { get; set; }

        [Display(Name = "Quê quán")]
        [StringLength(200)]
        public string? QueQuan { get; set; }

        [Display(Name = "Dân tộc")]
        [StringLength(50)]
        public string? DanToc { get; set; }

        [Display(Name = "Tôn giáo")]
        [StringLength(50)]
        public string? TonGiao { get; set; }

        [Display(Name = "Trình độ lý luận chính trị")]
        [StringLength(100)]
        public string? TrinhDoLyLuanChinhTri { get; set; }

        [Display(Name = "Trình độ chuyên môn nghiệp vụ")]
        [StringLength(100)]
        public string? TrinhDoChuyenMonNghiepVu { get; set; }

        [Display(Name = "Đảng viên đảng CSVN")]
        public bool? DangVienCSVN { get; set; }

        // PHẦN "CÁN BỘ, CÔNG CHỨC, KHÔNG CHUYÊN TRÁCH"
        [Display(Name = "Cán bộ - Cấp")]
        [StringLength(20)]
        public string? CanBo { get; set; }

        [Display(Name = "Công chức - Cấp")]
        [StringLength(20)]
        public string? CongChucLevel { get; set; }

        [Display(Name = "Người hoạt động không chuyên trách cấp xã")]
        public bool NguoiHoatDongKhongChuyenTrach { get; set; }

        // PHẦN CHỨC VỤ, CHỨC DANH
        [Display(Name = "Đảng")]
        [StringLength(100)]
        public string? ChucVu { get; set; }

        [Display(Name = "Chính quyền")]
        [StringLength(100)]
        public string? ChinhQuyen { get; set; }

        [Display(Name = "Chính quyền (Khác)")]
        [StringLength(100)]
        public string? ChucVuChinhQuyenKhac { get; set; }

        [Display(Name = "Đoàn thể")]
        [StringLength(100)]
        public string? DoanThe { get; set; }

        [Display(Name = "Đơn vị công tác")]
        [StringLength(200)]
        public string? DonViCongTac { get; set; }

        [Display(Name = "Ngạch công chức, viên chức")]
        [StringLength(50)]
        public string? NgachCongChuc { get; set; }

        [Display(Name = "Hệ số lương")]
        public decimal? HeSoLuong { get; set; }

        [Display(Name = "Hệ số phụ cấp chức vụ")]
        public decimal? HeSoPhuCapChucVu { get; set; }

        [Display(Name = "Điện thoại di động")]
        [StringLength(20)]
        public string? DienThoaiDiDong { get; set; }

        [Display(Name = "Email")]
        [StringLength(100)]
        public string? Email { get; set; }

        // Thuộc tính mới để đánh dấu trạng thái duyệt đậu của học viên
        [Display(Name = "Trạng thái duyệt")]
        public bool IsApproved { get; set; }

        public virtual ICollection<DanhSachLop>? DanhSachLops { get; set; }
        [NotMapped]
        public int? SelectedLopHocId { get; set; }
    }
}
