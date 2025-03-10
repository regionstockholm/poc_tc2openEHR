$config = Get-Content -Path '.\TakecareFunctions.json' | ConvertFrom-Json
$certificate = Get-Item $config.CertLocation
$JunoURL = $config.JunoURL
$XchangeURL = $config.XchangeURL
$XchangeUser = $config.XchangeUser
$XchangePwd = $config.XchangePwd

Function JunoLogin {
   # Make a login call to get a Juno access token
   param(
      [Parameter(Mandatory = $true, Position = 0)]
      [int]$careunit   
   )

   # Body for login call
   $URI = "$JunoURL/login?loginAt=$careunit"
   $JSON = @'
 {"user":"",
  "password":"",
  "authenticationMethod":"clientCertificate"
    }
'@

   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Post -Certificate $certificate -Body $JSON -ContentType "application/json" -DisableKeepAlive
   }
   Catch {
      throw $_
   }
   if ($response.StatusCode -eq '200') {
      $json = ConvertFrom-Json $([string]::new($response.Content))
      Write-Verbose "Juno access token retrieved"
      return $json.sessiontoken
   }
   else {
      Write-Error "$Timestamp Failed to get session ticket. Return Code: $response.StatusCode"
   }
}

Function JunoGetBookings {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $true, Position = 2)]
      [string]$token
   )
   
   $URI = "$JunoURL/patients/$patientid/bookingsFull?fromCareUnit=$careunit"
   $headers = @{Authorization = "Bearer $token" }
   
   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got Bookings"
      $json = [Text.Encoding]::Utf8.GetString(
         ($response).RawContentStream.ToArray()
      )
   }
   else {
      Write-Error "Failed to get bookings. Return Code: $response.StatusCode"
   }
   return $json
}

Function JunoGetActivities {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $true, Position = 2)]
      [string]$token
   )

   $URI = "$JunoURL/patients/$patientid/activities?fromCareUnit=$careunit&status=all"
   $headers = @{Authorization = "Bearer $token" }

   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got Activities"
      $json = [Text.Encoding]::Utf8.GetString(
         ($response).RawContentStream.ToArray()
      )
   }
   else {
      Write-Error "Failed to get activities. Return Code: $response.StatusCode"
   }
   return $json
}

Function JunoGetMeasurementsIndex {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $true, Position = 2)]
      [string]$token
   )
      
   $URI = "$JunoURL/patients/$patientid/measurements?fromCareUnit=$careunit"
   $headers = @{Authorization = "Bearer $token" }
      
   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got Measurements Index"
      $json = [Text.Encoding]::Utf8.GetString(
         ($response).RawContentStream.ToArray()
      )
   }
   else {
      Write-Error "Failed to get Measurements Index. Return Code: $response.StatusCode"
   }
   return $json
}

Function JunoGetMeasurements {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $true, Position = 2)]
      [string]$docid,
      [Parameter(Mandatory = $true, Position = 3)]
      [string]$token
   )
      
   $URI = "$JunoURL/patients/$patientid/measurements/${docid}?fromCareUnit=$careunit"
   $headers = @{Authorization = "Bearer $token" }
      
   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
      
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got Measurements document $docid"
      $json = [Text.Encoding]::Utf8.GetString(
               ($response).RawContentStream.ToArray()
      )   
   }
   else {
      Write-Error "Failed to get measurement doc $docid from Patient $patientid"
   }
   return $json
}

Function JunoGetMeasurementTemplatesIndex {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$token
   )
   $URI = "$JunoURL/measurementTemplates"
   $headers = @{Authorization = "Bearer $token" }
      
   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got Measurement Templates Index"
      $json = [Text.Encoding]::Utf8.GetString(
         ($response).RawContentStream.ToArray()
      )
   }
   else {
      Write-Error "Failed to get Measurement Templates Index. Return Code: $response.StatusCode"
   }
   return $json
}

