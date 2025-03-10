using Spine.Migration.OpenEhr.Etl.Core.Models;
using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.Medication.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.Medication.Extraction.Model;

namespace TakeCare.Migration.OpenEhr.Medication.Extraction
{
    public class MedicationExtractor : IMedicationExtractor
    {

        public Task<ExtractionResult<TResult>> Extract<TConfiguration, TResult>(ExtractionConfiguration<TConfiguration> configuration)
            where TConfiguration : class where TResult : class
        {
            var medication = ExtractMedicationData(configuration.Configuration as string);
            var result = new ExtractionResult<TResult>(medication as TResult);
            return Task.FromResult(result);
        }

        private object ExtractMedicationData(string file)
        {
            string xmlContent = string.Empty;
            XmlSerializer serializer;
            XMLResponse xmlResponse;
            MedicationDTO result;

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"Reading file - {file}");
            Console.WriteLine("----------------------------------------------");

            xmlContent = File.ReadAllText(file);
            serializer = new XmlSerializer(typeof(XMLResponse));

            using (StringReader reader = new StringReader(xmlContent))
            {
                xmlResponse = (XMLResponse)serializer.Deserialize(reader);
            }

            SetGuids(xmlResponse);
            //todos extract dto with parent and child guids 
            result = GetMedicationDTO(xmlResponse);

            return result;
        }

        private MedicationDTO GetMedicationDTO(XMLResponse? xmlResponse)
        {
            return new MedicationDTO
            {
                Time = xmlResponse.Time,
                User = xmlResponse.User,
                CareUnitIdType = xmlResponse.CareUnitIdType,
                CareUnitId = xmlResponse.CareUnitId,
                PatientId = xmlResponse.PatientId?.Id,
                Medications = xmlResponse.Medications
            };
        }

        private void SetGuids(XMLResponse? xmlResponse)
        {
            xmlResponse.Medications.ForEach(medication =>
            {
                medication.Guid = Guid.NewGuid();
                SetGuidsToChild(medication);
            });
        }

        private void SetGuidsToChild(Model.Medication medication)
        {
            var parentGuid = medication.Guid;

            medication.Prescription.ParentGuid = parentGuid;
            medication.Prescription.Guid = Guid.NewGuid();
            medication.ChildGuids.Add(medication.Prescription.Guid);

            medication.Drugs.ForEach(drug =>
            {
                drug.ParentGuid = parentGuid;
                drug.Guid = Guid.NewGuid();
                medication.ChildGuids.Add(drug.Guid);
            });

            medication.Dosage.ForEach(dosage => {
                dosage.ParentGuid = parentGuid;
                dosage.Guid = Guid.NewGuid();
                medication.ChildGuids.Add(dosage.Guid);

                dosage.DosageDrugs.ForEach(dd =>
                {
                    dd.ParentGuid = dosage.Guid;
                    dd.Guid = Guid.NewGuid();
                    //not added these guids in medication.ChildGuids for now
                });
            });

            medication.Days.ForEach(day =>
            {
                day.ParentGuid = parentGuid;
                day.Guid = Guid.NewGuid();
                medication.ChildGuids.Add(day.Guid);
            });

            medication.Administrations.ForEach(administration =>
            {
                administration.ParentGuid = parentGuid;
                administration.Guid = Guid.NewGuid();
                medication.ChildGuids.Add(administration.Guid);
            });

        }

    }
}
