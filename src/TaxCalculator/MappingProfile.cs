using AutoMapper;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace TaxCalculator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaxPerson, BasisTaxPerson>()
                .ForMember(d =>d.TaxableAmount, m => m.MapFrom( s=> s.TaxableIncome));
            CreateMap<TaxPerson, FederalTaxPerson>();
            CreateMap<TaxPerson, PollTaxPerson>();
        }
    }
}