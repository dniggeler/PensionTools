namespace TaxCalculator.Mapping
{
    using AutoMapper;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<CapitalBenefitTaxPerson, TaxPerson>();
            this.CreateMap<TaxPerson, BasisTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom( s=> s.TaxableIncome));
            this.CreateMap<TaxPerson, FederalTaxPerson>();
            this.CreateMap<TaxPerson, PollTaxPerson>();
            this.CreateMap<TaxPerson, ChurchTaxPerson>()
                .ForMember(d => d.ReligiousGroup, m => m.MapFrom(s => s.ReligiousGroupType))
                .ForMember(d => d.PartnerReligiousGroup, m => m.MapFrom(s => s.PartnerReligiousGroupType));
        }
    }
}