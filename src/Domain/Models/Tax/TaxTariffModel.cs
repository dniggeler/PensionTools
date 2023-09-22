﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Tax
{
    public class TaxTariffModel
    {
        [Column("Kanton")]
        public string Canton { get; set; }

        [Column("Steuer")]
        public decimal TaxAmount { get; set; }

        [Column("Einkommen")]
        public decimal IncomeLevel { get; set; }

        [Column("Jahr")]
        public int Year { get; set; }

        [Column("Steuerinkrement")]
        public decimal TaxTariffRatePercent { get; set; }

        [Column("Einkommensinkrement")] 
        public decimal IncomeIncrement { get; set; } = 1000;

        [Column("Tariftyp")]
        public int TariffType { get; set; }

        [Column("Steuertyp")]
        public int TaxType { get; set; }
    }
}
