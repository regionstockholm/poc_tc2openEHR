using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using System.Text;
using TakeCare.Foundation.OpenEhr.Application.Models;
using TakeCare.Foundation.OpenEhr.Application.Services;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;
using TakeCare.Migration.OpenEhr.Measurement.Transformer.Model;
using TakeCare.Migration.OpenEhr.Measurement.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Measurement.Extraction.Model;

namespace TakeCare.Migration.OpenEhr.Measurement.Transformer
{
    public class MeasurementTransformer : IMeasurementTransformer
    {
        private string _prefix = "vårdkontakt/ickm";
        private readonly IPatientService _patientService;
        private readonly ITerminologyProvider _terminologyProvider;
        private readonly IContextProvider _contextProvider;
        private readonly IUnitProvider _unitProvider;
        private readonly IOptions<MeasurementConfig> _options;
        Dictionary<string, int> counterMap = new Dictionary<string, int>();
        private string _txtFilePath = string.Empty;
        private OpenEhrMeasurement transformedData;
        private StringBuilder _finalCompositionJson;
        private Terminology Language = new Terminology() { Code = "sv", Value = "ISO_639-1" };
        private Terminology Encoding = new Terminology() { Code = "UTF-8", Value = "IANA_character-sets" };

        JObject composedObject;
        private string _createdByUserId;
        private string _createdByUserName;
        private string _createdOn;
        private string _savedByUserId;
        private string _savedByUserName;
        private string _savedOn;


        public MeasurementTransformer(IPatientService patientService, ITerminologyProvider terminologyProvider, IContextProvider contextProvider, 
            IUnitProvider unitProvider, IOptions<MeasurementConfig> options)
        {
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
            _txtFilePath = Path.Combine(AppContext.BaseDirectory, @"Assets\TemplatesTxts");
            _finalCompositionJson = new StringBuilder();
            _contextProvider = contextProvider;
            _unitProvider = unitProvider;
            _options = options;
        }

        public Task<TResult> Trasform<TInput, TResult>(ExtractionResult<TInput> input)
            where TInput : class
            where TResult : class
        {
            var result = new OpenEhrMeasurement();
            ArgumentNullException.ThrowIfNull(input);
            counterMap = new Dictionary<string, int>();
            result = TransformMeasurementInputToEhr(input.Result as MeasurementDto);

            return Task.FromResult<TResult>(result as TResult);
        }

