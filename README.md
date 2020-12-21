This is the source for a PowerPoint I did on working with file system data in SQL Server. This aimed at those familiar with C# and SQL Server, but looking for ideas on how to work with hiearchical file system data.

- I'm using a parent/child table structure (not `hierarchyid`). More info about hiearchical data in SQL Server [here](https://docs.microsoft.com/en-us/sql/relational-databases/hierarchical-data-sql-server?view=sql-server-ver15)
- This is an intro to recursive CTEs (common table expressions)
- This is also an example of using abstract classes in C# to capture file system data from two sources: local files and Azure blob storage

## Core Structure
See my [Models](https://github.com/adamfoneil/TreeData/tree/master/TreeData.Library/Models) folder for the two important tables in use `Folder` and `File`.

## Data capture with `FileSystemInspector` abstract class
I wanted to capture file system data both from local files and blob storage, but reuse the same data access code for both sources. I did this using the [FileSystemInspector](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs) abstract class that defines the data access once. Abstract methods [GetDirectoriesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs#L73) and [GetFilesAsync](https://github.com/adamfoneil/TreeData/blob/master/TreeData.Library/Abstract/FileSystemInspector.cs#L78)
