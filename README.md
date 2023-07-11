# PackageManagementConverter

Converts a solution to use Centralised Package Management

The conversion process to centralised package management involves the following steps:
- Extraction of package references from all csproj files (and other project types) and consolidation into the centralised Directory.Packages.props file.
- Removal of package version information from the package reference entries in the project files.

## Usage

To run the tool, install it using the following command:
`dotnet tool install PackageManagementConverter --global`
(This tool is still in development, so it is not yet published. The above command does not yet work)

After installation, run the tool using the following command
`dotnet convert-to-cpm <Path/to/Solution.sln>`
or 
`dotnet convert-to-cpm <Path/to/projects>`

## How does it work

If a solution file is specified, the tool will only convert the projects specified in the solution file. This is to keep old projects that have not yet been deleted from cluttering the centralized package management.

If a directory is specified, the tool will scan specified directory and its subdirectories for all .csproj, .fsproj, .vbproj, .target and .prop files and convert those.

If more than one version is used in the project, the highest version is selected, unless the parameter `--lowest-version` is specified, which will select the lowest version.

The tool will remove all version information from the project files, and create a Directory.Packages.props file in the same directory as the solution file specified or in the directory specified.

## Command line options
`<Solution file>` or `directory`: Select which solution or directory to work on. If not specified, the tool looks for a solution in the current directory. If not, it will use the current directory as the root and convert all projects in the subdirectories.
`--dry-run`: Do not modify files on disk
`--t`: Force versions on transitive dependencies (CentralPackageTransitivePinningEnabled=true)
`--lowest-version`: When selecting which version to use, select the lowest version found. Defaults to highest version.
`-crlf` `-lf` `-cr`: Select line ending to use. Defaults to system standard.
