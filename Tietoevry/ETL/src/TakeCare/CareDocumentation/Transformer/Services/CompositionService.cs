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
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Model.CareDoc;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Utils;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.iCkmArchetypes;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class CompositionService : ICompositionService
    {
        private readonly ITemplateServices _templateService;
        private readonly IPatientService _patientService;
        private readonly ITerminologyProvider _terminologyProvider;
        Dictionary<string, string> lookuptable = new Dictionary<string, string>();
        Dictionary<string, Object> template = new Dictionary<string, Object>();
        Dictionary<string, int> counterMap = new Dictionary<string, int>();
        JObject newComposition;
        JObject composedObject;

        // Create a composition object
        List<Dictionary<string, string>> responseList = new List<Dictionary<string, string>>();
        public CompositionService(ITemplateServices templateService,
                                  IPatientService patientService,
                                  ITerminologyProvider terminologyProvider)
        {
            _templateService = templateService;
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
        }

        public CareDocumentOpenEhrData Compose(CareDocumentationDto inputData)
        {
            template = _templateService.GetTemplate();
            //lookuptable = _terminologyservice.GetTerminology();

            newComposition = _templateService.GetJsonData();
            template.Remove("ctx");
            CareDocumentOpenEhrData careDocumentOpenEhrData = new CareDocumentOpenEhrData()
            {
                PatientID = _patientService.GetPatient(inputData.PatientId).PatientId
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
                    AddCtxData(caseNote);
                    
                    //Add ctx data
                    AddContextData(commonPrefix, caseNote);

                    //Add context metadata
                    AddContextMetadata(commonPrefix, caseNote);

                    // Add context care unit data
                    AddContextCareUnitdata(commonPrefix, caseNote);

                    //Add the Generic entry
                    if (caseNote != null && caseNote.Keywords != null && caseNote.Keywords.Count > 0)
                    {

                        foreach (var keyword in caseNote.Keywords)
                        {

                            TerminologyDetails termData = _terminologyProvider.GetTerminology(keyword.TermId);
                            if (termData!=null)
                            {
                                AddCKMEntry(keyword, termData, commonPrefix);
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

        private void AddCtxData(CaseNoteDto caseNote)
        {
            string commonPrefix = "ctx/";
            composedObject[$"{commonPrefix}{"language"}"] = "sv";
            composedObject[$"{commonPrefix}{"territory"}"] = "US";
            composedObject[$"{commonPrefix}{"composer_name"}"] = "UserName"; // verify
            composedObject[$"{commonPrefix}{"id_namespace"}"] = "HOSPITAL-NS"; //verify
            composedObject[$"{commonPrefix}{"id_scheme"}"] = "HOSPITAL-NS"; //verify
            composedObject[$"{commonPrefix}{"participation_name"}"] = "UserName"; //verify
            composedObject[$"{commonPrefix}{"participation_function"}"] = "requester"; //verify
            composedObject[$"{commonPrefix}{"participation_mode"}"] = "face-to-face communication"; //verify
            composedObject[$"{commonPrefix}{"participation_id"}"] = caseNote.DocSavedByUSerId; //verify
            composedObject[$"{commonPrefix}{"participation_name:1"}"] = caseNote.DocSavedByUSerId; //verify
            composedObject[$"{commonPrefix}{"participation_function:1"}"] = "performer"; //verify
            composedObject[$"{commonPrefix}{"participation_id:1"}"] = caseNote.DocSavedByUSerId; //verify
            composedObject[$"{commonPrefix}{"health_care_facility|name"}"] = caseNote.DocCreatedAtCareUnitId; //verify
            composedObject[$"{commonPrefix}{"health_care_facility|id"}"] = caseNote.DocCreatedAtCareUnitId; //verify
        }

        private void AddContextData(string commonPrefix, CaseNoteDto caseNote)
        {
            StringBuilder contextBuilder = new StringBuilder(commonPrefix);
            contextBuilder.Append("composer/_identifier:0");
            string contextCareUnit = contextBuilder.ToString();

            composedObject[$"{contextCareUnit}{"|name"}"] = "DocCreatedByUserId"; //verify
            composedObject[$"{contextCareUnit}{"|id"}"] = caseNote.DocCreatedByUserId;
            composedObject[$"{contextCareUnit}{"|type"}"] = "UserId";
            composedObject[$"{contextCareUnit}{"|issuer"}"] = "RSK";
            
            contextBuilder = new StringBuilder(commonPrefix);
            contextBuilder.Append("context/");
            contextCareUnit = contextBuilder.ToString();

            composedObject[$"{contextCareUnit}{"start_time"}"] = caseNote.EventDateTime;
            composedObject[$"{contextCareUnit}{"setting|code"}"] = "238";
            composedObject[$"{contextCareUnit}{"setting|value"}"] = "other care";
            composedObject[$"{contextCareUnit}{"setting|terminology"}"] = "openehr";

            contextBuilder.Append("_health_care_facility/_identifier:0");
            contextCareUnit = contextBuilder.ToString();

            composedObject[$"{contextCareUnit}{"|name"}"] = "DocCreatedAtCareUnitId"; //verify
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
            composedObject[$"{contextCareUnit}{"roll:0|value"}"] = "vårdenhet";
            
            string suffix = "identifierare:0";
            composedObject[$"{contextCareUnit}{suffix}"] = casenote.DocCreatedAtCareUnitId;
            composedObject[$"{contextCareUnit}{suffix}{"|issuer"}"] = "RSK";
            composedObject[$"{contextCareUnit}{suffix}{"|assigner"}"] = "RSK";
            composedObject[$"{contextCareUnit}{suffix}{"|type"}"] = "CareUnitId";

            contextCareUnitBuilder.Append("vårdgivare/");
            contextCareUnit = contextCareUnitBuilder.ToString();
            composedObject[$"{contextCareUnit}{"namn"}"] = casenote.DocCreatedAtCareUnitId; //verify

            composedObject[$"{contextCareUnit}{"roll:0|code"}"] = "143591000052106";
            composedObject[$"{contextCareUnit}{"roll:0|value"}"] = "vårdgivare";

            string orgNum = "organisationsnummer:0";
            composedObject[$"{contextCareUnit}{orgNum}"] = casenote.DocCreatedAtCareUnitId; //verify
            composedObject[$"{contextCareUnit}{orgNum}{"|issuer"}"] = "RSK";
            composedObject[$"{contextCareUnit}{orgNum}{"|assigner"}"] = "RSK";
            composedObject[$"{contextCareUnit}{orgNum}{"|type"}"] = "CareUnitId";

            contextCareUnitBuilder.Append("identifierare:0");
            composedObject[$"{contextCareUnitBuilder}"] = casenote.DocCreatedAtCareUnitId; //verify
            composedObject[$"{contextCareUnitBuilder}{"|issuer"}"] = "RSK";
            composedObject[$"{contextCareUnitBuilder}{"|assigner"}"] = "RSK"; //verify
            composedObject[$"{contextCareUnitBuilder}{"|type"}"] = "CareUnitId";
        }

        private void AddContextMetadata(string commonPrefix, CaseNoteDto caseNote)
        {
            // verify
            StringBuilder contextMetaData = new StringBuilder(commonPrefix);
            contextMetaData.Append("context/metadata/");
            string contextData = contextMetaData.ToString();
            composedObject[$"{contextData}{"dokument_id"}"] = caseNote.DocumentId;
            composedObject[$"{contextData}{"överordnat_dokument_id"}"] = caseNote.DocumentId; //verify
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
            if (caseNote.CounterSignerId != null) { 
                composedObject[$"{contextData}{"kontrasignerare_id"}"] = caseNote.CounterSignerId;
                composedObject[$"{contextData}{"kontrasignerare_id|issuer"}"] = "RSK";
                composedObject[$"{contextData}{"kontrasignerare_id|assigner"}"] = "RSK";
                composedObject[$"{contextData}{"kontrasignerare_id|type"}"] = "UserId";
            }

            composedObject[$"{contextData}{"tidsstämpel_för_sparat_dokument"}"] = caseNote.DocSavedTimestamp;
            composedObject[$"{contextData}{"rubriktext"}"] = caseNote.HeaderTerm;

            string suffix = "dokumentationsmall";
            composedObject[$"{contextData}{suffix}{"|code"}"] = caseNote.TemplateId;
            composedObject[$"{contextData}{suffix}{"|value"}"] = caseNote.TemplateName;
            composedObject[$"{contextData}{suffix}{"|terminology"}"] = "TC-Template-codes";

            suffix = "dokument_skapad_av_yrkestitel_id";
            composedObject[$"{contextData}{suffix}"] = caseNote.DocCreatedByProfessionId;
            composedObject[$"{contextData}{suffix}{"|issuer"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|assigner"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|type"}"] = "ProfessionId";

            suffix = "dokumentskaparens_användar_id";
            composedObject[$"{contextData}{suffix}"] = caseNote.DocCreatedByUserId;
            composedObject[$"{contextData}{suffix}{"|issuer"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|assigner"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|type"}"] = "UserId";

            suffix = "dokument_sparat_av_yrkesroll_id";
            composedObject[$"{contextData}{suffix}"] = caseNote.DocSavedByUSerId;
            composedObject[$"{contextData}{suffix}{"|issuer"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|assigner"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|type"}"] = "UserId";

            suffix = "dokument_sparat_av_användar_id";
            composedObject[$"{contextData}{suffix}"] = caseNote.DocSavedByUSerId;
            composedObject[$"{contextData}{suffix}{"|issuer"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|assigner"}"] = "RSK";
            composedObject[$"{contextData}{suffix}{"|type"}"] = "UserId";
        }

        private void AddGenericEntry(KeywordDto keyword, string commonPrefix)
        {
            int v = counterMap["generic"];
            GenericEntry.AddGenericData(composedObject, keyword, v, commonPrefix);
            counterMap["generic"]++;
        }

        private void AddCKMEntry(KeywordDto keyword, TerminologyDetails termData, string commonPrefix)
        {
            if (!counterMap.ContainsKey(keyword.TermId))
                counterMap.Add(keyword.TermId, 0);
            int v = counterMap[keyword.TermId];
            switch (termData.Code)
            {
                //create a class with the keywordname and add a method to add the data to composedObject
                case "3719": //Systolic Upper
                    SystolicUpper.AddSystolicUpperData(composedObject, keyword, v, commonPrefix);
                    break;
                case "3720": //Diastolic Lower
                    DiastolicLower.AddDiastolicLowerData(composedObject, keyword, v, commonPrefix);
                    break;
                case "4243": // Mean Artierial Pressure
                    MeanArterialPressure.AddMeanArterialPressureData(composedObject, keyword, v, commonPrefix);
                    break;
                case "4378": // Invasivt blodtryck systoliskt
                    SystolicInvasive.AddInvasiveSystolicData(composedObject, keyword, v, commonPrefix);
                    break;
                case "4379": // Invasivt blodtryck diastoliskt
                    DiastolicInvasive.AddDiastolicData(composedObject, keyword, v, commonPrefix);
                    break;
                case "6134": // Blodtryck hö arm, systoliskt
                    SystomlicHoArm.AddSystolicHoArmData(composedObject, keyword, v, commonPrefix);
                    break;
                case "6135": // Blodtryck hö arm, diastoliskt
                    DiastolicHoArm.AddDiastolicHoArmData(composedObject, keyword, v, commonPrefix);
                    break;
                case "6136": // Blodtryck vä arm, systoliskt
                    SystolicVaArm.AddSystolicVaArmData(composedObject, keyword, v, commonPrefix);
                    break;
                case "6137": // Blodtryck vä arm, diastoliskt
                    DiastolicVaArm.AddDiastolicVaArmData(composedObject, keyword, v, commonPrefix);
                    break;
                case "8980": // 24-timmars blodtryckskurva (24 hourly blood pressure curve)
                    A24HourlyBPCurve.AddA24HourlyBPCurveData(composedObject, keyword, v, commonPrefix);
                    break;
                case "1965": // Vikt
                    WeightCKMEntry.AddWeightData(composedObject, keyword, v, commonPrefix);
                    break;
                case "2896": // Födelsevikt (Birth Weight)
                    BirthWeight.AddBirthWeightData(composedObject, keyword, v, commonPrefix);
                    break;
                case "5028": // Nakenvikt (Bare weight)
                    BareWeight.AddBareWeightData(composedObject, keyword, v, commonPrefix);
                    break;
                case "1964": // Längd
                    Height.AddHeightData(composedObject, keyword, v, commonPrefix);
                    break;
                case "6179": // Längd liggande (Height Lying down)
                    HeightLyingDown.AddHeightLyingDownData(composedObject, keyword, v, commonPrefix);
                    break;
                case "6180": // Längd sittande (Height sitting)
                    HeightSitting.AddHeightSittingData(composedObject, keyword, v, commonPrefix);
                    break;
                case "8883": // BMI
                    BMI.AddBMIData(composedObject, keyword, v, commonPrefix);
                    break;
                case "2025": // Kroppstemperatur (temperature)
                    Temperature.AddTemperatureData(composedObject, keyword, v, commonPrefix);
                    break;
                case "11140": // Hjärtfrekvens (heart rate)
                    HeartRate.AddHeartRateData(composedObject, keyword, v, commonPrefix);
                    break;
                case "1978": // Pulsfrekvens (Pulse rate)
                    PulseRate.AddPulseRateData(composedObject, keyword, v, commonPrefix);
                    break;
                case "402": // Andningsfrekvens (respiratory rate)
                    RespiratoryRate.AddRespiratoryRateData(composedObject, keyword, v, commonPrefix);
                    break;
                case "1995": // Saturation
                    Saturation.AddSaturationData(composedObject, keyword, v, commonPrefix);
                    break;
                case "5251": // Syrgasnivå, % (Oxygen level)
                    OxygenLevel.AddOxygenLevelData(composedObject, keyword, v, commonPrefix);
                    break;
                case "10827": // Saturation med syrgas (Saturation with oxygen)
                    SaturationWithOxygen.AddSaturationWithOxygenData(composedObject, keyword, v, commonPrefix);
                    break;
                case "11273": // NEWS2, totalpoäng (Total Score)
                    News2TotalScore.AddTotalScoreData(composedObject, keyword, v, commonPrefix);
                    break;
                case "11274": // NEWS2, totalpoäng (hjärtfrekvens) (Total score heart rate)
                    News2TotalScoreHeartRate.AddTotalScoreHeartRateData(composedObject, keyword, v, commonPrefix);
                    break;
                case "11275": // NEWS2, totalpoäng (syremättnad2) (Total score oxygen saturation)
                    News2TotalScoreOxygenSaturation.AddTotalScoreOxygenSaturationData(composedObject, keyword, v, commonPrefix);
                    break;
                case "1276": // NEWS2, totalpoäng (syremättnad2, hjärtfrekvens) (Total score oxygen saturation, heart rate)
                    Saturation.AddSaturationData(composedObject, keyword, v, commonPrefix);
                    break;
                default:
                    AddGenericEntry(keyword, commonPrefix);
                    break;
            }
            counterMap[keyword.TermId]++;
        }

    }
}
