This is the source for a PowerPoint I did on working with file system data in SQL Server. This aimed at those familiar with C# and SQL Server and who are looking for ideas on how to work with hiearchical file system data.

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
The [Walkthrough.sql](https://github.com/adamfoneil/TreeData/blob/master/Sql/Walkthrough.sql) script shows the steps I went through building some folder queries. Notably, see

- [FnFolderTree](https://github.com/adamfoneil/TreeData/blob/master/Sql/Walkthrough.sql#L19) function
- Folder [rollup example](https://github.com/adamfoneil/TreeData/blob/master/Sql/Walkthrough.sql#L115)

## Data capture with `FileSystemInspector` abstract class
I wanted to capture file system data both from local files and blob storage, but reuse the same data access code for both sources.

I did this using the [FileSystemInspector](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs) abstract class that defines the data access once. Abstract methods [GetDirectoriesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs#L73) and [GetFilesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs#L78) are what subclasses must implement.

- [LocalFileInspector](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/LocalFileInspector.cs) is for interacting with a local file system. You can see its [GetDirectorieAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/LocalFileInspector.cs#L14) method along with its [GetFilesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/LocalFileInspector.cs#L33) method.

- [StorageAccountInspector](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs) is for interacting with Azure blob storage. You can see its [GetDirectoriesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs#L38) method along with its [GetFilesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs#L45) method. Note that this class uses a special override [BeforeInspectPathAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/StorageAccountInspector.cs#L26) that captures some info from blob storage before files and directories can be returned.

