module PackageManagementConverter.Log
open System.IO
open System.Xml.Linq
open SpectreCoff.Output

let PackageVersions packages =
    let list = packages |> Array.map (fun (package, version) -> P $"{package}: {version}") |> Array.toList// |> String.concat "\n"
    C $"Found the following packages:" |> toConsole
    BI list |> toConsole
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