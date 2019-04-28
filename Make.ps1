#!/usr/bin/env pwsh

[CmdletBinding()]

Param
(
  [Parameter(Mandatory=$True,Position=1)]
  [string]$command
)

$workdir = $(git rev-parse --show-toplevel);

Try
{
  If ($command -imatch "^(show-?)?times?$")
  {
    Write-Output "Starting show times";
    Set-Location .\Timereporter.Api;
    dotnet run time;
    Write-Output "Finished show times";
  }
  Else
  {
    Write-Output "Starting run (default)";
    Set-Location .\Timereporter.Api;
    dotnet run;  # Should run as a Job or something to not block
    Set-Location .\..\Timereporter\bin\Debug\Timereporter.exe;
    Write-Output "Finisihed run (default)";
  }
}
Catch
{
  Set-Location $workdir;
}

