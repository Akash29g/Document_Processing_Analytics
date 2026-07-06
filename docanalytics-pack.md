This file is a merged representation of a subset of the codebase, containing files not matching ignore patterns, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of a subset of the repository's contents that is considered the most important context.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching these patterns are excluded: **/bin/**, **/obj/**, **/node_modules/**, **/dist/**
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
.gitattributes
.github/CODEOWNERS
.gitignore
Design_Tasks_1-3_updated.pdf
docanalytics-web/.editorconfig
docanalytics-web/.gitignore
docanalytics-web/.prettierrc
docanalytics-web/.vscode/extensions.json
docanalytics-web/.vscode/launch.json
docanalytics-web/.vscode/mcp.json
docanalytics-web/.vscode/tasks.json
docanalytics-web/angular.json
docanalytics-web/package.json
docanalytics-web/proxy.conf.json
docanalytics-web/public/AVEVA_Logo_color_RGB.png
docanalytics-web/public/favicon.ico
docanalytics-web/src/app/app.config.ts
docanalytics-web/src/app/app.css
docanalytics-web/src/app/app.html
docanalytics-web/src/app/app.routes.ts
docanalytics-web/src/app/app.spec.ts
docanalytics-web/src/app/app.ts
docanalytics-web/src/app/core/guards/auth.guard.ts
docanalytics-web/src/app/core/guards/site-access.guard.ts
docanalytics-web/src/app/core/interceptors/auth-site.interceptor.ts
docanalytics-web/src/app/core/interceptors/error.interceptor.ts
docanalytics-web/src/app/core/models/api-response.model.ts
docanalytics-web/src/app/core/models/auth.model.ts
docanalytics-web/src/app/core/models/dashboard.model.ts
docanalytics-web/src/app/core/services/auth.service.ts
docanalytics-web/src/app/core/services/refresh-timer.service.ts
docanalytics-web/src/app/core/services/site-context.service.ts
docanalytics-web/src/app/core/services/theme.service.ts
docanalytics-web/src/app/core/services/toast.service.ts
docanalytics-web/src/app/features/activity-log/activity-log.component.css
docanalytics-web/src/app/features/activity-log/activity-log.component.html
docanalytics-web/src/app/features/activity-log/activity-log.component.ts
docanalytics-web/src/app/features/activity-log/activity-log.models.ts
docanalytics-web/src/app/features/activity-log/activity-log.service.ts
docanalytics-web/src/app/features/auth/login.component.css
docanalytics-web/src/app/features/auth/login.component.html
docanalytics-web/src/app/features/auth/login.component.ts
docanalytics-web/src/app/features/batches/batch-detail/batch-detail.component.css
docanalytics-web/src/app/features/batches/batch-detail/batch-detail.component.html
docanalytics-web/src/app/features/batches/batch-detail/batch-detail.component.ts
docanalytics-web/src/app/features/batches/batch-list.component.css
docanalytics-web/src/app/features/batches/batch-list.component.html
docanalytics-web/src/app/features/batches/batch-list.component.ts
docanalytics-web/src/app/features/batches/batch.models.ts
docanalytics-web/src/app/features/batches/batch.service.ts
docanalytics-web/src/app/features/dashboard/dashboard.component.css
docanalytics-web/src/app/features/dashboard/dashboard.component.html
docanalytics-web/src/app/features/dashboard/dashboard.component.ts
docanalytics-web/src/app/features/dashboard/dashboard.models.ts
docanalytics-web/src/app/features/dashboard/dashboard.service.ts
docanalytics-web/src/app/features/dashboard/status-distribution-chart/status-distribution-chart.component.css
docanalytics-web/src/app/features/dashboard/status-distribution-chart/status-distribution-chart.component.html
docanalytics-web/src/app/features/dashboard/status-distribution-chart/status-distribution-chart.component.ts
docanalytics-web/src/app/features/dashboard/throughput-chart/throughput-chart.component.css
docanalytics-web/src/app/features/dashboard/throughput-chart/throughput-chart.component.html
docanalytics-web/src/app/features/dashboard/throughput-chart/throughput-chart.component.ts
docanalytics-web/src/app/features/errors/error.service.ts
docanalytics-web/src/app/features/errors/errors.component.css
docanalytics-web/src/app/features/errors/errors.component.html
docanalytics-web/src/app/features/errors/errors.component.ts
docanalytics-web/src/app/features/errors/errors.models.ts
docanalytics-web/src/app/features/files/file-details.component.css
docanalytics-web/src/app/features/files/file-details.component.html
docanalytics-web/src/app/features/files/file-details.component.ts
docanalytics-web/src/app/features/files/file-details.models.ts
docanalytics-web/src/app/features/files/file-details.service.ts
docanalytics-web/src/app/layout/shell/shell.component.css
docanalytics-web/src/app/layout/shell/shell.component.html
docanalytics-web/src/app/layout/shell/shell.component.ts
docanalytics-web/src/app/shared/components/app-button/app-button.component.css
docanalytics-web/src/app/shared/components/app-button/app-button.component.html
docanalytics-web/src/app/shared/components/app-button/app-button.component.ts
docanalytics-web/src/app/shared/components/chart-card/chart-card.component.css
docanalytics-web/src/app/shared/components/chart-card/chart-card.component.html
docanalytics-web/src/app/shared/components/chart-card/chart-card.component.ts
docanalytics-web/src/app/shared/components/data-table/data-table.component.css
docanalytics-web/src/app/shared/components/data-table/data-table.component.html
docanalytics-web/src/app/shared/components/data-table/data-table.component.ts
docanalytics-web/src/app/shared/components/filter-bar/filter-bar.component.css
docanalytics-web/src/app/shared/components/filter-bar/filter-bar.component.html
docanalytics-web/src/app/shared/components/filter-bar/filter-bar.component.ts
docanalytics-web/src/app/shared/components/refresh-timer/refresh-timer.component.css
docanalytics-web/src/app/shared/components/refresh-timer/refresh-timer.component.html
docanalytics-web/src/app/shared/components/refresh-timer/refresh-timer.component.ts
docanalytics-web/src/app/shared/components/site-selector/site-selector.component.css
docanalytics-web/src/app/shared/components/site-selector/site-selector.component.html
docanalytics-web/src/app/shared/components/site-selector/site-selector.component.ts
docanalytics-web/src/app/shared/components/stat-card/stat-card.component.css
docanalytics-web/src/app/shared/components/stat-card/stat-card.component.html
docanalytics-web/src/app/shared/components/stat-card/stat-card.component.ts
docanalytics-web/src/app/shared/components/status-badge/status-badge.component.css
docanalytics-web/src/app/shared/components/status-badge/status-badge.component.html
docanalytics-web/src/app/shared/components/status-badge/status-badge.component.ts
docanalytics-web/src/environments/environment.ts
docanalytics-web/src/index.html
docanalytics-web/src/main.ts
docanalytics-web/src/styles.css
docanalytics-web/tsconfig.app.json
docanalytics-web/tsconfig.json
docanalytics-web/tsconfig.spec.json
DocAnalytics.Api/appsettings.Development.json
DocAnalytics.Api/appsettings.json
DocAnalytics.Api/Auth/JwtSettings.cs
DocAnalytics.Api/Common/ApiResponse.cs
DocAnalytics.Api/Common/BaseController.cs
DocAnalytics.Api/Common/CurrentUser.cs
DocAnalytics.Api/Controllers/ActivityLogController.cs
DocAnalytics.Api/Controllers/AuthController.cs
DocAnalytics.Api/Controllers/BatchesController.cs
DocAnalytics.Api/Controllers/DashboardAnalyticsController.cs
DocAnalytics.Api/Controllers/DashboardController.cs
DocAnalytics.Api/Controllers/ErrorAnalyticsController.cs
DocAnalytics.Api/Controllers/ErrorsController.cs
DocAnalytics.Api/Controllers/FilesController.cs
DocAnalytics.Api/Controllers/HealthController.cs
DocAnalytics.Api/Controllers/InvoiceLineItemsController.cs
DocAnalytics.Api/Controllers/SitesController.cs
DocAnalytics.Api/DocAnalytics.Api.csproj
DocAnalytics.Api/DocAnalytics.Api.http
DocAnalytics.Api/Extensions/ApiServiceExtensions.cs
DocAnalytics.Api/Extensions/ValidationExtensions.cs
DocAnalytics.Api/Middleware/ExceptionHandlingMiddleware.cs
DocAnalytics.Api/Middleware/TenantSiteMiddleware.cs
DocAnalytics.Api/Program.cs
DocAnalytics.Api/Properties/launchSettings.json
DocAnalytics.Api/Swagger/SiteHeaderOperationFilter.cs
DocAnalytics.Data/AppDbContext.cs
DocAnalytics.Data/DependencyInjection.cs
DocAnalytics.Data/DocAnalytics.Data.csproj
DocAnalytics.Data/Migrations/20260619102024_InitialCreate.cs
DocAnalytics.Data/Migrations/20260619102024_InitialCreate.Designer.cs
DocAnalytics.Data/Migrations/AppDbContextModelSnapshot.cs
DocAnalytics.Data/Seeding/DbSeeder.cs
DocAnalytics.Domain/Common/ICurrentUser.cs
DocAnalytics.Domain/Common/ITenantScoped.cs
DocAnalytics.Domain/DocAnalytics.Domain.csproj
DocAnalytics.Domain/Entities/ActivityLog.cs
DocAnalytics.Domain/Entities/DocumentType.cs
DocAnalytics.Domain/Entities/ErrorCatalog.cs
DocAnalytics.Domain/Entities/FileRecord.cs
DocAnalytics.Domain/Entities/FileStepHistory.cs
DocAnalytics.Domain/Entities/InvoiceLineItem.cs
DocAnalytics.Domain/Entities/ItemCategory.cs
DocAnalytics.Domain/Entities/Site.cs
DocAnalytics.Domain/Entities/Tenant.cs
DocAnalytics.Domain/Entities/Transaction.cs
DocAnalytics.Domain/Entities/User.cs
DocAnalytics.Domain/Entities/UserSiteAccess.cs
DocAnalytics.Service/ActivityLog/ActivityLogDtos.cs
DocAnalytics.Service/ActivityLog/ActivityLogFeatureExtensions.cs
DocAnalytics.Service/ActivityLog/ActivityLogService.cs
DocAnalytics.Service/ActivityLog/IActivityLogService.cs
DocAnalytics.Service/Analytics/AnalyticsDtos.cs
DocAnalytics.Service/Analytics/AnalyticsFeatureExtensions.cs
DocAnalytics.Service/Analytics/AnalyticsService.cs
DocAnalytics.Service/Analytics/IAnalyticsService.cs
DocAnalytics.Service/Auth/AuthDtos.cs
DocAnalytics.Service/Auth/AuthFeatureExtensions.cs
DocAnalytics.Service/Auth/AuthService.cs
DocAnalytics.Service/Auth/IAuthService.cs
DocAnalytics.Service/Auth/IJwtTokenService.cs
DocAnalytics.Service/Auth/JwtTokenService.cs
DocAnalytics.Service/Batches/BatchDtos.cs
DocAnalytics.Service/Batches/BatchFeatureExtensions.cs
DocAnalytics.Service/Batches/BatchService.cs
DocAnalytics.Service/Batches/IBatchService.cs
DocAnalytics.Service/Common/DateTimeExtensions.cs
DocAnalytics.Service/Common/OneOfAttribute.cs
DocAnalytics.Service/Common/PagedResult.cs
DocAnalytics.Service/Dashboard/DashboardDtos.cs
DocAnalytics.Service/Dashboard/DashboardFeatureExtensions.cs
DocAnalytics.Service/Dashboard/DashboardService.cs
DocAnalytics.Service/Dashboard/IDashboardService.cs
DocAnalytics.Service/DependencyInjection.cs
DocAnalytics.Service/DocAnalytics.Service.csproj
DocAnalytics.Service/Errors/ErrorCsvWriter.cs
DocAnalytics.Service/Errors/ErrorDtos.cs
DocAnalytics.Service/Errors/ErrorFeatureExtensions.cs
DocAnalytics.Service/Errors/ErrorService.cs
DocAnalytics.Service/Errors/IErrorService.cs
DocAnalytics.Service/Files/FileDetailsDtos.cs
DocAnalytics.Service/Files/FileDetailsFeatureExtensions.cs
DocAnalytics.Service/Files/FileDetailsService.cs
DocAnalytics.Service/Files/IFileDetailsService.cs
DocAnalytics.Service/Health/HealthFeatureExtensions.cs
DocAnalytics.Service/Health/HealthService.cs
DocAnalytics.Service/Health/IHealthService.cs
DocAnalytics.Service/Invoices/IInvoiceService.cs
DocAnalytics.Service/Invoices/InvoiceDtos.cs
DocAnalytics.Service/Invoices/InvoiceFeatureExtensions.cs
DocAnalytics.Service/Invoices/InvoiceService.cs
DocAnalytics.slnx
InternProject-Requirements.md
README.md
```

# Files

## File: .github/CODEOWNERS
````
# Both teammates own everything → cross-review enforced
*   @akash29g   @g9shubh
````

## File: .gitignore
````
## Ignore Visual Studio temporary files, build results, and
## files generated by popular Visual Studio add-ons.
##
## Get latest from `dotnet new gitignore`

# dotenv files
.env

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# User-specific files (MonoDevelop/Xamarin Studio)
*.userprefs

# Mono auto generated files
mono_crash.*

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio 2015/2017 cache/options directory
.vs/
# Uncomment if you have tasks that create the project's static files in wwwroot
#wwwroot/

# Visual Studio 2017 auto generated files
Generated\ Files/

# MSTest test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# NUnit
*.VisualState.xml
TestResult.xml
nunit-*.xml

# Build Results of an ATL Project
[Dd]ebugPS/
[Rr]eleasePS/
dlldata.c

# Benchmark Results
BenchmarkDotNet.Artifacts/

# .NET
project.lock.json
project.fragment.lock.json
artifacts/

# Tye
.tye/

# ASP.NET Scaffolding
ScaffoldingReadMe.txt

# StyleCop
StyleCopReport.xml

# Files built by Visual Studio
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
# but not Directory.Build.rsp, as it configures directory-level build defaults
!Directory.Build.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.tlog
*.vspscc
*.vssscc
.builds
*.pidb
*.svclog
*.scc

# Chutzpah Test files
_Chutzpah*

# Visual C++ cache files
ipch/
*.aps
*.ncb
*.opendb
*.opensdf
*.sdf
*.cachefile
*.VC.db
*.VC.VC.opendb

# Visual Studio profiler
*.psess
*.vsp
*.vspx
*.sap

# Visual Studio Trace Files
*.e2e

# TFS 2012 Local Workspace
$tf/

# Guidance Automation Toolkit
*.gpState

# ReSharper is a .NET coding add-in
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user

# TeamCity is a build add-in
_TeamCity*

# DotCover is a Code Coverage Tool
*.dotCover

# AxoCover is a Code Coverage Tool
.axoCover/*
!.axoCover/settings.json

# Coverlet is a free, cross platform Code Coverage Tool
coverage*.json
coverage*.xml
coverage*.info

# Visual Studio code coverage results
*.coverage
*.coveragexml

# NCrunch
_NCrunch_*
.*crunch*.local.xml
nCrunchTemp_*

# MightyMoose
*.mm.*
AutoTest.Net/

# Web workbench (sass)
.sass-cache/

# Installshield output folder
[Ee]xpress/

# DocProject is a documentation generator add-in
DocProject/buildhelp/
DocProject/Help/*.HxT
DocProject/Help/*.HxC
DocProject/Help/*.hhc
DocProject/Help/*.hhk
DocProject/Help/*.hhp
DocProject/Help/Html2
DocProject/Help/html

# Click-Once directory
publish/

# Publish Web Output
*.[Pp]ublish.xml
*.azurePubxml
# Note: Comment the next line if you want to checkin your web deploy settings,
# but database connection strings (with potential passwords) will be unencrypted
*.pubxml
*.publishproj

# Microsoft Azure Web App publish settings. Comment the next line if you want to
# checkin your Azure Web App publish settings, but sensitive information contained
# in these scripts will be unencrypted
PublishScripts/

# NuGet Packages
*.nupkg
# NuGet Symbol Packages
*.snupkg
# The packages folder can be ignored because of Package Restore
**/[Pp]ackages/*
# except build/, which is used as an MSBuild target.
!**/[Pp]ackages/build/
# Uncomment if necessary however generally it will be regenerated when needed
#!**/[Pp]ackages/repositories.config
# NuGet v3's project.json files produces more ignorable files
*.nuget.props
*.nuget.targets

# Microsoft Azure Build Output
csx/
*.build.csdef

# Microsoft Azure Emulator
ecf/
rcf/

# Windows Store app package directories and files
AppPackages/
BundleArtifacts/
Package.StoreAssociation.xml
_pkginfo.txt
*.appx
*.appxbundle
*.appxupload

# Visual Studio cache files
# files ending in .cache can be ignored
*.[Cc]ache
# but keep track of directories ending in .cache
!?*.[Cc]ache/

# Others
ClientBin/
~$*
*~
*.dbmdl
*.dbproj.schemaview
*.jfm
*.pfx
*.publishsettings
orleans.codegen.cs

# Including strong name files can present a security risk
# (https://github.com/github/gitignore/pull/2483#issue-259490424)
#*.snk

# Since there are multiple workflows, uncomment next line to ignore bower_components
# (https://github.com/github/gitignore/pull/1529#issuecomment-104372622)
#bower_components/

# RIA/Silverlight projects
Generated_Code/

# Backup & report files from converting an old project file
# to a newer Visual Studio version. Backup files are not needed,
# because we have git ;-)
_UpgradeReport_Files/
Backup*/
UpgradeLog*.XML
UpgradeLog*.htm
ServiceFabricBackup/
*.rptproj.bak

# SQL Server files
*.mdf
*.ldf
*.ndf

# Business Intelligence projects
*.rdl.data
*.bim.layout
*.bim_*.settings
*.rptproj.rsuser
*- [Bb]ackup.rdl
*- [Bb]ackup ([0-9]).rdl
*- [Bb]ackup ([0-9][0-9]).rdl

# Microsoft Fakes
FakesAssemblies/

# GhostDoc plugin setting file
*.GhostDoc.xml

# Node.js Tools for Visual Studio
.ntvs_analysis.dat
node_modules/

# Visual Studio 6 build log
*.plg

# Visual Studio 6 workspace options file
*.opt

# Visual Studio 6 auto-generated workspace file (contains which files were open etc.)
*.vbw

# Visual Studio 6 auto-generated project file (contains which files were open etc.)
*.vbp

# Visual Studio 6 workspace and project file (working project files containing files to include in project)
*.dsw
*.dsp

# Visual Studio 6 technical files
*.ncb
*.aps

# Visual Studio LightSwitch build output
**/*.HTMLClient/GeneratedArtifacts
**/*.DesktopClient/GeneratedArtifacts
**/*.DesktopClient/ModelManifest.xml
**/*.Server/GeneratedArtifacts
**/*.Server/ModelManifest.xml
_Pvt_Extensions

# Paket dependency manager
.paket/paket.exe
paket-files/

# FAKE - F# Make
.fake/

# CodeRush personal settings
.cr/personal

# Python Tools for Visual Studio (PTVS)
__pycache__/
*.pyc

# Cake - Uncomment if you are using it
# tools/**
# !tools/packages.config

# Tabs Studio
*.tss

# Telerik's JustMock configuration file
*.jmconfig

# BizTalk build output
*.btp.cs
*.btm.cs
*.odx.cs
*.xsd.cs

# OpenCover UI analysis results
OpenCover/

# Azure Stream Analytics local run output
ASALocalRun/

# MSBuild Binary and Structured Log
*.binlog

# NVidia Nsight GPU debugger configuration file
*.nvuser

# MFractors (Xamarin productivity tool) working folder
.mfractor/

# Local History for Visual Studio
.localhistory/

# Visual Studio History (VSHistory) files
.vshistory/

# BeatPulse healthcheck temp database
healthchecksdb

# Backup folder for Package Reference Convert tool in Visual Studio 2017
MigrationBackup/

# Ionide (cross platform F# VS Code tools) working folder
.ionide/

# Fody - auto-generated XML schema
FodyWeavers.xsd

# VS Code files for those working on multiple tools
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
*.code-workspace

# Local History for Visual Studio Code
.history/

# Windows Installer files from build outputs
*.cab
*.msi
*.msix
*.msm
*.msp

# JetBrains Rider
*.sln.iml
.idea/

##
## Visual studio for Mac
##


# globs
Makefile.in
*.userprefs
*.usertasks
config.make
config.status
aclocal.m4
install-sh
autom4te.cache/
*.tar.gz
tarballs/
test-results/

# content below from: https://github.com/github/gitignore/blob/main/Global/macOS.gitignore
# General
.DS_Store
.AppleDouble
.LSOverride

# Icon must end with two \r
Icon


# Thumbnails
._*

# Files that might appear in the root of a volume
.DocumentRevisions-V100
.fseventsd
.Spotlight-V100
.TemporaryItems
.Trashes
.VolumeIcon.icns
.com.apple.timemachine.donotpresent

# Directories potentially created on remote AFP share
.AppleDB
.AppleDesktop
Network Trash Folder
Temporary Items
.apdisk

# content below from: https://github.com/github/gitignore/blob/main/Global/Windows.gitignore
# Windows thumbnail cache files
Thumbs.db
ehthumbs.db
ehthumbs_vista.db

# Dump file
*.stackdump

# Folder config file
[Dd]esktop.ini

# Recycle Bin used on file shares
$RECYCLE.BIN/

# Windows Installer files
*.cab
*.msi
*.msix
*.msm
*.msp

# Windows shortcuts
*.lnk

# Vim temporary swap files
*.swp
````

## File: DocAnalytics.Api/appsettings.Development.json
````json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
````

## File: DocAnalytics.Api/appsettings.json
````json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=docanalytics;Username=postgres;Password="
  },
  "Jwt": {
    "Issuer": "DocAnalytics",
    "Audience": "DocAnalyticsClient",
    "Key": "SET_VIA_USER_SECRETS",
    "ExpiryMinutes": 120
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
````

## File: DocAnalytics.Api/Auth/JwtSettings.cs
````csharp
namespace DocAnalytics.Api.Auth;

public class JwtSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;
    public int ExpiryMinutes { get; set; } = 120;
}
````

## File: DocAnalytics.Api/Common/ApiResponse.cs
````csharp
namespace DocAnalytics.Api.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public Meta? Meta { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Data = data };
    public static ApiResponse<T> OkList(T data, Meta meta) => new() { Data = data, Meta = meta };
    public static ApiResponse<T> Fail(string code, string msg, object? details = null)
        => new() { Error = new ApiError { Code = code, Message = msg, Details = details } };
}
public class Meta { public int TotalCount { get; set; } public int Page { get; set; } public int PageSize { get; set; } public int TotalPages { get; set; } }
public class ApiError { public string Code { get; set; } = null!; public string Message { get; set; } = null!; public object? Details { get; set; } }
````

## File: DocAnalytics.Api/Common/BaseController.cs
````csharp
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Common;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Envelope<T>(T data) => Ok(ApiResponse<T>.Ok(data));
    protected IActionResult EnvelopeList<T>(T data, Meta meta) => Ok(ApiResponse<T>.OkList(data, meta));
}
````

## File: DocAnalytics.Api/Common/CurrentUser.cs
````csharp
using DocAnalytics.Domain.Common;

namespace DocAnalytics.Api.Common;

public class CurrentUser : ICurrentUser
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid SiteId { get; private set; }
    public string Role { get; private set; } = string.Empty;
    public bool IsAuthenticated { get; private set; }

    public void Set(Guid userId, Guid tenantId, Guid siteId, string role)
    {
        UserId = userId; TenantId = tenantId; SiteId = siteId; Role = role; IsAuthenticated = true;
    }
}
````

## File: DocAnalytics.Api/Controllers/AuthController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;

    public AuthController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    [AllowAnonymous]                      // the only auth endpoint with no token
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(req, ct);
        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail(
                "INVALID_CREDENTIALS", "Email or password is incorrect."));

        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var result = await _auth.GetMeAsync(_currentUser.UserId, ct);
        if (result is null) return Unauthorized();
        return Ok(ApiResponse<MeResponse>.Ok(result));
    }
}
````

## File: DocAnalytics.Api/Controllers/DashboardController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Service.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var data = await _dashboardService.GetSummaryAsync(ct);
        return Ok(ApiResponse<DashboardSummaryResponse>.Ok(data));
    }

    [HttpGet("recent-failures")]
    public async Task<IActionResult> GetRecentFailures(
        [FromQuery] RecentFailuresQuery query, CancellationToken ct)
    {
        var result = await _dashboardService.GetRecentFailuresAsync(query, ct);

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<RecentFailureDto>>.OkList(result.Items, meta));
    }
}
````

## File: DocAnalytics.Api/Controllers/HealthController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Service.Health;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly IHealthService _health;
    public HealthController(IHealthService health) => _health = health;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ok = await _health.IsDatabaseReachableAsync();
        if (!ok) return StatusCode(503, ApiResponse<object>.Fail("DB_UNREACHABLE", "Database is unreachable"));
        return Ok(ApiResponse<object>.Ok(new { status = "healthy", db = "connected" }));
    }
}
````

## File: DocAnalytics.Api/Controllers/InvoiceLineItemsController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Service.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/files")]
public sealed class InvoiceLineItemsController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    public InvoiceLineItemsController(IInvoiceService invoiceService) => _invoiceService = invoiceService;

    // GET /api/v1/files/{id}/line-items
    [HttpGet("{id:guid}/line-items")]
    public async Task<IActionResult> GetLineItems(Guid id, CancellationToken ct)
    {
        var invoice = await _invoiceService.GetInvoiceForFileAsync(id, ct);

        if (invoice is null)
            return NotFound(ApiResponse<InvoiceDetailDto>.Fail(
                "not_found", $"File '{id}' was not found."));

        return Ok(ApiResponse<InvoiceDetailDto>.Ok(invoice));
    }
}
````

## File: DocAnalytics.Api/Controllers/SitesController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using DocAnalytics.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Route("api/v1/sites")]                   // note: /sites, NOT /auth/sites (per DT-2)
[Authorize]
public class SitesController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;

    public SitesController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetSites(CancellationToken ct)
    {
        var sites = await _auth.GetSitesAsync(_currentUser.UserId, ct);
        return Ok(ApiResponse<IReadOnlyList<SiteDto>>.Ok(sites));
    }
}
````

## File: DocAnalytics.Api/DocAnalytics.Api.csproj
````
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UserSecretsId>ca41ef41-383d-431a-abc4-564aba6513a7</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.9" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.9" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.9">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="10.2.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\DocAnalytics.Service\DocAnalytics.Service.csproj" />
  </ItemGroup>

</Project>
````

## File: DocAnalytics.Api/DocAnalytics.Api.http
````
@DocAnalytics.Api_HostAddress = http://localhost:5256

GET {{DocAnalytics.Api_HostAddress}}/weatherforecast/
Accept: application/json

###
````

## File: DocAnalytics.Api/Properties/launchSettings.json
````json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "DocAnalytics.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7001;http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5256",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7042;http://localhost:5256",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
````

## File: DocAnalytics.Api/Swagger/SiteHeaderOperationFilter.cs
````csharp
using Microsoft.OpenApi;                    // ⚠️ 2.x: namespace COLLAPSED (was Microsoft.OpenApi.Models)
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DocAnalytics.Api.Swagger;

// Adds an optional "X-Site-Id" header box to EVERY endpoint in Swagger UI,
// so tenant-scoped requests can pass the site the middleware looks for.
public sealed class SiteHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // ⚠️ 2.x: collections are no longer auto-initialised — guard against null
        operation.Parameters ??= new List<IOpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Site-Id",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Site to scope this request to (tenant isolation). Paste your site_id GUID.",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }  // ⚠️ 2.x: Type is an ENUM, not "string"
        });
    }
}
````

## File: DocAnalytics.Data/AppDbContext.cs
````csharp
using System.Linq.Expressions;
using System.Reflection;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Data;

public class AppDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
        : base(options) => _currentUser = currentUser;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSiteAccess> UserSiteAccess => Set<UserSiteAccess>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FileRecord> Files => Set<FileRecord>();
    public DbSet<FileStepHistory> FileStepHistory => Set<FileStepHistory>();
    public DbSet<ErrorCatalog> ErrorCatalog => Set<ErrorCatalog>();
    public DbSet<ActivityLog> ActivityLog => Set<ActivityLog>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // ---- table names (match DT-1) ----
        b.Entity<FileRecord>().ToTable("files");
        b.Entity<FileStepHistory>().ToTable("file_step_history");
        b.Entity<ErrorCatalog>().ToTable("error_catalog");
        b.Entity<ActivityLog>().ToTable("activity_log");
        b.Entity<InvoiceLineItem>().ToTable("invoice_line_items");
        b.Entity<ItemCategory>().ToTable("item_categories");

        // ---- precision ----
        b.Entity<FileRecord>().Property(f => f.ExtractionConfidence).HasPrecision(4, 3);
        b.Entity<InvoiceLineItem>().Property(i => i.Quantity).HasPrecision(12, 3);
        b.Entity<InvoiceLineItem>().Property(i => i.UnitPrice).HasPrecision(12, 2);
        b.Entity<InvoiceLineItem>().Property(i => i.LineTotal).HasPrecision(12, 2);
        b.Entity<InvoiceLineItem>().Property(i => i.Confidence).HasPrecision(4, 3);

        // ---- uniqueness ----
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<ErrorCatalog>().HasIndex(e => e.ErrorCode).IsUnique();
        b.Entity<DocumentType>().HasIndex(d => d.TypeName).IsUnique();
        b.Entity<ItemCategory>().HasIndex(c => c.CategoryCode).IsUnique();
        b.Entity<UserSiteAccess>().HasIndex(x => new { x.UserId, x.SiteId }).IsUnique();

        // ---- performance indexes (DT-1) ----
        b.Entity<Transaction>().HasIndex(t => new { t.TenantId, t.SiteId, t.LastUpdatedAt });
        b.Entity<Transaction>().HasIndex(t => new { t.TenantId, t.SiteId, t.State });
        b.Entity<FileRecord>().HasIndex(f => f.TransactionId);
        b.Entity<FileRecord>().HasIndex(f => new { f.TenantId, f.SiteId, f.Status, f.LastUpdatedAt });
        b.Entity<FileRecord>().HasIndex(f => new { f.TenantId, f.SiteId, f.DocumentTypeId });
        b.Entity<FileStepHistory>().HasIndex(s => s.FileId);
        b.Entity<FileStepHistory>().HasIndex(s => new { s.StepName, s.Status });
        b.Entity<InvoiceLineItem>().HasIndex(i => i.FileId);
        b.Entity<InvoiceLineItem>().HasIndex(i => new { i.TenantId, i.SiteId, i.ItemCategoryId });
        b.Entity<ActivityLog>().HasIndex(a => new { a.TenantId, a.SiteId, a.CreatedAt });

        // ---- relationships ----
        b.Entity<Site>().HasOne(s => s.Tenant).WithMany(t => t.Sites)
            .HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<User>().HasOne(u => u.Tenant).WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<FileRecord>().HasOne(f => f.Transaction).WithMany(t => t.Files)
            .HasForeignKey(f => f.TransactionId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<FileStepHistory>().HasOne(s => s.File).WithMany(f => f.Steps)
            .HasForeignKey(s => s.FileId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<InvoiceLineItem>().HasOne(i => i.File).WithMany(f => f.LineItems)
            .HasForeignKey(i => i.FileId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<InvoiceLineItem>().HasOne(i => i.ItemCategory).WithMany(c => c.LineItems)
            .HasForeignKey(i => i.ItemCategoryId).OnDelete(DeleteBehavior.SetNull);

        // ---- GLOBAL TENANT/SITE FILTER (every ITenantScoped entity) ----
        foreach (var et in b.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(et.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(BuildTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(et.ClrType);
                b.Entity(et.ClrType).HasQueryFilter((LambdaExpression)method.Invoke(this, null)!);
            }
        }
    }

    // _currentUser is re-evaluated as a parameter at query time (model cached once).
    private LambdaExpression BuildTenantFilter<TEntity>() where TEntity : class, ITenantScoped
    {
        Expression<Func<TEntity, bool>> filter =
            e => e.TenantId == _currentUser.TenantId && e.SiteId == _currentUser.SiteId;
        return filter;
    }
}
````

## File: DocAnalytics.Data/DependencyInjection.cs
````csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(cfg.GetConnectionString("Default"))
               .UseSnakeCaseNamingConvention()
               .ConfigureWarnings(w => w.Ignore(
                   CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)));
        return services;
    }
}
````

## File: DocAnalytics.Data/DocAnalytics.Data.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\DocAnalytics.Domain\DocAnalytics.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.2.0" />
    <PackageReference Include="EFCore.NamingConventions" Version="10.0.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.9" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.9">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="10.0.9" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.2" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
````

## File: DocAnalytics.Data/Migrations/20260619102024_InitialCreate.cs
````csharp
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activity_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    entity_type = table.Column<string>(type: "text", nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_name = table.Column<string>(type: "text", nullable: true),
                    old_state = table.Column<string>(type: "text", nullable: true),
                    new_state = table.Column<string>(type: "text", nullable: true),
                    triggered_by = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_activity_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "document_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_name = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "error_catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    error_code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    remediation_msg = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_error_catalog", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_code = table.Column<string>(type: "text", nullable: false),
                    category_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    source_system = table.Column<string>(type: "text", nullable: false),
                    total_files = table.Column<int>(type: "integer", nullable: false),
                    uploaded_count = table.Column<int>(type: "integer", nullable: false),
                    processing_count = table.Column<int>(type: "integer", nullable: false),
                    failed_count = table.Column<int>(type: "integer", nullable: false),
                    completed_count = table.Column<int>(type: "integer", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sites", x => x.id);
                    table.ForeignKey(
                        name: "fk_sites_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    file_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    current_step = table.Column<string>(type: "text", nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: true),
                    extraction_status = table.Column<string>(type: "text", nullable: true),
                    extraction_confidence = table.Column<decimal>(type: "numeric(4,3)", precision: 4, scale: 3, nullable: true),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_files_document_types_document_type_id",
                        column: x => x.document_type_id,
                        principalTable: "document_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_files_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_site_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_site_access", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_site_access_sites_site_id",
                        column: x => x.site_id,
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_site_access_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_step_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    step_name = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_code = table.Column<string>(type: "text", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_step_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_file_step_history_document_types_document_type_id",
                        column: x => x.document_type_id,
                        principalTable: "document_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_file_step_history_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice_line_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    line_total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    confidence = table.Column<decimal>(type: "numeric(4,3)", precision: 4, scale: 3, nullable: true),
                    is_valid = table.Column<bool>(type: "boolean", nullable: false),
                    extracted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_line_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_line_items_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invoice_line_items_item_categories_item_category_id",
                        column: x => x.item_category_id,
                        principalTable: "item_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_activity_log_tenant_id_site_id_created_at",
                table: "activity_log",
                columns: new[] { "tenant_id", "site_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_document_types_type_name",
                table: "document_types",
                column: "type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_error_catalog_error_code",
                table: "error_catalog",
                column: "error_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_step_history_document_type_id",
                table: "file_step_history",
                column: "document_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_step_history_file_id",
                table: "file_step_history",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_step_history_step_name_status",
                table: "file_step_history",
                columns: new[] { "step_name", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_files_document_type_id",
                table: "files",
                column: "document_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_tenant_id_site_id_document_type_id",
                table: "files",
                columns: new[] { "tenant_id", "site_id", "document_type_id" });

            migrationBuilder.CreateIndex(
                name: "ix_files_tenant_id_site_id_status_last_updated_at",
                table: "files",
                columns: new[] { "tenant_id", "site_id", "status", "last_updated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_files_transaction_id",
                table: "files",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_line_items_file_id",
                table: "invoice_line_items",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_line_items_item_category_id",
                table: "invoice_line_items",
                column: "item_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_line_items_tenant_id_site_id_item_category_id",
                table: "invoice_line_items",
                columns: new[] { "tenant_id", "site_id", "item_category_id" });

            migrationBuilder.CreateIndex(
                name: "ix_item_categories_category_code",
                table: "item_categories",
                column: "category_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sites_tenant_id",
                table: "sites",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id_site_id_last_updated_at",
                table: "transactions",
                columns: new[] { "tenant_id", "site_id", "last_updated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id_site_id_state",
                table: "transactions",
                columns: new[] { "tenant_id", "site_id", "state" });

            migrationBuilder.CreateIndex(
                name: "ix_user_site_access_site_id",
                table: "user_site_access",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_site_access_user_id_site_id",
                table: "user_site_access",
                columns: new[] { "user_id", "site_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id",
                table: "users",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_log");

            migrationBuilder.DropTable(
                name: "error_catalog");

            migrationBuilder.DropTable(
                name: "file_step_history");

            migrationBuilder.DropTable(
                name: "invoice_line_items");

            migrationBuilder.DropTable(
                name: "user_site_access");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "item_categories");

            migrationBuilder.DropTable(
                name: "sites");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "document_types");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
````

## File: DocAnalytics.Data/Migrations/20260619102024_InitialCreate.Designer.cs
````csharp
// <auto-generated />
using System;
using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DocAnalytics.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260619102024_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ActivityLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid")
                        .HasColumnName("entity_id");

                    b.Property<string>("EntityName")
                        .HasColumnType("text")
                        .HasColumnName("entity_name");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entity_type");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("event_type");

                    b.Property<string>("NewState")
                        .HasColumnType("text")
                        .HasColumnName("new_state");

                    b.Property<string>("OldState")
                        .HasColumnType("text")
                        .HasColumnName("old_state");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<string>("TriggeredBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("triggered_by");

                    b.HasKey("Id")
                        .HasName("pk_activity_log");

                    b.HasIndex("TenantId", "SiteId", "CreatedAt")
                        .HasDatabaseName("ix_activity_log_tenant_id_site_id_created_at");

                    b.ToTable("activity_log", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.DocumentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type_name");

                    b.HasKey("Id")
                        .HasName("pk_document_types");

                    b.HasIndex("TypeName")
                        .IsUnique()
                        .HasDatabaseName("ix_document_types_type_name");

                    b.ToTable("document_types", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ErrorCatalog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("ErrorCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("error_code");

                    b.Property<string>("RemediationMsg")
                        .HasColumnType("text")
                        .HasColumnName("remediation_msg");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_error_catalog");

                    b.HasIndex("ErrorCode")
                        .IsUnique()
                        .HasDatabaseName("ix_error_catalog_error_code");

                    b.ToTable("error_catalog", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CurrentStep")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("current_step");

                    b.Property<Guid?>("DocumentTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("document_type_id");

                    b.Property<decimal?>("ExtractionConfidence")
                        .HasPrecision(4, 3)
                        .HasColumnType("numeric(4,3)")
                        .HasColumnName("extraction_confidence");

                    b.Property<string>("ExtractionStatus")
                        .HasColumnType("text")
                        .HasColumnName("extraction_status");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_name");

                    b.Property<long?>("FileSizeBytes")
                        .HasColumnType("bigint")
                        .HasColumnName("file_size_bytes");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_type");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_at");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid")
                        .HasColumnName("transaction_id");

                    b.HasKey("Id")
                        .HasName("pk_files");

                    b.HasIndex("DocumentTypeId")
                        .HasDatabaseName("ix_files_document_type_id");

                    b.HasIndex("TransactionId")
                        .HasDatabaseName("ix_files_transaction_id");

                    b.HasIndex("TenantId", "SiteId", "DocumentTypeId")
                        .HasDatabaseName("ix_files_tenant_id_site_id_document_type_id");

                    b.HasIndex("TenantId", "SiteId", "Status", "LastUpdatedAt")
                        .HasDatabaseName("ix_files_tenant_id_site_id_status_last_updated_at");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileStepHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("completed_at");

                    b.Property<Guid?>("DocumentTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("document_type_id");

                    b.Property<string>("ErrorCode")
                        .HasColumnType("text")
                        .HasColumnName("error_code");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text")
                        .HasColumnName("error_message");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid")
                        .HasColumnName("file_id");

                    b.Property<DateTime?>("StartedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("started_at");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<string>("StepName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("step_name");

                    b.HasKey("Id")
                        .HasName("pk_file_step_history");

                    b.HasIndex("DocumentTypeId")
                        .HasDatabaseName("ix_file_step_history_document_type_id");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_file_step_history_file_id");

                    b.HasIndex("StepName", "Status")
                        .HasDatabaseName("ix_file_step_history_step_name_status");

                    b.ToTable("file_step_history", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.InvoiceLineItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal?>("Confidence")
                        .HasPrecision(4, 3)
                        .HasColumnType("numeric(4,3)")
                        .HasColumnName("confidence");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTime>("ExtractedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("extracted_at");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid")
                        .HasColumnName("file_id");

                    b.Property<bool>("IsValid")
                        .HasColumnType("boolean")
                        .HasColumnName("is_valid");

                    b.Property<Guid?>("ItemCategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("item_category_id");

                    b.Property<int>("LineNumber")
                        .HasColumnType("integer")
                        .HasColumnName("line_number");

                    b.Property<decimal?>("LineTotal")
                        .HasPrecision(12, 2)
                        .HasColumnType("numeric(12,2)")
                        .HasColumnName("line_total");

                    b.Property<decimal?>("Quantity")
                        .HasPrecision(12, 3)
                        .HasColumnType("numeric(12,3)")
                        .HasColumnName("quantity");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<decimal?>("UnitPrice")
                        .HasPrecision(12, 2)
                        .HasColumnType("numeric(12,2)")
                        .HasColumnName("unit_price");

                    b.HasKey("Id")
                        .HasName("pk_invoice_line_items");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_invoice_line_items_file_id");

                    b.HasIndex("ItemCategoryId")
                        .HasDatabaseName("ix_invoice_line_items_item_category_id");

                    b.HasIndex("TenantId", "SiteId", "ItemCategoryId")
                        .HasDatabaseName("ix_invoice_line_items_tenant_id_site_id_item_category_id");

                    b.ToTable("invoice_line_items", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ItemCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CategoryCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category_code");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category_name");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.HasKey("Id")
                        .HasName("pk_item_categories");

                    b.HasIndex("CategoryCode")
                        .IsUnique()
                        .HasDatabaseName("ix_item_categories_category_code");

                    b.ToTable("item_categories", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Site", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Location")
                        .HasColumnType("text")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.HasKey("Id")
                        .HasName("pk_sites");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_sites_tenant_id");

                    b.ToTable("sites", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_tenants");

                    b.ToTable("tenants", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("completed_at");

                    b.Property<int>("CompletedCount")
                        .HasColumnType("integer")
                        .HasColumnName("completed_count");

                    b.Property<int>("FailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("failed_count");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_at");

                    b.Property<int>("ProcessingCount")
                        .HasColumnType("integer")
                        .HasColumnName("processing_count");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<string>("SourceSystem")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("source_system");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.Property<DateTime>("SubmittedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("submitted_at");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<int>("TotalFiles")
                        .HasColumnType("integer")
                        .HasColumnName("total_files");

                    b.Property<int>("UploadedCount")
                        .HasColumnType("integer")
                        .HasColumnName("uploaded_count");

                    b.HasKey("Id")
                        .HasName("pk_transactions");

                    b.HasIndex("TenantId", "SiteId", "LastUpdatedAt")
                        .HasDatabaseName("ix_transactions_tenant_id_site_id_last_updated_at");

                    b.HasIndex("TenantId", "SiteId", "State")
                        .HasDatabaseName("ix_transactions_tenant_id_site_id_state");

                    b.ToTable("transactions", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_users_tenant_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.UserSiteAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("GrantedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("granted_at");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_site_access");

                    b.HasIndex("SiteId")
                        .HasDatabaseName("ix_user_site_access_site_id");

                    b.HasIndex("UserId", "SiteId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_site_access_user_id_site_id");

                    b.ToTable("user_site_access", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileRecord", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.DocumentType", "DocumentType")
                        .WithMany()
                        .HasForeignKey("DocumentTypeId")
                        .HasConstraintName("fk_files_document_types_document_type_id");

                    b.HasOne("DocAnalytics.Domain.Entities.Transaction", "Transaction")
                        .WithMany("Files")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_files_transactions_transaction_id");

                    b.Navigation("DocumentType");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileStepHistory", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.DocumentType", "DocumentType")
                        .WithMany()
                        .HasForeignKey("DocumentTypeId")
                        .HasConstraintName("fk_file_step_history_document_types_document_type_id");

                    b.HasOne("DocAnalytics.Domain.Entities.FileRecord", "File")
                        .WithMany("Steps")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_file_step_history_files_file_id");

                    b.Navigation("DocumentType");

                    b.Navigation("File");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.InvoiceLineItem", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.FileRecord", "File")
                        .WithMany("LineItems")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_invoice_line_items_files_file_id");

                    b.HasOne("DocAnalytics.Domain.Entities.ItemCategory", "ItemCategory")
                        .WithMany("LineItems")
                        .HasForeignKey("ItemCategoryId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_invoice_line_items_item_categories_item_category_id");

                    b.Navigation("File");

                    b.Navigation("ItemCategory");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Site", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.Tenant", "Tenant")
                        .WithMany("Sites")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_sites_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.User", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.Tenant", "Tenant")
                        .WithMany("Users")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_users_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.UserSiteAccess", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_site_access_sites_site_id");

                    b.HasOne("DocAnalytics.Domain.Entities.User", "User")
                        .WithMany("SiteAccess")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_site_access_users_user_id");

                    b.Navigation("Site");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileRecord", b =>
                {
                    b.Navigation("LineItems");

                    b.Navigation("Steps");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ItemCategory", b =>
                {
                    b.Navigation("LineItems");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Tenant", b =>
                {
                    b.Navigation("Sites");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Transaction", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.User", b =>
                {
                    b.Navigation("SiteAccess");
                });
#pragma warning restore 612, 618
        }
    }
}
````

## File: DocAnalytics.Data/Migrations/AppDbContextModelSnapshot.cs
````csharp
// <auto-generated />
using System;
using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DocAnalytics.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ActivityLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid")
                        .HasColumnName("entity_id");

                    b.Property<string>("EntityName")
                        .HasColumnType("text")
                        .HasColumnName("entity_name");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entity_type");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("event_type");

                    b.Property<string>("NewState")
                        .HasColumnType("text")
                        .HasColumnName("new_state");

                    b.Property<string>("OldState")
                        .HasColumnType("text")
                        .HasColumnName("old_state");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<string>("TriggeredBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("triggered_by");

                    b.HasKey("Id")
                        .HasName("pk_activity_log");

                    b.HasIndex("TenantId", "SiteId", "CreatedAt")
                        .HasDatabaseName("ix_activity_log_tenant_id_site_id_created_at");

                    b.ToTable("activity_log", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.DocumentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type_name");

                    b.HasKey("Id")
                        .HasName("pk_document_types");

                    b.HasIndex("TypeName")
                        .IsUnique()
                        .HasDatabaseName("ix_document_types_type_name");

                    b.ToTable("document_types", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ErrorCatalog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("ErrorCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("error_code");

                    b.Property<string>("RemediationMsg")
                        .HasColumnType("text")
                        .HasColumnName("remediation_msg");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_error_catalog");

                    b.HasIndex("ErrorCode")
                        .IsUnique()
                        .HasDatabaseName("ix_error_catalog_error_code");

                    b.ToTable("error_catalog", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CurrentStep")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("current_step");

                    b.Property<Guid?>("DocumentTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("document_type_id");

                    b.Property<decimal?>("ExtractionConfidence")
                        .HasPrecision(4, 3)
                        .HasColumnType("numeric(4,3)")
                        .HasColumnName("extraction_confidence");

                    b.Property<string>("ExtractionStatus")
                        .HasColumnType("text")
                        .HasColumnName("extraction_status");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_name");

                    b.Property<long?>("FileSizeBytes")
                        .HasColumnType("bigint")
                        .HasColumnName("file_size_bytes");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_type");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_at");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid")
                        .HasColumnName("transaction_id");

                    b.HasKey("Id")
                        .HasName("pk_files");

                    b.HasIndex("DocumentTypeId")
                        .HasDatabaseName("ix_files_document_type_id");

                    b.HasIndex("TransactionId")
                        .HasDatabaseName("ix_files_transaction_id");

                    b.HasIndex("TenantId", "SiteId", "DocumentTypeId")
                        .HasDatabaseName("ix_files_tenant_id_site_id_document_type_id");

                    b.HasIndex("TenantId", "SiteId", "Status", "LastUpdatedAt")
                        .HasDatabaseName("ix_files_tenant_id_site_id_status_last_updated_at");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileStepHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("completed_at");

                    b.Property<Guid?>("DocumentTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("document_type_id");

                    b.Property<string>("ErrorCode")
                        .HasColumnType("text")
                        .HasColumnName("error_code");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text")
                        .HasColumnName("error_message");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid")
                        .HasColumnName("file_id");

                    b.Property<DateTime?>("StartedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("started_at");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<string>("StepName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("step_name");

                    b.HasKey("Id")
                        .HasName("pk_file_step_history");

                    b.HasIndex("DocumentTypeId")
                        .HasDatabaseName("ix_file_step_history_document_type_id");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_file_step_history_file_id");

                    b.HasIndex("StepName", "Status")
                        .HasDatabaseName("ix_file_step_history_step_name_status");

                    b.ToTable("file_step_history", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.InvoiceLineItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal?>("Confidence")
                        .HasPrecision(4, 3)
                        .HasColumnType("numeric(4,3)")
                        .HasColumnName("confidence");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTime>("ExtractedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("extracted_at");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid")
                        .HasColumnName("file_id");

                    b.Property<bool>("IsValid")
                        .HasColumnType("boolean")
                        .HasColumnName("is_valid");

                    b.Property<Guid?>("ItemCategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("item_category_id");

                    b.Property<int>("LineNumber")
                        .HasColumnType("integer")
                        .HasColumnName("line_number");

                    b.Property<decimal?>("LineTotal")
                        .HasPrecision(12, 2)
                        .HasColumnType("numeric(12,2)")
                        .HasColumnName("line_total");

                    b.Property<decimal?>("Quantity")
                        .HasPrecision(12, 3)
                        .HasColumnType("numeric(12,3)")
                        .HasColumnName("quantity");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<decimal?>("UnitPrice")
                        .HasPrecision(12, 2)
                        .HasColumnType("numeric(12,2)")
                        .HasColumnName("unit_price");

                    b.HasKey("Id")
                        .HasName("pk_invoice_line_items");

                    b.HasIndex("FileId")
                        .HasDatabaseName("ix_invoice_line_items_file_id");

                    b.HasIndex("ItemCategoryId")
                        .HasDatabaseName("ix_invoice_line_items_item_category_id");

                    b.HasIndex("TenantId", "SiteId", "ItemCategoryId")
                        .HasDatabaseName("ix_invoice_line_items_tenant_id_site_id_item_category_id");

                    b.ToTable("invoice_line_items", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ItemCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CategoryCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category_code");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("category_name");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.HasKey("Id")
                        .HasName("pk_item_categories");

                    b.HasIndex("CategoryCode")
                        .IsUnique()
                        .HasDatabaseName("ix_item_categories_category_code");

                    b.ToTable("item_categories", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Site", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Location")
                        .HasColumnType("text")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.HasKey("Id")
                        .HasName("pk_sites");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_sites_tenant_id");

                    b.ToTable("sites", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_tenants");

                    b.ToTable("tenants", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("completed_at");

                    b.Property<int>("CompletedCount")
                        .HasColumnType("integer")
                        .HasColumnName("completed_count");

                    b.Property<int>("FailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("failed_count");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_at");

                    b.Property<int>("ProcessingCount")
                        .HasColumnType("integer")
                        .HasColumnName("processing_count");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<string>("SourceSystem")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("source_system");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("state");

                    b.Property<DateTime>("SubmittedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("submitted_at");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.Property<int>("TotalFiles")
                        .HasColumnType("integer")
                        .HasColumnName("total_files");

                    b.Property<int>("UploadedCount")
                        .HasColumnType("integer")
                        .HasColumnName("uploaded_count");

                    b.HasKey("Id")
                        .HasName("pk_transactions");

                    b.HasIndex("TenantId", "SiteId", "LastUpdatedAt")
                        .HasDatabaseName("ix_transactions_tenant_id_site_id_last_updated_at");

                    b.HasIndex("TenantId", "SiteId", "State")
                        .HasDatabaseName("ix_transactions_tenant_id_site_id_state");

                    b.ToTable("transactions", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("tenant_id");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ix_users_tenant_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.UserSiteAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("GrantedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("granted_at");

                    b.Property<Guid>("SiteId")
                        .HasColumnType("uuid")
                        .HasColumnName("site_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_site_access");

                    b.HasIndex("SiteId")
                        .HasDatabaseName("ix_user_site_access_site_id");

                    b.HasIndex("UserId", "SiteId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_site_access_user_id_site_id");

                    b.ToTable("user_site_access", (string)null);
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileRecord", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.DocumentType", "DocumentType")
                        .WithMany()
                        .HasForeignKey("DocumentTypeId")
                        .HasConstraintName("fk_files_document_types_document_type_id");

                    b.HasOne("DocAnalytics.Domain.Entities.Transaction", "Transaction")
                        .WithMany("Files")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_files_transactions_transaction_id");

                    b.Navigation("DocumentType");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileStepHistory", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.DocumentType", "DocumentType")
                        .WithMany()
                        .HasForeignKey("DocumentTypeId")
                        .HasConstraintName("fk_file_step_history_document_types_document_type_id");

                    b.HasOne("DocAnalytics.Domain.Entities.FileRecord", "File")
                        .WithMany("Steps")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_file_step_history_files_file_id");

                    b.Navigation("DocumentType");

                    b.Navigation("File");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.InvoiceLineItem", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.FileRecord", "File")
                        .WithMany("LineItems")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_invoice_line_items_files_file_id");

                    b.HasOne("DocAnalytics.Domain.Entities.ItemCategory", "ItemCategory")
                        .WithMany("LineItems")
                        .HasForeignKey("ItemCategoryId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_invoice_line_items_item_categories_item_category_id");

                    b.Navigation("File");

                    b.Navigation("ItemCategory");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Site", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.Tenant", "Tenant")
                        .WithMany("Sites")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_sites_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.User", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.Tenant", "Tenant")
                        .WithMany("Users")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_users_tenants_tenant_id");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.UserSiteAccess", b =>
                {
                    b.HasOne("DocAnalytics.Domain.Entities.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_site_access_sites_site_id");

                    b.HasOne("DocAnalytics.Domain.Entities.User", "User")
                        .WithMany("SiteAccess")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_site_access_users_user_id");

                    b.Navigation("Site");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.FileRecord", b =>
                {
                    b.Navigation("LineItems");

                    b.Navigation("Steps");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.ItemCategory", b =>
                {
                    b.Navigation("LineItems");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Tenant", b =>
                {
                    b.Navigation("Sites");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.Transaction", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("DocAnalytics.Domain.Entities.User", b =>
                {
                    b.Navigation("SiteAccess");
                });
#pragma warning restore 612, 618
        }
    }
}
````

## File: DocAnalytics.Domain/Common/ICurrentUser.cs
````csharp
namespace DocAnalytics.Domain.Common;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid TenantId { get; }
    Guid SiteId { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}
````

## File: DocAnalytics.Domain/Common/ITenantScoped.cs
````csharp
namespace DocAnalytics.Domain.Common;

// Marker for tables carrying BOTH tenant_id + site_id -> auto global filter
public interface ITenantScoped
{
    Guid TenantId { get; }
    Guid SiteId { get; }
}
````

## File: DocAnalytics.Domain/DocAnalytics.Domain.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
````

## File: DocAnalytics.Domain/Entities/ActivityLog.cs
````csharp
// Entities/ActivityLog.cs
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
public class ActivityLog : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public string EventType { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public Guid EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? OldState { get; set; }
    public string? NewState { get; set; }
    public string TriggeredBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
````

## File: DocAnalytics.Domain/Entities/DocumentType.cs
````csharp
// Entities/DocumentType.cs  (global catalog)
namespace DocAnalytics.Domain.Entities;

public class DocumentType
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = null!;
    public string Category { get; set; } = null!;   // PDF | CSV
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
````

## File: DocAnalytics.Domain/Entities/ErrorCatalog.cs
````csharp
// Entities/ErrorCatalog.cs  (global)
namespace DocAnalytics.Domain.Entities;

public class ErrorCatalog
{
    public Guid Id { get; set; }
    public string ErrorCode { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? RemediationMsg { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
````

## File: DocAnalytics.Domain/Entities/FileRecord.cs
````csharp
// Entities/FileRecord.cs  -> table "files"
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
using System.Xml.Linq;

public class FileRecord : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid TransactionId { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string CurrentStep { get; set; } = null!;
    public long? FileSizeBytes { get; set; }
    public string? ExtractionStatus { get; set; }
    public decimal? ExtractionConfidence { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Transaction Transaction { get; set; } = null!;
    public DocumentType? DocumentType { get; set; }
    public ICollection<FileStepHistory> Steps { get; set; } = new List<FileStepHistory>();
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}
````

## File: DocAnalytics.Domain/Entities/FileStepHistory.cs
````csharp
// Entities/FileStepHistory.cs
using System.Xml.Linq;

namespace DocAnalytics.Domain.Entities;

public class FileStepHistory
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid? DocumentTypeId { get; set; }
    public string StepName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public FileRecord File { get; set; } = null!;
    public DocumentType? DocumentType { get; set; }
}
````

## File: DocAnalytics.Domain/Entities/InvoiceLineItem.cs
````csharp
// Entities/InvoiceLineItem.cs  (NEW - table 11)
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
public class InvoiceLineItem : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid? ItemCategoryId { get; set; }
    public int LineNumber { get; set; }
    public string Description { get; set; } = null!;
    public decimal? Quantity { get; set; }          // DECIMAL(12,3)
    public decimal? UnitPrice { get; set; }          // DECIMAL(12,2)
    public decimal? LineTotal { get; set; }          // DECIMAL(12,2)
    public decimal? Confidence { get; set; }         // DECIMAL(4,3)
    public bool IsValid { get; set; }
    public DateTime ExtractedAt { get; set; }
    public FileRecord File { get; set; } = null!;
    public ItemCategory? ItemCategory { get; set; }
}
````

## File: DocAnalytics.Domain/Entities/ItemCategory.cs
````csharp
// Entities/ItemCategory.cs  (NEW - table 12, master catalog, global)
namespace DocAnalytics.Domain.Entities;

public class ItemCategory
{
    public Guid Id { get; set; }
    public string CategoryCode { get; set; } = null!;   // UNIQUE
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}
````

## File: DocAnalytics.Domain/Entities/Site.cs
````csharp
// Entities/Site.cs
namespace DocAnalytics.Domain.Entities;

public class Site
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
````

## File: DocAnalytics.Domain/Entities/Tenant.cs
````csharp
// Entities/Tenant.cs
namespace DocAnalytics.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Site> Sites { get; set; } = new List<Site>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
````

## File: DocAnalytics.Domain/Entities/Transaction.cs
````csharp
// Entities/Transaction.cs  (the "TId" / batch)
namespace DocAnalytics.Domain.Entities;

using DocAnalytics.Domain.Common;
public class Transaction : ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public string State { get; set; } = null!;
    public string SourceSystem { get; set; } = null!;
    public int TotalFiles { get; set; }
    public int UploadedCount { get; set; }
    public int ProcessingCount { get; set; }
    public int FailedCount { get; set; }
    public int CompletedCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ICollection<FileRecord> Files { get; set; } = new List<FileRecord>();
}
````

## File: DocAnalytics.Domain/Entities/User.cs
````csharp
// Entities/User.cs
namespace DocAnalytics.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;     // Admin | Viewer
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public ICollection<UserSiteAccess> SiteAccess { get; set; } = new List<UserSiteAccess>();
}
````

## File: DocAnalytics.Domain/Entities/UserSiteAccess.cs
````csharp
// Entities/UserSiteAccess.cs
namespace DocAnalytics.Domain.Entities;

public class UserSiteAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SiteId { get; set; }
    public DateTime GrantedAt { get; set; }
    public User User { get; set; } = null!;
    public Site Site { get; set; } = null!;
}
````

## File: DocAnalytics.Service/Auth/AuthDtos.cs
````csharp
namespace DocAnalytics.Service.Auth;

// What the client SENDS to POST /auth/login
public record LoginRequest(string Email, string Password);

// What POST /auth/login RETURNS
public record LoginResponse(string Token, UserDto User, IReadOnlyList<SiteDto> Sites);

// What GET /auth/me RETURNS
public record MeResponse(UserDto User, IReadOnlyList<SiteDto> Sites);

// Safe view of a user — NOTE: no password hash ever leaves here
public record UserDto(Guid Id, string Email, string Role);

// One site the user is allowed to access
public record SiteDto(Guid SiteId, string SiteName);
````

## File: DocAnalytics.Service/Auth/AuthFeatureExtensions.cs
````csharp
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Auth;

public static class AuthFeatureExtensions
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Auth/IAuthService.cs
````csharp
namespace DocAnalytics.Service.Auth;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest req, CancellationToken ct);
    Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<SiteDto>> GetSitesAsync(Guid userId, CancellationToken ct);
}
````

## File: DocAnalytics.Service/Common/PagedResult.cs
````csharp
namespace DocAnalytics.Service.Common;

public sealed class PagedResult<T>
{
    public List<T> Items { get; init; } = new();   // the rows for THIS page
    public int TotalCount { get; init; }            // total rows across ALL pages
    public int Page { get; init; }                  // which page this is
    public int PageSize { get; init; }              // rows per page

    // computed: e.g. 95 items / 20 per page = 5 pages
    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling(TotalCount / (double)PageSize)
        : 0;
}
````

## File: DocAnalytics.Service/Dashboard/DashboardDtos.cs
````csharp
namespace DocAnalytics.Service.Dashboard;

// FR-1.1 — status counters (snake_case'd globally → queued, in_progress, ...)
public sealed class DashboardSummaryResponse
{
    public int Queued { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Failed { get; set; }
    public int Total { get; set; }
}

// FR-1.4 — one row per failed step
public sealed class RecentFailureDto
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = default!;
    public string FailedStep { get; set; } = default!;
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? FailedAt { get; set; }
}

// query-string params (same naming style as BatchListQuery)
public sealed class RecentFailuresQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }   // failed_at | file_name | failed_step
    public string? SortDir { get; set; }  // asc | desc
}
````

## File: DocAnalytics.Service/Dashboard/DashboardFeatureExtensions.cs
````csharp
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Dashboard;

public static class DashboardFeatureExtensions
{
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Dashboard/DashboardService.cs
````csharp
using DocAnalytics.Data;
using DocAnalytics.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    public DashboardService(AppDbContext db) => _db = db;

    // FR-1.1 — SUM the per-batch counters. tenant_id + site_id auto-applied
    // by the global query filter on Transaction (ITenantScoped).
    public async Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken ct = default)
    {
        var summary = await _db.Transactions
            .AsNoTracking()
            .GroupBy(_ => 1)                       // collapse all rows into ONE aggregate row
            .Select(g => new DashboardSummaryResponse
            {
                Queued = g.Sum(t => t.UploadedCount),
                InProgress = g.Sum(t => t.ProcessingCount),
                Completed = g.Sum(t => t.CompletedCount),
                Failed = g.Sum(t => t.FailedCount)
            })
            .FirstOrDefaultAsync(ct) ?? new DashboardSummaryResponse(); // no rows → all zeros

        summary.Total = summary.Queued + summary.InProgress + summary.Completed + summary.Failed;
        return summary;
    }

    // FR-1.4 — start FROM Files (tenant+site auto-filtered) then join the
    // failed steps. FileStepHistory is NOT ITenantScoped, so driving from
    // Files is what keeps tenant isolation intact.
    public async Task<PagedResult<RecentFailureDto>> GetRecentFailuresAsync(
        RecentFailuresQuery query, CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var baseQuery =
            from f in _db.Files.AsNoTracking()
            join s in _db.FileStepHistory.AsNoTracking() on f.Id equals s.FileId
            where s.Status == "Failed"             // matches DbSeeder literal exactly
            select new RecentFailureDto
            {
                FileId = f.Id,
                FileName = f.FileName,
                FailedStep = s.StepName,
                ErrorCode = s.ErrorCode,
                ErrorMessage = s.ErrorMessage,
                FailedAt = s.CompletedAt ?? s.StartedAt
            };

        var totalCount = await baseQuery.CountAsync(ct);

        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortDir);

        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<RecentFailureDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // whitelisted sort → no string concat → SQL-injection safe (NFR-3)
    private static IQueryable<RecentFailureDto> ApplySorting(
    IQueryable<RecentFailureDto> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        IOrderedQueryable<RecentFailureDto> ordered = (sortBy ?? "failed_at").ToLowerInvariant() switch
        {
            "file_name" => desc ? q.OrderByDescending(r => r.FileName) : q.OrderBy(r => r.FileName),
            "failed_step" or "step" => desc ? q.OrderByDescending(r => r.FailedStep) : q.OrderBy(r => r.FailedStep),
            _ => desc ? q.OrderByDescending(r => r.FailedAt) : q.OrderBy(r => r.FailedAt)
        };

        return ordered.ThenBy(r => r.FileId);   // stable page boundaries even on tied timestamps
    }

}
````

## File: DocAnalytics.Service/Dashboard/IDashboardService.cs
````csharp
using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken ct = default);
    Task<PagedResult<RecentFailureDto>> GetRecentFailuresAsync(
        RecentFailuresQuery query, CancellationToken ct = default);
}
````

## File: DocAnalytics.Service/DependencyInjection.cs
````csharp
using DocAnalytics.Service.Health;
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //services.AddScoped<IHealthService, HealthService>();
        // Dev A/B add their feature services here later:
        // services.AddScoped<IAuthService, AuthService>();
        //services.AddScoped<IBatchService, BatchService>(); 
        return services;
    }
}
````

## File: DocAnalytics.Service/DocAnalytics.Service.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\DocAnalytics.Data\DocAnalytics.Data.csproj" />
    <ProjectReference Include="..\DocAnalytics.Domain\DocAnalytics.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.2.0" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.19.1" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
````

## File: DocAnalytics.Service/Files/FileDetailsDtos.cs
````csharp
namespace DocAnalytics.Service.Files;

// ── GET /api/v1/files/{id}/details : the nested DTO ──
public sealed class FileDetailDto
{
    public FileInfoDto FileInfo { get; set; } = null!;          // → "file_info"
    public List<StepHistoryDto> History { get; set; } = new();  // → "history"
}

public sealed class FileInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;          // → "name"
    public string CurrentStatus { get; set; } = null!; // → "current_status"
    public string CurrentStep { get; set; } = null!;   // → "current_step"
}

public sealed class StepHistoryDto
{
    public string Step { get; set; } = null!;     // FileStepHistory.StepName
    public string Status { get; set; } = null!;   // Success | Failed | Processing
    public DateTime? Ts { get; set; }             // step timestamp
    public StepErrorDto? Error { get; set; }      // only present on failed steps
}

public sealed class StepErrorDto
{
    public string Code { get; set; } = null!;        // FileStepHistory.ErrorCode
    public string? Message { get; set; }             // FileStepHistory.ErrorMessage
    public string? SuggestedFix { get; set; }        // ErrorCatalog.RemediationMsg (by code)
}

// ── downloadable logs payload ──
public sealed class FileLogDto
{
    public string FileName { get; set; } = null!;   // e.g. file_xxx_log.txt
    public string Content { get; set; } = null!;    // plain-text trace
}
````

## File: DocAnalytics.Service/Files/FileDetailsFeatureExtensions.cs
````csharp
using DocAnalytics.Service.Files;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class FileDetailsFeatureExtensions
{
    public static IServiceCollection AddFileDetailsFeature(this IServiceCollection services)
    {
        services.AddScoped<IFileDetailsService, FileDetailsService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Files/IFileDetailsService.cs
````csharp
namespace DocAnalytics.Service.Files;

public interface IFileDetailsService
{
    Task<FileDetailDto?> GetFileDetailsAsync(Guid fileId, CancellationToken ct = default);
    Task<FileLogDto?> GetFileLogsAsync(Guid fileId, CancellationToken ct = default);
}
````

## File: DocAnalytics.Service/Health/HealthFeatureExtensions.cs
````csharp
using Microsoft.Extensions.DependencyInjection;
namespace DocAnalytics.Service.Health;

public static class HealthFeatureExtensions
{
    public static IServiceCollection AddHealthFeature(this IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Health/HealthService.cs
````csharp
using DocAnalytics.Data;

namespace DocAnalytics.Service.Health;

public class HealthService : IHealthService
{
    private readonly AppDbContext _db;
    public HealthService(AppDbContext db) => _db = db;

    public Task<bool> IsDatabaseReachableAsync() => _db.Database.CanConnectAsync();
}
````

## File: DocAnalytics.Service/Health/IHealthService.cs
````csharp
namespace DocAnalytics.Service.Health;

public interface IHealthService
{
    Task<bool> IsDatabaseReachableAsync();
}
````

## File: DocAnalytics.Service/Invoices/IInvoiceService.cs
````csharp
namespace DocAnalytics.Service.Invoices;

public interface IInvoiceService
{
    Task<InvoiceDetailDto?> GetInvoiceForFileAsync(Guid fileId, CancellationToken ct = default);
}
````

## File: DocAnalytics.Service/Invoices/InvoiceDtos.cs
````csharp
namespace DocAnalytics.Service.Invoices;

// The whole response: one file's invoice line items + computed totals.
public sealed class InvoiceDetailDto
{
    public Guid FileId { get; set; }
    public int LineItemCount { get; set; }
    public decimal GrandTotal { get; set; }              // a total is always a number
    public List<InvoiceLineItemDto> Items { get; set; } = new();
}

// One row on the invoice: the line item + its category (joined from the global catalog).
public sealed class InvoiceLineItemDto
{
    public Guid Id { get; set; }
    public int LineNumber { get; set; }
    public string Description { get; set; } = null!;
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineTotal { get; set; }
    public decimal? Confidence { get; set; }
    public bool IsValid { get; set; }
    public string? CategoryCode { get; set; }    // null when the line has no category (LEFT join)
    public string? CategoryName { get; set; }    // null when the line has no category (LEFT join)
}
````

## File: DocAnalytics.Service/Invoices/InvoiceFeatureExtensions.cs
````csharp
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Invoices;

public static class InvoiceFeatureExtensions
{
    public static IServiceCollection AddInvoiceFeature(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Invoices/InvoiceService.cs
````csharp
using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Invoices;

public sealed class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;
    public InvoiceService(AppDbContext db) => _db = db;

    public async Task<InvoiceDetailDto?> GetInvoiceForFileAsync(Guid fileId, CancellationToken ct = default)
    {
        // 1. Does this file exist for THIS tenant/site? (Files is tenant-scoped → auto-filtered)
        var fileExists = await _db.Files
            .AsNoTracking()
            .AnyAsync(f => f.Id == fileId, ct);

        if (!fileExists)
            return null;                       // → controller turns this into 404

        // 2. Pull this file's line items, LEFT-joined out to the global category catalog.
        var items = await _db.InvoiceLineItems
            .AsNoTracking()
            .Where(li => li.FileId == fileId)  // tenant_id + site_id auto-added by the global filter
            .OrderBy(li => li.LineNumber)
            .Select(li => new InvoiceLineItemDto
            {
                Id = li.Id,
                LineNumber = li.LineNumber,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                LineTotal = li.LineTotal,
                Confidence = li.Confidence,
                IsValid = li.IsValid,
                CategoryCode = li.ItemCategory != null ? li.ItemCategory.CategoryCode : null,
                CategoryName = li.ItemCategory != null ? li.ItemCategory.CategoryName : null
            })
            .ToListAsync(ct);

        // 3. Compute totals over ALL lines (null-safe sum).
        var grandTotal = items.Sum(i => i.LineTotal ?? 0m);

        // 4. Assemble the detail response.
        return new InvoiceDetailDto
        {
            FileId = fileId,
            LineItemCount = items.Count,
            GrandTotal = grandTotal,
            Items = items
        };
    }
}
````

## File: DocAnalytics.slnx
````
<Solution>
  <Project Path="DocAnalytics.Api/DocAnalytics.Api.csproj" />
  <Project Path="DocAnalytics.Data/DocAnalytics.Data.csproj" />
  <Project Path="DocAnalytics.Domain/DocAnalytics.Domain.csproj" />
  <Project Path="DocAnalytics.Service/DocAnalytics.Service.csproj" />
</Solution>
````

## File: InternProject-Requirements.md
````markdown
# Document Processing Analytics — Intern Project Requirements

## Overview

Your company runs a cloud-based **document processing platform**. Customers upload documents (PDFs, spreadsheets, CAD files) which go through multiple processing stages before being published to a central data store. Your task is to build an **analytics and monitoring web application** that gives operations teams visibility into the health and status of this pipeline.

You are **not** given a database schema, API design, or technology choices. You must analyze these requirements, design the system from scratch, and build it end to end.

---

## Business Context

- The platform serves **multiple customers** (called "tenants"). Each tenant has one or more **sites** (physical locations like factories or plants).
- Each tenant + site combination is completely isolated — one customer must never see another customer's data.
- Documents are uploaded in **batches**. A batch is a group of files submitted together as a single unit of work.
- Each file goes through a **multi-step pipeline**: Upload → Validate → Transform → Publish.
- Files can succeed, fail, or get stuck at any step.
- Operations teams need to monitor this pipeline, identify failures, and take corrective action.

---

## Functional Requirements

### FR-1: Dashboard

The main landing page should give an at-a-glance view of system health for a selected tenant and site.

**FR-1.1**: Show **summary counters** for total files in each status:
- Queued (waiting to be processed)
- In Progress (currently being processed)  
- Completed (successfully published)
- Failed (errored at any step)

**FR-1.2**: Show a **chart** of processing throughput — how many files were completed per hour/day over a configurable time range.

**FR-1.3**: Show a **chart** breaking down the current file status distribution (e.g., pie or bar chart).

**FR-1.4**: Show a **table of recent failures** with the file name, the step where it failed, the error message, and when it failed. This table should be sortable and paginated.

**FR-1.5**: The dashboard should **auto-refresh** at a configurable interval (e.g., every 30 seconds) without full page reload.

---

### FR-2: Batch Explorer

A page to browse and inspect batches and their files.

**FR-2.1**: Show a **paginated list of batches** with columns: Batch ID, status (In Progress / Completed / Failed), number of files, submission time, completion time, and source system.

**FR-2.2**: Allow **filtering** by:
- Status (all, in-progress, completed, failed)
- Date range (submitted between)
- Source system

**FR-2.3**: Allow **searching** by Batch ID.

**FR-2.4**: Clicking a batch should open a **batch detail view** showing:
- Batch summary (status, file counts per status, start/end time)
- A table of all files in the batch with: file name, current status, current step, last updated time
- For each file, the ability to drill down into step-by-step history

**FR-2.5**: The **file step history** should show a timeline/table of every processing step the file went through, with: step name, status (success/failed/skipped), timestamp, and error details (if any).

---

### FR-3: Error Analysis

A page dedicated to understanding and resolving failures.

**FR-3.1**: Show the **top 10 most frequent errors** with occurrence count, grouped by error code or error message.

**FR-3.2**: Show an **error trend chart** — number of failures per day over the last 30 days.

**FR-3.3**: For each error, show a **suggested fix** (remediation message) if one exists in the system.

**FR-3.4**: Allow **filtering errors** by:
- Date range
- Processing step where the error occurred
- Source system

**FR-3.5**: Allow **exporting** the filtered error list to CSV.

---

### FR-4: Activity Log

An audit trail of significant events in the system.

**FR-4.1**: Show a **chronological log** of events such as:
- Batch submitted
- File state changed (e.g., from "In Progress" to "Failed")
- Batch completed
- Remediation message updated

**FR-4.2**: Each log entry should include: timestamp, event type, related entity (batch ID or file name), old state, new state, and who/what triggered it.

**FR-4.3**: Allow **filtering** by event type, entity, and date range.

**FR-4.4**: The log should be **paginated** (not load all records at once).

---

### FR-5: Tenant & Site Selection

**FR-5.1**: The application must support multiple tenants and sites. Provide a way for the user to select which tenant and site they are viewing.

**FR-5.2**: All data displayed across every page must be scoped to the selected tenant + site. Switching tenant/site should reload all data.

**FR-5.3**: A user should not be able to access data from a tenant/site they are not authorized for.

---

## Non-Functional Requirements

### NFR-1: Performance
- Dashboard page must load within **3 seconds** with up to 1 million file records in the database.
- Paginated lists must return results within **1 second** for page sizes up to 50.
- The system should handle **10 concurrent users** without degradation.

### NFR-2: Usability
- The application must be **responsive** and usable on screens from 1024px to 1920px wide.
- Use consistent navigation (sidebar or top nav) across all pages.
- Show **loading indicators** when data is being fetched.
- Show **user-friendly error messages** when API calls fail.

### NFR-3: Security
- All API endpoints must require **authentication** (token-based).
- All database queries must enforce **tenant isolation** — a user must only see their own tenant's data.
- No raw SQL concatenation — all queries must be parameterized.
- API inputs must be validated (e.g., date ranges, page sizes, IDs).

### NFR-4: Reliability
- The application must include a **health check endpoint** that verifies database connectivity.
- Failed API calls on the frontend should show a retry option or a meaningful error — not a blank screen.

### NFR-5: Maintainability
- Code should follow **separation of concerns** — data access, business logic, and API/presentation layers should be distinct.
- Use a **migration-based approach** for database schema management (not manual DDL scripts).
- API responses should follow a **consistent format** (e.g., `{ data: ..., error: ..., pagination: ... }`).

---

## Design Tasks (Before You Write Code)

Complete these design exercises before building. Document your decisions.

### DT-1: Data Modeling
- What **entities** (tables) do you need? What are their columns and data types?
- What are the **relationships** between entities? (one-to-many, many-to-many)
- What **indexes** will you create and why?
- How will you enforce **tenant isolation** at the database level?
- How will you track **file step history** — one row per step, or a JSON array?

### DT-2: API Design
- List all your **API endpoints** with HTTP method, URL, request parameters, and response shape.
- How will you handle **pagination**? (offset-based, cursor-based)
- How will you handle **filtering and sorting**? (query parameters, request body)
- How will you structure **error responses**?
- How will you handle **authentication** on each request?

### DT-3: Frontend Architecture
- What **pages/routes** will your application have?
- What **reusable components** can you identify? (e.g., status badges appear everywhere)
- How will you manage **state**? (signals, services, NgRx, or simple observables)
- How will you handle the **tenant/site selection** globally?
- How will you implement **auto-refresh** on the dashboard?

### DT-4: Performance Thinking
- If the Files table has 1 million rows, how will your "file state distribution" query perform?
- Should you **pre-aggregate** counts or compute them on the fly?
- What data is safe to **cache** and for how long?
- How will you avoid the **N+1 query problem** when loading a batch with its files?

---

## Evaluation Criteria

Your project will be assessed on:

| Criteria | Weight | What We Look For |
|----------|--------|-----------------|
| **Data Model Design** | 20% | Normalized schema, appropriate indexes, tenant isolation, relationships |
| **API Design** | 20% | RESTful conventions, consistent responses, pagination, error handling |
| **Frontend Implementation** | 20% | Component architecture, routing, state management, UX quality |
| **Code Quality** | 15% | Separation of concerns, naming conventions, no hardcoded values, testability |
| **Performance Awareness** | 10% | Indexed queries, pagination, caching strategy, no unnecessary data loading |
| **Security** | 10% | Auth enforcement, tenant isolation, input validation, parameterized queries |
| **Documentation** | 5% | Design decisions recorded, README with setup instructions, API documented |

---

## Constraints & Guidelines

- **You choose the technology stack.** Suggested options (pick one per layer):
  - Backend: ASP.NET Core, Node.js (Express/NestJS), Python (FastAPI/Django), Java (Spring Boot)
  - Frontend: Angular, React, Vue
  - Database: PostgreSQL, MySQL, SQL Server
  - ORM: Entity Framework Core, Prisma, SQLAlchemy, TypeORM, Sequelize

- You must use a **relational database** (not MongoDB or similar). The goal is to practice relational design.

- Start with the **database design** before writing any code. Get it reviewed before proceeding.

- Build the **API layer** before the frontend. Verify it works with Postman or Swagger before connecting the UI.

- Use **seed data** to populate your database with realistic test data (at least 100 batches, 500+ files across multiple tenants).

---

## Stretch Goals (Optional)

If you finish early, consider adding:

- **S-1**: WebSocket/SignalR live updates — push file state changes to the dashboard in real time instead of polling.
- **S-2**: Dark mode toggle with persisted preference.
- **S-3**: Role-based access — Admin can see all tenants, Viewer can only see assigned tenant.
- **S-4**: Email notification configuration — let users set up alerts when failure rate exceeds a threshold.
- **S-5**: File processing time percentiles — P50, P90, P99 processing times per step.
- **S-6**: Comparison view — compare throughput between two date ranges side by side.

---

## Getting Started

1. Read all requirements above thoroughly
2. Complete **DT-1 through DT-4** (design tasks) and get them reviewed
3. Set up your database and create your schema via migrations
4. Write a seed script to generate test data
5. Build and test your API endpoints (use Swagger or Postman)
6. Build the frontend, connecting one page at a time
7. Demo your working application
````

## File: .gitattributes
````
**/package-lock.json linguist-generated=true
package-lock.json linguist-generated=true
yarn.lock linguist-generated=true
pnpm-lock.yaml linguist-generated=true
*.min.js linguist-generated=true
*.min.css linguist-generated=true
````

## File: docanalytics-web/.editorconfig
````
# Editor configuration, see https://editorconfig.org
root = true

[*]
charset = utf-8
indent_style = space
indent_size = 2
insert_final_newline = true
trim_trailing_whitespace = true

[*.ts]
quote_type = single
ij_typescript_use_double_quotes = false

[*.md]
max_line_length = off
trim_trailing_whitespace = false
````

## File: docanalytics-web/.gitignore
````
# See https://docs.github.com/get-started/getting-started-with-git/ignoring-files for more about ignoring files.

# Compiled output
/dist
/tmp
/out-tsc
/bazel-out

# Node
/node_modules
npm-debug.log
yarn-error.log

# IDEs and editors
.idea/
.project
.classpath
.c9/
*.launch
.settings/
*.sublime-workspace

# Visual Studio Code
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
!.vscode/mcp.json
.history/*

# Miscellaneous
/.angular/cache
.sass-cache/
/connect.lock
/coverage
/libpeerconnection.log
testem.log
/typings
__screenshots__/

# System files
.DS_Store
Thumbs.db
````

## File: docanalytics-web/.prettierrc
````
{
  "printWidth": 100,
  "singleQuote": true,
  "overrides": [
    {
      "files": "*.html",
      "options": {
        "parser": "angular"
      }
    }
  ]
}
````

## File: docanalytics-web/.vscode/extensions.json
````json
{
  // For more information, visit: https://go.microsoft.com/fwlink/?linkid=827846
  "recommendations": ["angular.ng-template"]
}
````

## File: docanalytics-web/.vscode/launch.json
````json
{
  // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
  "version": "0.2.0",
  "configurations": [
    {
      "name": "ng serve",
      "type": "chrome",
      "request": "launch",
      "preLaunchTask": "npm: start",
      "url": "http://localhost:4200/"
    },
    {
      "name": "ng test",
      "type": "chrome",
      "request": "launch",
      "preLaunchTask": "npm: test",
      "url": "http://localhost:9876/debug.html"
    }
  ]
}
````

## File: docanalytics-web/.vscode/mcp.json
````json
{
  // For more information, visit: https://angular.dev/ai/mcp
  "servers": {
    "angular-cli": {
      "command": "npx",
      "args": ["-y", "@angular/cli", "mcp"]
    }
  }
}
````

## File: docanalytics-web/.vscode/tasks.json
````json
{
  // For more information, visit: https://go.microsoft.com/fwlink/?LinkId=733558
  "version": "2.0.0",
  "tasks": [
    {
      "type": "npm",
      "script": "start",
      "isBackground": true,
      "problemMatcher": {
        "owner": "typescript",
        "pattern": "$tsc",
        "background": {
          "activeOnStart": true,
          "beginsPattern": {
            "regexp": "Changes detected"
          },
          "endsPattern": {
            "regexp": "bundle generation (complete|failed)"
          }
        }
      }
    },
    {
      "type": "npm",
      "script": "test",
      "isBackground": true,
      "problemMatcher": {
        "owner": "typescript",
        "pattern": "$tsc",
        "background": {
          "activeOnStart": true,
          "beginsPattern": {
            "regexp": "Changes detected"
          },
          "endsPattern": {
            "regexp": "bundle generation (complete|failed)"
          }
        }
      }
    }
  ]
}
````

## File: docanalytics-web/package.json
````json
{
  "name": "docanalytics-web",
  "version": "0.0.0",
  "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "build": "ng build",
    "watch": "ng build --watch --configuration development",
    "test": "ng test"
  },
  "private": true,
  "packageManager": "npm@11.13.0",
  "dependencies": {
    "@angular/common": "^22.0.0",
    "@angular/compiler": "^22.0.0",
    "@angular/core": "^22.0.0",
    "@angular/forms": "^22.0.0",
    "@angular/platform-browser": "^22.0.0",
    "@angular/router": "^22.0.0",
    "rxjs": "~7.8.0",
    "tslib": "^2.3.0"
  },
  "devDependencies": {
    "@angular/build": "^22.0.1",
    "@angular/cli": "^22.0.1",
    "@angular/compiler-cli": "^22.0.0",
    "jsdom": "^28.0.0",
    "prettier": "^3.8.1",
    "typescript": "~6.0.2",
    "vitest": "^4.0.8"
  }
}
````

## File: docanalytics-web/proxy.conf.json
````json
{
  "/api": {
    "target": "https://localhost:7001",
    "secure": false,
    "changeOrigin": true
  }
}
````

## File: docanalytics-web/src/app/app.css
````css

````

## File: docanalytics-web/src/app/app.html
````html
<router-outlet />
````

## File: docanalytics-web/src/app/app.spec.ts
````typescript
import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Hello, docanalytics-web');
  });
});
````

## File: docanalytics-web/src/app/app.ts
````typescript
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('docanalytics-web');
}
````

## File: docanalytics-web/src/app/core/guards/auth.guard.ts
````typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = async () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // No token at all → straight to login.
  if (!auth.token()) {
    return router.createUrlTree(['/login']);
  }

  // Token exists but signals empty (e.g. after refresh) → rehydrate via /auth/me.
  if (!auth.currentUser()) {
    const ok = await auth.ensureSession();
    if (!ok) return router.createUrlTree(['/login']);
  }

  return true;
};
````

## File: docanalytics-web/src/app/core/interceptors/auth-site.interceptor.ts
````typescript
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { SiteContextService } from '../services/site-context.service';

// NOTE: must match AuthService's storage key (coordinate with Dev A — we agreed on 'da_token').
// We read from localStorage (not AuthService) to avoid circular DI: AuthService uses HttpClient,
// and HttpClient runs these interceptors.
const TOKEN_KEY = 'da_token';

export const authSiteInterceptor: HttpInterceptorFn = (req, next) => {
  const siteId = inject(SiteContextService).selectedSiteId();
  const token = localStorage.getItem(TOKEN_KEY);

  let headers = req.headers;
  if (token) headers = headers.set('Authorization', `Bearer ${token}`);
  if (siteId) headers = headers.set('X-Site-Id', siteId);

  return next(req.clone({ headers }));
};
````

## File: docanalytics-web/src/app/core/models/api-response.model.ts
````typescript
// The single response contract every endpoint returns (DT-2, NFR-5).
export interface ApiResponse<T> {
  data: T | null;
  meta?: Meta;        // present only on list endpoints
  error: ApiError | null;
}

export interface Meta {
  total_count: number;
  page: number;
  page_size: number;
  total_pages: number;
}

export interface ApiError {
  code: string;
  message: string;
  details?: unknown;
}
````

## File: docanalytics-web/src/app/core/models/auth.model.ts
````typescript
export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthUser {
  id: string;
  email: string;
  role: string; // 'Admin' | 'Viewer'
}

// Backend returns sites as { site_id, site_name } (snake_case)
export interface SiteSummary {
  site_id: string;
  site_name: string;
}

// POST /auth/login → data
export interface LoginResponse {
  token: string;
  user: AuthUser;
  sites: SiteSummary[];
}

// GET /auth/me → data
export interface MeResponse {
  user: AuthUser;
  sites: SiteSummary[];
}
````

## File: docanalytics-web/src/app/core/models/dashboard.model.ts
````typescript
// One generic series shape — both throughput & status-distribution return this.
export interface SeriesPoint {
  label: string;   // throughput: "2026-05-30"  |  distribution: "Completed"
  value: number;
}
export interface ChartSeries {
  points: SeriesPoint[];
}
````

## File: docanalytics-web/src/app/core/services/auth.service.ts
````typescript
import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, firstValueFrom, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { AuthUser, LoginResponse, MeResponse, SiteSummary } from '../models/auth.model';

const TOKEN_KEY = 'da_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBase}/auth`;

  // --- writable signals (private) ---
  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly _currentUser = signal<AuthUser | null>(null);
  private readonly _sites = signal<SiteSummary[]>([]);

  // --- public readonly views ---
  readonly token = this._token.asReadonly();
  readonly currentUser = this._currentUser.asReadonly();
  readonly sites = this._sites.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token());

  /** POST /auth/login — stores token + user + sites on success. */
  login(email: string, password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${this.baseUrl}/login`, { email, password })
      .pipe(
        tap((res) => {
          if (res.data) {
            this.setSession(res.data.token, res.data.user, res.data.sites);
          }
        }),
      );
  }

  /** GET /auth/me — rehydrates user + sites (used after a page refresh). */
  loadMe(): Observable<ApiResponse<MeResponse>> {
    return this.http.get<ApiResponse<MeResponse>>(`${this.baseUrl}/me`).pipe(
      tap((res) => {
        if (res.data) {
          this._currentUser.set(res.data.user);
          this._sites.set(res.data.sites);
        }
      }),
    );
  }

  /**
   * Ensures the in-memory session is populated.
   * On a hard refresh the token survives in localStorage but signals are empty,
   * so we lazily call /auth/me. Returns false if there's no valid session.
   */
  async ensureSession(): Promise<boolean> {
    if (!this._token()) return false;
    if (this._currentUser()) return true;
    try {
      const res = await firstValueFrom(this.loadMe());
      return !!res.data;
    } catch {
      this.logout();
      return false;
    }
  }

  logout(): void {
    this._token.set(null);
    this._currentUser.set(null);
    this._sites.set([]);
    localStorage.removeItem(TOKEN_KEY);
  }

  /** Used by siteAccessGuard (FR-5.3 client-side mirror). */
  hasSiteAccess(siteId: string): boolean {
    return this._sites().some((s) => s.site_id === siteId);
  }

  private setSession(token: string, user: AuthUser, sites: SiteSummary[]): void {
    this._token.set(token);
    this._currentUser.set(user);
    this._sites.set(sites);
    localStorage.setItem(TOKEN_KEY, token);
  }
}
````

## File: docanalytics-web/src/app/core/services/refresh-timer.service.ts
````typescript
import { DestroyRef, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EMPTY, fromEvent, merge, of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class RefreshTimerService {
  /** Fires onTick now + every intervalMs. Pauses while tab hidden; re-fires on return. */
  start(intervalMs: number, onTick: () => void, destroyRef: DestroyRef): void {
    const visible$ = merge(
      of(!document.hidden),
      fromEvent(document, 'visibilitychange').pipe(map(() => !document.hidden)),
    );
    visible$
      .pipe(
        switchMap(visible => (visible ? timer(0, intervalMs) : EMPTY)),
        takeUntilDestroyed(destroyRef),
      )
      .subscribe(() => onTick());
  }
}
````

## File: docanalytics-web/src/app/core/services/site-context.service.ts
````typescript
import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SiteContextService {
  /** the currently selected site id (source of truth = the :siteId URL param) */
  readonly selectedSiteId = signal<string | null>(null);

  setSite(id: string | null): void {
    this.selectedSiteId.set(id);
  }
}
````

## File: docanalytics-web/src/app/core/services/theme.service.ts
````typescript
import { Injectable, signal, computed, effect } from '@angular/core';

export type Theme = 'light' | 'dark';
const STORAGE_KEY = 'da_theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private _theme = signal<Theme>(this.readInitial());
  readonly theme = this._theme.asReadonly();
  readonly isDark = computed(() => this._theme() === 'dark');

  constructor() {
    effect(() => {
      const t = this._theme();
      document.documentElement.setAttribute('data-theme', t);
      localStorage.setItem(STORAGE_KEY, t);
    });
  }

  toggle(): void { this._theme.update(t => (t === 'dark' ? 'light' : 'dark')); }
  set(theme: Theme): void { this._theme.set(theme); }

  private readInitial(): Theme {
    const saved = localStorage.getItem(STORAGE_KEY) as Theme | null;
    if (saved === 'dark' || saved === 'light') return saved;
    return window.matchMedia?.('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }
}
````

## File: docanalytics-web/src/app/features/activity-log/activity-log.component.css
````css
.al {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.al-head {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: var(--space-2);
  flex-wrap: wrap;
}

.al-eyebrow {
  margin: 0;
  font-size: 0.72rem;
  letter-spacing: .08em;
  text-transform: uppercase;
  color: var(--dark-gray-3);
}

.al-title {
  margin: 2px 0 0;
  font-family: var(--font-display);
  font-size: 1.25rem;
  color: var(--dark-gray);
}

.al-search {
  min-width: 240px;
  padding: 8px 12px;
  border: 1px solid var(--cool-gray);
  border-radius: 8px;
  font: inherit;
  color: var(--dark-gray);
  background: var(--white);
}

  .al-search:focus {
    outline: none;
    border-color: var(--slate-blue);
  }

.al-evt {
  font-weight: 600;
  color: var(--dark-gray);
}

.al-entity {
  color: var(--dark-gray);
}

.al-tag {
  margin-left: 8px;
  padding: 1px 8px;
  border-radius: 999px;
  font-size: 0.7rem;
  background: var(--bg-light);
  color: var(--dark-gray-3);
  border: 1px solid var(--cool-gray);
}

.al-transition {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.al-arrow {
  color: var(--dark-gray-3);
}

.al-actor {
  color: var(--dark-gray-3);
}

.al-muted {
  color: var(--cool-gray);
}

@media (max-width: 1180px) {
  .al-search {
    flex: 1 1 100%;
  }
}
````

## File: docanalytics-web/src/app/features/activity-log/activity-log.component.html
````html
<section class="al">
  <header class="al-head">
    <div>
      <p class="al-eyebrow">Audit trail</p>
      <h2 class="al-title">Activity Log</h2>
    </div>
    <input class="al-search"
           type="search"
           placeholder="Search entity (file / batch)…"
           (input)="onSearch($event)" />
  </header>

  <app-filter-bar statusLabel="Event type"
                  [statusOptions]="eventTypeOptions"
                  [showSource]="false"
                  [showDateRange]="true"
                  (changed)="onFilters($event)" />

  <app-data-table [columns]="columns"
                  [rows]="svc.rows()"
                  [loading]="svc.loading()"
                  [error]="svc.error()"
                  emptyMessage="No activity for this site yet."
                  [sortBy]="svc.query.sortBy"
                  [sortDir]="svc.query.sortDir"
                  [page]="svc.meta()?.page ?? 1"
                  [pageSize]="svc.meta()?.page_size ?? 20"
                  [totalCount]="svc.meta()?.total_count ?? 0"
                  [totalPages]="svc.meta()?.total_pages ?? 1"
                  (sortChange)="onSort($event)"
                  (pageChange)="svc.setPage($event)"
                  (pageSizeChange)="svc.setPageSize($event)"
                  (retry)="svc.load()">

    <ng-template dtCell="ts" let-row>
      {{ row.ts | date: 'medium' }}
    </ng-template>

    <ng-template dtCell="event_type" let-row>
      <span class="al-evt">{{ eventLabel(row.event_type) }}</span>
    </ng-template>

    <ng-template dtCell="entity" let-row>
      <span class="al-entity">{{ row.entity ?? '—' }}</span>
      <span class="al-tag">{{ row.entity_type }}</span>
    </ng-template>

    <ng-template dtCell="transition" let-row>
      @if (row.old_state && row.new_state) {
      <span class="al-transition">
        <app-status-badge [status]="row.old_state" />
        <span class="al-arrow">→</span>
        <app-status-badge [status]="row.new_state" />
      </span>
      } @else if (row.new_state) {
      <app-status-badge [status]="row.new_state" />
      } @else {
      <span class="al-muted">—</span>
      }
    </ng-template>

    <ng-template dtCell="actor" let-row>
      <span class="al-actor">{{ row.actor }}</span>
    </ng-template>
  </app-data-table>
</section>
````

## File: docanalytics-web/src/app/features/auth/login.component.css
````css
.login-wrap {
  min-height: 100vh;
  display: grid;
  place-items: center;
  background: var(--purple-900);
  padding: 24px;
}

.login-card {
  width: 100%;
  max-width: 380px;
  background: #fff;
  border: 1px solid var(--purple-200, #e6dbf0);
  border-radius: 14px;
  padding: 32px 28px;
  box-shadow: 0 10px 30px rgba(61, 17, 82, 0.08);
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.brand {
  margin: 0;
  color: var(--purple-900, #3d1152);
  font-size: 1.6rem;
}

.subtitle {
  margin: 0 0 8px;
  color: var(--muted, #6b6480);
  font-size: 0.9rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.label {
  font-size: 0.82rem;
  font-weight: 600;
  color: var(--ink, #1a1430);
}

input {
  padding: 10px 12px;
  border: 1px solid var(--line, #ece8f1);
  border-radius: 8px;
  font-size: 0.95rem;
  outline: none;
  transition: border-color 0.15s;
}

  input:focus {
    border-color: var(--purple-500, #7c3aed);
  }

  input.invalid {
    border-color: #d92d20;
  }

.hint {
  font-size: 0.78rem;
  color: #d92d20;
}

.alert {
  background: #fef3f2;
  border: 1px solid #fda29b;
  color: #b42318;
  padding: 10px 12px;
  border-radius: 8px;
  font-size: 0.86rem;
}

.btn {
  margin-top: 6px;
  padding: 11px 16px;
  border: none;
  border-radius: 8px;
  background: var(--purple-900, #3d1152);
  color: #fff;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: background 0.15s;
}

  .btn:hover:not(:disabled) {
    background: var(--purple-700, #5b2580);
  }

  .btn:disabled {
    opacity: 0.7;
    cursor: not-allowed;
  }

.spinner {
  width: 15px;
  height: 15px;
  border: 2px solid rgba(255,255,255,0.5);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
````

## File: docanalytics-web/src/app/features/auth/login.component.html
````html
<div class="login-wrap">
  <form class="login-card" [formGroup]="form" (ngSubmit)="submit()" novalidate>
    <h1 class="brand">DocAnalytics</h1>
    <p class="subtitle">Sign in to your monitoring dashboard</p>

    <!-- top-level error (bad creds / server / network) -->
    @if (errorMessage()) {
    <div class="alert" role="alert">{{ errorMessage() }}</div>
    }

    <label class="field">
      <span class="label">Email</span>
      <input type="email"
             formControlName="email"
             autocomplete="username"
             placeholder="you@company.com"
             [class.invalid]="isInvalid('email')" />
      @if (isInvalid('email')) {
      <span class="hint">
        @if (form.controls.email.hasError('required')) { Email is required. }
        @else { Enter a valid email address. }
      </span>
      }
    </label>

    <label class="field">
      <span class="label">Password</span>
      <input type="password"
             formControlName="password"
             autocomplete="current-password"
             placeholder="••••••••"
             [class.invalid]="isInvalid('password')" />
      @if (isInvalid('password')) {
      <span class="hint">Password is required.</span>
      }
    </label>

    <!-- Swap this for Shubh's <app-button> once the Round-1 atom merges. -->
    <button type="submit" class="btn" [disabled]="loading()">
      @if (loading()) { <span class="spinner" aria-hidden="true"></span> Signing in… }
      @else { Sign in }
    </button>
  </form>
</div>
````

## File: docanalytics-web/src/app/features/batches/batch-detail/batch-detail.component.css
````css
.batch {
  display: flex;
  flex-direction: column;
  gap: var(--space-3, 24px);
  padding: var(--space-3, 24px);
}

.bd-back {
  display: inline-block;
  font-size: .85rem;
}

.head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--space-2);
  flex-wrap: wrap;
}

.eyebrow {
  margin: 0;
  font-size: 0.72rem;
  text-transform: uppercase;
  letter-spacing: .04em;
  color: var(--dark-gray-3);
}

.page-title {
  font-family: var(--font-display);
  color: var(--dark-gray);
  margin: 2px 0 0;
  font-size: 1.1rem;
  word-break: break-all;
}

.source {
  margin: 4px 0 0;
  font-size: 0.82rem;
  color: var(--dark-gray-3);
}

.section-title {
  font-family: var(--font-display);
  font-size: 1.05rem;
  color: var(--dark-gray);
  margin: 0;
}

.counters {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: var(--space-2, 16px);
}

.times {
  margin: 0;
  font-size: 0.8rem;
  color: var(--dark-gray-3);
}

.inline-error {
  color: var(--text-error);
  font-size: 0.85rem;
}

  .inline-error button {
    margin-left: 8px;
  }

/* skeleton shimmer (tokens → auto-flips in dark) */
.skel {
  background: linear-gradient(90deg, var(--light-gray) 25%, var(--cool-gray) 37%, var(--light-gray) 63%);
  background-size: 400% 100%;
  animation: skel 1.4s ease infinite;
  border-radius: 4px;
}

@keyframes skel {
  0% {
    background-position: 100% 50%;
  }

  100% {
    background-position: 0 50%;
  }
}
````

## File: docanalytics-web/src/app/features/batches/batch-detail/batch-detail.component.html
````html
<section class="batch">
  <a class="bd-back" routerLink="..">← Back to batches</a>

  @if (batch.detailLoading()) {
  <!-- header skeleton -->
  <div class="head">
    <div class="titles">
      <div class="skel" style="height:11px;width:60px;"></div>
      <div class="skel" style="height:20px;width:280px;margin-top:8px;"></div>
      <div class="skel" style="height:13px;width:180px;margin-top:8px;"></div>
    </div>
    <div class="skel" style="height:26px;width:90px;border-radius:13px;"></div>
  </div>
  <div class="counters">
    <app-stat-card title="Uploaded" [loading]="true"></app-stat-card>
    <app-stat-card title="Processing" [loading]="true"></app-stat-card>
    <app-stat-card title="Completed" [loading]="true"></app-stat-card>
    <app-stat-card title="Failed" [loading]="true"></app-stat-card>
  </div>

  } @else if (batch.detailError()) {
  <p class="inline-error">
    {{ batch.detailError() }}
    <button type="button" (click)="batch.loadDetail()">Retry</button>
  </p>

  } @else if (batch.detail(); as d) {
  <!-- summary header -->
  <div class="head">
    <div class="titles">
      <p class="eyebrow">Batch</p>
      <h1 class="page-title">{{ d.id }}</h1>
      <p class="source">Source: {{ d.source }} · {{ d.total_files }} files</p>
    </div>
    <app-status-badge [status]="d.status" />
  </div>

  <!-- file_stats counters -->
  <div class="counters">
    <app-stat-card title="Uploaded" [value]="d.file_stats.uploaded"></app-stat-card>
    <app-stat-card title="Processing" [value]="d.file_stats.processing"></app-stat-card>
    <app-stat-card title="Completed" [value]="d.file_stats.completed"></app-stat-card>
    <app-stat-card title="Failed" [value]="d.file_stats.failed"></app-stat-card>
  </div>
  <p class="times">
    Submitted {{ d.times.submitted_at | date: 'short' }}
    · Updated {{ d.times.last_updated_at | date: 'short' }}
    @if (d.times.completed_at) { · Completed {{ d.times.completed_at | date: 'short' }} }
  </p>
  }

  <!-- nested files table (Akash's DataTable) — pagination only, no sort -->
  <h2 class="section-title">Files</h2>
  <app-data-table [columns]="fileColumns" [rows]="batch.files()" [clickable]="true" [rowId]="fileRowId"
                  [loading]="batch.filesLoading()" [error]="batch.filesError()"
                  emptyMessage="No files in this batch"
                  [page]="batch.filesQuery().page" [pageSize]="batch.filesQuery().pageSize"
                  [totalCount]="batch.filesMeta()?.total_count ?? 0" [totalPages]="batch.filesMeta()?.total_pages ?? 1"
                  (pageChange)="batch.setFilesPage($event)" (pageSizeChange)="batch.setFilesPageSize($event)"
                  (retry)="batch.loadFiles()" (rowClick)="openFile($event)">
    <ng-template dtCell="status" let-row><app-status-badge [status]="row.status" /></ng-template>
    <ng-template dtCell="created_at" let-row>{{ row.created_at | date: 'short' }}</ng-template>
  </app-data-table>
</section>
````

## File: docanalytics-web/src/app/features/batches/batch-detail/batch-detail.component.ts
````typescript
import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { BatchService } from '../batch.service';
import { BatchFile } from '../batch.models';
import { SiteContextService } from '../../../core/services/site-context.service';
import { StatCardComponent } from '../../../shared/components/stat-card/stat-card.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { ColumnDef, DataTableComponent, DtCellDirective } from '../../../shared/components/data-table/data-table.component';

@Component({
  selector: 'app-batch-detail',
  imports: [RouterLink, StatCardComponent, StatusBadgeComponent, DataTableComponent, DtCellDirective, DatePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './batch-detail.component.html',
  styleUrl: './batch-detail.component.css',
})
export class BatchDetailComponent {
  protected readonly batch = inject(BatchService);
  private readonly site = inject(SiteContextService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private readonly batchId = toSignal(
    this.route.paramMap.pipe(map(p => p.get('batchId'))), { initialValue: null as string | null });

  protected readonly fileColumns: ColumnDef<BatchFile>[] = [
    { key: 'file_name', header: 'File Name' },
    { key: 'file_type', header: 'Type', width: '80px' },
    { key: 'status', header: 'Status', width: '150px' },
    { key: 'current_step', header: 'Current Step', width: '140px' },
    { key: 'file_size_bytes', header: 'Size', align: 'right', width: '110px', value: (r) => this.formatSize(r.file_size_bytes) },
    { key: 'created_at', header: 'Created', align: 'right', width: '160px' },
  ];
  protected readonly fileRowId = (f: BatchFile) => f.id;

  constructor() {
    // re-fires on batch switch (param-only nav) AND on site switch — both guarded (R2 lesson)
    effect(() => {
      const id = this.batchId();
      const site = this.site.selectedSiteId();
      if (id && site) this.batch.load(id);
    });
  }

  private formatSize(bytes: number): string {
    if (!bytes) return '—';
    const kb = bytes / 1024;
    return kb < 1024 ? `${kb.toFixed(1)} KB` : `${(kb / 1024).toFixed(1)} MB`;
  }

  // navigate to /site/:siteId/batches/:batchId/files/:fileId (Akash's Round 4 route)
  protected openFile(f: BatchFile): void {
    this.router.navigate(['files', f.id], { relativeTo: this.route });
  }
}
````

## File: docanalytics-web/src/app/features/batches/batch-list.component.css
````css
.page {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  padding: var(--space-3);
}

.page-title {
  margin: 0;
  font-family: var(--font-display);
  color: var(--dark-gray);
}

.page-sub {
  margin: 4px 0 0;
  color: var(--dark-gray-3);
  font-size: 0.85rem;
}

.toolbar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: var(--space-2);
}

  .toolbar .search {
    flex: 1 1 220px;
    min-width: 200px;
  }

.search-input {
  height: 50px;
  min-width: 240px;
  padding: 0 10px;
  font: inherit;
  border: 1px solid var(--cool-gray);
  border-radius: 6px;
}

  .search-input:focus {
    outline: none;
    border-color: var(--slate-blue);
  }

.batch-link {
  color: var(--slate-blue);
  font-family: monospace;
  text-decoration: none;
}

  .batch-link:hover {
    text-decoration: underline;
  }
````

## File: docanalytics-web/src/app/features/batches/batch-list.component.html
````html
<section class="page">
  <header class="page-head">
    <h2 class="page-title">Batches</h2>
    <p class="page-sub">Browse and inspect document batches for this site.</p>
  </header>

  <div class="toolbar">
    <app-filter-bar [statusOptions]="statusOptions"
                    [sourceOptions]="sourceOptions()"
                    (changed)="onFilters($event)" />
    <input class="search-input" type="search" placeholder="Search by Batch ID…"
           [value]="svc.query().search ?? ''" (input)="onSearch($event)" />
  </div>

  <app-data-table [columns]="columns"
                  [rows]="svc.batches()"
                  [loading]="svc.loading()"
                  [error]="svc.error()"
                  emptyMessage="No batches match your filters."
                  [sortBy]="svc.query().sortBy"
                  [sortDir]="svc.query().sortDir"
                  [page]="svc.query().page"
                  [pageSize]="svc.query().pageSize"
                  [totalCount]="svc.meta()?.total_count ?? 0"
                  [totalPages]="svc.meta()?.total_pages ?? 1"
                  (sortChange)="onSort($event)"
                  (pageChange)="svc.setPage($event)"
                  (pageSizeChange)="svc.setPageSize($event)"
                  (retry)="svc.loadBatches()">

    <ng-template dtCell="transaction_id" let-row>
      <a class="batch-link" [routerLink]="[row.transaction_id]">{{ row.transaction_id }}</a>
    </ng-template>

    <ng-template dtCell="state" let-row>
      <app-status-badge [status]="row.state" />
    </ng-template>

    <ng-template dtCell="submitted_at" let-row>
      {{ row.submitted_at | date:'medium' }}
    </ng-template>

    <ng-template dtCell="last_updated" let-row>
      {{ row.last_updated_at | date:'medium' }}
    </ng-template>
  </app-data-table>
</section>
````

## File: docanalytics-web/src/app/features/dashboard/dashboard.component.css
````css
.dash {
  display: flex;
  flex-direction: column;
  gap: var(--space-3, 24px);
  padding: var(--space-3, 24px);
}

.dash-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: var(--space-2);
}

.page-title {
  font-family: var(--font-display);
  color: var(--dark-gray);
  margin: 0;
}

.section-title {
  font-family: var(--font-display);
  font-size: 1.05rem;
  color: var(--dark-gray);
  margin: 0;
}

.counters {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: var(--space-2);
}

.charts-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-2, 16px);
}

@media (max-width: 1280px) {
  .charts-grid {
    grid-template-columns: 1fr;
  }
}

.inline-error {
  color: var(--text-error);
  font-size: 0.85rem;
}

  .inline-error button {
    margin-left: 8px;
  }

.err-code {
  font-weight: 600;
  color: var(--dark-gray);
}

.err-msg {
  color: var(--dark-gray-3);
}
````

## File: docanalytics-web/src/app/features/dashboard/dashboard.component.html
````html
<section class="dash">
  <div class="dash-head">
    <h1 class="page-title">Dashboard</h1>
    <app-refresh-timer
      [lastUpdated]="dash.lastUpdated()"
      [intervalMs]="refreshMs"
      [busy]="busy()"
      (refresh)="dash.refreshAll()" />
  </div>

  <!-- FR-1.1 counters -->
  <div class="counters">
    <app-stat-card title="Queued"      [value]="dash.summary()?.queued ?? 0"></app-stat-card>
    <app-stat-card title="In Progress" [value]="dash.summary()?.in_progress ?? 0"></app-stat-card>
    <app-stat-card title="Completed"   [value]="dash.summary()?.completed ?? 0"></app-stat-card>
    <app-stat-card title="Failed"      [value]="dash.summary()?.failed ?? 0"></app-stat-card>
  </div>
  @if (dash.summaryError()) {
    <p class="inline-error">{{ dash.summaryError() }}
      <button type="button" (click)="dash.loadSummary()">Retry</button>
    </p>
  }

  <!-- FR-1.2 / FR-1.3 charts -->
  <div class="charts-grid">
    <app-throughput-chart
      [data]="dash.throughput()"
      [loading]="dash.throughputLoading()"
      [error]="dash.throughputError()"
      (retry)="dash.refreshAll()"/>
    <app-status-distribution-chart
      [data]="dash.statusDistribution()"
      [loading]="dash.distributionLoading()"
      [error]="dash.distributionError()"
      (retry)="dash.refreshAll()" />
  </div>

  <!-- FR-1.4 recent failures -->
  <h2 class="section-title">Recent Failures</h2>
  <app-data-table
    [columns]="columns"
    [rows]="dash.failures()"
    [loading]="dash.failuresLoading()"
    [error]="dash.failuresError()"
    emptyMessage="No recent failures 🎉"
    [sortBy]="dash.failuresQuery().sortBy"
    [sortDir]="dash.failuresQuery().sortDir"
    [page]="dash.failuresQuery().page"
    [pageSize]="dash.failuresQuery().pageSize"
    [totalCount]="dash.failuresMeta()?.total_count ?? 0"
    [totalPages]="dash.failuresMeta()?.total_pages ?? 1"
    (sortChange)="onSort($event)"
    (pageChange)="dash.setFailuresPage($event)"
    (pageSizeChange)="dash.setFailuresPageSize($event)"
    (retry)="dash.loadFailures()">

    <ng-template dtCell="error" let-row>
      <span class="err-code">{{ row.error_code || '—' }}</span>
      @if (row.error_message) { <span class="err-msg"> — {{ row.error_message }}</span> }
    </ng-template>

    <ng-template dtCell="failed_at" let-row>
      {{ row.failed_at | date: 'short' }}
    </ng-template>
  </app-data-table>
</section>
````

## File: docanalytics-web/src/app/features/dashboard/dashboard.models.ts
````typescript
// FR-1.1 — summary counters. ⚠️ VERIFY shape in Swagger (see checklist at the end):
// backend DashboardService returns a FLAT object via snake_case JSON.
export interface DashboardSummary {
  queued: number;
  in_progress: number;
  completed: number;
  failed: number;
  last_updated?: string; // present only if the backend sets it
}

// FR-1.4 — one row per failed step
export interface RecentFailure {
  file_id: string;
  file_name: string;
  failed_step: string;
  error_code?: string | null;
  error_message?: string | null;
  failed_at: string;
}

export type FailuresSortBy = 'failed_at' | 'file_name' | 'failed_step';

export interface RecentFailuresQuery {
  page: number;
  pageSize: number;
  sortBy: FailuresSortBy;
  sortDir: 'asc' | 'desc';
}
````

## File: docanalytics-web/src/app/features/dashboard/status-distribution-chart/status-distribution-chart.component.css
````css
.bars {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
  width: 100%;
  align-self: flex-start;
}

.row {
  display: grid;
  grid-template-columns: 110px 1fr 90px;
  align-items: center;
  gap: var(--space-1);
}

.label {
  font-size: 0.82rem;
  color: var(--dark-gray);
}

.track {
  background: var(--light-gray);
  border: 1px solid var(--cool-gray);
  border-radius: 6px;
  height: 18px;
  overflow: hidden;
}

.fill {
  height: 100%;
  border-radius: 6px 0 0 6px;
  transition: width .3s ease;
}

.val {
  font-size: 0.78rem;
  color: var(--dark-gray-3);
  text-align: right;
}
/* status colors = fills only (AVEVA rule) */
.st-completed {
  background: var(--status-confirmed);
}

.st-failed {
  background: var(--status-error);
}

.st-processing {
  background: var(--status-warning);
}

.st-queued {
  background: var(--cool-gray);
}
````

## File: docanalytics-web/src/app/features/errors/errors.component.css
````css
.page {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  padding: var(--space-3);
}

.page-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--space-2);
  flex-wrap: wrap;
}

.page-title {
  margin: 0;
  font-family: var(--font-display);
  color: var(--dark-gray);
}

.page-sub {
  margin: 4px 0 0;
  color: var(--dark-gray-3);
  font-size: 0.85rem;
}

.export-btn {
  height: 38px;
  padding: 0 16px;
  cursor: pointer;
  border-radius: 6px;
  border: 1px solid var(--slate-blue);
  background: var(--slate-blue);
  color: var(--white);
  font: inherit;
}

  .export-btn:disabled {
    opacity: .55;
    cursor: default;
  }

.inline-error {
  margin: 0;
  color: var(--text-error);
  font-size: 0.85rem;
}

.charts {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(340px, 1fr));
  gap: var(--space-2);
}

.bars {
  display: flex;
  flex-direction: column;
  gap: 8px;
  width: 100%;
}

.bar-row {
  display: grid;
  grid-template-columns: 190px 1fr 36px;
  align-items: center;
  gap: 10px;
}

.bar-label {
  font-size: 0.78rem;
  color: var(--dark-gray);
  font-family: monospace;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.bar-track {
  height: 14px;
  background: var(--bg-light);
  border-radius: 7px;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  background: var(--status-error);
  border-radius: 7px;
}

.bar-val {
  font-size: 0.78rem;
  color: var(--dark-gray-3);
  text-align: right;
}

.trend {
  display: flex;
  align-items: flex-end;
  gap: 6px;
  width: 100%;
  height: 180px;
}

.col {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-end;
  height: 100%;
}

.col-bar {
  width: 100%;
  min-height: 2px;
  background: var(--slate-blue);
  border-radius: 4px 4px 0 0;
}

.col-label {
  margin-top: 4px;
  font-size: 0.62rem;
  color: var(--dark-gray-3);
}

.col-val {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--dark-gray);
  margin-bottom: 2px;
}

.err-chip {
  font-family: monospace;
  font-size: 0.75rem;
  color: var(--text-error);
  background: var(--light-gray);
  padding: 2px 6px;
  border-radius: 4px;
}
````

## File: docanalytics-web/src/app/features/errors/errors.component.html
````html
<section class="page">
  <header class="page-head">
    <div>
      <h2 class="page-title">Error Analysis</h2>
      <p class="page-sub">Top failures, trend over time, and the full error log for this site.</p>
    </div>
    <button class="export-btn" type="button" [disabled]="svc.exporting()" (click)="svc.exportCsv()">
      {{ svc.exporting() ? 'Exporting…' : '⬇ Export CSV' }}
    </button>
  </header>

  @if (svc.exportError()) { <p class="inline-error">{{ svc.exportError() }}</p> }
  <div class="charts">
    <app-chart-card title="Top 10 Error Types" subtitle="Most frequent failure codes"
                    [loading]="svc.topLoading()" [error]="svc.topError()" [empty]="!svc.top().length"
                    emptyMessage="No errors recorded." (retry)="svc.loadTop()">
      <div class="bars">
        @for (p of svc.top(); track p.label) {
        <div class="bar-row">
          <span class="bar-label" [title]="p.label">{{ p.label }}</span>
          <div class="bar-track"><div class="bar-fill" [style.width.%]="pct(p.value, topMax())"></div></div>
          <span class="bar-val">{{ p.value }}</span>
        </div>
        }
      </div>
    </app-chart-card>

    <app-chart-card title="Error Trend" subtitle="Failures per day"
                    [loading]="svc.trendLoading()" [error]="svc.trendError()" [empty]="!svc.trend().length"
                    emptyMessage="No trend data." (retry)="svc.loadTrend()">
      <div class="trend">
        @for (p of svc.trend(); track p.label) {
        <div class="col" [title]="p.label + ': ' + p.value">
          <span class="col-val">{{ p.value }}</span>
          <div class="col-bar" [style.height.%]="pct(p.value, trendMax())"></div>
          <span class="col-label">{{ shortDate(p.label) }}</span>
        </div>
        }
      </div>
    </app-chart-card>
  </div>

  <app-filter-bar statusLabel="Step" [statusOptions]="stepOptions" [sourceOptions]="sourceOptions"
                  (changed)="onFilters($event)" />

  <app-data-table [columns]="columns" [rows]="svc.errors()" [loading]="svc.loading()" [error]="svc.error()"
                  emptyMessage="No errors match your filters."
                  [sortBy]="svc.query().sortBy" [sortDir]="svc.query().sortDir"
                  [page]="svc.query().page" [pageSize]="svc.query().pageSize"
                  [totalCount]="svc.meta()?.total_count ?? 0" [totalPages]="svc.meta()?.total_pages ?? 1"
                  (sortChange)="onSort($event)" (pageChange)="svc.setPage($event)"
                  (pageSizeChange)="svc.setPageSize($event)" (retry)="svc.loadErrors()">
    <ng-template dtCell="failed_at" let-row>{{ row.failed_at | date:'medium' }}</ng-template>
    <ng-template dtCell="error_code" let-row><span class="err-chip">{{ row.error_code }}</span></ng-template>
  </app-data-table>
</section>
````

## File: docanalytics-web/src/app/features/files/file-details.component.css
````css
.fd {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
  max-width: 1100px;
}

.fd-back {
  font-size: .85rem;
}

.fd-card {
  background: var(--white);
  border: 1px solid var(--cool-gray);
  border-radius: 8px;
  padding: var(--space-2);
}

.fd-header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  gap: var(--space-2);
}

.fd-header-right {
  display: flex;
  align-items: center;
  gap: var(--space-1);
}

.fd-eyebrow {
  margin: 0;
  font-size: .72rem;
  text-transform: uppercase;
  letter-spacing: .04em;
  color: var(--dark-gray-3);
}

.fd-title {
  margin: 2px 0 0;
  font-family: var(--font-display);
  font-size: 1.15rem;
  color: var(--dark-gray);
}

.fd-sub {
  margin: 4px 0 0;
  font-size: .82rem;
  color: var(--dark-gray-3);
}

.fd-h2 {
  margin: 0 0 var(--space-1);
  font-size: .95rem;
  color: var(--dark-gray);
}

.fd-state {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  color: var(--dark-gray-3);
  font-size: .88rem;
  padding: var(--space-1) 0;
}

.fd-error {
  color: var(--text-error);
}

.fd-btn {
  border: 1px solid var(--cool-gray);
  background: var(--white);
  color: var(--slate-blue);
  border-radius: 6px;
  padding: 6px 12px;
  font-size: .82rem;
  cursor: pointer;
}

  .fd-btn:hover {
    border-color: var(--slate-blue);
  }

/* timeline */
.tl {
  list-style: none;
  margin: 0;
  padding: 0;
}

.tl-item {
  position: relative;
  padding: 0 0 var(--space-2) var(--space-3);
  border-left: 2px solid var(--cool-gray);
}

  .tl-item:last-child {
    border-left-color: transparent;
    padding-bottom: 0;
  }

.tl-dot {
  position: absolute;
  left: -7px;
  top: 2px;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: var(--cool-gray);
}

.dot-success {
  background: var(--status-success, #2e7d32);
}

.dot-failed {
  background: var(--status-error, #c62828);
}

.dot-processing {
  background: var(--slate-blue);
}

.tl-row {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  flex-wrap: wrap;
}

.tl-step {
  font-weight: 600;
  color: var(--dark-gray);
}

.tl-ts {
  font-size: .78rem;
  color: var(--dark-gray-3);
  margin-left: auto;
}

.chip {
  font-size: .72rem;
  padding: 2px 8px;
  border-radius: 999px;
}

.chip-success {
  background: #e6f4ea;
  color: #1e7e34;
}

.chip-failed {
  background: #fdecea;
  color: #c62828;
}

.chip-processing {
  background: #e8f0fe;
  color: #1a56b0;
}

.tl-err {
  margin-top: 6px;
  font-size: .82rem;
  color: var(--text-error);
}

.tl-fix {
  margin-top: 4px;
  color: var(--dark-gray-3);
}

/* invoice table */
.fd-scroll {
  overflow-x: auto;
}

  .fd-scroll .tbl {
    min-width: 640px;
  }

.tbl {
  width: 100%;
  border-collapse: collapse;
  font-size: .85rem;
}

  .tbl th, .tbl td {
    padding: 8px 10px;
    border-bottom: 1px solid var(--cool-gray);
    text-align: left;
  }

  .tbl th {
    color: var(--dark-gray-3);
    font-weight: 600;
  }

  .tbl .r {
    text-align: right;
  }

  .tbl tfoot td {
    border-top: 2px solid var(--cool-gray);
    border-bottom: none;
  }

.spinner {
  width: 14px;
  height: 14px;
  border: 2px solid var(--cool-gray);
  border-top-color: var(--slate-blue);
  border-radius: 50%;
  animation: sp .7s linear infinite;
}

@keyframes sp {
  to {
    transform: rotate(360deg);
  }
}
````

## File: docanalytics-web/src/app/features/files/file-details.component.html
````html
<section class="fd">
  <!-- back link -->
  <a class="fd-back" routerLink="../..">← Back to batch</a>

  <!-- ─────────── File info header (FR-2.5) ─────────── -->
  @if (svc.detailLoading()) {
  <div class="fd-card fd-state"><span class="spinner"></span> Loading file…</div>
  } @else if (svc.detailError()) {
  <div class="fd-card fd-state fd-error">
    {{ svc.detailError() }}
    <button class="fd-btn" (click)="svc.loadDetails()">Retry</button>
  </div>
  } @else if (info(); as fi) {
  <header class="fd-card fd-header">
    <div>
      <p class="fd-eyebrow">File</p>
      <h1 class="fd-title">{{ fi.name }}</h1>
      <p class="fd-sub">Current step: <strong>{{ fi.current_step }}</strong></p>
    </div>
    <div class="fd-header-right">
      <app-status-badge [status]="fi.current_status" />
      <button class="fd-btn" (click)="svc.downloadLogs()">Download Logs</button>
    </div>
  </header>

  <!-- ─────────── Step timeline (FR-2.5) ─────────── -->
  <div class="fd-card">
    <h2 class="fd-h2">Processing timeline</h2>
    @if (history().length === 0) {
    <div class="fd-state">No steps recorded for this file.</div>
    } @else {
    <ol class="tl">
      @for (s of history(); track $index) {
      <li class="tl-item" [class.is-failed]="isFailed(s)">
        <span class="tl-dot" [class]="'dot-' + stepClass(s)"></span>
        <div class="tl-body">
          <div class="tl-row">
            <span class="tl-step">{{ s.step }}</span>
            <span class="chip" [class]="'chip-' + stepClass(s)">{{ s.status }}</span>
            <span class="tl-ts">{{ s.ts ? (s.ts | date: 'medium') : '—' }}</span>
          </div>
          @if (s.error; as e) {
          <div class="tl-err">
            <div><strong>{{ e.code }}</strong>{{ e.message ? ' — ' + e.message : '' }}</div>
            @if (e.suggested_fix) {
            <div class="tl-fix">💡 Suggested fix: {{ e.suggested_fix }}</div>
            }
          </div>
          }
        </div>
      </li>
      }
    </ol>
    }
  </div>
  }

  <!-- ─────────── Invoice line items ─────────── -->
  <div class="fd-card">
    <h2 class="fd-h2">Invoice line items</h2>
    @if (svc.invoiceLoading()) {
    <div class="fd-state"><span class="spinner"></span> Loading line items…</div>
    } @else if (!svc.hasInvoice()) {
    <div class="fd-state">This file could not be found.</div>
    } @else if (svc.invoiceError()) {
    <div class="fd-state fd-error">
      {{ svc.invoiceError() }}
      <button class="fd-btn" (click)="svc.loadLineItems()">Retry</button>
    </div>
    } @else if (items().length === 0) {
    <div class="fd-state">No line items — this file has no extracted invoice items.</div>
    } @else {
    <div class="fd-scroll">
      <table class="tbl">
        <thead>
          <tr>
            <th class="r">#</th>
            <th>Description</th>
            <th>Category</th>
            <th class="r">Qty</th>
            <th class="r">Unit price</th>
            <th class="r">Line total</th>
            <th class="r">Confidence</th>
          </tr>
        </thead>
        <tbody>
          @for (li of items(); track li.line_number) {
          <tr>
            <td class="r">{{ li.line_number }}</td>
            <td>{{ li.description }}</td>
            <td>{{ li.category_name ?? 'Uncategorized' }}</td>
            <td class="r">{{ num(li.quantity, 3) }}</td>
            <td class="r">{{ num(li.unit_price, 2) }}</td>
            <td class="r">{{ num(li.line_total, 2) }}</td>
            <td class="r">{{ pct(li.confidence) }}</td>
          </tr>
          }
        </tbody>
        <tfoot>
          <tr>
            <td colspan="5" class="r"><strong>Grand total</strong></td>
            <td class="r"><strong>{{ num(svc.invoice()?.grand_total ?? 0, 2) }}</strong></td>
            <td></td>
          </tr>
        </tfoot>
      </table>
    </div>
    }
  </div>
</section>
````

## File: docanalytics-web/src/app/features/files/file-details.models.ts
````typescript
// ── GET /api/v1/files/{id}/details  → FileDetailDto (snake_case) ──
export interface FileInfo {
  id: string;
  name: string;
  current_status: string;   // Completed | Failed | Processing | Queued
  current_step: string;
}

export interface StepError {
  code: string;
  message: string | null;
  suggested_fix: string | null;   // ErrorCatalog.remediation_msg joined by code
}

export interface StepHistoryItem {
  step: string;                    // Upload | Validate | Transform | Load
  status: string;                  // Success | Failed | Processing
  ts: string | null;               // ISO-8601
  error?: StepError | null;        // present only on failed steps
}

export interface FileDetail {
  file_info: FileInfo;
  history: StepHistoryItem[];
}

// ── GET /api/v1/files/{id}/line-items → InvoiceDetailDto (404 if no invoice) ──
export interface InvoiceLineItem {
  line_number: number;
  description: string;
  quantity: number | null;
  unit_price: number | null;
  line_total: number | null;
  confidence: number | null;       // DECIMAL(4,3) → 0–0.999
  is_valid: boolean;
  category_code: string | null;    // null when uncategorized (LEFT join)
  category_name: string | null;
}

export interface InvoiceDetail {
  grand_total: number;
  items: InvoiceLineItem[];
}
````

## File: docanalytics-web/src/app/features/files/file-details.service.ts
````typescript
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { FileDetail, InvoiceDetail } from './file-details.models';

@Injectable({ providedIn: 'root' })
export class FileDetailsService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBase;
  // widgets render their own errors → opt out of the global toast
  private readonly silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  private _fileId: string | null = null;

  // ── details slice (FR-2.5) ──
  private _detail = signal<FileDetail | null>(null);
  private _detailLoading = signal(false);
  private _detailError = signal<string | null>(null);
  readonly detail = this._detail.asReadonly();
  readonly detailLoading = this._detailLoading.asReadonly();
  readonly detailError = this._detailError.asReadonly();

  // ── invoice line-items slice ──
  private _invoice = signal<InvoiceDetail | null>(null);
  private _invoiceLoading = signal(false);
  private _invoiceError = signal<string | null>(null);
  private _hasInvoice = signal(true);          // false on 404 (file has no invoice)
  readonly invoice = this._invoice.asReadonly();
  readonly invoiceLoading = this._invoiceLoading.asReadonly();
  readonly invoiceError = this._invoiceError.asReadonly();
  readonly hasInvoice = this._hasInvoice.asReadonly();

  /** Load both slices for a file. Called from the page effect on file/site switch. */
  load(fileId: string): void {
    this._fileId = fileId;
    this.loadDetails();
    this.loadLineItems();
  }

  loadDetails(): void {
    if (!this._fileId) return;
    this._detailLoading.set(true);
    this._detailError.set(null);
    this.http
      .get<ApiResponse<FileDetail>>(`${this.base}/files/${this._fileId}/details`, this.silent)
      .pipe(finalize(() => this._detailLoading.set(false)))
      .subscribe({
        next: (res) => this._detail.set(res.data),
        error: (err) => this._detailError.set(this.msg(err, 'Could not load file details.')),
      });
  }

  loadLineItems(): void {
    if (!this._fileId) return;
    this._invoiceLoading.set(true);
    this._invoiceError.set(null);
    this._hasInvoice.set(true);
    this.http
      .get<ApiResponse<InvoiceDetail>>(`${this.base}/files/${this._fileId}/line-items`, this.silent)
      .pipe(finalize(() => this._invoiceLoading.set(false)))
      .subscribe({
        next: (res) => this._invoice.set(res.data),
        error: (err) => {
          if (err?.status === 404) {
            // not an error — this file simply isn't an invoice
            this._hasInvoice.set(false);
            this._invoice.set(null);
          } else {
            this._invoiceError.set(this.msg(err, 'Could not load line items.'));
          }
        },
      });
  }

  /** Bonus (Failed-Files feature): download the step-by-step .txt trace. */
  downloadLogs(): void {
    if (!this._fileId) return;
    this.http
      .get(`${this.base}/files/${this._fileId}/logs`, {
        context: new HttpContext().set(SKIP_ERROR_TOAST, true),
        responseType: 'blob',
        observe: 'response',
      })
      .subscribe({
        next: (resp) => {
          const blob = resp.body!;
          const url = URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download =
            this.filenameFrom(resp.headers.get('content-disposition')) ??
            `file_${this._fileId}_log.txt`;
          a.click();
          URL.revokeObjectURL(url);
        },
      });
  }

  reset(): void {
    this._fileId = null;
    this._detail.set(null);
    this._detailError.set(null);
    this._invoice.set(null);
    this._invoiceError.set(null);
    this._hasInvoice.set(true);
  }

  private msg(err: any, fallback: string): string {
    return err?.error?.error?.message ?? fallback;
  }

  private filenameFrom(cd: string | null): string | null {
    if (!cd) return null;
    const m = /filename="?([^"]+)"?/i.exec(cd);
    return m ? m[1] : null;
  }
}
````

## File: docanalytics-web/src/app/layout/shell/shell.component.css
````css
.shell {
  display: grid;
  grid-template-columns: 220px 1fr;
  height: 100vh;
}

/* Sidebar = brand chrome → AVEVA purple is allowed here */
.sidebar {
  background: var(--aveva-purple);
  color: #fff;
  padding: 16px 12px;
}

.brand {
  font-family: var(--font-display);
  font-weight: 700;
  font-size: 18px;
  padding: 8px 12px 20px;
}

nav {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

  nav a {
    font-family: var(--font-display);
    font-weight: 600;
    font-size: 16px;
    color: rgba(255,255,255,.75);
    padding: 10px 12px;
    border-radius: 8px;
  }

    nav a:hover {
      background: var(--purple);
      color: #fff;
    }

    nav a.active {
      background: var(--purple);
      color: #fff;
    }
/* active tab — allowed */

.main {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

/* Topbar = white surface (NOT purple) */
.topbar {
  height: 64px;
  display: flex;
  align-items: center;
  gap: 12px;
  background: var(--white);
  border-bottom: 1px solid var(--cool-gray);
  padding: 0 24px;
}

.spacer {
  flex: 1;
}

.user {
  color: var(--dark-gray-3);
  font-size: 14px;
}

/* Log out = ghost/secondary button (slate-blue, not purple) */
.logout-btn {
  font-family: var(--font-display);
  font-weight: 600;
  font-size: 14px;
  background: transparent;
  border: 1px solid var(--slate-blue);
  color: var(--slate-blue);
  border-radius: 4px;
  padding: 8px 16px;
  cursor: pointer;
  transition: all .15s;
}

  .logout-btn:hover {
    background: var(--slate-blue);
    color: #fff;
  }

.theme-toggle {
  background: transparent;
  border: 1px solid var(--cool-gray);
  color: var(--dark-gray-3);
  border-radius: 4px;
  width: 36px;
  height: 36px;
  cursor: pointer;
  font-size: 16px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all .15s;
}

  .theme-toggle:hover {
    border-color: var(--slate-blue);
    color: var(--slate-blue);
  }

/* Content canvas = light page bg, centered, capped for wide screens */
.content {
  flex: 1;
  overflow: auto;
  background: var(--bg-light);
  width: 100%;
  max-width: 1760px;
  margin-inline: auto; /* 👈 B-centered */
  padding: 24px;
}

@media (max-width: 1280px) {
  .content {
    padding: 16px;
  }
}
/* 👈 tighter on smaller widths */

/* Toasts — left color bar + filled icon handled in template; bg is dark per Notification Bar spec */
.toasts {
  position: fixed;
  bottom: 20px;
  right: 20px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  z-index: 1000;
}

.toast {
  font-family: var(--font-body);
  font-weight: 600;
  font-size: 14px;
  background: rgba(0,0,0,.85);
  color: #fff;
  padding: 12px 16px;
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0,0,0,.2);
  display: flex;
  gap: 12px;
  align-items: center;
  border-left: 4px solid var(--status-neutral); /* default = info */
}

  .toast.success {
    border-left-color: var(--status-confirmed);
  }

  .toast.error {
    border-left-color: var(--status-error);
  }

  .toast.warning {
    border-left-color: var(--status-warning);
  }

  .toast button {
    background: transparent;
    border: none;
    color: #fff;
    cursor: pointer;
    font-size: 16px;
    line-height: 1;
  }
````

## File: docanalytics-web/src/app/layout/shell/shell.component.html
````html
<div class="shell">
  <aside class="sidebar">
    <div class="brand">
      <img src="AVEVA_Logo_color_RGB.png" alt="AVEVA"
           style="width: 130px; height: auto; display: block; filter: brightness(0) invert(1);" />
      <div style="font-family: var(--font-body); font-size: 0.8rem; letter-spacing: .04em; color: #fff; opacity: .85; margin-top: 6px;">DocAnalytics</div>
    </div>

    <nav>
      <a [routerLink]="link('dashboard')" routerLinkActive="active">Dashboard</a>
      <a [routerLink]="link('batches')" routerLinkActive="active">Batches</a>
      <a [routerLink]="link('errors')" routerLinkActive="active">Errors</a>
      <a [routerLink]="link('activity-log')" routerLinkActive="active">Activity Log</a>
    </nav>
  </aside>

  <div class="main">
    <header class="topbar">
      <app-site-selector />
      <span class="spacer"></span>
      <button class="theme-toggle" type="button" (click)="theme.toggle()"
              [attr.aria-pressed]="theme.isDark()"
              [attr.aria-label]="theme.isDark() ? 'Switch to light mode' : 'Switch to dark mode'"
              title="Toggle theme">
        {{ theme.isDark() ? '☀️' : '🌙' }}
      </button>
      <span class="user">{{ user()?.role ?? 'Viewer' }}</span>
      <button class="logout-btn" (click)="logout()">Log out</button>
    </header>
    <main class="content"><router-outlet /></main>
  </div>

  <!-- global toast outlet -->
  <div class="toasts">
    @for (t of toast.toasts(); track t.id) {
    <div class="toast"
         [class.error]="t.type === 'error'"
         [class.success]="t.type === 'success'"
         [class.warning]="t.type === 'warning'">
      <span class="material-icons" aria-hidden="true">{{ icon(t.type) }}</span>
      <span class="toast-text">{{ t.text }}</span>
      <button (click)="toast.dismiss(t.id)">×</button>
    </div>
    }
  </div>
</div>
````

## File: docanalytics-web/src/app/layout/shell/shell.component.ts
````typescript
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { SiteContextService } from '../../core/services/site-context.service';
import { ToastService } from '../../core/services/toast.service';
import { AuthService } from '../../core/services/auth.service';
import { SiteSelectorComponent } from '../../shared/components/site-selector/site-selector.component';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, SiteSelectorComponent],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.css',
})
export class ShellComponent {
  private route = inject(ActivatedRoute);
  private siteCtx = inject(SiteContextService);
  private auth = inject(AuthService);
  private router = inject(Router);
  toast = inject(ToastService);
  protected theme = inject(ThemeService);

  // current :siteId from the URL, exposed as a signal for the template
  siteId = toSignal(this.route.paramMap.pipe(map(p => p.get('siteId'))), { initialValue: null });

  // current logged-in user (signal from AuthService) — drives the role label
  readonly user = this.auth.currentUser;

  constructor() {
    // mirror the :siteId URL param into the global service (DT-3 design)
    this.route.paramMap
      .pipe(takeUntilDestroyed())
      .subscribe(p => this.siteCtx.setSite(p.get('siteId')));
  }

  link(page: string) {
    return ['/site', this.siteId(), page];
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  icon(type: string): string {
    switch (type) {
      case 'success': return 'check_circle';
      case 'warning': return 'warning';
      case 'error': return 'error';
      default: return 'info';
    }
  }
}
````

## File: docanalytics-web/src/app/shared/components/app-button/app-button.component.css
````css
.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-family: var(--font-display);
  font-weight: 600;
  font-size: 18px;
  background: var(--slate-blue);
  color: #fff;
  border: none;
  border-radius: 4px;
  padding: 13px 32px;
  cursor: pointer;
}

  .btn:hover:not(:disabled) {
    background: #3f4fc4;
  }
  /* one shade darker */
  .btn:disabled {
    background: var(--cool-gray);
    color: #fff;
    cursor: not-allowed;
  }

.spinner {
  width: 14px;
  height: 14px;
  border: 2px solid rgba(255,255,255,.4);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin .6s linear infinite reverse; /* counter-clockwise */
}

@media (max-width: 1024px) {
  .btn {
    font-size: 14px;
  }
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
````

## File: docanalytics-web/src/app/shared/components/app-button/app-button.component.html
````html
<button class="btn" [disabled]="disabled() || loading()" (click)="clicked.emit()">
  @if (loading()) { <span class="spinner"></span> }
  <ng-content />
</button>
````

## File: docanalytics-web/src/app/shared/components/app-button/app-button.component.ts
````typescript
import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  templateUrl: './app-button.component.html',
  styleUrl: './app-button.component.css',
})
export class AppButtonComponent {
  loading = input(false);
  disabled = input(false);
  clicked = output<void>();
}
````

## File: docanalytics-web/src/app/shared/components/chart-card/chart-card.component.css
````css
.chart-card {
  background: var(--white);
  border: 1px solid var(--cool-gray);
  border-radius: 8px;
  padding: var(--space-2);
  display: flex;
  flex-direction: column;
}

.cc-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--space-1);
  margin-bottom: var(--space-2);
}

.cc-title {
  margin: 0;
  font-family: var(--font-display);
  font-size: 1rem;
  font-weight: 600;
  color: var(--dark-gray);
}

.cc-sub {
  margin: 4px 0 0;
  font-size: 0.78rem;
  color: var(--dark-gray-3);
}

.cc-body {
  flex: 1;
  min-height: 180px;
  display: flex;
}

.cc-state {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-1);
  color: var(--dark-gray-3);
  font-size: 0.85rem;
}

.cc-error {
  color: var(--text-error);
  flex-direction: column;
}

.cc-retry {
  font: inherit;
  font-size: 0.8rem;
  cursor: pointer;
  background: transparent;
  border: 1px solid var(--slate-blue);
  color: var(--slate-blue);
  border-radius: 4px;
  padding: 4px 12px;
}

  .cc-retry:hover {
    background: var(--slate-blue);
    color: #fff;
  }

.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid var(--cool-gray);
  border-top-color: var(--slate-blue);
  border-radius: 50%;
  animation: cc-spin .7s linear infinite;
}

@keyframes cc-spin {
  to {
    transform: rotate(360deg);
  }
}
````

## File: docanalytics-web/src/app/shared/components/chart-card/chart-card.component.html
````html
<section class="chart-card">
  <header class="cc-head">
    <div class="cc-titles">
      <h3 class="cc-title">{{ title() }}</h3>
      @if (subtitle()) { <p class="cc-sub">{{ subtitle() }}</p> }
    </div>
    <div class="cc-actions"><ng-content select="[card-actions]" /></div>
  </header>

  <div class="cc-body">
    @if (loading()) {
    <div class="cc-state"><span class="spinner"></span><span>Loading…</span></div>
    } @else if (error()) {
    <div class="cc-state cc-error">
      <span>⚠️ {{ error() }}</span>
      <button type="button" class="cc-retry" (click)="retry.emit()">Retry</button>
    </div>
    } @else if (empty()) {
    <div class="cc-state">{{ emptyMessage() }}</div>
    } @else {
    <ng-content />
    }
  </div>
</section>
````

## File: docanalytics-web/src/app/shared/components/chart-card/chart-card.component.ts
````typescript
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  selector: 'app-chart-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './chart-card.component.html',
  styleUrl: './chart-card.component.css',
})
export class ChartCardComponent {
  title = input.required<string>();
  subtitle = input<string>('');
  loading = input<boolean>(false);
  error = input<string | null>(null);
  empty = input<boolean>(false);
  emptyMessage = input<string>('No data to display');
  retry = output<void>();   // NEW
}
````

## File: docanalytics-web/src/app/shared/components/data-table/data-table.component.css
````css
.dt {
  background: var(--white);
  border: 1px solid var(--cool-gray);
  border-radius: 8px;
  overflow: hidden;
}

.dt-scroll {
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}

  .dt-scroll table {
    width: 100%;
    min-width: 720px;
    border-collapse: collapse;
  }

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.85rem;
}

thead th {
  text-align: left;
  font-family: var(--font-display);
  font-weight: 600;
  color: var(--dark-gray-3);
  background: var(--bg-light);
  padding: 10px 14px;
  border-bottom: 1px solid var(--cool-gray);
  white-space: nowrap;
  user-select: none;
}

th.sortable {
  cursor: pointer;
}

  th.sortable:hover {
    color: var(--slate-blue);
  }

.arrow {
  margin-left: 4px;
  font-size: 0.7rem;
  color: var(--slate-blue);
}

tbody td {
  padding: 10px 14px;
  border-bottom: 1px solid var(--bg-light);
  color: var(--dark-gray);
  vertical-align: top;
}

tbody tr.clickable {
  cursor: pointer;
}

  tbody tr.clickable:hover {
    background: var(--bg-light);
  }

.state {
  text-align: center;
  padding: 28px 14px;
  color: var(--dark-gray-3);
}

  .state.error {
    color: var(--text-error);
  }

.retry {
  margin-left: 10px;
  border: 1px solid var(--slate-blue);
  background: transparent;
  color: var(--slate-blue);
  border-radius: 6px;
  padding: 4px 12px;
  cursor: pointer;
}

.skeleton .bar {
  display: block;
  height: 12px;
  border-radius: 4px;
  background: linear-gradient(90deg, var(--bg-light), var(--cool-gray), var(--bg-light));
  background-size: 200% 100%;
  animation: dt-shimmer 1.2s infinite;
}

@keyframes dt-shimmer {
  to {
    background-position: -200% 0;
  }
}

.dt-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 14px;
  background: var(--bg-light);
  font-size: 0.8rem;
  color: var(--dark-gray-3);
}

.pager {
  display: flex;
  align-items: center;
  gap: 10px;
}

  .pager button {
    border: 1px solid var(--cool-gray);
    background: var(--white);
    border-radius: 6px;
    padding: 4px 10px;
    cursor: pointer;
    color: var(--dark-gray);
  }

    .pager button:disabled {
      opacity: 0.45;
      cursor: default;
    }

.psize select {
  margin-left: 4px;
  border: 1px solid var(--cool-gray);
  border-radius: 6px;
  padding: 2px 4px;
}
````

## File: docanalytics-web/src/app/shared/components/data-table/data-table.component.html
````html
<div class="dt-scroll">
  <table>
    <thead>
      <tr>
        @for (col of columns(); track col.key) {
        <th [style.width]="col.width"
            [class.sortable]="col.sortable"
            [style.text-align]="col.align || 'left'"
            (click)="onHeaderClick(col)">
          <span>{{ col.header }}</span>
          @if (col.sortable && sortBy() === col.key) {
          <span class="arrow">{{ sortDir() === 'asc' ? '▲' : '▼' }}</span>
          }
        </th>
        }
      </tr>
    </thead>

    <tbody>
      @if (error()) {
      <tr>
        <td [attr.colspan]="columns().length" class="state error">
          <span>{{ error() }}</span>
          <button type="button" class="retry" (click)="retry.emit()">Retry</button>
        </td>
      </tr>
      } @else if (loading()) {
      @for (r of skeletonRows; track r) {
      <tr class="skeleton">
        @for (col of columns(); track col.key) {
        <td><span class="bar"></span></td> }
      </tr>
      }
      } @else if (!rows().length) {
      <tr><td [attr.colspan]="columns().length" class="state empty">{{ emptyMessage() }}</td></tr>
      } @else {
      @for (row of rows(); track key(row, $index)) {
      <tr (click)="rowClick.emit(row)" [class.clickable]="clickable()">
        @for (col of columns(); track col.key) {
        <td [style.text-align]="col.align || 'left'">
          @if (cellTemplates().get(col.key); as tpl) {
          <ng-container [ngTemplateOutlet]="tpl"
                        [ngTemplateOutletContext]="{ $implicit: row, row }" />
          } @else {
          {{ display(row, col) }}
          }
        </td>
        }
      </tr>
      }
      }
    </tbody>
  </table>

  @if (!loading() && !error() && rows().length) {
  <div class="dt-footer">
    <span class="count">{{ totalCount() }} records</span>
    <div class="pager">
      <label class="psize">
        Rows:
        <select [value]="pageSize()" (change)="onPageSize($event)">
          @for (n of pageSizeOptions(); track n) {
          <option [value]="n" [selected]="n === pageSize()">{{ n }}</option> }
        </select>
      </label>
      <button type="button" [disabled]="page() <= 1" (click)="prev()">‹ Prev</button>
      <span class="page-info">Page {{ page() }} of {{ totalPages() }}</span>
      <button type="button" [disabled]="page() >= totalPages()" (click)="next()">Next ›</button>
    </div>
  </div>
  }
</div>
````

## File: docanalytics-web/src/app/shared/components/data-table/data-table.component.ts
````typescript
import {
  Component, Directive, TemplateRef, computed, contentChildren, inject, input, output,
} from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';

export type SortDir = 'asc' | 'desc';
export interface SortState { sortBy: string; sortDir: SortDir; }

export interface ColumnDef<T = any> {
  key: string;                 // property name OR a unique key when you use a cell template
  header: string;
  sortable?: boolean;
  align?: 'left' | 'right' | 'center';
  width?: string;              // e.g. '160px'
  value?: (row: T) => unknown; // optional accessor for computed/nested values
}

/** Override any column's cell:  <ng-template dtCell="error_message" let-row>...</ng-template> */
@Directive({ selector: 'ng-template[dtCell]' })
export class DtCellDirective {
  readonly dtCell = input.required<string>();
  readonly template = inject(TemplateRef);
}

@Component({
  selector: 'app-data-table',
  imports: [NgTemplateOutlet],

  templateUrl: './data-table.component.html',
  styleUrl: './data-table.component.css',
})
export class DataTableComponent<T = any> {
  readonly columns = input.required<ColumnDef<T>[]>();
  readonly rows = input<T[]>([]);
  readonly loading = input(false);
  readonly error = input<string | null>(null);
  readonly emptyMessage = input('No records to display.');

  readonly sortBy = input<string | null>(null);
  readonly sortDir = input<SortDir>('desc');

  readonly page = input(1);
  readonly pageSize = input(10);
  readonly totalCount = input(0);
  readonly totalPages = input(1);
  readonly pageSizeOptions = input<number[]>([10, 20, 50]);

  readonly clickable = input(false);
  readonly rowId = input<((row: T) => string | number) | null>(null);

  readonly sortChange = output<SortState>();
  readonly pageChange = output<number>();
  readonly pageSizeChange = output<number>();
  readonly retry = output<void>();
  readonly rowClick = output<T>();

  protected readonly skeletonRows = [0, 1, 2, 3, 4];

  private readonly cells = contentChildren(DtCellDirective);
  protected readonly cellTemplates = computed(() => {
    const map = new Map<string, TemplateRef<any>>();
    for (const c of this.cells()) map.set(c.dtCell(), c.template);
    return map;
  });

  protected display(row: T, col: ColumnDef<T>): unknown {
    return col.value ? col.value(row) : (row as any)[col.key];
  }
  protected key(row: T, i: number): string | number {
    const fn = this.rowId();
    return fn ? fn(row) : i;
  }
  protected onHeaderClick(col: ColumnDef<T>): void {
    if (!col.sortable) return;
    const same = this.sortBy() === col.key;
    const dir: SortDir = same ? (this.sortDir() === 'asc' ? 'desc' : 'asc') : 'desc';
    this.sortChange.emit({ sortBy: col.key, sortDir: dir });
  }
  protected prev(): void { if (this.page() > 1) this.pageChange.emit(this.page() - 1); }
  protected next(): void { if (this.page() < this.totalPages()) this.pageChange.emit(this.page() + 1); }
  protected onPageSize(e: Event): void {
    this.pageSizeChange.emit(Number((e.target as HTMLSelectElement).value));
  }
}
````

## File: docanalytics-web/src/app/shared/components/filter-bar/filter-bar.component.css
````css
.fb {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-end;
  gap: var(--space-2);
  padding: var(--space-2);
  background: var(--white);
  border: 1px solid var(--cool-gray);
  border-radius: 8px;
}

.fb-field {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.fb-label {
  font-size: 0.72rem;
  color: var(--dark-gray-3);
  text-transform: uppercase;
  letter-spacing: .04em;
}

.fb-input {
  height: 34px;
  padding: 0 8px;
  font: inherit;
  color: var(--dark-gray);
  border: 1px solid var(--cool-gray);
  border-radius: 6px;
  background: var(--white);
}

  .fb-input:focus {
    outline: none;
    border-color: var(--slate-blue);
  }

.fb-clear {
  height: 34px;
  margin-left: auto;
  padding: 0 14px;
  cursor: pointer;
  border: 1px solid var(--cool-gray);
  border-radius: 6px;
  background: var(--bg-light);
  color: var(--dark-gray-3);
}

  .fb-clear:disabled {
    opacity: .5;
    cursor: default;
  }
````

## File: docanalytics-web/src/app/shared/components/filter-bar/filter-bar.component.html
````html
<div class="fb">
  <label class="fb-field">
    <span class="fb-label">{{ statusLabel() }}</span>
    <select class="fb-input" [value]="status()" (change)="onStatus($event)">
      @for (o of statusOptions(); track o.value) {
      <option [value]="o.value">{{ o.label }}</option>
      }
    </select>
  </label>

  @if (showSource()) {
  <label class="fb-field">
    <span class="fb-label">Source</span>
    <select class="fb-input" [value]="source() ?? ''" (change)="onSource($event)">
      <option value="">All sources</option>
      @for (o of sourceOptions(); track o.value) {
      <option [value]="o.value">{{ o.label }}</option>
      }
    </select>
  </label>
  }

  @if (showDateRange()) {
  <label class="fb-field">
    <span class="fb-label">From</span>
    <input class="fb-input" type="date" [value]="from() ?? ''" (change)="onFrom($event)" />
  </label>
  <label class="fb-field">
    <span class="fb-label">To</span>
    <input class="fb-input" type="date" [value]="to() ?? ''" (change)="onTo($event)" />
  </label>
  }

  <button class="fb-clear" type="button" (click)="clear()" [disabled]="!isDirty()">Clear</button>
</div>
````

## File: docanalytics-web/src/app/shared/components/filter-bar/filter-bar.component.ts
````typescript
import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';

export interface FilterOption { value: string; label: string; }

export interface FilterValues {
  status: string;         // all | in_progress | completed | failed
  source: string | null;  // null = all sources
  from: string | null;    // 'YYYY-MM-DD' or null
  to: string | null;      // 'YYYY-MM-DD' or null
}

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,

  templateUrl: './filter-bar.component.html',
  styleUrl: './filter-bar.component.css',
})
export class FilterBarComponent {
  statusLabel = input<string>('Status'); 
  statusOptions = input<FilterOption[]>([
    { value: 'all', label: 'All statuses' },
    { value: 'in_progress', label: 'In Progress' },
    { value: 'completed', label: 'Completed' },
    { value: 'failed', label: 'Failed' },
  ]);
  sourceOptions = input<FilterOption[]>([]);
  showSource = input<boolean>(true);
  showDateRange = input<boolean>(true);

  private _status = signal('all');
  private _source = signal<string | null>(null);
  private _from = signal<string | null>(null);
  private _to = signal<string | null>(null);

  status = this._status.asReadonly();
  source = this._source.asReadonly();
  from = this._from.asReadonly();
  to = this._to.asReadonly();

  isDirty = computed(() =>
    this._status() !== 'all' || !!this._source() || !!this._from() || !!this._to());

  changed = output<FilterValues>();

  private emit(): void {
    this.changed.emit({
      status: this._status(), source: this._source(),
      from: this._from(), to: this._to(),
    });
  }
  onStatus(e: Event) { this._status.set((e.target as HTMLSelectElement).value); this.emit(); }
  onSource(e: Event) { const v = (e.target as HTMLSelectElement).value; this._source.set(v || null); this.emit(); }
  onFrom(e: Event) { const v = (e.target as HTMLInputElement).value; this._from.set(v || null); this.emit(); }
  onTo(e: Event) { const v = (e.target as HTMLInputElement).value; this._to.set(v || null); this.emit(); }
  clear() { this._status.set('all'); this._source.set(null); this._from.set(null); this._to.set(null); this.emit(); }
}
````

## File: docanalytics-web/src/app/shared/components/refresh-timer/refresh-timer.component.css
````css
.rt {
  display: flex;
  align-items: center;
  gap: var(--space-1);
}

.stamp {
  font-size: 0.78rem;
  color: var(--dark-gray-3);
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-family: var(--font-display);
  font-size: 0.8rem;
  font-weight: 600;
  color: var(--slate-blue);
  background: transparent;
  border: 1px solid var(--slate-blue);
  border-radius: 6px;
  padding: 4px 10px;
  cursor: pointer;
}

  .btn:hover:not(:disabled) {
    background: var(--slate-blue);
    color: var(--white);
  }

  .btn:disabled {
    color: var(--cool-gray);
    border-color: var(--cool-gray);
    cursor: default;
  }

.material-icons {
  font-size: 16px;
}
````

## File: docanalytics-web/src/app/shared/components/refresh-timer/refresh-timer.component.html
````html
<div class="rt">
  <span class="stamp">{{ stampText() }}</span>
  <button type="button" class="btn" (click)="refresh.emit()" [disabled]="busy()">
    <span class="material-icons" aria-hidden="true">refresh</span>
    Refresh
  </button>
</div>
````

## File: docanalytics-web/src/app/shared/components/refresh-timer/refresh-timer.component.ts
````typescript
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-refresh-timer',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './refresh-timer.component.html',
  styleUrl: './refresh-timer.component.css',
})
export class RefreshTimerComponent {
  lastUpdated = input<Date | null>(null);
  intervalMs = input<number>(30_000);
  busy = input<boolean>(false);
  refresh = output<void>();

  private now = signal(Date.now());

  constructor() {
    const id = setInterval(() => this.now.set(Date.now()), 1000); // 1s ticker for the countdown
    inject(DestroyRef).onDestroy(() => clearInterval(id));
  }

  secondsLeft = computed(() => {
    const lu = this.lastUpdated();
    if (!lu) return null;
    const left = Math.ceil((this.intervalMs() - (this.now() - lu.getTime())) / 1000);
    return Math.max(0, left);
  });

  stampText = computed(() => {
    if (this.busy()) return 'Refreshing…';
    const s = this.secondsLeft();
    return s === null ? '' : `Refreshing in ${s}s`;
  });
}
````

## File: docanalytics-web/src/app/shared/components/site-selector/site-selector.component.css
````css
.dd {
  position: relative;
  display: inline-block;
  min-width: 220px;
}

/* ----- closed trigger ----- */
.dd-trigger {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-family: var(--font-body);
  font-size: 16px;
  color: var(--dark-gray);
  background: var(--white);
  border: 1px solid var(--cool-gray);
  border-radius: 4px;
  padding: 8px 12px;
  cursor: pointer;
}

  .dd-trigger:focus {
    outline: none;
    border-color: var(--slate-blue);
  }

  .dd-trigger.open {
    border-color: var(--slate-blue);
  }

.chevron {
  font-size: 20px;
  color: var(--dark-gray-3);
  transition: transform .15s;
}

.dd-trigger.open .chevron {
  transform: rotate(180deg);
}

/* ----- open list ----- */
.dd-list {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  right: 0;
  z-index: 50;
  margin: 0;
  padding: 4px 0;
  list-style: none;
  background: var(--white);
  border: 1px solid var(--cool-gray);
  border-radius: 4px;
  box-shadow: 0 4px 16px 0 rgba(0,0,0,.08);
  max-height: 280px;
  overflow-y: auto;
}

  .dd-list li {
    font-family: var(--font-body);
    font-size: 16px;
    color: var(--dark-gray);
    padding: 10px 12px;
    cursor: pointer;
  }
    /* AVEVA spec: hovered/active row = light gray */
    .dd-list li.active {
      background: var(--light-gray);
    }
    /* selected row = light gray + emphasis (no native blue) */
    .dd-list li.selected {
      background: var(--light-gray);
      font-weight: 600;
    }
````

## File: docanalytics-web/src/app/shared/components/site-selector/site-selector.component.html
````html
<div class="dd">
  <!-- trigger button (closed state) -->
  <button type="button" class="dd-trigger"
          [class.open]="open()"
          (click)="toggle()"
          [attr.aria-expanded]="open()"
          aria-haspopup="listbox">
    <span class="label">{{ currentName() }}</span>
    <span class="material-icons chevron" aria-hidden="true">expand_more</span>
  </button>

  <!-- options list -->
  @if (open()) {
  <ul class="dd-list" role="listbox" tabindex="-1">
    @for (s of sites(); track s.site_id; let i = $index) {
    <li role="option"
        [attr.aria-selected]="s.site_id === currentSiteId()"
        [class.selected]="s.site_id === currentSiteId()"
        [class.active]="i === activeIndex()"
        (click)="choose(s.site_id)"
        (mouseenter)="activeIndex.set(i)">
      {{ s.site_name }}
    </li>
    }
  </ul>
  }
</div>
````

## File: docanalytics-web/src/app/shared/components/site-selector/site-selector.component.ts
````typescript
import {
  Component, ElementRef, HostListener, computed, inject, signal,
} from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { SiteContextService } from '../../../core/services/site-context.service';

@Component({
  selector: 'app-site-selector',
  standalone: true,
  templateUrl: './site-selector.component.html',
  styleUrl: './site-selector.component.css',
})
export class SiteSelectorComponent {
  private auth = inject(AuthService);
  private siteCtx = inject(SiteContextService);
  private router = inject(Router);
  private host = inject(ElementRef);

  sites = this.auth.sites;
  currentSiteId = this.siteCtx.selectedSiteId;

  open = signal(false);
  activeIndex = signal(0);

  currentName = computed(() => {
    const id = this.currentSiteId();
    return this.sites().find(s => s.site_id === id)?.site_name ?? 'Select site';
  });

  toggle(): void {
    this.open.update(v => !v);
    if (this.open()) {
      // start the keyboard highlight on the currently-selected row
      const idx = this.sites().findIndex(s => s.site_id === this.currentSiteId());
      this.activeIndex.set(idx >= 0 ? idx : 0);
    }
  }

  choose(siteId: string): void {
    this.open.set(false);
    if (siteId === this.currentSiteId()) return;
    const url = this.router.url.replace(/\/site\/[^/]+/, `/site/${siteId}`);
    this.router.navigateByUrl(url);
  }

  // ----- click outside closes -----
  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent): void {
    if (!this.host.nativeElement.contains(e.target)) this.open.set(false);
  }

  // ----- keyboard nav -----
  @HostListener('keydown', ['$event'])
  onKey(e: KeyboardEvent): void {
    if (!this.open()) {
      if (e.key === 'Enter' || e.key === ' ' || e.key === 'ArrowDown') {
        e.preventDefault(); this.toggle();
      }
      return;
    }
    const last = this.sites().length - 1;
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        this.activeIndex.update(i => Math.min(i + 1, last));
        break;
      case 'ArrowUp':
        e.preventDefault();
        this.activeIndex.update(i => Math.max(i - 1, 0));
        break;
      case 'Enter':
        e.preventDefault();
        this.choose(this.sites()[this.activeIndex()].site_id);
        break;
      case 'Escape':
        this.open.set(false);
        break;
    }
  }
}
````

## File: docanalytics-web/src/app/shared/components/stat-card/stat-card.component.css
````css
.card {
  background: var(--white);
  border: 1px solid var(--cool-gray);
  border-radius: 6px;
  padding: 16px 18px;
  box-shadow: 0 2px 10px 0 rgba(0,0,0,.08);
  min-width: 160px;
}

.title {
  font-family: var(--font-body);
  color: var(--dark-gray-3);
  font-size: 12px;
  font-weight: 600;
  line-height: 16px;
  text-transform: uppercase;
  letter-spacing: 2px;
}

.value {
  font-family: var(--font-body);
  color: var(--dark-gray);
  font-size: 32px;
  line-height: 38px;
  font-weight: 700;
  margin-top: 8px;
}
/* skeleton (tokens → auto-flips in dark) */
.skel {
  background: linear-gradient(90deg, var(--light-gray) 25%, var(--cool-gray) 37%, var(--light-gray) 63%);
  background-size: 400% 100%;
  animation: skel 1.4s ease infinite;
  border-radius: 4px;
}

.skel-title {
  height: 12px;
  width: 60%;
}

.skel-value {
  height: 30px;
  width: 45%;
  margin-top: 10px;
}

@keyframes skel {
  0% {
    background-position: 100% 50%;
  }

  100% {
    background-position: 0 50%;
  }
}
````

## File: docanalytics-web/src/app/shared/components/stat-card/stat-card.component.html
````html
<div class="card">
  @if (loading()) {
  <div class="skel skel-title"></div>
  <div class="skel skel-value"></div>
  } @else {
  <div class="title">{{ title() }}</div>
  <div class="value">{{ value() }}</div>
  }
</div>
````

## File: docanalytics-web/src/app/shared/components/stat-card/stat-card.component.ts
````typescript
import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './stat-card.component.html',
  styleUrl: './stat-card.component.css',
})
export class StatCardComponent {
  title = input.required<string>();
  value = input<string | number>('');   // was input.required — now optional for skeleton use
  loading = input<boolean>(false);       // NEW
}
````

## File: docanalytics-web/src/app/shared/components/status-badge/status-badge.component.css
````css
.badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 3px 10px;
  border-radius: 999px;
  font-family: var(--font-body);
  font-size: 12px;
  font-weight: 600;
  line-height: 1.6;
  white-space: nowrap;
}

.material-icons {
  font-size: 14px;
  line-height: 1;
}
````

## File: docanalytics-web/src/app/shared/components/status-badge/status-badge.component.html
````html
<span class="badge" [style.background]="style().bg" [style.color]="style().fg">
  <span class="material-icons" aria-hidden="true">{{ style().icon }}</span>
  {{ status() }}
</span>
````

## File: docanalytics-web/src/app/shared/components/status-badge/status-badge.component.ts
````typescript
import { Component, computed, input } from '@angular/core';

type BadgeStyle = { bg: string; fg: string; icon: string };

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.css',
})
export class StatusBadgeComponent {
  status = input.required<string>();

  private key = computed(() => this.status().toLowerCase().replace(/[\s_]/g, ''));

  style = computed<BadgeStyle>(() => {
    switch (this.key()) {
      case 'completed':
      case 'success':
        // confirmed: tint fill + muted green text + check icon
        return { bg: 'rgba(0,152,72,.12)', fg: 'var(--text-confirmed)', icon: 'check_circle' };
      case 'failed':
      case 'error':
        return { bg: 'rgba(220,10,10,.12)', fg: 'var(--text-error)', icon: 'error' };
      case 'inprogress':
      case 'processing':
        // warning amber (token fg auto-flips: dark-brown in light, bright-amber in dark)
        return { bg: 'rgba(245,166,36,.15)', fg: 'var(--text-warning)', icon: 'pause_circle' };
      case 'queued':
      default:
        return { bg: 'rgba(190,204,214,.25)', fg: 'var(--dark-gray-3)', icon: 'schedule' };
    }
  });
}
````

## File: docanalytics-web/src/environments/environment.ts
````typescript
export const environment = {
  production: false,
  apiBase: '/api/v1', // relative — the dev proxy (step 8) forwards /api → backend
};
````

## File: docanalytics-web/src/main.ts
````typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
````

## File: docanalytics-web/tsconfig.app.json
````json
/* To learn more about Typescript configuration file: https://www.typescriptlang.org/docs/handbook/tsconfig-json.html. */
/* To learn more about Angular compiler options: https://angular.dev/reference/configs/angular-compiler-options. */
{
  "extends": "./tsconfig.json",
  "compilerOptions": {
    "outDir": "./out-tsc/app",
    "types": []
  },
  "include": [
    "src/**/*.ts"
  ],
  "exclude": [
    "src/**/*.spec.ts"
  ]
}
````

## File: docanalytics-web/tsconfig.json
````json
/* To learn more about Typescript configuration file: https://www.typescriptlang.org/docs/handbook/tsconfig-json.html. */
/* To learn more about Angular compiler options: https://angular.dev/reference/configs/angular-compiler-options. */
{
  "compileOnSave": false,
  "compilerOptions": {
    "noImplicitOverride": true,
    "noPropertyAccessFromIndexSignature": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true,
    "skipLibCheck": true,
    "isolatedModules": true,
    "experimentalDecorators": true,
    "importHelpers": true,
    "target": "ES2022",
    "module": "preserve"
  },
  "angularCompilerOptions": {
    "enableI18nLegacyMessageIdFormat": false,
    "strictInjectionParameters": true,
    "strictInputAccessModifiers": true
  },
  "files": [],
  "references": [
    {
      "path": "./tsconfig.app.json"
    },
    {
      "path": "./tsconfig.spec.json"
    }
  ]
}
````

## File: docanalytics-web/tsconfig.spec.json
````json
/* To learn more about Typescript configuration file: https://www.typescriptlang.org/docs/handbook/tsconfig-json.html. */
/* To learn more about Angular compiler options: https://angular.dev/reference/configs/angular-compiler-options. */
{
  "extends": "./tsconfig.json",
  "compilerOptions": {
    "outDir": "./out-tsc/spec",
    "types": [
      "vitest/globals"
    ]
  },
  "include": [
    "src/**/*.d.ts",
    "src/**/*.spec.ts"
  ]
}
````

## File: DocAnalytics.Api/Controllers/ActivityLogController.cs
````csharp
using DocAnalytics.Api.Common;            // ApiResponse<T>, Meta
using DocAnalytics.Service.ActivityLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/activity-log")]
[Tags("ActivityLog")]
public sealed class ActivityLogController : ControllerBase
{
    private readonly IActivityLogService _service;
    public ActivityLogController(IActivityLogService service) => _service = service;

    // GET /api/v1/activity-log — paginated, filtered audit trail (FR-4.1–FR-4.4)
    [HttpGet]
    public async Task<IActionResult> GetActivityLog([FromQuery] ActivityLogQuery query, CancellationToken ct)
    {
        var result = await _service.GetActivityLogAsync(query, ct);

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<ActivityLogItemDto>>.OkList(result.Items, meta));
    }
}
````

## File: DocAnalytics.Api/Controllers/BatchesController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Service.Batches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/batches")]
public sealed class BatchesController : ControllerBase
{
    private readonly IBatchService _batchService;

    // the service is injected (depends on the INTERFACE, not the class)
    public BatchesController(IBatchService batchService) => _batchService = batchService;

    [HttpGet]
    public async Task<IActionResult> GetBatches(
        [FromQuery] BatchListQuery query, CancellationToken ct)
    {
        // 1. delegate the real work to the service
        var result = await _batchService.GetBatchesAsync(query, ct);

        // 2. build the paging meta
        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        // 3. wrap in the standard envelope and return
        return Ok(ApiResponse<List<BatchListItemDto>>.OkList(result.Items, meta));
    }

    // GET /api/v1/batches/sources — distinct source systems for the FilterBar dropdown
    [HttpGet("sources")]
    public async Task<IActionResult> GetSources(CancellationToken ct)
    {
        var sources = await _batchService.GetSourcesAsync(ct);
        return Ok(ApiResponse<List<string>>.Ok(sources));
    }

    // GET /api/v1/batches/{id} — one batch's detail
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBatchById(Guid id, CancellationToken ct)
    {
        var batch = await _batchService.GetBatchByIdAsync(id, ct);

        if (batch is null)
            return NotFound(ApiResponse<BatchDetailDto>.Fail(
                "not_found", $"Batch '{id}' was not found."));

        return Ok(ApiResponse<BatchDetailDto>.Ok(batch));
    }

    // GET /api/v1/batches/{id}/files — paged list of a batch's files
    [HttpGet("{id:guid}/files")]
    public async Task<IActionResult> GetBatchFiles(
        Guid id, [FromQuery] BatchFilesQuery query, CancellationToken ct)
    {
        var result = await _batchService.GetBatchFilesAsync(id, query, ct);

        if (result is null)
            return NotFound(ApiResponse<List<BatchFileDto>>.Fail(
                "not_found", $"Batch '{id}' was not found."));

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<BatchFileDto>>.OkList(result.Items, meta));
    }

}
````

## File: DocAnalytics.Api/Controllers/ErrorsController.cs
````csharp
using System.Text;
using DocAnalytics.Api.Common;            // ApiResponse<T>, Meta  (drop if global usings)
using DocAnalytics.Service.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/errors")]
public sealed class ErrorsController : ControllerBase
{
    private readonly IErrorService _errors;
    public ErrorsController(IErrorService errors) => _errors = errors;

    // GET /api/v1/errors — filtered + paginated error list (FR-3.4)
    [HttpGet]
    public async Task<IActionResult> GetErrors([FromQuery] ErrorListQuery query, CancellationToken ct)
    {
        var result = await _errors.GetErrorsAsync(query, ct);

        var meta = new Meta
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };

        return Ok(ApiResponse<List<ErrorListItemDto>>.OkList(result.Items, meta));
    }

    // GET /api/v1/errors/export — CSV of the filtered list (FR-3.5)
    [HttpGet("export")]
    public async Task<IActionResult> ExportErrors([FromQuery] ErrorListQuery query, CancellationToken ct)
    {
        var rows = await _errors.GetErrorsForExportAsync(query, ct);
        var csv = ErrorCsvWriter.Write(rows);

        // UTF-8 BOM so Excel renders accents/symbols correctly
        var bytes = Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv))
            .ToArray();

        var fileName = $"errors_export_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

        // File(...) with a download name sets Content-Disposition: attachment automatically
        return File(bytes, "text/csv", fileName);
    }
}
````

## File: DocAnalytics.Api/Controllers/FilesController.cs
````csharp
using System.Text;
using DocAnalytics.Api.Common;            // ApiResponse<T>  (adjust if your namespace differs)
using DocAnalytics.Service.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/files")]
public sealed class FilesController : ControllerBase
{

    private readonly IFileDetailsService _service;
    public FilesController(IFileDetailsService service) => _service = service;

    // GET /api/v1/files/{id}/details
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetFileDetailsAsync(id, ct);
        if (dto is null)
            return NotFound(ApiResponse<FileDetailDto>.Fail("NOT_FOUND", "File not found."));
        return Ok(ApiResponse<FileDetailDto>.Ok(dto));
    }

    // GET /api/v1/files/{id}/logs  → downloads a .txt
    [HttpGet("{id:guid}/logs")]
    public async Task<IActionResult> GetLogs(Guid id, CancellationToken ct)
    {
        var log = await _service.GetFileLogsAsync(id, ct);
        if (log is null)
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "File not found."));
        return File(Encoding.UTF8.GetBytes(log.Content), "text/plain", log.FileName);
    }

    
}
````

## File: DocAnalytics.Api/Extensions/ApiServiceExtensions.cs
````csharp
using System.Text;
using DocAnalytics.Api.Auth;
using DocAnalytics.Api.Common;
using DocAnalytics.Domain.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace DocAnalytics.Api.Extensions;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
        return services;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
    {
        var settings = cfg.GetSection("Jwt").Get<JwtSettings>()!;
        services.Configure<JwtSettings>(cfg.GetSection("Jwt"));

        services.AddAuthentication("Bearer").AddJwtBearer("Bearer", o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
            };
        });
        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste ONLY your JWT (no 'Bearer ' prefix)."
            });

            options.AddSecurityDefinition("SiteId", new OpenApiSecurityScheme
            {
                Name = "X-Site-Id",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "Paste your site_id GUID once — applied to every request (tenant/site isolation)."
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() },
            { new OpenApiSecuritySchemeReference("SiteId", doc), new List<string>() }
        });
        });
        return services;
    }


}
````

## File: DocAnalytics.Api/Middleware/ExceptionHandlingMiddleware.cs
````csharp
using System.Text.Json;
using DocAnalytics.Api.Common;

namespace DocAnalytics.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    // Reuse one options instance; matches the global snake_case JSON policy.
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);   // run everything inside the pipeline
        }
        catch (Exception ex)
        {
            // Full detail goes to server logs only — never to the client.
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                ctx.Request.Method, ctx.Request.Path);

            // Can't rewrite a response that already started streaming.
            if (ctx.Response.HasStarted)
                throw;

            ctx.Response.Clear();
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";

            var body = ApiResponse<object>.Fail(
                "internal_error",
                "An unexpected error occurred. Please try again later.");

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOpts), ctx.RequestAborted);
        }
    }
}
````

## File: DocAnalytics.Data/Seeding/DbSeeder.cs
````csharp
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Data.Seeding;

public static class DbSeeder
{
    // ─────────────────────────────────────────────────────────────
    // FIXED IDs — stable across resets so tokens & site_id never go stale.
    // Login: Password123!  for every user below.
    //
    //  TENANTS
    //   Acme   : 11111111-1111-1111-1111-111111111111
    //   Globex : 22222222-2222-2222-2222-222222222222
    //  SITES (paste these into X-Site-Id)
    //   Acme/Mumbai  : a1111111-1111-1111-1111-111111111111
    //   Acme/Delhi   : a2222222-2222-2222-2222-222222222222
    //   Acme/Chennai : a3333333-3333-3333-3333-333333333333
    //   Globex/Berlin: b1111111-1111-1111-1111-111111111111
    //   Globex/Munich: b2222222-2222-2222-2222-222222222222
    //  USERS
    //   user.a@acme.com    (Viewer, Acme   — Mumbai + Delhi)
    //   admin@acme.com     (Admin,  Acme   — all 3 Acme sites: Mumbai, Delhi, Chennai)
    //   user.b@acme.com    (Viewer, Acme   — Chennai only)
    //   user.c@globex.com  (Viewer, Globex — Berlin only)
    //   admin@globex.com   (Admin,  Globex — both Globex sites: Berlin, Munich)
    // ─────────────────────────────────────────────────────────────

    private static readonly Guid AcmeId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid GlobexId = new("22222222-2222-2222-2222-222222222222");

    private static readonly Guid AcmeMumbai = new("a1111111-1111-1111-1111-111111111111");
    private static readonly Guid AcmeDelhi = new("a2222222-2222-2222-2222-222222222222");
    private static readonly Guid AcmeChennai = new("a3333333-3333-3333-3333-333333333333");
    private static readonly Guid GlobexBerlin = new("b1111111-1111-1111-1111-111111111111");
    private static readonly Guid GlobexMunich = new("b2222222-2222-2222-2222-222222222222");

    private static readonly Guid AcmeUserA = new("c1111111-1111-1111-1111-111111111111");
    private static readonly Guid AdminAcme = new("c2222222-2222-2222-2222-222222222222");
    private static readonly Guid GlobexUserC = new("c3333333-3333-3333-3333-333333333333");
    private static readonly Guid AcmeUserB = new("c4444444-4444-4444-4444-444444444444");
    private static readonly Guid AdminGlobex = new("c5555555-5555-5555-5555-555555555555");

    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();
        if (await db.Tenants.AnyAsync()) return; // idempotent guard

        var now = DateTime.UtcNow;
        var hash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var rng = new Random(20260625); // deterministic → reproducible data

        // ── Identity ───────────────────────────────────────────────
        var tenants = new[]
        {
            new Tenant { Id = AcmeId,   Name = "Acme Corp",  CreatedAt = now, IsActive = true },
            new Tenant { Id = GlobexId, Name = "Globex Inc", CreatedAt = now, IsActive = true },
        };

        var sites = new[]
        {
            new Site { Id = AcmeMumbai,   TenantId = AcmeId,   Name = "Mumbai Plant",    Location = "Mumbai, IN",  CreatedAt = now, IsActive = true },
            new Site { Id = AcmeDelhi,    TenantId = AcmeId,   Name = "Delhi Warehouse", Location = "Delhi, IN",   CreatedAt = now, IsActive = true },
            new Site { Id = AcmeChennai,  TenantId = AcmeId,   Name = "Chennai Hub",     Location = "Chennai, IN", CreatedAt = now, IsActive = true },
            new Site { Id = GlobexBerlin, TenantId = GlobexId, Name = "Berlin DC",       Location = "Berlin, DE",  CreatedAt = now, IsActive = true },
            new Site { Id = GlobexMunich, TenantId = GlobexId, Name = "Munich Plant",    Location = "Munich, DE",  CreatedAt = now, IsActive = true },
        };

        var users = new[]
        {
            new User { Id = AcmeUserA,   TenantId = AcmeId,   Email = "user.a@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AdminAcme,   TenantId = AcmeId,   Email = "admin@acme.com",    PasswordHash = hash, Role = "Admin",  CreatedAt = now, IsActive = true },
            new User { Id = AcmeUserB,   TenantId = AcmeId,   Email = "user.b@acme.com",   PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = GlobexUserC, TenantId = GlobexId, Email = "user.c@globex.com", PasswordHash = hash, Role = "Viewer", CreatedAt = now, IsActive = true },
            new User { Id = AdminGlobex, TenantId = GlobexId, Email = "admin@globex.com",  PasswordHash = hash, Role = "Admin",  CreatedAt = now, IsActive = true },

        };

        var access = new[]
        {
            // Acme
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserA,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserA,   SiteId = AcmeDelhi,    GrantedAt = now },  // user.a: Mumbai + Delhi
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeMumbai,   GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeDelhi,    GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminAcme,   SiteId = AcmeChennai,  GrantedAt = now },  // admin@acme: all 3
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AcmeUserB,   SiteId = AcmeChennai,  GrantedAt = now },  // user.b: Chennai only
            // Globex
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = GlobexUserC, SiteId = GlobexBerlin, GrantedAt = now },  // user.c: Berlin only
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminGlobex, SiteId = GlobexBerlin, GrantedAt = now },
            new UserSiteAccess { Id = Guid.NewGuid(), UserId = AdminGlobex, SiteId = GlobexMunich, GrantedAt = now },  // admin@globex: both

        };

        // ── Global catalogs ────────────────────────────────────────
        var docTypes = new[]
        {
            new DocumentType { Id = Guid.NewGuid(), TypeName = "Invoice",       Category = "PDF", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "Manifest",      Category = "CSV", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "PurchaseOrder", Category = "PDF", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "Receipt",       Category = "PDF", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "PackingSlip",   Category = "CSV", IsActive = true, CreatedAt = now },
            new DocumentType { Id = Guid.NewGuid(), TypeName = "BillOfLading",  Category = "PDF", IsActive = true, CreatedAt = now },
        };

        var categories = new[]
        {
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "GOODS",    CategoryName = "Goods",    IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "SERVICES", CategoryName = "Services", IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "SHIPPING", CategoryName = "Shipping", IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "TAX",      CategoryName = "Tax",      IsActive = true, CreatedAt = now },
            new ItemCategory { Id = Guid.NewGuid(), CategoryCode = "MISC",     CategoryName = "Misc",     IsActive = true, CreatedAt = now },
        };

        var errorDefs = new (string Code, string Desc, string Remediation, string Msg)[]
        {
            ("ERR_BAD_SCHEMA",        "CSV column headers do not match the expected schema.", "Check your column headers against the template.", "Unexpected column 'qty2'."),
            ("ERR_TIMEOUT",           "Processing step exceeded the allotted time.",          "Retry; if it persists, reduce the file size.",    "Step timed out after 300s."),
            ("ERR_CORRUPT_FILE",      "The uploaded file is corrupt or unreadable.",          "Re-export the document and upload again.",        "Unable to open file: unexpected EOF."),
            ("ERR_OCR_LOW_CONFIDENCE","Extraction confidence below the accepted threshold.",  "Upload a higher-resolution scan.",                "OCR confidence 0.42 < 0.70 threshold."),
            ("ERR_MISSING_FIELD",     "A required field was missing from the document.",       "Ensure all mandatory fields are present.",        "Required field 'invoice_total' not found."),
            ("ERR_UNSUPPORTED_FORMAT","The file format is not supported.",                     "Convert the file to PDF or CSV.",                 "Format '.docx' is not supported."),
            ("ERR_DUPLICATE",         "A document with the same hash already exists.",         "Remove the duplicate before re-submitting.",      "Duplicate of an existing file."),
            ("ERR_AUTH_UPSTREAM",     "Authentication with an upstream service failed.",       "Renew upstream credentials and retry.",           "Upstream returned 401 Unauthorized."),
        };
        var errorCatalog = errorDefs
            .Select(e => new ErrorCatalog { Id = Guid.NewGuid(), ErrorCode = e.Code, Description = e.Desc, RemediationMsg = e.Remediation, CreatedAt = now, UpdatedAt = now })
            .ToArray();

        string[] sources = { "S3_Bucket_Alpha", "SFTP_Beta", "API_Upload", "Manual_Upload", "Azure_Blob_Gamma" };
        string[] pipeline = { "Upload", "Validate", "Transform", "Load" };

        // ── Bulk transactional data ────────────────────────────────
        var transactions = new List<Transaction>();
        var files = new List<FileRecord>();
        var steps = new List<FileStepHistory>();
        var lineItems = new List<InvoiceLineItem>();
        var activity = new List<ActivityLog>();

        var siteTenant = new (Guid TenantId, Guid SiteId)[]
        {
            (AcmeId, AcmeMumbai), (AcmeId, AcmeDelhi), (AcmeId, AcmeChennai),
            (GlobexId, GlobexBerlin), (GlobexId, GlobexMunich),
        };

        int batchSeq = 0;
        foreach (var (tenantId, siteId) in siteTenant)
        {
            int batchCount = rng.Next(22, 34); // ~22–33 batches per site
            for (int b = 0; b < batchCount; b++)
            {
                batchSeq++;
                var submittedAt = now.AddDays(-rng.Next(0, 30))
                                     .AddHours(-rng.Next(0, 24))
                                     .AddMinutes(-rng.Next(0, 60));

                int fileCount = rng.Next(1, 9); // 1–8 files
                int uploaded = 0, processing = 0, failed = 0, completed = 0;
                var lastUpdated = submittedAt;
                var txnId = Guid.NewGuid();

                for (int fi = 0; fi < fileCount; fi++)
                {
                    int r = rng.Next(100);
                    string status = r < 55 ? "Completed" : r < 75 ? "Failed" : r < 90 ? "Processing" : "Queued";

                    var docType = docTypes[rng.Next(docTypes.Length)];
                    string ext = docType.Category == "CSV" ? "csv" : "pdf";
                    var fileId = Guid.NewGuid();
                    var createdAt = submittedAt.AddMinutes(rng.Next(0, 10));
                    var fileUpdated = createdAt.AddMinutes(rng.Next(5, 180));
                    if (fileUpdated > lastUpdated) lastUpdated = fileUpdated;

                    string currentStep;
                    string? extractionStatus;
                    decimal? confidence;
                    var stepStart = createdAt;

                    if (status == "Completed")
                    {
                        foreach (var step in pipeline)
                        {
                            steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = step, Status = "Success", StartedAt = stepStart, CompletedAt = stepStart.AddMinutes(2) });
                            stepStart = stepStart.AddMinutes(3);
                        }
                        currentStep = "Load"; extractionStatus = "Done";
                        confidence = Math.Round((decimal)(0.80 + rng.NextDouble() * 0.19), 3);
                        completed++;
                    }
                    else if (status == "Failed")
                    {
                        int failAt = rng.Next(1, pipeline.Length); // fail at Validate/Transform/Load
                        var def = errorDefs[rng.Next(errorDefs.Length)];
                        for (int si = 0; si <= failAt; si++)
                        {
                            bool isFail = si == failAt;
                            steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = pipeline[si], Status = isFail ? "Failed" : "Success", StartedAt = stepStart, CompletedAt = stepStart.AddMinutes(2), ErrorCode = isFail ? def.Code : null, ErrorMessage = isFail ? def.Msg : null });
                            stepStart = stepStart.AddMinutes(3);
                        }
                        currentStep = pipeline[failAt]; extractionStatus = "Failed";
                        confidence = Math.Round((decimal)(rng.NextDouble() * 0.4), 3);
                        failed++;
                        activity.Add(new ActivityLog { Id = Guid.NewGuid(), TenantId = tenantId, SiteId = siteId, EventType = "FILE_STATE_CHANGED", EntityType = "File", EntityId = fileId, EntityName = $"{docType.TypeName.ToLowerInvariant()}_{batchSeq}_{fi + 1}.{ext}", OldState = "Processing", NewState = "Failed", TriggeredBy = "system", CreatedAt = fileUpdated });
                    }
                    else if (status == "Processing")
                    {
                        int cur = rng.Next(1, pipeline.Length);
                        for (int si = 0; si < cur; si++)
                        {
                            steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = pipeline[si], Status = "Success", StartedAt = stepStart, CompletedAt = stepStart.AddMinutes(2) });
                            stepStart = stepStart.AddMinutes(3);
                        }
                        steps.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileId, DocumentTypeId = docType.Id, StepName = pipeline[cur], Status = "Processing", StartedAt = stepStart, CompletedAt = null });
                        currentStep = pipeline[cur]; extractionStatus = "Processing"; confidence = null;
                        processing++;
                    }
                    else // Queued
                    {
                        currentStep = "Upload"; extractionStatus = null; confidence = null;
                        uploaded++;
                    }

                    files.Add(new FileRecord
                    {
                        Id = fileId,
                        TenantId = tenantId,
                        SiteId = siteId,
                        TransactionId = txnId,
                        DocumentTypeId = docType.Id,
                        FileName = $"{docType.TypeName.ToLowerInvariant()}_{batchSeq}_{fi + 1}.{ext}",
                        FileType = docType.Category,
                        Status = status,
                        CurrentStep = currentStep,
                        FileSizeBytes = rng.Next(2_000, 5_000_000),
                        ExtractionStatus = extractionStatus,
                        ExtractionConfidence = confidence,
                        CreatedAt = createdAt,
                        LastUpdatedAt = fileUpdated
                    });

                    // Line items for completed invoices
                    if (docType.TypeName == "Invoice" && status == "Completed")
                    {
                        int lines = rng.Next(2, 7);
                        for (int li = 1; li <= lines; li++)
                        {
                            // ~15% of lines left uncategorized → exercises the LEFT-join null-category path
                            ItemCategory? cat = rng.Next(100) < 15 ? null : categories[rng.Next(categories.Length)];
                            decimal qty = rng.Next(1, 200);
                            decimal unit = Math.Round((decimal)(rng.NextDouble() * 900 + 5), 2);
                            lineItems.Add(new InvoiceLineItem { Id = Guid.NewGuid(), FileId = fileId, TenantId = tenantId, SiteId = siteId, ItemCategoryId = cat?.Id, LineNumber = li, Description = $"{(cat?.CategoryName ?? "Uncategorized")} item {li}", Quantity = qty, UnitPrice = unit, LineTotal = Math.Round(qty * unit, 2), Confidence = Math.Round((decimal)(0.70 + rng.NextDouble() * 0.29), 3), IsValid = true, ExtractedAt = fileUpdated });

                        }
                    }
                }

                string state =
                    completed == fileCount ? "Completed" :
                    uploaded == fileCount ? "Queued" :
                    (processing > 0 || uploaded > 0) ? "Processing" : "Failed";

                bool terminal = state is "Completed" or "Failed";

                transactions.Add(new Transaction
                {
                    Id = txnId,
                    TenantId = tenantId,
                    SiteId = siteId,
                    State = state,
                    SourceSystem = sources[rng.Next(sources.Length)],
                    TotalFiles = fileCount,
                    UploadedCount = uploaded,
                    ProcessingCount = processing,
                    FailedCount = failed,
                    CompletedCount = completed,
                    SubmittedAt = submittedAt,
                    LastUpdatedAt = lastUpdated,
                    CompletedAt = terminal ? lastUpdated : null
                });

                activity.Add(new ActivityLog { Id = Guid.NewGuid(), TenantId = tenantId, SiteId = siteId, EventType = "BATCH_SUBMITTED", EntityType = "Batch", EntityId = txnId, EntityName = $"Batch {txnId.ToString()[..8]}", OldState = null, NewState = "Processing", TriggeredBy = "system", CreatedAt = submittedAt });
                if (terminal)
                    activity.Add(new ActivityLog { Id = Guid.NewGuid(), TenantId = tenantId, SiteId = siteId, EventType = state == "Completed" ? "BATCH_COMPLETED" : "BATCH_FAILED", EntityType = "Batch", EntityId = txnId, EntityName = $"Batch {txnId.ToString()[..8]}", OldState = "Processing", NewState = state, TriggeredBy = "system", CreatedAt = lastUpdated });
            }
        }

        // ── Persist (one round-trip) ───────────────────────────────
        db.AddRange(tenants);
        db.AddRange(sites);
        db.AddRange(users);
        db.AddRange(access);
        db.AddRange(docTypes);
        db.AddRange(categories);
        db.AddRange(errorCatalog);
        db.AddRange(transactions);
        db.AddRange(files);
        db.AddRange(steps);
        db.AddRange(lineItems);
        db.AddRange(activity);
        await db.SaveChangesAsync();
    }
}
````

## File: DocAnalytics.Service/ActivityLog/ActivityLogDtos.cs
````csharp
namespace DocAnalytics.Service.ActivityLog;

// query-string params (FR-4.3 filters + paging/sort)
public sealed class ActivityLogQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? EventType { get; set; }   // exact, e.g. FILE_STATE_CHANGED | BATCH_SUBMITTED
    public string? EntityType { get; set; }  // exact, e.g. File | Batch
    public string? Entity { get; set; }      // partial match on entity_name
    public DateTime? From { get; set; }       // created on/after  (ISO-8601)
    public DateTime? To { get; set; }         // created on/before (ISO-8601)
    public string? SortBy { get; set; }       // ts | event_type | entity
    public string? SortDir { get; set; }      // asc | desc (default desc → newest first)
}

// one audit row (FR-4.2)
public sealed class ActivityLogItemDto
{
    public DateTime Ts { get; set; }            // → "ts"          (CreatedAt)
    public string EventType { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string? Entity { get; set; }         // → "entity"      (EntityName)
    public string? OldState { get; set; }
    public string? NewState { get; set; }
    public string Actor { get; set; } = null!;  // → "actor"       (TriggeredBy)
}
````

## File: DocAnalytics.Service/ActivityLog/ActivityLogFeatureExtensions.cs
````csharp
using DocAnalytics.Service.ActivityLog;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class ActivityLogFeatureExtensions
{
    public static IServiceCollection AddActivityLogFeature(this IServiceCollection services)
    {
        services.AddScoped<IActivityLogService, ActivityLogService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/ActivityLog/IActivityLogService.cs
````csharp
using DocAnalytics.Service.Common;   // PagedResult<T> — match the namespace BatchService uses

namespace DocAnalytics.Service.ActivityLog;

public interface IActivityLogService
{
    Task<PagedResult<ActivityLogItemDto>> GetActivityLogAsync(
        ActivityLogQuery query, CancellationToken ct = default);
}
````

## File: DocAnalytics.Service/Analytics/AnalyticsDtos.cs
````csharp
using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Analytics;

public sealed class SeriesDto
{
    public List<SeriesPointDto> Points { get; set; } = new();
}

public sealed class SeriesPointDto
{
    public string Label { get; set; } = null!;   // "Completed", "2023-10-21", "ERR_OCR_40"
    public long Value { get; set; }               // the count
}

// Optional date-range filter for time-series analytics (throughput, error-trend).
public sealed class AnalyticsRangeQuery : IValidatableObject
{
    public DateTime? From { get; set; }   // include data on/after this instant
    public DateTime? To { get; set; }     // include data on/before this instant

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From.HasValue && To.HasValue && From > To)
        {
            yield return new ValidationResult(
                "'from' must be earlier than or equal to 'to'.",
                new[] { nameof(From), nameof(To) });
        }
    }
}
````

## File: DocAnalytics.Service/Analytics/AnalyticsFeatureExtensions.cs
````csharp
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Analytics;

public static class AnalyticsFeatureExtensions
{
    public static IServiceCollection AddAnalyticsFeature(this IServiceCollection services)
    {
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Auth/AuthService.cs
````csharp
using DocAnalytics.Data;               
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;

    public AuthService(AppDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        // 1) Find user by globally-unique email (safe pre-token lookup)
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email && u.IsActive, ct);
        if (user is null) return null;          // null => controller returns 401

        // 2) Verify password against the stored BCrypt hash
        bool passwordOk = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!passwordOk) return null;

        // 3) Which sites can this user see? (the join)
        var sites = await GetSitesForUserAsync(user.Id, ct);

        // 4) Mint the JWT
        var token = _jwt.CreateToken(user);

        return new LoginResponse(
            token,
            new UserDto(user.Id, user.Email, user.Role),
            sites);
    }

    public async Task<MeResponse?> GetMeAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);
        if (user is null) return null;

        var sites = await GetSitesForUserAsync(userId, ct);
        return new MeResponse(new UserDto(user.Id, user.Email, user.Role), sites);
    }

    public Task<IReadOnlyList<SiteDto>> GetSitesAsync(Guid userId, CancellationToken ct)
        => GetSitesForUserAsync(userId, ct);

    // The UserSiteAccess → Sites join, written once, reused by all three endpoints
    private async Task<IReadOnlyList<SiteDto>> GetSitesForUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserSiteAccess
            .Where(usa => usa.UserId == userId)
            .Join(
                _db.Sites.Where(s => s.IsActive),
                usa => usa.SiteId,
                s => s.Id,
                (usa, s) => new SiteDto(s.Id, s.Name))
            .ToListAsync(ct);
    }
}
````

## File: DocAnalytics.Service/Auth/IJwtTokenService.cs
````csharp
using DocAnalytics.Domain.Entities;   

namespace DocAnalytics.Service.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
````

## File: DocAnalytics.Service/Auth/JwtTokenService.cs
````csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocAnalytics.Domain.Entities;       
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DocAnalytics.Service.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    public string CreateToken(User user)
    {
        // Secret comes from user-secrets. Must be >= 32 chars or startup throws IDX10720.
        var keyString = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        var expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 120;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // The claims = the info "printed on the wristband"
        var claims = new List<Claim>
        {
            new("userId",   user.Id.ToString()),
            new("tenantId", user.TenantId.ToString()),
            new("role",     user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],     // 👈 "DocAnalytics"
            audience: _config["Jwt:Audience"],   // 👈 "DocAnalyticsClient"
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
````

## File: DocAnalytics.Service/Batches/BatchFeatureExtensions.cs
````csharp
using Microsoft.Extensions.DependencyInjection;

namespace DocAnalytics.Service.Batches;

public static class BatchFeatureExtensions
{
    public static IServiceCollection AddBatchFeature(this IServiceCollection services)
    {
        services.AddScoped<IBatchService, BatchService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Batches/IBatchService.cs
````csharp
using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Batches;

public interface IBatchService
{
    Task<PagedResult<BatchListItemDto>> GetBatchesAsync(
        BatchListQuery query, CancellationToken ct = default);

    Task<BatchDetailDto?> GetBatchByIdAsync(Guid id, CancellationToken ct = default);

    Task<List<string>> GetSourcesAsync(CancellationToken ct = default);

    Task<PagedResult<BatchFileDto>?> GetBatchFilesAsync(
        Guid id, BatchFilesQuery query, CancellationToken ct = default);

}
````

## File: DocAnalytics.Service/Common/DateTimeExtensions.cs
````csharp
namespace DocAnalytics.Service.Common;

public static class DateTimeExtensions
{
    // Postgres 'timestamptz' requires UTC. Query-string dates parse as Kind=Unspecified,
    // which Npgsql rejects — so normalise to UTC before using in a query.
    public static DateTime AsUtc(this DateTime dt) => dt.Kind switch
    {
        DateTimeKind.Utc => dt,
        DateTimeKind.Local => dt.ToUniversalTime(),
        _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc) // Unspecified → treat as UTC
    };
}
````

## File: DocAnalytics.Service/Common/OneOfAttribute.cs
````csharp
using System.ComponentModel.DataAnnotations;

namespace DocAnalytics.Service.Common;

// Reusable whitelist check: value must be one of the allowed strings (case-insensitive).
// Null/blank passes — use it on OPTIONAL fields. Combine with [Required] if mandatory.
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class OneOfAttribute : ValidationAttribute
{
    private readonly string[] _allowed;

    public OneOfAttribute(params string[] allowed) => _allowed = allowed;

    public override bool IsValid(object? value)
    {
        var s = value as string;
        if (string.IsNullOrWhiteSpace(s)) return true;   // not supplied → nothing to validate

        return _allowed.Any(a => string.Equals(a, s, StringComparison.OrdinalIgnoreCase));
    }

    public override string FormatErrorMessage(string name)
        => $"'{name}' must be one of: {string.Join(", ", _allowed)}.";
}
````

## File: DocAnalytics.Service/Errors/ErrorCsvWriter.cs
````csharp
using System.Text;

namespace DocAnalytics.Service.Errors;

public static class ErrorCsvWriter
{
    private static readonly string[] Header =
        { "file_id", "file_name", "error_code", "error_message",
          "step", "source", "failed_at", "suggested_fix" };

    public static string Write(IEnumerable<ErrorListItemDto> rows)
    {
        var sb = new StringBuilder();
        sb.Append(string.Join(',', Header)).Append('\n');

        foreach (var r in rows)
        {
            sb.Append(Escape(r.FileId.ToString())).Append(',')
              .Append(Escape(r.FileName)).Append(',')
              .Append(Escape(r.ErrorCode)).Append(',')
              .Append(Escape(r.ErrorMessage)).Append(',')
              .Append(Escape(r.Step)).Append(',')
              .Append(Escape(r.Source)).Append(',')
              .Append(Escape(r.FailedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"))).Append(',')
              .Append(Escape(r.SuggestedFix))
              .Append('\n');
        }
        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        var needsQuote = value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0;
        var escaped = value.Replace("\"", "\"\"");
        return needsQuote ? $"\"{escaped}\"" : escaped;
    }
}
````

## File: DocAnalytics.Service/Errors/ErrorDtos.cs
````csharp
namespace DocAnalytics.Service.Errors;

// query-string params (same naming style as BatchListQuery / RecentFailuresQuery)
public sealed class ErrorListQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? From { get; set; }      // failed on/after  (ISO-8601)
    public DateTime? To { get; set; }        // failed on/before (ISO-8601)
    public string? Step { get; set; }        // Upload | Validate | Transform | Load
    public string? Source { get; set; }      // source system (Transaction.SourceSystem)
    public string? SortBy { get; set; }      // failed_at | file_name | error_code | step | source
    public string? SortDir { get; set; }     // asc | desc
}

// one failed-step row
public sealed class ErrorListItemDto
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = null!;
    public string ErrorCode { get; set; } = null!;
    public string? ErrorMessage { get; set; }
    public string Step { get; set; } = null!;        // failed step name
    public string Source { get; set; } = null!;      // source system
    public DateTime? FailedAt { get; set; }
    public string? SuggestedFix { get; set; }        // ErrorCatalog.RemediationMsg (LEFT join)
}
````

## File: DocAnalytics.Service/Errors/ErrorFeatureExtensions.cs
````csharp
using DocAnalytics.Service.Errors;

namespace Microsoft.Extensions.DependencyInjection;   // matches your AddXxxFeature() pattern

public static class ErrorFeatureExtensions
{
    public static IServiceCollection AddErrorListFeature(this IServiceCollection services)
    {
        services.AddScoped<IErrorService, ErrorService>();
        return services;
    }
}
````

## File: DocAnalytics.Service/Errors/IErrorService.cs
````csharp
using DocAnalytics.Service.Common;   // PagedResult<T> — match the namespace BatchService uses

namespace DocAnalytics.Service.Errors;

public interface IErrorService
{
    Task<PagedResult<ErrorListItemDto>> GetErrorsAsync(
        ErrorListQuery query, CancellationToken ct = default);

    // export = same filters/sort, but ALL matching rows (no paging)
    Task<List<ErrorListItemDto>> GetErrorsForExportAsync(
        ErrorListQuery query, CancellationToken ct = default);
}
````

## File: DocAnalytics.Service/Files/FileDetailsService.cs
````csharp
using System.Text;
using DocAnalytics.Data;                 // AppDbContext
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Files;

public sealed class FileDetailsService : IFileDetailsService
{
    private readonly AppDbContext _db;
    public FileDetailsService(AppDbContext db) => _db = db;

    // GET /api/v1/files/{id}/details — joins Files + FileStepHistory + ErrorCatalog
    public async Task<FileDetailDto?> GetFileDetailsAsync(Guid fileId, CancellationToken ct = default)
    {
        // 1) Load the file SCOPED to this tenant/site (global query filter auto-applies).
        var file = await _db.Files.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);

        if (file is null) return null;   // 404 for both not-found AND other-tenant (no existence leak)

        // 2) Pull this file's steps in timeline order. (FileStepHistory is NOT tenant-scoped,
        //    so we always drive from the already-scoped file id — isolation stays intact.)
        var steps = await _db.FileStepHistory.AsNoTracking()
            .Where(s => s.FileId == fileId)
            .OrderBy(s => s.StartedAt)
            .ThenBy(s => s.Id)
            .ToListAsync(ct);

        // 3) Soft-join to ErrorCatalog BY error_code (one round-trip, no N+1).
        var codes = steps.Where(s => s.ErrorCode != null)
                         .Select(s => s.ErrorCode!)
                         .Distinct()
                         .ToList();

        var remediation = codes.Count == 0
            ? new Dictionary<string, string?>()
            : await _db.ErrorCatalog.AsNoTracking()
                .Where(e => codes.Contains(e.ErrorCode))
                .ToDictionaryAsync(e => e.ErrorCode, e => e.RemediationMsg, ct);

        // 4) Shape the nested DTO.
        var dto = new FileDetailDto
        {
            FileInfo = new FileInfoDto
            {
                Id = file.Id,
                Name = file.FileName,
                CurrentStatus = file.Status,
                CurrentStep = file.CurrentStep
            },
            History = steps.Select(s => new StepHistoryDto
            {
                Step = s.StepName,
                Status = s.Status,
                Ts = s.StartedAt ?? s.CompletedAt,
                Error = s.ErrorCode is null ? null : new StepErrorDto
                {
                    Code = s.ErrorCode,
                    Message = s.ErrorMessage,
                    SuggestedFix = remediation.TryGetValue(s.ErrorCode, out var fix) ? fix : null
                }
            }).ToList()
        };

        return dto;
    }

    // GET /api/v1/files/{id}/logs — downloadable step-by-step trace
    public async Task<FileLogDto?> GetFileLogsAsync(Guid fileId, CancellationToken ct = default)
    {
        var file = await _db.Files.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId, ct);

        if (file is null) return null;

        var steps = await _db.FileStepHistory.AsNoTracking()
            .Where(s => s.FileId == fileId)
            .OrderBy(s => s.StartedAt)
            .ThenBy(s => s.Id)
            .ToListAsync(ct);

        var codes = steps.Where(s => s.ErrorCode != null).Select(s => s.ErrorCode!).Distinct().ToList();
        var remediation = codes.Count == 0
            ? new Dictionary<string, string?>()
            : await _db.ErrorCatalog.AsNoTracking()
                .Where(e => codes.Contains(e.ErrorCode))
                .ToDictionaryAsync(e => e.ErrorCode, e => e.RemediationMsg, ct);

        var sb = new StringBuilder();
        sb.AppendLine("=== Document Processing — File Step Log ===");
        sb.AppendLine($"File          : {file.FileName}");
        sb.AppendLine($"File Id       : {file.Id}");
        sb.AppendLine($"Current Status: {file.Status}");
        sb.AppendLine($"Current Step  : {file.CurrentStep}");
        sb.AppendLine($"Generated     : {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}");
        sb.AppendLine(new string('-', 60));

        foreach (var s in steps)
        {
            var ts = (s.StartedAt ?? s.CompletedAt)?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "(no timestamp)";
            sb.AppendLine($"[{ts}] {s.StepName,-12} {s.Status}");
            if (s.ErrorCode is not null)
            {
                sb.AppendLine($"    Error      : {s.ErrorCode} — {s.ErrorMessage}");
                if (remediation.TryGetValue(s.ErrorCode, out var fix) && !string.IsNullOrWhiteSpace(fix))
                    sb.AppendLine($"    Suggested  : {fix}");
            }
        }

        return new FileLogDto
        {
            FileName = $"file_{file.Id}_log.txt",
            Content = sb.ToString()
        };
    }


    
}
````

## File: docanalytics-web/src/app/core/services/toast.service.ts
````typescript
import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  text: string;
  type: 'info' | 'warning' | 'error' | 'success';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toasts = signal<Toast[]>([]);
  private seq = 0;

  show(text: string, type: Toast['type'] = 'info', timeoutMs = 5000): void {
    const id = ++this.seq;
    this.toasts.update(list => [...list, { id, text, type }]);
    setTimeout(() => this.dismiss(id), timeoutMs);
  }

  error(text: string): void { this.show(text, 'error', 7000); }
  success(text: string): void { this.show(text, 'success'); }
  warning(text: string): void { this.show(text, 'warning'); }

  dismiss(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }
}
````

## File: docanalytics-web/src/app/features/activity-log/activity-log.models.ts
````typescript
import { SortDir } from '../../shared/components/data-table/data-table.component';

// GET /api/v1/activity-log item — matches ActivityLogItemDto (snake_case JSON)
export interface ActivityLogItem {
  ts: string;                 // ISO-8601 (CreatedAt)
  event_type: string;         // FILE_STATE_CHANGED | BATCH_SUBMITTED | BATCH_COMPLETED | BATCH_FAILED
  entity_type: string;        // File | Batch
  entity: string | null;      // EntityName (file name / "Batch xxxxxxxx")
  entity_id?: string;         // present in DTO; unused in the table (kept optional)
  old_state: string | null;   // null on BATCH_SUBMITTED
  new_state: string | null;
  actor: string;              // TriggeredBy (e.g. "system")
}

// DataTable column keys MUST equal backend sort tokens (ApplySorting whitelist: ts|event_type|entity)
export type ActivityLogSortBy = 'ts' | 'event_type' | 'entity';

export interface ActivityLogQuery {
  page: number;
  pageSize: number;
  eventType: string | null;   // null = all (exact match)
  entityType: string | null;  // null = all (exact match — reserved, backend supports it)
  entity: string | null;      // partial ILIKE match on entity_name
  from: string | null;
  to: string | null;
  sortBy: ActivityLogSortBy;
  sortDir: SortDir;
}
````

## File: docanalytics-web/src/app/features/activity-log/activity-log.service.ts
````typescript
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { SortDir } from '../../shared/components/data-table/data-table.component';
import { ActivityLogItem, ActivityLogQuery, ActivityLogSortBy } from './activity-log.models';

@Injectable({ providedIn: 'root' })
export class ActivityLogService {
  private http = inject(HttpClient);
  private base = environment.apiBase;

  // widget renders its own inline error → opt out of the global toast (NFR-2)
  private silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  // ── list slice ──
  private _rows = signal<ActivityLogItem[]>([]);
  private _meta = signal<Meta | null>(null);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  readonly rows = this._rows.asReadonly();
  readonly meta = this._meta.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();

  private _query: ActivityLogQuery = {
    page: 1, pageSize: 20,
    eventType: null, entityType: null, entity: null,
    from: null, to: null,
    sortBy: 'ts', sortDir: 'desc',           // newest first
  };
  get query(): ActivityLogQuery { return this._query; }

  // lowercase-first keys — ASP.NET binding is case-insensitive; matches your batches code
  private buildParams(q: ActivityLogQuery): HttpParams {
    let p = new HttpParams()
      .set('page', q.page)
      .set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy)
      .set('sortDir', q.sortDir);
    if (q.eventType) p = p.set('eventType', q.eventType);
    if (q.entityType) p = p.set('entityType', q.entityType);
    if (q.entity) p = p.set('entity', q.entity);
    if (q.from) p = p.set('from', q.from);
    if (q.to) p = p.set('to', q.to);
    return p;
  }

  load(): void {
    this._loading.set(true);
    this._error.set(null);
    this.http
      .get<ApiResponse<ActivityLogItem[]>>(`${this.base}/activity-log`, {
        params: this.buildParams(this._query),
        ...this.silent,
      })
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (res) => {
          this._rows.set(res.data ?? []);
          this._meta.set(res.meta ?? null);
        },
        error: (err) => this._error.set(this.msg(err, 'Failed to load activity log.')),
      });
  }

  private patch(p: Partial<ActivityLogQuery>, resetPage = true): void {
    this._query = { ...this._query, ...p, page: resetPage ? 1 : (p.page ?? this._query.page) };
    this.load();
  }

  setFilters(f: { eventType: string | null; from: string | null; to: string | null }): void {
    this.patch({ eventType: f.eventType, from: f.from, to: f.to });
  }
  setEntitySearch(entity: string): void { this.patch({ entity: entity.trim() || null }); }
  setSort(sortBy: ActivityLogSortBy, sortDir: SortDir): void { this.patch({ sortBy, sortDir }); }
  setPage(page: number): void { this.patch({ page }, false); }
  setPageSize(pageSize: number): void { this.patch({ pageSize }); }

  private msg(err: any, fallback: string): string {
    return err?.error?.error?.message ?? fallback;
  }
}
````

## File: docanalytics-web/src/app/features/dashboard/status-distribution-chart/status-distribution-chart.component.html
````html
<app-chart-card
  title="Status Distribution"
  subtitle="Documents by current status"
  [loading]="loading()"
  [error]="error()"
  [empty]="!loading() && !error() && data().length === 0"
   (retry)="retry.emit()">

  <div class="bars">
    @for (row of rows(); track row.label) {
      <div class="row">
        <span class="label">{{ row.label }}</span>
        <div class="track">
          <div class="fill" [class]="'st-' + row.key" [style.width.%]="row.pct"></div>
        </div>
        <span class="val">{{ row.value }} · {{ row.pct }}%</span>
      </div>
    }
  </div>
</app-chart-card>
````

## File: docanalytics-web/src/app/features/dashboard/status-distribution-chart/status-distribution-chart.component.ts
````typescript
import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ChartCardComponent } from '../../../shared/components/chart-card/chart-card.component';
import { SeriesPoint } from '../../../core/models/dashboard.model';


@Component({
  selector: 'app-status-distribution-chart',
  standalone: true,
  imports: [ChartCardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-chart-card
      title="Status Distribution"
      subtitle="Documents by current status"
      [loading]="loading()"
      [error]="error()"
      [empty]="!loading() && !error() && data().length === 0"
       (retry)="retry.emit()">

      <div class="bars">
        @for (row of rows(); track row.label) {
          <div class="row">
            <span class="label">{{ row.label }}</span>
            <div class="track">
              <div class="fill" [class]="'st-' + row.key" [style.width.%]="row.pct"></div>
            </div>
            <span class="val">{{ row.value }} · {{ row.pct }}%</span>
          </div>
        }
      </div>
    </app-chart-card>
  `,
  styles: [`
    .bars { display: flex; flex-direction: column; gap: var(--space-2); width: 100%; align-self: flex-start; }
    .row { display: grid; grid-template-columns: 110px 1fr 90px; align-items: center; gap: var(--space-1); }
    .label { font-size: 0.82rem; color: var(--dark-gray); }
    .track { background: var(--light-gray); border: 1px solid var(--cool-gray); border-radius: 6px; height: 18px; overflow: hidden; }
    .fill { height: 100%; border-radius: 6px 0 0 6px; transition: width .3s ease; }
    .val { font-size: 0.78rem; color: var(--dark-gray-3); text-align: right; }
    /* status colors = fills only (AVEVA rule) */
    .st-completed  { background: var(--status-confirmed); }
    .st-failed     { background: var(--status-error); }
    .st-processing { background: var(--status-warning); }
    .st-queued     { background: var(--cool-gray); }
  `]
})
export class StatusDistributionChartComponent {
  data = input<SeriesPoint[]>([]);
  loading = input(false);
  error = input<string | null>(null);
  retry = output<void>();

