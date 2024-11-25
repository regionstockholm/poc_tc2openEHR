using Microsoft.Extensions.Options;
using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Foundation.Web.OpenEhr.Client;
using System.Globalization;
using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Services;
using TakeCare.Foundation.OpenEhr.Application.Utils;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
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
        private readonly IOptions<CareDocConfig> _options;
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
                                  IUnitProvider unitProvider,
                                  IOptions<CareDocConfig> options)
        {
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
            _roleProvider = roleProvider;
            _formProvider = formProvider;
            _contextProvider = contextProvider;
            _userContextProvider = userContextProvider;
            _unitProvider = unitProvider;
            _options = options;
        }

        public CareDocumentOpenEhrData Compose(CareDocumentationDto inputData)
        {
            CareDocumentOpenEhrData careDocumentOpenEhrData = new CareDocumentOpenEhrData()
            {
                PatientID = _patientService.GetPatient(inputData.PatientId).PatientId,
                CaseNotes = new List<OpenEhrCaseNote>()
            };
            string commonPrefix = "";
            switch (_options.Value.Language.Current)
            {
                case "en":
                    commonPrefix = _options.Value.Template.Prefix.En;
                    break;
                case "sv":
                    commonPrefix = _options.Value.Template.Prefix.Sv;
                    break;
                default:
                    commonPrefix = _options.Value.Template.Prefix.Sv;
                    break;
            }

            if (inputData != null && inputData.CaseNotes != null && inputData.CaseNotes.Count > 0)
            {
                foreach (var caseNote in inputData.CaseNotes)
                {
                    counterMap = new Dictionary<string, int>();
                    counterMap.Add("generic", 0);
                    contextData = _contextProvider.GetContextData(caseNote.DocCreatedAtCareUnitId);
                    if(contextData == null)
                    {
                        throw new Exception($"Invalid CareUnit Id: {caseNote.DocCreatedAtCareUnitId}");
                    }
                    openEhrCaseNote = new OpenEhrCaseNote();


                    openEhrCaseNote.TemplateId = _options.Value.Template.TemplateId;
                    openEhrCaseNote.Namespace = _options.Value.Ehr.Namespace;
                    openEhrCaseNote.Format = _options.Value.Ehr.Format;
                    openEhrCaseNote.LifecycleState = _options.Value.Ehr.LifecycleState;
                    openEhrCaseNote.AuditChangeType = _options.Value.Ehr.AuditChangeType;


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
                        StartTime = caseNote.EventDateTime,
                        Language = new CodedText()
                        {
                            Code = "sv",
                            Terminology = "ISO_639-1"
                        },
                        Territory = new CodedText()
                        {
                            Code = "SV",
                            Terminology = "ISO_3166-1"
                        },
                        Category = new CodedText()
                        {
                            Code = "433",
                            Value = "event",
                            Terminology = "openehr"
                        }
                    };

                    //Add context metadata
                    openEhrCaseNote.ContextMetadata = new TcBase.TcCaseNoteContextMetadata($"{commonPrefix}")
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
                    openEhrCaseNote.CareUnitContext = new TcCaseNoteCareUnitContext($"{commonPrefix}")
                    {
                        CareUnitName = (contextData != null) ? contextData.CareUnitName : caseNote.DocCreatedAtCareUnitId,
                        CareProviderName = (contextData != null) ? contextData.CareProviderName : caseNote.DocCreatedAtCareUnitId,
                        CareUnitId = new Identifier()
                        {
                            Value = caseNote.DocCreatedAtCareUnitId,
                            Assigner = "RSK",
                            Issuer = "RSK",
                            Type = "CareUnitId"
                        },
                        CareProviderId = new Identifier()
                        {
                            Value = (contextData != null) ? contextData.CareProviderId : caseNote.DocCreatedAtCareUnitId,
                            Assigner = "RSK",
                            Issuer = "RSK",
                            Type = "CareProviderId"
                        },
                        OrgId = new Identifier()
                        {
                            Value = (contextData != null) ? contextData.CareProviderId : caseNote.DocCreatedAtCareUnitId,
                            Assigner = "RSK",
                            Issuer = "RSK",
                            Type = "CareProviderId"
                        },
                        CareUnitCode = "43741000",
                        CareUnitValue = "vårdenhet",
                        CareProviderCode = "143591000052106",
                        CareProviderValue = "vårdgivare"                       
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
            Form form = await _formProvider.GetFormDetails(_options.Value.Form.Name);

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
            //BaseEntry ckmEntry = null;
            TerminologyDetails termData = _terminologyProvider.GetTerminology(keyword.TermId);
            switch (keyword.TermId)
            {
                //create a class with the keywordname and add a method to add the data to composedObject
                case "3719": //Systolic Upper
                    TcSystolicUpper systolicUpper = new TcBase.TcSystolicUpper($"{commonPrefix}/ickm", v.ToString());
                    systolicUpper.Uid = keyword.Guid;
                    systolicUpper.Language = _language;
                    systolicUpper.Encoding = _encoding;
                    systolicUpper.Keyword.Value = termData.TermName;
                    systolicUpper.Keyword.Code = termData.TermId;
                    systolicUpper.Keyword.Terminology = termData.Terminology;
                    systolicUpper.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    systolicUpper.Keyword.EntryUid = keyword.Guid;
                    systolicUpper.Keyword.Comment = keyword.Comment;
                    systolicUpper.Keyword.Level = keyword.ParentCount;
                    systolicUpper.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        systolicUpper.Systolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        systolicUpper.Keyword.NumValue =keyword.Value.NumVal.GetDecimalValue() ?? null;
                        systolicUpper.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";

                        systolicUpper.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            systolicUpper.Systolic = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            systolicUpper.Keyword.NumValue = result;
                            systolicUpper.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Systolic Upper value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(systolicUpper);
                    break;

                case "3720": //Diastolic Lower
                    TcDiastolicLower diastolicLower = new TcBase.TcDiastolicLower($"{commonPrefix}/ickm", v.ToString());
                    diastolicLower.Uid = keyword.Guid;
                    diastolicLower.Language = _language;
                    diastolicLower.Encoding = _encoding;
                    diastolicLower.Keyword.Value = termData.TermName;
                    diastolicLower.Keyword.Code = termData.TermId;
                    diastolicLower.Keyword.Terminology = termData.Terminology;
                    diastolicLower.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    diastolicLower.Keyword.EntryUid = keyword.Guid;
                    diastolicLower.Keyword.Comment = keyword.Comment;
                    diastolicLower.Keyword.Level = keyword.ParentCount;
                    diastolicLower.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        diastolicLower.Diastolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        diastolicLower.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        diastolicLower.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        diastolicLower.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            diastolicLower.Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            diastolicLower.Keyword.NumValue = result;
                            diastolicLower.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Diastolic Lower value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(diastolicLower);
                    break;

                case "4243": // Mean Artierial Pressure
                    TcMeanArterialBP tcMeanArterialBP = new TcBase.TcMeanArterialBP($"{commonPrefix}/ickm", v.ToString());
                    tcMeanArterialBP.Uid = keyword.Guid;
                    tcMeanArterialBP.Language = _language;
                    tcMeanArterialBP.Encoding = _encoding;
                    tcMeanArterialBP.Keyword.Value = termData.TermName;
                    tcMeanArterialBP.Keyword.Code = termData.TermId;
                    tcMeanArterialBP.Keyword.Terminology = termData.Terminology;
                    tcMeanArterialBP.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcMeanArterialBP.Keyword.EntryUid = keyword.Guid;
                    tcMeanArterialBP.Keyword.Comment = keyword.Comment;
                    tcMeanArterialBP.Keyword.Level = keyword.ParentCount;
                    tcMeanArterialBP.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcMeanArterialBP.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        tcMeanArterialBP.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcMeanArterialBP.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        tcMeanArterialBP.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcMeanArterialBP.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            tcMeanArterialBP.Keyword.NumValue = result;
                            tcMeanArterialBP.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Mean Arterial Pressure value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcMeanArterialBP);
                    break;

                case "4378": // Invasivt blodtryck systoliskt
                    TcInvasiveSystolic tcInvasiveSystolic = new TcBase.TcInvasiveSystolic($"{commonPrefix}/ickm", v.ToString());
                    tcInvasiveSystolic.Uid = keyword.Guid;
                    tcInvasiveSystolic.Language = _language;
                    tcInvasiveSystolic.Encoding = _encoding;
                    tcInvasiveSystolic.Method = "at1040";
                    tcInvasiveSystolic.Keyword.Value = termData.TermName;
                    tcInvasiveSystolic.Keyword.Code = termData.TermId;
                    tcInvasiveSystolic.Keyword.Terminology = termData.Terminology;
                    tcInvasiveSystolic.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcInvasiveSystolic.Keyword.EntryUid = keyword.Guid;
                    tcInvasiveSystolic.Keyword.Comment = keyword.Comment;
                    tcInvasiveSystolic.Keyword.Level = keyword.ParentCount;
                    tcInvasiveSystolic.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcInvasiveSystolic.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        tcInvasiveSystolic.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcInvasiveSystolic.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        tcInvasiveSystolic.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcInvasiveSystolic.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            tcInvasiveSystolic.Keyword.NumValue = result;
                            tcInvasiveSystolic.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Invasive Systolic value is not a number: {keyword.Value.TextVal}");
                        }
                    }

                    openEhrCaseNote.Entries.Add(tcInvasiveSystolic);
                    break;

                case "4379": // Invasivt blodtryck diastoliskt
                    TcInvasiveDiastolic tcInvasiveDiastolic = new TcBase.TcInvasiveDiastolic($"{commonPrefix}/ickm", v.ToString());
                    tcInvasiveDiastolic.Uid = keyword.Guid;
                    tcInvasiveDiastolic.Language = _language;
                    tcInvasiveDiastolic.Encoding = _encoding;
                    tcInvasiveDiastolic.Method = "at1040";
                    tcInvasiveDiastolic.Keyword.Value = termData.TermName;
                    tcInvasiveDiastolic.Keyword.Code = termData.TermId;
                    tcInvasiveDiastolic.Keyword.Terminology = termData.Terminology;
                    tcInvasiveDiastolic.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcInvasiveDiastolic.Keyword.EntryUid = keyword.Guid;
                    tcInvasiveDiastolic.Keyword.Comment = keyword.Comment;
                    tcInvasiveDiastolic.Keyword.Level = keyword.ParentCount;
                    tcInvasiveDiastolic.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcInvasiveDiastolic.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        tcInvasiveDiastolic.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcInvasiveDiastolic.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        tcInvasiveDiastolic.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcInvasiveDiastolic.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            tcInvasiveDiastolic.Keyword.NumValue = result;
                            tcInvasiveDiastolic.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Invasive Diastolic value is not a number: {keyword.Value.TextVal}");
                        }
                    }

                    openEhrCaseNote.Entries.Add(tcInvasiveDiastolic);
                    break;

                case "6134": // Blodtryck right arm, systoliskt
                    TcRightArmSystolic tcRightArmSystolic = new TcBase.TcRightArmSystolic($"{commonPrefix}/ickm", v.ToString());
                    tcRightArmSystolic.Uid = keyword.Guid;
                    tcRightArmSystolic.Language = _language;
                    tcRightArmSystolic.Encoding = _encoding;
                    tcRightArmSystolic.Keyword.Value = termData.TermName;
                    tcRightArmSystolic.Keyword.Code = termData.TermId;
                    tcRightArmSystolic.Keyword.Terminology = termData.Terminology;
                    tcRightArmSystolic.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcRightArmSystolic.Keyword.EntryUid = keyword.Guid;
                    tcRightArmSystolic.Keyword.Comment = keyword.Comment;
                    tcRightArmSystolic.Keyword.Level = keyword.ParentCount;
                    tcRightArmSystolic.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcRightArmSystolic.Systolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        tcRightArmSystolic.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcRightArmSystolic.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        tcRightArmSystolic.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcRightArmSystolic.Systolic = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            tcRightArmSystolic.Keyword.NumValue = result;
                            tcRightArmSystolic.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Right Arm Systolic value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcRightArmSystolic);
                    break;

                case "6135": // Blodtryck right arm, diastoliskt
                    TcRightArmDiastolic tcRightArmDiastolic = new TcBase.TcRightArmDiastolic($"{commonPrefix}/ickm", v.ToString());
                    tcRightArmDiastolic.Uid = keyword.Guid;
                    tcRightArmDiastolic.Language = _language;
                    tcRightArmDiastolic.Encoding = _encoding;
                    tcRightArmDiastolic.Keyword.Value = termData.TermName;
                    tcRightArmDiastolic.Keyword.Code = termData.TermId;
                    tcRightArmDiastolic.Keyword.Terminology = termData.Terminology;
                    tcRightArmDiastolic.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcRightArmDiastolic.Keyword.EntryUid = keyword.Guid;
                    tcRightArmDiastolic.Keyword.Comment = keyword.Comment;
                    tcRightArmDiastolic.Keyword.Level = keyword.ParentCount;
                    tcRightArmDiastolic.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcRightArmDiastolic.Diastolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        tcRightArmDiastolic.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcRightArmDiastolic.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        tcRightArmDiastolic.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcRightArmDiastolic.Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            tcRightArmDiastolic.Keyword.NumValue = result;
                            tcRightArmDiastolic.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Right Arm Diastolic value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcRightArmDiastolic);
                    break;

                case "6136": // Blodtryck left arm, systoliskt
                    TcLeftArmSystolic tcLeftArmSystolic = new TcBase.TcLeftArmSystolic($"{commonPrefix}/ickm", v.ToString());
                    tcLeftArmSystolic.Uid = keyword.Guid;
                    tcLeftArmSystolic.Language = _language;
                    tcLeftArmSystolic.Encoding = _encoding;
                    tcLeftArmSystolic.Keyword.Value = termData.TermName;
                    tcLeftArmSystolic.Keyword.Code = termData.TermId;
                    tcLeftArmSystolic.Keyword.Terminology = termData.Terminology;
                    tcLeftArmSystolic.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcLeftArmSystolic.Keyword.EntryUid = keyword.Guid;
                    tcLeftArmSystolic.Keyword.Comment = keyword.Comment;
                    tcLeftArmSystolic.Keyword.Level = keyword.ParentCount;
                    tcLeftArmSystolic.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcLeftArmSystolic.Systolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        tcLeftArmSystolic.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcLeftArmSystolic.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        tcLeftArmSystolic.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcLeftArmSystolic.Systolic = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            tcLeftArmSystolic.Keyword.NumValue = result;
                            tcLeftArmSystolic.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Left Arm Systolic value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcLeftArmSystolic);
                    break;

                case "6137": // Blodtryck left arm, diastoliskt
                    TcLeftArmDiastolic tcLeftArmDiastolic = new TcBase.TcLeftArmDiastolic($"{commonPrefix}/ickm", v.ToString());
                    tcLeftArmDiastolic.Uid = keyword.Guid;
                    tcLeftArmDiastolic.Language = _language;
                    tcLeftArmDiastolic.Encoding = _encoding;
                    tcLeftArmDiastolic.Keyword.Value = termData.TermName;
                    tcLeftArmDiastolic.Keyword.Code = termData.TermId;
                    tcLeftArmDiastolic.Keyword.Terminology = termData.Terminology;
                    tcLeftArmDiastolic.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcLeftArmDiastolic.Keyword.EntryUid = keyword.Guid;
                    tcLeftArmDiastolic.Keyword.Comment = keyword.Comment;
                    tcLeftArmDiastolic.Keyword.Level = keyword.ParentCount;
                    tcLeftArmDiastolic.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcLeftArmDiastolic.Diastolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };

                        tcLeftArmDiastolic.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcLeftArmDiastolic.Keyword.NumUnit = (keyword.Value != null
                                                && keyword.Value.NumVal != null
                                                && !string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]";
                        tcLeftArmDiastolic.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcLeftArmDiastolic.Diastolic = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "mm[Hg]"
                            };
                            tcLeftArmDiastolic.Keyword.NumValue = result;
                            tcLeftArmDiastolic.Keyword.NumUnit = "mm[Hg]";
                        }
                        else
                        {
                            throw new Exception($"Left Arm Diastolic value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcLeftArmDiastolic);
                    break;

                case "8980": // 24-timmars blodtryckskurva (24 hourly blood pressure curve)
                    TcBPCurve24Hour tcBPCurve24Hour = new TcBase.TcBPCurve24Hour($"{commonPrefix}/ickm", v.ToString()); 
                    tcBPCurve24Hour.Uid = keyword.Guid;
                    tcBPCurve24Hour.Language = _language;
                    tcBPCurve24Hour.Encoding = _encoding;
                    tcBPCurve24Hour.MathFunctionCode = "146";
                    tcBPCurve24Hour.MathFunctionValue = "mean";
                    tcBPCurve24Hour.MathFunctionTerminology = "openehr";
                    tcBPCurve24Hour.Width = "P2DT10H38M";
                    tcBPCurve24Hour.Keyword.Value = termData.TermName;
                    tcBPCurve24Hour.Keyword.Code = termData.TermId;
                    tcBPCurve24Hour.Keyword.Terminology = termData.Terminology;
                    tcBPCurve24Hour.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcBPCurve24Hour.Keyword.EntryUid = keyword.Guid;
                    tcBPCurve24Hour.Keyword.Comment = keyword.Comment;
                    tcBPCurve24Hour.Keyword.Level = keyword.ParentCount;
                    tcBPCurve24Hour.Keyword.EhrUriValues = keyword.Children;

                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcBPCurve24Hour.Systolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = "mm[Hg]"
                        };
                        tcBPCurve24Hour.Diastolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = "mm[Hg]"
                        };    
                        tcBPCurve24Hour.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? 0;
                        tcBPCurve24Hour.Keyword.NumUnit = "mm[Hg]";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        string[] bpValue = keyword.Value.TextVal.Split('/');
                        tcBPCurve24Hour.Systolic = new SpineBase.Measurement
                        {
                            Magnitude = bpValue[0].GetDecimalNumberValue() ?? 0m,
                            Units = "mm[Hg]"
                        };
                        tcBPCurve24Hour.Diastolic = new SpineBase.Measurement
                        {
                            Magnitude = (bpValue.Length > 1 ? bpValue[1].GetDecimalNumberValue() : null) ?? 0m,
                            Units = "mm[Hg]"
                        };
                        tcBPCurve24Hour.Keyword.TextValue = keyword.Value.TextVal;
                    }
                    openEhrCaseNote.Entries.Add(tcBPCurve24Hour);
                    break;

                case "1965": // Vikt
                    TcWeight tcWeight = new TcBase.TcWeight($"{commonPrefix}/ickm", v.ToString());
                    tcWeight.Uid = keyword.Guid;
                    tcWeight.Language = _language;
                    tcWeight.Encoding = _encoding;
                    tcWeight.Keyword.Value = termData.TermName;
                    tcWeight.Keyword.Code = termData.TermId;
                    tcWeight.Keyword.Terminology = termData.Terminology;
                    tcWeight.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcWeight.Keyword.EntryUid = keyword.Guid;
                    tcWeight.Keyword.Comment = keyword.Comment;
                    tcWeight.Keyword.Level = keyword.ParentCount;
                    tcWeight.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcWeight.WeightValue = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg"
                        };

                        tcWeight.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcWeight.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg";
                        tcWeight.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcWeight.WeightValue = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "kg"
                            };
                            tcWeight.Keyword.NumValue = result;
                            tcWeight.Keyword.NumUnit = "kg";
                        }
                        else
                        {
                            throw new Exception($"Weight value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcWeight);
                    break;

                case "2896": // Födelsevikt (Birth Weight)
                    TcBirthWeight tcBirthWeight = new TcBase.TcBirthWeight($"{commonPrefix}/ickm", v.ToString());
                    tcBirthWeight.Uid = keyword.Guid;
                    tcBirthWeight.Language = _language;
                    tcBirthWeight.Encoding = _encoding;
                    tcBirthWeight.Keyword.Value = termData.TermName;
                    tcBirthWeight.Keyword.Code = termData.TermId;
                    tcBirthWeight.Keyword.Terminology = termData.Terminology;
                    tcBirthWeight.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcBirthWeight.Keyword.EntryUid = keyword.Guid;
                    tcBirthWeight.Keyword.Comment = keyword.Comment;
                    tcBirthWeight.Keyword.Level = keyword.ParentCount;
                    tcBirthWeight.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcBirthWeight.WeightValue = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg"
                        };

                        tcBirthWeight.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcBirthWeight.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg";
                        tcBirthWeight.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcBirthWeight.WeightValue = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "kg"
                            };
                            tcBirthWeight.Keyword.NumValue = result;
                            tcBirthWeight.Keyword.NumUnit = "kg";
                        }
                        else
                        {
                            throw new Exception($"Birth Weight value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcBirthWeight);

                    break;
                case "5028": // Nakenvikt (Bare weight)
                    TcBareWeight tcBareWeight   = new TcBase.TcBareWeight($"{commonPrefix}/ickm", v.ToString());
                    tcBareWeight.Uid = keyword.Guid;
                    tcBareWeight.Language = _language;
                    tcBareWeight.Encoding = _encoding;
                    tcBareWeight.Keyword.Value = termData.TermName;
                    tcBareWeight.Keyword.Code = termData.TermId;
                    tcBareWeight.Keyword.Terminology = termData.Terminology;
                    tcBareWeight.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcBareWeight.Keyword.EntryUid = keyword.Guid;
                    tcBareWeight.Keyword.Comment = keyword.Comment;
                    tcBareWeight.Keyword.Level = keyword.ParentCount;
                    tcBareWeight.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcBareWeight.WeightValue = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg"
                        };

                        tcBareWeight.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcBareWeight.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg";
                        tcBareWeight.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcBareWeight.WeightValue = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "kg"
                            };
                            tcBareWeight.Keyword.NumValue = result;
                            tcBareWeight.Keyword.NumUnit = "kg";
                        }
                        else
                        {
                            throw new Exception($"Bare Weight value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcBareWeight);
                    break;

                case "1964": // Längd
                    TcLength tcLength = new TcBase.TcLength($"{commonPrefix}/ickm", v.ToString());
                    tcLength.Uid = keyword.Guid;
                    tcLength.Language = _language;
                    tcLength.Encoding = _encoding;
                    tcLength.Keyword.Value = termData.TermName;
                    tcLength.Keyword.Code = termData.TermId;
                    tcLength.Keyword.Terminology = termData.Terminology;
                    tcLength.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcLength.Keyword.EntryUid = keyword.Guid;
                    tcLength.Keyword.Comment = keyword.Comment;
                    tcLength.Keyword.Level = keyword.ParentCount;
                    tcLength.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcLength.LengthValue = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm"
                        };

                        tcLength.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcLength.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm";
                        tcLength.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcLength.LengthValue = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "cm"
                            };
                            tcLength.Keyword.NumValue = result;
                            tcLength.Keyword.NumUnit = "cm";
                        }
                        else
                        {
                            throw new Exception($"Length value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcLength);
                    break;

                case "6179": // Längd liggande (Height Lying down)
                    TcHorizontalLength tcHorizontalLength = new TcBase.TcHorizontalLength($"{commonPrefix}/ickm", v.ToString());
                    tcHorizontalLength.Uid = keyword.Guid;
                    tcHorizontalLength.Language = _language;
                    tcHorizontalLength.Encoding = _encoding;
                    tcHorizontalLength.BodyPosition = new TcLengthTerminology()
                    {
                        Code = "at0020",
                        Value = "Liggande",
                        Terminology = "local"
                    };
                    tcHorizontalLength.Keyword.Value = termData.TermName;
                    tcHorizontalLength.Keyword.Code = termData.TermId;
                    tcHorizontalLength.Keyword.Terminology = termData.Terminology;
                    tcHorizontalLength.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcHorizontalLength.Keyword.EntryUid = keyword.Guid;
                    tcHorizontalLength.Keyword.Comment = keyword.Comment;
                    tcHorizontalLength.Keyword.Level = keyword.ParentCount;
                    tcHorizontalLength.Keyword.EhrUriValues = keyword.Children;

                    if (keyword.Value != null && keyword.Value.NumVal != null) 
                    { 
                        tcHorizontalLength.LengthValue = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm"
                        };
                        tcHorizontalLength.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcHorizontalLength.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm";
                        tcHorizontalLength.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if (keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if (decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcHorizontalLength.LengthValue = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "cm"
                            };
                            tcHorizontalLength.Keyword.NumValue = result;
                            tcHorizontalLength.Keyword.NumUnit = "cm";
                        }
                        else
                        {
                            throw new Exception($"Horizontal Length value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcHorizontalLength);
                    break;

                case "6180": // Längd sittande (Height sitting)
                    TcSittingLength tcSittingLength = new TcBase.TcSittingLength($"{commonPrefix}/ickm", v.ToString());
                    tcSittingLength.Uid = keyword.Guid;
                    tcSittingLength.Language = _language;
                    tcSittingLength.Encoding = _encoding;
                    tcSittingLength.BodyPosition = new TcLengthTerminology()
                    {
                        Code = "at0037",
                        Value = "Sittställning",
                        Terminology = "local"
                    };
                    tcSittingLength.BodySegmentName = new TcLengthTerminology()
                    {
                        Code = "at0017",
                        Value = "Sitthöjd",
                        Terminology = "local"
                    };
                    tcSittingLength.Method = "Mätt sittandes från sittknöl till huvudknopp";
                    tcSittingLength.Keyword.Value = termData.TermName;
                    tcSittingLength.Keyword.Code = termData.TermId;
                    tcSittingLength.Keyword.Terminology = termData.Terminology;
                    tcSittingLength.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcSittingLength.Keyword.EntryUid = keyword.Guid;
                    tcSittingLength.Keyword.Comment = keyword.Comment;
                    tcSittingLength.Keyword.Level = keyword.ParentCount;
                    tcSittingLength.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcSittingLength.LengthValue = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm"
                        };
                        tcSittingLength.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcSittingLength.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "cm";
                        tcSittingLength.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if (keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if (decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcSittingLength.LengthValue = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "cm"
                            };
                            tcSittingLength.Keyword.NumValue = result;
                            tcSittingLength.Keyword.NumUnit = "cm";
                        }
                        else
                        {
                            throw new Exception($"Sitting Length value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcSittingLength);
                    break;

                case "2275":

                    TcBase.TcBMICalculated tcBMIcalc = new TcBase.TcBMICalculated($"{commonPrefix}/ickm", v.ToString());
                    tcBMIcalc.Uid = keyword.Guid;
                    tcBMIcalc.Language = _language;
                    tcBMIcalc.Encoding = _encoding;
                    tcBMIcalc.Keyword.Value = termData.TermName;
                    tcBMIcalc.Keyword.Code = termData.TermId;
                    tcBMIcalc.Keyword.Terminology = termData.Terminology;
                    tcBMIcalc.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcBMIcalc.Keyword.EntryUid = keyword.Guid;
                    tcBMIcalc.Keyword.Comment = keyword.Comment;
                    tcBMIcalc.Keyword.Level = keyword.ParentCount;
                    tcBMIcalc.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcBMIcalc.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg/m2"
                        };
                        tcBMIcalc.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcBMIcalc.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg/m2";
                        tcBMIcalc.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if (keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if (decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcBMIcalc.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "kg/m2"
                            };
                            tcBMIcalc.Keyword.NumValue = result;
                            tcBMIcalc.Keyword.NumUnit = "kg/m2";
                        }
                        else
                        {
                            throw new Exception($"BMI value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcBMIcalc);
                    break;

                case "8883": // BMI
                    TcBase.TcBMI tcBMI = new TcBase.TcBMI($"{commonPrefix}/ickm", v.ToString());
                    tcBMI.Uid = keyword.Guid;
                    tcBMI.Language = _language;
                    tcBMI.Encoding = _encoding;
                    tcBMI.Keyword.Value = termData.TermName;
                    tcBMI.Keyword.Code = termData.TermId;
                    tcBMI.Keyword.Terminology = termData.Terminology;
                    tcBMI.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcBMI.Keyword.EntryUid = keyword.Guid;
                    tcBMI.Keyword.Comment = keyword.Comment;
                    tcBMI.Keyword.Level = keyword.ParentCount;
                    tcBMI.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcBMI.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg/m2"
                        };
                        tcBMI.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcBMI.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "kg/m2";
                        tcBMI.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcBMI.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "kg/m2"
                            };
                            tcBMI.Keyword.NumValue = result;
                            tcBMI.Keyword.NumUnit = "kg/m2";
                        }
                        else
                        {
                            throw new Exception($"BMI value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcBMI);
                    break;

                case "2025": // Kroppstemperatur (temperature)
                    TcBase.TcBodyTemparature tcBodyTemparature  = new TcBase.TcBodyTemparature($"{commonPrefix}/ickm", v.ToString());
                    tcBodyTemparature.Uid = keyword.Guid;
                    tcBodyTemparature.Language = _language;
                    tcBodyTemparature.Encoding = _encoding;
                    tcBodyTemparature.Keyword.Value = termData.TermName;
                    tcBodyTemparature.Keyword.Code = termData.TermId;
                    tcBodyTemparature.Keyword.Terminology = termData.Terminology;
                    tcBodyTemparature.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcBodyTemparature.Keyword.EntryUid = keyword.Guid;
                    tcBodyTemparature.Keyword.Comment = keyword.Comment;
                    tcBodyTemparature.Keyword.Level = keyword.ParentCount;
                    tcBodyTemparature.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcBodyTemparature.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "Cel"
                        };
                        tcBodyTemparature.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcBodyTemparature.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "Cel";
                        tcBodyTemparature.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcBodyTemparature.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "Cel"
                            };
                            tcBodyTemparature.Keyword.NumValue = result;
                            tcBodyTemparature.Keyword.NumUnit = "Cel";
                        }
                        else
                        {
                            throw new Exception($"Body Temperature value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcBodyTemparature);
                    break;

                case "11140": // Hjärtfrekvens (heart rate)
                    TcBase.TcHeartRate tcHeartRate = new TcBase.TcHeartRate($"{commonPrefix}/ickm", v.ToString());
                    tcHeartRate.Uid = keyword.Guid;
                    tcHeartRate.Language = _language;
                    tcHeartRate.Encoding = _encoding;
                    tcHeartRate.Keyword.Value = termData.TermName;
                    tcHeartRate.Keyword.Code = termData.TermId;
                    tcHeartRate.Keyword.Terminology = termData.Terminology;
                    tcHeartRate.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcHeartRate.Keyword.EntryUid = keyword.Guid;
                    tcHeartRate.Keyword.Comment = keyword.Comment;
                    tcHeartRate.Keyword.Level = keyword.ParentCount;
                    tcHeartRate.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcHeartRate.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min"
                        };
                        tcHeartRate.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcHeartRate.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min";
                        tcHeartRate.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcHeartRate.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "/min"
                            };
                            tcHeartRate.Keyword.NumValue = result;
                            tcHeartRate.Keyword.NumUnit = "/min";
                        }
                        else
                        {
                            throw new Exception($"Heart Rate value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcHeartRate);
                    break;

                case "1978": // Pulsfrekvens (Pulse rate)
                    TcBase.TcPulseRate tcPulseRate = new TcBase.TcPulseRate($"{commonPrefix}/ickm", v.ToString());
                    tcPulseRate.Uid = keyword.Guid;
                    tcPulseRate.Language = _language;
                    tcPulseRate.Encoding = _encoding;
                    tcPulseRate.Keyword.Value = termData.TermName;
                    tcPulseRate.Keyword.Code = termData.TermId;
                    tcPulseRate.Keyword.Terminology = termData.Terminology;
                    tcPulseRate.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcPulseRate.Keyword.EntryUid = keyword.Guid;
                    tcPulseRate.Keyword.Comment = keyword.Comment;
                    tcPulseRate.Keyword.Level = keyword.ParentCount;
                    tcPulseRate.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcPulseRate.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min"
                        };
                        tcPulseRate.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcPulseRate.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min";
                        tcPulseRate.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcPulseRate.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "/min"
                            };
                            tcPulseRate.Keyword.NumValue = result;
                            tcPulseRate.Keyword.NumUnit = "/min";
                        }
                        else
                        {
                            throw new Exception($"Pulse Rate value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcPulseRate);
                    break;

                case "402": // Andningsfrekvens (respiratory rate)
                    TcRespiratoryRate tcRespiratoryRate = new TcBase.TcRespiratoryRate($"{commonPrefix}/ickm", v.ToString());
                    tcRespiratoryRate.Uid = keyword.Guid;
                    tcRespiratoryRate.Language = _language;
                    tcRespiratoryRate.Encoding = _encoding;
                    tcRespiratoryRate.Keyword.Value = termData.TermName;
                    tcRespiratoryRate.Keyword.Code = termData.TermId;
                    tcRespiratoryRate.Keyword.Terminology = termData.Terminology;
                    tcRespiratoryRate.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcRespiratoryRate.Keyword.EntryUid = keyword.Guid;
                    tcRespiratoryRate.Keyword.Comment = keyword.Comment;
                    tcRespiratoryRate.Keyword.Level = keyword.ParentCount;
                    tcRespiratoryRate.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcRespiratoryRate.Measurement = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Units = !string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit) ?
                                            _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min"
                        };
                        tcRespiratoryRate.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcRespiratoryRate.Keyword.NumUnit = (!string.IsNullOrEmpty(keyword.Value.NumVal.Unit)) ?
                                                _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "/min";
                        tcRespiratoryRate.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcRespiratoryRate.Measurement = new SpineBase.Measurement
                            {
                                Magnitude = result,
                                Units = "/min"
                            };
                            tcRespiratoryRate.Keyword.NumValue = result;
                            tcRespiratoryRate.Keyword.NumUnit = "/min";
                        }
                        else
                        {
                            throw new Exception($"Respiratory Rate value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcRespiratoryRate);
                    break;
                case "1995": // Saturation
                    TcBase.TcSaturation tcSaturation = new TcBase.TcSaturation($"{commonPrefix}/ickm", v.ToString());
                    tcSaturation.Uid = keyword.Guid;
                    tcSaturation.Language = _language;
                    tcSaturation.Encoding = _encoding;
                    tcSaturation.Keyword.Value = termData.TermName;
                    tcSaturation.Keyword.Code = termData.TermId;
                    tcSaturation.Keyword.Terminology = termData.Terminology;
                    tcSaturation.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcSaturation.Keyword.EntryUid = keyword.Guid;
                    tcSaturation.Keyword.Comment = keyword.Comment;
                    tcSaturation.Keyword.Level = keyword.ParentCount;
                    tcSaturation.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcSaturation.Measurement = new SpineBase.MeasurementFraction
                        {
                            Numerator = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Denominator = 100,
                            Type = 2,
                        };
                        tcSaturation.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcSaturation.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcSaturation.Measurement = new SpineBase.MeasurementFraction
                            {
                                Numerator = result,
                                Denominator = 100,
                                Type = 2,
                            };
                            tcSaturation.Keyword.NumValue = result;
                        }
                        else
                        {
                            throw new Exception($"Saturation value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcSaturation);
                    break;

                case "5251": // Syrgasnivå, % (Oxygen level)
                    TcBase.TcOxygenLevel tcOxygenLevel = new TcBase.TcOxygenLevel($"{commonPrefix}/ickm", v.ToString());
                    tcOxygenLevel.Uid = keyword.Guid;
                    tcOxygenLevel.Language = _language;
                    tcOxygenLevel.Encoding = _encoding;
                    tcOxygenLevel.Keyword.Value = termData.TermName;
                    tcOxygenLevel.Keyword.Code = termData.TermId;
                    tcOxygenLevel.Keyword.Terminology = termData.Terminology;
                    tcOxygenLevel.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcOxygenLevel.Keyword.EntryUid = keyword.Guid;
                    tcOxygenLevel.Keyword.Comment = keyword.Comment;
                    tcOxygenLevel.Keyword.Level = keyword.ParentCount;
                    tcOxygenLevel.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcOxygenLevel.Measurement = new SpineBase.MeasurementFraction
                        {
                            Numerator = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Denominator = 100,
                            Type = 2,
                        };
                        tcOxygenLevel.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcOxygenLevel.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcOxygenLevel.Measurement = new SpineBase.MeasurementFraction
                            {
                                Numerator = result,
                                Denominator = 100,
                                Type = 2,
                            };
                            tcOxygenLevel.Keyword.NumValue = result;
                        }
                        else
                        {
                            throw new Exception($"Oxygen Level value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcOxygenLevel);
                    break;

                case "10827": // Saturation med syrgas (Saturation with oxygen)
                    TcBase.TcOxygenSaturation tcOxygenSaturation = new TcBase.TcOxygenSaturation($"{commonPrefix}/ickm", v.ToString());
                    tcOxygenSaturation.Uid = keyword.Guid;
                    tcOxygenSaturation.Language = _language;
                    tcOxygenSaturation.Encoding = _encoding;
                    tcOxygenSaturation.Keyword.Value = termData.TermName;
                    tcOxygenSaturation.Keyword.Code = termData.TermId;
                    tcOxygenSaturation.Keyword.Terminology = termData.Terminology;
                    tcOxygenSaturation.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcOxygenSaturation.Keyword.EntryUid = keyword.Guid;
                    tcOxygenSaturation.Keyword.Comment = keyword.Comment;
                    tcOxygenSaturation.Keyword.Level = keyword.ParentCount;
                    tcOxygenSaturation.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcOxygenSaturation.Measurement = new SpineBase.MeasurementFraction
                        {
                            Numerator = keyword.Value.NumVal.GetDecimalValue() ?? 0,
                            Denominator = 100,
                            Type = 2,
                        };
                        tcOxygenSaturation.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcOxygenSaturation.Keyword.OriginalUnit = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? keyword.Value.NumVal.Unit : "";
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcOxygenSaturation.Measurement = new SpineBase.MeasurementFraction
                            {
                                Numerator = result,
                                Denominator = 100,
                                Type = 2,
                            };
                            tcOxygenSaturation.Keyword.NumValue = result;
                        }
                        else
                        {
                            throw new Exception($"Oxygen Saturation value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcOxygenSaturation);
                    break;
                
                case "11273": // NEWS2, totalpoäng (Total Score)
                    TcNewsTotalScore tcNewsTotalScore = new TcBase.TcNewsTotalScore($"{commonPrefix}/ickm", v.ToString());
                    tcNewsTotalScore.Uid = keyword.Guid;
                    tcNewsTotalScore.Language = _language;
                    tcNewsTotalScore.Encoding = _encoding;
                    tcNewsTotalScore.Keyword.Value = termData.TermName;
                    tcNewsTotalScore.Keyword.Code = termData.TermId;
                    tcNewsTotalScore.Keyword.Terminology = termData.Terminology;
                    tcNewsTotalScore.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcNewsTotalScore.Keyword.EntryUid = keyword.Guid;
                    tcNewsTotalScore.Keyword.Comment = keyword.Comment;
                    tcNewsTotalScore.Keyword.Level = keyword.ParentCount;
                    tcNewsTotalScore.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcNewsTotalScore.ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0m;
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcNewsTotalScore.ScorePoint = result;
                        }
                        else
                        {
                            throw new Exception($"Total Score value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcNewsTotalScore);
                    break;
                case "11274": // NEWS2, totalpoäng (hjärtfrekvens) (Total score heart rate)
                    TcNewsTotalScoreHeartRate tcNewsTotalScoreHeartRate = new TcBase.TcNewsTotalScoreHeartRate($"{commonPrefix}/ickm", v.ToString());   
                    tcNewsTotalScoreHeartRate.Uid = keyword.Guid;
                    tcNewsTotalScoreHeartRate.Language = _language;
                    tcNewsTotalScoreHeartRate.Encoding = _encoding;
                    tcNewsTotalScoreHeartRate.Keyword.Value = termData.TermName;
                    tcNewsTotalScoreHeartRate.Keyword.Code = termData.TermId;
                    tcNewsTotalScoreHeartRate.Keyword.Terminology = termData.Terminology;
                    tcNewsTotalScoreHeartRate.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcNewsTotalScoreHeartRate.Keyword.EntryUid = keyword.Guid;
                    tcNewsTotalScoreHeartRate.Keyword.Comment = keyword.Comment;
                    tcNewsTotalScoreHeartRate.Keyword.Level = keyword.ParentCount;
                    tcNewsTotalScoreHeartRate.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcNewsTotalScoreHeartRate.ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0m;
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcNewsTotalScoreHeartRate.ScorePoint = result;
                        }
                        else
                        {
                            throw new Exception($"Total Score Heart Rate value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcNewsTotalScoreHeartRate);
                    break;

                case "11275": // NEWS2, totalpoäng (syremättnad2) (Total score saturation)
                    TcNewsTotalScoreO2Sat tcNewsTotalScoreO2Sat = new TcBase.TcNewsTotalScoreO2Sat($"{commonPrefix}/ickm", v.ToString());
                    tcNewsTotalScoreO2Sat.Uid = keyword.Guid;
                    tcNewsTotalScoreO2Sat.Language = _language;
                    tcNewsTotalScoreO2Sat.Encoding = _encoding;
                    tcNewsTotalScoreO2Sat.Keyword.Value = termData.TermName;
                    tcNewsTotalScoreO2Sat.Keyword.Code = termData.TermId;
                    tcNewsTotalScoreO2Sat.Keyword.Terminology = termData.Terminology;
                    tcNewsTotalScoreO2Sat.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcNewsTotalScoreO2Sat.Keyword.EntryUid = keyword.Guid;
                    tcNewsTotalScoreO2Sat.Keyword.Comment = keyword.Comment;
                    tcNewsTotalScoreO2Sat.Keyword.Level = keyword.ParentCount;
                    tcNewsTotalScoreO2Sat.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcNewsTotalScoreO2Sat.ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0m;
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        decimal result;
                        if(decimal.TryParse(keyword.Value.TextVal, NumberStyles.Number, new CultureInfo("sv-SE"), out result))
                        {
                            tcNewsTotalScoreO2Sat.ScorePoint = result;
                        }
                        else
                        {
                            throw new Exception($"Total Score Oxygen Saturation value is not a number: {keyword.Value.TextVal}");
                        }
                    }
                    openEhrCaseNote.Entries.Add(tcNewsTotalScoreO2Sat);
                    break;

                case "11276": // NEWS2, totalpoäng (syremättnad2, hjärtfrekvens) (Total score oxygen saturation, heart rate)
                    TcNewsScoreHRO2Sat tcNewsScoreHRO2Sat = new TcBase.TcNewsScoreHRO2Sat($"{commonPrefix}/ickm", v.ToString());
                    tcNewsScoreHRO2Sat.Uid = keyword.Guid;
                    tcNewsScoreHRO2Sat.Language = _language;
                    tcNewsScoreHRO2Sat.Encoding = _encoding;
                    tcNewsScoreHRO2Sat.Keyword.Value = termData.TermName;
                    tcNewsScoreHRO2Sat.Keyword.Code = termData.TermId;
                    tcNewsScoreHRO2Sat.Keyword.Terminology = termData.Terminology;                        
                    tcNewsScoreHRO2Sat.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcNewsScoreHRO2Sat.Keyword.EntryUid = keyword.Guid;
                    tcNewsScoreHRO2Sat.Keyword.Comment = keyword.Comment;
                    tcNewsScoreHRO2Sat.Keyword.Level = keyword.ParentCount;
                    tcNewsScoreHRO2Sat.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcNewsScoreHRO2Sat.ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0m;
                        tcNewsScoreHRO2Sat.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                    }
                    else if(keyword.Value != null && !string.IsNullOrWhiteSpace(keyword.Value.TextVal))
                    {
                        tcNewsScoreHRO2Sat.ScorePoint = keyword.Value.NumVal.GetDecimalValue() ?? 0m;
                        tcNewsScoreHRO2Sat.Keyword.TextValue = keyword.Value.TextVal;                        
                    }
                    openEhrCaseNote.Entries.Add(tcNewsScoreHRO2Sat);
                    break;
                
                case "1849": // Blood pressure
                    TcBloodPressure tcBloodPressure = new TcBase.TcBloodPressure($"{commonPrefix}/ickm", v.ToString());
                    tcBloodPressure.Uid = keyword.Guid;
                    tcBloodPressure.Language = _language;
                    tcBloodPressure.Encoding = _encoding;
                    tcBloodPressure.Keyword.Value = termData.TermName;
                    tcBloodPressure.Keyword.Code = termData.TermId;
                    tcBloodPressure.Keyword.Terminology = termData.Terminology;
                    tcBloodPressure.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcBloodPressure.Keyword.EntryUid = keyword.Guid;
                    tcBloodPressure.Keyword.Comment = keyword.Comment;
                    tcBloodPressure.Keyword.Level = keyword.ParentCount;
                    tcBloodPressure.Keyword.EhrUriValues = keyword.Children;
                    if(keyword.Value!=null && keyword.Value.NumVal != null)
                    {
                        tcBloodPressure.Systolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0m,
                            Units = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ? 
                                    _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };
                        tcBloodPressure.Diastolic = new SpineBase.Measurement
                        {
                            Magnitude = keyword.Value.NumVal.GetDecimalValue() ?? 0m,
                            Units = (!string.IsNullOrWhiteSpace(keyword.Value.NumVal.Unit)) ?
                                    _unitProvider.GetOpenEhrUnit(keyword.Value.NumVal.Unit) : "mm[Hg]"
                        };
                        tcBloodPressure.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                        tcBloodPressure.Keyword.NumUnit = "mm[Hg]";
                    }
                    else if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal))
                    {
                        string[] bpValue = keyword.Value.TextVal.Split('/');
                        tcBloodPressure.Systolic = new SpineBase.Measurement
                        {
                            Magnitude = bpValue[0].GetDecimalNumberValue() ?? 0m,
                            Units = "mm[Hg]"
                        };
                        tcBloodPressure.Diastolic = new SpineBase.Measurement
                        {
                            Magnitude = ((bpValue.Length>1 ) ? bpValue[1].GetDecimalNumberValue() : null) ?? 0m,
                            Units = "mm[Hg]"
                        };
                        tcBloodPressure.Keyword.TextValue = keyword.Value.TextVal;
                    }
                    openEhrCaseNote.Entries.Add(tcBloodPressure);   
                    break;

                case "6531": // diagnos_enl_icd-1 (Problem Diagnosis)
                    TcProblemDiagnosis tcProblemDiagnosis = new TcBase.TcProblemDiagnosis($"{commonPrefix}/ickm", v.ToString());
                    tcProblemDiagnosis.Uid = keyword.Guid;
                    tcProblemDiagnosis.Language = _language;
                    tcProblemDiagnosis.Encoding = _encoding;
                    tcProblemDiagnosis.Keyword.Value = termData.TermName;
                    tcProblemDiagnosis.Keyword.Code = termData.TermId;
                    tcProblemDiagnosis.Keyword.Terminology = termData.Terminology;
                    tcProblemDiagnosis.Keyword.Datatype = _terminologyProvider.GetTerminology(keyword.TermId).Datatype;
                    tcProblemDiagnosis.Keyword.EntryUid = keyword.Guid;
                    tcProblemDiagnosis.Keyword.Comment = keyword.Comment;
                    tcProblemDiagnosis.Keyword.Level = keyword.ParentCount;
                    tcProblemDiagnosis.Keyword.EhrUriValues = keyword.Children;
                    if (keyword.Value != null && !string.IsNullOrEmpty(keyword.Value.TextVal))
                    {
                        tcProblemDiagnosis.ProblemName = keyword.Value.TextVal;
                        tcProblemDiagnosis.Keyword.TextValue = keyword.Value.TextVal;
                    }
                    else if(keyword.Value != null && keyword.Value.NumVal != null)
                    {
                        tcProblemDiagnosis.ProblemName = keyword.Value.NumVal.Val;
                        tcProblemDiagnosis.Keyword.NumValue = keyword.Value.NumVal.GetDecimalValue() ?? null;
                    }
                    openEhrCaseNote.Entries.Add(tcProblemDiagnosis);
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
