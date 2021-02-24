This is the source for a [PowerPoint](https://1drv.ms/v/s!AvguHRnyJtWMme5LJlU1FGxjmQCcqA?e=ZBg5o0) I did on working with file system data in SQL Server. This aimed at those familiar with C# and SQL Server and who are looking for ideas on how to work with hiearchical file system data.

- I'm using a parent/child table structure (not `hierarchyid`). More info about hiearchical data in SQL Server [here](https://docs.microsoft.com/en-us/sql/relational-databases/hierarchical-data-sql-server?view=sql-server-ver15)
- This is an intro to recursive CTEs (common table expressions)
- This is also an example of using abstract classes in C# to capture file system data from two sources: local files and Azure blob storage
- Repo has a [Blazor Server app](https://github.com/adamfoneil/TreeData/tree/master/FolderViewer.Blazor) for demonstrating tree view features.

## Core Structure
See my [Models](https://github.com/adamfoneil/TreeData/tree/master/TreeData.Library/Models) folder for the two important tables in use `Folder` and `File`.

These two classes serve both as a basis for data capture as well as SQL querying.

See my diagram at [dbdiagram.io](https://dbdiagram.io/d/5f5ec5c810a0a51c74d4da02). This is a really nifty tool!

![img](https://adamosoftware.blob.core.windows.net/images/file-system.png)

## Query techniques
My query approach revolves around several table functions found in the [Sql](https://github.com/adamfoneil/TreeData/tree/master/Sql) directory:

- [FnFolderRollup.sql](https://github.com/adamfoneil/TreeData/blob/master/Sql/FnFolderRollup.sql) provides the sizes of folders directly below a given `folderId` along with the percent of total size of each folder. This is executed in the Blazor app [here](https://github.com/adamfoneil/TreeData/blob/master/FolderViewer.Blazor/Pages/Index.razor#L145) and integrated into the markup [here](https://github.com/adamfoneil/TreeData/blob/master/FolderViewer.Blazor/Pages/Index.razor#L41).
- [FnFolderTree.sql](https://github.com/adamfoneil/TreeData/blob/master/Sql/FnFolderTree.sql) is a low-level function that returns the entire folder structure below a given `folderId`. This is used within several other functions.
- [FnIgnoreFoldersAll.sql](https://github.com/adamfoneil/TreeData/blob/master/Sql/FnIgnoreFoldersAll.sql) returns all the folder structures that start with an explicitly ignore folder (such as `.git` and `packages` -- see the [IgnoreFolderName](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Models/IgnoreFolderName.cs) model class). This function is not used because it doesn't return unique folder Ids.
- [FnIgnoreFoldersUnique.sql](https://github.com/adamfoneil/TreeData/blob/master/Sql/FnIgnoreFoldersUnique.sql) is similar to `IgnoreFoldersAll` but is assured to return unique folder Ids.
- [FnPath.sql](https://github.com/adamfoneil/TreeData/blob/master/Sql/FnPath.sql) is used to get the containing folder names of a given `folderId`. It's recursive query that goes up instead of down. This is used to build the [breadcrumb navigation](https://github.com/adamfoneil/TreeData/blob/master/FolderViewer.Blazor/Pages/Index.razor#L16), and is retrieved as part of [Folder.GetRelatedAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Models/Folder.cs#L37) so that it's automatically fetched any time an individual folder is [retrieved](https://github.com/adamfoneil/TreeData/blob/master/FolderViewer.Blazor/Pages/Index.razor#L144).
- [Walkthrough.sql](https://github.com/adamfoneil/TreeData/blob/master/Sql/Walkthrough.sql) was just a scratchpad of sorts I used early on.


## Data capture with `FileSystemInspector` abstract class
I wanted to capture file system data both from local files and blob storage, but reuse the same data access code for both sources.

I did this using the [FileSystemInspector](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs) abstract class that defines the data access once. Abstract methods [GetDirectoriesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs#L73) and [GetFilesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs#L78) are what subclasses must implement.

- [LocalFileInspector](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/LocalFileInspector.cs) is for interacting with a local file system. You can see its [GetDirectorieAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/LocalFileInspector.cs#L14) method along with its [GetFilesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/LocalFileInspector.cs#L33) method.

- [StorageAccountInspector](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs) is for interacting with Azure blob storage. You can see its [GetDirectoriesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs#L38) method along with its [GetFilesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs#L45) method. Note that this class uses a special override [BeforeInspectPathAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs#L26) that captures some info from blob storage before files and directories can be returned.

