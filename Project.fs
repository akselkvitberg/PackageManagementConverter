module PackageManagementConverter.Project

open System.IO
open System.Xml.Linq
open System
open PackageManagementConverter
open PackageManagementConverter.Config



let private GetPackage (package:XElement) =
    let packageName =
        Xml.GetAttribute "Include" package
        |> Option.orElseWith (fun () -> Xml.GetAttribute "Update" package)
    
    let packageVersion =
        Xml.GetAttribute "Version" package
        |> Option.orElseWith (fun () -> Xml.GetVersionChild package)
        |> Option.bind NuGetVersion.Parse
        
    Option.map2 (fun a b -> a,b) packageName packageVersion


let GetProjectPackages (projectPath:string) =
    let xml = XDocument.Load projectPath
    let packageReferences = Xml.GetDescendants xml "PackageReference"
    
    packageReferences
    |> Seq.map GetPackage
    |> Seq.choose id
    |> Seq.toArray

let RemoveVersionFromProject options (projectPath: string) =
    let xml = XDocument.Load (projectPath, LoadOptions.PreserveWhitespace)
    let packageReferences = Xml.GetDescendants xml "PackageReference"
    Log.Project projectPath
    
    let RemoveVersionFromElement (element:XElement) =
        element.Attributes()
        |> Seq.tryFind (fun a -> String.Equals(a.Name.LocalName, "Version", StringComparison.OrdinalIgnoreCase))
        |> Option.map Log.RemoveAttribute
        |> Option.iter (fun a -> a.Remove())
        
        element.Descendants()
        |> Seq.tryFind (fun e -> String.Equals(e.Name.LocalName, "Version", StringComparison.OrdinalIgnoreCase))
        |> Option.map Log.RemoveChild
        |> Option.iter (fun e -> e.Remove())
    
    packageReferences
    |> Seq.iter RemoveVersionFromElement
    
    if not options.DryRun then
        File.WriteAllText (projectPath, xml |> Xml.ToString options.Linefeed, options.Encoding)

let ConvertProjects options projects =
    Log.Projects projects
    let versionSelector = if options.UseMinVersion then Array.min else Array.max

    projects
    |> Array.map GetProjectPackages
    |> Array.collect id
    |> Array.groupByMap fst (Array.map snd >> versionSelector)
    |> Log.PackageVersions
    |> CentralPackageManagement.WriteCentralPackageManagementFile options
    
    projects
    |> Array.iter (RemoveVersionFromProject options)