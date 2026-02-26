using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace HBarPdfRazor;

public class Program
{
    static async Task Main(string[] args)
    {
        var outputPdf = args.Length > 0 ? args[0] : "report.pdf";
        var culture = args.Length > 1 ? args[1] : "de-DE"; // e.g. de-DE, nl-NL, en-US

        // ---- Templates root ----
        var templatesRoot = Path.Combine(AppContext.BaseDirectory, "Templates");
        if (!Directory.Exists(templatesRoot))
            templatesRoot = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

        if (!Directory.Exists(templatesRoot))
        {
            throw new DirectoryNotFoundException(
                "Templates folder not found. Looked in:\n" +
                Path.Combine(AppContext.BaseDirectory, "Templates") + "\n" +
                Path.Combine(Directory.GetCurrentDirectory(), "Templates"));
        }

        // Template file name (relative to templatesRoot)
        var templateName = "Invoice.cshtml";
        var fullTemplatePath = Path.Combine(templatesRoot, templateName);
        if (!File.Exists(fullTemplatePath))
        {
            throw new FileNotFoundException(
                $"Template not found: {fullTemplatePath}\n" +
                "Make sure Templates/Invoice.cshtml exists and is copied to output.");
        }
        var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "fonts", "Limelight-Regular.ttf");
        byte[] fontBytes = await File.ReadAllBytesAsync(fontPath);
        string fontBase64 = $"data:font/ttf;base64,{Convert.ToBase64String(fontBytes)}";
        Console.WriteLine(fontBase64);

        // ---- Logo ----
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
                new Product
                {
                    Description = "Sample Product Description - Size M - Removable Insole",
                    Condition = "New",
                    IsbnOrEan = "0000000000000",
                    Quantity = 1
                },
                new Product
                {
                    Description = "Sample Product Description - Size M - Removable Insole",
                    Condition = "New",
                    IsbnOrEan = "0000000000000",
                    Quantity = 1
                },
                new Product
                {
                    Description = "Sample Product Description - Size M - Removable Insole",
                    Condition = "New",
                    IsbnOrEan = "0000000000000",
                    Quantity = 1
                },
                new Product
                {
                    Description = "Sample Product Description - Size M - Removable Insole",
                    Condition = "New",
                    IsbnOrEan = "0000000000000",
                    Quantity = 1
                },
            },
        };

        // ---- Render Razor -> HTML ----
        var renderer = new RazorReportRenderer(templatesRoot);

        
        var html = await renderer.RenderAsync(templateName, model); 
        // ---- Playwright -> PDF ----
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();

        await page.SetContentAsync(html, new() { WaitUntil = WaitUntilState.NetworkIdle });

        await page.EvaluateAsync("document.fonts.ready");

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

        // ✅ Base64 output
        await File.WriteAllBytesAsync(outputPdf, pdfBytes);

        Console.WriteLine($"PDF written to: {Path.GetFullPath(outputPdf)}");
        Console.WriteLine($"Locale used: {culture}");
    }

}