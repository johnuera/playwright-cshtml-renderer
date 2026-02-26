# Invoice / Label Generator

A .NET project that generates invoices and shipping labels using:

- Razor (CSHTML) templates
- RazorLight for server-side rendering
- Playwright for PDF / screenshot generation
- ZXing.Net for barcode generation

---

## 🚀 Features

- Server-side Razor template rendering
- A4 PDF generation via Playwright
- CODE128 / EAN / QR barcode support
- Embedded fonts (Base64)
- Embedded images (Base64)
- Fully server-side (no browser UI required)

---

## 🏗 Tech Stack

- .NET 7 / 8
- RazorLight
- Microsoft.Playwright
- ZXing.Net + SkiaSharp
- CSHTML templates

---

## 📂 Project Structure


```
/Templates
  └── Invoice.cshtml

/assets
  ├── styles/
  │   └── invoice.css
  └── fonts/

 /Services
  ├── BarcodeService.cs
  └── PdfService.cs

/wwwroot
  └── images/

Program.cs
HBarPdfRazor.csproj
README.md
```


---

## ⚙️ Installation

### 1️⃣ Restore dependencies

```bash
dotnet restore
```
### 2️⃣ Install Playwright browsers
```bash 
playwright install
```
Or:
```bash 
pwsh bin/Debug/net8.0/playwright.ps1 install
```

## 🧾 Generate PDF Example
``` c#
var html = await razorEngine.CompileRenderAsync("Templates/Invoice.cshtml", model);

await page.SetContentAsync(html);

await page.PdfAsync(new PagePdfOptions
{
    Format = "A4",
    PrintBackground = true
});

```
## 📦 Barcode Generation (ZXing)

#### Example CODE128:
``` c# 
model.BarcodeBase64 = BarcodeService.GenerateCode128(model.BarcodeValue);
```
### In CSHTML:
``` HTML
<img src="data:image/png;base64,@Model.BarcodeBase64" />
``` 

### 🎨 Styling

Uses A4 print CSS

Embedded fonts via @font-face

Compatible with Playwright Chromium engine

No client-side JavaScript required

🖨 PDF Settings

### Recommended:
``` c#
await page.PdfAsync(new()
{
    Format = "A4",
    Margin = new() { Top = "14mm", Bottom = "14mm" },
    PrintBackground = true
});
```