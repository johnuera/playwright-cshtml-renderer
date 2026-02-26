namespace PDFGenerator;

public sealed class InvoiceTemplateModel
{   
         public string Culture { get; set; } ="";
         public JsonLocale? Locale { get; set; } 

     public string LogoBase64 { get; set; } ="";
     public string BarcodeBase64 { get; set; } ="";
     public string FontBase64 { get; set; } ="";

     public string SellerAddress { get; init; } = "";
    public string PlatformName { get; init; } = "Wortmann Fashion Retail Onlineshops";
    public string PlatformAddress { get; init; } = "";
    public string OrderDate { get; init; } = "";
    public string OrderNumber { get; init; } = "";
    public string CustomerNumber { get; init; } = "";
    public string SellerReference { get; init; } = "";

    public List<Product> Lines { get; init; } = new();

    public string PaymentNote { get; init; } = "";
    public string ReturnIntro { get; init; } = "";
    public List<string> ReturnSteps { get; init; } = new();
    public string ReturnOutro { get; init; } = "";
    public string ReviewNote { get; init; } = "";
    public string SupportNote { get; init; } = "";
 
 }

public sealed class Product
{
    public string Description { get; init; } = "";
    public string Condition { get; init; } = "";
    public string IsbnOrEan { get; init; } = "";
    public int Quantity { get; init; }
}

