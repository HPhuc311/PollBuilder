using QRCoder;

namespace PollBuilder.Infrastructure.Services;

public class QrCodeService
{
    public string GenerateQrCode(string url)
    {
        using QRCodeGenerator generator = new();

        using QRCodeData data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

        Base64QRCode qr = new(data);

        return qr.GetGraphic(20);
    }
}