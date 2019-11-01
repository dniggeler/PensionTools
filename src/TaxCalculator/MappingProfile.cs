using AutoMapper;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaxPerson, BasisTaxPerson>()
                .ForMember(d =>d.TaxableAmount, m => m.MapFrom( s=> s.TaxableIncome));
            CreateMap<TaxPerson, FederalTaxPerson>();
        }
    }
}