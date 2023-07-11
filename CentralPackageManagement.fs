module PackageManagementConverter.CentralPackageManagement

open System.IO
open PackageManagementConverter.Config

let CreateCentralPackageManagementFile (options:Config) packages =
    let packageRefs =
        packages
        |> Array.sortBy fst
        |> Array.map (fun (package, version) -> $"""    <PackageVersion Include="{package}" Version="{version}"/>""")
    [|
        "<Project>"
        "  <PropertyGroup>"
        "    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>"
        $"    <CentralPackageTransitivePinningEnabled>{options.TransitivePinningEnabled}</CentralPackageTransitivePinningEnabled>"
        "  </PropertyGroup>"
        "  <ItemGroup>"
        yield! packageRefs
        "  </ItemGroup>"
        "</Project>"
    |]
    
let WriteCentralPackageManagementFile options packages =
    let lines = CreateCentralPackageManagementFile options packages
    let text = String.concat options.Linefeed lines
    
    Log.PackageFile text
    if not options.DryRun then
        let packageConfigPath = Path.Combine (options.BaseFolder, "Directory.Packages.props")
        File.WriteAllText (packageConfigPath, text, options.Encoding)
