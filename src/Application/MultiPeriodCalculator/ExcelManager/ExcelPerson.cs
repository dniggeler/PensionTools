using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelManager;

public record ExcelPerson(
    int? PersonNumber,
    string Name,
    DateOnly Birthdate,
    CivilStatus CivilStatus,
    Gender Gender,
    ReligiousGroupType ReligiousGroupType,
    ReligiousGroupType? PartnerReligiousGroupType);
