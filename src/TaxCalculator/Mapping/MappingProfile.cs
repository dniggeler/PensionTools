using System;
using System.Globalization;
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
            CreateMap<MunicipalityEntity, MunicipalityModel>()
                .ForMember(
                    d => d.DateOfMutation,
                    opt => opt.MapFrom(s => Convert(s.DateOfMutation)));

            CreateMap<CapitalBenefitTaxPerson, TaxPerson>();
            CreateMap<CapitalBenefitTaxPerson, ChurchTaxPerson>()
                .ForMember(d => d.ReligiousGroupType, m => m.MapFrom(s => s.ReligiousGroupType))
                .ForMember(d => d.PartnerReligiousGroupType, m => m.MapFrom(s => s.PartnerReligiousGroupType));

            CreateMap<CapitalBenefitTaxPerson, FederalTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom(s => s.TaxableBenefits));

            CreateMap<TaxPerson, BasisTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom(s => s.TaxableIncome));

            CreateMap<TaxPerson, FederalTaxPerson>()
                .ForMember(d => d.TaxableAmount, m => m.MapFrom(s => s.TaxableIncome));

            CreateMap<TaxPerson, PollTaxPerson>();

            CreateMap<TaxPerson, ChurchTaxPerson>()
                .ForMember(d => d.ReligiousGroupType, m => m.MapFrom(s => s.ReligiousGroupType))
                .ForMember(d => d.PartnerReligiousGroupType, m => m.MapFrom(s => s.PartnerReligiousGroupType));
        }

        private DateTime? Convert(string dateAsString)
        {
            string dateTimePattern = @"dd.MM.yyyy";

            if (!string.IsNullOrEmpty(dateAsString))
            {
                return DateTime.ParseExact(dateAsString, dateTimePattern, CultureInfo.CurrentCulture);
            }

            return null;
        }
    }
}