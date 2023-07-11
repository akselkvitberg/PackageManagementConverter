namespace PackageManagementConverter

open System.Text.RegularExpressions
open System.Xml.Linq
open System
open NuGet.Versioning

module Array =
    let groupByMap selector map (array:('a*'b) array) =
        array
        |> Array.groupBy selector
        |> Array.map (fun (key, items) -> key, items |> map)

module Xml =
    let GetDescendants (xml: XDocument) name =
        let rootNamespace =
            Option.ofObj xml.Root
            |> Option.map (fun root -> root.GetDefaultNamespace())
            |> Option.defaultValue XNamespace.None

        xml.Descendants(rootNamespace + name)


    let GetAttribute name (element: XElement) =
        element.Attributes()
        |> Seq.tryFind (fun x -> String.Equals(x.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
        |> Option.map (fun x -> x.Value)

    let GetVersionChild (element: XElement) =
        element.Descendants()
        |> Seq.tryFind (fun e -> String.Equals(e.Name.LocalName, "Version", StringComparison.OrdinalIgnoreCase))
        |> Option.map (fun x -> x.Value)

    
    let ToString (linefeed:string) (xml:XDocument)  =
        Regex.Replace(xml.ToString(), "(\r\n)|[\r\n]", linefeed)


module NuGetVersion = 
    let Parse (version: string) =
            let mutable nugetVersion: NuGetVersion = null
            let success = NuGetVersion.TryParse(version, &nugetVersion)
            if success then Some nugetVersion else None
