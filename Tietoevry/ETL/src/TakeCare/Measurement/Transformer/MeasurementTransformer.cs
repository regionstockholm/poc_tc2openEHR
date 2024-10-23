using Newtonsoft.Json.Linq;
using Spine.Foundation.Web.OpenEhr.Archetype.Entry;
using Spine.Migration.OpenEhr.Etl.Core.Models;
using System.Text;
using TakeCare.Foundation.OpenEhr.Archetype.Entry;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Services;
using TakeCare.Migration.OpenEhr.Measurement.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Measurement.Extraction.Model;
using TakeCare.Migration.OpenEhr.Measurement.Transformer.Model;

namespace TakeCare.Migration.OpenEhr.Measurement.Transformer
{
    public class MeasurementTransformer : IMeasurementTransformer
    {
        private const string _prefix = "vårdkontakt/ickm";
        private readonly IPatientService _patientService;
        private readonly ITerminologyProvider _terminologyProvider;
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


        public MeasurementTransformer(IPatientService patientService, ITerminologyProvider terminologyProvider)
        {
            _patientService = patientService;
            _terminologyProvider = terminologyProvider;
            _txtFilePath = Path.Combine(AppContext.BaseDirectory, @"Assets\TemplatesTxts");
            _finalCompositionJson = new StringBuilder();
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
            var careUnitDetails = new CareUnit() { Id = "2748", Name = "2748" };
            if (measurementDto.CreatedBy.CareUnit != null)
            {
                careUnitDetails = measurementDto.CreatedBy.CareUnit;
            }
            transformedData = new OpenEhrMeasurement()
            {
                PatientID = _patientService.GetPatient(measurementDto.PatientId).PatientId,
                Measurements = new List<BaseEntry>()
            };
            _createdByUserId = measurementDto.CreatedBy.User.Id;
            _createdByUserName = measurementDto.CreatedBy.User.FullName;
            _createdOn = measurementDto.CreatedBy.DateTime;

            _savedByUserId = measurementDto.SavedBy.User.Id;
            _savedByUserName = measurementDto.SavedBy.User.FullName;
            _savedOn = measurementDto.SavedBy.DateTime;
            counterMap.Add("generic", 0);

            //add ctx
            var context = new Context();
            context.ParticipationId = _savedOn;
            context.ParticipationName = _savedByUserName;
            context.Tags = new List<TagValue>();
            context.Tags.Add(new TagValue() { tag = "formName", value = "RSK - Journal Encounter Form" });
            context.Tags.Add(new TagValue() { tag = "formVersion", value = "1.0.0" });
            context.ComposerName = _createdByUserName;
            context.IdNamespace = "HOSPITAL-NS";
            context.IdScheme = "HOSPITAL-NS";
            context.ParticipationFunction = "requester";
            context.ParticipationMode = "face-to-face communication";
            context.HealthCareFacilityName = careUnitDetails.Name;
            context.HealthCareFacilityId = careUnitDetails.Id;
            transformedData.Context = context;
            _finalCompositionJson.Append(context.ToString());

            //add ctx care unit
            var careUnit = new TcCareUnitContext();
            transformedData.CareUnitContext = careUnit;
            _finalCompositionJson.Append(careUnit.ToString());

            //add context metadata
            var contextMetadata = new TcContextMetadata() { SavedByUserId = _savedByUserId, SavedOn = _savedOn, CreatedByUserId = _createdByUserId };
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

            //todo - check where to map decimal and scale

            var gEntry = new TcGenericEntry("vårdkontakt", Convert.ToString(v));
            gEntry.Uid = measurment.Guid;
            gEntry.Keyword.Code = Convert.ToString(measurment.Term.Id);
            gEntry.Keyword.Value = measurment.Term.Name;
            gEntry.Keyword.TextValue = measurment.Value.ToString();
            gEntry.Keyword.Comment = measurment.Comment;
            gEntry.Keyword.EntryUid = measurment.Guid;
            gEntry.Keyword.OriginalUnit = measurment.Term.Unit;
            gEntry.Keyword.Level = 0;
            gEntry.Language = Language;
            gEntry.Encoding = Encoding;
            transformedData.Measurements.Add(gEntry);
            _finalCompositionJson.Append(gEntry.ToString());

            counterMap["generic"]++;
        }

