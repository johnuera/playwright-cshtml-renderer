using System.Threading.Tasks;
using RazorLight;

namespace PDFGenerator;

public sealed class RazorReportRenderer
{
    private readonly RazorLightEngine _engine;

    public RazorReportRenderer(string templatesRoot)
    {
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(templatesRoot)
            .UseMemoryCachingProvider()
            .Build();
    }

    public Task<string> RenderAsync<TModel>(string templateFileName, TModel model)
        => _engine.CompileRenderAsync(templateFileName, model);
}