using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Foundation.Web.OpenEhr.Client;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Extension;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using SpineBase = Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using TcBase = TakeCare.Foundation.OpenEhr.Archetype.Entry;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services
{
    public class CompositionService : ICompositionService
    {
        private readonly IPatientService _patientService;
        private readonly ITerminologyProvider _terminologyProvider;
        private readonly IRoleProvider _roleProvider;
        private readonly IFormProvider _formProvider;
        private readonly IContextProvider _contextProvider;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IUnitProvider _unitProvider;
        Dictionary<string, int> counterMap = new Dictionary<string, int>();
        OpenEhrCaseNote openEhrCaseNote;
        ContextDetails contextData;
        UserContextDetails userData;
        private Terminology _language = new Terminology() { Code = "sv", Value = "ISO_639-1" };
        private Terminology _encoding = new Terminology() { Code = "UTF-8", Value = "IANA_character-sets" };

        public CompositionService(IPatientService patientService,
                                  ITerminologyProvider terminologyProvider,
                                  IRoleProvider roleProvider,
                                  IFormProvider formProvider,
                                  IContextProvider contextProvider,
                                  IUserContextProvider userContextProvider,
                                  IUnitProvider unitProvider)
        {
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
            _roleProvider = roleProvider;
            _formProvider = formProvider;
            _contextProvider = contextProvider;
            _userContextProvider = userContextProvider;
            _unitProvider = unitProvider;
        }

        public CareDocumentOpenEhrData Compose(CareDocumentationDto inputData)
        {
            CareDocumentOpenEhrData careDocumentOpenEhrData = new CareDocumentOpenEhrData()
            {
                PatientID = _patientService.GetPatient(inputData.PatientId).PatientId,
                CaseNotes = new List<OpenEhrCaseNote>()
            };
            string commonPrefix = "vårdkontakt";

            if (inputData != null && inputData.CaseNotes != null && inputData.CaseNotes.Count > 0)
            {
                foreach (var caseNote in inputData.CaseNotes)
                {
                    counterMap = new Dictionary<string, int>();
                    counterMap.Add("generic", 0);
                    contextData = _contextProvider.GetContextData(caseNote.DocCreatedAtCareUnitId);
                    openEhrCaseNote = new OpenEhrCaseNote();

                    // Add the ctx static data
                    openEhrCaseNote.Context = new SpineBase.Context()
                    {
                        Language = "sv",
                        Territory = "US",
                        HealthCareFacilityId = caseNote.DocCreatedAtCareUnitId,
                        HealthCareFacilityName = contextData.CareUnitName,
                        ComposerName = "UserName",
                        IdNamespace = "HOSPITAL-NS",
                        IdScheme = "HOSPITAL-NS",
                        ParticipationName = "UserName",
                        ParticipationFunction = caseNote.DocSavedByUSerId,
                        ParticipationMode = "face-to-face communication",
                        ParticipationId = caseNote.DocSavedByUSerId,
                        Tags = GetTags().GetAwaiter().GetResult()
                    };

                    //Add ctx data
                    openEhrCaseNote.ContextInformation = new TcBase.TcContextInformation(commonPrefix)
                    {
                        Composer = new TcBase.ComposerIdentifier()
                        {
                            Name = caseNote.DocCreatedByUserId,
                            Id = caseNote.DocCreatedByUserId,
                            Type = "UserId",
                            Issuer = "RSK"
                        },
                        HealthCareFacility = new TcBase.HealthCareFacilityIdentifier()
                        {
                            Name = (contextData != null) ? contextData.CareUnitName : caseNote.DocCreatedAtCareUnitId,
                            Id = caseNote.DocCreatedAtCareUnitId,
                            Type = "CareUnitId",
                            Issuer = "RSK"
                        },
                        Setting = new TcBase.Setting()
                        {
                            Code = "238",
                            Value = "other care",
                            Terminology = "openehr"
                        },
                        StartTime = caseNote.EventDateTime
                    };

                    //Add context metadata
                    openEhrCaseNote.ContextMetadata = new TcBase.TcCaseNoteContextMetadata($"{commonPrefix}/context/metadata")
                    {
                        DocumentTitle = caseNote.DocumentTitle,
                        DocumentId = caseNote.DocumentId,
                        ApprovedForPatient = caseNote.ApprovedForPatient,
                        DocumentCode = caseNote.DocumentCode,
                        DocCreatedTimestamp = caseNote.DocCreatedTimestamp,
                        DocCreatedByProfession = new TcBase.Profession()
                        {
                            Code = caseNote.DocCreatedByProfessionId,
                            Value = _roleProvider.GetProfessionName(caseNote.DocCreatedByProfessionId).Display,
                            Terminology = "http://tcpoc.rs.sk/tcpoc-professional-roles"
                        },
                        DocCreatedByUser = new TcBase.User()
                        {
                            Id = caseNote.DocCreatedByUserId,
                            Issuer = "RSK",
                            Assigner = "RSK",
                            Type = "UserId",
                            Name = _userContextProvider.GetUserContextData(caseNote.DocCreatedByUserId)
                        },
                        DocSavedByUSer = new TcBase.User()
                        {
                            Id = caseNote.DocSavedByUSerId,
                            Issuer = "RSK",
                            Assigner = "RSK",
                            Type = "UserId",
                            Name = _userContextProvider.GetUserContextData(caseNote.DocSavedByUSerId)
                        },
                        DocCreatedAtCareUnitId = caseNote.DocCreatedAtCareUnitId,

                        DocSavedTimestamp = caseNote.DocSavedTimestamp,
                        EventDateTime = caseNote.EventDateTime,
                        HeaderTerm = caseNote.HeaderTerm,
                        SignedBy = new TcBase.User()
                        {
                            Id = caseNote.SignedById,
                            Issuer = "RSK",
                            Assigner = "RSK",
                            Type = "UserId"
                        },
                        Signer = new TcBase.User()
                        {
                            Id = caseNote.SignerId,
                            Issuer = "RSK",
                            Assigner = "RSK",
                            Type = "UserId"
                        },
                        CounterSigner = new TcBase.User()
                        {
                            Id = caseNote.CounterSignerId,
                            Issuer = "RSK",
                            Assigner = "RSK",
                            Type = "UserId"
                        },
                        SignedTimestamp = caseNote.SignedTimestamp,
                        TemplateId = caseNote.TemplateId,
                        TemplateName = caseNote.TemplateName
                    };

                    // Add context care unit data
                    openEhrCaseNote.CareUnitContext = new TcCaseNoteCareUnitContext($"{commonPrefix}/context/vårdenhet")
                    {
                        CareUnitName = (contextData != null) ? contextData.CareUnitName : caseNote.DocCreatedAtCareUnitId,
                        Assigner = "RSK",
                        Issuer = "RSK",
                        CareUnitCode = "43741000",
                        CareUnitValue = "vårdenhet",
                        CareProviderId = (contextData != null) ? contextData.CareProviderId : caseNote.DocCreatedAtCareUnitId,
                        OrgId = (contextData != null) ? contextData.CareProviderId : caseNote.DocCreatedAtCareUnitId,
                        CareProviderCode = "143591000052106",
                        CareProviderValue = "vårdgivare",
                        CareProviderName = (contextData != null) ? contextData.CareProviderName : caseNote.DocCreatedAtCareUnitId
                    };

                    //Add the Generic entry
                    if (caseNote != null && caseNote.Keywords != null && caseNote.Keywords.Count > 0)
                    {

                        foreach (var keyword in caseNote.Keywords)
                        {
                            GetArchetypeEntry(keyword, commonPrefix);
                        }
                    }
                    careDocumentOpenEhrData.CaseNotes.Add(openEhrCaseNote);
                }
            }
            return careDocumentOpenEhrData;
        }

        private async Task<List<SpineBase.TagValue>> GetTags()
        {
            Form form = await _formProvider.GetFormDetails("RSK - Journal Encounter Form");

            return new List<SpineBase.TagValue>
            {
                new SpineBase.TagValue { tag = "formName", value = form.Name },
                new SpineBase.TagValue { tag = "formVersion", value = form.Version }
            };
        }

        private void GetArchetypeEntry(KeywordDto keyword, string commonPrefix)
        {
            if (!counterMap.ContainsKey(keyword.TermId))
                counterMap.Add(keyword.TermId, 0);
            int v = counterMap[keyword.TermId]++;
            BaseEntry ckmEntry = null;
            TerminologyDetails termData = _terminologyProvider.GetTerminology(keyword.TermId);
            switch (keyword.TermId)
            {
                //create a class with the keywordname and add a method to add the data to composedObject
                case "3719": //Systolic Upper

                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcSystolicUpper($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Systolic = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/blodtryck_systoliskt_-_övre:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "3720": //Diastolic Lower
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcDiastolicLower($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/blodtryck_diastoliskt_-_nedre:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "4243": // Mean Artierial Pressure
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcMeanArterialBP($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/map:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "4378": // Invasivt blodtryck systoliskt
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcInvasiveSystolic($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Method = "at1040",
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/invasivt_blodtryck_systoliskt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "4379": // Invasivt blodtryck diastoliskt
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcInvasiveDiastolic($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Method = "at1040",
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/invasivt_blodtryck_diastoliskt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "6134": // Blodtryck right arm, systoliskt
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcRightArmSystolic($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Systolic = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/blodtryck_hö_arm_systoliskt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "6135": // Blodtryck right arm, diastoliskt
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcRightArmDiastolic($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/blodtryck_hö_arm_diastoliskt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "6136": // Blodtryck left arm, systoliskt
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcLeftArmSystolic($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Systolic = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/blodtryck_vä_arm_systoliskt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "6137": // Blodtryck left arm, diastoliskt
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcLeftArmDiastolic($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/blodtryck_vä_arm_diastoliskt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "8980": // 24-timmars blodtryckskurva (24 hourly blood pressure curve)
                    if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal))
                    {
                        string[] bpValue = keyword.Value.TextVal.Split('/');
                        ckmEntry = new TcBase.TcBPCurve24Hour($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Systolic = new SpineBase.Measurement
                            {
                                Magnitude = bpValue[0].GetDecimalNumberValue() ?? 0m,
                                Units = "mm[Hg]"
                            },
                            Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = bpValue[1].GetDecimalNumberValue() ?? 0m,
                                Units = "mm[Hg]"
                            },
                            MathFunctionCode = "146",
                            MathFunctionValue = "mean",
                            MathFunctionTerminology = "openehr",
                            Width = "P2DT10H38M",
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/a24-timmars_blodtryckskurva:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;

                case "1965": // Vikt
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcWeight($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            WeightValue = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/vikt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "2896": // Födelsevikt (Birth Weight)

                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcBirthWeight($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            WeightValue = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/födelsevikt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "5028": // Nakenvikt (Bare weight)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcBareWeight($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            WeightValue = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/nakenvikt:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "1964": // Längd
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcLength($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            LengthValue = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/längd:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "6179": // Längd liggande (Height Lying down)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcHorizontalLength($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            BodyPosition = new TcLengthTerminology()
                            {
                                Code = "at0020",
                                Value = "Liggande",
                                Terminology = "local"
                            },
                            LengthValue = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/längd_liggande:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "6180": // Längd sittande (Height sitting)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcSittingLength($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Method = "Mätt sittandes från sittknöl till huvudknopp",
                            BodyPosition = new TcLengthTerminology()
                            {
                                Code = "at0037",
                                Value = "Sittställning",
                                Terminology = "local"
                            },
                            BodySegmentName = new TcLengthTerminology()
                            {
                                Code = "at0017",
                                Value = "Sitthöjd",
                                Terminology = "local"
                            },
                            LengthValue = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/längd_sittande:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "8883": // BMI
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcBMI($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg/m2"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/bmi:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "2025": // Kroppstemperatur (temperature)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcBodyTemparature($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "Cel"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/kroppstemperatur:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "11140": // Hjärtfrekvens (heart rate)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcHeartRate($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/hjärtfrekvens:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "1978": // Pulsfrekvens (Pulse rate)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcPulseRate($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/pulsfrekvens:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "402": // Andningsfrekvens (respiratory rate)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcRespiratoryRate($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.Measurement
                            {
                                Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Units = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min"
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/andningsfrekvens:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "1995": // Saturation
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcSaturation($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.MeasurementFraction
                            {
                                Numerator = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Denominator = 100,
                                Type = 2,
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/saturation:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "5251": // Syrgasnivå, % (Oxygen level)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcOxygenLevel($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.MeasurementFraction
                            {
                                Numerator = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Denominator = 100,
                                Type = 2,
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/syrgasnivå:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "10827": // Saturation med syrgas (Saturation with oxygen)
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        ckmEntry = new TcBase.TcOxygenSaturation($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Measurement = new SpineBase.MeasurementFraction
                            {
                                Numerator = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                                Denominator = 100,
                                Type = 2,
                            },
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/saturation_med_syrgas:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = !string.IsNullOrEmpty(keyword.Value.NumVal.Unit) ?
                                                keyword.Value.NumVal.Unit : ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                
                case "11273": // NEWS2, totalpoäng (Total Score)
                    if (keyword.Value != null)
                    {
                        if (keyword.Value.NumVal != null)
                        {
                            ckmEntry = new TcBase.TcNewsTotalScore($"{commonPrefix}/ickm", v.ToString())
                            {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0,

                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit = ""
                                }
                            };
                        }
                        else if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                        {
                            ckmEntry = new TcBase.TcNewsTotalScore($"{commonPrefix}/ickm", v.ToString())
                            {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.TextVal.GetDecimalNumberValue() ?? 0m,
                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit = ""
                                }
                            };
                        }
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "11274": // NEWS2, totalpoäng (hjärtfrekvens) (Total score heart rate)
                    if (keyword.Value != null)
                    {
                        if (keyword.Value.NumVal != null)
                        {
                            ckmEntry = new TcBase.TcNewsTotalScoreHeartRate($"{commonPrefix}/ickm", v.ToString())
                            {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0,

                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng_hjärtfrekvens:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit = ""
                                }
                            };
                        }
                        else if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                        {
                            ckmEntry = new TcBase.TcNewsTotalScoreHeartRate($"{commonPrefix}/ickm", v.ToString())
                            {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.TextVal.GetDecimalNumberValue() ?? 0m,
                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng_hjärtfrekvens:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit = ""
                                }
                            };
                        }
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "11275": // NEWS2, totalpoäng (syremättnad2) (Total score saturation)
                    if (keyword.Value != null)
                    {
                        if (keyword.Value.NumVal != null)
                        {
                            ckmEntry = new TcBase.TcNewsTotalScoreO2Sat($"{commonPrefix}/ickm", v.ToString())
                            {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0,

                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng_syremättnad2:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit = ""
                                }
                            };
                        }
                        else if (!string.IsNullOrEmpty(keyword.Value.TextVal))
                        {
                            ckmEntry = new TcBase.TcNewsTotalScoreO2Sat($"{commonPrefix}/ickm", v.ToString())
                            {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.TextVal.GetDecimalNumberValue() ?? 0m,
                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng_syremättnad2:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit = ""
                                }
                            };
                        }
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "11276": // NEWS2, totalpoäng (syremättnad2, hjärtfrekvens) (Total score oxygen saturation, heart rate)
                    if (keyword.Value != null)
                    {
                        if (keyword.Value.NumVal != null)
                        {
                             ckmEntry = new TcBase.TcNewsScoreHRO2Sat($"{commonPrefix}/ickm", v.ToString())
                             {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0,

                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng_syremättnad2_hjärtfrekvens:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit =  ""
                                }
                            };
                        }
                        else if(!string.IsNullOrEmpty(keyword.Value.TextVal))
                        {
                            ckmEntry = new TcBase.TcNewsScoreHRO2Sat($"{commonPrefix}/ickm", v.ToString())
                            {
                                Uid = keyword.Guid,
                                Language = _language,
                                Encoding = _encoding,
                                ScorePoint = keyword.Value.TextVal.GetDecimalNumberValue() ?? 0m,
                                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/news2_totalpoäng_syremättnad2_hjärtfrekvens:{v}")
                                {
                                    Value = termData.TermName,
                                    Code = termData.TermId,
                                    Terminology = termData.Terminology,
                                    Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                    EntryUid = keyword.Guid,
                                    Comment = keyword.Comment,
                                    Level = keyword.ParentCount,
                                    EhrUriValues = keyword.Children,
                                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                    OriginalUnit = ""
                                }
                            };
                        }
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                   break;
                
                case "1849": // Blood pressure
                    if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal))
                    {
                        string[] bpValue = keyword.Value.TextVal.Split('/');
                        ckmEntry = new TcBase.TcBloodPressure($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            Systolic = new SpineBase.Measurement
                            {
                                Magnitude = bpValue[0].GetDecimalNumberValue() ?? 0m,
                                Units = "mm[Hg]"
                            },
                            Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = bpValue[1].GetDecimalNumberValue() ?? 0m,
                                Units = "mm[Hg]"
                            },

                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/blodtryck:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                case "6531": // diagnos_enl_icd-1 (Problem Diagnosis)
                    if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal))
                    {
                        ckmEntry = new TcBase.TcProblemDiagnosis($"{commonPrefix}/ickm", v.ToString())
                        {
                            Uid = keyword.Guid,
                            Language = _language,
                            Encoding = _encoding,
                            ProblemName = keyword.Name,
                            Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/ickm/diagnos_enl_icd-10:{v}")
                            {
                                Value = termData.TermName,
                                Code = termData.TermId,
                                Terminology = termData.Terminology,
                                Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype,
                                EntryUid = keyword.Guid,
                                Comment = keyword.Comment,
                                Level = keyword.ParentCount,
                                EhrUriValues = keyword.Children,
                                TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                                NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                                NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                                OriginalUnit = ""
                            }
                        };
                        openEhrCaseNote.Entries.Add(ckmEntry);
                    }
                    break;
                default:
                    GetGenericEntry(keyword, commonPrefix);
                    break;
            }
        }

        private void GetGenericEntry(KeywordDto keyword, string commonPrefix)
        {
            int v = counterMap["generic"];
            TerminologyDetails datatype = _terminologyProvider.GetTerminology(keyword.TermId);
            BaseEntry genericEntry = new TcCaseNoteGenericEntry($"{commonPrefix}/genrisk_händelse", v.ToString())
            {
                Uid = keyword.Guid,
                Keyword = new TcBase.KeywordCaseNote($"{commonPrefix}/genrisk_händelse:{v}")
                {
                    Value = keyword.Name,
                    Code = keyword.TermId,
                    Terminology = "TC-Datatypes",
                    Datatype = (datatype != null) ? datatype.Datatype : "Text",
                    EntryUid = keyword.Guid,
                    Comment = keyword.Comment,
                    Level = keyword.ParentCount,
                    TextValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal)) ?
                                keyword.Value.TextVal : "",
                    NumValue = (keyword.Value != null && keyword.Value.NumVal != null) ?
                                keyword.Value.NumVal.GetDecimalValue() : null,
                    NumUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : null,
                    TermIDValue = (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TermId)) ?
                                keyword.Value.TermId : "",
                    EhrUriValues = keyword.Children,
                    OriginalUnit = (keyword.Value != null
                                && keyword.Value.NumVal != null
                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                keyword.Value.NumVal.Unit : ""
                }
            };

            counterMap["generic"]++;
            openEhrCaseNote.Entries.Add(genericEntry);
        }
    }
}
