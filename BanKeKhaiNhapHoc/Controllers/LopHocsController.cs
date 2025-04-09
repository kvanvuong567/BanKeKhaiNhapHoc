using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BanKeKhaiNhapHoc.Data;
using BanKeKhaiNhapHoc.Models;
using ClosedXML.Excel;
using System.IO;

namespace BanKeKhaiNhapHoc.Controllers
{
    public class LopHocsController : Controller
    {
        private readonly BanKeKhaiNhapHocContext _context;

        public LopHocsController(BanKeKhaiNhapHocContext context)
        {
            _context = context;
        }

        // GET: LopHocs
        public async Task<IActionResult> Index()
        {
            return View(await _context.LopHoc.ToListAsync());
        }

        // GET: LopHocs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHoc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lopHoc == null)
            {
                return NotFound();
            }

            return View(lopHoc);
        }

        // GET: LopHocs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LopHocs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenLop,MoTaLop,TenKhoaHoc,NgayBatDau,NgayKetThuc,MoTaKhoaHoc")] LopHoc lopHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lopHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lopHoc);
        }

        // GET: LopHocs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHoc.FindAsync(id);
            if (lopHoc == null)
            {
                return NotFound();
            }
            return View(lopHoc);
        }

        // POST: LopHocs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenLop,MoTaLop,TenKhoaHoc,NgayBatDau,NgayKetThuc,MoTaKhoaHoc")] LopHoc lopHoc)
        {
            if (id != lopHoc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lopHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopHocExists(lopHoc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lopHoc);
        }

        // GET: LopHocs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHoc = await _context.LopHoc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lopHoc == null)
            {
                return NotFound();
            }

            return View(lopHoc);
        }

        // POST: LopHocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lopHoc = await _context.LopHoc.FindAsync(id);
            if (lopHoc != null)
            {
                _context.LopHoc.Remove(lopHoc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LopHocExists(int id)
        {
            return _context.LopHoc.Any(e => e.Id == id);
        }

        // GET: LopHocs/ExportExcel
        public IActionResult ExportExcel()
        {
            try
            {
                // Lấy danh sách tất cả các lớp học
                var lopHocs = _context.LopHoc.ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("LopHocs");

                    // Ghi header cho file Excel
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Tên Lớp";
                    worksheet.Cell(1, 3).Value = "Mô Tả Lớp";
                    worksheet.Cell(1, 4).Value = "Tên Khóa Học";
                    worksheet.Cell(1, 5).Value = "Ngày Bắt Đầu";
                    worksheet.Cell(1, 6).Value = "Ngày Kết Thúc";
                    worksheet.Cell(1, 7).Value = "Mô Tả Khóa Học";

                    int row = 2;
                    foreach (var item in lopHocs)
                    {
                        worksheet.Cell(row, 1).Value = item.Id;
                        worksheet.Cell(row, 2).Value = item.TenLop;
                        worksheet.Cell(row, 3).Value = item.MoTaLop;
                        worksheet.Cell(row, 4).Value = item.TenKhoaHoc;
                        worksheet.Cell(row, 5).Value = item.NgayBatDau.HasValue ? item.NgayBatDau.Value.ToString("dd/MM/yyyy") : "";
                        worksheet.Cell(row, 6).Value = item.NgayKetThuc.HasValue ? item.NgayKetThuc.Value.ToString("dd/MM/yyyy") : "";
                        worksheet.Cell(row, 7).Value = item.MoTaKhoaHoc;

                        // Thêm viền cho mỗi ô dữ liệu
                        for (int col = 1; col <= 7; col++)
                        {
                            var cell = worksheet.Cell(row, col);
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        }

                        row++;
                    }

                    // Điều chỉnh kích thước cột tự động
                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "LopHocs.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Có lỗi khi xuất file Excel: " + ex.Message);
            }
        }
    }
}