        private OpenEhrMeasurement TransformMeasurementInputToEhr(MeasurementDto? measurementDto)
        {
            // Transform the input to the desired output
            var careUnitDetails = _contextProvider.GetContextData(measurementDto.CareUnitId);
            CareUnit careU = new CareUnit();
            careU.Id = careUnitDetails?.CareUnitId;
            careU.Name = careUnitDetails?.CareUnitName;
            
            switch (_options.Value.Language.Current)
            {
                case "en":
                    _prefix = _options.Value.Template.Prefix.En;
                    break;
                case "swe":
                    _prefix = _options.Value.Template.Prefix.Swe;
                    break;
                default:
                    _prefix = _options.Value.Template.Prefix.Swe;
                    break;
            }

            transformedData = new OpenEhrMeasurement()
            {
                PatientID = _patientService.GetPatient(measurementDto.PatientId).PatientId,
                Measurements = new List<BaseEntry>()
            };

            transformedData.TemplateId = _options.Value.Template.TemplateId;
            transformedData.Namespace = _options.Value.Ehr.Namespace;
            transformedData.Format = _options.Value.Ehr.Format;
            transformedData.LifecycleState = _options.Value.Ehr.LifecycleState;
            transformedData.AuditChangeType = _options.Value.Ehr.AuditChangeType;

            _createdByUserId = measurementDto.CreatedBy.User.Id;
            _createdByUserName = measurementDto.CreatedBy.User.FullName;
            _createdOn = measurementDto.CreatedBy.DateTime;

            _savedByUserId = measurementDto.SavedBy.User.Id;
            _savedByUserName = measurementDto.SavedBy.User.FullName;
            _savedOn = measurementDto.SavedBy.DateTime;
            counterMap.Add("generic", 0);


            transformedData.ContextInformation = new TcContextInformation("vårdkontakt")
            {
                Composer = new ComposerIdentifier()
                {
                    Name = _createdByUserName,
                    Id = _createdByUserId,
                    Type = "UserId",
                    Issuer = "RSK"
                },
                HealthCareFacility = new HealthCareFacilityIdentifier()
                {
                    Name = careU.Name,
                    Id = careU.Id,
                    Type = "CareUnitId",
                    Issuer = "RSK"
                },
                Setting = new Setting()
                {
                    Code = "238",
                    Value = "other care",
                    Terminology = "openehr"
                },
                StartTime = measurementDto.EventDateTime,
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

            //add ctx care unit
            var careUnit = new TcCareUnitContext("vårdkontakt");
            careUnit.CareUnitName = careU.Name;
            careUnit.CareUnitId = careU.Id;
            careUnit.CareProviderId = careUnitDetails.CareProviderName;
            transformedData.CareUnitContext = careUnit;
            _finalCompositionJson.Append(careUnit.ToString());

            //add context metadata
            var contextMetadata = new TcContextMetadata()
            {
                SavedByUserId = _savedByUserId,
                SavedOn = _savedOn,
                CreatedByUserId = _createdByUserId,
                SavedByFullName = _savedByUserName,
                CareUnitId = careU.Id,
                CareUnitName = careU.Name,
                DocumentCode = "MÄT",
                DocumentName = "Mätningar",
                CreatedOnDate = _createdOn,
                Heading = "Mätningar",
                LinkCode = measurementDto.LinkCode,
                VersionId = measurementDto.VersionId,
                TemplateId = measurementDto.TemplateId,
                TemplateName = measurementDto.TemplateName,
                DocId = measurementDto.DocId,
            };
            transformedData.ContextMetadata = contextMetadata;
            _finalCompositionJson.Append(contextMetadata.ToString());

            foreach (var measurment in measurementDto.Measurements)
            {
                //add generic/ickms entries 
                var termData = _terminologyProvider.GetTerminology(Convert.ToString(measurment.Term.Id));
                if (termData != null)
                {
                    AddCKMEntry(measurment, termData);
                }
                else
                {
                    AddGenericEntry(measurment);
                }
            }

            return transformedData;
        }

        private void AddGenericEntry(Extraction.Model.Measurement measurment)
        {
            int v = counterMap["generic"];
            TerminologyDetails datatype = _terminologyProvider.GetTerminology(Convert.ToString(measurment.Term.Id));

            //todo - check where to map decimal and scale

            var gEntry = new TcGenericEntry("vårdkontakt", Convert.ToString(v));
            gEntry.Uid = measurment.Guid;
            gEntry.Keyword.Code = Convert.ToString(measurment.Term.Id);
            gEntry.Keyword.Value = measurment.Term.Name;
            gEntry.Keyword.TextValue = measurment.Value.ToString();
            gEntry.Keyword.Comment = measurment.Comment;
            gEntry.Keyword.EntryUid = measurment.Guid;
            gEntry.Keyword.Terminology = "TC-Datatypes";
            gEntry.Keyword.Datatype = (datatype != null) ? datatype.Datatype : "Text";
            gEntry.Keyword.OriginalUnit = measurment.Term.Unit;
            gEntry.Keyword.Level = 0;
            gEntry.Language = Language;
            gEntry.Encoding = Encoding;
            gEntry.Keyword.NumValue = measurment.Value;
            transformedData.Measurements.Add(gEntry);
            _finalCompositionJson.Append(gEntry.ToString());

            counterMap["generic"]++;
        }

        private void AddCKMEntry(Extraction.Model.Measurement measurment, TerminologyDetails terminology)
        {
            object[] values = { };
            var fileName = string.Empty;

            var standardUnit = _unitProvider.GetOpenEhrUnit(measurment.Term.Unit);

            if (!counterMap.ContainsKey(measurment.Term.Id.ToString()))
                counterMap.Add(measurment.Term.Id.ToString(), 0);
            var v = Convert.ToString(counterMap[measurment.Term.Id.ToString()]);
            switch (terminology.TermId)
            {
                case "3719": //Systolic Upper
                    var systolicUpper = new TcSystolicUpper(_prefix, v);
                    systolicUpper.Uid = measurment.Guid;
                    systolicUpper.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    systolicUpper.Keyword.Comment = measurment.Comment;
                    systolicUpper.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    systolicUpper.Keyword.Value = measurment.Term.Name;
                    systolicUpper.Keyword.TextValue = measurment.Value.ToString();
                    systolicUpper.Keyword.Datatype = terminology.Datatype;
                    systolicUpper.Keyword.Terminology = terminology.Terminology;
                    systolicUpper.Keyword.OriginalUnit = measurment.Term.Unit;
                    systolicUpper.Keyword.Level = 0;
                    systolicUpper.Keyword.EntryUid = measurment.Guid;
                    systolicUpper.Language = Language;
                    systolicUpper.Encoding = Encoding;
                    systolicUpper.Keyword.NumValue = measurment.Value;
                    systolicUpper.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(systolicUpper);
                    _finalCompositionJson.Append(systolicUpper.ToString());

                    break;

                case "3720":
                    var diastolicLower = new TcDiastolicLower(_prefix, v);
                    diastolicLower.Uid = measurment.Guid;
                    diastolicLower.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    diastolicLower.Keyword.Comment = measurment.Comment;
                    diastolicLower.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    diastolicLower.Keyword.Value = measurment.Term.Name;
                    diastolicLower.Keyword.TextValue = measurment.Value.ToString();
                    diastolicLower.Keyword.Datatype = terminology.Datatype;
                    diastolicLower.Keyword.Terminology = terminology.Terminology;
                    diastolicLower.Keyword.OriginalUnit = measurment.Term.Unit;
                    diastolicLower.Keyword.Level = 0;
                    diastolicLower.Keyword.EntryUid = measurment.Guid;
                    diastolicLower.Language = Language;
                    diastolicLower.Encoding = Encoding;
                    diastolicLower.Keyword.NumValue = measurment.Value;
                    diastolicLower.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(diastolicLower);
                    _finalCompositionJson.Append(diastolicLower.ToString());
                    break;

                case "4243":
                    var meanArtBP = new TcMeanArterialBP(_prefix, v);
                    meanArtBP.Uid = measurment.Guid;
                    meanArtBP.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    meanArtBP.Keyword.Comment = measurment.Comment;
                    meanArtBP.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    meanArtBP.Keyword.Value = measurment.Term.Name;
                    meanArtBP.Keyword.TextValue = measurment.Value.ToString();
                    meanArtBP.Keyword.Datatype = terminology.Datatype;
                    meanArtBP.Keyword.Terminology = terminology.Terminology;
                    meanArtBP.Keyword.OriginalUnit = measurment.Term.Unit;
                    meanArtBP.Keyword.Level = 0;
                    meanArtBP.Keyword.EntryUid = measurment.Guid;
                    meanArtBP.Language = Language;
                    meanArtBP.Encoding = Encoding;
                    meanArtBP.Keyword.NumValue = measurment.Value;
                    meanArtBP.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(meanArtBP);
                    _finalCompositionJson.Append(meanArtBP.ToString());
                    break;

                case "4378":
                    var meanArtBPSys = new TcInvasiveSystolic("vårdkontakt/ickm", v);
                    meanArtBPSys.Uid = measurment.Guid;
                    meanArtBPSys.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    meanArtBPSys.Keyword.Comment = measurment.Comment;
                    meanArtBPSys.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    meanArtBPSys.Keyword.Value = measurment.Term.Name;
                    meanArtBPSys.Keyword.TextValue = measurment.Value.ToString();
                    meanArtBPSys.Keyword.Datatype = terminology.Datatype;
                    meanArtBPSys.Keyword.Terminology = terminology.Terminology;
                    meanArtBPSys.Keyword.OriginalUnit = measurment.Term.Unit;
                    meanArtBPSys.Keyword.Level = 0;
                    meanArtBPSys.Keyword.EntryUid = measurment.Guid;
                    meanArtBPSys.Language = Language;
                    meanArtBPSys.Encoding = Encoding;
                    meanArtBPSys.Keyword.NumValue = measurment.Value;
                    meanArtBPSys.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(meanArtBPSys);
                    _finalCompositionJson.Append(meanArtBPSys.ToString());
                    break;

                case "4379":
                    var meanArtBPDis = new TcInvasiveDiastolic("vårdkontakt/ickm", v);
                    meanArtBPDis.Uid = measurment.Guid;
                    meanArtBPDis.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    meanArtBPDis.Keyword.Comment = measurment.Comment;
                    meanArtBPDis.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    meanArtBPDis.Keyword.Value = measurment.Term.Name;
                    meanArtBPDis.Keyword.TextValue = measurment.Value.ToString();
                    meanArtBPDis.Keyword.Datatype = terminology.Datatype;
                    meanArtBPDis.Keyword.Terminology = terminology.Terminology;
                    meanArtBPDis.Keyword.OriginalUnit = measurment.Term.Unit;
                    meanArtBPDis.Keyword.Level = 0;
                    meanArtBPDis.Keyword.EntryUid = measurment.Guid;
                    meanArtBPDis.Language = Language;
                    meanArtBPDis.Encoding = Encoding;
                    meanArtBPDis.Keyword.NumValue = measurment.Value;
                    meanArtBPDis.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(meanArtBPDis);
                    _finalCompositionJson.Append(meanArtBPDis.ToString());
                    break;

                case "6134":
                    var rightArmSyst = new TcRightArmSystolic(_prefix, v);
                    rightArmSyst.Uid = measurment.Guid;
                    rightArmSyst.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    rightArmSyst.Keyword.Comment = measurment.Comment;
                    rightArmSyst.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    rightArmSyst.Keyword.Value = measurment.Term.Name;
                    rightArmSyst.Keyword.TextValue = measurment.Value.ToString();
                    rightArmSyst.Keyword.Datatype = terminology.Datatype;
                    rightArmSyst.Keyword.Terminology = terminology.Terminology;
                    rightArmSyst.Keyword.OriginalUnit = measurment.Term.Unit;
                    rightArmSyst.Keyword.Level = 0;
                    rightArmSyst.Keyword.EntryUid = measurment.Guid;
                    rightArmSyst.Language = Language;
                    rightArmSyst.Encoding = Encoding;
                    rightArmSyst.Keyword.NumValue = measurment.Value;
                    rightArmSyst.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(rightArmSyst);
                    _finalCompositionJson.Append(rightArmSyst.ToString());

                    break;

                case "6136":
                    var armLeftSyst = new TcLeftArmSystolic(_prefix, v);
                    armLeftSyst.Uid = measurment.Guid;
                    armLeftSyst.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    armLeftSyst.Keyword.Comment = measurment.Comment;
                    armLeftSyst.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    armLeftSyst.Keyword.Value = measurment.Term.Name;
                    armLeftSyst.Keyword.TextValue = measurment.Value.ToString();
                    armLeftSyst.Keyword.Datatype = terminology.Datatype;
                    armLeftSyst.Keyword.Terminology = terminology.Terminology;
                    armLeftSyst.Keyword.OriginalUnit = measurment.Term.Unit;
                    armLeftSyst.Keyword.Level = 0;
                    armLeftSyst.Keyword.EntryUid = measurment.Guid;
                    armLeftSyst.Language = Language;
                    armLeftSyst.Encoding = Encoding;
                    armLeftSyst.Keyword.NumValue = measurment.Value;
                    armLeftSyst.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(armLeftSyst);
                    _finalCompositionJson.Append(armLeftSyst.ToString());
                    break;

                case "6135":
                    var armRightDiastolic = new TcRightArmDiastolic(_prefix, v);
                    armRightDiastolic.Uid = measurment.Guid;
                    armRightDiastolic.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    armRightDiastolic.Keyword.Comment = measurment.Comment;
                    armRightDiastolic.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    armRightDiastolic.Keyword.Value = measurment.Term.Name;
                    armRightDiastolic.Keyword.TextValue = measurment.Value.ToString();
                    armRightDiastolic.Keyword.Datatype = terminology.Datatype;
                    armRightDiastolic.Keyword.Terminology = terminology.Terminology;
                    armRightDiastolic.Keyword.OriginalUnit = measurment.Term.Unit;
                    armRightDiastolic.Keyword.Level = 0;
                    armRightDiastolic.Keyword.EntryUid = measurment.Guid;
                    armRightDiastolic.Language = Language;
                    armRightDiastolic.Encoding = Encoding;
                    armRightDiastolic.Keyword.NumValue = measurment.Value;
                    armRightDiastolic.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(armRightDiastolic);
                    _finalCompositionJson.Append(armRightDiastolic.ToString());
                    break;

                case "6137":
                    var armdist = new TcLeftArmDiastolic(_prefix, v);
                    armdist.Uid = measurment.Guid;
                    armdist.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    armdist.Keyword.Comment = measurment.Comment;
                    armdist.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    armdist.Keyword.Value = measurment.Term.Name;
                    armdist.Keyword.TextValue = measurment.Value.ToString();
                    armdist.Keyword.Datatype = terminology.Datatype;
                    armdist.Keyword.Terminology = terminology.Terminology;
                    armdist.Keyword.OriginalUnit = measurment.Term.Unit;
                    armdist.Keyword.Level = 0;
                    armdist.Keyword.EntryUid = measurment.Guid;
                    armdist.Language = Language;
                    armdist.Encoding = Encoding;
                    armdist.Keyword.NumValue = measurment.Value;
                    armdist.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(armdist);
                    _finalCompositionJson.Append(armdist.ToString());
                    break;

                case "8980":
                    var bpCurve = new TcBPCurve24Hour(_prefix, v);
                    var measurementVal = measurment.Value.ToString().Split('/');
                    bpCurve.Uid = measurment.Guid;
                    bpCurve.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    bpCurve.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    bpCurve.MathFunctionCode = "146";
                    bpCurve.MathFunctionValue = "mean";
                    bpCurve.MathFunctionTerminology = "openehr";
                    bpCurve.Keyword.Comment = measurment.Comment;
                    bpCurve.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    bpCurve.Keyword.Value = measurment.Term.Name;
                    bpCurve.Keyword.TextValue = measurment.Value.ToString();
                    bpCurve.Keyword.Datatype = terminology.Datatype;
                    bpCurve.Keyword.Terminology = terminology.Terminology;
                    bpCurve.Keyword.OriginalUnit = measurment.Term.Unit;
                    bpCurve.Keyword.Level = 0;
                    bpCurve.Keyword.EntryUid = measurment.Guid;
                    bpCurve.Language = Language;
                    bpCurve.Encoding = Encoding;
                    bpCurve.Keyword.NumValue = measurment.Value;
                    bpCurve.Keyword.NumUnit = "mm[Hg]";
                    transformedData.Measurements.Add(bpCurve);
                    _finalCompositionJson.Append(bpCurve.ToString());
                    break;

                case "1965":
                    TcWeight weight = new TcWeight(_prefix, v);
                    weight.Uid = measurment.Guid;
                    weight.WeightValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "kg"
                    };
                    weight.Keyword.Comment = measurment.Comment;
                    weight.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    weight.Keyword.Value = measurment.Term.Name;
                    weight.Keyword.TextValue = measurment.Value.ToString();
                    weight.Keyword.Datatype = terminology.Datatype;
                    weight.Keyword.Terminology = terminology.Terminology;
                    weight.Keyword.OriginalUnit = measurment.Term.Unit;
                    weight.Keyword.Level = 0;
                    weight.Keyword.EntryUid = measurment.Guid;
                    weight.Time = _createdOn;
                    weight.Language = Language;
                    weight.Encoding = Encoding;
                    weight.Keyword.NumValue = measurment.Value;
                    weight.Keyword.NumUnit = "kg";
                    transformedData.Measurements.Add(weight);
                    _finalCompositionJson.Append(weight.ToString());
                    break;
                case "2896":
                    TcBirthWeight birthWeight = new TcBirthWeight(_prefix, v);
                    birthWeight.Uid = measurment.Guid;
                    birthWeight.WeightValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "kg"
                    };
                    birthWeight.Keyword.Comment = measurment.Comment;
                    birthWeight.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    birthWeight.Keyword.Value = measurment.Term.Name;
                    birthWeight.Keyword.TextValue = measurment.Value.ToString();
                    birthWeight.Keyword.Datatype = terminology.Datatype;
                    birthWeight.Keyword.Terminology = terminology.Terminology;
                    birthWeight.Keyword.OriginalUnit = measurment.Term.Unit;
                    birthWeight.Keyword.Level = 0;
                    birthWeight.Keyword.EntryUid = measurment.Guid;
                    birthWeight.Time = _createdOn;
                    birthWeight.Language = Language;
                    birthWeight.Encoding = Encoding;
                    birthWeight.Keyword.NumValue = measurment.Value;
                    birthWeight.Keyword.NumUnit = "kg";
                    transformedData.Measurements.Add(birthWeight);
                    _finalCompositionJson.Append(birthWeight.ToString());
                    break;

                case "5028":
                    TcBareWeight bweight = new TcBareWeight(_prefix, v);
                    bweight.Uid = measurment.Guid;
                    bweight.WeightValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "kg"
                    };
                    bweight.Keyword.Comment = measurment.Comment;
                    bweight.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    bweight.Keyword.Value = measurment.Term.Name;
                    bweight.Keyword.TextValue = measurment.Value.ToString();
                    bweight.Keyword.Datatype = terminology.Datatype;
                    bweight.Keyword.Terminology = terminology.Terminology;
                    bweight.Keyword.OriginalUnit = measurment.Term.Unit;
                    bweight.Keyword.Level = 0;
                    bweight.Keyword.EntryUid = measurment.Guid;
                    bweight.Time = _createdOn;
                    bweight.Language = Language;
                    bweight.Encoding = Encoding;
                    bweight.Keyword.NumValue = measurment.Value;
                    bweight.Keyword.NumUnit = "kg";
                    transformedData.Measurements.Add(bweight);
                    _finalCompositionJson.Append(bweight.ToString());
                    break;

