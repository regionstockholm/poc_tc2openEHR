namespace TakeCare.Foundation.OpenEhr.Models.Model
{
    public class TcPrescription
    {
        public Guid Guid { get; set; }
        public Guid ParentGuid { get; set; }
        public string TimestampSaved { get; set; }
        public string SavedByUserID { get; set; }
        public string SavedAtCareUnitID { get; set; }
        public string TreatmentReason { get; set; }
        public string TreatmentGoal { get; set; }
        public string Instruction { get; set; }
        public string ReviewDecisionByUserID { get; set; }
        public string IsReplaceable { get; set; }
        public string DilutionLiquid { get; set; }
        public string DilutionAmount { get; set; }
        public string CessationReasonID { get; set; }
        public string CessationReasonText { get; set; }
        public string IsStdSolution { get; set; }
        public string FullReviewDate { get; set; }
        public string FullFirstDoseDate { get; set; }
        public string FullLastDoseDate { get; set; }
        public string AdministrationOccasionID { get; set; }
        public string AdministrationOccasionText { get; set; }
        public string AdministrationRouteID { get; set; }
        public string AdministrationRouteText { get; set; }
        public string AdministrationTypeID { get; set; }
        public string AdministrationTypeText { get; set; }
        public string IsDispensionAllowed { get; set; }
        public string SolutionStrength { get; set; }
        public string SolutionStrengthUnitID { get; set; }
        public string SolutionStrengthUnitText { get; set; }
        public string IsReplacebleCode { get; set; }
        public string IsReplacebleValue { get; set; }
        public string IsReplacebleEquivalence { get; set; }

        public override string ToString()
        {
            var administrationOccasion = (string.IsNullOrWhiteSpace(AdministrationOccasionID) || string.IsNullOrWhiteSpace(AdministrationOccasionText)) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/startkriterium_för_order|code"": ""{AdministrationOccasionID}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/startkriterium_för_order|value"": ""{AdministrationOccasionText}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/startkriterium_för_order|terminology"": ""TC-Administration-Occasion"",";

            var cessationReason = (string.IsNullOrWhiteSpace(CessationReasonID) || string.IsNullOrWhiteSpace(CessationReasonText)) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/upphörande_skäl|code"": ""{CessationReasonID}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/upphörande_skäl|value"": ""{CessationReasonText}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/medicin_extension/upphörande_skäl|terminology"": ""TC-Meds-Cessation-Reason"",";
            
            var administrationRoute = (string.IsNullOrWhiteSpace(AdministrationRouteID) || string.IsNullOrWhiteSpace(AdministrationRouteText)) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|code"": ""{AdministrationRouteID}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|value"": ""{AdministrationRouteText}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsväg|terminology"": ""TC-Administration-Route"",";
            
            var administrationType = (string.IsNullOrWhiteSpace(AdministrationTypeID) || string.IsNullOrWhiteSpace(AdministrationTypeText)) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|code"": ""{AdministrationTypeID}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|value"": ""{AdministrationTypeText}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/administreringsmetod|terminology"": ""TC-Administration-Type"",";

            var result = $@"
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/styrka_koncentration|magnitude"": ""{SolutionStrength}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/styrka_koncentration|unit"": ""{SolutionStrengthUnitText}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/klinisk_indikation"": ""{TreatmentReason}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/behandlingsavsikt"": ""{TreatmentGoal}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/fullständig_beskrivning_av_anvisningar"": ""{Instruction}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/datum_tid_för_påbörjad_order"": ""{FullFirstDoseDate}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/datum_tid_för_avslutad_order"": ""{FullLastDoseDate}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/instruktion_för_dispensering:0"": ""{IsDispensionAllowed}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/instruktion_för_dispensering_2:0|terminology"": ""TC-DosageUnit"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsgenomgång/datum_och_tid_för_läkemedelsgenomgång"": ""{FullReviewDate}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsgenomgång/beslut_läkemedelsgenomgång_användarid"": ""{ReviewDecisionByUserID}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/anvisningar_för_substitution|code"": ""{IsReplacebleCode}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/anvisningar_för_substitution|value"": ""{IsReplacebleValue}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/anvisningar_för_substitution|terminology"": ""local"",";

            if (!string.IsNullOrWhiteSpace(DilutionLiquid))
            {
                result += @$"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedelsdetaljer_diluent/namn"": ""{DilutionLiquid}"",";
            }

            if (!string.IsNullOrWhiteSpace(DilutionAmount))
            {
                result += @$"""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedelsdetaljer_diluent/mängd|magnitude"": {DilutionAmount},
                        ""läkemedelsförskrivning/läkemedelsbeställning/order:0/läkemedelsdetaljer/läkemedelsdetaljer_diluent/mängd|unit"": ""ml"",";
            }

            if (!string.IsNullOrWhiteSpace(administrationOccasion))
            {
                result = $@"{result}
                          {administrationOccasion}";
            }
            if (!string.IsNullOrWhiteSpace(administrationType))
            {
                result = $@"{result}
                          {administrationType}";
            }
            if (!string.IsNullOrWhiteSpace(administrationRoute))
            {
                result = $@"{result}
                          {administrationRoute}";
            }
            if (!string.IsNullOrWhiteSpace(cessationReason))
            {
                result = $@"{result}
                          {cessationReason}";
            }

            return result;

        }
    }
}
