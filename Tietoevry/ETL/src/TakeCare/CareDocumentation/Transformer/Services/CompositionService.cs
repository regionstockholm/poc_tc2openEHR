using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class CompositionService : ICompositionService
    {
        private readonly ITerminolgyLookup _terminologyservice;
        private readonly ITemplateServices _templateService;

        Dictionary<string, string> lookuptable = new Dictionary<string, string>();
        Dictionary<string, Object> template = new Dictionary<string, Object>();
        Dictionary<string, int> counterMap = new Dictionary<string, int>();
        JObject newComposition;
        JObject composedObject;

        // Create a composition object
        List<Dictionary<string, string>> responseList = new List<Dictionary<string, string>>();
        public CompositionService(ITemplateServices templateService,
                                  ITerminolgyLookup terminologyservice)
        {
            _terminologyservice = terminologyservice;
            _templateService = templateService;
        }

        public CareDocumentOpenEhrData Compose(CareDocumentationDto inputData)
        {
            template = _templateService.GetTemplate();
            lookuptable = _terminologyservice.GetTerminology();

            newComposition = _templateService.GetJsonData();
            template.Remove("ctx");
            CareDocumentOpenEhrData careDocumentOpenEhrData = new CareDocumentOpenEhrData()
            {
                PatientID = GetPatientID(inputData.PatientId)
            };

            string commonPrefix = _templateService.GetCommonPrefix(template);

            if (inputData != null && inputData.CaseNotes != null && inputData.CaseNotes.Count > 0)
            {
                foreach (var caseNote in inputData.CaseNotes)
                {
                    StringBuilder commonKeyPrefix = new StringBuilder();
                    counterMap = new Dictionary<string, int>();
                    composedObject = new JObject();
                    counterMap.Add("generic", 0);

                    
                    // Add the ctx static data
                    foreach (var prop in newComposition.Properties())
                    {
                        if (prop.Name.StartsWith("ctx"))
                        {
                            composedObject[prop.Name] = prop.Value; // verify
                        }
                    }
                    

                    AddCtxData(commonPrefix, caseNote);

                    //Add context metadata
                    AddContextMetadata(commonPrefix, caseNote);

                    // Add context care unit data
                    AddContextCareUnitdata(commonPrefix, caseNote);

                    //Add the Generic entry
                    if (caseNote != null && caseNote.Keywords != null && caseNote.Keywords.Count > 0)
                    {

                        foreach (var keyword in caseNote.Keywords)
                        {

                            if (lookuptable.ContainsKey(keyword.TermId))
                            {
                                if (!counterMap.ContainsKey(keyword.TermId))
                                    counterMap.Add(keyword.TermId, 0);
                                AddCKMEntry(keyword, lookuptable[keyword.TermId], commonPrefix);
                            }
                            else
                            {
                                AddGenericEntry(keyword, commonPrefix);
                            }

                        }
                    }
                    careDocumentOpenEhrData.Compositions.Add(composedObject);
                }
            }
            return careDocumentOpenEhrData;
        }

        private void AddCtxData(string commonPrefix, CaseNoteDto caseNote)
        {
            composedObject[$"{commonPrefix}{"_name"}"] = caseNote.HeaderTerm;
            composedObject[$"{commonPrefix}{"language|code"}"] = "sv";
            composedObject[$"{commonPrefix}{"language|terminology"}"] = "ISO_639-1";
            composedObject[$"{commonPrefix}{"territory|code"}"] = "SV";
            composedObject[$"{commonPrefix}{"territory|terminology"}"] = "ISO_3166-1";
            composedObject[$"{commonPrefix}{"category|code"}"] = "433";
            composedObject[$"{commonPrefix}{"category|value"}"] = "event";
            composedObject[$"{commonPrefix}{"category|terminology"}"] = "openehr";

            
            StringBuilder contextBuilder = new StringBuilder(commonPrefix);

            string contextCareUnit = contextBuilder.ToString();
            /*
            contextBuilder.Append("composer/identifier:0");
            string contextCareUnit = contextBuilder.ToString();

            composedObject[$"{contextCareUnit}{"|name"}"] = caseNote.DocCreatedByUserId; //verify
            composedObject[$"{contextCareUnit}{"|id"}"] = caseNote.DocCreatedByUserId;
            composedObject[$"{contextCareUnit}{"|type"}"] = "UserId";
            composedObject[$"{contextCareUnit}{"|issuer"}"] = "RSK";
            */
            contextBuilder = new StringBuilder(commonPrefix);
            contextBuilder.Append("context/");
            contextCareUnit = contextBuilder.ToString();

            composedObject[$"{contextCareUnit}{"start_time"}"] = caseNote.EventDateTime;
            composedObject[$"{contextCareUnit}{"setting|code"}"] = "238";
            composedObject[$"{contextCareUnit}{"setting|value"}"] = "other care";
            composedObject[$"{contextCareUnit}{"setting|terminology"}"] = "openehr";

            contextBuilder.Append("_health_care_facility/_identifier:0");
            contextCareUnit = contextBuilder.ToString();

            composedObject[$"{contextCareUnit}{"|name"}"] = caseNote.DocCreatedAtCareUnitId;
            composedObject[$"{contextCareUnit}{"|id"}"] = caseNote.DocCreatedAtCareUnitId;
            composedObject[$"{contextCareUnit}{"|type"}"] = "CareUnitId";
            composedObject[$"{contextCareUnit}{"|issuer"}"] = "RSK";


        }

        private void AddContextCareUnitdata(string commonPrefix, CaseNoteDto casenote)
        {
            // verify
            StringBuilder contextCareUnitBuilder = new StringBuilder(commonPrefix);
            contextCareUnitBuilder.Append("context/vårdenhet/");
            string contextCareUnit = contextCareUnitBuilder.ToString();
            composedObject[$"{contextCareUnit}{"namn"}"] = casenote.DocCreatedAtCareUnitId;
            composedObject[$"{contextCareUnit}{"roll:0|code"}"] = "43741000"; 
            composedObject[$"{contextCareUnit}{"roll:0|value"}"] = "vårdenhet"; //verify
            composedObject[$"{contextCareUnit}{"roll:0|terminology"}"] = "http://snomed.info/sct/900000000000207008"; //verify wrong
            contextCareUnitBuilder.Append("identifierare:0");
            contextCareUnit = contextCareUnitBuilder.ToString();
            composedObject[$"{contextCareUnit}"] = casenote.DocCreatedAtCareUnitId;
            composedObject[$"{contextCareUnit}{"|issuer"}"] = "RSK";
            composedObject[$"{contextCareUnit}{"|assigner"}"] = ""; //verify
            composedObject[$"{contextCareUnit}{"|type"}"] = "CareUnitId";

            contextCareUnitBuilder = new StringBuilder(commonPrefix);
            contextCareUnitBuilder.Append("context/vårdenhet/vårdgivare/");
            contextCareUnit = contextCareUnitBuilder.ToString();
            composedObject[$"{contextCareUnit}{"namn"}"] = ""; //verify

            composedObject[$"{contextCareUnit}{"roll:0|code"}"] = "143591000052106";
            composedObject[$"{contextCareUnit}{"roll:0|value"}"] = "vårdgivare";
            composedObject[$"{contextCareUnit}{"roll:0|terminology"}"] = "http://snomed.info/sct/45991000052106"; //verify wrong 


            contextCareUnitBuilder.Append("identifierare:0");
            composedObject[$"{contextCareUnitBuilder}"] = Guid.NewGuid(); //verify
            composedObject[$"{contextCareUnitBuilder}{"|issuer"}"] = "RSK";
            composedObject[$"{contextCareUnitBuilder}{"|assigner"}"] = ""; //verify
            composedObject[$"{contextCareUnitBuilder}{"|type"}"] = "CareUnitId";
        }

        private void AddContextMetadata(string commonPrefix, CaseNoteDto caseNote)
        {
            // verify
            StringBuilder contextMetaData = new StringBuilder(commonPrefix);
            contextMetaData.Append("context/metadata/");
            string contextData = contextMetaData.ToString();
            composedObject[$"{contextData}{"dokument_id"}"] = caseNote.DocumentId;
            composedObject[$"{contextData}{"godkänd_för_patient"}"] = caseNote.ApprovedForPatient;
            composedObject[$"{contextData}{"dokumentationskod"}"] = caseNote.DocumentCode;
            composedObject[$"{contextData}{"dokumentationstidpunkt"}"] = caseNote.DocCreatedTimestamp;
            composedObject[$"{contextData}{"dokumentnamn"}"] = caseNote.DocumentTitle;
            composedObject[$"{contextData}{"signeringstidpunkt"}"] = caseNote.SignedTimestamp;
            composedObject[$"{contextData}{"signerat_av_id"}"] = caseNote.SignedById;
            composedObject[$"{contextData}{"signerat_av_id|issuer"}"] = "RSK";
            composedObject[$"{contextData}{"signerat_av_id|assigner"}"] = "RSK";
            composedObject[$"{contextData}{"signerat_av_id|type"}"] = "UserId";
            composedObject[$"{contextData}{"signerare_id"}"] = caseNote.SignerId;
            composedObject[$"{contextData}{"signerare_id|issuer"}"] = "RSK";
            composedObject[$"{contextData}{"signerare_id|assigner"}"] = "RSK"; 
            composedObject[$"{contextData}{"signerare_id|type"}"] = "UserId";

            contextMetaData.Append("dokumentationsmall");
            contextData = contextMetaData.ToString();
            composedObject[$"{contextData}{"|code"}"] = caseNote.TemplateId;
            composedObject[$"{contextData}{"|value"}"] = caseNote.TemplateName;
            composedObject[$"{contextData}{"|terminology"}"] = "TC-Template-codes";

            contextMetaData = new StringBuilder(commonPrefix);
            contextMetaData.Append("context/metadata/");
            contextMetaData.Append("dokument_skapad_av_yrkestitel_id");
            contextData = contextMetaData.ToString();
            composedObject[$"{contextData}"] = caseNote.DocCreatedByProfessionId;
            composedObject[$"{contextData}{"|issuer"}"] = "RSK";
            composedObject[$"{contextData}{"|assigner"}"] = "RSK";

        }

        private void AddGenericEntry(KeywordDto keyword, string commonPrefix)
        {
            int v = counterMap["generic"];
            StringBuilder prefixBuilder = new StringBuilder(commonPrefix);
            prefixBuilder.Append("genrisk_händelse:");
            string prefix = prefixBuilder.ToString();
            composedObject[$"{prefix}{v}{"/_uid"}"] = keyword.Guid;
            string suffix = "/sökord/";
            composedObject[$"{prefix}{v}{suffix}{"namn|code"}"] = keyword.TermId;
            composedObject[$"{prefix}{v}{suffix}{"namn|value"}"] = keyword.Name;
            composedObject[$"{prefix}{v}{suffix}{"namn|terminology"}"] = "TC-Catalog";
            if (keyword.Value!=null)
            {
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|code"}"] = keyword.TermId; //verify
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|value"}"] = (keyword.Value != null) ? ((keyword.Value.NumVal != null) ? keyword.Value.NumVal.Val : keyword.Value.TextVal) : "";
                composedObject[$"{prefix}{v}{suffix}{"värde/coded_text_value|terminology"}"] = "external_terminology";

            }
            composedObject[$"{prefix}{v}{suffix}{"datatyp|code"}"] = "1";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|value"}"] = "Text";
            composedObject[$"{prefix}{v}{suffix}{"datatyp|terminology"}"] = "TC-Datatypes";
            composedObject[$"{prefix}{v}{suffix}{"egenskaper:0|code"}"] = "at0009";
            composedObject[$"{prefix}{v}{suffix}{"originalenhet"}"] = (keyword.Value != null) ? ((keyword.Value.NumVal != null) ? keyword.Value.NumVal.Unit : "") : ""; ;
            composedObject[$"{prefix}{v}{suffix}{"kommentar"}"] = keyword.Comment;
            composedObject[$"{prefix}{v}{suffix}{"nivå"}"] = keyword.ParentCount;
            if (keyword.Childs != null)
            {
                for (int i = 0; i < keyword.Childs.Count; i++)
                {
                    composedObject[$"{prefix}{v}{suffix}{"underordnat_sökord:"}{i}"] = "ehr://" + keyword.Childs[i];
                }
            }
            counterMap["generic"]++;
        }

        private void AddCKMEntry(KeywordDto keyword, string keywordname, string commonPrefix)
        {
            int v = counterMap[keyword.TermId];
            switch (keywordname)
            {
                //create a class with the keywordname and add a method to add the data to composedObject
                case "systoliskt":
                    SystolicCKMEntry.AddSystolicData(composedObject, keyword, v, keywordname, commonPrefix);
                    break;
                case "height":
                    HeightCKMEntry.AddHeightData(composedObject, keyword, v, keywordname, commonPrefix);
                    break;
                case "weight":
                    WeightCKMEntry.AddWeightData(composedObject, keyword, v, keywordname, commonPrefix);
                    break;
                case "diastoliskt":
                    DiastolicCKMEntry.AddDiastolicData(composedObject, keyword, v, keywordname, commonPrefix);
                    break;
                case "problem_diagnos":
                    ProblemDiagnosisCKMEntry.AddProblemDiagnosisData(composedObject, keyword, v, keywordname, commonPrefix);
                    break;
                default:
                    AddGenericEntry(keyword, commonPrefix);
                    break;
            }
            counterMap[keyword.TermId]++;
        }

        private string GetPatientID(string patientId)
        {
            // Todo: Implement a service to get the patient id from the patient id
            Dictionary<string, string> patientData = new()
            {
                {
                    "201402192387","691f4cc6-af39-400c-9373-a4ff9f828459"
                },
                {
                    "195906113187","6e2dcbcd-928c-4ecd-8d95-8a29ea98ffc6"
                },
                {
                    "195207291591","cd424b25-4b44-43f4-8b7f-d8a3b27554c6"
                },
                {
                    "195401171334","1f2ca173-839e-4ca0-b83c-c4ab1465e2ab"
                },
                {
                    "195705172590","5151df39-a57b-4991-8763-e53f4766ee39"
                }
            };
            
            return patientData.First(x=>x.Key == patientId).Value;
        }
    }
}
