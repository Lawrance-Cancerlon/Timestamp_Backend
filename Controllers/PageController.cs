using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services;

namespace Timestamp_Backend.Controllers;

[Route("views")]
[ApiController]
public class PageController(DatabaseService database, IAuthenticationService authentication) : Controller
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;

    [HttpGet("{id:length(24)}")]
    public async Task<ViewResult> Index(string id)
    {
        Page page = await _database.GetCollection<Page>("pages").Find(x => x.Id == id).FirstOrDefaultAsync();
        if(page == null) return View("Views/Pages/invalid.cshtml");
        ViewBag.images = await _database.GetCollection<Image>("images").Find(x => page.ImageIds.Contains(x.Id)).ToListAsync();
        ViewBag.video = await _database.GetCollection<Video>("videos").Find(x => x.Id == page.VideoId).FirstOrDefaultAsync();
        return View("Views/Pages/index.cshtml");
    }
}
