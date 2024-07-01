using Transdit.Core.Constants;

namespace Transdit.API.Configuration.BackgroundTasks
{
    public class TemporaryFoldersCleanUp : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TemporaryFoldersCleanUp> _logger;
        private readonly AppConfiguration _config;

        public TemporaryFoldersCleanUp(IWebHostEnvironment env, ILogger<TemporaryFoldersCleanUp> logger, AppConfiguration config)
        {
            _env = env;
            _logger = logger;
            _config = config;
        }
        /// <summary>
        /// Deve limpar as pastas temporárias
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_config.CleanUpTaskSpan);
            while(!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation($"Running temporary files clean up task at {DateTime.Now.ToShortTimeString()}");
                    var pathWWWroot = _env.WebRootPath;
                    DeleteFolderContent($"{pathWWWroot}/Temp");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        private bool DeleteFolderContent(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                DeleteInnerFolder(directoryInfo);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private void DeleteInnerFolder(DirectoryInfo directory)
        {
            if (directory == null) return;

            DeleteFolderFiles(directory);

            foreach (var dir in directory.EnumerateDirectories())
            {
                DeleteInnerFolder(dir);
            }
        }
        private void DeleteFolderFiles(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.EnumerateFiles())
            {
                try
                {
                    file.Delete();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Não foi possível apagar o arquivo {file.Name} devido à {ex.Message}", ex.StackTrace);
                }
            }
        }
    }
}
