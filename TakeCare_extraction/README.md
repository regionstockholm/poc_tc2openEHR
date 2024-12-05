# Simple scripts to extract data from TakeCare APIs

## Description

### TakeCareFunctions.psm1

Is used by the extraction scripts and contains functions used to extract data using Xchange and Juno APIs.
TakeCareFunctions.json contains the configuration information.

### ExtractPocTemplates.ps1

Extracts measurement templates from TakeCare.
Careunit used for access is configured within the source.

Usage: ExtractPocData.ps1 *output-directory*

## ExtractPocUnits.ps1

Extracts Careproviders, Lockgroups and Careunits info.
Careunit used for access is configured within the source.

Usage: ExtractPocData.ps1 *output-directory*

## ExtractPocData.ps1

Extracts selected patient information from TakeCare.
Expects an input file with a list of personnummer, one per line.
Careunit used for access is configured within the source.

Usage: ExtractPocData.ps1 *patientlista.txt* *output-directory*
