using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BanKeKhaiNhapHoc.Models;

namespace BanKeKhaiNhapHoc.Data
{
    public class BanKeKhaiNhapHocContext : DbContext
    {
        public BanKeKhaiNhapHocContext (DbContextOptions<BanKeKhaiNhapHocContext> options)
            : base(options)
        {
        }
        public DbSet<BanKeKhaiNhapHoc.Models.DanhSachLop> DanhSachLop { get; set; } = default!;
        public DbSet<BanKeKhaiNhapHoc.Models.HocVien> HocVien { get; set; } = default!;
        public DbSet<BanKeKhaiNhapHoc.Models.LopHoc> LopHoc { get; set; } = default!;
        public DbSet<BanKeKhaiNhapHoc.Models.User> Users { get; set; } = default!;
    }
}
