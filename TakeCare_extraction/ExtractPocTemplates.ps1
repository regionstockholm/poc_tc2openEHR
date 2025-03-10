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

try {
    $token = JunoLogin $careunit
}
catch {
    Write-Host "Error getting access token, exiting"
    exit 20
}

# Fetch Measurement templates
Write-Host "Fetching Measurement templates"
$measurementtemplatesindexresponse = JunoGetMeasurementTemplatesIndex ($token)
$measurementtemplatesindex = $measurementtemplatesindexresponse | ConvertFrom-Json
if ($measurementtemplatesindex.templates.id) { 
    $measurementtemplatesindexresponse | Out-File ${outputdirectory}\MeasurementsTemplatesIndex.json
    foreach ($templateid in $measurementtemplatesindex.templates.id) {
        JunoGetMeasurementTemplates ($templateid) ($token) | Out-File ${outputdirectory}\MeasurementsTemplate_${templateid}.json
        Write-Host "Got Measurements Template id: $templateid"
    }
}