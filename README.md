# PDF Generator - Playwright + dotnet

A .NET project that generates invoices and shipping labels using:

- Blazor (.razor) templates
- Blazor for server-side rendering
- Playwright for PDF / PDF generation
- ZXing.Net for barcode generation

---

## 🚀 Features

- Server-side Blazor template rendering
- A4 PDF generation via Playwright
- CODE128 / EAN / QR barcode support
- Embedded fonts (Base64)
- Embedded images (Base64)
- Fully server-side (no browser UI required)

---

## 🏗 Tech Stack

- .NET 8
- Blazor
- Microsoft.Playwright
- ZXing.Net + SkiaSharp
- Blazor templates

---

## 📂 Project Structure


```
/Components
  └── Invoice.razor

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
PDFGenerator.csproj
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
 var rendered = await renderer.RenderComponentAsync<Invoice>(parameters);

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
### In .Razor:
``` HTML
<img src="data:image/png;base64,@Model.BarcodeBase64" alt="Barcode" />
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