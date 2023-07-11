﻿open System
open System.IO
open Microsoft.FSharp.Core
open PackageManagementConverter
open PackageManagementConverter.Solution
open PackageManagementConverter.Directory
open PackageManagementConverter.Config

let args = Environment.GetCommandLineArgs()[1..] |> Array.toList

let baseOptions =
    {
        TransitivePinningEnabled = true
        BaseFolder = Path.GetFullPath "."
        DryRun = false
        UseMinVersion = false
        WorkingPath = NotSpecified
        Linefeed = Environment.NewLine
    }
    
let rec ParseCommandLine args (options:Config) =
    match args with
    | [] -> options
    | "-t"::xs -> ParseCommandLine xs { options with TransitivePinningEnabled = true}
    | "-d"::xs -> ParseCommandLine xs { options with DryRun = true }
    | "--dry-run"::xs -> ParseCommandLine xs { options with DryRun = true }
    | "--min"::xs -> ParseCommandLine xs {options with UseMinVersion = true}
    | "--min-version"::xs -> ParseCommandLine xs {options with UseMinVersion = true}
    | "-crlf"::xs -> ParseCommandLine xs {options with Linefeed = "\r\n" }
    | "-lf"::xs -> ParseCommandLine xs {options with Linefeed = "\n" }
    | "-cr"::xs -> ParseCommandLine xs {options with Linefeed = "\r" }
    | path::xs when File.Exists path && Path.GetExtension path = ".sln" -> ParseCommandLine xs {options with WorkingPath = SolutionFile path; BaseFolder = Path.GetDirectoryName path }
    | path::xs when Directory.Exists path -> ParseCommandLine xs {options with WorkingPath = Directory path; BaseFolder = path } 
    | l -> failwith $"Unsupported argument {l}"

let options = ParseCommandLine args baseOptions

let AssumeWorkingPath options =
    let executingFolder = Directory.GetCurrentDirectory()
    let sln = Directory.GetFiles "*.sln" |> Array.tryHead
    match sln with
    | Some path -> ConvertSolution {options with WorkingPath = SolutionFile path; BaseFolder = executingFolder } path
    | None ->
        Console.WriteLine "Do you want to convert all projects in all subfolders? (y/n)"
        match Console.ReadLine() with
        | "y" -> ConvertDirectory { options with WorkingPath = Directory executingFolder; BaseFolder = executingFolder} executingFolder
        | _ -> failwith "Must specify directory or solution file"

let result = 
    match options.WorkingPath with
    | SolutionFile file -> ConvertSolution options file
    | Directory path -> ConvertDirectory options path
    | NotSpecified -> AssumeWorkingPath options
