using AutoMapper;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace TaxCalculator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<TaxPerson, BasisTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom( s=> s.TaxableIncome));
            this.CreateMap<TaxPerson, FederalTaxPerson>();
            this.CreateMap<TaxPerson, PollTaxPerson>();
            this.CreateMap<TaxPerson, ChurchTaxPerson>();
        }
    }
}