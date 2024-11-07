using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using System.Text;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;
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

        public MedicationTransformer(IPatientService patientService, ITerminologyProvider terminologyProvider, IContextProvider contextProvider,
            IUnitProvider unitProvider, IMedicationService medicationService)
        {
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
            _contextProvider = contextProvider;
            _unitProvider = unitProvider;
            _medicationService = medicationService;
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
                savedAtCU = _contextProvider.GetContextData(medication.SavedAtCareUnitID);
                createdAtCU = _contextProvider.GetContextData(medication.CreatedAtCareUnitID);
                openEhrMedication = new OpenEhrMedication()
                {
                    PatientID = _patientService.GetPatient(medicationDto.PatientId).PatientId
                };
                
                var context = new Context();
                context.IdNamespace = "HOSPITAL-NS";
                context.IdScheme = "HOSPITAL-NS";
                context.ParticipationFunction = "requester";
                context.ParticipationMode = "face-to-face communication";
                context.HealthCareFacilityName = createdAtCU.CareUnitName; //need to confirm if need to use savedat or createdby careunit
                context.HealthCareFacilityId = createdAtCU.CareUnitId;
                context.ComposerName = medication.SignedByUserID;
                context.ParticipationId = medication.SavedByUserID;
                context.ParticipationName = medication.SavedByUserID;
                openEhrMedication.Context = context;

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
                    StartTime = medication.TimestampCreated
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
                medicationOrder.ExternalStartDate = medication.ExternalStartDate;
                medicationOrder.ExternalPrescriber = medication.ExternalPrescriber;
                medicationOrder.ChangeReasonID = medication.ChangeReasonID;
                medicationOrder.ChangeReasonText = medication.ChangeReasonText;
                medicationOrder.HasOrdinationReason  = medication.HasOrdinationReason;
                medicationOrder.IsTriggeredByATC = medication.IsTriggeredByATC;
                medicationOrder.ProfylaxID = medication.ProfylaxID;
                medicationOrder.PrescriptionDocumentIDs = medication.PrescriptionDocumentIDs;
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
                prescription.IsDispensionAllowed = medication.Prescription.IsDispensionAllowed;
                prescription.SolutionStrength = medication.Prescription.SolutionStrength;
                prescription.SolutionStrengthUnitID = medication.Prescription.SolutionStrengthUnitID;
                prescription.SolutionStrengthUnitText = medication.Prescription.SolutionStrengthUnitText;
                openEhrMedication.Prescription = prescription;

                TcDrug drug;
                foreach (var item in medication.Drugs)
                {
                    drug = new TcDrug();
                    drug.DrugID = item.DrugID;
                    drug.DrugCode = item.DrugCode;
                    drug.DoseForm = item.DoseForm;
                    drug.DoseFormCode = item.DoseFormCode;
                    drug.ATCCode = item.ATCCode;
                    drug.ATCName = item.ATCName;
                    drug.Strength = item.Strength;
                    drug.StrengthUnit = item.StrengthUnit;
                    drug.InternalArticleStrength = item.InternalArticleStrength;
                    drug.UnitCode = item.UnitCode;
                    drug.DosageUnitID = item.DosageUnitID;
                    drug.DosageUnitText = item.DosageUnitText;
                    drug.StdSolutionAmount = item.StdSolutionAmount;
                    drug.ProductType = item.ProductType;
                    drug.IsApproved = item.IsApproved;
                    drug.PreparationText = item.PreparationText;
                    drug.Row = item.Row;
                    drug.SpecialDrugCode = item.SpecialDrugCode;
                    drug.SpecialityID = item.SpecialityID;
                    drug.AdministrationRouteID = medication.Prescription.AdministrationRouteID;
                    drug.AdministrationRouteText = medication.Prescription.AdministrationRouteText;
                    drug.AdministrationTypeID = medication.Prescription.AdministrationRouteID;
                    drug.AdministrationTypeText = medication.Prescription.AdministrationRouteText;
                    openEhrMedication.Drugs.Add(drug);
                }

                Console.WriteLine("Medication transformed successfully");

                Console.WriteLine(openEhrMedication.ToString());

                transformedData.Add(openEhrMedication);
            }
            return transformedData;
        }
    }
}
