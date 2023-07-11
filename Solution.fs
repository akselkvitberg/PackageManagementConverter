module PackageManagementConverter.Solution

open System.IO
open System.Text.RegularExpressions
open System

let GetProjects lines =
    let GuidPattern(text:string)= $@"\{{(?<{Regex.Escape(text)}>[0-9a-fA-F]{{8}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{4}}-[0-9a-fA-F]{{12}})\}}"
    let TextPattern(name:string)= $@"""(?<{Regex.Escape(name)}>[^""]*)"""
    let projectRegex = $"""^Project\("%s{GuidPattern("typeId")}"\)\s*=\s*%s{TextPattern("name")},\s*%s{TextPattern("path")},\s*"%s{GuidPattern("projectId")}"$"""
    let regex = Regex(projectRegex)

    lines
    |> Array.map regex.Match
    |> Array.filter (fun x -> x.Success)
    |> Array.map (fun x -> x.Groups["path"].Value, x.Groups["typeId"].Value)
    |> Array.filter (fun (_,typeId) -> not (typeId = "2150E333-8FDC-42A3-9474-1A3956D46DE8")) // Solution folder
    |> Array.map fst

let GetProjectsFromSolutionFile solutionFile =
    let solutionContent = File.ReadAllLines solutionFile
    let lines = 
        solutionContent
        |> Array.filter (fun x -> not (String.IsNullOrEmpty(x)))
        |> Array.map (fun x -> x.Trim())
    let projects = GetProjects lines
    
    let solutionFolder = Path.GetDirectoryName solutionFile
    
    projects
    |> Array.map (fun path -> Path.Combine (solutionFolder, path))
    
let ConvertSolution options solutionFile =
    GetProjectsFromSolutionFile solutionFile
    |> Project.ConvertProjects options