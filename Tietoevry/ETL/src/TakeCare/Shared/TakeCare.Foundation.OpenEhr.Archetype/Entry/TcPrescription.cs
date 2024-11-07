namespace TakeCare.Foundation.OpenEhr.Archetype.Entry
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
        public string IsDispensionAllowed { get; set; }
        public string SolutionStrength { get; set; }
        public string SolutionStrengthUnitID { get; set; }
        public string SolutionStrengthUnitText { get; set; }
        public string IsReplacebleCode { get; set; }
        public string IsReplacebleValue { get; set; }
        public string IsReplacebleEquivalence { get; set; }

        public override string ToString()
        {
            var administrationOccasion = string.IsNullOrWhiteSpace(AdministrationOccasionID) ? string.Empty :
                            $@"""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/startkriterium_för_order|code"": ""{AdministrationOccasionID}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/startkriterium_för_order|value"": ""{AdministrationOccasionText}"",
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/orderdetaljer/startkriterium_för_order|terminology"": ""TC-Administration-Occasion"",";

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
                         ""läkemedelsförskrivning/läkemedelsbeställning/order:0/anvisningar_för_dispensering/anvisningar_för_substitution|terminology"": ""local"",
                         ";

            if (!string.IsNullOrWhiteSpace(administrationOccasion))
            {
                result = $@"{result}
                          {administrationOccasion}";
            }
            return result;

        }
    }
}
