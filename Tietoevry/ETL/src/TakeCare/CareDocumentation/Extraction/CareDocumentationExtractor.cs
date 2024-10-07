using Spine.Migration.OpenEhr.Etl.Core.Models;
using System.Xml.Serialization;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.DtoModel;
using TakeCare.Migration.OpenEhr.CareDocumentation.Extraction.Model.CareDoc;

namespace TakeCare.Migration.OpenEhr.CareDocumentation.Extraction
{
    public class CareDocumentationExtractor : ICareDocumentationExtractor
    {
        private static int parentCount = 0;


        public Task<ExtractionResult<TResult>> Extract<TConfiguration, TResult>(ExtractionConfiguration<TConfiguration> configuration)
            where TConfiguration : class where TResult : class
        {
            var caseNotes = ExtractCaseNotes(configuration.Configuration as string);
            var result = new ExtractionResult<TResult>(caseNotes as TResult);
            return Task.FromResult(result);
        }

        private static CareDocumentationDto ExtractCaseNotes(string file)
        {
            string xmlContent = string.Empty;
            xmlContent = File.ReadAllText(file);
            var serializer = new XmlSerializer(typeof(XMLResponse));
            var result = new CareDocumentationDto();

            using (StringReader reader = new StringReader(xmlContent))
            {
                var xmlResponse = (XMLResponse)serializer.Deserialize(reader);

                UpdateParentChildIds(xmlResponse);

                result.PatientId = xmlResponse.CareDocumentation.PatientId;
                result.CaseNotes = CreateFlatKeywordsPerCaseNote(xmlResponse);
                return result;
            }
        }

        private static void UpdateParentChildIds(XMLResponse xmlResponse)
        {
            xmlResponse.CareDocumentation.CaseNotes.ForEach(casenote =>
            {
                parentCount = 0;
                var newGuid = Guid.NewGuid();
                casenote.Content.Keyword.Guid = newGuid;
                casenote.Content.Keyword.ParentCount = ++parentCount;
                SetGuidsRecursively(casenote.Content.Keyword.Keywords, newGuid);
            });
        }

        private static void SetGuidsRecursively(List<Keyword> keywords, Guid parentId)
        {
            keywords.ForEach(k =>
            {
                k.ParentId = parentId;
                var newGuid = Guid.NewGuid();
                k.Guid = newGuid;
                if (k.Keywords != null && k.Keywords.Count > 0)
                {
                    k.ParentCount = ++parentCount;

                    SetGuidsRecursively(k.Keywords, newGuid);
                    --parentCount;
                }
            });
        }

        private static List<CaseNoteDto> CreateFlatKeywordsPerCaseNote(XMLResponse xmlResponse)
        {
            var allKeywords = new List<KeywordDto>();
            var result = new List<CaseNoteDto>();
            var cNoteVM = new CaseNoteDto();
            xmlResponse.CareDocumentation.CaseNotes.ForEach(casenote =>
            {
                allKeywords = new List<KeywordDto>();
                cNoteVM = GetCaseNoteVM(casenote);
                allKeywords.Add(GetVmFromKeyword(casenote.Content.Keyword));
                cNoteVM.Keywords.AddRange(GetKeywordsRecursively(casenote.Content.Keyword.Keywords, allKeywords));
                result.Add(cNoteVM);
            });

            UpdateChildGuids(result);

            return result;
        }

        private static void UpdateChildGuids(List<CaseNoteDto> result)
        {
            result.ForEach(casenote =>
            {
                casenote.Keywords.ForEach(key1 =>
                {
                    key1.Childs = new List<Guid>();
                    casenote.Keywords.ForEach(key2 =>
                    {
                        if (key2.ParentId == key1.Guid)
                        {
                            key1.Childs.Add(key2.Guid);
                        }
                    });
                });
            });
        }

        private static CaseNoteDto GetCaseNoteVM(Casenote casenote)
        {
            return new CaseNoteDto
            {
                DocumentId = casenote.DocumentId,
                DocCreatedByUserId = casenote.DocCreatedByUserId,
                DocCreatedByProfessionId = casenote.DocCreatedByProfessionId,
                DocCreatedAtCareUnitId = casenote.DocCreatedAtCareUnitId,
                DocCreatedTimestamp = casenote.DocCreatedTimestamp,
                DocSavedByUSerId = casenote.DocSavedByUSerId,
                DocSavedTimestamp = casenote.DocSavedTimestamp,
                SignerId = casenote.SignerId,
                SignedById = casenote.SignedById,
                CounterSignerId = casenote.CounterSignerId,
                SignedTimestamp = casenote.SignedTimestamp,
                ApprovedForPatient = casenote.ApprovedForPatient,
                EventDateTime = casenote.EventDateTime,
                DocumentTitle = casenote.DocumentTitle,
                TemplateName = casenote.TemplateName,
                TemplateId = casenote.TemplateId,
                DocumentCode = casenote.DocumentCode,
                HeaderTerm = casenote.HeaderTerm,
                Keywords = new List<KeywordDto>()
            };
        }

        private static List<KeywordDto> GetKeywordsRecursively(List<Keyword> keywords, List<KeywordDto> allKeyword)
        {
            foreach (var item in keywords)
            {
                allKeyword.Add(GetVmFromKeyword(item));

                if (item.Keywords.Count > 0)
                {
                    GetKeywordsRecursively(item.Keywords, allKeyword);
                }
            };

            return allKeyword;
        }

        private static KeywordDto GetVmFromKeyword(Keyword key)
        {
            return new KeywordDto
            {
                Guid = key.Guid,
                Name = key.Name,
                ParentCount = key.ParentCount,
                ParentId = key.ParentId,
                TermId = key.TermId,
                Value = key.Value,
                Comment = key.Comment
            };
        }
    }
}
