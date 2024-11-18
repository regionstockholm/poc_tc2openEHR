using Microsoft.Extensions.Options;
using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using System.Text;
using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Services;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;
using TakeCare.Foundation.OpenEhr.Models.Model;
using TakeCare.Migration.OpenEhr.Medication.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Service;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer
{
    public class MedicationTransformer : IMedicationTransformer
    {
        //private const string _contextPrefix = "prescription_en/context";
        //private const string _orderPrefix = "prescription_en/läkemedelsbeställning";

        private readonly IPatientService _patientService;
        private readonly ITerminologyProvider _terminologyProvider;
        private readonly IContextProvider _contextProvider;
        private readonly IUnitProvider _unitProvider;
        private readonly IMedicationService _medicationService;
        Dictionary<string, int> counterMap = new Dictionary<string, int>();
        private string _txtFilePath = string.Empty;
        private List<OpenEhrMedication> transformedData;
        private StringBuilder _finalCompositionJson;
        private Terminology Language = new Terminology() { Code = "sv", Value = "ISO_639-1" };
        private Terminology Encoding = new Terminology() { Code = "UTF-8", Value = "IANA_character-sets" };
        private readonly IOptions<MedicationConfig> _options;


        public MedicationTransformer(IPatientService patientService, ITerminologyProvider terminologyProvider, IContextProvider contextProvider,
            IUnitProvider unitProvider, IMedicationService medicationService, IOptions<MedicationConfig> options)
        {
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
            _contextProvider = contextProvider;
            _unitProvider = unitProvider;
            _medicationService = medicationService;
            _options = options;
        }
        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            var result = new List<OpenEhrMedication>();
            ArgumentNullException.ThrowIfNull(input);
            counterMap = new Dictionary<string, int>();
            result = TransformMeasurementInputToEhr(input.Result as MedicationDTO);

            return Task.FromResult<TResult>(result as TResult);
        }

        private List<OpenEhrMedication> TransformMeasurementInputToEhr(MedicationDTO? medicationDto)
        {
            transformedData = new List<OpenEhrMedication>();
            var savedAtCU = new ContextDetails();
            var createdAtCU = new ContextDetails();
            var openEhrMedication = new OpenEhrMedication();

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


                    openEhrMedication.ContextInformation = new TcContextInformation("läkemedelsförskrivning")
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

                    var careUnit = new TcCareUnitContext("läkemedelsförskrivning");
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
                    medicationOrder.DatabaseID = medication.DatabaseID;
                    medicationOrder.IsMixture = medication.IsMixture;
                    medicationOrder.DosageType = medication.DosageType;
                    medicationOrder.ExternalStartDate = medication.ExternalISOStartDate;
                    medicationOrder.ExternalPrescriber = medication.ExternalPrescriber;
                    medicationOrder.ChangeReasonID = medication.ChangeReasonID;
                    medicationOrder.ChangeReasonText = medication.ChangeReasonText;
                    medicationOrder.HasOrdinationReason = medication.HasOrdinationReason;
                    medicationOrder.IsTriggeredByATC = medication.IsTriggeredByATC;
                    medicationOrder.ProfylaxID = medication.ProfylaxID;
                    medicationOrder.PrescriptionDocumentIDs = medication.PrescriptionDocumentIDs;
                    medicationOrder.RegistrationStatus = medication.RegistrationStatus;
                    openEhrMedication.Medication = medicationOrder;

                    var equivalenceDetails = _medicationService.GetEquivalenceDetails(medication.Prescription.IsReplaceable);

                    var prescription = new TcPrescription();
                    prescription.Guid = medication.Prescription.Guid;
                    prescription.ParentGuid = medication.Prescription.ParentGuid;
                    prescription.TimestampSaved = medication.Prescription.TimestampSaved;
                    prescription.SavedByUserID = medication.Prescription.SavedByUserID;
                    prescription.SavedAtCareUnitID = medication.Prescription.SavedAtCareUnitID;
                    prescription.TreatmentReason = medication.Prescription.TreatmentReason;
                    prescription.TreatmentGoal = medication.Prescription.TreatmentGoal;
                    prescription.Instruction = medication.Prescription.Instruction;
                    prescription.ReviewDecisionByUserID = medication.Prescription.ReviewDecisionByUserID;
                    prescription.IsReplaceable = medication.Prescription.IsReplaceable;
                    prescription.IsReplacebleCode = equivalenceDetails.Code;
                    prescription.IsReplacebleValue = equivalenceDetails.Display;
                    prescription.IsReplacebleEquivalence = equivalenceDetails.Equivalence;
                    prescription.DilutionLiquid = medication.Prescription.DilutionLiquid;
                    prescription.DilutionAmount = medication.Prescription.DilutionAmount;
                    prescription.CessationReasonID = medication.Prescription.CessationReasonID;
                    prescription.CessationReasonText = medication.Prescription.CessationReasonText;
                    prescription.IsStdSolution = medication.Prescription.IsStdSolution;
                    prescription.FullReviewDate = medication.Prescription.FullReviewDate;
                    prescription.FullFirstDoseDate = medication.Prescription.FullFirstDoseDate;
                    prescription.FullLastDoseDate = medication.Prescription.FullLastDoseDate;
                    prescription.AdministrationOccasionID = medication.Prescription.AdministrationOccasionID;
                    prescription.AdministrationOccasionText = medication.Prescription.AdministrationOccasionText;
                    prescription.AdministrationRouteID = medication.Prescription.AdministrationRouteID;
                    prescription.AdministrationRouteText = medication.Prescription.AdministrationRouteText;
                    prescription.AdministrationTypeID = medication.Prescription.AdministrationTypeID;
                    prescription.AdministrationTypeText = medication.Prescription.AdministrationTypeText;
                    prescription.IsDispensionAllowed = medication.Prescription.IsDispensionAllowed;
                    prescription.SolutionStrength = medication.Prescription.SolutionStrength;
                    prescription.SolutionStrengthUnitID = medication.Prescription.SolutionStrengthUnitID;
                    prescription.SolutionStrengthUnitText = medication.Prescription.SolutionStrengthUnitText;
                    openEhrMedication.Prescription = prescription;

                    var drugSingleDto = medication.Drugs.FirstOrDefault();

                    TcDrug drug = new TcDrug();
                    drug.DrugID = drugSingleDto.DrugID;
                    drug.DrugCode = drugSingleDto.DrugCode;
                    drug.DoseForm = drugSingleDto.DoseForm;
                    drug.DoseFormCode = drugSingleDto.DoseFormCode;
                    drug.ATCCode = drugSingleDto.ATCCode;
                    drug.ATCName = drugSingleDto.ATCName;
                    drug.Strength = drugSingleDto.Strength;
                    drug.StrengthUnit = drugSingleDto.StrengthUnit;
                    drug.InternalArticleStrength = drugSingleDto.InternalArticleStrength;
                    drug.UnitCode = drugSingleDto.UnitCode;
                    drug.DosageUnitID = drugSingleDto.DosageUnitID;
                    drug.DosageUnitText = drugSingleDto.DosageUnitText;
                    drug.StdSolutionAmount = drugSingleDto.StdSolutionAmount;
                    drug.ProductType = drugSingleDto.ProductType;
                    drug.IsApproved = drugSingleDto.IsApproved;
                    drug.PreparationText = drugSingleDto.PreparationText;
                    drug.Row = drugSingleDto.Row;
                    drug.SpecialDrugCode = drugSingleDto.SpecialDrugCode;
                    drug.SpecialityID = drugSingleDto.SpecialityID;
                    drug.AdministrationRouteID = medication.Prescription.AdministrationRouteID;
                    drug.AdministrationRouteText = medication.Prescription.AdministrationRouteText;
                    drug.AdministrationTypeID = medication.Prescription.AdministrationRouteID;
                    drug.AdministrationTypeText = medication.Prescription.AdministrationRouteText;
                    openEhrMedication.Drugs.Add(drug);

                    var recDosageDto = medication.Dosage.OrderByDescending(x => x.FullStartDate).FirstOrDefault();
                    var dosageDrug = recDosageDto?.DosageDrugs.Where(x => x.DrugCode.Equals(drug.DrugCode)).FirstOrDefault();
                    TcDosage recentDosage = new TcDosage();
                    recentDosage.DosageID = recDosageDto.DosageID;
                    recentDosage.TimestampSaved = recDosageDto.TimestampSaved;
                    recentDosage.SavedByUserID = recDosageDto.SavedByUserID;
                    recentDosage.SavedAtCareUnitID = recDosageDto.SavedAtCareUnitID;
                    recentDosage.StartDate = recDosageDto.StartDate;
                    recentDosage.StartTime = recDosageDto.StartTime;
                    recentDosage.StartFullDateTime = recDosageDto.FullStartDate;
                    recentDosage.ScheduleType = recDosageDto.ScheduleType;
                    recentDosage.Period = recDosageDto.Period;
                    recentDosage.IsGivenOnMondays = recDosageDto.IsGivenOnMondays;
                    recentDosage.IsGivenOnTuesdays = recDosageDto.IsGivenOnTuesdays;
                    recentDosage.IsGivenOnWednesdays = recDosageDto.IsGivenOnWednesdays;
                    recentDosage.IsGivenOnThursdays = recDosageDto.IsGivenOnThursdays;
                    recentDosage.IsGivenOnFridays = recDosageDto.IsGivenOnFridays;
                    recentDosage.IsGivenOnSaturdays = recDosageDto.IsGivenOnSaturdays;
                    recentDosage.IsGivenOnSundays = recDosageDto.IsGivenOnSundays;
                    recentDosage.DosageDrugs = new List<DosageDrug>()
                    {
                    new DosageDrug(){
                                    DoseNumerical = dosageDrug.DoseNumerical,
                                    DoseText = dosageDrug.DoseText,
                                    DrugCode = dosageDrug.DrugCode,
                                    DrugRow = dosageDrug.DrugRow
                                    }
                    };

                    openEhrMedication.Dosages.Add(recentDosage);

                    var dayDto = medication.Days.OrderByDescending(x => x.TimestampSaved).FirstOrDefault();
                    TcDay recentDay = new TcDay();
                    recentDay.TimestampSaved = dayDto.TimestampSaved;
                    recentDay.SavedByUserID = dayDto.SavedByUserID;
                    recentDay.SavedAtCareUnitID = dayDto.SavedAtCareUnitID;
                    recentDay.AdministrationFullStartDateTime = dayDto.AdministrationFullStartDateTime;
                    recentDay.InfusionTime = dayDto.InfusionTime;
                    recentDay.MaxDailyDose = dayDto.MaxDailyDose;
                    recentDay.IsSelfAdministered = dayDto.IsSelfAdministered;
                    recentDay.DosageInstruction = dayDto.DosageInstruction;
                    recentDay.DosageInstructionTemplate = dayDto.DosageInstructionTemplate;
                    openEhrMedication.Days.Add(recentDay);

                    transformedData.Add(openEhrMedication);
                }
            }
            Console.WriteLine("Medication transformed successfully");
            return transformedData;
        }
    }
}
