namespace Respicere.Server.Jobs;

using Quartz;
using Interfaces;
using System.Threading.Tasks;
using Configuration = Shared.Configuration;
using Data;
using Helpers;
using Microsoft.EntityFrameworkCore;

public class DeletePhotosJob : IJob
{
    private readonly ILogger<DeletePhotosJob> _logger;
    private readonly IWritableOptions<Configuration> _options;
    private readonly DataDbContext _db;

    public DeletePhotosJob(ILogger<DeletePhotosJob> logger, IWritableOptions<Configuration> options, DataDbContext db)
    {
        _logger = logger;
        _options = options;
        _db = db;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var time = DateTime.UtcNow;
        var deleteAfter = _options.Value.GetPhotoDeleteOlderThan();

        await foreach (var image in 
            _db.Images.Where(x => 
                ((time - x.Created) > deleteAfter) && 
                !x.DoNotDelete)
            .AsAsyncEnumerable())
        {
            _db.Images.DeleteImage(image, _options.Value.GetPhotoPath());
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("deleted old images according to config");
    }
}
