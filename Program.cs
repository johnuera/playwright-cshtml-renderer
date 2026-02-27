using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PDFGenerator.Components;
namespace PDFGenerator;

public class Program
{
    static async Task Main(string[] args)
    {
        var outputPdf = args.Length > 0 ? args[0] : "report.pdf";
        var culture = args.Length > 1 ? args[1] : "de";

        // ---- Assets ----
        var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "fonts", "Limelight-Regular.woff2");
        byte[] fontBytes = await File.ReadAllBytesAsync(fontPath);
        string fontBase64 =Convert.ToBase64String(fontBytes);
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "images", "logo.png");
        byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
        string logoBase64 = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
         // ---- Load locale json ----
        var locale = Localisation.LoadLocale(culture);

        // ---- Build model ----
        var model = new InvoiceTemplateModel
        {
            Culture = culture,
            Locale = locale,
            BarcodeBase64 = BarcodeService.GenerateCode128("123456789012"),
            FontBase64 = fontBase64,
            LogoBase64 = logoBase64,

            SellerAddress =
            @"Distorted People GmbH
            Example Street 123
            1234 AB Example City
            COUNTRY",

                        PlatformName = "Online Marketplace Platform",
                        PlatformAddress =
                        @"Platform Street 456
            5678 CD Platform City
            COUNTRY",

            OrderDate = "01.01.2026",
            OrderNumber = "ORD-0000001",
            CustomerNumber = "CUST-000001",
            SellerReference = "REF-000001",

            Lines = new()
            {
                new Product { Description = "Sample Product Description - Size M - Removable Insole", Condition = "New", IsbnOrEan = "0000000000000", Quantity = 1 },
                new Product { Description = "Sample Product Description - Size M - Removable Insole", Condition = "New", IsbnOrEan = "0000000000000", Quantity = 1 },
                new Product { Description = "Sample Product Description - Size M - Removable Insole", Condition = "New", IsbnOrEan = "0000000000000", Quantity = 1 },
                new Product { Description = "Sample Product Description - Size M - Removable Insole", Condition = "New", IsbnOrEan = "0000000000000", Quantity = 1 },
            },
        };

        // ---- Blazor (component) -> HTML ----
        var html = await RenderBlazorComponentToHtmlAsync(model);

        // ---- Playwright -> PDF ----
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();

        // Load your HTML first (no external resources needed)
        await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.Load });

        // Generate PDF
        var pdfBytes = await page.PdfAsync(new()
        {
            Format = "A4",
            PrintBackground = true,
            Margin = new Margin
            {
                Top = "14mm",
                Right = "14mm",
                Bottom = "14mm",
                Left = "14mm"
            }
        });

        await File.WriteAllBytesAsync(outputPdf, pdfBytes);

        Console.WriteLine($"PDF written to: {Path.GetFullPath(outputPdf)}");
        Console.WriteLine($"Locale used: {culture}");
    }

    private static async Task<string> RenderBlazorComponentToHtmlAsync(InvoiceTemplateModel model)
    {
        var services = new ServiceCollection();

        services.AddLogging(b => b.AddConsole());

        // Add any services your component needs here, e.g. localisation, formatting, etc.
        // services.AddSingleton<SomeService>();

        await using var sp = services.BuildServiceProvider();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        var renderer = new HtmlRenderer(sp, loggerFactory);

        // HtmlRenderer requires work to run on its dispatcher
        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                ["Model"] = model
            });

            var rendered = await renderer.RenderComponentAsync<Invoice>(parameters);
            return rendered.ToHtmlString();
        });
    }
}