using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iKfz.Backend.DTOs;
using iKfz.Backend.Models;
using iKfz.Backend.Repositories;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementRepository _announcementRepo;

    public AnnouncementsController(IAnnouncementRepository announcementRepo)
    {
        _announcementRepo = announcementRepo;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnnouncementDto>>> GetActive()
    {
        var announcements = await _announcementRepo.GetActiveAsync();
        return Ok(announcements.Select(a => new AnnouncementDto
        {
            Id = a.Id,
            Titel = a.Titel,
            Nachricht = a.Nachricht,
            Typ = a.Typ,
        }).ToList());
    }
}
