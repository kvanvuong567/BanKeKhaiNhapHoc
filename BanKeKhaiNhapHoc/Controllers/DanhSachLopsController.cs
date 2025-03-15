using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BanKeKhaiNhapHoc.Data;
using BanKeKhaiNhapHoc.Models;

namespace BanKeKhaiNhapHoc.Controllers
{
    public class DanhSachLopsController : Controller
    {
        private readonly BanKeKhaiNhapHocContext _context;

        public DanhSachLopsController(BanKeKhaiNhapHocContext context)
        {
            _context = context;
        }

        // 📌 Danh sách tất cả học viên trong lớp
        public async Task<IActionResult> Index()
        {
            var danhSach = _context.DanhSachLop
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc);
            return View(await danhSach.ToListAsync());
        }

        // 📌 Xem chi tiết học viên trong lớp
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var danhSachLop = await _context.DanhSachLop
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (danhSachLop == null) return NotFound();

            return View(danhSachLop);
        }

        // 📌 Form tạo mới danh sách lớp
        public IActionResult Create()
        {
            ViewData["HocVienId"] = new SelectList(_context.Set<HocVien>(), "Id", "HoTen");
            ViewData["LopHocId"] = new SelectList(_context.Set<LopHoc>(), "Id", "TenLop");
            return View();
        }

        // 📌 Xử lý khi tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HocVienId,LopHocId,TrangThai,NgayDangKy,NgayDuyet")] DanhSachLop danhSachLop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhSachLop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HocVienId"] = new SelectList(_context.Set<HocVien>(), "Id", "HoTen", danhSachLop.HocVienId);
            ViewData["LopHocId"] = new SelectList(_context.Set<LopHoc>(), "Id", "TenLop", danhSachLop.LopHocId);
            return View(danhSachLop);
        }

        // 📌 Form chỉnh sửa danh sách lớp
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var danhSachLop = await _context.DanhSachLop.FindAsync(id);
            if (danhSachLop == null) return NotFound();

            ViewData["HocVienId"] = new SelectList(_context.Set<HocVien>(), "Id", "HoTen", danhSachLop.HocVienId);
            ViewData["LopHocId"] = new SelectList(_context.Set<LopHoc>(), "Id", "TenLop", danhSachLop.LopHocId);
            return View(danhSachLop);
        }

        // 📌 Xử lý khi chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HocVienId,LopHocId,TrangThai,NgayDangKy,NgayDuyet")] DanhSachLop danhSachLop)
        {
            if (id != danhSachLop.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhSachLop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DanhSachLop.Any(e => e.Id == danhSachLop.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HocVienId"] = new SelectList(_context.Set<HocVien>(), "Id", "HoTen", danhSachLop.HocVienId);
            ViewData["LopHocId"] = new SelectList(_context.Set<LopHoc>(), "Id", "TenLop", danhSachLop.LopHocId);
            return View(danhSachLop);
        }

        // 📌 Form xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var danhSachLop = await _context.DanhSachLop
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (danhSachLop == null) return NotFound();

            return View(danhSachLop);
        }

        // 📌 Xử lý khi xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhSachLop = await _context.DanhSachLop.FindAsync(id);
            if (danhSachLop != null) _context.DanhSachLop.Remove(danhSachLop);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
