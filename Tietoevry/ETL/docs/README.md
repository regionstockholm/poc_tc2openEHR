## ETL Introduction 
ETL stands for Extract, Transform, and Load. Main goal in this project is to migrate and convert Takecare data to openEHR.  

#### Extraction
Extract data in ETL compatible format​ and transfer to Transformation layer​.

#### Transformation
Uses openEHR template, binds clinical data and context metadata with template structure. ​Creates openEHR compositon ​and transfers the same to Loader Layer​.

#### Loader
Resolve/Create Patient EhrId from (FHIR/EHR), then maps EhrId to Composition ​and saves data to opeEHR.

As a part of this application ETL tool was developed for following information types:
- Care Documentation
- Measurement
- Medication
- Chemistry
- Activity

#### Project struture for ETL
All ETL project shares the same structure (each one has different logic for extranction and transformation), each of ETL consists of 2 projects:
- Extraction  
- Transformer as shown in below screenshot. 

![alt text](image/README/image-1.png)

- And there respective EtlHandler in TakeCare.Migration.OpenEhr.Etl project

![alt text](image/README/image-2.png)


## Services
List of common services used throughout the application based on their need.

#### Unit Provider

The Unit Provider provides the standard OpenEHR units of the TakeCare units. Units in this context means for example 'mmHg' and units used for measurements data mainly.

#### Context Provider

Provides User context data

#### Terminology Provider

Maps the TermID from the Casenote document to the openEHR standard TermID and gives the details.

#### Patient Service

Provides the Patient Id based on the SSN (Social security number) ID provided in the Care Documentation.

#### EHR ID resolver

Provides the EHR ID based of the Patient Id fetched earlier.

#### Terminology Lookup Provider

Reads the TermCatalog and provide information about the TermID, its names, datatype and unit

#### Role Provider

Provides lists of the role Id (profession ID) and the respctive roleName (profession Name).

#### Form Provider

Provides form API details like Name and latest version.

#### Medication Service

Calls substituable service which used to get code and values based on the value of Isreplaceble.

#### Notes :

To add new iCKM archetype, add the Terminology details to the Terminolgy.json, then add the composition creation logic to iCKMArchetypes, and finally add to the switch logic in Composition to call the apt composition logic.



## Care Documentation
In Care documentation, data is in xml format in the form of Casenotes. Each xml file consists of list of casenotes and each case note consists of list of nested keywords.

For each case not a composition is created.

Challenges - the care documentation xml structure was complicated and dynamic due to TakeCare structur containing nesting of keywords and their values.

## Measurements
In Measurement ETL patient measurements data is in Json file. These files are processing json files through ETL.

File name conventions:
- Measurements_195207291591_20241023084031-197803070189.json
- 195207291591 - Stands for Patient Id
- 20241023084031 - Stands for Created datetime
- 197803070189 - Stands for Createdby user Id

In each json file there are template field which contains list of terms. These templates are saved by creating separate json file with name - "template-{templateId}.json. For the POC these are stored in local file system.

![alt text](image/README/image-3.png)


## Medication
In Medication ETL, the test data was provided in xml file and had collection of medication, prescription drug dosages and days details along with administration and infustion lists. 

#### Challenges- 
- The xml input file has complex structure and have list of elements (Medication, Prescription, Drugs, Dosages, Days, Administration, Infusion). In the POC simple template and mapping were used to save Medication, Prescription, Drugs etc. The more complex templates are not used in POC in the visualization work. They will be needed if all medication data needs to be visaluized in the future.
- **Simple mapping is considering Single drug, if IsMixture value is 0 and drug count is 1 then Medication is saved, the rest will be skipped for POC. Multi-drug case will be covered in complex mapping (2nd phase).**
- In 2nd phase complex compostion will be saved - contains inter-relation between drug, dosages and days.
- Medication is saved in a composition and Administration and infusion are saved in separate compositions where they are linked them to their respective medication.

## SUMMARY

| Module    | Input file format | Template Name  |  View Used    | Widget Used |        UI    |
| --------- | -------------- |----------------- |---------------- |------------ |------------- | 
| Care Doc  |   .xml    | RSK - Journal Encounter   |    -     |     -    |     ![alt text](image/README/image-6.png)     | 
| Measurement |   .json   | RSK - Journal Encounter |   -      |    -     |     ![alt text](image/README/image-7.png)    |
| Medication  |   .xml    | RSK - Medication order  |   -      |    -     |     -       |
| Chemistry   |   .json   |  -                      |   -      |    -     |    ![alt text](image/README/image-8.png)      | 
| Activities  |   .xml    | RS - Activity           |   -      |    -     |     -       |
---------------------------------------------------------------------------------------------------------------

## Solution Structure

![alt text](image/README/image-5.png)

- CareDocumentation >
    - **TakeCare.Migration.OpenEhr.CareDocumentation.Extraction**- Care documentation extraction
    - **TakeCare.Migration.OpenEhr.CareDocumentation.Transformer** - Care documentation transformation

- Chemistry >
    - **TakeCare.Migration.OpenEhr.Chemistry.Extraction** - Chemistry extraction
    - **TakeCare.Migration.OpenEhr.Chemistry.Transformation** - chemistry transformation

- Measurement >
    - **TakeCare.Migration.OpenEhr.Measurement.Extraction** - measurement extraction
    - **TakeCare.Migration.OpenEhr.Measurement.Transformer** - measurement transformation

- Medication >
    - **TakeCare.Migration.OpenEhr.Medication.Extraction** - medication extraction
    - **TakeCare.Migration.OpenEhr.Medication.Transformer** - medication trasformation

- Shared >
    - **TakeCare.Foundation.OpenEhr.Application** - has common functionalities like services and models
    - **TakeCare.Foundation.OpenEhr.Archetype** - has common ickm archetype models 
    - **TakeCare.Foundation.OpenEhr.Models** - has common models other than ickm archetypes

- **TakeCare.Migration.OpenEhr.Etl** >
    - Main project where we add etl handler for each modules and set this as a startup project. All etls can be run in one go or one at a time.


## How to Setup and Run

Startup project

TakeCare.Migration.OpenEhr.Etl
Program.cs

Set **TakeCare.Migration.OpenEhr.Etl** project as startup project

![alt text](image/README/image-4.png)