  rows = computed(() => {
    const d = this.data();
    const total = d.reduce((sum, p) => sum + p.value, 0) || 1;
    return d.map(p => {
      const k = p.label.toLowerCase().replace(/\s+/g, '');
      return {
        label: p.label,
        value: p.value,
        pct: Math.round((p.value / total) * 100),
        key: k === 'inprogress' ? 'processing' : k, // "In Progress" OR "Processing" → same blue fill
      };
    });
  });
}
````

## File: docanalytics-web/src/app/features/dashboard/throughput-chart/throughput-chart.component.css
````css
.tp {
  display: grid;
  grid-template-columns: auto auto 1fr; /* y-title | y-labels | plot */
  grid-template-rows: 1fr auto auto; /* plot | x-labels | x-title */
  column-gap: var(--space-1);
  width: 100%;
}

.y-title {
  grid-column: 1;
  grid-row: 1;
  writing-mode: vertical-rl;
  transform: rotate(180deg);
  align-self: center;
  font-size: 0.7rem;
  color: var(--dark-gray-3);
}

.y-labels {
  grid-column: 2;
  grid-row: 1;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  text-align: right;
  font-size: 0.7rem;
  color: var(--dark-gray-3);
  padding-right: 4px;
}

.chart {
  grid-column: 3;
  grid-row: 1;
  width: 100%;
  height: 220px;
}

