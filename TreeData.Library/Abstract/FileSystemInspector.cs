using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TreeData.Library.Models;

namespace TreeData.Library.Abstract
{
    public abstract class FileSystemInspector
    {
        protected abstract char PathSeparator { get; }

        public async Task InspectAsync(SqlConnection cn, string path, IProgress<Progress> progress)
        {
            // find or create the Folder.Id for the starting path
            var rootFolderId = await cn.MergeAsync(new Folder()
            {
                ParentId = 0, // we are assuming our root folders always have a parent Id = 0
                Name = path
            });

            // drill into the entire folder structure with a recursive method
            await InspectInnerAsync(cn, rootFolderId, path, progress);
        }

        private async Task InspectInnerAsync(SqlConnection cn, int parentFolderId, string path, IProgress<Progress> progress)
        {
            progress.Report(new Progress()
            {
                Path = path                
            });

            var directories = await GetDirectoriesAsync(path);

            foreach (var dir in directories)
            {
                // find or create the folder Id for this subdirectory
                var folderId = await cn.MergeAsync(new Folder()
                {
                    ParentId = parentFolderId,
                    Name = dir.Split(PathSeparator).Last()
                });

                var files = await GetFilesAsync(folderId, dir);

                // add all these files to the database if they don't exist
                foreach (var file in files) await cn.MergeAsync(file);

                // repeat this method on subfolders of this path
                await InspectInnerAsync(cn, folderId, dir, progress);
            }
        }

        /// <summary>
        /// gets the immediate sub directories of a given path (don't drill into sub folders)
        /// </summary>
        protected abstract Task<IEnumerable<string>> GetDirectoriesAsync(string path);

        /// <summary>
        /// gets the file info in the specified path (don't drill into sub folders)
        /// </summary>
        protected abstract Task<IEnumerable<Models.File>> GetFilesAsync(int parentFolderId, string path);        

        public class Progress
        {
            public string Path { get; set; }
            public int TotalDirectories { get; set; }
            public int TotalFiles { get; set; }
        }
    }
}
