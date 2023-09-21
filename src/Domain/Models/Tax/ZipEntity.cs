using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Tax;

public class ZipEntity
{
    [Column("BfsNumber")]
    public int BfsNumber { get; set; }

    [Column("ZipCode")]
    public string ZipCode { get; set; }

    [Column("ZipCodeAddOn")]
    public string ZipCodeAddOn { get; set; }

    [Column("LanguageCode")]
    public int LanguageCode { get; set; }

    [Column("DateOfValidity")]
    public DateTime DateOfValidity { get; set; }

    [Column("Name")]
    public string Name { get; set; }

    [Column("Canton")]
    public string Canton { get; set; }
}
