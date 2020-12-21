using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeData.Library.Models;

namespace TreeData.Library.Abstract
{
    public abstract class FileSystemInspector
    {
        protected abstract char PathSeparator { get; }

        protected abstract string GetRootFolderName();

        protected virtual string GetRootPathName(string path) => path;

        public async Task InspectAsync(SqlConnection cn, IProgress<Progress> progress) => await InspectAsync(cn, GetRootFolderName(), progress);
        
        public async Task InspectAsync(SqlConnection cn, string path, IProgress<Progress> progress)
        {
            // find or create the Folder.Id for the starting path
            var rootFolderId = await cn.MergeAsync(new Folder()
            {
                ParentId = 0, // we are assuming our root folders always have a parent Id = 0
                Name = path
            });

            // drill into the entire folder structure with a recursive method            
            await InspectInnerAsync(cn, rootFolderId, GetRootPathName(path), progress, 1, 0);
        }

        private async Task InspectInnerAsync(SqlConnection cn, int parentFolderId, string path, IProgress<Progress> progress, int dirCount, int fileCount)
        {
            progress.Report(new Progress()
            {
                Path = path,
                CurrentDepth = dirCount,
                CurrentFileCount = fileCount
            });

            await OnInspectPathAsync(cn, path, parentFolderId);

            var directories = await GetDirectoriesAsync(path);

            var files = await GetFilesAsync(path);

            // add all these files to the database if they don't exist
            foreach (var file in files)
            {
                file.FolderId = parentFolderId;
                await cn.MergeAsync(file);
            }            

            // examine all the directories for *their* files
            foreach (var dir in directories)
            {
                // find or create the folder Id for this subdirectory
                var folderId = await cn.MergeAsync(new Folder()
                {
                    ParentId = parentFolderId,
                    Name = dir.Split(new char[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries).Last()
                });
                
                await InspectInnerAsync(cn, folderId, dir, progress, dirCount + 1, fileCount + files.Count());
            }
        }

        /// <summary>
        /// gets the immediate sub directories of a given path (don't drill into sub folders)
        /// </summary>
        protected abstract Task<IEnumerable<string>> GetDirectoriesAsync(string path);

        /// <summary>
        /// gets the file info in the specified path (don't drill into sub folders)
        /// </summary>
        protected abstract Task<IEnumerable<File>> GetFilesAsync(string path);        

        /// <summary>
        /// perform any tasks prior to inspecting directories and files (necessary for blob storage)
        /// </summary>
        protected virtual async Task OnInspectPathAsync(SqlConnection cn, string path, int folderId)
        {
            await Task.CompletedTask;
        }

        public class Progress
        {
            public string Path { get; set; }
            public int CurrentDepth { get; set; }
            public int CurrentFileCount { get; set; }
        }
    }
}
