using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace WebPOSCafe.Pages.Admin
{
    public class GenerateQRCodesModel : PageModel
    {
        public List<TableQRCode> QRCodes { get; set; } = new();

        public void OnGet()
        {
            string baseUrl = "http://192.168.254.114:5292";

            for (int tableNo = 1; tableNo <= 20; tableNo++)
            {
                string url = $"{baseUrl}/Menu/MenuDisplay?table={tableNo}";

                string qrCodeBase64 = GenerateQRCode(url);

                QRCodes.Add(new TableQRCode
                {
                    TableNumber = tableNo,
                    Url = url,
                    QRCodeBase64 = qrCodeBase64
                });
            }
        }

        private string GenerateQRCode(string url)
        {
            using QRCodeGenerator qrGenerator = new();

            using QRCodeData qrCodeData =
                qrGenerator.CreateQrCode(url,
                    QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode qrCode =
                new(qrCodeData);

            byte[] qrBytes =
                qrCode.GetGraphic(20);

            return Convert.ToBase64String(qrBytes);
        }

        public class TableQRCode
        {
            public int TableNumber { get; set; }

            public string Url { get; set; } = "";

            public string QRCodeBase64 { get; set; } = "";
        }
    }
}