                case "1964":
                    TcLength length = new TcLength(_prefix, v);
                    length.Uid = measurment.Guid;
                    length.LengthValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "cm"
                    };
                    length.Keyword.Comment = measurment.Comment;
                    length.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    length.Keyword.Value = measurment.Term.Name;
                    length.Keyword.TextValue =  measurment.Value.ToString();
                    length.Keyword.Datatype = terminology.Datatype;
                    length.Keyword.Terminology = terminology.Terminology;
                    length.Keyword.OriginalUnit = measurment.Term.Unit;
                    length.Keyword.Level = 0;
                    length.Keyword.EntryUid = measurment.Guid;
                    length.Time = _createdOn;
                    length.Language = Language;
                    length.Encoding = Encoding;
                    length.Keyword.NumValue = measurment.Value;
                    length.Keyword.NumUnit = length.LengthValue.Units;
                    transformedData.Measurements.Add(length);
                    _finalCompositionJson.Append(length.ToString());
                    break;

                case "6179":
                    TcHorizontalLength hLength = new TcHorizontalLength(_prefix, v);
                    hLength.Uid = measurment.Guid;
                    hLength.LengthValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "cm"
                    };
                    hLength.Keyword.Comment = measurment.Comment;
                    hLength.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    hLength.Keyword.Value = measurment.Term.Name;
                    hLength.Keyword.TextValue = measurment.Value.ToString();
                    hLength.Keyword.Datatype = terminology.Datatype;
                    hLength.Keyword.Terminology = terminology.Terminology;
                    hLength.Keyword.OriginalUnit = measurment.Term.Unit;
                    hLength.Keyword.Level = 0;
                    hLength.Keyword.EntryUid = measurment.Guid;
                    hLength.Time = _createdOn;
                    hLength.Language = Language;
                    hLength.Encoding = Encoding;
                    hLength.Keyword.NumValue = measurment.Value;
                    hLength.Keyword.NumUnit = hLength.LengthValue.Units;
                    hLength.BodyPosition = new TcLengthTerminology() { Code = "at0020", Value = "Liggande", Terminology = "local" };
                    transformedData.Measurements.Add(hLength);
                    _finalCompositionJson.Append(hLength.ToString());
                    break;

                case "6180":
                    TcSittingLength sLength = new TcSittingLength(_prefix, v);
                    sLength.Uid = measurment.Guid;
                    sLength.LengthValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "cm"
                    };
                    sLength.Keyword.Comment = measurment.Comment;
                    sLength.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    sLength.Keyword.Value = measurment.Term.Name;
                    sLength.Keyword.TextValue = measurment.Value.ToString();
                    sLength.Keyword.Datatype = terminology.Datatype;
                    sLength.Keyword.Terminology = terminology.Terminology;
                    sLength.Keyword.OriginalUnit = measurment.Term.Unit;
                    sLength.Keyword.Level = 0;
                    sLength.Keyword.EntryUid = measurment.Guid;
                    sLength.Time = _createdOn;
                    sLength.Language = Language;
                    sLength.Encoding = Encoding;
                    sLength.Keyword.NumValue = measurment.Value;
                    sLength.Keyword.NumUnit = sLength.LengthValue.Units;
                    sLength.BodyPosition = new TcLengthTerminology() { Code = "at0037", Value = "Sittställning", Terminology = "local" };
                    sLength.BodySegmentName = new TcLengthTerminology() { Code = "at0017", Value = "Sitthöjd", Terminology = "local" };
                    transformedData.Measurements.Add(sLength);
                    _finalCompositionJson.Append(sLength.ToString());
                    break;

                case "2275":
                    TcBMICalculated tcBMICalc = new TcBMICalculated(_prefix, v);
                    tcBMICalc.Uid = measurment.Guid;
                    tcBMICalc.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "kg/m2"
                    };
                    tcBMICalc.Keyword.Comment = measurment.Comment;
                    tcBMICalc.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcBMICalc.Keyword.Value = measurment.Term.Name;
                    tcBMICalc.Keyword.TextValue = measurment.Value.ToString();
                    tcBMICalc.Keyword.Datatype = terminology.Datatype;
                    tcBMICalc.Keyword.Terminology = terminology.Terminology;
                    tcBMICalc.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcBMICalc.Keyword.Level = 0;
                    tcBMICalc.Keyword.EntryUid = measurment.Guid;
                    tcBMICalc.Time = _createdOn;
                    tcBMICalc.Keyword.NumValue = measurment.Value;
                    tcBMICalc.Keyword.NumUnit = tcBMICalc.Measurement.Units;
                    tcBMICalc.Language = Language;
                    tcBMICalc.Encoding = Encoding;
                    transformedData.Measurements.Add(tcBMICalc);
                    _finalCompositionJson.Append(tcBMICalc.ToString());
                    break;

                case "8883":
                    TcBMI tcBMI = new TcBMI(_prefix, v);
                    tcBMI.Uid = measurment.Guid;
                    tcBMI.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "kg/m2"
                    };
                    tcBMI.Keyword.Comment = measurment.Comment;
                    tcBMI.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcBMI.Keyword.Value = measurment.Term.Name;
                    tcBMI.Keyword.TextValue = measurment.Value.ToString();
                    tcBMI.Keyword.Datatype = terminology.Datatype;
                    tcBMI.Keyword.Terminology = terminology.Terminology;
                    tcBMI.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcBMI.Keyword.Level = 0;
                    tcBMI.Keyword.EntryUid = measurment.Guid;
                    tcBMI.Time = _createdOn;
                    tcBMI.Keyword.NumValue = measurment.Value;
                    tcBMI.Keyword.NumUnit = tcBMI.Measurement.Units;
                    tcBMI.Language = Language;
                    tcBMI.Encoding = Encoding;
                    transformedData.Measurements.Add(tcBMI);
                    _finalCompositionJson.Append(tcBMI.ToString());
                    break;

                case "2025":
                    TcBodyTemparature tcBodyTemp = new TcBodyTemparature(_prefix, v);
                    tcBodyTemp.Uid = measurment.Guid;
                    tcBodyTemp.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "Cel"
                    };
                    tcBodyTemp.Keyword.Comment = measurment.Comment;
                    tcBodyTemp.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcBodyTemp.Keyword.Value = measurment.Term.Name;
                    tcBodyTemp.Keyword.TextValue = measurment.Value.ToString();
                    tcBodyTemp.Keyword.Datatype = terminology.Datatype;
                    tcBodyTemp.Keyword.Terminology = terminology.Terminology;
                    tcBodyTemp.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcBodyTemp.Keyword.Level = 0;
                    tcBodyTemp.Keyword.EntryUid = measurment.Guid;
                    tcBodyTemp.Time = _createdOn;
                    tcBodyTemp.Language = Language;
                    tcBodyTemp.Encoding = Encoding;
                    tcBodyTemp.Keyword.NumValue = measurment.Value;
                    tcBodyTemp.Keyword.NumUnit = tcBodyTemp.Measurement.Units;
                    transformedData.Measurements.Add(tcBodyTemp);
                    _finalCompositionJson.Append(tcBodyTemp.ToString());
                    break;

                case "11140":
                    TcHeartRate tcHeartRt = new TcHeartRate(_prefix, v);
                    tcHeartRt.Uid = measurment.Guid;
                    tcHeartRt.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "/min"
                    };
                    tcHeartRt.Keyword.Comment = measurment.Comment;
                    tcHeartRt.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcHeartRt.Keyword.Value = measurment.Term.Name;
                    tcHeartRt.Keyword.TextValue = measurment.Value.ToString();
                    tcHeartRt.Keyword.Datatype = terminology.Datatype;
                    tcHeartRt.Keyword.Terminology = terminology.Terminology;
                    tcHeartRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcHeartRt.Keyword.Level = 0;
                    tcHeartRt.Keyword.EntryUid = measurment.Guid;
                    tcHeartRt.Time = _createdOn;
                    tcHeartRt.Language = Language;
                    tcHeartRt.Encoding = Encoding;
                    tcHeartRt.Keyword.NumValue = measurment.Value;
                    tcHeartRt.Keyword.NumUnit = tcHeartRt.Measurement.Units;
                    transformedData.Measurements.Add(tcHeartRt);
                    _finalCompositionJson.Append(tcHeartRt.ToString());
                    break;

                case "1978":
                    TcPulseRate tcPulseRt = new TcPulseRate(_prefix, v);
                    tcPulseRt.Uid = measurment.Guid;
                    tcPulseRt.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "/min"
                    };
                    tcPulseRt.Keyword.Comment = measurment.Comment;
                    tcPulseRt.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcPulseRt.Keyword.Value = measurment.Term.Name;
                    tcPulseRt.Keyword.TextValue = measurment.Value.ToString();
                    tcPulseRt.Keyword.Datatype = terminology.Datatype;
                    tcPulseRt.Keyword.Terminology = terminology.Terminology;
                    tcPulseRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcPulseRt.Keyword.Level = 0;
                    tcPulseRt.Keyword.EntryUid = measurment.Guid;
                    tcPulseRt.Time = _createdOn;
                    tcPulseRt.Language = Language;
                    tcPulseRt.Encoding = Encoding;
                    tcPulseRt.Keyword.NumValue = measurment.Value;
                    tcPulseRt.Keyword.NumUnit = tcPulseRt.Measurement.Units;
                    transformedData.Measurements.Add(tcPulseRt);
                    _finalCompositionJson.Append(tcPulseRt.ToString());
                    break;

                case "402":
                    TcRespiratoryRate tcRespRt = new TcRespiratoryRate(_prefix, v);
                    tcRespRt.Uid = measurment.Guid;
                    tcRespRt.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = !string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "/min"
                    };
                    tcRespRt.Keyword.Comment = measurment.Comment;
                    tcRespRt.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcRespRt.Keyword.Value = measurment.Term.Name;
                    tcRespRt.Keyword.TextValue = measurment.Value.ToString();
                    tcRespRt.Keyword.Datatype = terminology.Datatype;
                    tcRespRt.Keyword.Terminology = terminology.Terminology;
                    tcRespRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcRespRt.Keyword.Level = 0;
                    tcRespRt.Keyword.EntryUid = measurment.Guid;
                    tcRespRt.Time = _createdOn;
                    tcRespRt.Language = Language;
                    tcRespRt.Encoding = Encoding;
                    tcRespRt.Keyword.NumValue = measurment.Value;
                    tcRespRt.Keyword.NumUnit = tcRespRt.Measurement.Units;
                    transformedData.Measurements.Add(tcRespRt);
                    _finalCompositionJson.Append(tcRespRt.ToString());
                    break;

                case "1995":
                    TcSaturation tcSat = new TcSaturation(_prefix, v);
                    tcSat.Uid = measurment.Guid;
                    tcSat.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.MeasurementFraction() { Numerator = measurment.Value, Denominator = 100.0M, Type = 2 };
                    tcSat.Keyword.Comment = measurment.Comment;
                    tcSat.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcSat.Keyword.Value = measurment.Term.Name;
                    tcSat.Keyword.TextValue = measurment.Value.ToString();
                    tcSat.Keyword.Datatype = terminology.Datatype;
                    tcSat.Keyword.Terminology = terminology.Terminology;
                    tcSat.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcSat.Keyword.Level = 0;
                    tcSat.Keyword.EntryUid = measurment.Guid;
                    tcSat.Time = _createdOn;
                    tcSat.Language = Language;
                    tcSat.Encoding = Encoding;
                    tcSat.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(tcSat);
                    _finalCompositionJson.Append(tcSat.ToString());
                    break;

                case "5251":
                    TcOxygenLevel tcOxyLvl = new TcOxygenLevel(_prefix, v);
                    tcOxyLvl.Uid = measurment.Guid;
                    tcOxyLvl.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.MeasurementFraction() { Numerator = measurment.Value, Denominator = 100.0M, Type = 2 };
                    tcOxyLvl.Keyword.Comment = measurment.Comment;
                    tcOxyLvl.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcOxyLvl.Keyword.Value = measurment.Term.Name;
                    tcOxyLvl.Keyword.TextValue = measurment.Value.ToString();
                    tcOxyLvl.Keyword.Datatype = terminology.Datatype;
                    tcOxyLvl.Keyword.Terminology = terminology.Terminology;
                    tcOxyLvl.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcOxyLvl.Keyword.Level = 0;
                    tcOxyLvl.Keyword.EntryUid = measurment.Guid;
                    tcOxyLvl.Time = _createdOn;
                    tcOxyLvl.Language = Language;
                    tcOxyLvl.Encoding = Encoding;
                    tcOxyLvl.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(tcOxyLvl);
                    _finalCompositionJson.Append(tcOxyLvl.ToString());
                    break;

                case "10827":
                    TcOxygenSaturation tcOxySat = new TcOxygenSaturation(_prefix, v);
                    tcOxySat.Uid = measurment.Guid;
                    tcOxySat.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.MeasurementFraction() { Numerator = measurment.Value, Denominator = 100.0M, Type = 2 };
                    tcOxySat.Keyword.Comment = measurment.Comment;
                    tcOxySat.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcOxySat.Keyword.Value = measurment.Term.Name;
                    tcOxySat.Keyword.TextValue = measurment.Value.ToString();
                    tcOxySat.Keyword.Datatype = terminology.Datatype;
                    tcOxySat.Keyword.Terminology = terminology.Terminology;
                    tcOxySat.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcOxySat.Keyword.Level = 0;
                    tcOxySat.Keyword.EntryUid = measurment.Guid;
                    tcOxySat.Time = _createdOn;
                    tcOxySat.Language = Language;
                    tcOxySat.Encoding = Encoding;
                    tcOxySat.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(tcOxySat);
                    _finalCompositionJson.Append(tcOxySat.ToString());
                    break;

                case "11273":
                    TcNewsTotalScore totalScore = new TcNewsTotalScore(_prefix, v);
                    totalScore.Uid = measurment.Guid;
                    totalScore.ScorePoint = measurment.Value;
                    totalScore.Keyword.Comment = measurment.Comment;
                    totalScore.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    totalScore.Keyword.Value = measurment.Term.Name;
                    totalScore.Keyword.TextValue = measurment.Value.ToString();
                    totalScore.Keyword.Datatype = terminology.Datatype;
                    totalScore.Keyword.Terminology = terminology.Terminology;
                    totalScore.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScore.Keyword.Level = 0;
                    totalScore.Keyword.EntryUid = measurment.Guid;
                    totalScore.Time = _createdOn;
                    totalScore.Language = Language;
                    totalScore.Encoding = Encoding;
                    totalScore.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(totalScore);
                    _finalCompositionJson.Append(totalScore.ToString());
                    break;

                case "11274":
                    TcNewsTotalScoreHeartRate totalScoreHtRt = new TcNewsTotalScoreHeartRate(_prefix, v);
                    totalScoreHtRt.Uid = measurment.Guid;
                    totalScoreHtRt.ScorePoint = measurment.Value;
                    totalScoreHtRt.Keyword.Comment = measurment.Comment;
                    totalScoreHtRt.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    totalScoreHtRt.Keyword.Value = measurment.Term.Name;
                    totalScoreHtRt.Keyword.TextValue = measurment.Value.ToString();
                    totalScoreHtRt.Keyword.Datatype = terminology.Datatype;
                    totalScoreHtRt.Keyword.Terminology = terminology.Terminology;
                    totalScoreHtRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScoreHtRt.Keyword.Level = 0;
                    totalScoreHtRt.Keyword.EntryUid = measurment.Guid;
                    totalScoreHtRt.Time = _createdOn;
                    totalScoreHtRt.Language = Language;
                    totalScoreHtRt.Encoding = Encoding;
                    totalScoreHtRt.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(totalScoreHtRt);
                    _finalCompositionJson.Append(totalScoreHtRt.ToString());
                    break;

                case "11275":
                    TcNewsTotalScoreO2Sat totalScoreO2Sat = new TcNewsTotalScoreO2Sat(_prefix, v);
                    totalScoreO2Sat.Uid = measurment.Guid;
                    totalScoreO2Sat.ScorePoint = measurment.Value;
                    totalScoreO2Sat.Keyword.Comment = measurment.Comment;
                    totalScoreO2Sat.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    totalScoreO2Sat.Keyword.Value = measurment.Term.Name;
                    totalScoreO2Sat.Keyword.TextValue = measurment.Value.ToString();
                    totalScoreO2Sat.Keyword.Datatype = terminology.Datatype;
                    totalScoreO2Sat.Keyword.Terminology = terminology.Terminology;
                    totalScoreO2Sat.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScoreO2Sat.Keyword.Level = 0;
                    totalScoreO2Sat.Keyword.EntryUid = measurment.Guid;
                    totalScoreO2Sat.Time = _createdOn;
                    totalScoreO2Sat.Language = Language;
                    totalScoreO2Sat.Encoding = Encoding;
                    totalScoreO2Sat.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(totalScoreO2Sat);
                    _finalCompositionJson.Append(totalScoreO2Sat.ToString());
                    break;

                case "11276":
                    TcNewsScoreHRO2Sat totalScoreHRO2Sat = new TcNewsScoreHRO2Sat(_prefix, v);
                    totalScoreHRO2Sat.Uid = measurment.Guid;
                    totalScoreHRO2Sat.ScorePoint = measurment.Value;
                    totalScoreHRO2Sat.Keyword.Comment = measurment.Comment;
                    totalScoreHRO2Sat.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    totalScoreHRO2Sat.Keyword.Value = measurment.Term.Name;
                    totalScoreHRO2Sat.Keyword.TextValue = measurment.Value.ToString();
                    totalScoreHRO2Sat.Keyword.Datatype = terminology.Datatype;
                    totalScoreHRO2Sat.Keyword.Terminology = terminology.Terminology;
                    totalScoreHRO2Sat.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScoreHRO2Sat.Keyword.Level = 0;
                    totalScoreHRO2Sat.Keyword.EntryUid = measurment.Guid;
                    totalScoreHRO2Sat.Time = _createdOn;
                    totalScoreHRO2Sat.Language = Language;
                    totalScoreHRO2Sat.Encoding = Encoding;
                    totalScoreHRO2Sat.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(totalScoreHRO2Sat);
                    _finalCompositionJson.Append(totalScoreHRO2Sat.ToString());
                    break;

                case "1849":
                    var bp = new TcBloodPressure(_prefix, v);
                    var measureVal = measurment.Value.ToString().Split('/');
                    bp.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    bp.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement()
                    {
                        Magnitude = measurment.Value,
                        Units = string.IsNullOrWhiteSpace(standardUnit) ? standardUnit : "mm[Hg]"
                    };
                    bp.Keyword.Comment = measurment.Comment;
                    bp.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    bp.Keyword.Value = measurment.Term.Name;
                    bp.Keyword.TextValue = measurment.Value.ToString();
                    bp.Keyword.Datatype = terminology.Datatype;
                    bp.Keyword.Terminology = terminology.Terminology;
                    bp.Keyword.OriginalUnit = measurment.Term.Unit;
                    bp.Keyword.Level = 0;
                    bp.Keyword.EntryUid = measurment.Guid;
                    bp.Keyword.NumValue = measurment.Value;
                    bp.Keyword.NumUnit = bp.Systolic.Units;
                    bp.Language = Language;
                    bp.Encoding = Encoding;
                    transformedData.Measurements.Add(bp);
                    _finalCompositionJson.Append(bp.ToString());
                    break;

                case "6531":
                    var tcProbDiag = new TcProblemDiagnosis(_prefix, v);
                    tcProbDiag.Uid = measurment.Guid;
                    tcProbDiag.ProblemName = string.Empty;
                    tcProbDiag.Variant = string.Empty;
                    tcProbDiag.Keyword.Comment = measurment.Comment;
                    tcProbDiag.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcProbDiag.Keyword.Value = measurment.Term.Name;
                    tcProbDiag.Keyword.TextValue =measurment.Value.ToString();
                    tcProbDiag.Keyword.Datatype = terminology.Datatype;
                    tcProbDiag.Keyword.Terminology = terminology.Terminology;
                    tcProbDiag.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcProbDiag.Keyword.Level = 0;
                    tcProbDiag.Keyword.EntryUid = measurment.Guid;
                    tcProbDiag.Time = _createdOn;
                    tcProbDiag.Language = Language;
                    tcProbDiag.Encoding = Encoding;
                    tcProbDiag.Keyword.NumValue = measurment.Value;
                    transformedData.Measurements.Add(tcProbDiag);
                    _finalCompositionJson.Append(tcProbDiag.ToString());
                    break;

                default:
                    AddGenericEntry(measurment);
                    break;
            }
            counterMap[measurment.Term.Id.ToString()]++;

        }
    }
}
