module PackageManagementConverter.Log
open System.IO
open System.Xml.Linq
open SpectreCoff.Output

let PackageVersions packages =
    C $"Found the following packages:" |> toConsole
    let groups =
        packages
        |> Array.sortBy fst
        |> Array.groupBy fst
        |> Array.map (fun (key, i) -> key, i |> Array.map snd)
        |> Array.iter (fun (key, versions) ->
            V key |> toConsole
            versions
            |> Array.distinct
            |> Array.sort
            |> Array.map (fun v -> P (v.ToString())) |> Array.toList |> BI |> toConsole
            )
    packages

let Projects projects =
    let list = projects |> String.concat "\n- "
    C $"Converting the following projects:\n- {list}" |> toConsole 

let currentPath = Directory.GetCurrentDirectory()

let Project project =
    printMarkedUp $"Convering project [green]{project}[/]:"
    
let Solution solutionFile =
    printMarkedUp $"Converting solution [green]{solutionFile}[/]:"

let PackageFile text =
    printMarkedUp $"Package file: [green]\n{text}[/]"
    
let RemoveAttribute (attribute:XAttribute) =
    let prev = (attribute.Parent.ToString())
    let attributeStr = attribute.ToString()
    let newLine = prev.Replace (attributeStr, $"[red bold][[-{attributeStr}]][/]")
    printMarkedUp newLine
    attribute
    
let RemoveChild (element:XElement) =
    let prev = element.Parent.ToString()
    let elementStr = element.ToString()
    let newLine = prev.Replace (elementStr, $"[red bold][[-{elementStr}]][/]")
    printMarkedUp newLine
    element