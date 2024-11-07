namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
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
            var doseform = string.IsNullOrWhiteSpace(DoseFormCode) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/form|code"": ""{DoseFormCode}"",,
                            ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/form|value"": ""{DoseForm}"",
                            ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/form|terminology"": ""TC-Drug-Form"",";

            var result = 
                $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel|code"":""{DrugCode}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel|value"":""{PreparationText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel|terminology"":""external_terminology"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|code"":""{AdministrationRouteID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|value"":""{AdministrationRouteText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|terminology"": ""TC-Administration-Route"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|code"":""{AdministrationTypeID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|value"": ""{AdministrationTypeText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|terminology"": ""TC-Administration-Type"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/instruktion_för_dispensering_2:0|code"":""{DosageUnitID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/instruktion_för_dispensering_2:0|value"":""{DosageUnitText}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|code"":""{ATCCode}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|terminology"": ""ATC"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping|match"": ""="",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|code"": ""{DrugID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|terminology"": ""NPL-Package"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping|match"": ""="",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|code"": ""{SpecialityID}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|terminology"": ""SpecialityId"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping|match"": ""="",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|code"": ""{ATCCode}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping/target|terminology"":""ATC"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsartikel/_mapping|match"":""="",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/kategori|code"":""{ProductType}"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/kategori|value"":""Ad-hoc blandning"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/kategori|terminology"": ""TC-Product-Type"",
                    ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedel_extension/är_godkänd"": {IsApproved},";

            if (!string.IsNullOrWhiteSpace(doseform))
            {
              result = $@"{result}
                          {doseform}";
            }
            return result;
            
        }
    }
}
