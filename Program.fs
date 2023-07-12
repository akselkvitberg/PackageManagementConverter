open System
open System.IO
open System.Text
open Microsoft.FSharp.Core
open PackageManagementConverter
open PackageManagementConverter.Solution
open PackageManagementConverter.Directory
open PackageManagementConverter.Config
open SpectreCoff

let ShowHelp () =
    let str = """Package Management Converter
[green]<Solution file>[/] or [green]directory[/]: Select which solution or directory to work on. If not specified, the tool looks for a solution in the current directory. If not, it will use the current directory as the root and convert all projects in the subdirectories.
[green]--dry-run[/]: Do not modify files on disk
[green]--t[/]: Force versions on transitive dependencies (CentralPackageTransitivePinningEnabled=true)
[green]--lowest-version[/]: When selecting which version to use, select the lowest version found. Defaults to highest version.
[green]-crlf -lf -cr[/]: Select line ending to use. Defaults to system standard.
[green]--utf-8 --utf-8bom --utf-16[/]: Select encoding. Utf8 without bom is default.
"""
    printMarkedUp str

let args = Environment.GetCommandLineArgs()[1..] |> Array.toList

if (args |> List.contains "-h") then ShowHelp()
else
    let baseOptions =
        {
            TransitivePinningEnabled = false
            BaseFolder = Path.GetFullPath "."
            DryRun = false
            UseMinVersion = false
            WorkingPath = NotSpecified
            Linefeed = Environment.NewLine
            Encoding = UTF8Encoding(false)
        }

    let rec ParseCommandLine args (options:Config) =
        match args with
        | [] -> options
        | "-t"::xs -> ParseCommandLine xs { options with TransitivePinningEnabled = true}
        | "--dry-run"::xs -> ParseCommandLine xs { options with DryRun = true }
        | "--low"::xs -> ParseCommandLine xs {options with UseMinVersion = true}
        | "--lowest"::xs -> ParseCommandLine xs {options with UseMinVersion = true}
        | "--lowest-version"::xs -> ParseCommandLine xs {options with UseMinVersion = true}
        | "-crlf"::xs -> ParseCommandLine xs {options with Linefeed = "\r\n" }
        | "-lf"::xs -> ParseCommandLine xs {options with Linefeed = "\n" }
        | "-cr"::xs -> ParseCommandLine xs {options with Linefeed = "\r" }
        | "--utf-8"::xs -> ParseCommandLine xs {options with Encoding = UTF8Encoding(false) }
        | "--utf-8bom"::xs -> ParseCommandLine xs {options with Encoding = UTF8Encoding(true) }
        | "--utf-16"::xs -> ParseCommandLine xs {options with Encoding = Encoding.Unicode }
        | path::xs when File.Exists path && Path.GetExtension path = ".sln" -> ParseCommandLine xs {options with WorkingPath = SolutionFile path; BaseFolder = Path.GetDirectoryName path }
        | path::xs when Directory.Exists path -> ParseCommandLine xs {options with WorkingPath = Directory path; BaseFolder = path } 
        | l -> failwith $"Unsupported argument {l}"

    let options = ParseCommandLine args baseOptions

    let AssumeWorkingPath options =
        let executingFolder = Directory.GetCurrentDirectory()
        let sln = System.IO.Directory.GetFiles (executingFolder, "*.sln") |> Array.tryHead
        match sln with
        | Some path -> ConvertSolution {options with WorkingPath = SolutionFile path; BaseFolder = executingFolder } path
        | None ->
            let answer = ask $"Do you want to convert all projects in all subfolders of {executingFolder}? (y/n)" 
            match answer with
            | "y" -> ConvertDirectory { options with WorkingPath = Directory executingFolder; BaseFolder = executingFolder} executingFolder
            | _ -> failwith "Must specify directory or solution file"

    match options.WorkingPath with
        | SolutionFile file -> ConvertSolution options file
        | Directory path -> ConvertDirectory options path
        | NotSpecified -> AssumeWorkingPath options