Function JunoGetMeasurementTemplates {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$templateid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$token
   )
      
   $URI = "$JunoURL/measurementTemplates/${templateid}"
   $headers = @{Authorization = "Bearer $token" }
      
   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
      
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got Measurements template $templateid"
      $json = [Text.Encoding]::Utf8.GetString(
               ($response).RawContentStream.ToArray()
      )   
   }
   else {
      Write-Error "Failed to get Measurements Template $templateid"
   }
   return $json
}

Function JunoGetChemLabRepliesIndex {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $true, Position = 2)]
      [string]$token
   )
   $URI = "$JunoURL/patients/$patientid/lab/replies/chemistry?fromCareUnit=$careunit"
   $headers = @{Authorization = "Bearer $token" }
      
   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got ChemLab Replies Index"
      $json = [Text.Encoding]::Utf8.GetString(
         ($response).RawContentStream.ToArray()
      )
   }
   else {
      Write-Error "Failed to get ChemLab Replies Index. Return Code: $response.StatusCode"
   }
   return $json
}

Function JunoGetChemLabReplies {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $true, Position = 2)]
      [string]$docid,
      [Parameter(Mandatory = $true, Position = 3)]
      [string]$token
   )
      
   $URI = "$JunoURL/patients/$patientid/lab/replies/chemistry/${docid}?fromCareUnit=$careunit"
   $headers = @{Authorization = "Bearer $token" }
      
   #Perform call
   Try {
      $response = Invoke-WebRequest -Uri $URI -Method Get -Headers $headers
      
   }
   catch {
      throw $_
   }
   if ($response.StatusCode -eq "200") {
      Write-Verbose "Got Measurements document $docid"
      $json = [Text.Encoding]::Utf8.GetString(
               ($response).RawContentStream.ToArray()
      )   
   }
   else {
      Write-Error "Failed to get ChemLab reply $docid from Patient $patientid"
   }
   return $json
}

Function XchangeCareDocumentationGet {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $false, Position = 2)]
      [string]$docremains
   )

   #Build generic call arguments
   $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
   $headers.Add('Content-type', 'text/xml;charset=UTF-8')
   $URI = "$XchangeURL/dal/dalkernel.asmx?op=WebCall"

   $body = '<?xml version="1.0" encoding="utf-8"?>
   <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
   <soap:Body>
	<WebCall xmlns="http://tempuri.org/">
   <header>
      <string>CaseNote.CareDocumentationGet.1</string>
      <string>PoC</string>
   </header>
   <auth>
      <string>USER</string>
      <string>' + $XchangeUser + '</string>
      <string>' + $XchangePwd + '</string>
      <string>Internal</string>
      <string>' + $careunit + '</string>
   </auth>
   <params>
      <string>'+ $patientid + '</string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
      <string>'+ $docremains + '</string>
   </params>
   </WebCall>
   </soap:Body>
   </soap:Envelope>'

   Try {
   $response = Invoke-RestMethod -Uri $URI -Method Post -Certificate $certificate -Headers $headers -Body $body
   $X2Message = $response.envelope.body.webcallresponse.webcallresult
   }
   catch {
   throw $_
   }
   Write-Verbose "Got CareDocumentation"
   return $X2Message
}

Function XchangeMedicationHistoryGet {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$patientid,
      [Parameter(Mandatory = $true, Position = 1)]
      [string]$careunit,
      [Parameter(Mandatory = $false, Position = 2)]
      [string]$docremains
   )

   #Build generic call arguments
   $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
   $headers.Add('Content-type', 'text/xml;charset=UTF-8')
   $URI = "$XchangeURL/dal/dalkernel.asmx?op=WebCall"

   $body = '<?xml version="1.0" encoding="utf-8"?>
   <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
   <soap:Body>
	<WebCall xmlns="http://tempuri.org/">
   <header>
      <string>Medications.MedicationHistoryGet.1</string>
      <string>PoC</string>
   </header>
   <auth>
      <string>USER</string>
      <string>' + $XchangeUser + '</string>
      <string>' + $XchangePwd + '</string>
      <string>Internal</string>
      <string>' + $careunit + '</string>
   </auth>
   <params>
      <string>'+ $patientid + '</string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
      <string>'+ $docremains + '</string>
   </params>
   </WebCall>
   </soap:Body>
   </soap:Envelope>'

   Try {
   $response = Invoke-RestMethod -Uri $URI -Method Post -Certificate $certificate -Headers $headers -Body $body
   $X2Message = $response.envelope.body.webcallresponse.webcallresult
   }
   catch {
   throw $_
   }
   Write-Verbose "Got MedicationHistory"
   return $X2Message
}

