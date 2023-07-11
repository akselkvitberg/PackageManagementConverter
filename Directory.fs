module PackageManagementConverter.Directory
open System.IO

let extensions =
    [
        ".csproj"
        ".vbproj"
        ".fsproj"
        ".props"
        ".targets"
    ] |> Set.ofList

let IsProjectFile (filePath:string) =
    Path.GetExtension filePath |> extensions.Contains

let ConvertDirectory options path =
    Directory.GetFiles (path, "*.*", SearchOption.AllDirectories)
    |> Array.filter IsProjectFile
    |> Project.ConvertProjects options