.x-labels {
  grid-column: 3;
  grid-row: 2;
  display: flex;
  justify-content: space-between;
  font-size: 0.7rem;
  color: var(--dark-gray-3);
  margin-top: 4px;
}

.x-title {
  grid-column: 3;
  grid-row: 3;
  text-align: center;
  font-size: 0.7rem;
  color: var(--dark-gray-3);
  margin-top: 2px;
}

.grid {
  stroke: var(--light-gray);
  stroke-width: 1;
}

.line {
  fill: none;
  stroke: var(--slate-blue);
  stroke-width: 2;
}
````

## File: docanalytics-web/src/app/features/dashboard/throughput-chart/throughput-chart.component.html
````html
<app-chart-card
  title="Throughput"
  subtitle="Documents processed per day"
  [loading]="loading()"
  [error]="error()"
  [empty]="!loading() && !error() && data().length === 0"
  (retry)="retry.emit()">

  <div class="tp">
    <!-- Y axis title -->
    <span class="y-title">Documents</span>

    <!-- Y axis tick labels (max → mid → 0) -->
    <div class="y-labels">
      <span>{{ maxVal() }}</span>
      <span>{{ midVal() }}</span>
      <span>0</span>
    </div>

    <!-- the plot -->
    <svg class="chart" [attr.viewBox]="'0 0 ' + W + ' ' + H"
         preserveAspectRatio="none" role="img" aria-label="Documents processed per day">
      @for (gl of gridLines(); track gl) {
        <line class="grid" x1="0" [attr.y1]="gl" [attr.x2]="W" [attr.y2]="gl" />
      }
      <polyline class="line" [attr.points]="linePoints()" />
    </svg>

    <!-- X axis tick labels (first → last date) -->
    <div class="x-labels">
      <span>{{ firstLabel() }}</span>
      <span>{{ lastLabel() }}</span>
    </div>
    <!-- X axis title -->
    <span class="x-title">Date</span>
  </div>
