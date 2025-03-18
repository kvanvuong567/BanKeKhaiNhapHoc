using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace BanKeKhaiNhapHoc.Controllers
{
    public class QRCodeController : Controller
    {
        [HttpGet]
        public IActionResult GenerateQRCodeSvg()
        {
            // Tạo URL cho action "Create" của controller HocViens
            var createUrl = Url.Action("Create", "HocViens", null, Request.Scheme);

            // Kiểm tra nếu createUrl null hoặc rỗng thì trả về BadRequest
            if (string.IsNullOrEmpty(createUrl))
            {
                return BadRequest("Không thể tạo URL cho QR Code.");
            }

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(createUrl, QRCodeGenerator.ECCLevel.Q);

            var svgQrCode = new SvgQRCode(qrCodeData);
            string svgImage = svgQrCode.GetGraphic(10);

            return Content(svgImage, "image/svg+xml");
        }

        [HttpGet]
        public IActionResult Show()
        {
            return View();
        }
    }
}
