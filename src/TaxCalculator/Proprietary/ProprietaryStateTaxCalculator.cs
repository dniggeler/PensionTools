using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Enums;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Proprietary;

public class ProprietaryStateTaxCalculator : IStateTaxCalculator
{
    private readonly IAggregatedBasisTaxCalculator basisTaxCalculator;
    private readonly IChurchTaxCalculator churchTaxCalculator;
    private readonly IPollTaxCalculator pollTaxCalculator;
    private readonly IMapper mapper;
    private readonly Func<TaxRateDbContext> dbContext;

    public ProprietaryStateTaxCalculator(
        IAggregatedBasisTaxCalculator basisTaxCalculator,
        IChurchTaxCalculator churchTaxCalculator,
        IPollTaxCalculator pollTaxCalculator,
        IMapper mapper,
        Func<TaxRateDbContext> dbContext)
    {
        this.basisTaxCalculator = basisTaxCalculator;
        this.churchTaxCalculator = churchTaxCalculator;
        this.pollTaxCalculator = pollTaxCalculator;
        this.mapper = mapper;
        this.dbContext = dbContext;
    }

    public async Task<Either<string, StateTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        TaxPerson person)
    {
        await using var ctxt = dbContext();
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

        Option<TaxRateEntity> taxRate = ctxt.Rates.AsNoTracking()
            .FirstOrDefault(item => item.BfsId == municipalityId
                                    && item.Year == calculationYear);

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
                    from cTax in r.CantonTaxAmount
                    from mTax in r.MunicipalityTaxAmount
                    select cTax + mTax;

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
