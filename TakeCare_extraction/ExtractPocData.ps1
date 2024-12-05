param
(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$patientlist,
    [Parameter(Mandatory = $true, Position = 1)]
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

foreach ($patientid in Get-Content -Path $patientlist) {
    if ($patientid -match '^\d{12}$') {
        # CareDocumentation
        Write-Host "Fetching CareDocumentation for Patient $patientid"
        $filecounter = 0
        $X2Message = XchangeCareDocumentationGet ($patientid) ($careunit)
        $X2Message | Out-File ${outputdirectory}\CareDocumentationGet_${patientid}_${filecounter}.xml
        $xmlresponse = [xml]$X2Message
        $nrdocsremaining = $xmlresponse.X2Message.caredocumentation.remainingdocuments.NrDocs
        [string]$docsremaining = -split $xmlresponse.X2Message.caredocumentation.remainingdocuments.docIds

        # Fetch remaining documents, if any
        while ( $nrdocsremaining ) {
            Write-Host "Docs remaining: $nrdocsremaining"
            $filecounter++
            $X2Message = XchangeCareDocumentationGet ($patientid) ($careunit) ($docsremaining)
            $X2Message | Out-File ${outputdirectory}\CareDocumentationGet_${patientid}_${filecounter}.xml
            $xmlresponse = [xml]$X2Message
            $nrdocsremaining = $xmlresponse.X2Message.caredocumentation.remainingdocuments.NrDocs
            [string]$docsremaining = -split $xmlresponse.X2Message.caredocumentation.remainingdocuments.docIds
        }

        # ChemistryReplies
        Write-Host "Fetching Chemistry Replies for Patient $patientid"
        $labindexresponse = JunoGetChemLabRepliesIndex ($patientid) ($careunit) ($token)
        $labrepliesindex = $labindexresponse | ConvertFrom-Json
        if ($labrepliesindex.id) { 
            $labindexresponse | Out-File ${outputdirectory}\ChemistryRepliesIndex_${patientid}.json
            foreach ($replyid in $labrepliesindex.id) {
                JunoGetChemLabReplies ($patientid) ($careunit) ($replyid) ($token) | Out-File ${outputdirectory}\ChemistryReply_${patientid}_${replyid}.json
                Write-Host "Got reply id: $replyid"
            }
        }
        else {
            Write-Host "No Chemistry replies found"
        }

        # MedicationHistory
        Write-Host "Fetching Medication History for Patient $patientid"
        $filecounter = 0
        $X2Message = XchangeMedicationHistoryGet ($patientid) ($careunit)
        $X2Message | Out-File ${outputdirectory}\MedicationHistoryGet_${patientid}_${filecounter}.xml
        $xmlresponse = [xml]$X2Message
        $nrdocsremaining = $xmlresponse.X2Message.caredocumentation.remainingdocuments.NrDocs
        [string]$docsremaining = -split $xmlresponse.X2Message.caredocumentation.remainingdocuments.docIds

        # Fetch remaining documents, if any
        while ( $nrdocsremaining ) {
            Write-Host "Docs remaining: $nrdocsremaining"
            $filecounter++
            $X2Message = XchangeMedicationHistoryGet ($patientid) ($careunit) ($docsremaining)
            $X2Message | Out-File ${outputdirectory}\MedicationHistoryGet_${patientid}_${filecounter}.xml
            $xmlresponse = [xml]$X2Message
            $nrdocsremaining = $xmlresponse.X2Message.medications.remainingdocuments.NrDocs
            [string]$docsremaining = -split $xmlresponse.X2Message.medications.remainingdocuments.docIds
        }

        # Measurements
        Write-Host "Fetching Measurements for Patient $patientid"
        $measurementsresponse = JunoGetMeasurementsIndex ($patientid) ($careunit) ($token)
        $measurementsindex = $measurementsresponse | ConvertFrom-Json
        if ($measurementsindex.id) { 
            $measurementsresponse | Out-File ${outputdirectory}\MeasurementsIndex_${patientid}.json
            foreach ($measurementsid in $measurementsindex.id) {
                JunoGetMeasurements ($patientid) ($careunit) ($measurementsid) ($token) | Out-File ${outputdirectory}\Measurements_${patientid}_${measurementsid}.json
                Write-Host "Got measurement id: $measurementsid"
            }
        }
        else {
            Write-Host "No Measurements found"
        }

        # Activities
        Write-Host "Fetching Activities for Patient $patientid"
        $activities = JunoGetActivities $patientid $careunit $token
        if ( ($activities | ConvertFrom-Json).id ) {
            $activities | Out-File ${outputdirectory}\Activities_${patientid}.json
        }
        else {
            Write-Host "No Activities found"
        }

        # Bookings
        Write-Host "Fetching Bookings for Patient $patientid"
        $bookings = JunoGetBookings $patientid $careunit $token
        if ( ($bookings | ConvertFrom-Json).id ) {
            $bookings | Out-File ${outputdirectory}\Bookings_${patientid}.json
        }
        else {
            Write-Host "No Bookings found"
        }

    }
    else {
        Write-Host "$patientid is in the wrong format"
    }
}