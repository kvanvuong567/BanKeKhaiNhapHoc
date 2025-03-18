using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BanKeKhaiNhapHoc.Data;
using BanKeKhaiNhapHoc.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;

namespace BanKeKhaiNhapHoc.Controllers
{
    public class HocViensController : Controller
    {
        private readonly BanKeKhaiNhapHocContext _context;

        public HocViensController(BanKeKhaiNhapHocContext context)
        {
            _context = context;
        }

        // GET: HocViens
        public async Task<IActionResult> Index()
        {
            return View(await _context.HocVien.ToListAsync());
        }

        // GET: HocViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var hocVien = await _context.HocVien.FirstOrDefaultAsync(m => m.Id == id);
            if (hocVien == null)
                return NotFound();

            return View(hocVien);
        }

        // GET: HocViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HocViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocVien hocVien, string actionType)
        {
            if (!ModelState.IsValid)
            {
                return View(hocVien);
            }

            if (actionType == "save")
            {
                _context.Add(hocVien);
                await _context.SaveChangesAsync();

                // Sử dụng toán tử null-coalescing để tránh truyền giá trị null vào Session
                HttpContext.Session.SetString("LastHocVienName", hocVien.HoTen ?? string.Empty);

                return RedirectToAction(nameof(Details), new { id = hocVien.Id });
            }
            else if (actionType == "pdf")
            {
                using (var ms = new MemoryStream())
                {
                    // Tạo tài liệu PDF với kích thước A4 và margin 50
                    Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    // Đường dẫn tới file font Unicode hỗ trợ tiếng Việt (ví dụ arialuni.ttf)
                    string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "arialuni.ttf");
                    if (!System.IO.File.Exists(fontPath))
                    {
                        throw new FileNotFoundException("Không tìm thấy file font tại: " + fontPath);
                    }

                    BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font titleFont = new Font(bf, 18, Font.BOLD, BaseColor.Black);
                    Font labelFont = new Font(bf, 12, Font.BOLD, BaseColor.Black);
                    Font valueFont = new Font(bf, 12, Font.NORMAL, BaseColor.Black);

                    // Tiêu đề trang PDF
                    Paragraph title = new Paragraph("BẢN KÊ KHẢI NHẬP HỌC", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    doc.Add(title);

                    // Tạo bảng 2 cột để hiển thị thông tin
                    PdfPTable table = new PdfPTable(2)
                    {
                        WidthPercentage = 100,
                        SpacingBefore = 10f,
                        SpacingAfter = 10f
                    };
                    table.SetWidths(new float[] { 30f, 70f });

                    // Sửa hàm GetCell để nhận giá trị null
                    PdfPCell GetCell(string? text, Font font)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(text ?? string.Empty, font));
                        cell.Border = Rectangle.NO_BORDER;
                        cell.PaddingBottom = 5;
                        return cell;
                    }

                    // Thêm các dòng thông tin vào bảng theo thứ tự của form
                    table.AddCell(GetCell("1. Họ và tên:", labelFont));
                    table.AddCell(GetCell(hocVien.HoTen, valueFont));

                    table.AddCell(GetCell("2. Giới tính:", labelFont));
                    string gt = hocVien.GioiTinh.HasValue ? (hocVien.GioiTinh.Value ? "Nam" : "Nữ") : "Chưa chọn";
                    table.AddCell(GetCell(gt, valueFont));

                    table.AddCell(GetCell("3. Ngày sinh:", labelFont));
                    string ns = hocVien.NgaySinh.HasValue ? hocVien.NgaySinh.Value.ToString("dd/MM/yyyy") : "Chưa nhập";
                    table.AddCell(GetCell(ns, valueFont));

                    table.AddCell(GetCell("4. Nơi sinh (Tỉnh):", labelFont));
                    table.AddCell(GetCell(hocVien.NoiSinh, valueFont));

                    table.AddCell(GetCell("5. Quê quán (Tỉnh):", labelFont));
                    table.AddCell(GetCell(hocVien.QueQuan, valueFont));

                    table.AddCell(GetCell("6. Dân tộc:", labelFont));
                    table.AddCell(GetCell(hocVien.DanToc, valueFont));

                    table.AddCell(GetCell("7. Tôn giáo:", labelFont));
                    table.AddCell(GetCell(hocVien.TonGiao, valueFont));

                    table.AddCell(GetCell("8. Trình độ lý luận chính trị:", labelFont));
                    table.AddCell(GetCell(hocVien.TrinhDoLyLuanChinhTri, valueFont));

                    table.AddCell(GetCell("9. Trình độ chuyên môn nghiệp vụ:", labelFont));
                    table.AddCell(GetCell(hocVien.TrinhDoChuyenMonNghiepVu, valueFont));

                    table.AddCell(GetCell("10. Đảng viên đảng CSVN:", labelFont));
                    string dv = hocVien.DangVienCSVN.HasValue ? (hocVien.DangVienCSVN.Value ? "Đảng viên" : "Không phải đảng viên") : "Chưa chọn";
                    table.AddCell(GetCell(dv, valueFont));

                    table.AddCell(GetCell("11. Cán bộ - Cấp:", labelFont));
                    table.AddCell(GetCell(hocVien.CanBo, valueFont));

                    table.AddCell(GetCell("12. Công chức - Cấp:", labelFont));
                    table.AddCell(GetCell(hocVien.CongChucLevel, valueFont));

                    table.AddCell(GetCell("13. Người hoạt động không chuyên trách cấp xã:", labelFont));
                    string nd = hocVien.NguoiHoatDongKhongChuyenTrach ? "Có" : "Không";
                    table.AddCell(GetCell(nd, valueFont));

                    table.AddCell(GetCell("14. Đảng:", labelFont));
                    table.AddCell(GetCell(hocVien.ChucVu, valueFont));

                    table.AddCell(GetCell("15. Chính quyền:", labelFont));
                    table.AddCell(GetCell(hocVien.ChinhQuyen, valueFont));

                    table.AddCell(GetCell("16. Đoàn thể:", labelFont));
                    table.AddCell(GetCell(hocVien.DoanThe, valueFont));

                    table.AddCell(GetCell("17. Đơn vị công tác:", labelFont));
                    table.AddCell(GetCell(hocVien.DonViCongTac, valueFont));

                    table.AddCell(GetCell("18. Ngạch công chức, viên chức:", labelFont));
                    table.AddCell(GetCell(hocVien.NgachCongChuc, valueFont));

                    table.AddCell(GetCell("19. Hệ số lương:", labelFont));
                    table.AddCell(GetCell(hocVien.HeSoLuong?.ToString() ?? string.Empty, valueFont));

                    table.AddCell(GetCell("20. Hệ số phụ cấp chức vụ:", labelFont));
                    table.AddCell(GetCell(hocVien.HeSoPhuCapChucVu?.ToString() ?? string.Empty, valueFont));

                    table.AddCell(GetCell("21. Điện thoại di động:", labelFont));
                    table.AddCell(GetCell(hocVien.DienThoaiDiDong, valueFont));

                    table.AddCell(GetCell("22. Email:", labelFont));
                    table.AddCell(GetCell(hocVien.Email, valueFont));

                    doc.Add(table);
                    doc.Close();

                    byte[] fileBytes = ms.ToArray();
                    return File(fileBytes, "application/pdf", "BanKeKhaiNhapHoc.pdf");
                }
            }

            return View(hocVien);
        }

        // GET: HocViens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var hocVien = await _context.HocVien.FindAsync(id);
            if (hocVien == null)
                return NotFound();

            return View(hocVien);
        }

        // POST: HocViens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HocVien hocVien)
        {
            if (id != hocVien.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.HocVien.Any(e => e.Id == hocVien.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hocVien);
        }

        // GET: HocViens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hocVien = await _context.HocVien.FirstOrDefaultAsync(m => m.Id == id);
            if (hocVien == null)
                return NotFound();

            return View(hocVien);
        }

        // POST: HocViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocVien = await _context.HocVien.FindAsync(id);
            if (hocVien != null)
            {
                _context.HocVien.Remove(hocVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