        private void AddCKMEntry(Extraction.Model.Measurement measurment, TerminologyDetails terminology)
        {
            object[] values = { };
            var fileName = string.Empty;

            if (!counterMap.ContainsKey(measurment.Term.Id.ToString()))
                counterMap.Add(measurment.Term.Id.ToString(), 0);
            var v = Convert.ToString(counterMap[measurment.Term.Id.ToString()]);
            switch (terminology.TermId)
            {
                case "3719": //Systolic Upper
                    var systolicUpper = new TcSystolicUpper(_prefix, v);
                    systolicUpper.Uid = measurment.Guid;
                    systolicUpper.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    systolicUpper.Keyword.Comment = measurment.Comment;
                    systolicUpper.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    systolicUpper.Keyword.Value = measurment.Term.Name;
                    systolicUpper.Keyword.TextValue = measurment.Term.Name;
                    systolicUpper.Keyword.Datatype = terminology.Datatype;
                    systolicUpper.Keyword.OriginalUnit = measurment.Term.Unit;
                    systolicUpper.Keyword.Level = 0;
                    systolicUpper.Keyword.EntryUid = measurment.Guid;
                    systolicUpper.Language = Language;
                    systolicUpper.Encoding = Encoding;
                    transformedData.Measurements.Add(systolicUpper);
                    _finalCompositionJson.Append(systolicUpper.ToString());

                    break;

                case "3720":
                    var diastolicLower = new TcDiastolicLower(_prefix, v);
                    diastolicLower.Uid = measurment.Guid;
                    diastolicLower.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    diastolicLower.Keyword.Comment = measurment.Comment;
                    diastolicLower.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    diastolicLower.Keyword.Value = measurment.Term.Name;
                    diastolicLower.Keyword.TextValue = measurment.Term.Name;
                    diastolicLower.Keyword.Datatype = terminology.Datatype;
                    diastolicLower.Keyword.OriginalUnit = measurment.Term.Unit;
                    diastolicLower.Keyword.Level = 0;
                    diastolicLower.Keyword.EntryUid = measurment.Guid;
                    diastolicLower.Language = Language;
                    diastolicLower.Encoding = Encoding;
                    transformedData.Measurements.Add(diastolicLower);
                    _finalCompositionJson.Append(diastolicLower.ToString());
                    break;

                case "4243":
                    var meanArtBP = new TcMeanArterialBP(_prefix, v);
                    meanArtBP.Uid = measurment.Guid;
                    meanArtBP.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    meanArtBP.Keyword.Comment = measurment.Comment;
                    meanArtBP.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    meanArtBP.Keyword.Value = measurment.Term.Name;
                    meanArtBP.Keyword.TextValue = measurment.Term.Name;
                    meanArtBP.Keyword.Datatype = terminology.Datatype;
                    meanArtBP.Keyword.OriginalUnit = measurment.Term.Unit;
                    meanArtBP.Keyword.Level = 0;
                    meanArtBP.Keyword.EntryUid = measurment.Guid;
                    meanArtBP.Language = Language;
                    meanArtBP.Encoding = Encoding;
                    transformedData.Measurements.Add(meanArtBP);
                    _finalCompositionJson.Append(meanArtBP.ToString());
                    break;

                case "4378":
                    var meanArtBPSys = new TcInvasiveSystolic("vårdkontakt/ickm", v);
                    meanArtBPSys.Uid = measurment.Guid;
                    meanArtBPSys.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    meanArtBPSys.Keyword.Comment = measurment.Comment;
                    meanArtBPSys.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    meanArtBPSys.Keyword.Value = measurment.Term.Name;
                    meanArtBPSys.Keyword.TextValue = measurment.Term.Name;
                    meanArtBPSys.Keyword.Datatype = terminology.Datatype;
                    meanArtBPSys.Keyword.OriginalUnit = measurment.Term.Unit;
                    meanArtBPSys.Keyword.Level = 0;
                    meanArtBPSys.Keyword.EntryUid = measurment.Guid;
                    meanArtBPSys.Language = Language;
                    meanArtBPSys.Encoding = Encoding;
                    transformedData.Measurements.Add(meanArtBPSys);
                    _finalCompositionJson.Append(meanArtBPSys.ToString());
                    break;

                case "4379":
                    var meanArtBPDis = new TcInvasiveDiastolic("vårdkontakt/ickm", v);
                    meanArtBPDis.Uid = measurment.Guid;
                    meanArtBPDis.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    meanArtBPDis.Keyword.Comment = measurment.Comment;
                    meanArtBPDis.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    meanArtBPDis.Keyword.Value = measurment.Term.Name;
                    meanArtBPDis.Keyword.TextValue = measurment.Term.Name;
                    meanArtBPDis.Keyword.Datatype = terminology.Datatype;
                    meanArtBPDis.Keyword.OriginalUnit = measurment.Term.Unit;
                    meanArtBPDis.Keyword.Level = 0;
                    meanArtBPDis.Keyword.EntryUid = measurment.Guid;
                    meanArtBPDis.Language = Language;
                    meanArtBPDis.Encoding = Encoding;
                    transformedData.Measurements.Add(meanArtBPDis);
                    _finalCompositionJson.Append(meanArtBPDis.ToString());
                    break;

                case "6134":
                    var rightArmSyst = new TcRightArmSystolic(_prefix, v);
                    rightArmSyst.Uid = measurment.Guid;
                    rightArmSyst.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    rightArmSyst.Keyword.Comment = measurment.Comment;
                    rightArmSyst.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    rightArmSyst.Keyword.Value = measurment.Term.Name;
                    rightArmSyst.Keyword.TextValue = measurment.Term.Name;
                    rightArmSyst.Keyword.Datatype = terminology.Datatype;
                    rightArmSyst.Keyword.OriginalUnit = measurment.Term.Unit;
                    rightArmSyst.Keyword.Level = 0;
                    rightArmSyst.Keyword.EntryUid = measurment.Guid;
                    rightArmSyst.Language = Language;
                    rightArmSyst.Encoding = Encoding;
                    transformedData.Measurements.Add(rightArmSyst);
                    _finalCompositionJson.Append(rightArmSyst.ToString());

                    break;

                case "6136":
                    var armLeftSyst = new TcLeftArmSystolic(_prefix, v);
                    armLeftSyst.Uid = measurment.Guid;
                    armLeftSyst.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    armLeftSyst.Keyword.Comment = measurment.Comment;
                    armLeftSyst.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    armLeftSyst.Keyword.Value = measurment.Term.Name;
                    armLeftSyst.Keyword.TextValue = measurment.Term.Name;
                    armLeftSyst.Keyword.Datatype = terminology.Datatype;
                    armLeftSyst.Keyword.OriginalUnit = measurment.Term.Unit;
                    armLeftSyst.Keyword.Level = 0;
                    armLeftSyst.Keyword.EntryUid = measurment.Guid;
                    armLeftSyst.Language = Language;
                    armLeftSyst.Encoding = Encoding;
                    transformedData.Measurements.Add(armLeftSyst);
                    _finalCompositionJson.Append(armLeftSyst.ToString());
                    break;

                case "6135":
                    var armRightDiastolic = new TcRightArmDiastolic(_prefix, v);
                    armRightDiastolic.Uid = measurment.Guid;
                    armRightDiastolic.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    armRightDiastolic.Keyword.Comment = measurment.Comment;
                    armRightDiastolic.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    armRightDiastolic.Keyword.Value = measurment.Term.Name;
                    armRightDiastolic.Keyword.TextValue = measurment.Term.Name;
                    armRightDiastolic.Keyword.Datatype = terminology.Datatype;
                    armRightDiastolic.Keyword.OriginalUnit = measurment.Term.Unit;
                    armRightDiastolic.Keyword.Level = 0;
                    armRightDiastolic.Keyword.EntryUid = measurment.Guid;
                    armRightDiastolic.Language = Language;
                    armRightDiastolic.Encoding = Encoding;
                    transformedData.Measurements.Add(armRightDiastolic);
                    _finalCompositionJson.Append(armRightDiastolic.ToString());
                    break;

                case "6137":
                    var armdist = new TcLeftArmDiastolic(_prefix, v);
                    armdist.Uid = measurment.Guid;
                    armdist.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    armdist.Keyword.Comment = measurment.Comment;
                    armdist.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    armdist.Keyword.Value = measurment.Term.Name;
                    armdist.Keyword.TextValue = measurment.Term.Name;
                    armdist.Keyword.Datatype = terminology.Datatype;
                    armdist.Keyword.OriginalUnit = measurment.Term.Unit;
                    armdist.Keyword.Level = 0;
                    armdist.Keyword.EntryUid = measurment.Guid;
                    armdist.Language = Language;
                    armdist.Encoding = Encoding;
                    transformedData.Measurements.Add(armdist);
                    _finalCompositionJson.Append(armdist.ToString());
                    break;

                case "8980":
                    var bpCurve = new TcBPCurve24Hour(_prefix, v);
                    bpCurve.Uid = measurment.Guid;
                    bpCurve.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    bpCurve.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    bpCurve.MathFunctionCode = "146";
                    bpCurve.MathFunctionValue = "mean";
                    bpCurve.MathFunctionTerminology = "openehr";
                    bpCurve.Keyword.Comment = measurment.Comment;
                    bpCurve.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    bpCurve.Keyword.Value = measurment.Term.Name;
                    bpCurve.Keyword.TextValue = measurment.Term.Name;
                    bpCurve.Keyword.Datatype = terminology.Datatype;
                    bpCurve.Keyword.OriginalUnit = measurment.Term.Unit;
                    bpCurve.Keyword.Level = 0;
                    bpCurve.Keyword.EntryUid = measurment.Guid;
                    bpCurve.Language = Language;
                    bpCurve.Encoding = Encoding;
                    transformedData.Measurements.Add(bpCurve);
                    _finalCompositionJson.Append(bpCurve.ToString());
                    break;

                case "1965":
                    TcWeight weight = new TcWeight(_prefix, v);
                    weight.Uid = measurment.Guid;
                    weight.WeightValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "kg"/*measurment.Term.Unit*/ };
                    weight.Keyword.Comment = measurment.Comment;
                    weight.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    weight.Keyword.Value = measurment.Term.Name;
                    weight.Keyword.TextValue = measurment.Term.Name;
                    weight.Keyword.Datatype = terminology.Datatype;
                    weight.Keyword.OriginalUnit = measurment.Term.Unit;
                    weight.Keyword.Level = 0;
                    weight.Keyword.EntryUid = measurment.Guid;
                    weight.Time = _createdOn;
                    weight.Language = Language;
                    weight.Encoding = Encoding;
                    transformedData.Measurements.Add(weight);
                    _finalCompositionJson.Append(weight.ToString());
                    break;
                case "2896":
                    TcBirthWeight birthWeight = new TcBirthWeight(_prefix, v);
                    birthWeight.Uid = measurment.Guid;
                    birthWeight.WeightValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "kg"/*measurment.Term.Unit*/ };
                    birthWeight.Keyword.Comment = measurment.Comment;
                    birthWeight.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    birthWeight.Keyword.Value = measurment.Term.Name;
                    birthWeight.Keyword.TextValue = measurment.Term.Name;
                    birthWeight.Keyword.Datatype = terminology.Datatype;
                    birthWeight.Keyword.OriginalUnit = measurment.Term.Unit;
                    birthWeight.Keyword.Level = 0;
                    birthWeight.Keyword.EntryUid = measurment.Guid;
                    birthWeight.Time = _createdOn;
                    birthWeight.Language = Language;
                    birthWeight.Encoding = Encoding;
                    transformedData.Measurements.Add(birthWeight);
                    _finalCompositionJson.Append(birthWeight.ToString());
                    break;

