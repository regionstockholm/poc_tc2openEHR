using System.ComponentModel.Design;

namespace TakeCare.Foundation.OpenEhr.Models.Model
{
    public class TcDrug
    {
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
        public string AdministrationRouteID { get; set; }
        public string AdministrationRouteText { get; set; }
        public string AdministrationTypeID { get; set; }
        public string AdministrationTypeText { get; set; }
        public override string ToString()
        {
            var doseform = string.Empty;
            if ((string.IsNullOrWhiteSpace(DoseFormCode) && string.IsNullOrWhiteSpace(DoseForm)))
            {
                doseform = string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(DoseForm))
            {
                doseform = $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/form"": ""{DoseForm}"",";
            }
            else if (!string.IsNullOrWhiteSpace(DoseForm) && !string.IsNullOrWhiteSpace(DoseFormCode))
            {
                doseform = $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/form|code"": ""{DoseFormCode}"",,
                            ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/form|value"": ""{DoseForm}"",
                            ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/form|terminology"": ""TC-Drug-Form"",";
            }

            var result =
                $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel|code"":""{DrugCode}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel|value"":""{PreparationText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel|terminology"":""TC-Drug-Code"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|code"":""{AdministrationRouteID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|value"":""{AdministrationRouteText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|terminology"": ""TC-Administration-Route"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|code"":""{AdministrationTypeID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|value"": ""{AdministrationTypeText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|terminology"": ""TC-Administration-Type"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/instruktion_för_dispensering_2:0|code"":""{DosageUnitID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/instruktion_för_dispensering_2:0|value"":""{DosageUnitText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/kategori|code"":""{ProductType}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/kategori|value"":""{(ProductTypeEnum)Convert.ToInt16(ProductType)}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/kategori|terminology"": ""TC-Product-Type"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedel_extension/är_godkänd"": {IsApproved},
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedel_extension/atc-kodnamn"": ""{ATCName}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedel_extension/styrka_beskrivning"": ""{InternalArticleStrength}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedel_extension/särskild_läkemedelskod"": ""{SpecialDrugCode}"",";

            if (!string.IsNullOrWhiteSpace(Strength))
            {
                result += $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/styrka/styrka_i_täljare|magnitude"": {Strength},
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/styrka/styrka_i_täljare|unit"": ""{StrengthUnit}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/styrka/styrka_i_nämnare|magnitude"": 1,
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/styrka/styrka_i_nämnare|unit"": ""1"",";
            }

            if (!string.IsNullOrWhiteSpace(StdSolutionAmount))
            {
                result+= $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/mängd|magnitude"": {StdSolutionAmount},
                            ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/mängd|unit"": ""ml"",";
            }
            if (!string.IsNullOrWhiteSpace(doseform))
            {
                result = $@"{result}
                          {doseform}";
            }
            var mappingOccurrance = 0;
            var atcCodeAql = string.IsNullOrWhiteSpace(ATCCode) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|code"":""{ATCCode}"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|terminology"": ""ATC"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}|match"": ""="",";

            if (!string.IsNullOrWhiteSpace(atcCodeAql))
            {
                result = $@"{result}
                          {atcCodeAql}";
            }

            var drugIdAql = string.IsNullOrWhiteSpace(DrugID) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{++mappingOccurrance}/target|code"": ""{DrugID}"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|terminology"": ""NPL-Package"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}|match"": ""="",";
            if (!string.IsNullOrWhiteSpace(drugIdAql))
            {
                result = $@"{result}
                          {drugIdAql}";
            }

            var specialtyIdAql = string.IsNullOrWhiteSpace(SpecialityID) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{++mappingOccurrance}/target|code"": ""{SpecialityID}"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}/target|terminology"": ""SpecialityId"",
                                ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping:{mappingOccurrance}|match"": ""="",";
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
