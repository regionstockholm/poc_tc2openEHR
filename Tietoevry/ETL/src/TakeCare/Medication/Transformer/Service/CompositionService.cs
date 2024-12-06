using AutoMapper;
using Microsoft.Extensions.Options;
using TakeCare.Migration.OpenEhr.Application.Models;
using TakeCare.Migration.OpenEhr.Application.Services;
using TakeCare.Migration.OpenEhr.Archetype.Entry;
using TakeCare.Migration.OpenEhr.Medication.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Service
{
    public class CompositionService : ICompositionService
    {
        private string _prefix = "läkemedelsförskrivning";
        private readonly IPatientService _patientService;
        private readonly ITerminologyProvider _terminologyProvider;
        private readonly IContextProvider _contextProvider;
        private readonly IUnitProvider _unitProvider;
        private readonly IMedicationService _medicationService;
        private List<OpenEhrMedication> transformedData;
        private readonly IOptions<MedicationConfig> _options;
        private IMapper _mapper;

        public CompositionService(IPatientService patientService, ITerminologyProvider terminologyProvider, IContextProvider contextProvider,
            IUnitProvider unitProvider, IMedicationService medicationService, IOptions<MedicationConfig> options, IMapper mapper)
        {
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
            _contextProvider = contextProvider;
            _unitProvider = unitProvider;
            _medicationService = medicationService;
            _options = options;
            _mapper = mapper;
        }
        public List<OpenEhrMedication> TransformMeasurementInputToEhr(MedicationDTO? medicationDto)
        {
            transformedData = new List<OpenEhrMedication>();
            var savedAtCU = new ContextDetails();
            var createdAtCU = new ContextDetails();
            var openEhrMedication = new OpenEhrMedication();

            switch (_options.Value.Language.Current)
            {
                case "en":
                    _prefix = _options.Value.Template.Prefix.En;
                    break;
                case "sv":
                    _prefix = _options.Value.Template.Prefix.Sv;
                    break;
                default:
                    _prefix = _options.Value.Template.Prefix.Sv;
                    break;
            }

            foreach (var medication in medicationDto.Medications)
            {
                if (medication.IsMixture.Equals("0") && medication.Drugs.Count == 1)
                {
                    savedAtCU = _contextProvider.GetContextData(medication.SavedAtCareUnitID);
                    createdAtCU = _contextProvider.GetContextData(medication.CreatedAtCareUnitID);
                    openEhrMedication = new OpenEhrMedication()
                    {
                        PatientID = _patientService.GetPatient(medicationDto.PatientId).PatientId,
                        TemplateId = _options.Value.Template.TemplateId,
                        Namespace = _options.Value.Ehr.Namespace,
                        Format = _options.Value.Ehr.Format,
                        LifecycleState = _options.Value.Ehr.LifecycleState,
                        AuditChangeType = _options.Value.Ehr.AuditChangeType
                    };

                    openEhrMedication.ContextInformation = new TcContextInformation(_prefix)
                    {
                        Composer = new ComposerIdentifier()
                        {
                            Name = medication.SignedByUserID,
                            Id = medication.SignedByUserID,
                            Type = "UserId",
                            Issuer = "RSK"
                        },
                        HealthCareFacility = new HealthCareFacilityIdentifier()
                        {
                            Name = createdAtCU.CareUnitName,
                            Id = createdAtCU.CareUnitId,
                            Type = "CareUnitId",
                            Issuer = "RSK"
                        },
                        Setting = new Setting()
                        {
                            Code = "238",
                            Value = "other care",
                            Terminology = "openehr"
                        },
                        StartTime = medication.TimestampCreated,
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

                    var careUnit = new TcCareUnitContext(_prefix);
                    careUnit.CareUnitName = savedAtCU.CareUnitName;
                    careUnit.CareUnitId = savedAtCU.CareUnitId;
                    careUnit.CareProviderId = savedAtCU.CareProviderName;
                    openEhrMedication.CareUnitContext = careUnit;

                    var metadata = new TcMedicationContext();
                    metadata.CreatedAtCareUnitName = createdAtCU.CareUnitName;
                    metadata.CreatedAtCareUnitId = createdAtCU.CareUnitId;
                    metadata.SavedByUserId = medication.SavedByUserID;
                    metadata.SavedOn = medication.TimestampSaved;
                    metadata.SavedAtCareUnitId = savedAtCU.CareUnitId;
                    metadata.SavedAtCareUnitName = savedAtCU.CareUnitName;
                    metadata.CreatedOn = medication.TimestampCreated;
                    metadata.DocumentId = medication.DocumentID;
                    metadata.ParentDocId = medication.ParentDocumentID;
                    metadata.SignedByUserId = medication.SignedByUserID;
                    metadata.SignerUserID = medication.SignerUserID;
                    metadata.SignedDateTime = medication.SignedDatetime;
                    metadata.PrescriptionGuid = medication.Prescription.Guid;
                    metadata.PrescriptionDocumentIDs = medication.PrescriptionDocumentIDs;
                    metadata.ApprovedForPatient = medication.ApprovedForPatient;
                    openEhrMedication.MedicationContext = metadata;

                    var medicationOrder = new TcMedicationOrder();
                    _mapper.Map(medication, medicationOrder);
                    openEhrMedication.Medication = medicationOrder;

                    var equivalenceDetails = _medicationService.GetEquivalenceDetails(medication.Prescription.IsReplaceable);
                    var prescription = new TcPrescription();
                    _mapper.Map(medication.Prescription, prescription);
                    openEhrMedication.Prescription = prescription;

                    var drugSingleDto = medication.Drugs.FirstOrDefault();
                    TcDrug drug = new TcDrug();
                    _mapper.Map(drugSingleDto, drug);
                    openEhrMedication.Drugs.Add(drug);

                    var recDosageDto = medication.Dosage.OrderByDescending(x => x.FullStartDate).FirstOrDefault();
                    var dosageDrug = recDosageDto?.DosageDrugs.Where(x => x.DrugCode.Equals(drug.DrugCode)).FirstOrDefault();
                    
                    TcDosage recentDosage = new TcDosage();
                    _mapper.Map(recDosageDto, recentDosage);
                    _mapper.Map(recDosageDto?.DosageDrugs.Where(x => x.DrugCode.Equals(drug.DrugCode)), recentDosage.DosageDrugs);
                    openEhrMedication.Dosages.Add(recentDosage);

                    var dayDto = medication.Days.OrderByDescending(x => x.TimestampSaved).FirstOrDefault();
                    TcDay recentDay = new TcDay();
                    var mappedDay = _mapper.Map(dayDto, recentDay);
                    openEhrMedication.Days.Add(recentDay);

                    transformedData.Add(openEhrMedication);
                }
            }
            Console.WriteLine("Medication transformed successfully");
            return transformedData;
        }

    }
}