</app-chart-card>
````

## File: docanalytics-web/src/app/features/dashboard/throughput-chart/throughput-chart.component.ts
````typescript
import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ChartCardComponent } from '../../../shared/components/chart-card/chart-card.component';
import { SeriesPoint } from '../../../core/models/dashboard.model';

@Component({
  selector: 'app-throughput-chart',
  standalone: true,
  imports: [ChartCardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,

  templateUrl: './throughput-chart.component.html',
  styleUrl: './throughput-chart.component.css',
})
export class ThroughputChartComponent {
  data = input<SeriesPoint[]>([]);
  loading = input(false);
  error = input<string | null>(null);
  retry = output<void>();

  readonly W = 600;
  readonly H = 240;
  private pad = { top: 16, right: 12, bottom: 20, left: 8 };

  // public so the template axis labels can read them
  maxVal = computed(() => Math.max(1, ...this.data().map(p => p.value)));
  midVal = computed(() => Math.round(this.maxVal() / 2));
  firstLabel = computed(() => this.data()[0]?.label ?? '');
  lastLabel = computed(() => {
    const d = this.data();
    return d.length ? d[d.length - 1].label : '';
  });

  private x(i: number, n: number): number {
    const innerW = this.W - this.pad.left - this.pad.right;
    return this.pad.left + (n <= 1 ? innerW / 2 : (innerW * i) / (n - 1));
  }
  private y(v: number): number {
    const innerH = this.H - this.pad.top - this.pad.bottom;
    return this.pad.top + innerH * (1 - v / this.maxVal());
  }

  linePoints = computed(() => {
    const d = this.data();
    return d.map((p, i) => `${this.x(i, d.length)},${this.y(p.value)}`).join(' ');
  });

  gridLines = computed(() => {
    const innerH = this.H - this.pad.top - this.pad.bottom;
    return [0, 0.25, 0.5, 0.75, 1].map(t => this.pad.top + innerH * t);
  });
}
````

## File: docanalytics-web/src/app/features/errors/errors.models.ts
````typescript
import { SortDir } from '../../shared/components/data-table/data-table.component';

// top-frequencies + trend both return { data: { points: [{label,value}] } }
export interface ChartPoint { label: string; value: number; }

// GET /errors item — VERIFIED against Swagger
export interface ErrorListItem {
  file_id: string;
  file_name: string;
  error_code: string;      // e.g. ERR_TIMEOUT
  error_message: string;
  step: string;            // Validate | Transform | Load | ...
  source: string;          // S3_Bucket_Alpha | Manual_Upload | ...
  failed_at: string;       // ISO timestamp
  suggested_fix: string;
}

// DataTable column keys MUST equal backend sort tokens. ⚠️ VERIFY vs backend ApplySorting whitelist
export type ErrorSortBy = 'failed_at' | 'error_code' | 'file_name' | 'step' | 'source';

export interface ErrorQuery {
  page: number; pageSize: number;
  step: string | null;      // null = all steps
  source: string | null;    // null = all sources
  from: string | null; to: string | null;
  sortBy: ErrorSortBy; sortDir: SortDir;
}
````

## File: docanalytics-web/src/index.html
````html
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>DocanalyticsWeb</title>
  <base href="/">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="icon" type="image/x-icon" href="favicon.ico">
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link href="https://fonts.googleapis.com/css2?family=Barlow:wght@300;400;500;600&family=Mulish:wght@400;500;600;700;800&display=swap" rel="stylesheet">
  <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

</head>
<body>
  <app-root></app-root>
</body>
</html>
````

## File: DocAnalytics.Api/Controllers/DashboardAnalyticsController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboard")]
[Tags("Dashboard")]
public sealed class DashboardAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    public DashboardAnalyticsController(IAnalyticsService chartService) => _analyticsService = chartService;

    // GET /api/v1/dashboard/status-distribution
    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution(CancellationToken ct)
    {
        var series = await _analyticsService.GetStatusDistributionAsync(ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    // GET /api/v1/dashboard/throughput?from=...&to=...
    [HttpGet("throughput")]
    public async Task<IActionResult> GetThroughput([FromQuery] AnalyticsRangeQuery query, CancellationToken ct)
    {
        var series = await _analyticsService.GetThroughputAsync(query.From, query.To, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }



}
````

## File: DocAnalytics.Api/Controllers/ErrorAnalyticsController.cs
````csharp
using DocAnalytics.Api.Common;
using DocAnalytics.Service.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/errors")]
[Tags("Errors")]
public sealed class ErrorAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _chartService;
    public ErrorAnalyticsController(IAnalyticsService chartService) => _chartService = chartService;

    // GET /api/v1/errors/top-frequencies?topN=5
    [HttpGet("top-frequencies")]
    public async Task<IActionResult> GetTopErrors([FromQuery] int topN = 5, CancellationToken ct = default)
    {
        var series = await _chartService.GetTopErrorsAsync(topN, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

    // GET /api/v1/errors/trend?from=...&to=...
    [HttpGet("trend")]
    public async Task<IActionResult> GetErrorTrend([FromQuery] AnalyticsRangeQuery query, CancellationToken ct)
    {
        var series = await _chartService.GetErrorTrendAsync(query.From, query.To, ct);
        return Ok(ApiResponse<SeriesDto>.Ok(series));
    }

}
````

## File: DocAnalytics.Api/Extensions/ValidationExtensions.cs
````csharp
using DocAnalytics.Api.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DocAnalytics.Api.Extensions;

public static class ValidationExtensions
{
    // Replaces the framework's default 400 (ProblemDetails) with our ApiResponse.Fail envelope.
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                // Flatten every field's errors into a tidy list for the `details` bag.
                var errors = context.ModelState
                    .Where(kvp => kvp.Value is not null && kvp.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value!.Errors.Select(e => new
                    {
                        field = JsonNamingPolicy.SnakeCaseLower.ConvertName(kvp.Key),
                        error = string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value."
                            : e.ErrorMessage
                    }))
                    .ToList();

                var body = ApiResponse<object>.Fail(
                    "validation_error",
                    "One or more fields are invalid.",
                    errors);

                return new BadRequestObjectResult(body);   // 400 + your envelope
            };
        });

        return services;
    }
}
````

