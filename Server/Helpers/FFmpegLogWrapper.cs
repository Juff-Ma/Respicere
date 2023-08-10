// Ignore Spelling: Respicere Fmpeg

using FFMediaToolkit;

namespace Respicere.Server.Helpers
{
    public class FFmpegLogWrapper
    {
        private readonly ILogger<FFmpegLogWrapper> _logger;

        public FFmpegLogWrapper(ILogger<FFmpegLogWrapper> logger)
        {
            _logger = logger;

            FFmpegLoader.LogCallback += FFmpegLogger;
        }

        private void FFmpegLogger(string message) =>
            _logger.LogDebug("{message}", message);
    }
}
