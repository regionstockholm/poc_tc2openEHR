using System.ComponentModel.Design;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.Model
{
    public class TcDrug
    {
        private const string _prefix = "läkemedelsförskrivning/läkemedelsbeställning";
        public Guid ParentGuid { get; set; }
        public Guid Guid { get; set; }
        public string Row { get; set; }
        public string PreparationText { get; set; }
        public string DrugCode { get; set; }
        public string SpecialityID { get; set; }
        public string SpecialDrugCode { get; set; }
        public string DrugID { get; set; }
        public string DoseForm { get; set; }
        public string DoseFormCode { get; set; }
        public string ATCCode { get; set; }
        public string ATCName { get; set; }
        public string Strength { get; set; }
        public string StrengthUnit { get; set; }
        public string InternalArticleStrength { get; set; }
        public string UnitCode { get; set; }
        public string DosageUnitID { get; set; }
        public string DosageUnitText { get; set; }
        public string StdSolutionAmount { get; set; }
        public string ProductType { get; set; }
        public string IsApproved { get; set; }
        public override string ToString()
        {
            var doseform = string.Empty;
            if ((string.IsNullOrWhiteSpace(DoseFormCode) && string.IsNullOrWhiteSpace(DoseForm)))
            {
                doseform = string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(DoseForm))
            {
                doseform = $@"""{_prefix}/order:0/läkemedelsdetaljer/form"": ""{DoseForm}"",";
            }
            else if (!string.IsNullOrWhiteSpace(DoseForm) && !string.IsNullOrWhiteSpace(DoseFormCode))
            {
                doseform = $@"""{_prefix}/order:0/läkemedelsdetaljer/form|code"": ""{DoseFormCode}"",,
                            ""{_prefix}/order:0/läkemedelsdetaljer/form|value"": ""{DoseForm}"",
                            ""{_prefix}/order:0/läkemedelsdetaljer/form|terminology"": ""TC-Drug-Form"",";
            }

            var result =
                $@"""{_prefix}/order:0/läkemedelsartikel|code"":""{DrugCode}"",
                    ""{_prefix}/order:0/läkemedelsartikel|value"":""{PreparationText}"",
                    ""{_prefix}/order:0/läkemedelsartikel|terminology"":""TC-Drug-Code"",
                    ""{_prefix}/order:0/anvisningar_för_dispensering/instruktion_för_dispensering_2:0|code"":""{DosageUnitID}"",
                    ""{_prefix}/order:0/anvisningar_för_dispensering/instruktion_för_dispensering_2:0|value"":""{DosageUnitText}"",
                    ""{_prefix}/order:0/läkemedelsdetaljer/kategori|code"":""{ProductType}"",
                    ""{_prefix}/order:0/läkemedelsdetaljer/kategori|value"":""{(ProductTypeEnum)Convert.ToInt16(ProductType)}"",
                    ""{_prefix}/order:0/läkemedelsdetaljer/kategori|terminology"": ""TC-Product-Type"",
                    ""{_prefix}/order:0/läkemedelsdetaljer/läkemedel_extension/är_godkänd"": {IsApproved},
                    ""{_prefix}/order:0/läkemedelsdetaljer/läkemedel_extension/atc-kodnamn"": ""{ATCName}"",
                    ""{_prefix}/order:0/läkemedelsdetaljer/läkemedel_extension/styrka_beskrivning"": ""{InternalArticleStrength}"",
                    ""{_prefix}/order:0/läkemedelsdetaljer/läkemedel_extension/särskild_läkemedelskod"": ""{SpecialDrugCode}"",";

            if (!string.IsNullOrWhiteSpace(Strength))
            {
                result += $@"""{_prefix}/order:0/läkemedelsdetaljer/styrka/styrka_i_täljare|magnitude"": {Strength},
                    ""{_prefix}/order:0/läkemedelsdetaljer/styrka/styrka_i_täljare|unit"": ""{StrengthUnit}"",
                    ""{_prefix}/order:0/läkemedelsdetaljer/styrka/styrka_i_nämnare|magnitude"": 1,
                    ""{_prefix}/order:0/läkemedelsdetaljer/styrka/styrka_i_nämnare|unit"": ""1"",";
            }

            if (!string.IsNullOrWhiteSpace(StdSolutionAmount))
            {
                result+= $@"""{_prefix}/order:0/läkemedelsdetaljer/mängd|magnitude"": {StdSolutionAmount},
                            ""{_prefix}/order:0/läkemedelsdetaljer/mängd|unit"": ""ml"",";
            }
            if (!string.IsNullOrWhiteSpace(doseform))
            {
                result = $@"{result}
                          {doseform}";
            }
            var mappingOccurrance = 0;
            var atcCodeAql = string.IsNullOrWhiteSpace(ATCCode) ? string.Empty :
                            $@"""{_prefix}/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|code"":""{ATCCode}"",
                                ""{_prefix}/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|terminology"": ""ATC"",
                                ""{_prefix}/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}|match"": ""="",";

            if (!string.IsNullOrWhiteSpace(atcCodeAql))
            {
                result = $@"{result}
                          {atcCodeAql}";
            }

            var drugIdAql = string.IsNullOrWhiteSpace(DrugID) ? string.Empty :
                            $@"""{_prefix}/order:0/läkemedelsartikel/_mapping:{++mappingOccurrance}/target|code"": ""{DrugID}"",
                                ""{_prefix}/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|terminology"": ""NPL-Package"",
                                ""{_prefix}/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}|match"": ""="",";
            if (!string.IsNullOrWhiteSpace(drugIdAql))
            {
                result = $@"{result}
                          {drugIdAql}";
            }

            var specialtyIdAql = string.IsNullOrWhiteSpace(SpecialityID) ? string.Empty :
                            $@"""{_prefix}/order:0/läkemedelsartikel/_mapping:{++mappingOccurrance}/target|code"": ""{SpecialityID}"",
                                ""{_prefix}/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|terminology"": ""SpecialityId"",
                                ""{_prefix}/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}|match"": ""="",";
            if (!string.IsNullOrWhiteSpace(specialtyIdAql))
            {
                result = $@"{result}
                          {specialtyIdAql}";
            }

            return result;

        }
    }

    enum ProductTypeEnum
    {
        NoTypeSpecified = 0,
        Extemporaneous = 1,
        LicensedPreparations = 2
    }
    /*Product. For own preparations.
        0 = no type specified
        1 = extemporaneous
        2 = licensed preparations
        */
}
