using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;
using Spine.Foundation.Web.OpenEhr.Client;
using System.Text;
using System.Text.RegularExpressions;
using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Services;
using TakeCare.Foundation.OpenEhr.Application.Utils;
using TakeCare.Migration.OpenEhr.Chemistry.Extraction.Models;
using TakeCare.Migration.OpenEhr.Chemistry.Transformation.Models;

namespace TakeCare.Migration.OpenEhr.Chemistry.Transformation.Services
{
    internal class CompositionService : ICompositionService
    {
        private readonly IFormProvider _formProvider;
        private readonly IPatientService _patientService;
        private readonly IUnitProvider _unitProvider;
        private readonly IResultStatusService _resultService;
        private readonly IOptions<ChemistryConfig> _options;
        private readonly IContextProvider _contextProvider;
        ContextDetails contextDetails;

        private ChemistryOpenEhrData chemistryOpenEhrData { get; set; }

        public CompositionService(IFormProvider formProvider,
                                  IPatientService patientService,
                                  IUnitProvider unitProvider,
                                  IResultStatusService resultService,
                                  IOptions<ChemistryConfig> options,
                                  IContextProvider contextProvider)
        {
            _formProvider = formProvider;
            _patientService = patientService;
            _unitProvider = unitProvider;
            _resultService = resultService;
            _options = options;
            _contextProvider = contextProvider;
        }

