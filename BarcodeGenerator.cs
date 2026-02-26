using System;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp.Rendering;
using SkiaSharp;
namespace HBarPdfRazor;

public static class BarcodeService
{
    public static string GenerateCode128(string value, int width = 400, int height = 120)
    {
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 2,
                PureBarcode = true
            },
            Renderer = new SKBitmapRenderer()
        };

        using var bitmap = writer.Write(value);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return Convert.ToBase64String(data.ToArray());
    }
}