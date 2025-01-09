using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.Person;

public class CalculationPerson
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Gender? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public CivilStatus CivilStatus { get; set; }
    
    public int? NumberOfChildren { get; set; }
    
    public ReligiousGroupType ReligiousGroupType { get; set; }
    
    public ReligiousGroupType? PartnerReligiousGroupType { get; set; }
}
