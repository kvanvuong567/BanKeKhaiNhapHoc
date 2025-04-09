using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace BanKeKhaiNhapHoc.Controllers
{
    public class QRCodeController : Controller
    {
        // Action này tạo mã QR dạng SVG cho trang Create của HocViens,
        // kèm theo tham số lopHocId để biết lớp học tương ứng.
        [HttpGet]
        public IActionResult GenerateQRCodeSvgForClass(int lopHocId)
        {
            // Tạo URL: ví dụ http://domain/HocViens/Create?lopHocId=5
            var createUrl = Url.Action("Create", "HocViens", new { lopHocId = lopHocId }, Request.Scheme);
            if (string.IsNullOrEmpty(createUrl))
            {
                return BadRequest("Không thể tạo URL cho QR Code.");
            }

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(createUrl, QRCodeGenerator.ECCLevel.Q);
            var svgQrCode = new SvgQRCode(qrCodeData);
            // Tham số GetGraphic(10) điều chỉnh kích thước mã QR (có thể thay đổi theo ý bạn)
            string svgImage = svgQrCode.GetGraphic(10);

            return Content(svgImage, "image/svg+xml");
        }

        // Action hiển thị trang test mã QR (nếu cần)
        [HttpGet]
        public IActionResult Show()
        {
            return View();
        }
    }
}
