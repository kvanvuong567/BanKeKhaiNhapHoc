using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BanKeKhaiNhapHoc.Models
{
    [Table("DanhSachLop")]
    public class DanhSachLop
    {
        [Key]
        public int Id { get; set; }

        public int HocVienId { get; set; }
        public virtual HocVien? HocVien { get; set; }

        public int LopHocId { get; set; }
        public virtual LopHoc? LopHoc { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; }

        public DateTime? NgayDangKy { get; set; }
        public DateTime? NgayDuyet { get; set; }
    }
}
