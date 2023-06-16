namespace Respicere.Server.Helpers;

using Microsoft.EntityFrameworkCore;
using Respicere.Server.Data;
using Shared.Models;

public static class DataDbImageSetExtensions
{
    public static bool DeleteImage(this DbSet<SecurityImage> set, SecurityImage? image, string basePath)
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

        set.Remove(image);

        return result;
    }
}
