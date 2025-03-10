param
(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$outputdirectory
)
Import-Module .\TakeCareFunctions.psm1

$careunit = <careunitid>

# Create output dir if needed
if (!(Test-Path -PathType Container $outputdirectory)) {
    Write-Host "Creating folder $outputdirectory"
    New-Item -ItemType Directory -Path $outputdirectory
}
else {
    Write-Host "Output directory already exists"
}

# Fetch CareProviders
Write-Host "Fetching CareProviders"
$X2Message = XchangeCareProvidersGet ($careunit)
$X2Message | Out-File ${outputdirectory}\CareProvidersGet.xml

# Fetch LockGroups
Write-Host "Fetching LockGroups"
$X2Message = XchangeLockGroupsGet ($careunit)
$X2Message | Out-File ${outputdirectory}\LockGroupsGet.xml

# Fetch CareUnits
Write-Host "Fetching CareUnits"
$X2Message = XchangeCareUnitsGet ($careunit)
$X2Message | Out-File ${outputdirectory}\CareUnitsGet.xml
 