using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ModelSync.Models;
using SqlServer.LocalDb;
using System;
using System.Threading.Tasks;
using TreeData.Library;
using TreeData.Library.Abstract;
using TreeData.Library.Models;

namespace TreeData.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = CreateLogger();

            using (var cn = LocalDb.GetConnection("TreeData"))
            {
                await CreateModelsAsync(cn);

                var progress = new Progress<FileSystemInspector.Progress>((progress) => ShowProgress(logger, progress));

                foreach (var path in args)
                {
                    await new LocalFileInspector().InspectAsync(cn, path, progress);
                }
            }
        }

        private static void ShowProgress(ILogger logger, FileSystemInspector.Progress progress)
        {
            logger.LogInformation($"{progress.Path} : {progress.CurrentDepth:n0} directories, {progress.CurrentFileCount:n0} files");
        }

        /// <summary>
        /// help from https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0#non-host-console-app
        /// </summary>        
        private static ILogger CreateLogger()
        {
            using var factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            return factory.CreateLogger<Program>();
        }

        private static async Task CreateModelsAsync(SqlConnection cn)
        {
            Console.WriteLine("Creating database tables if they don't exist...");

            await DataModel.CreateTablesAsync(new Type[]
            {
                typeof(Folder),
                typeof(File)
            }, cn);
        }
    }
}
