namespace Respicere.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Data;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using Interfaces;
using Configuration = Shared.Configuration;
using Server.Helpers;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly DataDbContext _db;
    private readonly IWritableOptions<Configuration> _config;

    public ImagesController(DataDbContext db, IWritableOptions<Configuration> config)
    {
        _db = db;
        _config = config;
    }

    [HttpGet]
    public IEnumerable<SecurityImage> Get()
    {
        return _db.Images.OrderByDescending(x => x.Created);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SecurityImage>> Get(Guid id)
    {
        var image = await _db.Images.FindAsync(id);

        if (image is null)
        {
            return NotFound();
        }

        return Ok(image);
    }

    [HttpGet("{id}/image.jpg")]
    public IActionResult GetImage(Guid id)
    {
        var imagesPath = _config.Value.GetPhotoPath();
        if (!Path.Exists(imagesPath))
        {
            return NotFound();
        }
        var imagePath = Path.Combine(imagesPath, $"{id}.jpg");

        if (!Path.Exists(imagePath))
        {
            return NotFound();
        }

        var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

        return File(stream, "image/jpeg");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var image = await _db.Images.FindAsync(id);

        bool deleted = _db.Images.DeleteImage(image, _config.Value.GetPhotoPath());

        await _db.SaveChangesAsync();

        if(!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, SecurityImage image)
    {
        if (id != image.Id)
        {
            return BadRequest();
        }

        _db.Entry(image).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!ImageExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    private bool ImageExists(Guid id)
    {
        return _db.Images.Any(e => e.Id == id);
    }
}
