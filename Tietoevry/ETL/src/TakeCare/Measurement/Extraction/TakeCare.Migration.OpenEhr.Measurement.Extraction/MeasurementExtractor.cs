using Newtonsoft.Json;
using Spine.Migration.OpenEhr.Etl.Core.Models;

namespace TakeCare.Migration.OpenEhr.Measurement.Extraction
{
    using DtoModel;
    using Model;

    public class MeasurementExtractor : IMeasurementExtractor
    {
        public Task<ExtractionResult<TResult>> Extract<TConfiguration, TResult>(ExtractionConfiguration<TConfiguration> configuration)
            where TConfiguration : class
            where TResult : class
        {
            var measurements = ExtractMeasurementData(configuration.Configuration as string);
            var result = new ExtractionResult<TResult>(measurements as TResult);
            return Task.FromResult(result);
        }

        private static MeasurementDto ExtractMeasurementData(string file)
        {
            var fileName = Path.GetFileName(file);
            var result = new MeasurementDto();
           
            string fileContent = File.ReadAllText(file);

            JsonResult jsonResult = JsonConvert.DeserializeObject<JsonResult>(fileContent);
            UpdateParentChildIds(jsonResult);

            //Measurements_196607113146_20240927103608-198407031684
            var namesplits = fileName.Split(['-', '_']);
            result.PatientId = namesplits[1]; //patient id
            result.CreatedOn = string.IsNullOrWhiteSpace(namesplits[2]) ? jsonResult.Created.DateTime : namesplits[2]; //created on
            result.CreatedByUserId = string.IsNullOrWhiteSpace(namesplits[3]) ? jsonResult.Created.User.Id : namesplits[3]; //created by user id
            result.Measurements = jsonResult.Measurements;
            result.SavedBy = jsonResult.Saved;
            result.CreatedBy = jsonResult.Created;
            result.CareUnitId = jsonResult.Template?.Id?.Split('-')[0];
            result.LinkCode = jsonResult.LinkCode;
            result.VersionId = jsonResult.VersionId;
            result.TemplateId = jsonResult.Template?.Id;
            result.TemplateName = jsonResult.Template?.Name;
            //creating template file
            string templateContent = JsonConvert.SerializeObject(jsonResult.Template, Formatting.Indented);
            var extractedFilePath = Path.GetDirectoryName(file).Replace("TestData", "ExtractedData");
            extractedFilePath = Path.Combine(extractedFilePath, "Templates");
            bool createTemplateFile = true;
            if (!Directory.Exists(extractedFilePath))
            {
                Directory.CreateDirectory(extractedFilePath);
            }
            else
            {
                Directory.EnumerateFiles(extractedFilePath).ToList().ForEach(f =>
                {
                    if(Path.GetFileNameWithoutExtension(f).Contains(jsonResult.Template.Id))
                    {
                        createTemplateFile = false;
                    }
                });
            }

            if(createTemplateFile)
                CreateTemplateFile(jsonResult.Template.Id, templateContent, extractedFilePath);

            return result;
        }

        private static void CreateTemplateFile(string templateId, string templateContent, string extractedFilePath)
        {
            var templateFileName = Path.Combine(extractedFilePath, $"template-{templateId}.json");
            File.WriteAllText(templateFileName, templateContent);
        }

        private static void UpdateParentChildIds(JsonResult? jsonResult)
        {
            jsonResult?.Measurements?.ForEach(measurement =>
            {
                var mesureGuid = Guid.NewGuid();
                measurement.Guid = mesureGuid;

                measurement.Term.MeasurementGuid = mesureGuid;
                var termGuid = Guid.NewGuid();
                measurement.Term.Guid = termGuid;
                
            });
        }
    }
}