## File: DocAnalytics.Api/Middleware/TenantSiteMiddleware.cs
````csharp
using System.Security.Claims;
using System.Text.Json;
using DocAnalytics.Api.Common;          // ApiResponse<T>
using DocAnalytics.Data;                // AppDbContext
using Microsoft.EntityFrameworkCore;    // AnyAsync

namespace DocAnalytics.Api.Middleware;

public class TenantSiteMiddleware
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private readonly RequestDelegate _next;
    public TenantSiteMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx, CurrentUser currentUser, AppDbContext db)
    {
        if (ctx.User.Identity?.IsAuthenticated == true)
        {
            var userId = ctx.User.FindFirstValue("userId");
            var tenantId = ctx.User.FindFirstValue("tenantId");
            var role = ctx.User.FindFirstValue("role") ?? "Viewer";
            var siteIdRaw = ctx.Request.Query["site_id"].FirstOrDefault()
                            ?? ctx.Request.Headers["X-Site-Id"].FirstOrDefault();

            if (Guid.TryParse(userId, out var uid) && Guid.TryParse(tenantId, out var tid))
            {
                Guid.TryParse(siteIdRaw, out var sid);

                // NEW: if a site is supplied, the user must be granted access to it (FR-5.3)
                if (sid != Guid.Empty)
                {
                    var hasAccess = await db.UserSiteAccess
                        .AsNoTracking()
                        .AnyAsync(x => x.UserId == uid && x.SiteId == sid);

                    if (!hasAccess)
                    {
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        ctx.Response.ContentType = "application/json";
                        var body = ApiResponse<object>.Fail(
                            "SITE_FORBIDDEN",
                            "You do not have access to the requested site.");
                        await ctx.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOpts));
                        return;
                    }
                }

                currentUser.Set(uid, tid, sid, role);
            }
        }

        await _next(ctx);
    }
}
````

