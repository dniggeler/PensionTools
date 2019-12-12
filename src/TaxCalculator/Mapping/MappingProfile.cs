using AutoMapper;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data.Abstractions.Models;


namespace TaxCalculator.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<MunicipalityEntity, MunicipalityModel>();

            this.CreateMap<CapitalBenefitTaxPerson, TaxPerson>();

            this.CreateMap<CapitalBenefitTaxPerson, FederalTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom(s => s.TaxableBenefits));

            this.CreateMap<TaxPerson, BasisTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom(s => s.TaxableIncome));

            this.CreateMap<TaxPerson, FederalTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom(s => s.TaxableIncome));

            this.CreateMap<TaxPerson, PollTaxPerson>();

            this.CreateMap<TaxPerson, ChurchTaxPerson>()
                .ForMember(d => d.ReligiousGroup, m => m.MapFrom(s => s.ReligiousGroupType))
                .ForMember(d => d.PartnerReligiousGroup, m => m.MapFrom(s => s.PartnerReligiousGroupType));
            }
    }
}