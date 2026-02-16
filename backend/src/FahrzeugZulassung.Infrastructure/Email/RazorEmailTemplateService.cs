using FahrzeugZulassung.Application.Interfaces;
using RazorLight;

namespace FahrzeugZulassung.Infrastructure.Email;

public class RazorEmailTemplateService : IEmailTemplateService
{
    private readonly RazorLightEngine _engine;
    private readonly string _templatePath;

    public RazorEmailTemplateService()
    {
        _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
        
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(_templatePath)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model)
    {
        try
        {
            var templatePath = $"{templateName}.cshtml";
            return await _engine.CompileRenderAsync(templatePath, model);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Rendern des Templates '{templateName}': {ex.Message}", ex);
        }
    }
}
