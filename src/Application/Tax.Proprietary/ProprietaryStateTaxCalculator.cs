﻿using Application.Tax.Proprietary.Contracts;
using Application.Tax.Proprietary.Repositories;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using Domain.Models.Tax.Person;
using LanguageExt;

namespace Application.Tax.Proprietary;

public class ProprietaryStateTaxCalculator : IStateTaxCalculator
{
    private readonly IAggregatedBasisTaxCalculator basisTaxCalculator;
    private readonly IChurchTaxCalculator churchTaxCalculator;
    private readonly IPollTaxCalculator pollTaxCalculator;
    private readonly IMapper mapper;
    private readonly IStateTaxRateRepository stateTaxRateRepository;

    public ProprietaryStateTaxCalculator(
        IAggregatedBasisTaxCalculator basisTaxCalculator,
        IChurchTaxCalculator churchTaxCalculator,
        IPollTaxCalculator pollTaxCalculator,
        IMapper mapper,
        IStateTaxRateRepository stateTaxRateRepository)
    {
        this.stateTaxRateRepository = stateTaxRateRepository;
        this.basisTaxCalculator = basisTaxCalculator;
        this.churchTaxCalculator = churchTaxCalculator;
        this.pollTaxCalculator = pollTaxCalculator;
        this.mapper = mapper;
    }

    public async Task<Either<string, StateTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        TaxPerson person)
    {
        var aggregatedTaxResultTask =
            basisTaxCalculator.CalculateAsync(calculationYear, canton, person);

        var pollTaxPerson = mapper.Map<PollTaxPerson>(person);
        var pollTaxResultTask =
            pollTaxCalculator.CalculateAsync(
                calculationYear, municipalityId, canton, pollTaxPerson);

        var churchTaxPerson = mapper.Map<ChurchTaxPerson>(person);

        await Task.WhenAll(pollTaxResultTask, aggregatedTaxResultTask);

        var aggregatedTaxResult = await aggregatedTaxResultTask;
        var churchTaxResult = await aggregatedTaxResult
            .BindAsync(r => churchTaxCalculator.CalculateAsync(
                calculationYear, municipalityId, churchTaxPerson, r));

        var pollTaxResult = await pollTaxResultTask;

        Option<TaxRateEntity> taxRate = stateTaxRateRepository.TaxRates(calculationYear, municipalityId);

        var stateTaxResult = new StateTaxResult();

        return aggregatedTaxResult
            .Bind(r =>
            {
                stateTaxResult.BasisIncomeTax = r.IncomeTax;
                stateTaxResult.BasisWealthTax = r.WealthTax;

                return churchTaxResult;
            })
            .Bind(r =>
            {
                stateTaxResult.ChurchTax = r;

                return pollTaxResult;
            })
            .Bind(r =>
            {
                stateTaxResult.PollTaxAmount =
                    (from cTax in r.CantonTaxAmount
                        from mTax in r.MunicipalityTaxAmount
                        select cTax + mTax)
                    .IfNone(0);

                return taxRate.ToEither("No tax rate found");
            })
            .Map(r =>
            {
                stateTaxResult.MunicipalityRate = r.TaxRateMunicipality;
                stateTaxResult.CantonRate = r.TaxRateCanton;

                return stateTaxResult;
            });
    }
}