Function XchangeCareProvidersGet {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$careunit
   )

   #Build generic call arguments
   $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
   $headers.Add('Content-type', 'text/xml;charset=UTF-8')
   $URI = "$XchangeURL/dal/dalkernel.asmx?op=WebCall"

   $body = '<?xml version="1.0" encoding="utf-8"?>
   <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
   <soap:Body>
	<WebCall xmlns="http://tempuri.org/">
   <header>
      <string>System.CareProvidersGet.1</string>
      <string>PoC</string>
   </header>
   <auth>
      <string>USER</string>
      <string>' + $XchangeUser + '</string>
      <string>' + $XchangePwd + '</string>
      <string>Internal</string>
      <string>' + $careunit + '</string>
   </auth>
   <params>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
   </params>
   </WebCall>
   </soap:Body>
   </soap:Envelope>'

   Try {
   $response = Invoke-RestMethod -Uri $URI -Method Post -Certificate $certificate -Headers $headers -Body $body
   $X2Message = $response.envelope.body.webcallresponse.webcallresult
   }
   catch {
   throw $_
   }
   Write-Verbose "Got CareProviders"
   return $X2Message
}
Function XchangeLockGroupsGet {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$careunit
   )

   #Build generic call arguments
   $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
   $headers.Add('Content-type', 'text/xml;charset=UTF-8')
   $URI = "$XchangeURL/dal/dalkernel.asmx?op=WebCall"

   $body = '<?xml version="1.0" encoding="utf-8"?>
   <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
   <soap:Body>
	<WebCall xmlns="http://tempuri.org/">
   <header>
      <string>System.LockGroupsGet.1</string>
      <string>PoC</string>
   </header>
   <auth>
      <string>USER</string>
      <string>' + $XchangeUser + '</string>
      <string>' + $XchangePwd + '</string>
      <string>Internal</string>
      <string>' + $careunit + '</string>
   </auth>
   <params>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
   </params>
   </WebCall>
   </soap:Body>
   </soap:Envelope>'

   Try {
   $response = Invoke-RestMethod -Uri $URI -Method Post -Certificate $certificate -Headers $headers -Body $body
   $X2Message = $response.envelope.body.webcallresponse.webcallresult
   }
   catch {
   throw $_
   }
   Write-Verbose "Got LockGroups"
   return $X2Message
}

Function XchangeCareUnitsGet {
   Param (
      [Parameter(Mandatory = $true, Position = 0)]
      [string]$careunit
   )

   #Build generic call arguments
   $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
   $headers.Add('Content-type', 'text/xml;charset=UTF-8')
   $URI = "$XchangeURL/dal/dalkernel.asmx?op=WebCall"

   $body = '<?xml version="1.0" encoding="utf-8"?>
   <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
   <soap:Body>
	<WebCall xmlns="http://tempuri.org/">
   <header>
      <string>System.CareUnitsGet.1</string>
      <string>PoC</string>
   </header>
   <auth>
      <string>USER</string>
      <string>' + $XchangeUser + '</string>
      <string>' + $XchangePwd + '</string>
      <string>Internal</string>
      <string>' + $careunit + '</string>
   </auth>
   <params>
      <string>ADDRESS COSTCTR</string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
      <string></string>
   </params>
   </WebCall>
   </soap:Body>
   </soap:Envelope>'

   Try {
   $response = Invoke-RestMethod -Uri $URI -Method Post -Certificate $certificate -Headers $headers -Body $body
   $X2Message = $response.envelope.body.webcallresponse.webcallresult
   }
   catch {
   throw $_
   }
   Write-Verbose "Got CareUnits"
   return $X2Message
}