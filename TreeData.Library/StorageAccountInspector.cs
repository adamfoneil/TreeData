using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeData.Library.Abstract;
using TreeData.Library.Models;

namespace TreeData.Library
{
    public class StorageAccountInspector : FileSystemInspector
    {
        protected override char PathSeparator => '/';        

        private readonly BlobContainerClient _client;        

        private List<BlobHierarchyItem> _hierarchyItems;
        
        public StorageAccountInspector(string connectionString, string container)
        {
            _client = new BlobContainerClient(connectionString, container);            
        }

        protected override async Task OnInspectPathAsync(SqlConnection cn, string path, int folderId)
        {
            var pages = _client.GetBlobsByHierarchyAsync(delimiter: PathSeparator.ToString(), prefix: path);

            _hierarchyItems = new List<BlobHierarchyItem>();

            await foreach (var page in pages.AsPages())
            {
                _hierarchyItems.AddRange(page.Values);
            }
        }

        protected override async Task<IEnumerable<string>> GetDirectoriesAsync(string path)
        {
            // ToArray makes this a bit easier to inspect while debugging
            var result = _hierarchyItems.Where(item => item.IsPrefix).Select(item => item.Prefix).ToArray();
            return await Task.FromResult(result);
        }

        protected override async Task<IEnumerable<File>> GetFilesAsync(string path)
        {
            var result = _hierarchyItems.Where(item => item.IsBlob).Select(item => new File()
            {                
                Name = item.Blob.Name.Split(new char[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries).Last(),
                Length = item.Blob.Properties.ContentLength ?? 0,
                DateCreated = item.Blob.Properties.CreatedOn.Value.Date,
                DateModified = item.Blob.Properties.LastModified.Value.Date
            }).ToArray();

            return await Task.FromResult(result);
        }
      
        protected override string GetRootPathName(string path) => null;

        protected override string GetRootFolderName() => $"{_client.AccountName}:{_client.Name}";
    }
}