## File: DocAnalytics.Service/ActivityLog/ActivityLogService.cs
````csharp
using DocAnalytics.Data;                         // AppDbContext
using DocAnalytics.Service.Common;               // PagedResult<T>
using Microsoft.EntityFrameworkCore;
using DomainActivityLog = DocAnalytics.Domain.Entities.ActivityLog;

namespace DocAnalytics.Service.ActivityLog;

public sealed class ActivityLogService : IActivityLogService
{
    private readonly AppDbContext _db;
    public ActivityLogService(AppDbContext db) => _db = db;

    public async Task<PagedResult<ActivityLogItemDto>> GetActivityLogAsync(
        ActivityLogQuery query, CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        // ActivityLog IS ITenantScoped → tenant_id + site_id auto-applied by the global filter.
        IQueryable<DomainActivityLog> q = _db.ActivityLog.AsNoTracking();

        // ---- FILTERS (added only if provided) ----
        if (!string.IsNullOrWhiteSpace(query.EventType))
            q = q.Where(a => a.EventType == query.EventType);

        if (!string.IsNullOrWhiteSpace(query.EntityType))
            q = q.Where(a => a.EntityType == query.EntityType);

        if (!string.IsNullOrWhiteSpace(query.Entity))
        {
            var term = query.Entity.Trim();
            q = q.Where(a => a.EntityName != null && EF.Functions.ILike(a.EntityName, $"%{term}%"));
        }

        // Postgres timestamptz requires Kind=Utc; query-string dates arrive as Kind=Unspecified.
        // (Swap for query.From.Value.AsUtc() once feature/validation's Service/Common helper merges.)
        if (query.From.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(query.From.Value, DateTimeKind.Utc);
            q = q.Where(a => a.CreatedAt >= fromUtc);
        }

        if (query.To.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(query.To.Value, DateTimeKind.Utc);
            q = q.Where(a => a.CreatedAt <= toUtc);
        }


        // ---- COUNT before paging ----
        var totalCount = await q.CountAsync(ct);

        // ---- SORT (whitelisted) → PAGE → SHAPE ----
        var items = await ApplySorting(q, query.SortBy, query.SortDir)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ActivityLogItemDto
            {
                Ts = a.CreatedAt,
                EventType = a.EventType,
                EntityType = a.EntityType,
                Entity = a.EntityName,
                OldState = a.OldState,
                NewState = a.NewState,
                Actor = a.TriggeredBy
            })
            .ToListAsync(ct);