                case "5028":
                    TcBareWeight bweight = new TcBareWeight(_prefix, v);
                    bweight.Uid = measurment.Guid;
                    bweight.WeightValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "kg"/*measurment.Term.Unit*/ };
                    bweight.Keyword.Comment = measurment.Comment;
                    bweight.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    bweight.Keyword.Value = measurment.Term.Name;
                    bweight.Keyword.TextValue = measurment.Term.Name;
                    bweight.Keyword.Datatype = terminology.Datatype;
                    bweight.Keyword.OriginalUnit = measurment.Term.Unit;
                    bweight.Keyword.Level = 0;
                    bweight.Keyword.EntryUid = measurment.Guid;
                    bweight.Time = _createdOn;
                    bweight.Language = Language;
                    bweight.Encoding = Encoding;
                    transformedData.Measurements.Add(bweight);
                    _finalCompositionJson.Append(bweight.ToString());
                    break;

                case "1964":
                    TcLength length = new TcLength(_prefix, v);
                    length.Uid = measurment.Guid;
                    length.LengthValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = measurment.Term.Unit };
                    length.Keyword.Comment = measurment.Comment;
                    length.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    length.Keyword.Value = measurment.Term.Name;
                    length.Keyword.TextValue = measurment.Term.Name;
                    length.Keyword.Datatype = terminology.Datatype;
                    length.Keyword.OriginalUnit = measurment.Term.Unit;
                    length.Keyword.Level = 0;
                    length.Keyword.EntryUid = measurment.Guid;
                    length.Time = _createdOn;
                    length.Language = Language;
                    length.Encoding = Encoding;

                    transformedData.Measurements.Add(length);
                    _finalCompositionJson.Append(length.ToString());
                    break;

                case "6179":
                    TcHorizontalLength hLength = new TcHorizontalLength(_prefix, v);
                    hLength.Uid = measurment.Guid;
                    hLength.LengthValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = measurment.Term.Unit };
                    hLength.Keyword.Comment = measurment.Comment;
                    hLength.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    hLength.Keyword.Value = measurment.Term.Name;
                    hLength.Keyword.TextValue = measurment.Term.Name;
                    hLength.Keyword.Datatype = terminology.Datatype;
                    hLength.Keyword.OriginalUnit = measurment.Term.Unit;
                    hLength.Keyword.Level = 0;
                    hLength.Keyword.EntryUid = measurment.Guid;
                    hLength.Time = _createdOn;
                    hLength.Language = Language;
                    hLength.Encoding = Encoding;
                    hLength.BodyPosition = new TcLengthTerminology() { Code = "at0020", Value = "Liggande", Terminology = "local" };
                    transformedData.Measurements.Add(hLength);
                    _finalCompositionJson.Append(hLength.ToString());
                    break;

                case "6180":
                    TcSittingLength sLength = new TcSittingLength(_prefix, v);
                    sLength.Uid = measurment.Guid;
                    sLength.LengthValue = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = measurment.Term.Unit };
                    sLength.Keyword.Comment = measurment.Comment;
                    sLength.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    sLength.Keyword.Value = measurment.Term.Name;
                    sLength.Keyword.TextValue = measurment.Term.Name;
                    sLength.Keyword.Datatype = terminology.Datatype;
                    sLength.Keyword.OriginalUnit = measurment.Term.Unit;
                    sLength.Keyword.Level = 0;
                    sLength.Keyword.EntryUid = measurment.Guid;
                    sLength.Time = _createdOn;
                    sLength.Language = Language;
                    sLength.Encoding = Encoding;
                    sLength.BodyPosition = new TcLengthTerminology() { Code = "at0037", Value = "Sittställning", Terminology = "local" };
                    sLength.BodySegmentName = new TcLengthTerminology() { Code = "at0017", Value = "Sitthöjd", Terminology = "local" };
                    transformedData.Measurements.Add(sLength);
                    _finalCompositionJson.Append(sLength.ToString());
                    break;

                case "8883":
                    TcBMI tcBMI = new TcBMI(_prefix, v);
                    tcBMI.Uid = measurment.Guid;
                    tcBMI.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = measurment.Term.Unit };
                    tcBMI.Keyword.Comment = measurment.Comment;
                    tcBMI.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcBMI.Keyword.Value = measurment.Term.Name;
                    tcBMI.Keyword.TextValue = measurment.Term.Name;
                    tcBMI.Keyword.Datatype = terminology.Datatype;
                    tcBMI.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcBMI.Keyword.Level = 0;
                    tcBMI.Keyword.EntryUid = measurment.Guid;
                    tcBMI.Time = _createdOn;
                    tcBMI.Language = Language;
                    tcBMI.Encoding = Encoding;
                    transformedData.Measurements.Add(tcBMI);
                    _finalCompositionJson.Append(tcBMI.ToString());
                    break;

                case "2025":
                    TcBodyTemparature tcBodyTemp = new TcBodyTemparature(_prefix, v);
                    tcBodyTemp.Uid = measurment.Guid;
                    tcBodyTemp.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "Cel"/*measurment.Term.Unit*/ };
                    tcBodyTemp.Keyword.Comment = measurment.Comment;
                    tcBodyTemp.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcBodyTemp.Keyword.Value = measurment.Term.Name;
                    tcBodyTemp.Keyword.TextValue = measurment.Term.Name;
                    tcBodyTemp.Keyword.Datatype = terminology.Datatype;
                    tcBodyTemp.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcBodyTemp.Keyword.Level = 0;
                    tcBodyTemp.Keyword.EntryUid = measurment.Guid;
                    tcBodyTemp.Time = _createdOn;
                    tcBodyTemp.Language = Language;
                    tcBodyTemp.Encoding = Encoding;
                    transformedData.Measurements.Add(tcBodyTemp);
                    _finalCompositionJson.Append(tcBodyTemp.ToString());
                    break;

                case "11140":
                    TcHeartRate tcHeartRt = new TcHeartRate(_prefix, v);
                    tcHeartRt.Uid = measurment.Guid;
                    tcHeartRt.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "/min"/*measurment.Term.Unit*/ };
                    tcHeartRt.Keyword.Comment = measurment.Comment;
                    tcHeartRt.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcHeartRt.Keyword.Value = measurment.Term.Name;
                    tcHeartRt.Keyword.TextValue = measurment.Term.Name;
                    tcHeartRt.Keyword.Datatype = terminology.Datatype;
                    tcHeartRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcHeartRt.Keyword.Level = 0;
                    tcHeartRt.Keyword.EntryUid = measurment.Guid;
                    tcHeartRt.Time = _createdOn;
                    tcHeartRt.Language = Language;
                    tcHeartRt.Encoding = Encoding;
                    transformedData.Measurements.Add(tcHeartRt);
                    _finalCompositionJson.Append(tcHeartRt.ToString());
                    break;

                case "1978":
                    TcPulseRate tcPulseRt = new TcPulseRate(_prefix, v);
                    tcPulseRt.Uid = measurment.Guid;
                    tcPulseRt.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "/min"/*measurment.Term.Unit*/ };
                    tcPulseRt.Keyword.Comment = measurment.Comment;
                    tcPulseRt.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcPulseRt.Keyword.Value = measurment.Term.Name;
                    tcPulseRt.Keyword.TextValue = measurment.Term.Name;
                    tcPulseRt.Keyword.Datatype = terminology.Datatype;
                    tcPulseRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcPulseRt.Keyword.Level = 0;
                    tcPulseRt.Keyword.EntryUid = measurment.Guid;
                    tcPulseRt.Time = _createdOn;
                    tcPulseRt.Language = Language;
                    tcPulseRt.Encoding = Encoding;
                    transformedData.Measurements.Add(tcPulseRt);
                    _finalCompositionJson.Append(tcPulseRt.ToString());
                    break;

                case "402":
                    TcRespiratoryRate tcRespRt = new TcRespiratoryRate(_prefix, v);
                    tcRespRt.Uid = measurment.Guid;
                    tcRespRt.Measurement = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "/min"/*measurment.Term.Unit*/ };
                    tcRespRt.Keyword.Comment = measurment.Comment;
                    tcRespRt.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    tcRespRt.Keyword.Value = measurment.Term.Name;
                    tcRespRt.Keyword.TextValue = measurment.Term.Name;
                    tcRespRt.Keyword.Datatype = terminology.Datatype;
                    tcRespRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcRespRt.Keyword.Level = 0;
                    tcRespRt.Keyword.EntryUid = measurment.Guid;
                    tcRespRt.Time = _createdOn;
                    tcRespRt.Language = Language;
                    tcRespRt.Encoding = Encoding;
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
                    tcSat.Keyword.TextValue = measurment.Term.Name;
                    tcSat.Keyword.Datatype = terminology.Datatype;
                    tcSat.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcSat.Keyword.Level = 0;
                    tcSat.Keyword.EntryUid = measurment.Guid;
                    tcSat.Time = _createdOn;
                    tcSat.Language = Language;
                    tcSat.Encoding = Encoding;
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
                    tcOxyLvl.Keyword.TextValue = measurment.Term.Name;
                    tcOxyLvl.Keyword.Datatype = terminology.Datatype;
                    tcOxyLvl.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcOxyLvl.Keyword.Level = 0;
                    tcOxyLvl.Keyword.EntryUid = measurment.Guid;
                    tcOxyLvl.Time = _createdOn;
                    tcOxyLvl.Language = Language;
                    tcOxyLvl.Encoding = Encoding;
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
                    tcOxySat.Keyword.TextValue = measurment.Term.Name;
                    tcOxySat.Keyword.Datatype = terminology.Datatype;
                    tcOxySat.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcOxySat.Keyword.Level = 0;
                    tcOxySat.Keyword.EntryUid = measurment.Guid;
                    tcOxySat.Time = _createdOn;
                    tcOxySat.Language = Language;
                    tcOxySat.Encoding = Encoding;
                    transformedData.Measurements.Add(tcOxySat);
                    _finalCompositionJson.Append(tcOxySat.ToString());
                    break;

                case "11273":
                    TcNewsTotalScore totalScore = new TcNewsTotalScore(_prefix, v);
                    totalScore.Uid = measurment.Guid;
                    totalScore.ScorePoint = (decimal)measurment.Value;
                    totalScore.Keyword.Comment = measurment.Comment;
                    totalScore.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    totalScore.Keyword.Value = measurment.Term.Name;
                    totalScore.Keyword.TextValue = measurment.Term.Name;
                    totalScore.Keyword.Datatype = terminology.Datatype;
                    totalScore.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScore.Keyword.Level = 0;
                    totalScore.Keyword.EntryUid = measurment.Guid;
                    totalScore.Time = _createdOn;
                    totalScore.Language = Language;
                    totalScore.Encoding = Encoding;
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
                    totalScoreHtRt.Keyword.TextValue = measurment.Term.Name;
                    totalScoreHtRt.Keyword.Datatype = terminology.Datatype;
                    totalScoreHtRt.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScoreHtRt.Keyword.Level = 0;
                    totalScoreHtRt.Keyword.EntryUid = measurment.Guid;
                    totalScoreHtRt.Time = _createdOn;
                    totalScoreHtRt.Language = Language;
                    totalScoreHtRt.Encoding = Encoding;
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
                    totalScoreO2Sat.Keyword.TextValue = measurment.Term.Name;
                    totalScoreO2Sat.Keyword.Datatype = terminology.Datatype;
                    totalScoreO2Sat.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScoreO2Sat.Keyword.Level = 0;
                    totalScoreO2Sat.Keyword.EntryUid = measurment.Guid;
                    totalScoreO2Sat.Time = _createdOn;
                    totalScoreO2Sat.Language = Language;
                    totalScoreO2Sat.Encoding = Encoding;
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
                    totalScoreHRO2Sat.Keyword.TextValue = measurment.Term.Name;
                    totalScoreHRO2Sat.Keyword.Datatype = terminology.Datatype;
                    totalScoreHRO2Sat.Keyword.OriginalUnit = measurment.Term.Unit;
                    totalScoreHRO2Sat.Keyword.Level = 0;
                    totalScoreHRO2Sat.Keyword.EntryUid = measurment.Guid;
                    totalScoreHRO2Sat.Time = _createdOn;
                    totalScoreHRO2Sat.Language = Language;
                    totalScoreHRO2Sat.Encoding = Encoding;
                    transformedData.Measurements.Add(totalScoreHRO2Sat);
                    _finalCompositionJson.Append(totalScoreHRO2Sat.ToString());
                    break;

                case "1849":
                    var bp = new TcBloodPressure(_prefix, v);
                    bp.Systolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    //todo need to confirm from where to map diastolic from measurement json
                    //also from where to map math_function|code value
                    bp.Diastolic = new Spine.Foundation.Web.OpenEhr.Archetype.Entry.Measurement() { Magnitude = (decimal)measurment.Value, Units = "mm[Hg]"/*measurment.Term.Unit*/ };
                    bp.Keyword.Comment = measurment.Comment;
                    bp.Keyword.Code = Convert.ToString(measurment.Term.Id);
                    bp.Keyword.Value = measurment.Term.Name;
                    bp.Keyword.TextValue = measurment.Term.Name;
                    bp.Keyword.Datatype = terminology.Datatype;
                    bp.Keyword.OriginalUnit = measurment.Term.Unit;
                    bp.Keyword.Level = 0;
                    bp.Keyword.EntryUid = measurment.Guid;
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
                    tcProbDiag.Keyword.TextValue = measurment.Term.Name;
                    tcProbDiag.Keyword.Datatype = terminology.Datatype;
                    tcProbDiag.Keyword.OriginalUnit = measurment.Term.Unit;
                    tcProbDiag.Keyword.Level = 0;
                    tcProbDiag.Keyword.EntryUid = measurment.Guid;
                    tcProbDiag.Time = _createdOn;
                    tcProbDiag.Language = Language;
                    tcProbDiag.Encoding = Encoding;
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
