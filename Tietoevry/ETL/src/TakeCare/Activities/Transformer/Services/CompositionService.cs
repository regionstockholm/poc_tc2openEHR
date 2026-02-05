using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeCare.Migration.OpenEhr.Application.Models;
using TakeCare.Migration.OpenEhr.Application.Services;
using TakeCare.Migration.OpenEhr.Activities.Extraction.Models;
using TakeCare.Migration.OpenEhr.Activities.Transformer.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TakeCare.Migration.OpenEhr.Activities.Transformer.Services
{
    internal class CompositionService : ICompositionService
    {
        private readonly IContextProvider _contextProvider;
        private readonly IPatientService _patientService;
        private readonly IOptions<ActivitiesConfig> _options;

        public ActivityOpenEhrData activityOpenEhrData { get; set; }

        public CompositionService(IContextProvider contextProvider,
                                  IPatientService patientService,
                                  IOptions<ActivitiesConfig> options)
        {
            _contextProvider = contextProvider;
            _patientService = patientService;
            _options = options;
        }

        public ActivityOpenEhrData Compose(TakeCareActivities inputData)
        {
            activityOpenEhrData = new ActivityOpenEhrData();

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

            foreach( var data in inputData.Activities)
            {
                OpenEhrActivity openEhrActivityData = new OpenEhrActivity();

                openEhrActivityData.TemplateId = _options.Value.Template.TemplateId;
                openEhrActivityData.Namespace = _options.Value.Ehr.Namespace;
                openEhrActivityData.Format = _options.Value.Ehr.Format;
                openEhrActivityData.LifecycleState = _options.Value.Ehr.LifecycleState;
                openEhrActivityData.AuditChangeType = _options.Value.Ehr.AuditChangeType;

                var patientDetails = _patientService.GetPatient(inputData.PatientId);
                if(patientDetails==null)
                {
                    throw new Exception("Patient not found; Incorrect PatientId in the filename : "+ inputData.PatientId);
                }
                activityOpenEhrData.PatientId = patientDetails.PatientId;

                //context care unit data
                ContextDetails contextDetails = new ContextDetails();
                if (data.Created != null && data.Created.CareUnit != null)
                {
                    contextDetails = _contextProvider.GetContextData(data.Created.CareUnit.Id.ToString());
                }
                if (contextDetails == null)
                {
                    throw new Exception($"Invalid Created CareUnit Id: " +
                        $"{(data.Created != null && data.Created.CareUnit != null ? data.Created.CareUnit.Id : null)}");
                }




                #region ComposerContext
                openEhrActivityData.ComposerContext = new TcComposerContext($"{commonPrefix}");
                
                #region Health care Facility
                openEhrActivityData.ComposerContext.HealthCareFacility = new TakeCare.Migration.OpenEhr.Archetype.Entry.HealthCareFacilityIdentifier()
                {

                    Name = contextDetails.HealthCareFacilityName,
                    Id =  contextDetails.HealthCareFacilityId,
                    Type = CompositionConstants.CARE_UNIT_HSA_ID_OID_MARKER,
                    Issuer = "RSK",
                    Scheme = CompositionConstants.SCHEMA_ID,
                    Namespace = CompositionConstants.NAMESPACE_ID
                };
                #endregion


                if (data.Created!=null && data.Created.User != null)
                {


                    openEhrActivityData.ComposerContext.UserFullName = data.Created.User.FullName;
                    openEhrActivityData.ComposerContext.User = new Identifier()
                    {
                        Issuer = "TC",
                        Type = "username",
                        Value = data.Created.User.Id
                    };
                    openEhrActivityData.ComposerContext.StartTime = data.Created.DateTime??"";
                    openEhrActivityData.ComposerContext.Setting = new CodedText()
                    {
                        Terminology = "openehr",
                        Code = "238",
                        Value = "other care"
                    };
                }
                openEhrActivityData.ComposerContext.Category = new CodedText()
                {
                    Code = "433",
                    Value = "event",
                    Terminology = "openehr",
                };
                openEhrActivityData.ComposerContext.Language = new CodedText()
                {
                    Code = "sv",
                    Terminology = "ISO_639-1"
                };
                openEhrActivityData.ComposerContext.Territory = new CodedText()
                {
                    Code = "SV",
                    Terminology = "ISO_3166-1"
                };
                #endregion

                #region ContextCareUnit

                openEhrActivityData.CareUnitContext = new TcCareUnitContext($"{commonPrefix}/context/vårdenhet")
                {
                    CareUnitName = contextDetails != null ? contextDetails.CareUnitName : "To Be decided", //verify
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
                    CareProviderTerminology = "http://snomed.info/sct/45991000052106",
                    OrgNumber = new Identifier()
                    {
                        Value = contextDetails != null ? contextDetails.OrganisationNumber : "Add CareProviderId", //verify,
                        Type = CompositionConstants.CARE_PROVIDER_TYPE
                    }
                };
                #endregion

                #region ContextMetadata

                openEhrActivityData.ContextMetadata = new TcActivityMetadata($"{commonPrefix}/context/metadata");
                openEhrActivityData.ContextMetadata.DocumentId = data.Id;
                if (data.Created != null) 
                {
                    openEhrActivityData.ContextMetadata.DocCreatedByUser = new ActivityUser();
                    if (data.Created.User != null)
                    {
                        openEhrActivityData.ContextMetadata.DocCreatedByUser.UserId = new Identifier()
                        {
                            Value = data.Created.User.Id
                        };
                        openEhrActivityData.ContextMetadata.DocCreatedByUser.FullName = data.Created.User.FullName;
                        openEhrActivityData.ContextMetadata.DocCreatedByUser.Username = data.Created.User.Username;
                    }
                    openEhrActivityData.ContextMetadata.DocCreatedByUser.ActivityTimestamp = data.Created.DateTime ?? "";
                    openEhrActivityData.ContextMetadata.DocCreatedByUser.ActivityCode = new CodedText()
                    {
                        Code = "at0010",
                        Terminology = "local",
                        Value = "Skapat"
                    };
                    if (data.Created.CareUnit != null)
                    {
                        openEhrActivityData.ContextMetadata.DocCreatedByUser.CareUnitId = new Identifier()
                        {
                            Value = data.Created.CareUnit.Id.ToString(),
                        };
                        openEhrActivityData.ContextMetadata.DocCreatedByUser.CareUnitName = data.Created.CareUnit.Name;
                    }
                }
                if (data.LastSaved != null)
                {
                    openEhrActivityData.ContextMetadata.DocSavedByUser = new ActivityUser();
                    if (data.LastSaved.User != null)
                    {
                        openEhrActivityData.ContextMetadata.DocSavedByUser.UserId = new Identifier()
                        {
                            Value = data.LastSaved.User.Id
                        };
                        openEhrActivityData.ContextMetadata.DocSavedByUser.FullName = data.LastSaved.User.FullName;
                        openEhrActivityData.ContextMetadata.DocSavedByUser.Username = data.LastSaved.User.Username;
                    }
                    openEhrActivityData.ContextMetadata.DocSavedByUser.ActivityTimestamp = data.LastSaved.DateTime ?? "";
                    openEhrActivityData.ContextMetadata.DocSavedByUser.ActivityCode = new CodedText()
                    {
                        Code = "at0011",
                        Terminology = "local",
                        Value = "Sparad"
                    };
                    if (data.LastSaved.CareUnit != null)
                    {
                        openEhrActivityData.ContextMetadata.DocSavedByUser.CareUnitId = new Identifier()
                        {
                            Value = data.LastSaved.CareUnit.Id.ToString(),
                        };
                        openEhrActivityData.ContextMetadata.DocSavedByUser.CareUnitName = data.LastSaved.CareUnit.Name;
                    }
                }
                if (data.Locked != null)
                {
                    openEhrActivityData.ContextMetadata.LockInfo = new ActivityUser();
                    if (data.Locked.User != null)
                    {
                        openEhrActivityData.ContextMetadata.LockInfo.UserId = new Identifier()
                        {
                            Value = data.Locked.User.Id
                        };
                        openEhrActivityData.ContextMetadata.LockInfo.FullName = data.Locked.User.FullName;
                        openEhrActivityData.ContextMetadata.LockInfo.Username = data.Locked.User.Username;
                    }
                    openEhrActivityData.ContextMetadata.LockInfo.ActivityTimestamp = data.Locked.DateTime?? "";
                    openEhrActivityData.ContextMetadata.LockInfo.ActivityCode = new CodedText()
                    {
                        Code = "at0017",
                        Terminology = "local",
                        Value = "Låst"
                    };
                }
                if (data.Signed != null)
                {
                    openEhrActivityData.ContextMetadata.Signed = new ActivityUser();
                    if (data.Signed.User != null)
                    {
                        openEhrActivityData.ContextMetadata.Signed.UserId = new Identifier()
                        {
                            Value = data.Signed.User.Id
                        };
                        openEhrActivityData.ContextMetadata.Signed.FullName = data.Signed.User.FullName;
                        openEhrActivityData.ContextMetadata.Signed.Username = data.Signed.User.Username;
                        openEhrActivityData.ContextMetadata.Signed.ActivityTimestamp = data.Signed.DateTime ?? "";
                    }
                    openEhrActivityData.ContextMetadata.Signed.ActivityCode = new CodedText()
                    {
                        Code = "at0012",
                        Terminology = "local",
                        Value = "Signerad"
                    };
                }
                if (data.SignerUser!=null)
                {
                    openEhrActivityData.ContextMetadata.Signer = new ActivityUser();
                    openEhrActivityData.ContextMetadata.Signer.UserId = new Identifier()
                    {
                        Value = data.SignerUser.Id
                    };
                    openEhrActivityData.ContextMetadata.Signer.FullName = data.SignerUser.FullName;
                    openEhrActivityData.ContextMetadata.Signer.Username = data.SignerUser.Username;

                    openEhrActivityData.ContextMetadata.Signer.ActivityCode = new CodedText()
                    {
                        Code = "at0013",
                        Terminology = "local",
                        Value = "Signerat av"
                    };
                }
                #endregion

                #region ServiceRequest
                openEhrActivityData.ServiceRequest = new TcServiceRequest($"{commonPrefix}");
                openEhrActivityData.ServiceRequest.Uid = data.Guid.ToString();
                openEhrActivityData.ServiceRequest.Narrative = "Human readable instruction narrative";
                openEhrActivityData.ServiceRequest.CurrentActivity.Timing = "R3/2024-11-08T16:00:00Z/P3M";
                openEhrActivityData.ServiceRequest.CurrentActivity.TimingFormalism = "timing";

                if (data.Term != null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.ServiceName = new CodedText()
                    {
                        Code = data.Term.Id.ToString(),
                        Value = data.Term.Name,
                        Terminology = "TC-Catalog"
                    };
                }

                if (data.Profession != null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.ServiceType = new CodedText()
                    {
                        Code = data.Profession.Code,
                        Value = data.Profession.PluralName,
                        Terminology = "TC-ProfessionCode"
                    };
                }

                if (!string.IsNullOrWhiteSpace(data.ExplanationAndInstruction))
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Description = data.ExplanationAndInstruction;
                }

                if(data.Priority!=null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Urgency = new CodedText()
                    {
                        Code = data.Priority.Id.ToString(),
                        Value = data.Priority.Name,
                        Terminology = "TC-Priority"
                    };
                }

                if (data.Frequency!=null && data.Frequency.Content != null)
                {
                    if (data.Frequency.Type != null)
                    {
                        if (data.Frequency.Type.Id == 0)
                        {
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDue = new ServiceDue()
                            {
                                DateTimeValue = data.Frequency.Content.PlannedDateTime ?? ""
                            };
                        }
                        else if (data.Frequency.Type.Id == 1)
                        {
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDue = new ServiceDue()
                            {
                                Lower = data.Frequency.Content.StartDateTime ?? "",
                                Upper = data.Frequency.Content.EndDateTime ?? "",
                                LowerUnbounded = "false",
                                UpperUnbounded = "false",
                            };
                            if (data.Frequency.Content.Times != null && data.Frequency.Content.ConvertedTimes != null)
                            {
                                var serviceActivities = new Models.ServiceActivity();
                                foreach (var item in data.Frequency.Content.ConvertedTimes)
                                {
                                    serviceActivities.SpecificTimes.Add(item.FormattedTime ?? "");
                                }
                                openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDirection.Activities.Add(serviceActivities);
                            }
                        }
                        else if (data.Frequency.Type.Id == 2)
                        {
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDue = new ServiceDue()
                            {
                                Lower = data.Frequency.Content.StartDateTime ?? "",
                                Upper = data.Frequency.Content.EndDateTime ?? "",
                                LowerUnbounded = "false",
                                UpperUnbounded = "false",
                            };
                            if (data.Frequency.Content.Times != null && data.Frequency.Content.ConvertedTimes != null)
                            {
                                var serviceActivities = new Models.ServiceActivity();
                                foreach (var item in data.Frequency.Content.ConvertedTimes)
                                {
                                    serviceActivities.SpecificTimes.Add(item.FormattedTime ?? "");
                                }
                                openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDirection.Activities.Add(serviceActivities);
                                openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDirection.PeriodicActivities = new PeriodicActivity()
                                {
                                    RecurringInterval = $"P{data.Frequency.Content.EveryNDays}D" 
                                };
                            }
                        }
                        else if (data.Frequency.Type.Id == 3)
                        {
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDue = new ServiceDue()
                            {
                                Lower = data.Frequency.Content.StartDateTime ?? "",
                                Upper = data.Frequency.Content.EndDateTime ?? "",
                                LowerUnbounded = "false",
                                UpperUnbounded = "false",
                            };
                            if (data.Frequency.Content.Times != null && data.Frequency.Content.ConvertedTimes != null)
                            {
                                var serviceActivities = new Models.ServiceActivity();
                                foreach (var item in data.Frequency.Content.ConvertedTimes)
                                {
                                    serviceActivities.SpecificTimes.Add(item.FormattedTime ?? "");
                                }
                                openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDirection.Activities.Add(serviceActivities);
                                openEhrActivityData.ServiceRequest.CurrentActivity.ServiceDirection.PeriodicActivities = new PeriodicActivity()
                                {
                                    RecurringInterval = $"P{data.Frequency.Content.EveryNDays}W"
                                };
                            }
                        }
                        else if(data.Frequency.Type.Id == 4)
                        {
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServicePeriodStart = data.Frequency.Content.StartDateTime ?? "";
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServicePeriodExpiry = data.Frequency.Content.EndDateTime ?? "";
                        }
                        else if(data.Frequency.Type.Id == 5)
                        {
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServicePeriodStart = data.Frequency.Content.StartDateTime ?? "";
                            openEhrActivityData.ServiceRequest.CurrentActivity.ServicePeriodExpiry = data.Frequency.Content.EndDateTime ?? "";
                            if (data.Frequency.Content.IrregularDateTimes != null)
                            {
                                foreach(var item in data.Frequency.Content.IrregularDateTimes)
                                {
                                    openEhrActivityData.ServiceRequest.CurrentActivity.Extension.IrregularDateTimes.Add(item);
                                }
                            }
                        }
                    }
                }

                if (data.BasedOnActivityId != null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Extension.BasedOnActivityId = data.BasedOnActivityId;
                }
                if (data.Profession != null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Extension.Profession = new Models.Profession()
                    {
                        Id = data.Profession.Id,
                        ShortName = data.Profession.ShortName,
                        SingularName = data.Profession.SingularName
                    };
                }
                if(data.CompletedSetBySystemDateTime!=null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Extension.DateTimeCompletedSetBySystem = data.CompletedSetBySystemDateTime;
                }
                if (data.CompletedSetByUserDateTime != null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Extension.DateTimeCompletedSetByUser = data.CompletedSetByUserDateTime;
                }
                if (data.LinkedDocument != null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Extension.LinkedDocumentId = new Identifier()
                    {
                        Value = data.LinkedDocument.Id.ToString()
                    };
                    if (data.LinkedDocument.Type != null)
                    {
                        openEhrActivityData.ServiceRequest.CurrentActivity.Extension.LinkedDocumentTypeId = new Identifier()
                        {
                            Value = data.LinkedDocument.Type.Id.ToString()
                        };
                        openEhrActivityData.ServiceRequest.CurrentActivity.Extension.LinkedDocumentTypeName = data.LinkedDocument.Type.Name;
                    }
                }
                if (data.Priority != null)
                {
                    openEhrActivityData.ServiceRequest.CurrentActivity.Extension.PriorityValue = data.Priority.Value;
                }
                openEhrActivityData.ServiceRequest.CurrentActivity.Extension.Status = new CodedText()
                {
                    Code = data.Status.ToString(),
                    Terminology = "TC-Activity-Status"
                };
                if (data.PlannedAt != null)
                {
                    foreach(var item in data.PlannedAt)
                    {
                        openEhrActivityData.ServiceRequest.CurrentActivity.Extension.PlannedAt.Add(item.DateTime??"");
                    }
                }
                
                #endregion

                activityOpenEhrData.ActivityData.Add(openEhrActivityData);
            }

            return activityOpenEhrData;
        }
    }
}
