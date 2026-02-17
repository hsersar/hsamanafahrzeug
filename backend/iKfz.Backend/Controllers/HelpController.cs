using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iKfz.Backend.DTOs;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HelpController : ControllerBase
{
    /// <summary>
    /// Get help articles (demo content).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HelpArticleDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<HelpArticleDto>> GetHelpArticles()
    {
        var now = DateTime.UtcNow;
        var articles = new List<HelpArticleDto>
        {
            new HelpArticleDto
            {
                Id = 1,
                Category = "Antrag",
                Title = "Wie reiche ich einen Antrag ein?",
                Summary = "Schritt-für-Schritt-Anleitung für die Erfassung.",
                Content = "Nutzen Sie die Seite Neue Zulassung und füllen Sie alle Pflichtfelder aus. "
                          + "Nach dem Absenden erscheint der Antrag unter Meine Anträge.",
                UpdatedAt = now.AddDays(-3)
            },
            new HelpArticleDto
            {
                Id = 2,
                Category = "Status",
                Title = "Wo sehe ich den Bearbeitungsstand?",
                Summary = "Aktueller Status in der Subbar und im Bereich Meine Anträge.",
                Content = "Der Status wird alle 30 Sekunden aktualisiert. "
                          + "Detaillierte Informationen finden Sie in Meine Anträge.",
                UpdatedAt = now.AddDays(-7)
            },
            new HelpArticleDto
            {
                Id = 3,
                Category = "Kontakt",
                Title = "Wie erreiche ich die zuständige Stelle?",
                Summary = "Kontaktkanal für Rückfragen und Dokumente.",
                Content = "Bitte nutzen Sie die hinterlegte E-Mail-Adresse oder das Kontaktformular. "
                          + "Antworten erfolgen in der Regel innerhalb von 2 Werktagen.",
                UpdatedAt = now.AddDays(-12)
            }
        };

        return Ok(articles);
    }
}