        return new PagedResult<ActivityLogItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // whitelisted sort → no string concat → SQL-injection safe (NFR-3)
    private static IQueryable<DomainActivityLog> ApplySorting(
        IQueryable<DomainActivityLog> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        IOrderedQueryable<DomainActivityLog> ordered = (sortBy ?? "ts").ToLowerInvariant() switch
        {
            "event_type" => desc ? q.OrderByDescending(a => a.EventType) : q.OrderBy(a => a.EventType),
            "entity" => desc ? q.OrderByDescending(a => a.EntityName) : q.OrderBy(a => a.EntityName),
            _ => desc ? q.OrderByDescending(a => a.CreatedAt) : q.OrderBy(a => a.CreatedAt)
        };

        return ordered.ThenBy(a => a.Id);  // stable page boundaries on tied timestamps
    }
}
````

## File: DocAnalytics.Service/Analytics/AnalyticsService.cs
````csharp
using DocAnalytics.Data;
using Microsoft.EntityFrameworkCore;
using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Analytics;



public sealed class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _db;
    public AnalyticsService(AppDbContext db) => _db = db;

    public async Task<SeriesDto> GetStatusDistributionAsync(CancellationToken ct = default)
    {
        var points = await _db.Files
            .AsNoTracking()
            .GroupBy(f => f.Status)                    // bucket files by their Status
            .Select(g => new SeriesPointDto
            {
                Label = g.Key,                         // the status value, e.g. "Completed"
                Value = g.LongCount()                  // COUNT(*) for that status
            })
            .OrderByDescending(p => p.Value)           // biggest slice first
            .ThenBy(p => p.Label)                      // tiebreaker → deterministic order
            .ToListAsync(ct);

        return new SeriesDto { Points = points };
    }

    public async Task<SeriesDto> GetThroughputAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = _db.Files
            .AsNoTracking()
            .Where(f => f.Status == "Completed");                 // FR-1.2: completed only

        if (from.HasValue)
        {
            var fromUtc = from.Value.AsUtc();
            q = q.Where(f => f.LastUpdatedAt >= fromUtc);   // optional lower bound (UTC-normalised)
        }
        if (to.HasValue)
        {
            var toUtc = to.Value.AsUtc();
            q = q.Where(f => f.LastUpdatedAt <= toUtc);     // optional upper bound (UTC-normalised)
        }

        var raw = await q
            .GroupBy(f => f.LastUpdatedAt.Date)                   // bucket by completion day
            .Select(g => new { Day = g.Key, Count = g.LongCount() })
            .OrderBy(x => x.Day)
            .ToListAsync(ct);

        var points = raw
            .Select(x => new SeriesPointDto { Label = x.Day.ToString("yyyy-MM-dd"), Value = x.Count })
            .ToList();

        return new SeriesDto { Points = points };
    }


    public async Task<SeriesDto> GetTopErrorsAsync(int topN = 5, CancellationToken ct = default)
    {
        // Light guard so a silly topN can't break the chart (full validation = Round 5).
        if (topN < 1) topN = 5;
        if (topN > 20) topN = 20;

        var raw = await _db.Files                    // ① ANCHOR on the tenant-scoped table
            .AsNoTracking()
            .SelectMany(f => f.Steps)                // ② navigate OUT to non-scoped FileStepHistory
            .Where(s => s.ErrorCode != null)         // ③ only steps that actually errored
            .GroupBy(s => s.ErrorCode!)              // ④ bucket by error code
            .Select(g => new { Code = g.Key, Count = g.LongCount() })
            .OrderByDescending(x => x.Count)         // ⑤ most frequent first
            .ThenBy(x => x.Code)                     // ⑥ deterministic tiebreaker
            .Take(topN)                              // ⑦ TOP N → SQL LIMIT
            .ToListAsync(ct);

        var points = raw
            .Select(x => new SeriesPointDto { Label = x.Code, Value = x.Count })
            .ToList();

        return new SeriesDto { Points = points };
    }
    public async Task<SeriesDto> GetErrorTrendAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var q = _db.Files
            .AsNoTracking()
            .SelectMany(f => f.Steps)
            .Where(s => s.ErrorCode != null && s.StartedAt != null);   // errored AND timestamped

        if (from.HasValue)
        {
            var fromUtc = from.Value.AsUtc();
            q = q.Where(s => s.StartedAt >= fromUtc);
        }
        if (to.HasValue)
        {
            var toUtc = to.Value.AsUtc();
            q = q.Where(s => s.StartedAt <= toUtc);
        }

        var raw = await q
            .GroupBy(s => s.StartedAt!.Value.Date)                // null-guarded above
            .Select(g => new { Day = g.Key, Count = g.LongCount() })
            .OrderBy(x => x.Day)
            .ToListAsync(ct);

        var points = raw
            .Select(x => new SeriesPointDto { Label = x.Day.ToString("yyyy-MM-dd"), Value = x.Count })
            .ToList();

        return new SeriesDto { Points = points };
    }


}
````

## File: DocAnalytics.Service/Analytics/IAnalyticsService.cs
````csharp
namespace DocAnalytics.Service.Analytics;

public interface IAnalyticsService
{
    Task<SeriesDto> GetStatusDistributionAsync(CancellationToken ct = default);
    Task<SeriesDto> GetThroughputAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<SeriesDto> GetTopErrorsAsync(int topN = 5, CancellationToken ct = default);
    Task<SeriesDto> GetErrorTrendAsync(DateTime? from, DateTime? to, CancellationToken ct = default);


}
````

## File: DocAnalytics.Service/Batches/BatchDtos.cs
````csharp
using System.ComponentModel.DataAnnotations;
using DocAnalytics.Service.Common;

namespace DocAnalytics.Service.Batches;


// The filters/options the client sends in the URL (?page=1&status=failed...)
public sealed class BatchListQuery : IValidatableObject
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }   // all | in_progress | completed | failed
    public string? Source { get; set; }   // filter by source system
    public DateTime? From { get; set; }    // submitted on/after
    public DateTime? To { get; set; }      // submitted on/before
    public string? Search { get; set; }    // partial batch id
    public string? SortBy { get; set; }    // which column to sort by

    [OneOf("asc", "desc")]
    public string? SortDir { get; set; }   // asc or desc

    // Cross-field rule: a date window only makes sense if from is on/before to.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Only check when BOTH are supplied — null means "no bound on this side".
        if (From.HasValue && To.HasValue && From > To)
        {
            yield return new ValidationResult(
                "'from' must be earlier than or equal to 'to'.",
                new[] { nameof(From), nameof(To) });
        }
    }
}


// One row of the batch list that we send back
public sealed class BatchListItemDto
{
    public Guid TransactionId { get; set; }
    public string State { get; set; } = default!;
    public string SourceSystem { get; set; } = default!;
    public int TotalFiles { get; set; }
    public int UploadedCount { get; set; }
    public int ProcessingCount { get; set; }
    public int FailedCount { get; set; }
    public int CompletedCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

// ── GET /api/v1/batches/{id} : drill-down detail ──
public sealed class BatchDetailDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;   // from Transaction.State
    public string Source { get; set; } = null!;    // from Transaction.SourceSystem
    public int TotalFiles { get; set; }
    public FileStatsDto FileStats { get; set; } = null!;   // → "file_stats"
    public BatchTimesDto Times { get; set; } = null!;      // → "times"
}

public sealed class FileStatsDto
{
    public int Uploaded { get; set; }
    public int Processing { get; set; }
    public int Failed { get; set; }
    public int Completed { get; set; }
}

public sealed class BatchTimesDto
{
    public DateTime SubmittedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }   // nullable
}

// ── GET /api/v1/batches/{id}/files : one file row ──
public sealed class BatchFileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string CurrentStep { get; set; } = null!;
    public long? FileSizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

public sealed class BatchFilesQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
````

## File: DocAnalytics.Service/Errors/ErrorService.cs
````csharp
using DocAnalytics.Data;                 // AppDbContext
using DocAnalytics.Service.Common;       // PagedResult<T>
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Errors;

public sealed class ErrorService : IErrorService
{
    private readonly AppDbContext _db;
    public ErrorService(AppDbContext db) => _db = db;

    public async Task<PagedResult<ErrorListItemDto>> GetErrorsAsync(
        ErrorListQuery query, CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        var baseQuery = BuildFilteredQuery(query);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await ApplySorting(baseQuery, query.SortBy, query.SortDir)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<ErrorListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<ErrorListItemDto>> GetErrorsForExportAsync(
        ErrorListQuery query, CancellationToken ct = default)
        => await ApplySorting(BuildFilteredQuery(query), query.SortBy, query.SortDir)
            .ToListAsync(ct);

    // ── core query: Files (scoped) → FileStepHistory → Transactions (scoped) → ErrorCatalog (LEFT) ──
    private IQueryable<ErrorListItemDto> BuildFilteredQuery(ErrorListQuery query)
    {
        // tenant_id + site_id auto-applied to Files AND Transactions by the global filter.
        var q =
            from f in _db.Files.AsNoTracking()
            join s in _db.FileStepHistory.AsNoTracking() on f.Id equals s.FileId
            join t in _db.Transactions.AsNoTracking() on f.TransactionId equals t.Id
            join ec in _db.ErrorCatalog.AsNoTracking() on s.ErrorCode equals ec.ErrorCode into ecg
            from ec in ecg.DefaultIfEmpty()        // LEFT join → suggested_fix null if no catalog row
            where s.Status == "Failed"             // matches DbSeeder literal exactly
            select new { f, s, t, ec };

        // Postgres timestamptz requires Kind=Utc; query-string dates arrive as Kind=Unspecified.
        if (query.From.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(query.From.Value, DateTimeKind.Utc);
            q = q.Where(x => (x.s.CompletedAt ?? x.s.StartedAt) >= fromUtc);
        }

        if (query.To.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(query.To.Value, DateTimeKind.Utc);
            q = q.Where(x => (x.s.CompletedAt ?? x.s.StartedAt) <= toUtc);
        }

        if (!string.IsNullOrWhiteSpace(query.Step))
            q = q.Where(x => x.s.StepName == query.Step);

        if (!string.IsNullOrWhiteSpace(query.Source))
            q = q.Where(x => x.t.SourceSystem == query.Source);

        return q.Select(x => new ErrorListItemDto
        {
            FileId = x.f.Id,
            FileName = x.f.FileName,
            ErrorCode = x.s.ErrorCode!,            // failed steps always carry a code in seed
            ErrorMessage = x.s.ErrorMessage,
            Step = x.s.StepName,
            Source = x.t.SourceSystem,
            FailedAt = x.s.CompletedAt ?? x.s.StartedAt,
            SuggestedFix = x.ec != null ? x.ec.RemediationMsg : null
        });
    }

    // whitelisted sort → no string concat → SQL-injection safe (NFR-3)
    private static IQueryable<ErrorListItemDto> ApplySorting(
        IQueryable<ErrorListItemDto> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        IOrderedQueryable<ErrorListItemDto> ordered = (sortBy ?? "failed_at").ToLowerInvariant() switch
        {
            "file_name" => desc ? q.OrderByDescending(r => r.FileName) : q.OrderBy(r => r.FileName),
            "error_code" or "code" => desc ? q.OrderByDescending(r => r.ErrorCode) : q.OrderBy(r => r.ErrorCode),
            "step" => desc ? q.OrderByDescending(r => r.Step) : q.OrderBy(r => r.Step),
            "source" => desc ? q.OrderByDescending(r => r.Source) : q.OrderBy(r => r.Source),
            _ => desc ? q.OrderByDescending(r => r.FailedAt) : q.OrderBy(r => r.FailedAt)
        };

        return ordered.ThenBy(r => r.FileId);  // stable page boundaries on tied timestamps
    }
}
````

## File: docanalytics-web/angular.json
````json
{
  "$schema": "./node_modules/@angular/cli/lib/config/schema.json",
  "version": 1,
  "cli": {
    "packageManager": "npm",
    "analytics": false
  },
  "newProjectRoot": "projects",
  "projects": {
    "docanalytics-web": {
      "projectType": "application",
      "schematics": {
        "@schematics/angular:component": {
          "inlineTemplate": false,
          "inlineStyle": false,
          "style": "css"
        }
      },
      "root": "",
      "sourceRoot": "src",
      "prefix": "app",
      "architect": {
        "build": {
          "builder": "@angular/build:application",
          "options": {
            "browser": "src/main.ts",
            "tsConfig": "tsconfig.app.json",
            "assets": [
              {
                "glob": "**/*",
                "input": "public"
              }
            ],
            "styles": [
              "src/styles.css"
            ]
          },
          "configurations": {
            "production": {
              "budgets": [
                {
                  "type": "initial",
                  "maximumWarning": "500kB",
                  "maximumError": "1MB"
                },
                {
                  "type": "anyComponentStyle",
                  "maximumWarning": "4kB",
                  "maximumError": "8kB"
                }
              ],
              "outputHashing": "all"
            },
            "development": {
              "optimization": false,
              "extractLicenses": false,
              "sourceMap": true
            }
          },
          "defaultConfiguration": "production"
        },
        "serve": {
          "builder": "@angular/build:dev-server",
          "options": {
            "proxyConfig": "proxy.conf.json"
          },
          "configurations": {
            "production": {
              "buildTarget": "docanalytics-web:build:production"
            },
            "development": {
              "buildTarget": "docanalytics-web:build:development"
            }
          },
          "defaultConfiguration": "development"
        },
        "test": {
          "builder": "@angular/build:unit-test"
        }
      }
    }
  }
}
````

## File: docanalytics-web/src/app/app.config.ts
````typescript
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { authSiteInterceptor } from './core/interceptors/auth-site.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authSiteInterceptor, errorInterceptor])),
  ],
};
````

## File: docanalytics-web/src/app/core/guards/site-access.guard.ts
````typescript
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { SiteContextService } from '../services/site-context.service';

export const siteAccessGuard: CanActivateFn = async (route) => {
  const auth = inject(AuthService);
  const siteCtx = inject(SiteContextService);
  const router = inject(Router);

  const siteId = route.paramMap.get('siteId');
  if (!siteId) return router.createUrlTree(['/login']);

  // ensure user + sites are loaded (handles hard refresh where only the token survives)
  const ok = await auth.ensureSession();
  if (!ok) return router.createUrlTree(['/login']);

  // FR-5.3 client-side check (server still enforces)
  if (auth.hasSiteAccess(siteId)) {
    siteCtx.setSite(siteId);
    return true;
  }

  // logged in but not authorized for THIS site → first allowed site, else login
  const fallback = auth.sites()[0];
  return router.createUrlTree(fallback ? ['/site', fallback.site_id, 'dashboard'] : ['/login']);
};
````

## File: docanalytics-web/src/app/core/interceptors/error.interceptor.ts
````typescript
import { HttpInterceptorFn, HttpContextToken } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';

const TOKEN_KEY = 'da_token';

// the on/off switch — defaults to "show toast"
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError(err => {
  const isLogin = req.url.includes('/auth/login');
  // Login handles its own errors inline — skip all global toasts/redirects.
  if (isLogin) {
    return throwError(() => err);
  }

  // Calls that show their own inline error (dashboard widgets) opt out of the toast.
  const skipToast = req.context.get(SKIP_ERROR_TOAST);
  const apiMsg = err?.error?.error?.message as string | undefined;

  if (err.status === 401) {
    // session handling stays regardless of the toast preference
    localStorage.removeItem(TOKEN_KEY);
    router.navigate(['/login']);
    if (!skipToast) toast.error('Session expired — please log in again.');
  } else if (!skipToast) {
    if (err.status === 403) {
      toast.error('You are not authorized for this site.');
    } else if (err.status === 0) {
      toast.error('Cannot reach the server. Is the API running?');
    } else {
      toast.error(apiMsg ?? `Something went wrong (${err.status}).`);
    }
  }

  return throwError(() => err);
})

  );
};
````

## File: docanalytics-web/src/app/features/activity-log/activity-log.component.ts
````typescript
import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, effect, inject, untracked } from '@angular/core';
import { ActivityLogService } from './activity-log.service';
import { ActivityLogItem, ActivityLogSortBy } from './activity-log.models';
import { SiteContextService } from '../../core/services/site-context.service';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import {
  FilterBarComponent, FilterOption, FilterValues,
} from '../../shared/components/filter-bar/filter-bar.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table/data-table.component';

@Component({
  selector: 'app-activity-log',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, StatusBadgeComponent, FilterBarComponent, DataTableComponent, DtCellDirective,
  ],

  templateUrl: './activity-log.component.html',
  styleUrl: './activity-log.component.css',
})
export class ActivityLogComponent {
  protected svc = inject(ActivityLogService);
  private siteCtx = inject(SiteContextService);
  private searchTimer: any;

  protected readonly eventTypeOptions: FilterOption[] = [
    { value: 'all', label: 'All events' },
    { value: 'FILE_STATE_CHANGED', label: 'File state changed' },
    { value: 'BATCH_SUBMITTED', label: 'Batch submitted' },
    { value: 'BATCH_COMPLETED', label: 'Batch completed' },
    { value: 'BATCH_FAILED', label: 'Batch failed' },
  ];

  // 'transition' has no backing field → rendered via the dtCell template (not sortable)
  protected readonly columns: ColumnDef<ActivityLogItem>[] = [
    { key: 'ts', header: 'Timestamp', sortable: true, width: '190px' },
    { key: 'event_type', header: 'Event', sortable: true },
    { key: 'entity', header: 'Entity', sortable: true },
    { key: 'transition', header: 'Change' },
    { key: 'actor', header: 'Actor', width: '120px' },
  ];

  private static readonly EVENT_LABELS: Record<string, string> = {
    FILE_STATE_CHANGED: 'File state changed',
    BATCH_SUBMITTED: 'Batch submitted',
    BATCH_COMPLETED: 'Batch completed',
    BATCH_FAILED: 'Batch failed',
  };

  constructor() {
    // load on entry + reload on site switch; loader runs untracked so query-signal
    // reads inside load() don't re-fire the effect (the R3/R4 lesson).
    effect(() => {
      const site = this.siteCtx.selectedSiteId();
      if (!site) return;
      untracked(() => this.svc.load());
    });
  }

  protected eventLabel(t: string): string { return ActivityLogComponent.EVENT_LABELS[t] ?? t; }

  protected onFilters(f: FilterValues): void {
    this.svc.setFilters({
      eventType: f.status === 'all' ? null : f.status,   // first field repurposed as Event type
      from: f.from,
      to: f.to,
    });
  }

  protected onSort(s: SortState): void {
    this.svc.setSort(s.sortBy as ActivityLogSortBy, s.sortDir);
  }

  protected onSearch(e: Event): void {
    const v = (e.target as HTMLInputElement).value;
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => this.svc.setEntitySearch(v), 300);  // debounce
  }
}
````

## File: docanalytics-web/src/app/features/errors/error.service.ts
````typescript
import { HttpClient, HttpContext, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { SortDir } from '../../shared/components/data-table/data-table.component';
import { ChartPoint, ErrorListItem, ErrorQuery, ErrorSortBy } from './errors.models';

const DEFAULT_QUERY: ErrorQuery = {
  page: 1, pageSize: 20, step: null, source: null,
  from: null, to: null, sortBy: 'failed_at', sortDir: 'desc', // ⚠️ VERIFY default sort token
};
const TOP_N = 10;

@Injectable({ providedIn: 'root' })
export class ErrorService {
  private http = inject(HttpClient);
  private base = environment.apiBase;
  private silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  // ===== 1 · Top-10 frequencies =====
  private _top = signal<ChartPoint[]>([]);
  private _topLoading = signal(false);
  private _topError = signal<string | null>(null);
  readonly top = this._top.asReadonly();
  readonly topLoading = this._topLoading.asReadonly();
  readonly topError = this._topError.asReadonly();

  loadTop(): void {
    const params = new HttpParams().set('topN', TOP_N);
    this._topLoading.set(true); this._topError.set(null);
    this.http.get<ApiResponse<{ points: ChartPoint[] }>>(`${this.base}/errors/top-frequencies`, { params, ...this.silent })
      .pipe(finalize(() => this._topLoading.set(false)))
      .subscribe({
        next: (res) => this._top.set(res.data?.points ?? []),
        error: () => this._topError.set('Could not load top errors. Please retry.'),
      });
  }

  // ===== 2 · Trend (respects from/to) =====
  private _trend = signal<ChartPoint[]>([]);
  private _trendLoading = signal(false);
  private _trendError = signal<string | null>(null);
  readonly trend = this._trend.asReadonly();
  readonly trendLoading = this._trendLoading.asReadonly();
  readonly trendError = this._trendError.asReadonly();

  loadTrend(): void {
    const q = this._query();
    let params = new HttpParams();
    if (q.from) params = params.set('from', q.from);
    if (q.to) params = params.set('to', q.to);
    this._trendLoading.set(true); this._trendError.set(null);
    this.http.get<ApiResponse<{ points: ChartPoint[] }>>(`${this.base}/errors/trend`, { params, ...this.silent })
      .pipe(finalize(() => this._trendLoading.set(false)))
      .subscribe({
        next: (res) => this._trend.set(res.data?.points ?? []),
        error: () => this._trendError.set('Could not load error trend. Please retry.'),
      });
  }

  // ===== 3 · Filtered/paginated errors list =====
  private _errors = signal<ErrorListItem[]>([]);
  private _meta = signal<Meta | null>(null);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  private _query = signal<ErrorQuery>({ ...DEFAULT_QUERY });
  readonly errors = this._errors.asReadonly();
  readonly meta = this._meta.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly query = this._query.asReadonly();

  // lowercase param keys — ASP.NET binding is case-insensitive; matches Akash's working batches code
  private buildParams(q: ErrorQuery): HttpParams {
    let params = new HttpParams()
      .set('page', q.page).set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy).set('sortDir', q.sortDir);
    if (q.step) params = params.set('step', q.step);
    if (q.source) params = params.set('source', q.source);
    if (q.from) params = params.set('from', q.from);
    if (q.to) params = params.set('to', q.to);
    return params;
  }

  loadErrors(): void {
    const params = this.buildParams(this._query());
    this._loading.set(true); this._error.set(null);
    this.http.get<ApiResponse<ErrorListItem[]>>(`${this.base}/errors`, { params, ...this.silent })
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (res) => { this._errors.set(res.data ?? []); this._meta.set(res.meta ?? null); },
        error: () => this._error.set('Could not load errors. Please retry.'),
      });
  }

  private patch(p: Partial<ErrorQuery>, resetPage = true): void {
    this._query.update(q => ({ ...q, ...p, page: resetPage ? 1 : (p.page ?? q.page) }));
    this.loadErrors();
  }

  // FilterBar's first field is repurposed as STEP on this page ('all' → null)
  setFilters(f: { status: string; source: string | null; from: string | null; to: string | null }): void {
    const step = f.status && f.status !== 'all' ? f.status : null;
    this.patch({ step, source: f.source, from: f.from, to: f.to });
    this.loadTrend(); // trend follows the same date range
  }
  setSort(sortBy: ErrorSortBy, sortDir: SortDir): void { this.patch({ sortBy, sortDir }); }
  setPage(page: number): void { this.patch({ page }, false); }
  setPageSize(pageSize: number): void { this.patch({ pageSize }); }

  // ===== 4 · CSV export (applies current filters; keeps server filename) =====
  private _exporting = signal(false);
  private _exportError = signal<string | null>(null);
  readonly exporting = this._exporting.asReadonly();
  readonly exportError = this._exportError.asReadonly();

  exportCsv(): void {
    const params = this.buildParams(this._query());
    this._exporting.set(true); this._exportError.set(null);
    this.http.get(`${this.base}/errors/export`, {
      params, observe: 'response', responseType: 'blob', ...this.silent,
    })
      .pipe(finalize(() => this._exporting.set(false)))
      .subscribe({
        next: (res) => this.saveBlob(res),
        error: () => this._exportError.set('CSV export failed. Please retry.'),
      });
  }

  private saveBlob(res: HttpResponse<Blob>): void {
    const blob = res.body;
    if (!blob) { this._exportError.set('CSV export returned no file.'); return; }
    const cd = res.headers.get('content-disposition') ?? '';
    const m = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(cd);
    const filename = m ? decodeURIComponent(m[1]) : 'errors_export.csv';
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url; a.download = filename;
    document.body.appendChild(a); a.click();
    a.remove(); URL.revokeObjectURL(url);
  }

  load(): void { this.loadTop(); this.loadTrend(); this.loadErrors(); }
}
````

## File: DocAnalytics.Service/Batches/BatchService.cs
````csharp
using DocAnalytics.Data;                 // ① AppDbContext lives here
using DocAnalytics.Domain.Entities;      // ① the Transaction entity
using DocAnalytics.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.Batches;


public sealed class BatchService : IBatchService
{
    private readonly AppDbContext _db;

    // ② constructor injection — the DbContext is handed to us
    public BatchService(AppDbContext db) => _db = db;

    public async Task<PagedResult<BatchListItemDto>> GetBatchesAsync(
        BatchListQuery query, CancellationToken ct = default)
    {
        // --- normalise paging (never trust raw input) ---
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        // ③ start a query. tenant_id + site_id filter is AUTO-applied.
        IQueryable<Transaction> q = _db.Transactions.AsNoTracking();

        // ④ ---- FILTERS (added only if provided) ----
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            !query.Status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            var state = MapStatusToState(query.Status);
            if (state is not null)
                q = q.Where(b => b.State == state);
        }

        if (!string.IsNullOrWhiteSpace(query.Source))
            q = q.Where(b => b.SourceSystem == query.Source);

        if (query.From.HasValue)
        {
            var fromUtc = query.From.Value.AsUtc();
            q = q.Where(b => b.SubmittedAt >= fromUtc);
        }
        if (query.To.HasValue)
        {
            var toUtc = query.To.Value.AsUtc();
            q = q.Where(b => b.SubmittedAt <= toUtc);
        }


        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(b => EF.Functions.ILike(b.Id.ToString(), $"%{term}%"));
        }

        // ⑤ ---- COUNT before paging ----
        var totalCount = await q.CountAsync(ct);

        // ⑥ ---- SORT (whitelisted) ----
        q = ApplySorting(q, query.SortBy, query.SortDir);

        // ⑦ ---- PAGE + SHAPE into DTOs ----
        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BatchListItemDto
            {
                TransactionId = b.Id,
                State = b.State,
                SourceSystem = b.SourceSystem,
                TotalFiles = b.TotalFiles,
                UploadedCount = b.UploadedCount,
                ProcessingCount = b.ProcessingCount,
                FailedCount = b.FailedCount,
                CompletedCount = b.CompletedCount,
                SubmittedAt = b.SubmittedAt,
                LastUpdatedAt = b.LastUpdatedAt,
                CompletedAt = b.CompletedAt
            })
            .ToListAsync(ct);

        return new PagedResult<BatchListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // Distinct source systems for the current tenant/site (Transactions is ITenantScoped
    // → tenant_id + site_id auto-applied by the global query filter).
    public async Task<List<string>> GetSourcesAsync(CancellationToken ct = default)
    {
        return await _db.Transactions
            .AsNoTracking()
            .Select(t => t.SourceSystem)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(ct);
    }


    // ── GET /api/v1/batches/{id} : drill into ONE batch ──
    public async Task<BatchDetailDto?> GetBatchByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        return await _db.Transactions
            .AsNoTracking()
            .Where(b => b.Id == id)            // tenant_id + site_id auto-added by the filter
            .Select(b => new BatchDetailDto
            {
                Id = b.Id,
                Status = b.State,
                Source = b.SourceSystem,
                TotalFiles = b.TotalFiles,
                FileStats = new FileStatsDto
                {
                    Uploaded = b.UploadedCount,
                    Processing = b.ProcessingCount,
                    Failed = b.FailedCount,
                    Completed = b.CompletedCount
                },
                Times = new BatchTimesDto
                {
                    SubmittedAt = b.SubmittedAt,
                    LastUpdatedAt = b.LastUpdatedAt,
                    CompletedAt = b.CompletedAt
                }
            })
            .FirstOrDefaultAsync(ct);          // null = not found
    }

    // ── GET /api/v1/batches/{id}/files : list the files in ONE batch (paged) ──
    public async Task<PagedResult<BatchFileDto>?> GetBatchFilesAsync(
        Guid id, BatchFilesQuery query, CancellationToken ct = default)
    {
        // 1. batch exists? (for this tenant/site) — if not → null → 404
        var batchExists = await _db.Transactions
            .AsNoTracking()
            .AnyAsync(b => b.Id == id, ct);

        if (!batchExists)
            return null;

        // 2. normalise paging
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        // 3. ONE query against files, filtered to this batch (no N+1)
        var q = _db.Files
            .AsNoTracking()
            .Where(f => f.TransactionId == id);

        // 4. count before paging
        var totalCount = await q.CountAsync(ct);

        // 5. order → page → shape
        var items = await q
        .OrderByDescending(f => f.CreatedAt)
        .ThenBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new BatchFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                FileType = f.FileType,
                Status = f.Status,
                CurrentStep = f.CurrentStep,
                FileSizeBytes = f.FileSizeBytes,
                CreatedAt = f.CreatedAt,
                LastUpdatedAt = f.LastUpdatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<BatchFileDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }


    // friendly API word -> the DB's state value
    private static string? MapStatusToState(string status) =>
        status.ToLowerInvariant() switch
        {
            "failed" => "Failed",
            "completed" => "Completed",
            "in_progress" => "Processing",
            "queued" => "Queued",     // ← ADD THIS
            _ => null
        };

    // ⑥ ONLY these columns can be sorted -> blocks SQL injection via sortBy
    private static IQueryable<Transaction> ApplySorting(
        IQueryable<Transaction> q, string? sortBy, string? sortDir)
    {
        var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);

        return (sortBy ?? "last_updated").ToLowerInvariant() switch
        {
            "submitted_at" => desc ? q.OrderByDescending(b => b.SubmittedAt)
                                                : q.OrderBy(b => b.SubmittedAt),
            "state" or "status" => desc ? q.OrderByDescending(b => b.State)
                                                : q.OrderBy(b => b.State),
            "source" or "source_system" => desc ? q.OrderByDescending(b => b.SourceSystem)
                                                : q.OrderBy(b => b.SourceSystem),
            "total_files" => desc ? q.OrderByDescending(b => b.TotalFiles)
                                                : q.OrderBy(b => b.TotalFiles),
            _ => desc ? q.OrderByDescending(b => b.LastUpdatedAt)
                                                : q.OrderBy(b => b.LastUpdatedAt)
        };
    }
}
````

