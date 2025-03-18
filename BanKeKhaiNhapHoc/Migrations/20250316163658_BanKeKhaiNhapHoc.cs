using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanKeKhaiNhapHoc.Migrations
{
    /// <inheritdoc />
    public partial class BanKeKhaiNhapHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HocVien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiSinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QueQuan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DanToc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TonGiao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrinhDoLyLuanChinhTri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrinhDoChuyenMonNghiepVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DangVienCSVN = table.Column<bool>(type: "bit", nullable: true),
                    CanBo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CongChucLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NguoiHoatDongKhongChuyenTrach = table.Column<bool>(type: "bit", nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChinhQuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChucVuChinhQuyenKhac = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DoanThe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DonViCongTac = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NgachCongChuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HeSoLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HeSoPhuCapChucVu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DienThoaiDiDong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocVien", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LopHoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLop = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTaLop = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenKhoaHoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTaKhoaHoc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhSachLop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HocVienId = table.Column<int>(type: "int", nullable: false),
                    LopHocId = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhSachLop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhSachLop_HocVien_HocVienId",
                        column: x => x.HocVienId,
                        principalTable: "HocVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhSachLop_LopHoc_LopHocId",
                        column: x => x.LopHocId,
                        principalTable: "LopHoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachLop_HocVienId",
                table: "DanhSachLop",
                column: "HocVienId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachLop_LopHocId",
                table: "DanhSachLop",
                column: "LopHocId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhSachLop");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "LopHoc");
        }
    }
}
