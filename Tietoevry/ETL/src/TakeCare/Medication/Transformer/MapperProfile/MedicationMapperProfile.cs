using AutoMapper;
using TakeCare.Migration.OpenEhr.Medication.Extraction.Model;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Model;
using TakeCare.Migration.OpenEhr.Medication.Transformer.Service;

namespace TakeCare.Migration.OpenEhr.Medication.Transformer.MapperProfile
{
    public class MedicationMapperProfile : Profile
    {
        private readonly IMedicationService _medicationService;

        public MedicationMapperProfile(IMedicationService medicationService)
        {
            _medicationService = medicationService;
            CreateMap<TakeCare.Migration.OpenEhr.Medication.Extraction.Model.Medication, TcMedicationOrder>()
               .ForMember(dest => dest.ExternalStartDate, src => src.MapFrom(src => src.ExternalISOStartDate));

            CreateMap<Prescription, TcPrescription>()
                .ForMember(dest => dest.IsReplacebleCode, src => src.MapFrom(src => _medicationService.GetEquivalenceDetails(src.IsReplaceable).Code))
                .ForMember(dest => dest.IsReplacebleValue, src => src.MapFrom(src => _medicationService.GetEquivalenceDetails(src.IsReplaceable).Display))
                .ForMember(dest => dest.IsReplacebleEquivalence, src => src.MapFrom(src => _medicationService.GetEquivalenceDetails(src.IsReplaceable).Equivalence));

            CreateMap<Drug, TcDrug>();

            CreateMap<Dosage, TcDosage>()
                .ForMember(dest => dest.StartFullDateTime, src => src.MapFrom(src => src.FullStartDate))
                .ForMember(dest => dest.DosageDrugs, src => src.Ignore());

            CreateMap<DosageDrug, TcDosageDrug>();

            CreateMap<Day, TcDay>();
        }
    }
}
