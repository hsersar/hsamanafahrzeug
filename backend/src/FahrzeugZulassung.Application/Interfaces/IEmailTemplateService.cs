namespace FahrzeugZulassung.Application.Interfaces;

public interface IEmailTemplateService
{
    Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model);
}
