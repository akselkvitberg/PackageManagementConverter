module PackageManagementConverter.Config

open System.Text

type WorkingPath =
    | NotSpecified
    | SolutionFile of string
    | Directory of string

type Config =
    {
        TransitivePinningEnabled: bool
        BaseFolder: string
        DryRun: bool
        UseMinVersion: bool
        WorkingPath: WorkingPath
        Linefeed: string
        Encoding: Encoding
    }