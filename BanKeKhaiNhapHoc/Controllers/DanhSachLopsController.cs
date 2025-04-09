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

        // GET: DanhSachLops
        public async Task<IActionResult> Index()
        {
            var danhSach = _context.DanhSachLop
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                // Lọc chỉ những học viên đã duyệt, tức là khi HocVien.IsApproved == true 
                // (hoặc có thể lọc theo TrangThai == "Đậu" nếu bạn sử dụng trường này làm tiêu chí)
                .Where(d => d.HocVien.IsApproved == true);
            return View(await danhSach.ToListAsync());
        }


        // GET: DanhSachLops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var danhSachLop = await _context.DanhSachLop
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhSachLop == null)
                return NotFound();

            return View(danhSachLop);
        }

        // GET: DanhSachLops/Create
        public IActionResult Create()
        {
            ViewData["HocVienId"] = new SelectList(_context.Set<HocVien>(), "Id", "HoTen");
            ViewData["LopHocId"] = new SelectList(_context.Set<LopHoc>(), "Id", "TenLop");
            return View();
        }

        // POST: DanhSachLops/Create
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

        // GET: DanhSachLops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var danhSachLop = await _context.DanhSachLop.FindAsync(id);
            if (danhSachLop == null)
                return NotFound();

            ViewData["HocVienId"] = new SelectList(_context.Set<HocVien>(), "Id", "HoTen", danhSachLop.HocVienId);
            ViewData["LopHocId"] = new SelectList(_context.Set<LopHoc>(), "Id", "TenLop", danhSachLop.LopHocId);
            return View(danhSachLop);
        }

        // POST: DanhSachLops/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HocVienId,LopHocId,TrangThai,NgayDangKy,NgayDuyet")] DanhSachLop danhSachLop)
        {
            if (id != danhSachLop.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhSachLop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DanhSachLop.Any(e => e.Id == danhSachLop.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HocVienId"] = new SelectList(_context.Set<HocVien>(), "Id", "HoTen", danhSachLop.HocVienId);
            ViewData["LopHocId"] = new SelectList(_context.Set<LopHoc>(), "Id", "TenLop", danhSachLop.LopHocId);
            return View(danhSachLop);
        }

        // GET: DanhSachLops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var danhSachLop = await _context.DanhSachLop
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhSachLop == null)
                return NotFound();

            return View(danhSachLop);
        }

        // POST: DanhSachLops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhSachLop = await _context.DanhSachLop.FindAsync(id);
            if (danhSachLop != null)
            {
                _context.DanhSachLop.Remove(danhSachLop);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
