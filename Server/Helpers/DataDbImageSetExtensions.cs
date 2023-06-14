namespace Respicere.Server.Helpers;

using Microsoft.EntityFrameworkCore;
using Respicere.Server.Data;
using Shared.Models;

public static class DataDbImageSetExtensions
{
    public static async Task<bool> DeleteImageAsync(this DataDbContext db, SecurityImage? image, string basePath)
    {
        bool result = true;
        if (image is null)
        {
            return false;
        }

        string? imagePath = null;

        if (Path.Exists(basePath))
        {
            imagePath = Path.Combine(basePath, $"{image.Id}.jpg");
        }
        else
        {
            result = false;
        }

        if (Path.Exists(imagePath))
        {
            File.Delete(imagePath);
        }
        else
        {
            result = false;
        }

        db.Images.Remove(image);
        await db.SaveChangesAsync();

        return result;
    }
}
