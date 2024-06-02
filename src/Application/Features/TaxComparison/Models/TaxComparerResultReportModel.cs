using Domain.Enums;

namespace Application.Features.TaxComparison.Models
{
    public class TaxComparerResultReportModel
    {
        public int MunicipalityId { get; set; }

        public string MunicipalityName { get; set; }

        public Canton Canton { get; set; }

        public decimal TotalTaxAmount { get; set; }

        public decimal FederalTaxAmount { get; set; }

        public decimal MunicipalityTaxAmount { get; set; }

        public decimal CantonTaxAmount { get; set; }

        public decimal ChurchTaxAmount { get; set; }
    }
}
