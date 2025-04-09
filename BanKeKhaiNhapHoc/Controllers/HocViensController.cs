using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BanKeKhaiNhapHoc.Data;
using BanKeKhaiNhapHoc.Models;
using Microsoft.AspNetCore.Http;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
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
            // Include thông tin DanhSachLops và trong đó có LopHoc để hiển thị tên lớp
            var hocViens = await _context.HocVien
                .Include(hv => hv.DanhSachLops)
                    .ThenInclude(ds => ds.LopHoc)
                .ToListAsync();
            return View(hocViens);
        }

        // GET: HocViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // Include thêm thông tin DanhSachLops và LopHoc nếu cần hiển thị chi tiết lớp học
            var hocVien = await _context.HocVien
                .Include(hv => hv.DanhSachLops)
                    .ThenInclude(ds => ds.LopHoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hocVien == null)
                return NotFound();

            return View(hocVien);
        }

        // GET: HocViens/Create
        public async Task<IActionResult> Create(int? lopHocId)
        {
            // Tạo một đối tượng HocVien mới để bind vào View
            var model = new HocVien();

            // Nếu quét QR => lopHocId có giá trị
            if (lopHocId.HasValue)
            {
                var lopHoc = await _context.LopHoc.FindAsync(lopHocId);
                if (lopHoc != null)
                {
                    // Gán vào ViewBag để View biết lớp nào được chọn
                    ViewBag.LopHocId = lopHocId;
                    ViewBag.LopHocName = lopHoc.TenLop;
                    ViewBag.LopHocDescription = lopHoc.MoTaLop;

                    // QUAN TRỌNG: Gán SelectedLopHocId cho model
                    model.SelectedLopHocId = lopHocId;
                }
                else
                {
                    return NotFound();
                }
            }

            // Luôn chuẩn bị danh sách lớp => Dùng cho dropdown khi KHÔNG quét QR
            ViewBag.LopHocList = new SelectList(await _context.LopHoc.ToListAsync(), "Id", "TenLop");

            // Trả về View kèm model (rất quan trọng)
            return View(model);
        }

        // POST: HocViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocVien hocVien, string actionType)
        {
            if (!ModelState.IsValid)
                return View(hocVien);

            if (actionType == "save")
            {
                // Lưu học viên vào DB
                _context.Add(hocVien);
                await _context.SaveChangesAsync();

                if (hocVien.SelectedLopHocId.HasValue)
                {
                    var danhSachLop = new DanhSachLop
                    {
                        HocVienId = hocVien.Id,
                        LopHocId = hocVien.SelectedLopHocId.Value,
                        TrangThai = "Chờ duyệt",
                        NgayDangKy = DateTime.Now,
                    };
                    _context.DanhSachLop.Add(danhSachLop);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Details), new { id = hocVien.Id });
            }
            else if (actionType == "pdf")
            {
                using (var ms = new MemoryStream())
                {
                    Document pdfDoc = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                    pdfDoc.Open();

                    string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Times New Roman Regular.ttf");
                    if (!System.IO.File.Exists(fontPath))
                        throw new FileNotFoundException("Không tìm thấy font Times New Roman tại: " + fontPath);
                    BaseFont bfTimes = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font normalFont = new Font(bfTimes, 13, Font.NORMAL, BaseColor.Black);
                    Font boldFont = new Font(bfTimes, 13, Font.BOLD, BaseColor.Black);
                    Font italicFont = new Font(bfTimes, 13, Font.ITALIC, BaseColor.Black);

                    // ======= HEADER =======
                    PdfPTable headerTable = new PdfPTable(2);
                    headerTable.WidthPercentage = 100;
                    headerTable.SetWidths(new float[] { 50f, 50f });
                    headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell leftCell = new PdfPCell { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                    Paragraph line1Left = new Paragraph("TỈNH ỦY LÂM ĐỒNG", normalFont);
                    Paragraph line2Left = new Paragraph("TRƯỜNG CHÍNH TRỊ", boldFont);
                    leftCell.AddElement(line1Left);
                    leftCell.AddElement(line2Left);
                    headerTable.AddCell(leftCell);

                    PdfPCell rightCell = new PdfPCell { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                    Chunk dangChunk = new Chunk("ĐẢNG CỘNG SẢN VIỆT NAM", boldFont);
                    dangChunk.SetUnderline(1, -1);
                    Paragraph line1Right = new Paragraph();
                    line1Right.Add(dangChunk);
                    DateTime now = DateTime.Now;
                    string dateStr = $"Lâm Đồng, ngày {now:dd} tháng {now:MM} năm {now:yyyy}";
                    Paragraph line2Right = new Paragraph(dateStr, italicFont);
                    rightCell.AddElement(line1Right);
                    rightCell.AddElement(line2Right);
                    headerTable.AddCell(rightCell);

                    pdfDoc.Add(headerTable);
                    pdfDoc.Add(new Paragraph("\n", normalFont));

                    // ======= TIÊU ĐỀ =======
                    Paragraph title = new Paragraph("BẢN KÊ KHAI NHẬP HỌC", boldFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 5
                    };
                    pdfDoc.Add(title);

                    // ======= LỚP: ... =======
                    string tenLop = "";
                    if (hocVien.SelectedLopHocId.HasValue)
                    {
                        var lopHoc = await _context.LopHoc.FindAsync(hocVien.SelectedLopHocId.Value);
                        if (lopHoc != null)
                            tenLop = lopHoc.TenLop;
                    }
                    Paragraph lopPara = new Paragraph($"LỚP: {tenLop}", boldFont)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 30f,
                        SpacingAfter = 10
                    };
                    pdfDoc.Add(lopPara);

                    // ======= BẢNG THÔNG TIN HỌC VIÊN =======
                    PdfPTable infoTable = new PdfPTable(1);
                    infoTable.WidthPercentage = 100;
                    infoTable.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell MakeCell(string text, Font font, float spacingAfter = 3f)
                    {
                        var cell = new PdfPCell(new Phrase(text, font));
                        cell.Border = Rectangle.NO_BORDER;
                        cell.PaddingBottom = spacingAfter;
                        cell.NoWrap = false; // Cho phép xuống hàng tự động
                        return cell;
                    }

                    // Sử dụng ký tự "√" nếu tick, nếu không thì chuỗi rỗng.
                    string tick = "√";

                    // 1. Họ và tên
                    infoTable.AddCell(MakeCell($"1. Họ và tên (viết chữ in hoa): {hocVien.HoTen ?? ""}", normalFont));

                    // 2. Giới tính: hiển thị trên 1 dòng, đẩy sang bên phải một chút.
                    string genderText = "2. Giới tính: ";
                    if (hocVien.GioiTinh == true)
                        genderText += "Nam " + tick + "    Nữ";
                    else if (hocVien.GioiTinh == false)
                        genderText += "Nam    Nữ " + tick;
                    else
                        genderText += "Nam    Nữ";
                    // Thêm khoảng cách indent nhỏ (ví dụ 20 đơn vị) sau nhãn
                    Paragraph genderPara = new Paragraph(genderText, normalFont)
                    {
                        IndentationLeft = 20f
                    };
                    infoTable.AddCell(genderPara);

                    // 3. Ngày sinh
                    string ngaySinh = hocVien.NgaySinh?.ToString("dd/MM/yyyy") ?? "";
                    infoTable.AddCell(MakeCell($"3. Ngày sinh: {ngaySinh}", normalFont));

                    // 4. Nơi sinh (Tỉnh)
                    infoTable.AddCell(MakeCell($"4. Nơi sinh (Tỉnh): {hocVien.NoiSinh ?? ""}", normalFont));

                    // 5. Quê quán (Tỉnh)
                    infoTable.AddCell(MakeCell($"5. Quê quán (Tỉnh): {hocVien.QueQuan ?? ""}", normalFont));

                    // 6. Dân tộc
                    infoTable.AddCell(MakeCell($"6. Dân tộc: {hocVien.DanToc ?? ""}", normalFont));

                    // 7. Tôn giáo
                    infoTable.AddCell(MakeCell($"7. Tôn giáo: {hocVien.TonGiao ?? ""}", normalFont));

                    // 8. Trình độ LLCT: Sơ cấp, Trung cấp, Cao cấp
                    string tdLLCT = hocVien.TrinhDoLyLuanChinhTri ?? "";
                    string soCap = (tdLLCT == "Sơ cấp") ? tick : "";
                    string trungCap = (tdLLCT == "Trung cấp") ? tick : "";
                    string caoCap = (tdLLCT == "Cao cấp") ? tick : "";
                    infoTable.AddCell(MakeCell($"8. Trình độ LLCT: Sơ cấp {soCap}   Trung cấp {trungCap}   Cao cấp {caoCap}", normalFont));

                    // 9. Trình độ chuyên môn: chia làm 2 dòng
                    Paragraph tdcmHeader = new Paragraph("9. Trình độ chuyên môn:", normalFont)
                    { SpacingAfter = 3 };
                    infoTable.AddCell(tdcmHeader);
                    string[] tdcmOptionsLine1 = new string[] { "Trung cấp", "Cao đẳng", "Đại học" };
                    string[] tdcmOptionsLine2 = new string[] { "Thạc sĩ", "Tiến sĩ" };
                    Paragraph tdcmLine1 = new Paragraph("", normalFont);
                    foreach (var option in tdcmOptionsLine1)
                    {
                        string tickOption = (hocVien.TrinhDoChuyenMonNghiepVu == option) ? tick : "";
                        tdcmLine1.Add(new Chunk($"{option} {tickOption}    ", normalFont));
                    }
                    Paragraph tdcmLine2 = new Paragraph("", normalFont);
                    foreach (var option in tdcmOptionsLine2)
                    {
                        string tickOption = (hocVien.TrinhDoChuyenMonNghiepVu == option) ? tick : "";
                        tdcmLine2.Add(new Chunk($"{option} {tickOption}    ", normalFont));
                    }
                    infoTable.AddCell(tdcmLine1);
                    infoTable.AddCell(tdcmLine2);

                    // 10. Đảng viên đảng CSVN: Dự bị / Chính thức
                    string dbSymbol = "";
                    string ctSymbol = "";
                    if (hocVien.DangVienCSVN.HasValue)
                    {
                        if (hocVien.DangVienCSVN.Value)
                            dbSymbol = tick;
                        else
                            ctSymbol = tick;
                    }
                    infoTable.AddCell(MakeCell($"10. Đảng viên đảng CSVN: Dự bị {dbSymbol}   Chính thức {ctSymbol}", normalFont));

                    // 11. Cán bộ (Cấp): Lựa chọn: Cấp tỉnh, Cấp huyện, Cấp xã
                    Paragraph canBoHeader = new Paragraph("11. Cán bộ (Cấp):", normalFont) { SpacingAfter = 3 };
                    infoTable.AddCell(canBoHeader);
                    string[] canBoOptions = new string[] { "Cấp tỉnh", "Cấp huyện", "Cấp xã" };
                    Paragraph canBoLine = new Paragraph("", normalFont);
                    foreach (var option in canBoOptions)
                    {
                        string tickOption = (hocVien.CanBo == option) ? tick : "";
                        canBoLine.Add(new Chunk($"{option} {tickOption}    ", normalFont));
                    }
                    infoTable.AddCell(canBoLine);

                    // 12. Công chức (Cấp): Lựa chọn: Cấp tỉnh, Cấp huyện, Cấp xã, Công chức hành chính
                    Paragraph congChucHeader = new Paragraph("12. Công chức (Cấp):", normalFont) { SpacingAfter = 3 };
                    infoTable.AddCell(congChucHeader);
                    string[] congChucOptions = new string[] { "Cấp tỉnh", "Cấp huyện", "Cấp xã", "Công chức hành chính" };
                    Paragraph congChucLine = new Paragraph("", normalFont);
                    foreach (var option in congChucOptions)
                    {
                        string tickOption = (hocVien.CongChucLevel == option) ? tick : "";
                        congChucLine.Add(new Chunk($"{option} {tickOption}    ", normalFont));
                    }
                    infoTable.AddCell(congChucLine);

                    // 13. Người hoạt động không chuyên trách:
                    string kctText = hocVien.NguoiHoatDongKhongChuyenTrach ? "Có" : "Không";
                    infoTable.AddCell(MakeCell($"13. Người hoạt động không chuyên trách: {kctText}", normalFont));

                    // 14. Chức vụ (Đảng)
                    infoTable.AddCell(MakeCell($"14. Chức vụ (Đảng): {hocVien.ChucVu ?? ""}", normalFont));

                    // 15. Chính quyền: Lựa chọn: Tỉnh, Huyện, Xã, Khác
                    Paragraph cqHeader = new Paragraph("15. Chính quyền:", normalFont) { SpacingAfter = 3 };
                    infoTable.AddCell(cqHeader);
                    string[] cqOptions = new string[] { "Tỉnh", "Huyện", "Xã", "Khác" };
                    Paragraph cqLine = new Paragraph("", normalFont);
                    foreach (var option in cqOptions)
                    {
                        string tickOption = (hocVien.ChinhQuyen == option) ? tick : "";
                        cqLine.Add(new Chunk($"{option} {tickOption}    ", normalFont));
                    }
                    infoTable.AddCell(cqLine);

                    // 16. Đoàn thể
                    infoTable.AddCell(MakeCell($"16. Đoàn thể: {hocVien.DoanThe ?? ""}", normalFont));

                    // 17. Đơn vị công tác
                    infoTable.AddCell(MakeCell($"17. Đơn vị công tác: {hocVien.DonViCongTac ?? ""}", normalFont));

                    // 18. Ngạch công chức, viên chức
                    infoTable.AddCell(MakeCell($"18. Ngạch công chức, viên chức: {hocVien.NgachCongChuc ?? ""}", normalFont));

                    // 19. Hệ số lương
                    infoTable.AddCell(MakeCell($"19. Hệ số lương: {hocVien.HeSoLuong}", normalFont));

                    // 20. Hệ số phụ cấp chức vụ
                    infoTable.AddCell(MakeCell($"20. Hệ số phụ cấp chức vụ: {hocVien.HeSoPhuCapChucVu}", normalFont));

                    // 21. Liên hệ: Hiển thị xuống hàng tự động khi không đủ chỗ
                    string contactInfo = $"Điện thoại di động: {hocVien.DienThoaiDiDong ?? ""}\nEmail: {hocVien.Email ?? ""}";
                    infoTable.AddCell(MakeCell($"21. {contactInfo}", normalFont));

                    pdfDoc.Add(infoTable);

                    pdfDoc.Close();
                    byte[] pdfBytes = ms.ToArray();
                    return File(pdfBytes, "application/pdf", $"HocVien_{hocVien.Id}.pdf");
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

            // Nạp danh sách lớp để hiển thị dropdown
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "Id", "TenLop");

            // Nếu học viên đã có lớp, gán giá trị SelectedLopHocId từ record DanhSachLop (nếu chỉ cho phép 1 lớp)
            var ds = await _context.DanhSachLop.FirstOrDefaultAsync(d => d.HocVienId == hocVien.Id);
            if (ds != null)
            {
                hocVien.SelectedLopHocId = ds.LopHocId;
            }
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
                    // Cập nhật thông tin học viên
                    _context.Update(hocVien);

                    // Tìm record DanhSachLop hiện tại cho học viên
                    var dsRecord = await _context.DanhSachLop.FirstOrDefaultAsync(d => d.HocVienId == hocVien.Id);

                    if (hocVien.SelectedLopHocId.HasValue)
                    {
                        // Nếu có record, cập nhật; nếu chưa có, tạo mới
                        if (dsRecord != null)
                        {
                            dsRecord.LopHocId = hocVien.SelectedLopHocId.Value;
                            dsRecord.TrangThai = "Chờ duyệt"; // hoặc trạng thái bạn mong muốn
                                                              // Nếu cần cập nhật thêm các trường khác như NgayDangKy, NgayDuyet, bạn có thể làm ở đây
                            _context.Update(dsRecord);
                        }
                        else
                        {
                            var newDs = new DanhSachLop
                            {
                                HocVienId = hocVien.Id,
                                LopHocId = hocVien.SelectedLopHocId.Value,
                                TrangThai = "Chờ duyệt",
                                NgayDangKy = DateTime.Now
                                // NgayDuyet có thể để null cho đến khi duyệt
                            };
                            _context.DanhSachLop.Add(newDs);
                        }
                    }
                    else
                    {
                        // Nếu không chọn lớp, xóa record nếu có
                        if (dsRecord != null)
                        {
                            _context.DanhSachLop.Remove(dsRecord);
                        }
                    }

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

        // GET: HocViens/Approve/5?lopHocId=...
        // Action này xử lý việc duyệt đậu học viên và lưu record vào bảng DanhSachLop
        public async Task<IActionResult> Approve(int id, int? lopHocId)
        {
            // Lấy học viên kèm theo DanhSachLops
            var hocVien = await _context.HocVien
                .Include(hv => hv.DanhSachLops)
                    .ThenInclude(ds => ds.LopHoc)
                .FirstOrDefaultAsync(hv => hv.Id == id);

            if (hocVien == null)
            {
                return NotFound();
            }

            // Nếu parameter lopHocId không có, hãy lấy từ record đã lưu
            int selectedLopHocId = lopHocId ?? hocVien.DanhSachLops.FirstOrDefault()?.LopHocId ?? 0;
            if (selectedLopHocId == 0)
            {
                return BadRequest("Chưa có lớp học được chọn.");
            }

            // Cập nhật trạng thái duyệt đậu của học viên
            hocVien.IsApproved = true;
            _context.Update(hocVien);
            await _context.SaveChangesAsync();

            // Kiểm tra nếu chưa có record trong DanhSachLop cho lớp này
            bool exists = await _context.DanhSachLop
                                .AnyAsync(d => d.HocVienId == hocVien.Id && d.LopHocId == selectedLopHocId);
            if (!exists)
            {
                var danhSachLop = new DanhSachLop
                {
                    HocVienId = hocVien.Id,
                    LopHocId = selectedLopHocId,
                    TrangThai = "Đậu",
                    NgayDangKy = DateTime.Now,
                    NgayDuyet = DateTime.Now
                };
                _context.DanhSachLop.Add(danhSachLop);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = hocVien.Id });
        }

        public async Task<IActionResult> ExportExcel()
        {
            // Lấy danh sách học viên, kèm theo thông tin lớp học nếu có
            var hocViens = await _context.HocVien
                .Include(hv => hv.DanhSachLops)
                    .ThenInclude(ds => ds.LopHoc)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                // Tạo một worksheet mới
                var worksheet = workbook.Worksheets.Add("HocViens");

                // Tạo header cho bảng Excel
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Họ và tên";
                worksheet.Cell(1, 3).Value = "Ngày sinh";
                worksheet.Cell(1, 4).Value = "Giới tính";
                worksheet.Cell(1, 5).Value = "Dân tộc";
                worksheet.Cell(1, 6).Value = "Tôn giáo";
                worksheet.Cell(1, 7).Value = "Quê quán";
                worksheet.Cell(1, 8).Value = "Nơi sinh";
                worksheet.Cell(1, 9).Value = "Lớp";
                worksheet.Cell(1, 10).Value = "Lý luận CT";
                worksheet.Cell(1, 11).Value = "Chuyên môn";
                worksheet.Cell(1, 12).Value = "Cán bộ";
                worksheet.Cell(1, 13).Value = "Công chức";
                worksheet.Cell(1, 14).Value = "Không chuyên trách";
                worksheet.Cell(1, 15).Value = "Đảng";
                worksheet.Cell(1, 16).Value = "Chính quyền";
                worksheet.Cell(1, 17).Value = "Đoàn thể";
                worksheet.Cell(1, 18).Value = "Hệ số lương";
                worksheet.Cell(1, 19).Value = "Hệ số PC";
                worksheet.Cell(1, 20).Value = "Điện thoại";
                worksheet.Cell(1, 21).Value = "Email";

                // Định dạng header (in đậm, nền xám nhạt, và viền)
                for (int col = 1; col <= 21; col++)
                {
                    var cell = worksheet.Cell(1, col);
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                // Đổ dữ liệu vào bảng Excel
                int row = 2;
                int stt = 1;
                foreach (var hv in hocViens)
                {
                    worksheet.Cell(row, 1).Value = stt;
                    worksheet.Cell(row, 2).Value = hv.HoTen;
                    worksheet.Cell(row, 3).Value = hv.NgaySinh?.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 4).Value = hv.GioiTinh.HasValue ? (hv.GioiTinh.Value ? "Nam" : "Nữ") : "";
                    worksheet.Cell(row, 5).Value = hv.DanToc;
                    worksheet.Cell(row, 6).Value = hv.TonGiao;
                    worksheet.Cell(row, 7).Value = hv.QueQuan;
                    worksheet.Cell(row, 8).Value = hv.NoiSinh;
                    if (hv.DanhSachLops != null && hv.DanhSachLops.Any())
                    {
                        worksheet.Cell(row, 9).Value = string.Join(", ", hv.DanhSachLops.Select(ds => ds.LopHoc?.TenLop));
                    }
                    else
                    {
                        worksheet.Cell(row, 9).Value = "-";
                    }
                    worksheet.Cell(row, 10).Value = hv.TrinhDoLyLuanChinhTri;
                    worksheet.Cell(row, 11).Value = hv.TrinhDoChuyenMonNghiepVu;
                    worksheet.Cell(row, 12).Value = hv.CanBo;
                    worksheet.Cell(row, 13).Value = hv.CongChucLevel;
                    worksheet.Cell(row, 14).Value = hv.NguoiHoatDongKhongChuyenTrach ? "Có" : "Không";
                    worksheet.Cell(row, 15).Value = hv.ChucVu;
                    worksheet.Cell(row, 16).Value = hv.ChinhQuyen;
                    worksheet.Cell(row, 17).Value = hv.DoanThe;
                    worksheet.Cell(row, 18).Value = hv.HeSoLuong;
                    worksheet.Cell(row, 19).Value = hv.HeSoPhuCapChucVu;
                    worksheet.Cell(row, 20).Value = hv.DienThoaiDiDong;
                    worksheet.Cell(row, 21).Value = hv.Email;

                    // Thêm viền cho mỗi ô dữ liệu
                    for (int col = 1; col <= 21; col++)
                    {
                        var cell = worksheet.Cell(row, col);
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    }

                    row++;
                    stt++;
                }

                // Tự động điều chỉnh độ rộng cột cho vừa nội dung
                worksheet.Columns().AdjustToContents();

                // Lưu workbook vào MemoryStream và trả về file Excel
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "HocViens.xlsx");
                }
            }
        }

    }
}