## File: docanalytics-web/src/app/features/auth/login.component.ts
````typescript
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',       
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  constructor() {
    // If a valid session already exists (e.g. user hits /login with a live token),
    // skip the form and go straight to their first site's dashboard.
    this.auth.ensureSession().then((ok) => {
      if (ok) this.goToFirstSite();
    });
  }

  isInvalid(name: 'email' | 'password'): boolean {
    const c = this.form.controls[name];
    return c.invalid && (c.touched || c.dirty);
  }

  submit(): void {
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { email, password } = this.form.getRawValue();

    this.auth.login(email, password).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.error || !res.data) {
          this.errorMessage.set('Invalid email or password.');
          return;
        }
        if (!this.goToFirstSite()) {
          this.errorMessage.set('Your account has no site access. Contact your administrator.');
          this.auth.logout();
        }
      },
      // Login 401 is handled HERE locally (not via the global "Session expired" toast).
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        if (err.status === 401) {
          this.errorMessage.set('Invalid email or password.');
        } else if (err.status === 0) {
          this.errorMessage.set('Cannot reach the server. Check your connection and try again.');
        } else {
          this.errorMessage.set('Something went wrong. Please try again.');
        }
      },
    });
  }

  /** Navigate to the first authorized site's dashboard. Returns false if none. */
  private goToFirstSite(): boolean {
    const sites = this.auth.sites();
    if (!sites.length) return false;
    this.router.navigate(['/site', sites[0].site_id, 'dashboard']);
    return true;
  }
}
````

## File: docanalytics-web/src/app/features/batches/batch.models.ts
````typescript
import { SortDir } from '../../shared/components/data-table/data-table.component';

// ── Batch List (Dev A · FR-2.1–2.3) ──
export interface BatchListItem {
  transaction_id: string;
  state: string;                 // Processing | Completed | Failed
  source_system: string;
  total_files: number;
  uploaded_count: number;
  processing_count: number;
  failed_count: number;
  completed_count: number;
  submitted_at: string;
  last_updated_at: string;
  completed_at: string | null;
}
export type BatchSortBy = 'last_updated' | 'submitted_at' | 'state' | 'source_system' | 'total_files';
export interface BatchListQuery {
  page: number; pageSize: number; status: string; source: string | null;
  from: string | null; to: string | null; search: string | null;
  sortBy: BatchSortBy; sortDir: SortDir;
}

// ── Batch Detail + Files (Dev B · FR-2.4) ──
export interface BatchFileStats { uploaded: number; processing: number; failed: number; completed: number; }
export interface BatchTimes { submitted_at: string; last_updated_at: string; completed_at: string | null; }
export interface BatchDetail {
  id: string; status: string; source: string; total_files: number;
  file_stats: BatchFileStats; times: BatchTimes;
}
export interface BatchFile {
  id: string; file_name: string; file_type: string; status: string;
  current_step: string | null; file_size_bytes: number;
  created_at: string; last_updated_at: string;
}
export interface FilesQuery { page: number; pageSize: number; }
````

## File: docanalytics-web/src/app/features/dashboard/dashboard.service.ts
````typescript
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { ChartSeries, SeriesPoint } from '../../core/models/dashboard.model';
import {
  DashboardSummary, FailuresSortBy, RecentFailure, RecentFailuresQuery,
} from './dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBase;

  // tells the error interceptor "don't toast — widgets show errors inline"
  private readonly silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  // ───────── Dev A · Summary (FR-1.1) ─────────
  private readonly _summary = signal<DashboardSummary | null>(null);
  private readonly _summaryLoading = signal(false);
  private readonly _summaryError = signal<string | null>(null);
  readonly summary = this._summary.asReadonly();
  readonly summaryLoading = this._summaryLoading.asReadonly();
  readonly summaryError = this._summaryError.asReadonly();

  loadSummary(): void {
    this._summaryLoading.set(true);
    this._summaryError.set(null);
    this.http.get<ApiResponse<DashboardSummary>>(`${this.base}/dashboard/summary`, this.silent)
      .pipe(finalize(() => this._summaryLoading.set(false)))
      .subscribe({
        next: (res) => this._summary.set(res.data),
        error: () => this._summaryError.set('Could not load summary counters.'),
      });
  }

  // ───────── Dev A · Recent Failures (FR-1.4) ─────────
  private readonly _failures = signal<RecentFailure[]>([]);
  private readonly _failuresMeta = signal<Meta | null>(null);
  private readonly _failuresLoading = signal(false);
  private readonly _failuresError = signal<string | null>(null);
  private readonly _failuresQuery = signal<RecentFailuresQuery>({
    page: 1, pageSize: 10, sortBy: 'failed_at', sortDir: 'desc',
  });
  readonly failures = this._failures.asReadonly();
  readonly failuresMeta = this._failuresMeta.asReadonly();
  readonly failuresLoading = this._failuresLoading.asReadonly();
  readonly failuresError = this._failuresError.asReadonly();
  readonly failuresQuery = this._failuresQuery.asReadonly();

  loadFailures(): void {
    const q = this._failuresQuery();
    this._failuresLoading.set(true);
    this._failuresError.set(null);
    const params = new HttpParams()
      .set('page', q.page).set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy).set('sortDir', q.sortDir);
    this.http.get<ApiResponse<RecentFailure[]>>(
      `${this.base}/dashboard/recent-failures`, { params, ...this.silent })
      .pipe(finalize(() => this._failuresLoading.set(false)))
      .subscribe({
        next: (res) => { this._failures.set(res.data ?? []); this._failuresMeta.set(res.meta ?? null); },
        error: () => this._failuresError.set('Could not load recent failures.'),
      });
  }

  setFailuresSort(sortBy: FailuresSortBy, sortDir: 'asc' | 'desc'): void {
    this._failuresQuery.update((q) => ({ ...q, sortBy, sortDir, page: 1 }));
    this.loadFailures();
  }
  setFailuresPage(page: number): void {
    this._failuresQuery.update((q) => ({ ...q, page }));
    this.loadFailures();
  }
  setFailuresPageSize(pageSize: number): void {
    this._failuresQuery.update((q) => ({ ...q, pageSize, page: 1 }));
    this.loadFailures();
  }

  // ───────── Dev B · Throughput + Status Distribution ─────────
  readonly throughput = signal<SeriesPoint[]>([]);
  readonly throughputLoading = signal(false);
  readonly throughputError = signal<string | null>(null);
  readonly statusDistribution = signal<SeriesPoint[]>([]);
  readonly distributionLoading = signal(false);
  readonly distributionError = signal<string | null>(null);

  loadThroughput(): void {
    this.throughputLoading.set(true);
    this.throughputError.set(null);
    this.http.get<ApiResponse<ChartSeries>>(`${this.base}/dashboard/throughput`, this.silent)
      .pipe(finalize(() => this.throughputLoading.set(false)))
      .subscribe({
        next: (res) => this.throughput.set(res.data?.points ?? []),
        error: () => this.throughputError.set('Could not load throughput.'),
      });
  }

  loadStatusDistribution(): void {
    this.distributionLoading.set(true);
    this.distributionError.set(null);
    this.http.get<ApiResponse<ChartSeries>>(`${this.base}/dashboard/status-distribution`, this.silent)
      .pipe(finalize(() => this.distributionLoading.set(false)))
      .subscribe({
        next: (res) => this.statusDistribution.set(res.data?.points ?? []),
        error: () => this.distributionError.set('Could not load status distribution.'),
      });
  }

  // ───────── CO-OWNED poll target (FR-1.5) ─────────
  readonly lastUpdated = signal<Date | null>(null);
  refreshAll(): void {
    this.loadSummary();
    this.loadFailures();
    this.loadThroughput();
    this.loadStatusDistribution();
    this.lastUpdated.set(new Date());
  }
}
````

## File: docanalytics-web/src/app/features/files/file-details.component.ts
````typescript
import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy, Component, computed, effect, inject, untracked,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { FileDetailsService } from './file-details.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import { InvoiceLineItem, StepHistoryItem } from './file-details.models';

@Component({
  selector: 'app-file-details',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, RouterLink, StatusBadgeComponent],

  templateUrl: './file-details.component.html',
  styleUrl: './file-details.component.css',
})
export class FileDetailsComponent {
  protected readonly svc = inject(FileDetailsService);
  private readonly route = inject(ActivatedRoute);
  private readonly site = inject(SiteContextService);

  private readonly fileId = toSignal(
    this.route.paramMap.pipe(map((p) => p.get('fileId'))),
    { initialValue: this.route.snapshot.paramMap.get('fileId') },
  );

  protected readonly info = computed(() => this.svc.detail()?.file_info ?? null);
  protected readonly history = computed<StepHistoryItem[]>(() => this.svc.detail()?.history ?? []);
  protected readonly items = computed<InvoiceLineItem[]>(() => this.svc.invoice()?.items ?? []);

  constructor() {
    // reload on file switch (param-only nav) AND on site switch — both tracked,
    // loads run in untracked so query reads inside don't re-fire the effect (R3 lesson).
    effect(() => {
      const id = this.fileId();
      this.site.selectedSiteId();
      if (!id) return;
      untracked(() => this.svc.load(id));
    });
  }

  protected isFailed(s: StepHistoryItem): boolean {
    return s.status?.toLowerCase() === 'failed';
  }
  protected stepClass(s: StepHistoryItem): 'success' | 'failed' | 'processing' {
    const v = s.status?.toLowerCase();
    if (v === 'failed') return 'failed';
    if (v === 'processing') return 'processing';
    return 'success';
  }
  protected num(v: number | null | undefined, dp: number): string {
    return v == null ? '—' : Number(v).toFixed(dp);
  }
  protected pct(c: number | null): string {
    return c == null ? '—' : (c * 100).toFixed(1) + '%';
  }
}
````

## File: docanalytics-web/src/styles.css
````css
:root {
  /* ===== AVEVA Digital UI Palette (exact hex from style guide) ===== */
  --aveva-purple: #3D1152; /* brand chrome ONLY: top bar, sidebar, active tab */
  --purple: #6B04A8; /* marketing background fills only */
  --bright-purple: #C530FF;
  /* Neutrals / "furniture" */
  --black: #000000;
  --dark-gray: #363D42; /* DEFAULT TEXT (light mode) */
  --dark-gray-3: #545C69; /* secondary text, eyebrow labels */
  --cool-gray: #BECCD6; /* disabled, borders */
  --light-gray: #F2F5F7;
  --bg-light: #F6F8FA; /* page canvas (light) */
  --bg-dark: #23282B; /* page canvas (dark mode) */
  --white: #FFFFFF; /* card fill (light) */
  /* Interactive — Slate Blue, NEVER purple */
  --slate-blue: #4D5EE0;
  --slate-blue-3: #78A1FC;
  /* Status / Feedback (fill + icon ONLY) */
  --status-neutral: #01A9F4;
  --status-confirmed: #009848;
  --status-warning: #F5A624;
  --status-error: #DC0A0A;
  /* Light-mode feedback TEXT colors */
  --text-neutral: #363D42;
  --text-confirmed: #217046;
  --text-warning: #7D5D29;
  --text-error: #8F2727;
  /* Typography */
  --font-display: 'Barlow', sans-serif;
  --font-body: 'Mulish', 'Muli', sans-serif;
  /* 8px grid spacing scale */
  --space-1: 8px;
  --space-2: 16px;
  --space-3: 24px;
  --space-4: 32px;
  --space-5: 40px;
  --space-9: 72px;
}

* {
  box-sizing: border-box;
}

html, body {
  margin: 0;
  padding: 0;
  background: var(--bg-light);
}

body {
  font-family: var(--font-body);
  color: var(--dark-gray);
  font-size: 16px;
  line-height: 24px;
}

h1, h2, h3, h4 {
  font-family: var(--font-display);
  color: var(--dark-gray);
}

a {
  color: var(--slate-blue);
  text-decoration: none;
}

  a:hover {
    text-decoration: underline;
  }

/* ===== S-2 Dark mode — Dev B, R5. Scoped overrides only; :root untouched. ===== */
[data-theme="dark"] {
  /* Surfaces */
  --bg-light: #23282B; /* page canvas → reuses existing --bg-dark hue */
  --white: #2C3338; /* cards / topbar / table fill */
  --light-gray: #2C3338; /* raised fills / skeleton base */
  /* Text + borders (invert the greys) */
  --dark-gray: #E6EAF0; /* primary text */
  --dark-gray-3: #AEB8C4; /* secondary text / eyebrow */
  --cool-gray: #3C454C; /* borders / dividers / skeleton highlight */
  /* Interactive — lift slate blue for contrast on dark */
  --slate-blue: #78A1FC; /* = your existing --slate-blue-3 */
  /* Feedback TEXT — brighten for legibility on dark bg */
  --text-error: #FF9B9B;
  --text-warning: #F0B95E;
  --text-confirmed: #5FD08A;
  --text-neutral: #C9D2DA;
}
````

## File: README.md
````markdown
# Document Processing Analytics Platform

A **multi-tenant document-processing analytics & monitoring backend** built with **ASP.NET Core (.NET 10)** and **PostgreSQL 18**. It ingests batches of documents (PDF invoices, CSV manifests), tracks each file through a pipeline (**Upload → Validate → Transform → Load**), records errors + remediation, extracts invoice line items, and serves dashboards/analytics over a clean REST API.

> **📖 Full docs live in the [Wiki](../../wiki)** — architecture, database, isolation model, API examples, and troubleshooting. This README just gets you running.

> **Status:** Backend feature-complete (Phase 0 + all 5 rounds + site-level access enforcement / FR-5.3). Frontend (Angular, DT-3) next.

---

## Quick Start

### Prerequisites
- **.NET SDK 10.x** · **PostgreSQL 18** · **Git** · `dotnet-ef` global tool
  (`dotnet tool install --global dotnet-ef`, then open a fresh terminal)
- Remember the **`postgres`** superuser password you set during install.

### 1. Clone & open
```bash
git clone https://github.com/Akash29g/Document_Processing_Analytics.git
cd Document_Processing_Analytics
```
Open **`DocAnalytics.slnx`** (don't create a new project).

### 2. Set secrets (git-ignored)
```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=docanalytics;Username=postgres;Password=YOUR_LOCAL_PW" --project DocAnalytics.Api
dotnet user-secrets set "Jwt:Key" "any-32+-character-secret-key-for-local-dev" --project DocAnalytics.Api
```
> `Jwt:Key` must be ≥ 32 chars. Username/password must match a real PostgreSQL role.

### 3. Create the database (EF Core does it all)
```bash
dotnet ef database update --project DocAnalytics.Data --startup-project DocAnalytics.Api
```
If `docanalytics` doesn't exist, EF creates it, then builds all 12 tables. No hand-written SQL — the schema lives in migrations.

### 4. Run
```bash
dotnet watch run --project DocAnalytics.Api   # auto-opens Swagger + hot-reload
# or: dotnet run --project DocAnalytics.Api    # then open http://localhost:<port>/swagger
```
First run also **seeds** the database (2 tenants, users, batches, etc.).

### 5. Log in
Seed users (password `Password123!` for all): `user.a@acme.com`, `admin@acme.com`, `user.b@acme.com`, `user.c@globex.com`, `admin@globex.com`. Full roster + smoke test in the **[API Reference wiki page](../../wiki/API-Reference)**.

---

## Tech Stack
ASP.NET Core Web API (**.NET 10**) · EF Core 10 (migration-based) · **PostgreSQL 18** · JWT auth · BCrypt · Swagger/Swashbuckle · snake_case via EFCore.NamingConventions.

> Design docs say ".NET 8" but the project targets **.NET 10 (`net10.0`)** — that's the source of truth.

## Architecture (one-liner)
`Api → Service → Data → Domain`. Controllers never touch `AppDbContext` — they call a service; an EF global query filter enforces tenant/site scoping. Details: **[Architecture wiki](../../wiki/Architecture)**.

## Tenant & Site Isolation
Four layers (data columns → JWT → global query filter → `UserSiteAccess` 403 enforcement / FR-5.3) keep customers and sites separate. Details: **[Tenant and Site Isolation wiki](../../wiki/Tenant-and-Site-Isolation)**.

## Git Workflow
`main` is the runnable baseline; work on `feature/*` or `fix/*` branches → PR → merge. Conventional Commits (`feat:`, `fix:`, `docs:`, …). Details: **[Git Workflow wiki](../../wiki/Git-Workflow)**.

## Troubleshooting
Common errors (`28P01`, `IDX10720`, empty results after DB reset) are covered in **[FAQ and Troubleshooting wiki](../../wiki/FAQ-and-Troubleshooting)**.

---

## Frontend (docanalytics-web)

The frontend is an **Angular 22** single-page app (standalone components + Signals) located in `docanalytics-web/`. It consumes the backend REST API under `/api/v1` and renders the Dashboard, Batch Explorer, Error Analysis, and Activity Log.

### Tech Stack

| Layer | Choice |
|---|---|
| Framework | Angular 22 (standalone components, Signals) |
| Routing | Lazy-loaded, nested/parameterized routes (`/site/:siteId/...`) |
| State | Angular Signals inside injectable services (one service per feature) |
| HTTP | `HttpClient` + functional interceptors (auth + site, global error) |
| Styling | CSS variables (AVEVA purple/white theme), functional-first layout |

### Prerequisites

- **Node.js 22+** and npm
- **Angular CLI 22** — `npm install -g @angular/cli`

### Setup & Run

```Windows Powershell
cd docanalytics-web
npm install        # one-time — installs dependencies (node_modules is git-ignored)
ng serve -o        # serves at http://localhost:4200
```


## Team
- **Dev A** — Akash Goswami
- **Dev B** — Shubh Gupta

Full design rationale: `Design_Tasks_1-3_updated.pdf`.
````

## File: docanalytics-web/src/app/features/batches/batch.service.ts
````typescript
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Meta } from '../../core/models/api-response.model';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { SortDir } from '../../shared/components/data-table/data-table.component';
import {
  BatchListItem, BatchListQuery, BatchSortBy,
  BatchDetail, BatchFile, FilesQuery,
} from './batch.models';

const DEFAULT_QUERY: BatchListQuery = {
  page: 1, pageSize: 20, status: 'all', source: null,
  from: null, to: null, search: null, sortBy: 'last_updated', sortDir: 'desc',
};

@Injectable({ providedIn: 'root' })
export class BatchService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBase;
  private readonly silent = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };

  // ─────────────────────────────────────────────
  // Batch List  (Dev A · FR-2.1–2.3)
  // ─────────────────────────────────────────────
  private _batches = signal<BatchListItem[]>([]);
  private _meta = signal<Meta | null>(null);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  private _query = signal<BatchListQuery>({ ...DEFAULT_QUERY });

  readonly batches = this._batches.asReadonly();
  readonly meta = this._meta.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly query = this._query.asReadonly();

  loadBatches(): void {
    const q = this._query();
    let params = new HttpParams()
      .set('page', q.page)
      .set('pageSize', q.pageSize)
      .set('sortBy', q.sortBy)
      .set('sortDir', q.sortDir);

    if (q.status && q.status !== 'all') params = params.set('status', q.status);
    if (q.source) params = params.set('source', q.source);
    if (q.from) params = params.set('from', q.from);
    if (q.to) params = params.set('to', q.to);
    if (q.search?.trim()) params = params.set('search', q.search.trim());

    this._loading.set(true);
    this._error.set(null);

    this.http.get<ApiResponse<BatchListItem[]>>(`${this.base}/batches`, { params, ...this.silent })
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: (res) => { this._batches.set(res.data ?? []); this._meta.set(res.meta ?? null); },
        error: () => this._error.set('Could not load batches. Please retry.'),
      });
  }

  private patch(p: Partial<BatchListQuery>, resetPage = true): void {
    this._query.update(q => ({ ...q, ...p, page: resetPage ? 1 : (p.page ?? q.page) }));
    this.loadBatches();
  }

  setFilters(f: { status: string; source: string | null; from: string | null; to: string | null }): void { this.patch(f); }
  setSearch(search: string): void { this.patch({ search }); }
  setSort(sortBy: BatchSortBy, sortDir: SortDir): void { this.patch({ sortBy, sortDir }, false); }
  setPage(page: number): void { this.patch({ page }, false); }
  setPageSize(pageSize: number): void { this.patch({ pageSize }); }
  reset(): void { this._query.set({ ...DEFAULT_QUERY }); this.loadBatches(); }

  // ── Source options for the FilterBar (distinct SourceSystem for this site)
  private _sources = signal<string[]>([]);
  readonly sources = this._sources.asReadonly();

  loadSources(): void {
    this.http.get<ApiResponse<string[]>>(`${this.base}/batches/sources`, this.silent)
      .subscribe({ next: (res) => this._sources.set(res.data ?? []) });
  }                          

  // ─────────────────────────────────────────────
  // Batch Detail + Files  (Dev B · FR-2.4)
  // ─────────────────────────────────────────────
  private readonly _batchId = signal<string | null>(null);

  private readonly _detail = signal<BatchDetail | null>(null);
  private readonly _detailLoading = signal(false);
  private readonly _detailError = signal<string | null>(null);
  readonly detail = this._detail.asReadonly();
  readonly detailLoading = this._detailLoading.asReadonly();
  readonly detailError = this._detailError.asReadonly();

  loadDetail(): void {
    const id = this._batchId();
    if (!id) return;
    this._detailLoading.set(true); this._detailError.set(null);
    this.http.get<ApiResponse<BatchDetail>>(`${this.base}/batches/${id}`, this.silent)
      .pipe(finalize(() => this._detailLoading.set(false)))
      .subscribe({
        next: (res) => this._detail.set(res.data ?? null),
        error: () => this._detailError.set('Could not load batch details.'),
      });
  }

  private readonly _files = signal<BatchFile[]>([]);
  private readonly _filesMeta = signal<Meta | null>(null);
  private readonly _filesLoading = signal(false);
  private readonly _filesError = signal<string | null>(null);
  private readonly _filesQuery = signal<FilesQuery>({ page: 1, pageSize: 10 });
  readonly files = this._files.asReadonly();
  readonly filesMeta = this._filesMeta.asReadonly();
  readonly filesLoading = this._filesLoading.asReadonly();
  readonly filesError = this._filesError.asReadonly();
  readonly filesQuery = this._filesQuery.asReadonly();

  loadFiles(): void {
    const id = this._batchId();
    if (!id) return;
    const q = this._filesQuery();
    this._filesLoading.set(true); this._filesError.set(null);
    const params = new HttpParams().set('page', q.page).set('pageSize', q.pageSize);
    this.http.get<ApiResponse<BatchFile[]>>(`${this.base}/batches/${id}/files`, { params, ...this.silent })
      .pipe(finalize(() => this._filesLoading.set(false)))
      .subscribe({
        next: (res) => { this._files.set(res.data ?? []); this._filesMeta.set(res.meta ?? null); },
        error: () => this._filesError.set('Could not load files.'),
      });
  }
  setFilesPage(page: number): void { this._filesQuery.update(q => ({ ...q, page })); this.loadFiles(); }
  setFilesPageSize(pageSize: number): void { this._filesQuery.update(q => ({ ...q, pageSize, page: 1 })); this.loadFiles(); }

  load(batchId: string): void {
    this._batchId.set(batchId);
    this._filesQuery.set({ page: 1, pageSize: 10 });
    this.loadDetail();
    this.loadFiles();
  }
}
````

## File: docanalytics-web/src/app/features/batches/batch-list.component.ts
````typescript
import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, untracked } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BatchService } from './batch.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { FilterBarComponent, FilterValues, FilterOption } from '../../shared/components/filter-bar/filter-bar.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table/data-table.component';
import { BatchListItem, BatchSortBy } from './batch.models';

@Component({
  selector: 'app-batch-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    DatePipe, RouterLink, FilterBarComponent, StatusBadgeComponent,
    DataTableComponent, DtCellDirective,
  ],

  templateUrl: './batch-list.component.html',
  styleUrl: './batch-list.component.css',

})
export class BatchListComponent {
  protected svc = inject(BatchService);
  private site = inject(SiteContextService);
  private destroyRef = inject(DestroyRef);
  private searchTimer?: ReturnType<typeof setTimeout>;

  // Status filter — VALUE 'in_progress' stays (backend maps → Processing); LABEL reads "Processing".
  // 'queued' now supported after the backend MapStatusToState fix.
  protected statusOptions: FilterOption[] = [
    { value: 'all', label: 'All statuses' },
    { value: 'queued', label: 'Queued' },
    { value: 'in_progress', label: 'Processing' },
    { value: 'completed', label: 'Completed' },
    { value: 'failed', label: 'Failed' },
  ];

  // built from the endpoint
  protected sourceOptions = computed<FilterOption[]>(() =>
    this.svc.sources().map(s => ({ value: s, label: s })),
  );

  // Column keys = backend sort tokens. 'last_updated' is a sort token, so its
  // display value is pulled from last_updated_at via the accessor + cell template.
  protected columns: ColumnDef<BatchListItem>[] = [
    { key: 'transaction_id', header: 'Batch ID', width: '300px' },
    { key: 'state', header: 'Status', sortable: true },
    { key: 'total_files', header: 'Files', sortable: true, align: 'right' },
    { key: 'source_system', header: 'Source', sortable: true },
    { key: 'submitted_at', header: 'Submitted', sortable: true },
    { key: 'last_updated', header: 'Updated', sortable: true, value: r => r.last_updated_at },
  ];

  constructor() {
    effect(() => {
      const siteId = this.site.selectedSiteId();
      if (!siteId) return;
      untracked(() => {
        this.svc.loadBatches();
        this.svc.loadSources();
      });
    });
  }

  onFilters(f: FilterValues) { this.svc.setFilters(f); }
  onSort(s: SortState) { this.svc.setSort(s.sortBy as BatchSortBy, s.sortDir); }

  onSearch(e: Event) {
    const v = (e.target as HTMLInputElement).value;
    clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => this.svc.setSearch(v), 350);
  }
}
````

## File: docanalytics-web/src/app/features/errors/errors.component.ts
````typescript
import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { ErrorService } from './error.service';
import { ErrorListItem, ErrorSortBy } from './errors.models';
import { SiteContextService } from '../../core/services/site-context.service';
import { ChartCardComponent } from '../../shared/components/chart-card/chart-card.component';
import { FilterBarComponent, FilterOption, FilterValues } from '../../shared/components/filter-bar/filter-bar.component';
import { ColumnDef, DataTableComponent, DtCellDirective, SortState } from '../../shared/components/data-table/data-table.component';

@Component({
  selector: 'app-errors',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, ChartCardComponent, FilterBarComponent, DataTableComponent, DtCellDirective],
  templateUrl: './errors.component.html',
  styleUrl: './errors.component.css',
})
export class ErrorsComponent {
  protected svc = inject(ErrorService);
  private site = inject(SiteContextService);

  // ⚠️ VERIFY step tokens/casing accepted by backend `step` filter (seed shows Validate/Transform/Load)
  protected stepOptions: FilterOption[] = [
    { value: 'all', label: 'All steps' },
    { value: 'Upload', label: 'Upload' },
    { value: 'Validate', label: 'Validate' },
    { value: 'Transform', label: 'Transform' },
    { value: 'Load', label: 'Load' },
  ];
  // ⚠️ VERIFY full source list from DbSeeder sources[]
  protected sourceOptions: FilterOption[] = [
    { value: 'S3_Bucket_Alpha', label: 'S3_Bucket_Alpha' },
    { value: 'SFTP_Beta', label: 'SFTP_Beta' },
    { value: 'Manual_Upload', label: 'Manual_Upload' },
    { value: 'API_Upload', label: 'API_Upload' },
    { value: 'Azure_Blob_Gamma', label: 'Azure_Blob_Gamma' },
  ];

  protected columns: ColumnDef<ErrorListItem>[] = [
    { key: 'failed_at', header: 'Failed At', sortable: true, width: '170px' },
    { key: 'file_name', header: 'File', sortable: true },
    { key: 'error_code', header: 'Error', sortable: true, width: '160px' },
    { key: 'error_message', header: 'Message' },
    { key: 'step', header: 'Step', sortable: true, width: '110px' },
    { key: 'source', header: 'Source', sortable: true, width: '150px' },
    { key: 'suggested_fix', header: 'Suggested Fix' },
  ];

  protected topMax = computed(() => Math.max(1, ...this.svc.top().map(p => p.value)));
  protected trendMax = computed(() => Math.max(1, ...this.svc.trend().map(p => p.value)));

  constructor() {
    // reload everything on site switch (same guarded-effect pattern as batches)
    effect(() => { const s = this.site.selectedSiteId(); if (s) this.svc.load(); });
  }

  protected pct(v: number, max: number): number { return Math.round((v / max) * 100); }
  protected shortDate(label: string): string { return label?.length >= 10 ? label.slice(5) : label; } // MM-DD
  protected onFilters(f: FilterValues): void { this.svc.setFilters(f); }
  protected onSort(s: SortState): void { this.svc.setSort(s.sortBy as ErrorSortBy, s.sortDir); }
}
````

## File: DocAnalytics.Api/Program.cs
````csharp
using DocAnalytics.Api.Extensions;
using DocAnalytics.Api.Middleware;
using DocAnalytics.Api.Swagger;
using DocAnalytics.Data;
using DocAnalytics.Data.Seeding;
using DocAnalytics.Service;
using DocAnalytics.Service.Dashboard;
using DocAnalytics.Service.Auth;
using DocAnalytics.Service.Batches;
using DocAnalytics.Service.Health;
using DocAnalytics.Service.Invoices;
using DocAnalytics.Service.Analytics;
using Microsoft.OpenApi;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCurrentUser();                              // Api
builder.Services.AddPersistence(builder.Configuration);         // Data
builder.Services.AddApplicationServices();                      // Service
builder.Services.AddJwtAuth(builder.Configuration);             // Api
builder.Services.AddSwaggerWithJwt();                           // Api
builder.Services.AddBatchFeature();
builder.Services.AddHealthFeature();
builder.Services.AddAuthFeature();
builder.Services.AddDashboardFeature();
builder.Services.AddInvoiceFeature();
builder.Services.AddFileDetailsFeature();
builder.Services.AddAnalyticsFeature();
builder.Services.AddErrorListFeature();
builder.Services.AddActivityLogFeature();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddValidationBehavior();   // <- Piece B: bad input -> ApiResponse.Fail (400)

builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.WithOrigins("http://localhost:4200")
     .AllowAnyHeader()     // allows Authorization + X-Site-Id
     .AllowAnyMethod()));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

app.UseMiddleware<ExceptionHandlingMiddleware>();   // outermost net — catches everything below
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantSiteMiddleware>();
app.MapControllers();

app.Run();
````

## File: docanalytics-web/src/app/app.routes.ts
````typescript
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { siteAccessGuard } from './core/guards/site-access.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login.component').then((m) => m.LoginComponent),
  },
  {
    // Everything under a site is guarded + rendered inside Shubh's App shell.
    path: 'site/:siteId',
    canActivate: [authGuard, siteAccessGuard],
    loadComponent: () =>
      import('./layout/shell/shell.component').then((m) => m.ShellComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
      },

      {
        path: 'batches',
        loadComponent: () =>
          import('./features/batches/batch-list.component').then(m => m.BatchListComponent),
      },

      {
        path: 'activity-log',
        loadComponent: () =>
          import('./features/activity-log/activity-log.component').then((m) => m.ActivityLogComponent),
      },

      {
        path: 'batches/:batchId/files/:fileId',
        loadComponent: () =>
          import('./features/files/file-details.component').then((m) => m.FileDetailsComponent),
      },

      // 👇 Future rounds add their routes here (keep BOTH entries on merge):
      //   batches            (Round 3 — you)
      //   batches/:batchId   (Round 3 — Shubh)
      //   batches/:batchId/files/:fileId (Round 4 — you)
      //   errors             (Round 4 — Shubh)
      //   activity-log       (Round 5 — you)
      {
        path: 'batches/:batchId',
        loadComponent: () =>
          import('./features/batches/batch-detail/batch-detail.component').then(
            (m) => m.BatchDetailComponent,
          ),
      },

      {
        path: 'errors',
        loadComponent: () =>
          import('./features/errors/errors.component').then((m) => m.ErrorsComponent),
      },

      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    ],
  },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' },
];
````

## File: docanalytics-web/src/app/features/dashboard/dashboard.component.ts
````typescript
import { DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject,
} from '@angular/core';
import { DashboardService } from './dashboard.service';
import { RefreshTimerService } from '../../core/services/refresh-timer.service';
import { SiteContextService } from '../../core/services/site-context.service';
import { RefreshTimerComponent } from '../../shared/components/refresh-timer/refresh-timer.component';
import { StatCardComponent } from '../../shared/components/stat-card/stat-card.component';
import {
  ColumnDef, DataTableComponent, DtCellDirective, SortState,
} from '../../shared/components/data-table/data-table.component';
import { ThroughputChartComponent } from './throughput-chart/throughput-chart.component';
import { StatusDistributionChartComponent } from './status-distribution-chart/status-distribution-chart.component';
import { FailuresSortBy, RecentFailure } from './dashboard.models';

const DASHBOARD_REFRESH_MS = 30_000;

@Component({
  selector: 'app-dashboard',
  imports: [
    StatCardComponent, DataTableComponent, DtCellDirective, DatePipe,
    ThroughputChartComponent, StatusDistributionChartComponent, RefreshTimerComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent {
  protected readonly dash = inject(DashboardService);
  private readonly poll = inject(RefreshTimerService);
  private readonly site = inject(SiteContextService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly refreshMs = DASHBOARD_REFRESH_MS;

  protected readonly busy = computed(() =>
    this.dash.summaryLoading() || this.dash.failuresLoading() ||
    this.dash.throughputLoading() || this.dash.distributionLoading());

  protected readonly columns: ColumnDef<RecentFailure>[] = [
    { key: 'file_name', header: 'File Name', sortable: true },
    { key: 'failed_step', header: 'Failed Step', sortable: true },
    { key: 'error', header: 'Error', sortable: false },
    { key: 'failed_at', header: 'Failed At', sortable: true, align: 'right', width: '160px' },
  ];

  constructor() {
    // initial load + reload on site switch (guarded so we never fire site-less)
    effect(() => {
      const siteId = this.site.selectedSiteId();
      if (siteId) this.dash.refreshAll();
    });
    // recurring 30s + pause-on-hidden + refresh-on-return; tick is guarded too
    this.poll.start(DASHBOARD_REFRESH_MS, () => {
      if (this.site.selectedSiteId()) this.dash.refreshAll();
    }, this.destroyRef);
  }

  protected onSort(s: SortState): void {
    this.dash.setFailuresSort(s.sortBy as FailuresSortBy, s.sortDir);
  }
}
````
