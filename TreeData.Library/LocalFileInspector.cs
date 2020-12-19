using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TreeData.Library.Abstract;

namespace TreeData.Library
{
    public class LocalFileInspector : FileSystemInspector
    {
        protected override char PathSeparator => '\\';

        protected override async Task<IEnumerable<string>> GetDirectoriesAsync(string path)
        {
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(path);

            try
            {
                return await Task.FromResult(Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly));
            }
            catch 
            {
                // most likely a permission error
                return Enumerable.Empty<string>();
            }
        }

        protected override async Task<IEnumerable<Models.File>> GetFilesAsync(string path)
        {
            try
            {
                var files = Directory.GetFiles(path);

                var result = files.Select(fullPath =>
                {
                    var fileInfo = new FileInfo(fullPath);
                    return new Models.File()
                    {                        
                        Name = fileInfo.Name,
                        Length = fileInfo.Length,
                        DateCreated = fileInfo.CreationTime,
                        DateModified = fileInfo.LastWriteTime
                    };
                });

                return await Task.FromResult(result);
            }
            catch 
            {
                // most likely permission denied
                return Enumerable.Empty<Models.File>();
            }
        }

        // Local file inspector doesn't need special root folder naming because you always pass the root path explicitly.
        // Inspecting blobs is different because the root folder name is derived from the account and container info
        protected override string GetRootFolderName() => throw new NotImplementedException();

    }
}
