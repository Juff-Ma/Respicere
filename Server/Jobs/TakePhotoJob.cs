namespace Respicere.Server.Jobs;

using Quartz;
using Interfaces;
using Configuration = Shared.Configuration;
using System.Threading.Tasks;
using Data;
using Shared.Models;
using Helpers;

public class TakePhotoJob : IJob
{
    private readonly ICam _cam;
    private readonly ILogger<TakePhotoJob> _logger;
    private readonly IWritableOptions<Configuration> _options;
    private readonly DataDbContext _db;

    public TakePhotoJob(ICam cam, ILogger<TakePhotoJob> logger, IWritableOptions<Configuration> options, DataDbContext db)
    {
        _cam = cam;
        _logger = logger;
        _options = options;
        _db = db;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var image = new SecurityImage()
        {
            Created = DateTime.UtcNow,
            DoNotDelete = false
        };

        await _db.Images.AddAsync(image);
        await _db.SaveChangesAsync();

        if (!Path.Exists(_options.Value.GetPhotoPath()))
        {
            Directory.CreateDirectory(_options.Value.GetPhotoPath());
        }
        string imagePath = Path.Combine(_options.Value.GetPhotoPath(), $"{image.Id}.jpg");

        using var stream = new FileStream(imagePath, FileMode.Create);

        await _cam.TakeSnapshotAsync(stream);

        _logger.LogInformation("Took new Photo with id: {id}", image.Id);
    }
}
