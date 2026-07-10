# ============================================================
# coverage.ps1 - Backend code coverage (coverlet + ReportGenerator)
# Usage:  .\coverage.ps1          -> run tests + generate HTML report
#         .\coverage.ps1 -Open    -> also open the report in browser
# ============================================================
param([switch]$Open)

$ErrorActionPreference = 'Stop'

# 1) Clean previous results
Remove-Item -Recurse -Force ./TestResults, ./coverage-report -ErrorAction SilentlyContinue

# 2) Run ALL test projects with the XPlat coverage collector
dotnet test --collect:"XPlat Code Coverage" --settings ./coverage.runsettings --results-directory ./TestResults

if ($LASTEXITCODE -ne 0) { Write-Error "Tests failed - coverage report not generated."; exit 1 }

# 3) Merge every cobertura file into one HTML report
reportgenerator `
  -reports:"./TestResults/**/coverage.cobertura.xml" `
  -targetdir:"./coverage-report" `
  -reporttypes:"Html;TextSummary" `
  -assemblyfilters:"+DocAnalytics.*;-DocAnalytics.*.Tests" `
  -classfilters:"-*.Migrations.*"

# 4) Print the summary to the console
Get-Content ./coverage-report/Summary.txt

if ($Open) { Invoke-Item ./coverage-report/index.html }
