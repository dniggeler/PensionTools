using System.ComponentModel.DataAnnotations;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.WebApi.Models
{
    public class MultiPeriodRequest
    {
        [MaxLength(50)]
        public string Name { get; set; }

        [Range(2018, 2099, ErrorMessage = "Valid tax years start from 2018")]
        public int StartingYear { get; set; }

        [Range(1, 100, ErrorMessage = "Number of periods to simulate")]
        public int NumberOfPeriods { get; set; }

        public MultiPeriodCalculatorPerson Person { get; set; }

        public CashFlowDefinitionHolder CashFlowDefinitionHolder { get; set; }
    }
}