        public ChemistryOpenEhrData Compose(TakeCareChemistry inputData)
        {
            chemistryOpenEhrData = new ChemistryOpenEhrData();
            string commonPrefix = "";
            switch (_options.Value.Language.Current)
            {
                case "en":
                    commonPrefix = _options.Value.Template.Prefix.En;
                    break;
                case "swe":
                    commonPrefix = _options.Value.Template.Prefix.Swe;
                    break;
                default:
                    commonPrefix = _options.Value.Template.Prefix.Swe;
                    break;
            }
            
            string labResult = "laboratorieresultat";
            foreach (var data in inputData.ChemistryData)
            {
                OpenEhrChemistry chemistryData = new OpenEhrChemistry();

                chemistryData.TemplateId = _options.Value.Template.TemplateId;
                chemistryData.Namespace = _options.Value.Ehr.Namespace;
                chemistryData.Format = _options.Value.Ehr.Format;
                chemistryData.LifecycleState = _options.Value.Ehr.LifecycleState;
                chemistryData.AuditChangeType = _options.Value.Ehr.AuditChangeType;

                if (data.Attestation == null || data.Attestation.Patient == null)
                    continue;
                chemistryOpenEhrData.PatientID = _patientService.GetPatient(data.Attestation.Patient.Id).PatientId;

                #region Context reportID
                //context report id
                chemistryData.ReportID = new TcContextReportID($"{commonPrefix}")
                {
                    ReportID = data.Id,
                    Synopsis = data.Comment??"",
                };

                if (data.Lab != null && data.Lab.CareUnit != null)
                {
                    chemistryData.ReportID.LabResult = new LabResult()
                    {
                        Name = data.Lab.CareUnit.Name,
                        Context = new Context()
                        {
                            StartTime = data.ReplyTime?? "",
                            HealthCareFacility = new HealthCareFacility()
                            {
                                Name = data.Lab.CareUnit.Name,
                                Identifiers = new List<string>()
                                {
                                    data.Lab.CareUnit.Id
                                }
                            }
                        }
                    };
                }  

                chemistryData.ReportID.Language = new CodedText()
                {
                    Code = "sv",
                    Terminology = "ISO_639-1"
                };

                chemistryData.ReportID.Territory = new CodedText()
                {
                    Code = "SV",
                    Terminology = "ISO_3166-1"
                };

                chemistryData.ReportID.Category = new CodedText()
                {
                    Code = "433",
                    Terminology = "openehr"
                };

                #endregion

                #region Context CareUnit
                //context care unit data

                if (data.Saved != null && data.Saved.CareUnit != null)
                {
                    contextDetails = _contextProvider.GetContextData(data.Saved.CareUnit.Id);
                }
                chemistryData.CareUnitContext = new TcChemistryCareUnitContext($"{commonPrefix}/context/vårdenhet")
                {
                    CareUnitName = contextDetails!=null ? contextDetails.CareUnitName : "To Be decided", //verify
                    CareProviderName = contextDetails != null ? contextDetails.CareProviderName : "To be decided", //verify
                    Issuer = "RSK",
                    Assigner = "RSK",
                    CareUnitCode = "43741000",
                    CareUnitValue = "vårdenhet",
                    CareProviderId = contextDetails != null ? contextDetails.CareProviderId : "Add CareProviderId", //verify
                    CareUnitId = contextDetails != null ? contextDetails.CareUnitId : "Add Careunit id", //verify
                    CareProviderCode = "143591000052106",
                    CareProviderValue = "vårdgivare",
                    CareUnitTerminology = "http://snomed.info/sct/900000000000207008",
                    CareProviderTerminology = "http://snomed.info/sct/45991000052106"
                };
                #endregion

                #region Context Metadata
                //context metadata
                chemistryData.ContextMetadata = new TcChemistryContextMetadata($"{commonPrefix}/context/metadata");
                if(data.Type!=null)
                {
                    chemistryData.ContextMetadata.DocumentName = data.Type.Name;
                    chemistryData.ContextMetadata.DocumentCode = data.Type.Id;
                };
                if (data.Saved != null)
                {
                    chemistryData.ContextMetadata.DocumnetSavedTimestamp = data.Saved.SavedTimestamp;
                    if (data.Saved.CareUnit != null)
                    {
                        chemistryData.ContextMetadata.DocumentSavedAtCareUnit = new CareUnitDetails()
                        {
                            Id = data.Saved.CareUnit.Id,
                            Name = data.Saved.CareUnit.Name,
                            Type = "CareUnitData",
                            Assigner = "RSK",
                            Issuer = "RSK"
                        };
                    }
                }
                chemistryData.ContextMetadata.VersionId = data.VersionId;
                chemistryData.ContextMetadata.IsCopy = data.IsCopy ? "true" : "false";

                #endregion

                #region Context Attestation
                //context attestation data
                chemistryData.AttestationData = new TcChemistryAttestationData($"{commonPrefix}/context/vidimering");
                if (data.Attestation != null)
                {
                    chemistryData.AttestationData.AttestionCreatedTimestamp = data.Attestation.CreatedDateTime;
                    chemistryData.AttestationData.ResponsibleAttesterName = data.Attestation.ResponsibleAttesterName;
                    if (data.Attestation.Document != null)
                    {
                        if (data.Attestation.Document.HasDeviatingAnalysis != null)
                        {
                            chemistryData.AttestationData.HasDeviatingAnalysisResults = data.Attestation.Document.HasDeviatingAnalysis.GetValueOrDefault() ? "true" : "false";
                        }
                        
                        chemistryData.AttestationData.IsLatestVersionAttested = data.Attestation.Document.IsLatestVersionAttested ? "true" : "false";
                    }
                    chemistryData.AttestationData.Activities = new List<Activity>();

                    var activityData = new Activity();
                    activityData.ActivityData =new CodedText()
                    {
                        Code = "at0015",
                        Value = "Vidimerad",
                        Terminology = "local"
                    };
                    
                    if (data.Attestation.Attested != null)
                    {
                        activityData.ActivityTimestamp = data.Attestation.Attested.DateTime;
                        if (data.Attestation.Attested.User != null)
                        {
                            activityData.User = new User()
                            {
                                Id = data.Attestation.Attested.User.Id,
                                FullName = data.Attestation.Attested.User.FullName,
                                Assigner = "RSK",
                                Issuer = "RSK",
                                Username = data.Attestation.Attested.User.Username,
                                Type = "UserData"
                            };
                        }
                    }
                    chemistryData.AttestationData.Activities.Add(activityData);
                    chemistryData.AttestationData.ResponsibleAttester = new Attester()
                    {
                        Activity = new Activity()
                        {
                            ActivityData = new CodedText()
                            {
                                Code = "at0015",
                                Value = "Vidimerad",
                                Terminology = "local"
                            },
                        }
                    };
                    if (data.Attestation.ResponsibleAttester != null) 
                    {
                        chemistryData.AttestationData.ResponsibleAttester.User = new User()
                        {
                            Id = data.Attestation.ResponsibleAttester.Id,
                            FullName = data.Attestation.ResponsibleAttester.FullName,
                            Assigner = "RSK",
                            Issuer = "RSK",
                            Username = data.Attestation.ResponsibleAttester.Username,
                            Type = "UserData"
                        };
                    }
                    if (data.Attestation.CareUnit != null) 
                    {
                        chemistryData.AttestationData.ResponsibleAttester.CareUnit = new CareUnitDetails()
                        {
                            Id = data.Attestation.CareUnit.Id,
                            Name = data.Attestation.CareUnit.Name,
                            Type = "CareUnitData",
                            Assigner = "RSK",
                            Issuer = "RSK"                            
                        };
                    }
                }
                #endregion

                #region TestResult
                if (data.Samples != null)
                {
                    for (int i = 0; i < data.Samples.Count; i++)
                    {
                        // TC result starts here
                        var testData = new TcLabResult($"{commonPrefix}/{labResult}", i);

                        testData.OrganizationInfo = new OrganizationInfo($"{commonPrefix}/{labResult}:{i}");
                        testData.RequestingOrganization = new RequestingOrganization($"{commonPrefix}/{labResult}:{i}");

                        //test result - org info
                        if (data.Lab != null)
                        {
                            if (!string.IsNullOrWhiteSpace(data.Lab.SID))
                            {
                                testData.OrganizationInfo.Sid = new Identifier()
                                {
                                    Id = data.Lab.SID,
                                    Assigner = "RSK",
                                    Issuer = "RSK",
                                };
                            }
                            if (data.Lab.CareUnit != null)
                            {
                                testData.OrganizationInfo.CareUnit = new Identifier()
                                {
                                    Id = data.Lab.CareUnit.Id,
                                    Assigner = "RSK",
                                    Issuer = "RSK",
                                };
                                testData.OrganizationInfo.Name = data.Lab.CareUnit.Name;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(data.OriginalRecipientsAddress))
                            testData.OrganizationInfo.Address.Add(data.OriginalRecipientsAddress);

                        if (!string.IsNullOrWhiteSpace(data.ReferringDoctor))
                            testData.RequestingOrganization.PersonName = data.ReferringDoctor;

                        // test result - test request details
                        if (!string.IsNullOrWhiteSpace(data.ReferralId))
                        {
                            testData.TestRequestDetails = new TestRequestDetails()
                            {
                                ReceiverOrderIdentifier = data.LID,
                                RequesterOrderIdentifier = data.ReferralId
                            };
                        }

                        if (data.InvoiceeCareUnitExternalId != null)
                        {
                            testData.RequestingOrganization.InvoiceeCareUnitExternalId = new Identifier()
                            {
                                Id = data.InvoiceeCareUnitExternalId,
                                Assigner = "RSK",
                                Issuer = "RSK",
                                Type = "CareUnitData"
                            };
                        }

                        if (data.OrdererCareUnitExternalId != null)
                        {
                            testData.RequestingOrganization.CareUnitExternalId = new Identifier()
                            {
                                Id = data.OrdererCareUnitExternalId,
                                Assigner = "RSK",
                                Issuer = "RSK",
                                Type = "CareUnitData"
                            };
                        }

                        testData.InternalLaboratoryIdentifier = data.Samples[i].Group;

                        //test result - point of care test
                        testData.PointOfCareTest = "false";

                        //test result - Event data

                        string testResultPrefix = $"{commonPrefix}/{labResult}";
                        testData.Events = new AnyEvent($"{testResultPrefix}", i);

                        // event - time
                        if (!string.IsNullOrWhiteSpace(data.SamplingDateTime))
                        {
                            testData.Events.Time = data.SamplingDateTime;
                        }

                        // event - test name
                        if (data.Order != null)
                        {
                            if (data.Order.Type != null)
                            {
                                testData.Events.TestName = new CodedText()
                                {
                                    Code = data.Order.Type.Id,
                                    Value = data.Order.Type.Name,
                                    Terminology = "TC-Lab-Test"
                                };
                            }
                        }

                        testData.Events.TestName = new CodedText()
                        {
                            Code = (data.Order!=null && data.Order.Type!=null) ? data.Order.Type.Id: "KliniskKemi",
                            Value = (data.Order != null && data.Order.Type != null) ? data.Order.Type.Id : "Klinisk Kemi",
                            Terminology = "TC-Lab-Test"
                        };

                        // event - overall test status

                        if (data.IsFinal!=null)
                        {
                            ResultStatusDetails result = _resultService.GetResult(data.IsFinal);
                            if (result != null)
                            {
                                testData.Events.OverallTestStatus = new CodedText()
                                {
                                    Code = result.Target[0].Code,
                                    Value = result.Target[0].Display,
                                    Terminology = "local"
                                };
                            }
                        }

                        // event - overall test status timestamp
                        if (data.ReplyTime != null)
                        {
                            testData.Events.OverallTestStatusTimestamp = data.ReplyTime;
                        }

                        // event - clinical info provided
                        if (!string.IsNullOrWhiteSpace(data.OrderComment))
                        {
                            testData.Events.ClinicalInformationProvided = data.OrderComment;
                        }

                        testData.Events.DiagnosticServiceCategory = new CodedText()
                        {
                            Code = "4311000179106",
                            Value = "Klinisk kemi (utlåtande från kemisk patologi)",
                            Terminology = "http://snomed.info/id/"
                        };

                        /*// event - conclusion
                        if (data.Attestation != null && data.Attestation.Document != null)
                        {
                            testData.Events.Conclusion = "To be decided"; // verify
                        }*/

                        // event - comments
                        if (!string.IsNullOrWhiteSpace(data.Samples[i].GroupComment))
                        {
                            testData.Events.Comments.Add(data.Samples[i].GroupComment);
                        }

                        if (data.Samples[i].Analyses != null && data.Samples[i].Analyses.Count > 0)
                        {
                            for (int k = 0; k < data.Samples[i].Analyses.Count; k++)
                            {
                                var analyteResult = new LabAnalyteResult($"{testResultPrefix}:{i}", k);

                                //name
                                if (!string.IsNullOrWhiteSpace(data.Samples[i].Analyses[k].AnalysisId))
                                {
                                    analyteResult.AnalyteName = new CodedText()
                                    {
                                        Code = data.Samples[i].Analyses[k].AnalysisId, 
                                        Value = data.Samples[i].Analyses[k].AnalysisInfo,
                                        Terminology = "TC-Lab-Analyte"
                                    };
                                }

                                //comment
                                if (!string.IsNullOrWhiteSpace(data.Samples[i].Analyses[k].AnalysisComment))
                                {
                                    analyteResult.Comments.Add(data.Samples[i].Analyses[k].AnalysisComment);
                                }

                                // reference range guide
                                if (!string.IsNullOrWhiteSpace(data.Samples[i].Analyses[k].ReferenceArea))
                                {
                                    analyteResult.ReferenceRangeGuide = ConvertToCorrectFormat(data.Samples[i].Analyses[k].ReferenceArea);
                                }

                                // keyword
                                analyteResult.AnalyteKeyword = new Keyword($"{testResultPrefix}:{i}/analysresultat:{k}")
                                {
                                    //Comment = data.Samples[j].Analyses[k].AnalysisComment,
                                    //NameCode = data.Samples[j].Analyses[k].AnalysisId,
                                    //NameValue = data.Samples[j].Analyses[k].AnalysisInfo,
                                    //NameTerminology = "external_terminology",
                                    //ValueQuantityMagnitude = data.Samples[j].Analyses[k].Value,
                                    //ValueQuantityUnit = data.Samples[j].Analyses[k].Unit,
                                    OriginalUnit = data.Samples[i].Analyses[k].Unit
                                };

                                // free text and quantity
                                if (!string.IsNullOrWhiteSpace(data.Samples[i].Analyses[k].Value))
                                {
                                    
                                    if (decimal.TryParse(data.Samples[i].Analyses[k].Value, out decimal result)){
                                        if (!string.IsNullOrWhiteSpace(data.Samples[i].Analyses[k].Unit))
                                        {
                                            analyteResult.AnalyteResultQuantity.Add(new MetricValue()
                                            {
                                                Magnitude = data.Samples[i].Analyses[k].Value,
                                                Unit = _unitProvider.GetOpenEhrUnit(data.Samples[i].Analyses[k].Unit),
                                                NormalStatus = data.Samples[i].Analyses[k].IsDeviating ? "H" : "N"
                                            });
                                        }
                                        else
                                        {
                                            /*analyteResult.AnalyteResultCodedText = new List<CodedText>()
                                            {
                                                new CodedText()
                                                {
                                                    Code = data.Samples[i].Analyses[k].Value,
                                                    Value = data.Samples[i].Analyses[k].Value,
                                                    Terminology = "external_terminology"
                                                }
                                            };*/
                                            analyteResult.AnalyteResultFreeText.Add(data.Samples[i].Analyses[k].Value);
                                        }
                                    }
                                    else {
                                        analyteResult.AnalyteResultFreeText.Add(data.Samples[i].Analyses[k].Value);
                                    }
                                }

                                testData.Events.AnalyteResult.Add(analyteResult);
                            }
                        }                     

                        chemistryData.TestResult.Add(testData);
                    }
                }
                #endregion

                chemistryOpenEhrData.ChemistryData.Add(chemistryData);
            }

            return chemistryOpenEhrData;
        }


        private string ConvertToCorrectFormat(string input)
        {
            // Use a regular expression to find numbers with commas and replace them with periods
            string pattern = @"(\d),(\d)";
            string replacement = "$1.$2";

            // Perform the replacement
            string result = Regex.Replace(input, pattern, replacement);

            return result;
        }
    }

}
